using System;

namespace CommonUtil {
    public interface IScheduler {

        void WaitCompletion(TimeSpan timeout);

        void Schedule(int runQueueId, Action action);

        void Schedule(int runQueueId, Action action, TimeSpan delay);

        void ScheduleGeneralTask(Action action, TimeSpan delay);

        void Cancel();
    }
}
