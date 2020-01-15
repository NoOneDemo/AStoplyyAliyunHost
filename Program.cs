using System;
using System.Drawing;
using System.IO;
using System.ServiceProcess;
using System.Windows.Forms;
using Topshelf;

namespace JobSchedulerService
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            ServiceBase.Run(new ServiceBase[]
            {
                new QuartzService()
            });

            //log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(Environment.CurrentDirectory + "log4net.config"));
            //HostFactory.Run(x =>
            //{
            //    x.Service<QuartzService>();

            //    x.SetDescription("QuartzDemo服务描述");
            //    x.SetDisplayName("QuartzDemo服务显示名称");
            //    x.SetServiceName("QuartzDemo服务名称");

            //    x.EnablePauseAndContinue();
            //});
        }
    }
}

