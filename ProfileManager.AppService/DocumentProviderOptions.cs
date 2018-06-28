using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProfileManager.AppService
{
    public class DocumentProviderOptions
    {
        public string Endpoint { get; set; }
        public string Key { get; set; }
        public string Database { get; set; }
        public string Collection { get; set; }
    }
}
