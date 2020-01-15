using System.Configuration;
using Common.Logging;
using Quartz;
using Quartz.Impl;
using System.ServiceProcess;
using System;
using System.Runtime.Remoting;
using Topshelf;
using Microsoft.Owin.Hosting;
using CrystalQuartz.Owin;
using CrystalQuartz.Application;

namespace JobSchedulerService
{
    public partial class QuartzService : ServiceBase
    {
        public static ILog Logger { get; set; }
        private IDisposable _webApp;

        private IScheduler _scheduler;

        public QuartzService()
        {
            InitializeComponent();
            Logger = LogManager.GetLogger(this.GetType());
        }

        protected override void OnStart(string[] args)
        {
            _scheduler = new StdSchedulerFactory().GetScheduler();
            var uri = "http://*:5555";
            var index = uri.IndexOf('/', 10);
            //_webApp = WebApp.Start(index > 0 ? uri.Substring(0, index) : uri, app =>
            //{
            //    app.UseCrystalQuartz(_scheduler, new CrystalQuartzOptions
            //    {
            //        Path = index > 0 ? uri.Substring(index) : ""
            //    });
            //});

            _scheduler.Start();
        }

        protected override void OnStop()
        {
            if (_webApp != null)
            {
                _webApp.Dispose();
                _webApp = null;
            }

            if (_scheduler != null && _scheduler.IsStarted && !_scheduler.IsShutdown)
                _scheduler.Shutdown(false);
        }

        protected override void OnPause()
        {
            _scheduler.PauseAll();
        }

        protected override void OnContinue()
        {
            _scheduler.ResumeAll();
        }

        //public bool Start(HostControl hostControl)
        //{
        //    _scheduler.Start();
        //    return true;
        //}
        //public bool Stop(HostControl hostControl)
        //{
        //    _scheduler.Shutdown(false);
        //    return true;
        //}

        //public bool Pause(HostControl hostControl)
        //{
        //    _scheduler.PauseAll();
        //    return true;
        //}

        //public bool Continue(HostControl hostControl)
        //{
        //    _scheduler.ResumeAll();
        //    return true;
        //}
    }
    partial class QuartzService
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            this.ServiceName = "QuartzService";
        }

        #endregion
    }
}
