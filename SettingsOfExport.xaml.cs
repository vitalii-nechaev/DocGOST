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

using DocGOST.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DocGOST
{
    /// <summary>
    /// Interaction logic for SettingsOfExport.xaml
    /// </summary>
    public partial class SettingsOfExport : Window
    {
        public SettingsOfExport()
        {
            InitializeComponent();

            SettingsItem propNameItem = new SettingsItem();
            SettingsDB settingsDB = new SettingsDB();

            if (settingsDB.GetItem("drawGraf30") == null)
            {
                propNameItem.name = "drawGraf30";
                propNameItem.valueString = "True";
                settingsDB.SaveSettingItem(propNameItem);
            }
            else if (settingsDB.GetItem("drawGraf30").valueString == "False")
            {
                drawGraf30Checkbox.IsChecked = false;
            }

            if (settingsDB.GetItem("outputFolder") == null)
            {
                propNameItem.name = "outputFolder";
                propNameItem.valueString = "DocGostLoc/КД/DocNumber+' '+DocName";
                outputFolderTextBox.Text = propNameItem.valueString;
                settingsDB.SaveSettingItem(propNameItem);
            }
            else
            {
                outputFolderTextBox.Text = settingsDB.GetItem("outputFolder").valueString;
            }

            if (settingsDB.GetItem("outputFile") == null)
            {
                propNameItem.name = "outputFile";
                propNameItem.valueString = "DocNumber+' '+DocType+' '+DocName";
                outputFileTextBox.Text = propNameItem.valueString;
                settingsDB.SaveSettingItem(propNameItem);
            }
            else
            {
                outputFileTextBox.Text = settingsDB.GetItem("outputFile").valueString;
            }
        }

        private void drawGraf30Checkbox_Checked(object sender, RoutedEventArgs e)
        {
            SettingsItem propNameItem = new SettingsItem();
            SettingsDB settingsDB = new SettingsDB();

            if (drawGraf30Checkbox.IsChecked == false)
            {
                propNameItem.name = "drawGraf30";
                propNameItem.valueString = "False";
                settingsDB.SaveSettingItem(propNameItem);
            }
            else
            {
                propNameItem.name = "drawGraf30";
                propNameItem.valueString = "True";
                settingsDB.SaveSettingItem(propNameItem);
            }
        }
        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            SettingsItem propNameItem = new SettingsItem();
            SettingsDB settingsDB = new SettingsDB();

            propNameItem.name = "outputFolder";
            propNameItem.valueString = outputFolderTextBox.Text;
            settingsDB.SaveSettingItem(propNameItem);

            propNameItem.name = "outputFile";
            propNameItem.valueString = outputFileTextBox.Text;
            settingsDB.SaveSettingItem(propNameItem);


            if (drawGraf30Checkbox.IsChecked == false)
            {
                propNameItem.name = "drawGraf30";
                propNameItem.valueString = "False";
                settingsDB.SaveSettingItem(propNameItem);
            }
            else
            {
                propNameItem.name = "drawGraf30";
                propNameItem.valueString = "True";
                settingsDB.SaveSettingItem(propNameItem);
            }

            this.DialogResult = true;
        }

        
    }
}
