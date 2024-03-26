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
using System.Collections.Generic;

namespace DocGOST.Data
{
    class ProjectDB
    {
        SQLiteConnection db;

        Global id;

        #region Операции со всей базой данных проекта

        /// <summary>
        /// Создание/открытие базы данных проекта по пути databasePath
        /// </summary>
        /// <param name="databasePath"> Расположение проекта .DocGOST на диске </param>
        /// <param name="createOrOpen"> True - создаёт файл, false - открывает файл </param>
        public ProjectDB(string databasePath)
        {
            db = new SQLiteConnection(databasePath);
            db.CreateTable<PerechenItem>();
            db.CreateTable<SpecificationItem>();
            db.CreateTable<VedomostItem>();
            db.CreateTable<PcbSpecificationItem>();
            db.CreateTable<OsnNadpisItem>();
            db.CreateTable<ParameterItem>();

            //Если новый проект, заполняем основную надпись и параметры проекта значениями по умолчанию
            if (db.Table<OsnNadpisItem>().Where(x => x.grapha == "1a").FirstOrDefault() == null)
            {
                FillDefaultValues();
                FillParameterDefaultValues();
            }

            id = new Global();
        }

        /// <summary>
        /// Сохранение проекта, т.е. запись в базу данных прокта с номером промежуточного сохранения "0"
        /// </summary>
        /// <param name="perTempNumber"> Номер текущего промежуточного сохранения перечня элементов</param>
        /// <param name="specTempNumber"> Номер текущего промежуточного сохранения спецификации</param>
        public void Save(int perTempNumber, int specTempNumber, int vedomostTempNumber, int pcbSpecTempNumber)
        {
            //Перечень элементов:
            //Удаление сохранённых ранее данных:
            db.Table<PerechenItem>().Delete(x => x.id < Global.TempStartPosMask); //Удаляем все элементы, у которых tempNum = 0

            //Сохранение текущих элементов:
            int perLength = GetPerechenLength(perTempNumber);
            for (int i = 1; i <= perLength; i++)
            {
                PerechenItem perItem = new PerechenItem();
                perItem = GetPerechenItem(i, perTempNumber);
                perItem.id = i;
                AddPerechenItem(perItem);
            }

            //Спецификация:
            //Удаление сохранённых ранее данных:
            db.Table<SpecificationItem>().Delete(x => x.id <= Global.TempStartPosMask); //Удаляем все элементы, у которых tempNum = 0
            //Сохранение текущих элементов:
            int specLength = GetSpecLength(specTempNumber);
            for (int i = 1; i <= specLength; i++)
            {
                SpecificationItem specItem = new SpecificationItem();
                specItem = GetSpecItem(i, specTempNumber);
                specItem.id = i;
                AddSpecItem(specItem);
            }

            //Ведомость:
            //Удаление сохранённых ранее данных:
            db.Table<VedomostItem>().Delete(x => x.id <= Global.TempStartPosMask); //Удаляем все элементы, у которых tempNum = 0
            //Сохранение текущих элементов:
            int vedomostLength = GetVedomostLength(vedomostTempNumber);
            for (int i = 1; i <= vedomostLength; i++)
            {
                VedomostItem vedomostItem = new VedomostItem();
                vedomostItem = GetVedomostItem(i, vedomostTempNumber);
                vedomostItem.id = i;
                AddVedomostItem(vedomostItem);
            }

            //Спецификация на печатную плату:
            //Удаление сохранённых ранее данных:
            db.Table<PcbSpecificationItem>().Delete(x => x.id <= Global.TempStartPosMask); //Удаляем все элементы, у которых tempNum = 0
            //Сохранение текущих элементов:
            int pcbSpecLength = GetPcbSpecLength(pcbSpecTempNumber);
            for (int i = 1; i <= pcbSpecLength; i++)
            {
                PcbSpecificationItem specItem = new PcbSpecificationItem();
                specItem = GetPcbSpecItem(i, pcbSpecTempNumber);
                specItem.id = i;
                AddPcbSpecItem(specItem);
            }
        }

        /// <summary>
        /// Удаляет все временные данные проекта
        /// </summary>
        public void DeleteTempData()
        {
            db.Table<PerechenItem>().Delete(x => x.id > Global.TempStartPosMask);
            db.Table<SpecificationItem>().Delete(x => x.id > Global.TempStartPosMask);
            db.Table<PcbSpecificationItem>().Delete(x => x.id > Global.TempStartPosMask);
            db.Table<VedomostItem>().Delete(x => x.id > Global.TempStartPosMask);
        }

        #endregion

        #region Операции с перечнем элементов

