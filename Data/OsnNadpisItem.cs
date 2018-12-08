using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocGOST.Data
{
    class OsnNadpisItem
    {
        [PrimaryKey]
        public string grapha { get; set; } // Номер графы        
        public string perechenValue { get; set; } // Значение        
    }
}
