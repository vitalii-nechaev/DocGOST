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

using DocGOST.Data;
using System.Windows;

namespace DocGOST
{
    /// <summary>
    /// Логика взаимодействия для EditSpecItemWindow.xaml
    /// </summary>
    public partial class EditSpecItemWindow : Window
    {
        public EditSpecItemWindow(string projectPath, int strNum, int currentSave, int nextSave)
        {
            InitializeComponent();

            project = new ProjectDB(projectPath);

            specItem = new SpecificationItem();

            specItem = project.GetSpecItem(strNum, currentSave);

            specItem.id = (new Global()).makeID(strNum, nextSave);

            formatTextBox.Text = specItem.format;
            zonaTextBox.Text = specItem.zona;
            positionTextBox.Text = specItem.position;
            oboznachenieTextBox.Text = specItem.oboznachenie;
            nameTextBox.Text = specItem.name;
            quantityTextBox.Text = specItem.quantity;
            noteTextBox.Text = specItem.note;
        }

        SpecificationItem specItem;
        ProjectDB project;

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            if ((formatTextBox.Text != specItem.format) |
                (zonaTextBox.Text != specItem.zona) |
                (positionTextBox.Text != specItem.quantity) |
                (oboznachenieTextBox.Text != specItem.note) |
                (nameTextBox.Text != specItem.quantity) |
                (quantityTextBox.Text != specItem.quantity) |
                (noteTextBox.Text != specItem.quantity))
            {
                specItem.format = formatTextBox.Text;
                specItem.zona = zonaTextBox.Text;
                specItem.position = positionTextBox.Text;
                specItem.oboznachenie = oboznachenieTextBox.Text;
                specItem.name = nameTextBox.Text;
                specItem.quantity = quantityTextBox.Text;
                specItem.note = noteTextBox.Text;

                project.AddSpecItem(specItem);

                this.DialogResult = true;
            }
        }
    }
}