        /// <summary>
        /// Возвращает длину перечня с номером временного сохранения tempNumber
        /// </summary>
        /// <param name="tempNumber"> Номер временного сохранения </param>
        /// <returns></returns>
        public int GetPerechenLength(int tempNumber)
        {
            for (int i = 1; i < int.MaxValue; i++)
            {
                int tempID = id.makeID(i, tempNumber);
                if (db.Table<PerechenItem>().Where(x => x.id == tempID).FirstOrDefault() == null)
                {
                    return i - 1;
                }

            }
            return 0;
        }

        /// <summary>
        /// Возвращает элемент перечня c номеров временного сохранения tempNumber из строки srtNum
        /// </summary>
        /// <param name="strNum"> Номер строки элемента </param>
        /// <param name="tempNumber"> Номер текущего сохранения элемента </param>
        /// <returns></returns>
        public PerechenItem GetPerechenItem(int strNum, int tempNumber)
        {
            int tempID = id.makeID(strNum, tempNumber);
            return db.Table<PerechenItem>().Where(x => x.id == tempID).FirstOrDefault();
        }

        /// <summary>
        /// Добавление элемента item в перечень элементов 
        /// </summary>
        /// <param name="item"> Элемент перечня для добавления </param>
        /// <returns></returns>
        public int AddPerechenItem(PerechenItem item)
        {
            return db.InsertOrReplace(item);
        }

        /// <summary>
        /// Удаляет временные данные перечня после perTempNumber 
        /// </summary>        
        public void DeletePerechenTempData(int perTempNumber)
        {
            int perStartID = (perTempNumber + 1) << Global.TempStartPos;
            db.Table<PerechenItem>().Delete(x => x.id > perStartID);
        }

        #endregion

        #region Операции со спецификацией
        /// <summary>
        /// Возвращает длину спецификации с номером временного сохранения tempNumber
        /// </summary>
        /// <param name="tempNumber"> Номер временного сохранения </param>
        /// <returns></returns>
        public int GetSpecLength(int tempNumber)
        {
            for (int i = 1; i < int.MaxValue; i++)
            {
                int tempID = id.makeID(i, tempNumber);
                if (db.Table<SpecificationItem>().Where(x => x.id == tempID).FirstOrDefault() == null)
                {
                    return i - 1;
                }

            }
            return 0;
        }

        /// <summary>
        /// Возвращает элемент спецификации с номером временного сохранения tempNumber из строчки strNumber
        /// </summary>
        /// <param name="strNum"> Номер строки элемента спецификации </param>
        /// <param name="tempNumber"> Номер временного сохранения </param>
        /// <returns></returns>
        public SpecificationItem GetSpecItem(int strNum, int tempNumber)
        {
            int tempID = id.makeID(strNum, tempNumber);
            return db.Table<SpecificationItem>().Where(x => x.id == tempID).FirstOrDefault();
        }

        /// <summary>
        /// Возвращает список записей спецификации с номером текущего сохранения tempNumber
        /// </summary>
        /// <param name="tempNumber"> номер текущего сохранения </param>
        /// <returns></returns>
        public List<SpecificationItem> GetSpecificationList(int tempNumber)
        {
            int minID = tempNumber << 20;
            int maxID = (tempNumber + 1) << 20;
            return db.Table<SpecificationItem>().Where(s => s.id > minID).Where(s => s.id < maxID).ToList();
        }

        /// <summary>
        /// Добавляет элемент спецификации в базу данных проекта
        /// </summary>
        /// <param name="item"> Элемент спецификации </param>
        /// <returns></returns>
        public int AddSpecItem(SpecificationItem item)
        {
            return db.InsertOrReplace(item);
        }

        /// <summary>Удаляет временные данные спецификации после specTempNumber </summary> 
        public void DeleteSpecTempData(int specTempNumber)
        {
            int specStartID = (specTempNumber + 1) << Global.TempStartPos;
            db.Table<SpecificationItem>().Delete(x => x.id > specStartID);
        }
        #endregion

        #region Операции с ведомостью покупных изделий
        /// <summary>
        /// Возвращает длину ведомости с номером временного сохранения tempNumber
        /// </summary>
        /// <param name="tempNumber"> Номер временного сохранения </param>
        /// <returns></returns>
        public int GetVedomostLength(int tempNumber)
        {
            for (int i = 1; i < int.MaxValue; i++)
            {
                int tempID = id.makeID(i, tempNumber);
                if (db.Table<VedomostItem>().Where(x => x.id == tempID).FirstOrDefault() == null)
                {
                    return i - 1;
                }

            }
            return 0;
        }

