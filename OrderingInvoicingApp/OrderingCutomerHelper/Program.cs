using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderingCutomerHelper
{
    class Program
    {
        private const string myInvoices = @"<View Scope='Recursive'>
    <Query>
      <OrderBy>
        <FieldRef Name='FileLeafRef' />
      </OrderBy>
      <Where>
        <And>
          <Eq>
            <FieldRef Name='PredicaInvoiceOwner' />
            <Value Type='Integer'>
              <UserID Type='Integer' />
            </Value>
          </Eq>
          <Eq>
            <FieldRef Name='PredicaInvoicePaid' />
            <Value Type='Boolean'>0</Value>
          </Eq>
        </And>
      </Where>
    </Query>
    <ViewFields>
      <FieldRef Name='Title' />
      <FieldRef Name='LinkFilename' />
      <FieldRef Name='PredicaInvoiceOwner' />
      <FieldRef Name='PredicaInvoicePaid' />     
    </ViewFields>   
  </View>
";
        static void Main(string[] args)
        {
            Console.WriteLine("write invoices site collection url: ");
            string scUrl = Console.ReadLine();
            try
            {
                CheckMyOrders(scUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }

            Console.WriteLine("Press any key to finish");
            Console.ReadKey();
        }

        private static void CheckMyOrders(string siteUrl)
        {
            using (ClientContext ctx = new ClientContext(siteUrl))
            {
                ctx.Load(ctx.Web);
                List orders = ctx.Web.Lists.GetByTitle("Orders");

                CamlQuery query = new CamlQuery();
                query.ViewXml = myInvoices;
                ListItemCollection invoices = orders.GetItems(query);
                ctx.Load(invoices);

                ctx.ExecuteQuery();
                Console.WriteLine("My unpaid invoices:");

                foreach (ListItem item in invoices)
                {
                    Console.WriteLine("Invoice:" + item.FieldValues["FileLeafRef"]);
                }
            }
        }
    }
}
