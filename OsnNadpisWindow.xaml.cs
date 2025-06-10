﻿/*
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
            gr1PcbSpecTextBox.Text = project.GetOsnNadpisItem("1a").pcbSpecificationValue;
            gr1bTextBox.Text = "<Авто>";
            gr2TextBox.Text = project.GetOsnNadpisItem("2").specificationValue;
            gr2PcbSpecTextBox.Text = project.GetOsnNadpisItem("2").pcbSpecificationValue;
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
            gr19PcbSpecTextBox.Text = project.GetOsnNadpisItem("19").pcbSpecificationValue;
            gr21SpecTextBox.Text = project.GetOsnNadpisItem("21").specificationValue;
            gr21PerechenTextBox.Text = project.GetOsnNadpisItem("21").perechenValue;
            gr21PcbSpecTextBox.Text = project.GetOsnNadpisItem("21").pcbSpecificationValue;
            gr22SpecTextBox.Text = project.GetOsnNadpisItem("22").specificationValue;
            gr22PerechenTextBox.Text = project.GetOsnNadpisItem("22").perechenValue;
            gr22PcbSpecTextBox.Text = project.GetOsnNadpisItem("22").pcbSpecificationValue;
            gr24TextBox.Text = project.GetOsnNadpisItem("24").perechenValue;
            gr25SpecTextBox.Text = project.GetOsnNadpisItem("25").specificationValue;
            gr25PerechenTextBox.Text = project.GetOsnNadpisItem("25").perechenValue;
            gr25VedomostTextBox.Text = project.GetOsnNadpisItem("25").vedomostValue;
            gr25PcbSpecTextBox.Text = project.GetOsnNadpisItem("25").pcbSpecificationValue;
            gr32TextBox.Text = project.GetOsnNadpisItem("32").perechenValue;

            string perechenType = project.GetOsnNadpisItem("2").perechenValue;
            if (perechenType.Length >= 3)
                perechenType = perechenType.Substring(perechenType.Length - 3, 3);
            else perechenType = "ПЭ3";

            switch (perechenType)
            {
                case "ПЭ1":
                    PerechenTypeComboBox.SelectedIndex = 0;
                    break;
                case "ПЭ2":
                    PerechenTypeComboBox.SelectedIndex = 1;
                    break;
                case "ПЭ3":
                    PerechenTypeComboBox.SelectedIndex = 2;
                    break;
                case "ПЭ4":
                    PerechenTypeComboBox.SelectedIndex = 3;
                    break;
                case "ПЭ5":
                    PerechenTypeComboBox.SelectedIndex = 4;
                    break;
                case "ПЭ6":
                    PerechenTypeComboBox.SelectedIndex = 5;
                    break;
                case "ПЭ7":
                    PerechenTypeComboBox.SelectedIndex = 6;
                    break;
                case "ПЭ0":
                    PerechenTypeComboBox.SelectedIndex = 7;
                    break;

            }

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
            osnNadpisItem.pcbSpecificationValue = gr1PcbSpecTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "1b";
            osnNadpisItem.specificationValue = String.Empty;
            osnNadpisItem.perechenValue = "Перечень элементов";
            osnNadpisItem.vedomostValue = "Ведомость покупных изделий";
            osnNadpisItem.pcbSpecificationValue = String.Empty;
            project.SaveOsnNadpisItem(osnNadpisItem);

            string perechenType = "ПЭ3";
            switch (PerechenTypeComboBox.SelectedIndex)
            {
                case 0:
                    perechenType = " ПЭ1";
                    break;
                case 1:
                    perechenType = " ПЭ2";
                    break;
                case 2:
                    perechenType = " ПЭ3";
                    break;
                case 3:
                    perechenType = " ПЭ4";
                    break;
                case 4:
                    perechenType = " ПЭ5";
                    break;
                case 5:
                    perechenType = " ПЭ6";
                    break;
                case 6:
                    perechenType = " ПЭ7";
                    break;
                case 7:
                    perechenType = " ПЭ0";
                    break;
            }

            osnNadpisItem.grapha = "2";
            osnNadpisItem.specificationValue = gr2TextBox.Text;
            osnNadpisItem.perechenValue = gr2TextBox.Text + perechenType;
            osnNadpisItem.vedomostValue = gr2TextBox.Text + " ВП";
            osnNadpisItem.pcbSpecificationValue = gr2PcbSpecTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "3";
            osnNadpisItem.specificationValue = gr3TextBox.Text;
            osnNadpisItem.perechenValue = gr3TextBox.Text;
            osnNadpisItem.vedomostValue = gr3TextBox.Text;
            osnNadpisItem.pcbSpecificationValue = gr3TextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "4a";
            osnNadpisItem.specificationValue = gr4aTextBox.Text;
            osnNadpisItem.perechenValue = gr4aTextBox.Text;
            osnNadpisItem.vedomostValue = gr4aTextBox.Text;
            osnNadpisItem.pcbSpecificationValue = gr4aTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "4b";
            osnNadpisItem.specificationValue = gr4bTextBox.Text;
            osnNadpisItem.perechenValue = gr4bTextBox.Text;
            osnNadpisItem.vedomostValue = gr4bTextBox.Text;
            osnNadpisItem.pcbSpecificationValue = gr4bTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "4c";
            osnNadpisItem.specificationValue = gr4cTextBox.Text;
            osnNadpisItem.perechenValue = gr4cTextBox.Text;
            osnNadpisItem.vedomostValue = gr4cTextBox.Text;
            osnNadpisItem.pcbSpecificationValue = gr4cTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "5";
            osnNadpisItem.specificationValue = gr5TextBox.Text;
            osnNadpisItem.perechenValue = gr5TextBox.Text;
            osnNadpisItem.vedomostValue = gr5TextBox.Text;
            osnNadpisItem.pcbSpecificationValue = gr5TextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "6";
            osnNadpisItem.specificationValue = gr6TextBox.Text;
            osnNadpisItem.perechenValue = gr6TextBox.Text;
            osnNadpisItem.vedomostValue = gr6TextBox.Text;
            osnNadpisItem.pcbSpecificationValue = gr6TextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "7";
            osnNadpisItem.specificationValue = gr7TextBox.Text;
            osnNadpisItem.perechenValue = gr7TextBox.Text;
            osnNadpisItem.vedomostValue = gr7TextBox.Text;
            osnNadpisItem.pcbSpecificationValue = gr7TextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "8";
            osnNadpisItem.specificationValue = gr8TextBox.Text;
            osnNadpisItem.perechenValue = gr8TextBox.Text;
            osnNadpisItem.vedomostValue = gr8TextBox.Text;
            osnNadpisItem.pcbSpecificationValue = gr8TextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "9";
            osnNadpisItem.specificationValue = gr9TextBox.Text;
            osnNadpisItem.perechenValue = gr9TextBox.Text;
            osnNadpisItem.vedomostValue = gr9TextBox.Text;
            osnNadpisItem.pcbSpecificationValue = gr9TextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "10";
            osnNadpisItem.specificationValue = gr10TextBox.Text;
            osnNadpisItem.perechenValue = gr10TextBox.Text;
            osnNadpisItem.vedomostValue = gr10TextBox.Text;
            osnNadpisItem.pcbSpecificationValue = gr10TextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "11a";
            osnNadpisItem.specificationValue = gr11aTextBox.Text;
            osnNadpisItem.perechenValue = gr11aTextBox.Text;
            osnNadpisItem.vedomostValue = gr11aTextBox.Text;
            osnNadpisItem.pcbSpecificationValue = gr11aTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "11b";
            osnNadpisItem.specificationValue = gr11bTextBox.Text;
            osnNadpisItem.perechenValue = gr11bTextBox.Text;
            osnNadpisItem.vedomostValue = gr11bTextBox.Text;
            osnNadpisItem.pcbSpecificationValue = gr11bTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "11c";
            osnNadpisItem.specificationValue = gr11cTextBox.Text;
            osnNadpisItem.perechenValue = gr11cTextBox.Text;
            osnNadpisItem.vedomostValue = gr11cTextBox.Text;
            osnNadpisItem.pcbSpecificationValue = gr11cTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "11d";
            osnNadpisItem.specificationValue = gr11dTextBox.Text;
            osnNadpisItem.perechenValue = gr11dTextBox.Text;
            osnNadpisItem.vedomostValue = gr11dTextBox.Text;
            osnNadpisItem.pcbSpecificationValue = gr11dTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "11e";
            osnNadpisItem.specificationValue = gr11eTextBox.Text;
            osnNadpisItem.perechenValue = gr11eTextBox.Text;
            osnNadpisItem.vedomostValue = gr11eTextBox.Text;
            osnNadpisItem.pcbSpecificationValue = gr11eTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "14a";
            osnNadpisItem.specificationValue = gr14aTextBox.Text;
            osnNadpisItem.perechenValue = gr14aTextBox.Text;
            osnNadpisItem.vedomostValue = gr14aTextBox.Text;
            osnNadpisItem.pcbSpecificationValue = gr14aTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "15a";
            osnNadpisItem.specificationValue = gr15aTextBox.Text;
            osnNadpisItem.perechenValue = gr15aTextBox.Text;
            osnNadpisItem.vedomostValue = gr15aTextBox.Text;
            osnNadpisItem.pcbSpecificationValue = gr15aTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "16a";
            osnNadpisItem.specificationValue = gr16aTextBox.Text;
            osnNadpisItem.perechenValue = gr16aTextBox.Text;
            osnNadpisItem.vedomostValue = gr16aTextBox.Text;
            osnNadpisItem.pcbSpecificationValue = gr16aTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "19";
            osnNadpisItem.specificationValue = gr19SpecTextBox.Text;
            osnNadpisItem.perechenValue = gr19PerechenTextBox.Text;
            osnNadpisItem.vedomostValue = gr19VedomostTextBox.Text;
            osnNadpisItem.pcbSpecificationValue = gr19PcbSpecTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "21";
            osnNadpisItem.specificationValue = gr21SpecTextBox.Text;
            osnNadpisItem.perechenValue = gr21PerechenTextBox.Text;
            osnNadpisItem.vedomostValue = gr21VedomostTextBox.Text;
            osnNadpisItem.pcbSpecificationValue = gr21PcbSpecTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "22";
            osnNadpisItem.specificationValue = gr22SpecTextBox.Text;
            osnNadpisItem.perechenValue = gr22PerechenTextBox.Text;
            osnNadpisItem.vedomostValue = gr22VedomostTextBox.Text;
            osnNadpisItem.vedomostValue = gr22PcbSpecTextBox.Text;
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
            osnNadpisItem.pcbSpecificationValue = gr25PcbSpecTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "32";
            osnNadpisItem.specificationValue = "А4";
            osnNadpisItem.perechenValue = "А4";
            osnNadpisItem.vedomostValue = "А3";
            osnNadpisItem.pcbSpecificationValue = "А4";
            project.SaveOsnNadpisItem(osnNadpisItem);
        }

        private void PerechenTypeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}
