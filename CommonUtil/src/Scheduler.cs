using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CommonUtil {
    public class Scheduler : IScheduler {
        private CancellationTokenSource cancelSource = new CancellationTokenSource();
        private CancellationToken CancelToken { get { return cancelSource.Token; } }
        private ConcurrentDictionary<long, Task> runningQueueTasks = new ConcurrentDictionary<long, Task>();
        private TaskScheduler taskScheduler = TaskScheduler.Default;
        private volatile Task runningGeneralTask = Task.CompletedTask;
        private object lockObj = new object();

        private Task GetRunningTask(long runQueueId) {
            if (!runningQueueTasks.ContainsKey(runQueueId))
                runningQueueTasks[runQueueId] = Task.CompletedTask;
            return runningQueueTasks[runQueueId];
        }

        private void ContinueWith(long runQueueId, Action action) {
            lock(lockObj) {
                Task newTask = GetRunningTask(runQueueId).ContinueWith((t) => action(), CancelToken,
                    TaskContinuationOptions.OnlyOnRanToCompletion, taskScheduler);
                runningQueueTasks[runQueueId] = newTask;
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
                    CancelToken, TaskContinuationOptions.OnlyOnRanToCompletion, taskScheduler);
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

        public void Schedule(long runQueueId, Action action) {
            Schedule(runQueueId, action, TimeSpan.Zero);
        }

        public void Schedule(long runQueueId, Action action, TimeSpan delay) {
            if (delay.Ticks == TimeSpan.Zero.Ticks)
                ContinueWith(runQueueId, action);
            else
                Task.Factory.StartNew(() => {
                    Task.Delay((int) delay.TotalMilliseconds, CancelToken).Wait();
                    if (!CancelToken.IsCancellationRequested)
                        ContinueWith(runQueueId, action);
                }, CancelToken);
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
