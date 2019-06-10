using Microsoft.Office.DocumentManagement.DocumentSets;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using OrderingInvoicingApp.Common;
using System;
using System.Collections.Generic;
using System.Linq;

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
                            query.ViewAttributes = "Scope=\"Recursive\"";
                            
                            SPListItemCollection orders = list.GetItems(query);
                            List<SPListItem> dsToUpdate = new List<SPListItem>();

                            foreach (SPListItem listItem in orders)
                            {
                                SPListItem parentFolderItem = listItem.File.ParentFolder.Item;
                                if (parentFolderItem == null)
                                {
                                    continue;
                                }

                                if (parentFolderItem.ContentTypeId.IsChildOf(new SPContentTypeId("0x0120D52000B7FF4D802E3E4631A4AEDDA271D87E78")))
                                {
                                    if (dsToUpdate.FirstOrDefault(x => x.ID.Equals(parentFolderItem.ID))== null)
                                    {
                                        //add dataset to update
                                        dsToUpdate.Add(parentFolderItem);
                                    }
                                }
                            }

                            foreach (SPListItem listItem in dsToUpdate)
                            {
                                listItem[Guid.Parse("1fc87c65-f371-46d3-bb42-6174eeaeea6e")] = 1;
                                listItem.Update();
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
