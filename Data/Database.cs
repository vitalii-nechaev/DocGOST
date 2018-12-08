using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocGOST.Data
{
    class Database
    {
        SQLiteConnection db;

        public Database(string databasePath)
        {  
            db = new SQLiteConnection(databasePath);

            db.CreateTable<PerechenItem>();
            db.CreateTable<SpecificationItem>();
            db.CreateTable<OsnNadpisItem>();

            //Если новый проект, заполняем основную надпись значениями по умолчанию
            if (db.Table<OsnNadpisItem>().Where(x => x.grapha == "1a").FirstOrDefault() == null) FillDefaultValues();

        }

        public int GetPerechenLength()
        {
            return db.Table<PerechenItem>().OrderByDescending(p => p.ID).FirstOrDefault().ID;
        }
          

        public PerechenItem GetPerechenItem(int id)
        {
            return db.Table<PerechenItem>().Where(x => x.ID == id).FirstOrDefault();           
        }

        public int AddPerechenItem(PerechenItem item)
        {
            return db.Insert(item);           
        }

        public int SaveOsnNadpisItem(OsnNadpisItem item)
        {
            return db.InsertOrReplace(item);
        }

        public OsnNadpisItem GetOsnNadpisItem(string grapha)
        {
            return db.Table<OsnNadpisItem>().Where(x => x.grapha == grapha).FirstOrDefault();
        }

        private void FillDefaultValues()
        {
            OsnNadpisItem osnNadpisItem = new OsnNadpisItem();
            osnNadpisItem.grapha = "1a";
            osnNadpisItem.perechenValue = String.Empty;
            SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "1b";
            osnNadpisItem.perechenValue = "Перечень элементов";
            SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "2";
            osnNadpisItem.perechenValue = "ПЭ3";
            SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "3";
            osnNadpisItem.perechenValue = "";
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
            SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "11a";
            osnNadpisItem.perechenValue = "";
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
            SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "32";
            SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.perechenValue = "А4";
        }

        public void Close ()
        {
            db.Close();            
        }
    }
}
