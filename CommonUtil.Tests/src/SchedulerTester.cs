using System;
using System.Threading.Tasks;
using CommonUtil;
using Xunit;

namespace CommonUtil.Tests {

    public class SchedulerTester {
        [Fact]
        public void TestRunQueueTaskSequence() {
            Scheduler scheduler = new Scheduler();
            int val = 0;
            scheduler.Schedule(0, () => {
                if (val == 0)
                    ++val;
            });
            TestUtil.Pause(10);
            scheduler.Schedule(0, () => {
                if (val == 1)
                    ++val;
            });
            TestUtil.Pause(10);
            scheduler.Schedule(0, () => {
                if (val == 2)
                    ++val;
            });
            TestUtil.Pause(10);
            scheduler.WaitCompletion(TimeSpan.FromSeconds(1));
            Assert.Equal(3, val);
        }

        [Fact]
        public void TestReturnExecutedTask() {
            Scheduler scheduler = new Scheduler();
            int val = 0;
            scheduler.Schedule(0, () => {
                if (val == 0)
                    ++val;
            });
            TestUtil.Pause(10);
            scheduler.Schedule(0, () => {
                if (val == 1)
                    ++val;
            });
            TestUtil.Pause(10);
            var lastTask = scheduler.Schedule(0, () => {
                if (val == 2)
                    ++val;
            });
            lastTask.Wait(TimeSpan.FromSeconds(1));
            Assert.Equal(3, val);
        }

        [Fact]
        public void TestGeneralTaskSequence() {
            Scheduler scheduler = new Scheduler();
            int val = 0;
            scheduler.ScheduleGeneralTask(() => {
                if (val == 0)
                    ++val;
            }, TimeSpan.Zero);
            TestUtil.Pause(10);
            scheduler.ScheduleGeneralTask(() => {
                if (val == 1)
                    ++val;
            }, TimeSpan.Zero);
            TestUtil.Pause(10);
            scheduler.ScheduleGeneralTask(() => {
                if (val == 2)
                    ++val;
            }, TimeSpan.Zero);
            TestUtil.Pause(10);
            scheduler.WaitCompletion(TimeSpan.FromSeconds(1));
            Assert.Equal(3, val);
        }

        [Fact]
        public void TestSimultaneousScheduling() {
            var scheduler = new Scheduler();
            int val = 0;
            int iterations = 1000;
            for (int i = 0; i < iterations; ++i) {
                Task.Run(() => scheduler.Schedule(0, () => ++val));
            }
            TestUtil.Pause(1000);
            scheduler.WaitCompletion(TimeSpan.FromSeconds(10));
            Assert.Equal(iterations, val);
        }

        [Fact]
        public void TestParallelQueuesScheduling() {
            var scheduler = new Scheduler();
            int val = 0;
            int iterations = 500;
            for (int i = 0; i < iterations; ++i) {
                Task.Run(() => scheduler.Schedule(0, () => ++val));
                Task.Run(() =>
                    scheduler.Schedule(1, () => {
                        while (val < iterations)
                            TestUtil.Pause(10);
                        ++val;
                    })
                );
            }
            TestUtil.Pause(1000);
            scheduler.WaitCompletion(TimeSpan.FromSeconds(10));
            Assert.Equal(iterations * 2, val);
        }

        [Fact]
        public void TestQueueAndGeneralTaskSequence() {
            var scheduler = new Scheduler();
            int val = 0;
            int iterations = 10;
            for (int i = 0; i < iterations; ++i) {
                scheduler.Schedule(0, () => ++val);
                scheduler.Schedule(1, () => {
                    while (val < iterations)
                        TestUtil.Pause(10);
                    ++val;
                });
            }
            TestUtil.Pause(10);
            scheduler.ScheduleGeneralTask(() => {
                while (val < iterations * 2)
                    TestUtil.Pause(10);
                ++val;
            }, TimeSpan.Zero);
            TestUtil.Pause(10);
            scheduler.Schedule(1, () => {
                while (val < iterations * 2 + 1)
                    TestUtil.Pause(10);
                ++val;
            });
            TestUtil.Pause(100);
            scheduler.WaitCompletion(TimeSpan.FromSeconds(10));
            Assert.Equal(iterations * 2 + 2, val);
        }

        [Fact]
        public void TestDelayedTasks() {
            Scheduler scheduler = new Scheduler();
            int val = 0;
            scheduler.ScheduleGeneralTask(() => {
                if (val == 1)
                    val = 2;
            }, TimeSpan.FromMilliseconds(400));
            scheduler.ScheduleGeneralTask(() => {
                if (val == 0)
                    val = 1;
            }, TimeSpan.FromMilliseconds(200));
            TestUtil.Pause(600);
            Assert.Equal(2, val);
        }

        [Fact]
        public void TestCancelledTasks() {
            Scheduler scheduler = new Scheduler();
            int val = 0;
            scheduler.ScheduleGeneralTask(() => {
                if (val == 0)
                    val = 1;
            }, TimeSpan.FromMilliseconds(200));
            scheduler.ScheduleGeneralTask(() => {
                if (val == 1)
                    val = 2;
            }, TimeSpan.FromMilliseconds(400));
            scheduler.Cancel();
            TestUtil.Pause(TimeSpan.FromMilliseconds(500));
            Assert.Equal(0, val);
        }

        [Fact]
        public void WaitThrowsException() {
            Scheduler scheduler = new Scheduler();
            scheduler.ScheduleGeneralTask(() => {
                throw new ArgumentException("Test");
            }, TimeSpan.FromMilliseconds(10));
            TestUtil.Pause(200);
            Assert.Throws<ArgumentException>(() => scheduler.WaitCompletion(TimeSpan.FromSeconds(1)));
        }
    }
}
