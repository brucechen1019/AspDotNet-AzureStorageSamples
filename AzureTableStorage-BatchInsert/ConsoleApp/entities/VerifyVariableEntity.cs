using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.entities
{
    public class VerifyVariableEntity : TableEntity
    {
        public string ConsumerId { get; set; }
        public string Score { get; set; }
    }
}
