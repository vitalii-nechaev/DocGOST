using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocGOST.Data
{
    internal class DielProperties
    {
        public string Name { get; set; }
        public string Height { get; set; }
        public int DielType { get; set; } //1 - ядро, 2 - препрег
        public int Quantity { get; set; }
    }
}
