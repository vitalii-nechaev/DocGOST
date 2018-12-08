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
    class DesignatorDB
    {
        SQLiteConnection db;

        public DesignatorDB()
        {
            string databasePath = "desDescr.ddGOST";
            db = new SQLiteConnection(databasePath);

            db.CreateTable<DesignatorDescriptionItem> ();
        }

        public int SaveDesignatorItem(DesignatorDescriptionItem item)
        {
            return db.InsertOrReplace(item);            
        }

        public int DeliteDesignatorItem(DesignatorDescriptionItem item)
        {
            return db.Table<DesignatorDescriptionItem>().Delete(x => x.Designator == item.Designator);
        }

        public int GetLength()
        {
            return db.Table<DesignatorDescriptionItem>().OrderByDescending(p => p.Designator).Count();
        }

        public DesignatorDescriptionItem GetItem(int id)
        {
            return db.Table<DesignatorDescriptionItem>().OrderBy(p => p.Designator).ToArray()[id-1];
        }
    }
}
