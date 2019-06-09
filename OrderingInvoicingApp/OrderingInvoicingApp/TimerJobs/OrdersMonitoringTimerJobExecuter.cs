using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using OrderingInvoicingApp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderingInvoicingApp.TimerJobs
{
    class OrdersMonitoringTimerJobExecuter
    {
        private const string Orders = "Lists/Orders";

        private const string QueryNotPaidOrders =
                                   @"<Where>
                                    <And>
                                      <And>
                                        <Eq>
                                          <FieldRef Name='PredicaInvoicePaid' />
                                          <Value Type='Boolean'>0</Value>
                                        </Eq>
                                        <Lt>
                                          <FieldRef Name='PredicaInvoiceLTPaymentDate' />
                                          <Value Type='DateTime'>
                                            <Today />
                                          </Value>
                                        </Lt>
                                      </And>
                                      <Eq>
                                        <FieldRef Name='ContentType' />
                                        <Value Type='Computed'>Predica invoice</Value>
                                      </Eq>
                                    </And></Where>";


        internal void Execute(OrdersMonitoringTimerJob notificationTimerJob)
        {
            SPWebApplication webApplication = notificationTimerJob.WebApplication;
            string siteUrl = FindOrderingSiteUrl(webApplication);
            if (!string.IsNullOrEmpty(siteUrl))
            {
                try
                {
                    using (SPSite site = new SPSite(siteUrl))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            SPList list = web.GetList(SPUtility.ConcatUrls(web.Url, Orders));
                            SPQuery query = new SPQuery();

                            // not paid orders
                            query.Query = QueryNotPaidOrders;
                            SPListItemCollection orders = list.GetItems(query);
                            foreach (SPListItem taskItem in orders)
                            {
                            }
                        }
                    }

                }
                catch (Exception exception)
                {
                    Logger.WriteLog(Logger.Category.Unexpected, typeof(OrdersMonitoringTimerJobExecuter).FullName, string.Format("Error while checking orders:{0}", exception.Message));
                }
            }
        }

        /// Iterates through all site collections od the WebApplication and returns the url of the Site, where the "OrderingInfrastructure"-Feature is activated
        /// </summary>
        /// <param name="webApp">SPWebApplication to search for the SiteCollection</param>
        /// <returns>string of the business development site. Returns string.Empty if not found</returns>
        private string FindOrderingSiteUrl(SPWebApplication webApp)
        {
            if (webApp == null) throw new ArgumentNullException("WebApplication must be not NULL!");

            Guid infrastructureFeatureGuid = new Guid("b98f22e7-7143-4904-84ba-8f193d469b0a");

            try
            {
                foreach (SPSite site in webApp.Sites)
                {
                    bool featureFound = (site.RootWeb.Features[infrastructureFeatureGuid] != null);
                    if (featureFound) return site.RootWeb.Url;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Logger.Category.Unexpected, typeof(OrdersMonitoringTimerJobExecuter).Name, string.Format("error:{0}", ex.Message));
            }

            return string.Empty;
        }
    }
}
