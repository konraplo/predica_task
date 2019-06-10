using Microsoft.SharePoint;
using System.Collections.Generic;
using System.Text;

namespace OrderingInvoicingApp.Common
{
    /// <summary>
    /// Helpermethods with solutionwide accessible methods and functions.
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// batch comand to delete items
        /// </summary>
        private const string BATCH_DELETE_ITEM_CMD = "<Method><SetList Scope=\"Request\">{0}</SetList><SetVar Name=\"ID\">{1}</SetVar><SetVar Name=\"Cmd\">Delete</SetVar></Method>";

        /// <summary>
        /// batch comand to update items
        /// </summary>
        public const string BATCH_UPDATE_ITEM_CMD = "<Method ID=\"{0}\">" +
                    "<SetList>{1}</SetList>" +
                    "<SetVar Name=\"Cmd\">Save</SetVar>" +
                    "<SetVar Name=\"ID\">{2}</SetVar>" +
                    "{3}" +
                    "</Method>";
        /// <summary>
        /// batch row used to update items
        /// </summary>
        public const string BATCH_ADD_ITEM_CMD = "<Method ID=\"{0}\">" +
                   "<SetList>{1}</SetList>" +
                   "<SetVar Name=\"Cmd\">Save</SetVar>" +
                   "<SetVar Name=\"ID\">New</SetVar>" +
                   "{2}" +
                   "</Method>";

        /// <summary>
        /// batch row used to set values for item in batch commands
        /// </summary>
        public const string BATCH_ITEM_SET_VAR = "<SetVar Name=\"urn:schemas-microsoft-com:office:office#{0}\">{1}</SetVar>";

        /// <summary>
        /// Update list items
        /// </summary>
        /// <param name="web"></param>
        /// <param name="formatedUpdateBatchCommands">formated batch update comamnd - use BuildBatchUpdateCommand method</param>
        /// <returns></returns>
        public static string BatchUpdateListItems(SPWeb web, List<string> formatedUpdateBatchCommands)
        {
            StringBuilder methodBuilder = new StringBuilder();

            string batch = string.Empty;
            string batchFormat = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><ows:Batch OnError=\"Return\">{0}</ows:Batch>";

            foreach (string item in formatedUpdateBatchCommands)
            {
                methodBuilder.Append(item);
            }

            // put the pieces together.
            batch = string.Format(batchFormat, methodBuilder);

            // process batch commands.
            string batchReturn = web.ProcessBatchData(batch);

            return batchReturn;
        }

        /// <summary>
        /// Build batch update command for specified item and field
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="itemId"></param>
        /// <param name="fieldInternalName"></param>
        /// <param name="fieldValue"></param>
        /// <returns></returns>
        public static string BuildBatchUpdateCommand(string listId, int itemId, string fieldInternalName, string fieldValue)
        {
            string value = string.Format(BATCH_ITEM_SET_VAR, fieldInternalName, fieldValue);
            return string.Format(BATCH_UPDATE_ITEM_CMD, itemId, listId, itemId, value);
        }

        /// <summary>
        /// Add specified field to content type (or update existing with specified props)
        /// </summary>
        /// <param name="pWeb"></param>
        /// <param name="pContentType"> </param>
        /// <param name="pField"></param>
        /// <param name="pRequired">should this field be required or not</param>
        /// <param name="pReadOnly"> </param>
        public static void AddFieldToContentType(SPWeb pWeb, SPContentType pContentType, SPField pField, bool pRequired, bool pReadOnly, string pDisplayName)
        {
            using (SPSite site = new SPSite(pWeb.Site.ID))
            {
                using (SPWeb rootWeb = site.OpenWeb(site.RootWeb.ID))
                {
                    rootWeb.AllowUnsafeUpdates = true;
                    SPFieldLink fieldLink;
                    if (!pContentType.Fields.Contains(pField.Id))
                    {

                        fieldLink = new SPFieldLink(pField);

                        pContentType.FieldLinks.Add(fieldLink);

                    }
                    else
                    {
                        fieldLink = pContentType.FieldLinks[pField.Id];
                    }

                    fieldLink.Required = pRequired;
                    fieldLink.DisplayName = string.IsNullOrEmpty(pDisplayName) ? pField.Title : pDisplayName;

                    if (pRequired)
                    {
                        fieldLink.ReadOnly = false;
                    }
                    else
                    {
                        fieldLink.ReadOnly = pReadOnly;

                    }

                    SPContentType checkContentType = rootWeb.AvailableContentTypes[pContentType.Id];
                    pContentType.Update(null != checkContentType);
                    rootWeb.AllowUnsafeUpdates = false;
                }
            }
        }

    }
}
