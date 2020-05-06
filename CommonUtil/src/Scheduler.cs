using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CommonUtil {
    public enum SchedulerContinuation {
        ContinueOnCancelled,
        NotContinueOnCancelled
    }
    public class Scheduler : IScheduler {
        private CancellationTokenSource cancelSource = new CancellationTokenSource();
        private CancellationToken CancelToken { get { return cancelSource.Token; } }
        private ConcurrentDictionary<long, Task> runningQueueTasks = new ConcurrentDictionary<long, Task>();
        private TaskScheduler taskScheduler = TaskScheduler.Default;
        private volatile Task runningGeneralTask = Task.CompletedTask;
        private object lockObj = new object();
        private TaskContinuationOptions continuationOptions;

        public Scheduler(SchedulerContinuation schedulerContinuation = SchedulerContinuation.NotContinueOnCancelled) {
            continuationOptions = schedulerContinuation switch {
                SchedulerContinuation.ContinueOnCancelled => TaskContinuationOptions.None,
                _ => TaskContinuationOptions.OnlyOnRanToCompletion
            };
        }

        private Task GetRunningTask(long runQueueId) {
            if (!runningQueueTasks.ContainsKey(runQueueId))
                runningQueueTasks[runQueueId] = Task.CompletedTask;
            return runningQueueTasks[runQueueId];
        }

        private Task ContinueWith(long runQueueId, Action action) {
            lock(lockObj) {
                Task newTask = GetRunningTask(runQueueId)
                    .ContinueWith((t) => action(), CancelToken,
                    continuationOptions, taskScheduler);
                runningQueueTasks[runQueueId] = newTask;
                return newTask;
            }
        }

        private void WaitForAllRunningTasks() {
            Task.WaitAll(runningQueueTasks.Values.ToArray(), CancelToken);
        }

        private Action<Task> CreateGeneralTaskAction(Action taskPayload) {
            return (t) => {
                lock(lockObj) {
                    WaitForAllRunningTasks();
                    taskPayload();
                }
            };
        }

        private void ContinueWithGeneralTask(Action taskPayload) {
            lock(lockObj) {
                Task newGeneralTask = runningGeneralTask.ContinueWith(CreateGeneralTaskAction(taskPayload),
                    CancelToken, continuationOptions, taskScheduler);
                runningGeneralTask = newGeneralTask;
            }
        }

        public void WaitCompletion(TimeSpan timeout) {
            try {
                List<Task> allTasks = new List<Task>(runningQueueTasks.Values);
                allTasks.Add(runningGeneralTask);
                Task.WaitAll(allTasks.ToArray(), timeout);
            } catch (AggregateException ex) {
                ThreadUtil.ThrowInnerException(ex);
            }
        }

        public Task Schedule(long runQueueId, Action action) {
            return ContinueWith(runQueueId, action);
        }

        public void Schedule(long runQueueId, Action action, TimeSpan delay) {
            Task.Delay((int) delay.TotalMilliseconds, CancelToken)
                .ContinueWith((t) => Schedule(runQueueId, action), CancelToken,
                    continuationOptions, taskScheduler);
        }

        public void ScheduleGeneralTask(Action action, TimeSpan delay) {
            Task.Factory.StartNew(() => {
                Task.Delay((int) delay.TotalMilliseconds, CancelToken).Wait();
                if (!CancelToken.IsCancellationRequested)
                    ContinueWithGeneralTask(action);
            }, CancelToken);
        }

        public void Cancel() {
            cancelSource.Cancel();
        }
    }
}