        /// <summary>
        /// Возвращает элемент ведомости с номером временного сохранения tempNumber из строчки strNumber
        /// </summary>
        /// <param name="strNum"> Номер строки элемента спецификации </param>
        /// <param name="tempNumber"> Номер временного сохранения </param>
        /// <returns></returns>
        public VedomostItem GetVedomostItem(int strNum, int tempNumber)
        {
            int tempID = id.makeID(strNum, tempNumber);
            return db.Table<VedomostItem>().Where(x => x.id == tempID).FirstOrDefault();
        }

        /// <summary>
        /// Возвращает список записей ведомости с номером текущего сохранения tempNumber
        /// </summary>
        /// <param name="tempNumber"> номер текущего сохранения </param>
        /// <returns></returns>
        public List<VedomostItem> GetVedomostList(int tempNumber)
        {
            int minID = tempNumber << 20;
            int maxID = (tempNumber + 1) << 20;
            return db.Table<VedomostItem>().Where(s => s.id > minID).Where(s => s.id < maxID).ToList();
        }

        /// <summary>
        /// Добавляет элемент ведомости в базу данных проекта
        /// </summary>
        /// <param name="item"> Элемент ведомости</param>
        /// <returns></returns>
        public int AddVedomostItem(VedomostItem item)
        {
            return db.InsertOrReplace(item);
        }

        /// <summary>Удаляет временные данные ведомости после vesomostTempNumber </summary> 
        public void DeleteVedomostTempData(int vedomostTempNumber)
        {
            int specStartID = (vedomostTempNumber + 1) << Global.TempStartPos;
            db.Table<VedomostItem>().Delete(x => x.id > specStartID);
        }
        #endregion

        #region Операции со спецификацией на печатную плату
        /// <summary>
        /// Возвращает длину спецификации с номером временного сохранения tempNumber
        /// </summary>
        /// <param name="tempNumber"> Номер временного сохранения </param>
        /// <returns></returns>
        public int GetPcbSpecLength(int tempNumber)
        {
            for (int i = 1; i < int.MaxValue; i++)
            {
                int tempID = id.makeID(i, tempNumber);
                if (db.Table<PcbSpecificationItem>().Where(x => x.id == tempID).FirstOrDefault() == null)
                {
                    return i - 1;
                }

            }
            return 0;
        }

        /// <summary>
        /// Возвращает элемент спецификации с номером временного сохранения tempNumber из строчки strNumber
        /// </summary>
        /// <param name="strNum"> Номер строки элемента спецификации </param>
        /// <param name="tempNumber"> Номер временного сохранения </param>
        /// <returns></returns>
        public PcbSpecificationItem GetPcbSpecItem(int strNum, int tempNumber)
        {
            int tempID = id.makeID(strNum, tempNumber);
            return db.Table<PcbSpecificationItem>().Where(x => x.id == tempID).FirstOrDefault();
        }

        /// <summary>
        /// Возвращает список записей спецификации с номером текущего сохранения tempNumber
        /// </summary>
        /// <param name="tempNumber"> номер текущего сохранения </param>
        /// <returns></returns>
        public List<PcbSpecificationItem> GetPcbSpecificationList(int tempNumber)
        {
            int minID = tempNumber << 20;
            int maxID = (tempNumber + 1) << 20;
            return db.Table<PcbSpecificationItem>().Where(s => s.id > minID).Where(s => s.id < maxID).ToList();
        }

        /// <summary>
        /// Добавляет элемент спецификации в базу данных проекта
        /// </summary>
        /// <param name="item"> Элемент спецификации </param>
        /// <returns></returns>
        public int AddPcbSpecItem(PcbSpecificationItem item)
        {
            return db.InsertOrReplace(item);
        }

        /// <summary>Удаляет временные данные спецификации после specTempNumber </summary> 
        public void DeletePcbSpecTempData(int specTempNumber)
        {
            int specStartID = (specTempNumber + 1) << Global.TempStartPos;
            db.Table<PcbSpecificationItem>().Delete(x => x.id > specStartID);
        }
        #endregion


        #region Операции с основной надписью

        /// <summary>
        /// Сохраняет в базу данных проекта графу основной надписи <c>item</c>
        /// </summary>
        /// <param name="item"> Графа основной надписи </param>
        /// <returns></returns>
        public int SaveOsnNadpisItem(OsnNadpisItem item)
        {
            return db.InsertOrReplace(item);
        }

        /// <summary>
        /// Возвращает графу основной надписи с названием <c>grapha</c>
        /// </summary>
        /// <param name="grapha">Название графы основной надписи, например, "1a", "1b", "2" и т.д.</param>
        /// <returns></returns>
        public OsnNadpisItem GetOsnNadpisItem(string grapha)
        {
            return db.Table<OsnNadpisItem>().Where(x => x.grapha == grapha).FirstOrDefault();
        }

