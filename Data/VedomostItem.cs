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

namespace DocGOST.Data
{
    class VedomostItem
    {
        [PrimaryKey]
        public int id { get; set; } // Порядковый номер (номер строки с нулевого + номер сохранения с 20-го бита)  
        public string designator { get; set; } // Позиционное обозначение
        public string name { get; set; } // Наименование
        public string kod { get; set; } // Код продукции
        public string docum { get; set; } // Документ на поставку
        public string supplier { get; set; } // Постав
        public string belongs { get; set; } // Куда входит        
        public string quantityIzdelie { get; set; } // Количество на изделие
        public string quantityComplects { get; set; } // Количество в комплекты
        public string quantityTotal { get; set; } // Количество, всего
        public string note { get; set; } //Примечание
        public string group { get; set; } // Название группы компонента в ед.ч.
        public string groupPlural { get; set; } //Название группы компонента во мн.ч. 
        //Оформление текста
        public bool isNameUnderlined { get; set; } //Подчёркивание наименования
    }
}
