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
    // В базе данных хранятся следующие настройки:

    // designatorPropName - наименование свойства с позиционным обозначением
    // designatorPropNameKiCad - наименование свойства с позиционным обозначением в KiCad
    // namePropName - наименование свойства с наименованием компонента
    // namePropNameKiCad - наименование свойства с наименованием компонента в KiCad
    // documPropName - наименование свойства с документом на поставку компонента
    // documPropNameKiCad - наименование свойства с документом на поставку компонента в KiCad
    // notePropName - наименование свойства с комментарием
    // notePropNameKiCad - наименование свойства с комментарием в KiCad

    // drawGraf30 - чертить ли графы 27-30 основной надписи (True/False)

    // prjDocNumberPropName - свойств0 с наименованием децимального номера изделия
    // prjDocNumberPropNameKiCad - свойство с наименованием децимального номера изделия в KiCad
    // prjNameStr1PropName - свойство с первой строкой наименования изделия
    // prjNameStr2PropName - свойство со второй строкой наименования изделия
    // prjNameStr1PropNameeKiCad - свойство с первой строкой наименования изделия в KiCad
    // prjNameStr2PropNameeKiCad - свойство со второй строкой наименования изделия в KiCad
    // prjNamePropNameKiCad - свойство с наименованием изделия в KiCad
    // pcbDocNumberPropName - свойство с наименованием децимального номера платы
    // pcbDocNumberPropNameKiCad - свойство с наименованием децимального номера платы в KiCad
    // pcbNameStr1PropName - свойство с первой строкой наименования платы в KiCad
    // pcbNameStr2PropName - свойство со второй строкой наименования платы в KiCad
    // pcbNameStr1PropName - свойство с первой строкой наименования платы в KiCad
    // pcbNameStr2PropName - свойство со второй строкой наименования платы в KiCad


    class SettingsDB
    {
        SQLiteConnection db;

        public SettingsDB()
        {
            string databasePath = "settings.sGOST";
            db = new SQLiteConnection(databasePath);

            db.CreateTable<SettingsItem>();
        }

        public int SaveSettingItem(SettingsItem item)
        {
            return db.InsertOrReplace(item);
        }

        public int DeleteSettingsItem(SettingsItem item)
        {
            return db.Table<SettingsItem>().Delete(x => x.name == item.name);
        }

        public int GetLength()
        {
            return db.Table<SettingsItem>().OrderByDescending(p => p.name).Count();
        }

        public SettingsItem GetItem(string name)
        {
            return db.Table<SettingsItem>().Where(x => x.name == name).FirstOrDefault();
        }
    }
}
