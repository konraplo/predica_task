using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
                AddFieldsCtToLists(web);
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

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}
    }
}
