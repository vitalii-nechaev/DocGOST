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
    /// Логика взаимодействия для EditPerechenItemWindow.xaml
    /// </summary>
    public partial class EditPerechenItemWindow : Window
    {
        public EditPerechenItemWindow(string projectPath, int strNum, int currentSave, int nextSave)
        {
            InitializeComponent();
           

            project = new ProjectDB(projectPath);

            perItem = new PerechenItem();

            perItem = project.GetPerechenItem(strNum, currentSave);

            perItem.id = (new Global()).makeID(strNum, nextSave);

            designatorTextBox.Text = perItem.designator;
            nameTextBox.Text = perItem.name;
            quantityTextBox.Text = perItem.quantity;
            noteTextBox.Text = perItem.note;
            isNameUnderlinedCheckBox.IsChecked = perItem.isNameUnderlined;
        }

        PerechenItem perItem;
        ProjectDB project;        

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            if ((designatorTextBox.Text != perItem.designator) |
                (nameTextBox.Text != perItem.name) |
                (quantityTextBox.Text != perItem.quantity) |
                (noteTextBox.Text != perItem.note)|
                ((bool)isNameUnderlinedCheckBox.IsChecked != perItem.isNameUnderlined))
            {
                perItem.designator = designatorTextBox.Text;
                perItem.name = nameTextBox.Text;
                perItem.quantity = quantityTextBox.Text;
                perItem.note = noteTextBox.Text;
                perItem.isNameUnderlined = (bool)isNameUnderlinedCheckBox.IsChecked;
                
                project.AddPerechenItem(perItem);

                this.DialogResult = true;
            }
        }


    }
}
