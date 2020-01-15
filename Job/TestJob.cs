using System;
using System.Data.SqlClient;
using Common.Logging;
using Quartz;

namespace JobSchedulerService.Job
{
    /// <summary>测试用Job huhengxing 2016-4-21 </summary>
    [DisallowConcurrentExecution]
    public class TestJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(TestJob));
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                Logger.Info("启动任务Test");
                try
                {

                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                }

                Logger.Info("结束任务Test");

            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
        }

    }
}
