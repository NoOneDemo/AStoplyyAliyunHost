using AliyunOpenApiJob.AliyunOpen;
using AliyunOpenApiJob.AliyunResponseModel;
using Common.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace AliyunOpenApiJob.Job
{
    [DisallowConcurrentExecution]
    public class UpdateDomainJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(UpdateDomainJob));

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                Logger.Info("启动任务Test");
                try
                {
                    var curIp = AliyunOpenHelper.GetCurIp();
                    var domainResponse = AliyunOpenHelper.GetDomainList("codersun.cn");
                    var domainResponse2 = AliyunOpenHelper.GetDomainList("codersun.com");
                    var records = JsonConvert.DeserializeObject<Record[]>(domainResponse?.SelectToken("DomainRecords")?.First().First().ToString() ?? "")
                        .Concat(JsonConvert.DeserializeObject<Record[]>(domainResponse2?.SelectToken("DomainRecords")?.First().First().ToString() ?? ""));
                    if (records == null)
                    {
                        Logger.Warn($"更新ip：result=null :{JsonConvert.SerializeObject(domainResponse)}");
                        return;
                    }
                    var check = records?.Where(p => p.Value != curIp)?.ToList() ?? new List<Record>();
                    if (check.Any())
                    {
                        check.Select(p => new Dictionary<string, string>
                            {
                                { "RecordId", p.RecordId },
                                { "RR", p.RR },
                                { "Type", p.Type },
                                { "Value", curIp },
                                { "TTL", p.TTL?.ToString() ?? "600" },
                                //select.Add("Priority", "test");//记录类型为MX记录时填写
                                { "Line", p.Line }
                            }).ToList().ForEach(update =>
                        {
                            var result = AliyunOpenHelper.UpdateDomain(update);
                            if (result == null)
                            {
                                Logger.Error($"db error result is nul=> p=>{JsonConvert.SerializeObject(update)}");
                                return;
                            }
                            var requestid = JsonConvert.DeserializeObject<JObject>(result).Value<string>("RequestId");
                            var RecordId = JsonConvert.DeserializeObject<JObject>(result).Value<string>("RecordId");
                            if (string.IsNullOrEmpty(RecordId))
                                Logger.Warn($"更新ip失败：result:{result};{update["Value"]}=>{curIp}");
                            else
                                Logger.Warn($"更新ip：result:{result};{update["Value"]}=>{curIp}");
                        });
                    }
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
