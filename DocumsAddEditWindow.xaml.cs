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

using System.Windows;
using DocGOST.Data;

namespace DocGOST
{
    /// <summary>
    /// Interaction logic for DocumsAddEditWindow.xaml
    /// </summary>
    public partial class DocumsAddEditWindow : Window
    {
        private bool isDocums4Pcb=false;
        public DocumsAddEditWindow(bool isDocums4Pcb)
        {
            InitializeComponent();
            this.isDocums4Pcb = isDocums4Pcb;
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            DocumsItem dItem = new DocumsItem();
            dItem.code = codeTextBox.Text;
            dItem.name = nameTextBox.Text;
            dItem.format = formatTextBox.Text;
            dItem.note= noteTextBox.Text;
            DocumsDB documsDB = new DocumsDB(isDocums4Pcb);
            documsDB.SaveDocumsItem(dItem);
            this.DialogResult = true;

        }
    }
}
