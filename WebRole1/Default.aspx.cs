using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

using WebRole1.Models;

namespace WebRole1
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;

            var tableClient = storageAccount.CreateCloudTableClient();

            tableClient.CreateTableIfNotExist("Comment");

            var context = new ComplexTypeTableServiceContext(tableClient.BaseUri.AbsoluteUri, tableClient.Credentials);

            if (IsPostBack)
            {
                var comment = new Comment
                {
                    PartitionKey = "comment",
                    RowKey = Guid.NewGuid().ToString(),
                    Text = "ほげほげ",
                    User = new User
                    {
                        Name = "はうはう"
                    },
                    Tags = new List<string>
                    {
                        "foo", "bar", "baz"
                    }
                };

                context.AddObject("Comment", comment);
                context.SaveChanges();
            }

            ListView1.DataSource = context.CreateQuery<Comment>("Comment").ToList();
            ListView1.DataBind();
        }
    }
}
