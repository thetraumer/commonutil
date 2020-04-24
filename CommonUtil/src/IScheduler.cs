using System;

namespace CommonUtil {
    public interface IScheduler {

        void WaitCompletion(TimeSpan timeout);

        void Schedule(long runQueueId, Action action);

        void Schedule(long runQueueId, Action action, TimeSpan delay);

        void ScheduleGeneralTask(Action action, TimeSpan delay);

        void Cancel();
    }
}
