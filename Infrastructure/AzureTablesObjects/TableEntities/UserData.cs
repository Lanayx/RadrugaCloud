using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.AzureTablesObjects.TableEntities
{
    using Microsoft.WindowsAzure.Storage.Table;

  
    public class UserData : TableEntity
    {
        public UserData()
        {
            RowKey = Guid.NewGuid().ToString("N");
        }

        public UserData(string type):this()
        {
            PartitionKey = type;
        }

      
        public string Text { get; set; }

       
        public string IpAddress { get; set; }

       
        public string Browser { get; set; }

        public string OSType { get; set; }

        public string UserId { get; set; }
    }
}
