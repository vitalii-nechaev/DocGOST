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

using System;
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
        public string supplier { get; set; } // Поставщик
        public string supplierRef { get; set; } // Гиперссылка на компонент у поставщика
        public string belongs { get; set; } // Куда входит        
        public string quantityIzdelie { get; set; } // Количество на изделие
        public string quantityComplects { get; set; } // Количество в комплекты
        public string quantityRegul { get; set; } // Количество в комплекты
        public string quantityTotal { get; set; } // Количество, всего
        public string note { get; set; } //Примечание
        public string group { get; set; } // Название группы компонента в ед.ч.
        public string groupPlural { get; set; } //Название группы компонента во мн.ч. 
        //Оформление текста
        public bool isNameUnderlined { get; set; } //Подчёркивание наименования

        public void makeEmpty()
        {
            this.designator = String.Empty;
            this.name = String.Empty;
            this.kod = String.Empty;
            this.docum = String.Empty;
            this.supplier = String.Empty;
            this.supplierRef = String.Empty;
            this.belongs = String.Empty;
            this.quantityIzdelie = String.Empty;
            this.quantityComplects = String.Empty;
            this.quantityRegul = String.Empty;
            this.quantityTotal = String.Empty;
            this.group = String.Empty;
            this.groupPlural = String.Empty;
            this.note = String.Empty;
            this.isNameUnderlined = false;
        }
    }
}
