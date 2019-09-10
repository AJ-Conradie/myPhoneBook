using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace myPhoneBookApi
{
    public class entry : TableEntity
    {
        public string name { get; set; }
        public string phone { get; set; }
    }
}
