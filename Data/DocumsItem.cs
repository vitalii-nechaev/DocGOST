using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocGOST.Data
{
    internal class DocumsItem
    {
        [PrimaryKey]
        public string code { get; set; } // Код документа
        public string name { get; set; } // Наименование документа
        public string format { get; set; } // Формат        
        public string note { get; set; } // Примечание 
        public int numberByOrder { get; set; } // Порядок по ГОСТ Р 2.102-2023 
    }
}
