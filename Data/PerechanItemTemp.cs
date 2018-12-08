using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocGOST.Data
{
    // Класс для хранения текущего и всех сохранённых для возможности отмены состояний базы данных
    class PerechanItemTemp
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; } // Порядковый номер (номер строки)
        public int tempNumber { get; set; } // Номер сохранённого состояния
        public string designator { get; set; } // Поз. обозначение
        public string name { get; set; } // Наименование
        public string quantity { get; set; } // Кол.
        public string note { get; set; } // Примечание
        public string docum { get; set; } // Документ на поставку
        public string type { get; set; } // Тип компонента
        public string group { get; set; } // Название группы компонента в ед.ч.
        public string groupPlural { get; set; } //Название группы компонента во мн. ч.
    }
}
