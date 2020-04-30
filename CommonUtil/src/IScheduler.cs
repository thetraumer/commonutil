using System;
using System.Threading.Tasks;

namespace CommonUtil {
    public interface IScheduler {

        void WaitCompletion(TimeSpan timeout);

        Task Schedule(long runQueueId, Action action);

        void Schedule(long runQueueId, Action action, TimeSpan delay);

        void ScheduleGeneralTask(Action action, TimeSpan delay);

        void Cancel();
    }
}
