using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Office.DocumentManagement.DocumentSets;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using OrderingInvoicingApp.Common;

namespace OrderingInvoicingApp.Features.OrderingInfrastructure
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("ce73b2dd-0d37-47fa-b72e-d533f4161868")]
    public class OrderingInfrastructureEventReceiver : SPFeatureReceiver
    {
        private const string Orders = "Lists/Orders";

        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPWeb web = properties.Feature.Parent as SPWeb;

            if (web != null)
            {
                try
                {
                    AddFieldsCtToLists(web);
                    Upgradeto112(web);
                }
                catch (Exception ex)
                {
                    Logger.WriteLog(Logger.Category.Information, this.GetType().Name, "Error while activating feature OrderingInfrastructure");
                    throw;

                }
            }
        }

        /// <summary>
        /// Add fields, content types end event receivers to list
        /// </summary>
        /// <param name="web"></param>
        private void AddFieldsCtToLists(SPWeb web)
        {
            SPContentType invoice = web.Site.RootWeb.ContentTypes[new SPContentTypeId("0x0101006667822C2C904046B11878F79EFAF7A6")];
            invoice.JSLink = "~sitecollection/SiteAssets/Scripts/jquery-3.1.0.min.js | ~sitecollection/SiteAssets/Scripts/AddMonth.js";
            invoice.Update(true);

            SPContentType ltInvoice = web.Site.RootWeb.ContentTypes[new SPContentTypeId("0x0101006667822C2C904046B11878F79EFAF7A60035D6DBDCBCBB47D8B3D9F882A2652E25")];
            ltInvoice.JSLink = "~sitecollection/SiteAssets/Scripts/jquery-3.1.0.min.js | ~sitecollection/SiteAssets/Scripts/AddMonth.js";
            ltInvoice.Update(true);

            SPContentType orderContentType = web.Site.RootWeb.ContentTypes[new SPContentTypeId("0x0120D52000B7FF4D802E3E4631A4AEDDA271D87E78")];
            List<string> colnames = new List<string>();
            colnames.Add("PredicaNotStandardFee");
            colnames.Add("FileLeafRef");
            colnames.Add("DocumentSetDescription");

            for (int i = 0; i < orderContentType.FieldLinks.Count; i++)
            {
                if (colnames.Contains(orderContentType.FieldLinks[i].Name))
                {
                   continue;
                }

                colnames.Add(orderContentType.FieldLinks[i].Name);
            }

            orderContentType.FieldLinks.Reorder(colnames.ToArray());
            orderContentType.FieldLinks[SPBuiltInFieldId.FileLeafRef].DisplayName = "$Resources:PredicaOrders,PredicaColOrderName";
            orderContentType.FieldLinks[Guid.Parse("{1fc87c65-f371-46d3-bb42-6174eeaeea6e}")].ReadOnly = true;

            DocumentSetTemplate docsetTemplate = DocumentSetTemplate.GetDocumentSetTemplate(orderContentType);
            // Setting the content types
            docsetTemplate.AllowedContentTypes.Remove(web.ContentTypes["Document"].Id);
            docsetTemplate.AllowedContentTypes.Add(invoice.Id);
            docsetTemplate.AllowedContentTypes.Add(ltInvoice.Id);
            docsetTemplate.Update(true);

            orderContentType.Update(true);

            string ordersUrl = SPUrlUtility.CombineUrl(web.ServerRelativeUrl.TrimEnd('/'), Orders);
            SPList ordersList = web.GetList(ordersUrl);
            ordersList.ContentTypesEnabled = true;
            ordersList.ContentTypes.Add(orderContentType);
            ordersList.ContentTypes.Add(invoice);
            ordersList.ContentTypes.Add(ltInvoice);
            ordersList.Update();
          
        }

        // Uncomment the method below to handle the event raised before a feature is deactivated.

        //public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised after a feature has been installed.

        //public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised before a feature is uninstalled.

        //public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        //{
        //}

        // Uncomment the method below to handle the event raised when a feature is upgrading.

        public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        {
            try
            {

                SPWeb web = properties.Feature.Parent as SPWeb;

                switch (upgradeActionName)
                {

                    case "UpgradeToV1.2":
                        Upgradeto112(web);
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Logger.Category.Unexpected, this.GetType().Name, string.Format("Error wgile activating Feature:{0}", ex.Message));
                throw;
            }
        }

        private void Upgradeto112(SPWeb web)
        {
            Logger.WriteLog(Logger.Category.Information, this.GetType().Name, "Upgradeto112");
            SPField owner = web.Fields.GetFieldByInternalName("PredicaInvoiceOwner");

            SPContentType invoice = web.Site.RootWeb.ContentTypes[new SPContentTypeId("0x0101006667822C2C904046B11878F79EFAF7A6")];
            Logger.WriteLog(Logger.Category.Information, this.GetType().Name, string.Format("add filed:{0} to ct:{1}", owner.InternalName, invoice.Name));
            Helper.AddFieldToContentType(web, invoice, owner, true, false, "$Resources:PredicaOrders,PredicaColInvoiceOwner");

            SPContentType ltInvoice = web.Site.RootWeb.ContentTypes[new SPContentTypeId("0x0101006667822C2C904046B11878F79EFAF7A60035D6DBDCBCBB47D8B3D9F882A2652E25")];
            Logger.WriteLog(Logger.Category.Information, this.GetType().Name, string.Format("add filed:{0} to ct:{1}", owner.InternalName, ltInvoice.Name));
            Helper.AddFieldToContentType(web, ltInvoice, owner, true, false, "$Resources:PredicaOrders,PredicaColInvoiceOwner");
        }
    }
}
