/*
 *
 * This file is part of the DocGOST project.    
 * Copyright (C) 2018 Vitalii Nechaev.
 * 
 * This program is free software; you can redistribute it and/or modify it 
 * under the terms of the GNU Affero General Public License version 3 as 
 * published by the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of 
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
 * GNU Affero General Public License for more details.
 * 
 * You should have received a copy of the GNU Affero General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>. *
 * 
 */

using SQLite;
using System;
namespace DocGOST.Data
{
    class PcbSpecificationItem
    {        
        [PrimaryKey, AutoIncrement]
        public int id { get; set; } // Порядковый номер (номер строки с 0-го бита + номер сохранения с 20-го бита)
        public int spSection { get; set; } // Раздел спецификации
        public string format { get; set; } // Формат
        public string zona { get; set; } // Зона
        public string position { get; set; } // Поз.
        public string oboznachenie { get; set; } // Обозначение
        public string name { get; set; } // Наименование
        public string quantity { get; set; } // Кол.
        public string note { get; set; } // Примечание        
        public string group { get; set; } // Обозначение группы, т.е. "C", "DD", "R" и т.д. (нужно для сортировки)
        public string docum { get; set; } // Документ на поставку
        public string designator { get; set; } // Поз. обозначение
        //Оформление текста
        public bool isNameUnderlined { get; set; } //Подчёркивание наименования

        public void makeEmpty()
        {
            this.spSection = 0;
            this.format = String.Empty;
            this.zona = String.Empty;
            this.position = String.Empty;
            this.oboznachenie = String.Empty;
            this.name = String.Empty;
            this.quantity = String.Empty;
            this.note = String.Empty;
            this.isNameUnderlined = false;
        }
    }
}
