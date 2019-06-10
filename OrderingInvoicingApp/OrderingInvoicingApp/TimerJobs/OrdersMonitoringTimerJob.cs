namespace OrderingInvoicingApp.TimerJobs
{
    using Microsoft.SharePoint.Administration;
    using OrderingInvoicingApp.Common;
    using System;

    /// <summary>
    /// This job definition represents the Timer job responsible for the Change notifications
    /// </summary>
    public class OrdersMonitoringTimerJob : SPJobDefinition
    {
        public const string TimerJobName = "Predica orders monitoring Timer job";
        /// <summary>
        /// Empty CTOR
        /// </summary>
        public OrdersMonitoringTimerJob() : base()
        {

        }

        /// <summary>
        /// Unused CTOR
        /// </summary>
        /// <param name="jobName">Name of the job</param>
        /// <param name="service">The Service</param>
        /// <param name="server">The server</param>
        /// <param name="targetType">SPJobLockType</param>
        public OrdersMonitoringTimerJob(string jobName, SPService service, SPServer server, SPJobLockType targetType) : base(jobName, service, server, targetType)
        {

        }

        /// <summary>
        /// Unused CTOR
        /// </summary>
        /// <param name="jobName">Name of the job</param>
        /// <param name="webApplication">WebApplication object</param>
        public OrdersMonitoringTimerJob(string jobName, SPWebApplication webApplication) : base(jobName, webApplication, null, SPJobLockType.Job)
        {
            this.Title = TimerJobName;
        }

        /// <summary>
        /// Execute-Method.
        /// </summary>
        /// <param name="targetInstanceId">ID of the job instance</param>
        public override void Execute(Guid targetInstanceId)
        {
            Logger.WriteLog(Logger.Category.Information, this.GetType().Name, "Entered Executemethod.");
            OrdersMonitoringTimerJobExecuter executer = new OrdersMonitoringTimerJobExecuter();
            executer.Execute(this);
        }
    }
}
