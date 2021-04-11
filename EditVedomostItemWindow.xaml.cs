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
    /// Логика взаимодействия для EditVedomostItemWindow.xaml
    /// </summary>
    public partial class EditVedomostItemWindow : Window
    {
        public EditVedomostItemWindow(string projectPath, int strNum, int currentSave, int nextSave)
        {
            InitializeComponent();
            
            project = new ProjectDB(projectPath);

            vedomostItem = new VedomostItem();

            vedomostItem = project.GetVedomostItem(strNum, currentSave);

            vedomostItem.id = (new Global()).makeID(strNum, nextSave);

            nameTextBox.Text = vedomostItem.name;
            kodTextBox.Text = vedomostItem.kod;
            documTextBox.Text = vedomostItem.docum;
            supplierTextBox.Text = vedomostItem.supplier;
            supplierRefTextBox.Text = vedomostItem.supplierRef;
            quantityIzdelieTextBox.Text = vedomostItem.quantityIzdelie;
            quantityRegulTextBox.Text = vedomostItem.quantityRegul;
            noteTextBox.Text = vedomostItem.note;
            isNameUnderlinedCheckBox.IsChecked = vedomostItem.isNameUnderlined;            
        }

        VedomostItem vedomostItem;
        ProjectDB project;

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            if ((nameTextBox.Text != vedomostItem.name) |
                (kodTextBox.Text != vedomostItem.kod) |
                (documTextBox.Text != vedomostItem.docum) |
                (supplierTextBox.Text != vedomostItem.supplier) |
                (supplierRefTextBox.Text != vedomostItem.supplierRef) |
                (quantityIzdelieTextBox.Text != vedomostItem.quantityIzdelie) |
                (quantityRegulTextBox.Text != vedomostItem.quantityRegul) |
                (noteTextBox.Text != vedomostItem.note) |
                ((bool)isNameUnderlinedCheckBox.IsChecked != vedomostItem.isNameUnderlined))
            {
                vedomostItem.name = nameTextBox.Text;
                vedomostItem.kod = kodTextBox.Text;
                vedomostItem.docum = documTextBox.Text;
                vedomostItem.supplier = supplierTextBox.Text;
                vedomostItem.supplierRef = supplierRefTextBox.Text;
                vedomostItem.note = noteTextBox.Text;

                int quantityIzdelie=-1;

                while (quantityIzdelie<0)
                {
                    if (quantityIzdelieTextBox.Text == string.Empty) quantityIzdelie = 0;
                    else if (int.TryParse(quantityIzdelieTextBox.Text, out quantityIzdelie) == false) quantityIzdelieTextBox.Text = vedomostItem.quantityIzdelie;
                }
                vedomostItem.quantityIzdelie = quantityIzdelieTextBox.Text;

                int quantityRegul = -1;

                while (quantityRegul < 0)
                {
                    if (quantityRegulTextBox.Text == string.Empty) quantityRegul = 0;
                    else if (int.TryParse(quantityRegulTextBox.Text, out quantityRegul) == false) quantityRegulTextBox.Text = vedomostItem.quantityRegul;
                }
                vedomostItem.quantityRegul = quantityRegulTextBox.Text;
                

                vedomostItem.quantityTotal = (quantityIzdelie + quantityRegul).ToString();
                                
                vedomostItem.isNameUnderlined = (bool)isNameUnderlinedCheckBox.IsChecked;

                project.AddVedomostItem(vedomostItem);

                this.DialogResult = true;
            }
        }
    }
}
