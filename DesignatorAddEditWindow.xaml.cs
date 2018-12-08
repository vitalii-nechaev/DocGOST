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
using DocGOST.Data;

namespace DocGOST
{
    /// <summary>
    /// Логика взаимодействия для DesignatorAddEditWindow.xaml
    /// </summary>
    public partial class DesignatorAddEditWindow : Window
    {
        public DesignatorAddEditWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Запись введённого в окне значения в базу данных при нажатии на "Сохранить"
        /// </summary>
        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            DesignatorDescriptionItem ddItem = new DesignatorDescriptionItem();
            ddItem.Designator = designatorTextBox.Text;
            ddItem.Group = groupTextBox.Text;
            ddItem.GroupPlural = groupPluralTextBox.Text;
            DesignatorDB desDescr = new DesignatorDB();
            desDescr.SaveDesignatorItem(ddItem);
            this.DialogResult = true; 
            
        }
    }
}
