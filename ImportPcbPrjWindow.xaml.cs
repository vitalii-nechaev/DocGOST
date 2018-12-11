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
using System.Windows.Controls;
namespace DocGOST
{
    /// <summary>
    /// Логика взаимодействия для ImportPcbPrjWindow.xaml
    /// </summary>
    public partial class ImportPcbPrjWindow : Window
    {
        public ImportPcbPrjWindow()
        {
            InitializeComponent();
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {

            this.DialogResult = true;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (designatorComboBox.HasItems)
            {
                if ((designatorComboBox.SelectedIndex == 0) | (nameComboBox.SelectedIndex == 0))
                {
                    nextButton.ToolTip = "Свойства \"Поз. обозначение\" и \"Наименование должны быть заданы\"";
                    nextButton.IsEnabled = false;
                }
                else
                {
                    nextButton.ToolTip = "Приступить к импорту данных";
                    nextButton.IsEnabled = true;
                }
            }
            
                
        }
    }
}
