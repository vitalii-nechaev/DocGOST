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

using System.Windows;
using System;
using DocGOST.Data;

namespace DocGOST
{
    /// <summary>
    /// Логика взаимодействия для OsnNadpisWindow.xaml
    /// </summary>
    public partial class OsnNadpisWindow : Window
    {


        ProjectDB project;

        public OsnNadpisWindow(string projecPath)
        {
            
            InitializeComponent();
                        
            project = new Data.ProjectDB(projecPath);

            //Заполнение граф окна значениями из базы данных проекта
            gr1aTextBox.Text = project.GetOsnNadpisItem("1a").specificationValue;
            gr1bTextBox.Text = "<Авто>";
            gr2TextBox.Text = project.GetOsnNadpisItem("2").specificationValue;
            gr3TextBox.Text = project.GetOsnNadpisItem("3").perechenValue;
            gr4aTextBox.Text = project.GetOsnNadpisItem("4a").perechenValue;
            gr4bTextBox.Text = project.GetOsnNadpisItem("4b").perechenValue;
            gr4cTextBox.Text = project.GetOsnNadpisItem("4c").perechenValue;
            gr5TextBox.Text = project.GetOsnNadpisItem("5").perechenValue;
            gr6TextBox.Text = project.GetOsnNadpisItem("6").perechenValue;
            gr7TextBox.Text = project.GetOsnNadpisItem("7").perechenValue;
            gr8TextBox.Text = project.GetOsnNadpisItem("8").perechenValue;
            gr9TextBox.Text = project.GetOsnNadpisItem("9").perechenValue;
            gr10TextBox.Text = project.GetOsnNadpisItem("10").perechenValue;
            gr11aTextBox.Text = project.GetOsnNadpisItem("11a").perechenValue;
            gr11bTextBox.Text = project.GetOsnNadpisItem("11b").perechenValue;
            gr11cTextBox.Text = project.GetOsnNadpisItem("11c").perechenValue;
            gr11dTextBox.Text = project.GetOsnNadpisItem("11d").perechenValue;
            gr11eTextBox.Text = project.GetOsnNadpisItem("11e").perechenValue;
            gr14aTextBox.Text = project.GetOsnNadpisItem("14a").perechenValue;
            gr15aTextBox.Text = project.GetOsnNadpisItem("15a").perechenValue;
            gr16aTextBox.Text = project.GetOsnNadpisItem("16a").perechenValue;
            gr19SpecTextBox.Text = project.GetOsnNadpisItem("19").specificationValue;
            gr19PerechenTextBox.Text = project.GetOsnNadpisItem("19").perechenValue;
            gr21SpecTextBox.Text = project.GetOsnNadpisItem("21").specificationValue;
            gr21PerechenTextBox.Text = project.GetOsnNadpisItem("21").perechenValue;
            gr22SpecTextBox.Text = project.GetOsnNadpisItem("22").specificationValue;
            gr22PerechenTextBox.Text = project.GetOsnNadpisItem("22").perechenValue;
            gr24TextBox.Text = project.GetOsnNadpisItem("24").perechenValue;
            gr25SpecTextBox.Text = project.GetOsnNadpisItem("25").specificationValue;
            gr25PerechenTextBox.Text = project.GetOsnNadpisItem("25").perechenValue;
            gr25VedomostTextBox.Text = project.GetOsnNadpisItem("25").vedomostValue;
            gr32TextBox.Text = project.GetOsnNadpisItem("32").perechenValue;

        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            Save();
            this.DialogResult = true;
        }
               
