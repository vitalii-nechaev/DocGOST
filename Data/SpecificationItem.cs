using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocGOST.Data
{
    class SpecificationItem
    {
        public SpecificationItem()
        {
            designator = String.Empty;
            name = String.Empty;
            quantity = String.Empty;
            note = String.Empty;
            docum = String.Empty;
            type = String.Empty;
            group = String.Empty;
            
        }
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; } // Порядковый номер (номер строки)
        public string format { get; set; } // Формат
        public string zona { get; set; } // Зона
        public string position { get; set; } // Поз.
        public string name { get; set; } // Наименование
        public string quantity { get; set; } // Кол.
        public string note { get; set; } // Примечание
        public string docum { get; set; } // Документ на поставку
        public string type { get; set; } // Тип компонента
        public string group { get; set; } // Название группы компонента в ед.ч.
        public string designator { get; set; } // Поз. обозначение
    }
}