        /// <summary>
        /// Заполнение основной надписи значениями по умолчанию
        /// </summary>
        /// <remarks> Вызывается при создании нового проекта </remarks>
        private void FillDefaultValues()
        {
            OsnNadpisItem osnNadpisItem = new OsnNadpisItem();
            osnNadpisItem.grapha = "1a";
            osnNadpisItem.perechenValue = "Наименование";
            osnNadpisItem.specificationValue = osnNadpisItem.perechenValue;
            osnNadpisItem.vedomostValue = osnNadpisItem.perechenValue;
            osnNadpisItem.pcbSpecificationValue = "Плата печатная";
            SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "1b";
            osnNadpisItem.perechenValue = "Перечень элементов";
            osnNadpisItem.specificationValue = String.Empty;
            osnNadpisItem.pcbSpecificationValue = String.Empty;
            osnNadpisItem.vedomostValue = "Ведомость покупных изделий";
            SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "2";
            osnNadpisItem.specificationValue = "АБВГ.ХХХХХХ.ХХХ";
            osnNadpisItem.perechenValue = osnNadpisItem.specificationValue + " ПЭ3";
            osnNadpisItem.vedomostValue = osnNadpisItem.specificationValue + " ВП";
            osnNadpisItem.pcbSpecificationValue = "АБВГ.6ХХХХХ.ХХХ";
            SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "3";
            osnNadpisItem.perechenValue = String.Empty;
            osnNadpisItem.specificationValue = osnNadpisItem.perechenValue;
            osnNadpisItem.vedomostValue = osnNadpisItem.perechenValue;
            osnNadpisItem.pcbSpecificationValue = osnNadpisItem.perechenValue;
            SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "4a";
            SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "4b";
            SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "4c";
            SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "5";
            SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "6";
            SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "7";
            SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "8";
            SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "9";
            SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "10";
            osnNadpisItem.perechenValue = "Согл.";
            osnNadpisItem.specificationValue = osnNadpisItem.perechenValue;
            osnNadpisItem.vedomostValue = osnNadpisItem.perechenValue;
            osnNadpisItem.pcbSpecificationValue = osnNadpisItem.perechenValue;
            SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "11a";
            osnNadpisItem.perechenValue = String.Empty;
            osnNadpisItem.specificationValue = osnNadpisItem.perechenValue;
            osnNadpisItem.vedomostValue = osnNadpisItem.perechenValue;
            osnNadpisItem.pcbSpecificationValue = osnNadpisItem.perechenValue;
            SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "11b";
            SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "11c";
            SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "11d";
            SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "11e";
            SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "14a";
            SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "15a";
            SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "16a";
            SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "19";
            SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "21";
            SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "22";
            SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "24";
            SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "25";
            osnNadpisItem.perechenValue = "АБВГ.ХХХХХХ.ХХХ"; //Для перечня здесь указываем спецификацию
            osnNadpisItem.vedomostValue = "АБВГ.ХХХХХХ.ХХХ"; //Для ведомости здесь указываем спецификацию
            osnNadpisItem.specificationValue = String.Empty;
            osnNadpisItem.pcbSpecificationValue = "АБВГ.ХХХХХХ.ХХХ"; //Для ведомости здесь указываем спецификацию
            SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "32";
            osnNadpisItem.perechenValue = "А4";
            osnNadpisItem.vedomostValue = "А3";
            osnNadpisItem.specificationValue = "А4";
            SaveOsnNadpisItem(osnNadpisItem);            
        }

        #endregion

        #region Операции с параметрами проекта

        /// <summary>
        /// Сохраняет в базу данных проекта параметр <c>item</c>
        /// </summary>
        /// <param name="item"> Название параметра </param>
        /// <returns></returns>
        public int SaveParameterItem(ParameterItem item)
        {
            return db.InsertOrReplace(item);
        }

        /// <summary>
        /// Возвращает параметр с названием <c>name</c>
        /// </summary>
        /// <param name="name">Название параметра (Variant, isListRegistrChecked, isStartFromSecondChecked)</param>
        /// <returns></returns>
        public ParameterItem GetParameterItem(string name)
        {
            return db.Table<ParameterItem>().Where(x => x.name == name).FirstOrDefault();
        }

        /// <summary>
        /// Заполнение параметров значениями по умолчанию
        /// </summary>
        /// <remarks> Вызывается при создании нового проекта </remarks>
        private void FillParameterDefaultValues()
        {
            ParameterItem paramItem = new ParameterItem();
            paramItem.name = "Variant";
            paramItem.value = "[No Variations]";
            SaveParameterItem(paramItem);
        }

        #endregion
    }
}
