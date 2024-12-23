/*
 *
 * This file is part of the DocGOST project.    
 * Copyright (C) 2025 Vitalii Nechaev.
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
using System.Text.RegularExpressions;

namespace DocGOST.Data
{
    internal class DocumsDB
    {
        SQLiteConnection db;

        private enum docsOrder // Порядок перечисления документации в спецификации по таблице 1 ГОСТ Р 2.102-2023 
        {
            ЭМД = 0,
            ЧД = 1,
            ЭСБ = 2,
            ЭМ = 3,
            СБ = 4,
            ВО = 5,
            ТЧ = 6,
            ГЧ = 7,
            МЭ = 8,
            МЧ = 9,
            УЧ = 10,
            Э1 = 11, // по ГОСТ 2.701-2008
            Э2 = 12,
            Э3 = 13,
            Э4 = 14,
            Э5 = 15,
            Э6 = 16,
            Э7 = 17,
            Э0 = 18,
            ТЭ1 = 19,
            ТЭ2 = 20,
            ТЭ3 = 21,
            ТЭ4 = 22,
            ТЭ5 = 23,
            ТЭ6 = 24,
            ТЭ7 = 25,
            ТЭ0 = 26,
            ПЭ1 = 27,
            ПЭ2 = 28,
            ПЭ3 = 29,
            ПЭ4 = 30,
            ПЭ5 = 31,
            ПЭ6 = 32,
            ПЭ7 = 33,
            ПЭ0 = 34,

            Е1 = 202,
            Е2 = 203,
            Е3 = 204,
            Е4 = 205,
            Е5 = 206,
            Е6 = 207,
            Е7 = 208,
            Е0 = 209,
            ТЕ1 = 210,
            ТЕ2 = 211,
            ТЕ3 = 212,
            ТЕ4 = 213,
            ТЕ5 = 214,
            ТЕ6 = 215,
            ТЕ7 = 216,
            ТЕ0 = 217,
            ПЕ1 = 218,
            ПЕ2 = 219,
            ПЕ3 = 220,
            ПЕ4 = 221,
            ПЕ5 = 222,
            ПЕ6 = 223,
            ПЕ7 = 224,
            ПЕ0 = 225,

            ЭСК = 250, // Снова порядок перечисления документации в спецификации по таблице 1 ГОСТ Р 2.102-2023 
            СП = 251,
            ВС = 252,
            ВД = 253,
            ВП = 254,
            ВИ = 255,
            ДП = 256,
            ПТ = 257,
            ЭП = 258,
            ТП = 259,
            ПЗ = 260,
            ВЭД = 261,
            ТУ = 262,
            ПМ = 263,

            ТБ = 264, 
            РР = 364,
            И = 464,
            Д = 564,

            РЭ = 664, // по табилце 2 ГОСТ 2.601-2019 
            ИМ = 665,
            ФО = 666,
            ПС = 667,
            ЭТ = 668,
            КИ = 669,
            НЗЧ = 670,
            НМ = 671,
            ЗИ = 672,
            УП = 673,
            ИС = 674,
            ВЭ = 675,

            РК = 676, // по табилце 3 ГОСТ 2.602-2013 
            РС = 677,
            УК = 678,
            УС = 679,
            ЗК = 690,
            ЗС = 691,
            МК = 692,
            МС = 693,
            ЗИК = 694,
            ЗИС = 695,
            ВРК = 696,
            ВРС = 697
        }

        public DocumsDB(bool isDocums4Pcb)
        {
            string databasePath = "documentsList.dGOST";
            if (isDocums4Pcb) { databasePath = "documents4PcbList.dGOST"; }
            db = new SQLiteConnection(databasePath);

            db.CreateTable<DocumsItem>();

            // Заполняем значениями по умолчанию, если база данных не заполнена
            if (db.Table<DocumsItem>().OrderByDescending(p => p.code).Count() == 0)
            {
                DocumsItem item = new DocumsItem();
               
                if (isDocums4Pcb)
                {

                    item.code = "СБ";
                    item.name = "Сборочный чертёж";
                    item.format = "А1";
                    item.note = string.Empty;
                    item.numberByOrder = (int)docsOrder.СБ;
                    db.InsertOrReplace(item);

                    item.code = "Т5М";
                    item.name = "Данные проектирования";
                    item.format = "*)";
                    item.note = "*) CD-диск";
                    item.numberByOrder = 1000;
                    db.InsertOrReplace(item);
                }
                else
                {
                    item.code = "СБ";
                    item.name = "Сборочный чертёж";
                    item.format = "А1";
                    item.note = string.Empty;
                    item.numberByOrder = (int)docsOrder.СБ;
                    db.InsertOrReplace(item);

                    item.code = "Э3";
                    item.name = "Схема электрическая принципиальная";
                    item.format = "А3";
                    item.note = string.Empty;
                    item.numberByOrder = (int)docsOrder.Э3;
                    db.InsertOrReplace(item);

                    item.code = "ПЭ3";
                    item.name = "Перечень элементов";
                    item.format = "А4";
                    item.note = string.Empty;
                    item.numberByOrder = (int)docsOrder.ПЭ3;
                    db.InsertOrReplace(item);

                    item.code = "ВП";
                    item.name = "Ведомость покупных изделий";
                    item.format = "А3";
                    item.note = string.Empty;
                    item.numberByOrder = (int)docsOrder.ВП;
                    db.InsertOrReplace(item);

                    item.code = "Д33";
                    item.name = "Данные проектирования модуля";
                    item.format = "*)";
                    item.note = "*) CD-диск";
                    item.numberByOrder = 597;
                    db.InsertOrReplace(item);
                }
            }
        }

        public int SaveDocumsItem(DocumsItem item)
        {
            docsOrder numberByOrder;

            if (Enum.TryParse(item.code.Trim(), true, out numberByOrder)) item.numberByOrder = (int)numberByOrder;
            else if (Enum.TryParse(Regex.Replace(item.code.Trim(), "[0-9]", "", RegexOptions.IgnoreCase), true, out numberByOrder))
            {
                string code = item.code.Trim().Replace(Regex.Replace(item.code.Trim(), "[0-9]", "", RegexOptions.IgnoreCase), "");// оставляем только цифры
                item.numberByOrder = (int)numberByOrder + int.Parse(code);
            }
            else item.numberByOrder = 1000;

            return db.InsertOrReplace(item);
        }

        public int DeliteDocumsItem(DocumsItem item)
        {
            return db.Table<DocumsItem>().Delete(x => x.code == item.code);
        }

        public int GetLength()
        {
            return db.Table<DocumsItem>().OrderBy(p => p.numberByOrder).Count();
        }

        public DocumsItem GetItem(int id)
        {
            return db.Table<DocumsItem>().OrderBy(p => p.numberByOrder).ToArray()[id - 1];
        }


    }
}