        private void Save()
        {            
            OsnNadpisItem osnNadpisItem = new OsnNadpisItem();
            osnNadpisItem.grapha = "1a";
            osnNadpisItem.specificationValue = gr1aTextBox.Text;
            osnNadpisItem.perechenValue = gr1aTextBox.Text;
            osnNadpisItem.vedomostValue = gr1aTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "1b";
            osnNadpisItem.specificationValue = String.Empty;
            osnNadpisItem.perechenValue = "Перечень элементов";
            osnNadpisItem.vedomostValue = "Ведомость покупных изделий";
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "2";
            osnNadpisItem.specificationValue = gr2TextBox.Text;
            osnNadpisItem.perechenValue = gr2TextBox.Text + " ПЭ3";
            osnNadpisItem.vedomostValue = gr2TextBox.Text + " ВП";
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "3";
            osnNadpisItem.specificationValue = gr3TextBox.Text;
            osnNadpisItem.perechenValue = gr3TextBox.Text;
            osnNadpisItem.vedomostValue = gr3TextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "4a";
            osnNadpisItem.specificationValue = gr4aTextBox.Text;
            osnNadpisItem.perechenValue = gr4aTextBox.Text;
            osnNadpisItem.vedomostValue = gr4aTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "4b";
            osnNadpisItem.specificationValue = gr4bTextBox.Text;
            osnNadpisItem.perechenValue = gr4bTextBox.Text;
            osnNadpisItem.vedomostValue = gr4bTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "4c";
            osnNadpisItem.specificationValue = gr4cTextBox.Text;
            osnNadpisItem.perechenValue = gr4cTextBox.Text;
            osnNadpisItem.vedomostValue = gr4cTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "5";
            osnNadpisItem.specificationValue = gr5TextBox.Text;
            osnNadpisItem.perechenValue = gr5TextBox.Text;
            osnNadpisItem.vedomostValue = gr5TextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "6";
            osnNadpisItem.specificationValue = gr6TextBox.Text;
            osnNadpisItem.perechenValue = gr6TextBox.Text;
            osnNadpisItem.vedomostValue = gr6TextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "7";
            osnNadpisItem.specificationValue = gr7TextBox.Text;
            osnNadpisItem.perechenValue = gr7TextBox.Text;
            osnNadpisItem.vedomostValue = gr7TextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "8";
            osnNadpisItem.specificationValue = gr8TextBox.Text;
            osnNadpisItem.perechenValue = gr8TextBox.Text;
            osnNadpisItem.vedomostValue = gr8TextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "9";
            osnNadpisItem.specificationValue = gr9TextBox.Text;
            osnNadpisItem.perechenValue = gr9TextBox.Text;
            osnNadpisItem.vedomostValue = gr9TextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "10";
            osnNadpisItem.specificationValue = gr10TextBox.Text;
            osnNadpisItem.perechenValue = gr10TextBox.Text;
            osnNadpisItem.vedomostValue = gr10TextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "11a";
            osnNadpisItem.specificationValue = gr11aTextBox.Text;
            osnNadpisItem.perechenValue = gr11aTextBox.Text;
            osnNadpisItem.vedomostValue = gr11aTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "11b";
            osnNadpisItem.specificationValue = gr11bTextBox.Text;
            osnNadpisItem.perechenValue = gr11bTextBox.Text;
            osnNadpisItem.vedomostValue = gr11bTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "11c";
            osnNadpisItem.specificationValue = gr11cTextBox.Text;
            osnNadpisItem.perechenValue = gr11cTextBox.Text;
            osnNadpisItem.vedomostValue = gr11cTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "11d";
            osnNadpisItem.specificationValue = gr11dTextBox.Text;
            osnNadpisItem.perechenValue = gr11dTextBox.Text;
            osnNadpisItem.vedomostValue = gr11dTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "11e";
            osnNadpisItem.specificationValue = gr11eTextBox.Text;
            osnNadpisItem.perechenValue = gr11eTextBox.Text;
            osnNadpisItem.vedomostValue = gr11eTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "14a";
            osnNadpisItem.specificationValue = gr14aTextBox.Text;
            osnNadpisItem.perechenValue = gr14aTextBox.Text;
            osnNadpisItem.vedomostValue = gr14aTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "15a";
            osnNadpisItem.specificationValue = gr15aTextBox.Text;
            osnNadpisItem.perechenValue = gr15aTextBox.Text;
            osnNadpisItem.vedomostValue = gr15aTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "16a";
            osnNadpisItem.specificationValue = gr16aTextBox.Text;
            osnNadpisItem.perechenValue = gr16aTextBox.Text;
            osnNadpisItem.vedomostValue = gr16aTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "19";
            osnNadpisItem.specificationValue = gr19SpecTextBox.Text;
            osnNadpisItem.perechenValue = gr19PerechenTextBox.Text;
            osnNadpisItem.vedomostValue = gr19VedomostTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "21";
            osnNadpisItem.specificationValue = gr21SpecTextBox.Text;
            osnNadpisItem.perechenValue = gr21PerechenTextBox.Text;
            osnNadpisItem.vedomostValue = gr21VedomostTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "22";
            osnNadpisItem.specificationValue = gr22SpecTextBox.Text;
            osnNadpisItem.perechenValue = gr22PerechenTextBox.Text;
            osnNadpisItem.vedomostValue = gr22VedomostTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "24";
            osnNadpisItem.specificationValue = gr24TextBox.Text;
            osnNadpisItem.perechenValue = gr24TextBox.Text;
            osnNadpisItem.vedomostValue = gr24TextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "25";
            osnNadpisItem.specificationValue = gr25SpecTextBox.Text;
            osnNadpisItem.perechenValue = gr25PerechenTextBox.Text;
            osnNadpisItem.vedomostValue = gr25VedomostTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "32";
            osnNadpisItem.specificationValue = "А4";
            osnNadpisItem.perechenValue = "А4";
            osnNadpisItem.vedomostValue = "А3";
            project.SaveOsnNadpisItem(osnNadpisItem);
        }
    }
}
