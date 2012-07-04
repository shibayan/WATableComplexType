using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.WindowsAzure.StorageClient;

namespace WebRole1.Models
{
    public class Comment : TableServiceEntity
    {
        public string Text { get; set; }

        public User User { get; set; }

        public IList<string> Tags { get; set; }
    }
}