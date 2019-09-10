using System;
using System.Collections.Generic;
using System.Text;

namespace myPhoneBookApi
{
    public class phonebook
    {
        public string name { get; set; }
        public List<entry> entries = new List<entry>();
    }
}
