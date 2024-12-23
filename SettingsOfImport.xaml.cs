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
    /// Interaction logic for SettingsOfImport.xaml
    /// </summary>
    public partial class SettingsOfImport : Window
    {
        SettingsItem propNameItem = new SettingsItem();
        SettingsDB settingsDB = new SettingsDB();
        public SettingsOfImport()
        {
            InitializeComponent();
            string defaultDesignatorPropName = "Designator";
            string defaultKiCadDesignatorPropName = "Reference";
            string defaultNamePropName = "PartNumber";
            string defaultKiCadNamePropName = "PartNumber";
            string defaultDocumPropName = "Manufacturer";
            string defaultKiCadDocumPropName = "Manufacturer";
            string defaultNotePropName = "Note";
            string defaultKiCadNotePropName = "Note";
            string defaultPrjNameStr1PropName = "TitleStr1";
            string defaultKiCadPrjNameStr1PropName = "TitleStr1";
            string defaultPrjNameStr2PropName = "TitleStr2";
            string defaultKiCadPrjNameStr2PropName = "TitleStr2";
            string defaultPrjDocNumberPropName = "PrjDocNumber";
            string defaultKiCadPrjDocNumberPropName = "PrjDocNumber";
            string defaultPcbDocNumberPropName = "PcbDocNumber";
            string defaultKiCadPcbDocNumberPropName = "PcbDocNumber";
            string defaultPcbNameStr1PropName = "PcbTitle";
            string defaultKiCadPcbNameStr1PropName = "PcbTitle";
            string defaultPcbNameStr2PropName = String.Empty;
            string defaultKiCadPcbNameStr2PropName = String.Empty;
            string defaultDrawnByPropName = "DrawnBy";
            string defaultKiCadDrawnByPropName = "DrawnBy";
            string defaultCheckedByPropName = "CheckedBy";
            string defaultKiCadCheckedByPropName = "CheckedBy";
            string defaultDopPodpisantPropName = "SoglasBy";
            string defaultKiCadDopPodpisantPropName = "SoglasBy";
            string defaultNormInspectionPropName = "NormInspection";
            string defaultKiCadNormInspectionPropName = "NormInspection";
            string defaultApprovedByPropName = "ApprovedBy";
            string defaultKiCadApprovedByPropName = "ApprovedBy";
            string defaultCompanyPropName = "CompanyName";
            string defaultKiCadCompanyPropName = "CompanyName";

            #region Считывание наименований параметров компонентов из базы данных           

            // Чтение наименования свойства с позиционным обозначением
            if (settingsDB.GetItem("designatorPropName") == null)
            {
                propNameItem.name = "designatorPropName";
                propNameItem.valueString = defaultDesignatorPropName;
                settingsDB.SaveSettingItem(propNameItem);
            }
            else propNameItem = settingsDB.GetItem("designatorPropName");
            compDesignatorAltiumTextBox.Text = propNameItem.valueString;

            if (settingsDB.GetItem("designatorPropNameKiCad") == null)
            {
                propNameItem.name = "designatorPropNameKiCad";
                propNameItem.valueString = defaultKiCadDesignatorPropName;
                settingsDB.SaveSettingItem(propNameItem);
            }
            else propNameItem = settingsDB.GetItem("designatorPropNameKiCad");
            compDesignatorKiCadTextBox.Text = propNameItem.valueString;


            // Чтение наименования свойства с наименованием компонента
            if (settingsDB.GetItem("namePropName") == null)
            {
                propNameItem.name = "namePropName";
                propNameItem.valueString = defaultNamePropName;
                settingsDB.SaveSettingItem(propNameItem);
            }
            else propNameItem = settingsDB.GetItem("namePropName");
            compNameAltiumTextBox.Text = propNameItem.valueString;

            if (settingsDB.GetItem("namePropNameKiCad") == null)
            {
                propNameItem.name = "namePropNameKiCad";
                propNameItem.valueString = defaultKiCadNamePropName;
                settingsDB.SaveSettingItem(propNameItem);
            }
            else propNameItem = settingsDB.GetItem("namePropNameKiCad");
            compNameKiCadTextBox.Text = propNameItem.valueString;

            // Чтение наименования свойства с документом на поставку компонента
            if (settingsDB.GetItem("documPropName") == null)
            {
                propNameItem.name = "documPropName";
                propNameItem.valueString = defaultDocumPropName;
                settingsDB.SaveSettingItem(propNameItem);
            }
            else propNameItem = settingsDB.GetItem("documPropName");
            compDocumAltiumTextBox.Text = propNameItem.valueString;

            if (settingsDB.GetItem("documPropNameKiCad") == null)
            {
                propNameItem.name = "documPropNameKiCad";
                propNameItem.valueString = defaultKiCadDocumPropName;
                settingsDB.SaveSettingItem(propNameItem);
            }
            else propNameItem = settingsDB.GetItem("documPropNameKiCad");
            compDocumKiCadTextBox.Text = propNameItem.valueString;

            // Чтение наименования свойства с комментарием
            if (settingsDB.GetItem("notePropName") == null)
            {
                propNameItem.name = "notePropName";
                propNameItem.valueString = defaultNotePropName;
                settingsDB.SaveSettingItem(propNameItem);
            }
            else propNameItem = settingsDB.GetItem("notePropName");
            compNoteAltiumTextBox.Text = propNameItem.valueString;

            if (settingsDB.GetItem("notePropNameKiCad") == null)
            {
                propNameItem.name = "notePropNameKiCad";
                propNameItem.valueString = defaultKiCadNotePropName;
                settingsDB.SaveSettingItem(propNameItem);
            }
            else propNameItem = settingsDB.GetItem("notePropNameKiCad");
            compNoteKiCadTextBox.Text = propNameItem.valueString;

            // Чтение наименования свойства с наименованием децимального номера изделия
            if (settingsDB.GetItem("prjDocNumberPropName") == null)
            {
                propNameItem.name = "prjDocNumberPropName";
                propNameItem.valueString = defaultPrjDocNumberPropName;
                settingsDB.SaveSettingItem(propNameItem);
            }
            else propNameItem = settingsDB.GetItem("prjDocNumberPropName");
            prjDocNumberAltiumTextBox.Text = propNameItem.valueString;

            if (settingsDB.GetItem("prjDocNumberPropNameKiCad") == null)
            {
                propNameItem.name = "prjDocNumberPropNameKiCad";
                propNameItem.valueString = defaultKiCadPrjDocNumberPropName;
                settingsDB.SaveSettingItem(propNameItem);
            }
            else propNameItem = settingsDB.GetItem("prjDocNumberPropNameKiCad");
            prjDocNumberKiCadTextBox.Text = propNameItem.valueString;

            // Чтение наименования свойства с первой строкой наименования изделия
            if (settingsDB.GetItem("prjNameStr1PropName") == null)
            {
                propNameItem.name = "prjNameStr1PropName";
                propNameItem.valueString = defaultPrjNameStr1PropName;
                settingsDB.SaveSettingItem(propNameItem);
            }
            else propNameItem = settingsDB.GetItem("prjNameStr1PropName");
            prjNameStr1AltiumTextBox.Text = propNameItem.valueString;

            if (settingsDB.GetItem("prjNameStr1PropNameKiCad") == null)
            {
                propNameItem.name = "prjNameStr1PropNameKiCad";
                propNameItem.valueString = defaultKiCadPrjNameStr1PropName;
                settingsDB.SaveSettingItem(propNameItem);
            }
            else propNameItem = settingsDB.GetItem("prjNameStr1PropNameKiCad");
            prjNameStr1KiCadTextBox.Text = propNameItem.valueString;

            // Чтение наименования свойства со второй строкой наименования изделия
            if (settingsDB.GetItem("prjNameStr2PropName") == null)
            {
                propNameItem.name = "prjNameStr2PropName";
                propNameItem.valueString = defaultPrjNameStr2PropName;
                settingsDB.SaveSettingItem(propNameItem);
            }
            else propNameItem = settingsDB.GetItem("prjNameStr2PropName");
            prjNameStr2AltiumTextBox.Text = propNameItem.valueString;

            if (settingsDB.GetItem("prjNameStr2PropNameKiCad") == null)
            {
                propNameItem.name = "prjNameStr2PropNameKiCad";
                propNameItem.valueString = defaultKiCadPrjNameStr2PropName;
                settingsDB.SaveSettingItem(propNameItem);
            }
            else propNameItem = settingsDB.GetItem("prjNameStr2PropNameKiCad");
            prjNameStr2KiCadTextBox.Text = propNameItem.valueString;

            // Чтение наименования свойства с наименованием децимального номера платы
            if (settingsDB.GetItem("pcbDocNumberPropName") == null)
            {
                propNameItem.name = "pcbDocNumberPropName";
                propNameItem.valueString = defaultPcbDocNumberPropName;
                settingsDB.SaveSettingItem(propNameItem);
            }
            else propNameItem = settingsDB.GetItem("pcbDocNumberPropName");
            pcbDocNumberAltiumTextBox.Text = propNameItem.valueString;

            if (settingsDB.GetItem("pcbDocNumberPropNameKiCad") == null)
            {
                propNameItem.name = "pcbDocNumberPropNameKiCad";
                propNameItem.valueString = defaultKiCadPcbDocNumberPropName;
                settingsDB.SaveSettingItem(propNameItem);
            }
            else propNameItem = settingsDB.GetItem("pcbDocNumberPropNameKiCad");
            pcbDocNumberKiCadTextBox.Text = propNameItem.valueString;

            // Чтение наименования свойства с первой строкой наименования платы
            if (settingsDB.GetItem("pcbNameStr1PropName") == null)
            {
                propNameItem.name = "pcbNameStr1PropName";
                propNameItem.valueString = defaultPcbNameStr1PropName;
                settingsDB.SaveSettingItem(propNameItem);
            }
            else propNameItem = settingsDB.GetItem("pcbNameStr1PropName");
            pcbNameStr1AltiumTextBox.Text = propNameItem.valueString;

            if (settingsDB.GetItem("pcbNameStr1PropNameKiCad") == null)
            {
                propNameItem.name = "pcbNameStr1PropNameKiCad";
                propNameItem.valueString = defaultKiCadPcbNameStr1PropName;
                settingsDB.SaveSettingItem(propNameItem);
            }
            else propNameItem = settingsDB.GetItem("pcbNameStr1PropNameKiCad");
            pcbNameStr1KiCadTextBox.Text = propNameItem.valueString;

            // Чтение наименования свойства со второй строкой наименования платы
            if (settingsDB.GetItem("pcbNameStr2PropName") == null)
            {
                propNameItem.name = "pcbNameStr2PropName";
                propNameItem.valueString = defaultPcbNameStr2PropName;
                settingsDB.SaveSettingItem(propNameItem);
            }
            else propNameItem = settingsDB.GetItem("pcbNameStr2PropName");
            pcbNameStr2AltiumTextBox.Text = propNameItem.valueString;

            if (settingsDB.GetItem("pcbNameStr2PropNameKiCad") == null)
            {
                propNameItem.name = "pcbNameStr2PropNameKiCad";
                propNameItem.valueString = defaultKiCadPcbNameStr2PropName;
                settingsDB.SaveSettingItem(propNameItem);
            }
            else propNameItem = settingsDB.GetItem("pcbNameStr2PropNameKiCad");
            pcbNameStr2KiCadTextBox.Text = propNameItem.valueString;

            // Чтение наименования свойства с фамилией разработчика
            if (settingsDB.GetItem("drawnByPropName") == null)
            {
                propNameItem.name = "drawnByPropName";
                propNameItem.valueString = defaultDrawnByPropName;
                settingsDB.SaveSettingItem(propNameItem);
            }
            else propNameItem = settingsDB.GetItem("drawnByPropName");
            drawnByAltiumTextBox.Text = propNameItem.valueString;

            if (settingsDB.GetItem("drawnByPropNameKiCad") == null)
            {
                propNameItem.name = "drawnByPropNameKiCad";
                propNameItem.valueString = defaultKiCadDrawnByPropName;
                settingsDB.SaveSettingItem(propNameItem);
            }
            else propNameItem = settingsDB.GetItem("drawnByPropNameKiCad");
            drawnByKiCadTextBox.Text = propNameItem.valueString;

            // Чтение наименования свойства с фамилией проверяющего
            if (settingsDB.GetItem("checkedByPropName") == null)
            {
                propNameItem.name = "checkedByPropName";
                propNameItem.valueString = defaultCheckedByPropName;
                settingsDB.SaveSettingItem(propNameItem);
            }
            else propNameItem = settingsDB.GetItem("checkedByPropName");
            checkedByAltiumTextBox.Text = propNameItem.valueString;

            if (settingsDB.GetItem("checkedByPropNameKiCad") == null)
            {
                propNameItem.name = "checkedByPropNameKiCad";
                propNameItem.valueString = defaultKiCadCheckedByPropName;
                settingsDB.SaveSettingItem(propNameItem);
            }
            else propNameItem = settingsDB.GetItem("checkedByPropNameKiCad");
            checkedByKiCadTextBox.Text = propNameItem.valueString;

            // Чтение наименования свойства с фамилией доп. подписанта
            if (settingsDB.GetItem("dopPodpisantPropName") == null)
            {
                propNameItem.name = "dopPodpisantPropName";
                propNameItem.valueString = defaultDopPodpisantPropName;
                settingsDB.SaveSettingItem(propNameItem);
            }
            else propNameItem = settingsDB.GetItem("dopPodpisantPropName");
            dopPodpisantAltiumTextBox.Text = propNameItem.valueString;

            if (settingsDB.GetItem("dopPodpisantPropNameKiCad") == null)
            {
                propNameItem.name = "dopPodpisantPropNameKiCad";
                propNameItem.valueString = defaultKiCadDopPodpisantPropName;
                settingsDB.SaveSettingItem(propNameItem);
            }
            else propNameItem = settingsDB.GetItem("dopPodpisantPropNameKiCad");
            dopPodpisantKiCadTextBox.Text = propNameItem.valueString;
            

            // Чтение наименования свойства с фамилией нормоконтролёра
            if (settingsDB.GetItem("normInspectionPropName") == null)
            {
                propNameItem.name = "normInspectionPropName";
                propNameItem.valueString = defaultNormInspectionPropName;
                settingsDB.SaveSettingItem(propNameItem);
            }
            else propNameItem = settingsDB.GetItem("normInspectionPropName");
            normInspectionAltiumTextBox.Text = propNameItem.valueString;

            if (settingsDB.GetItem("normInspectionPropNameKiCad") == null)
            {
                propNameItem.name = "normInspectionPropNameKiCad";
                propNameItem.valueString = defaultKiCadNormInspectionPropName;
                settingsDB.SaveSettingItem(propNameItem);
            }
            else propNameItem = settingsDB.GetItem("normInspectionPropNameKiCad");
            normInspectionKiCadTextBox.Text = propNameItem.valueString;

            // Чтение наименования свойства с фамилией утверждиющего
            if (settingsDB.GetItem("approvedByPropName") == null)
            {
                propNameItem.name = "approvedByPropName";
                propNameItem.valueString = defaultApprovedByPropName;
                settingsDB.SaveSettingItem(propNameItem);
            }
            else propNameItem = settingsDB.GetItem("approvedByPropName");
            approvedByAltiumTextBox.Text = propNameItem.valueString;

            if (settingsDB.GetItem("approvedByPropNameKiCad") == null)
            {
                propNameItem.name = "approvedByPropNameKiCad";
                propNameItem.valueString = defaultKiCadApprovedByPropName;
                settingsDB.SaveSettingItem(propNameItem);
            }
            else propNameItem = settingsDB.GetItem("approvedByPropNameKiCad");
            approvedByKiCadTextBox.Text = propNameItem.valueString;

            

            // Чтение наименования свойства с наименованием компании
            if (settingsDB.GetItem("companyPropName") == null)
            {
                propNameItem.name = "companyPropName";
                propNameItem.valueString = defaultCompanyPropName;
                settingsDB.SaveSettingItem(propNameItem);
            }
            else propNameItem = settingsDB.GetItem("companyPropName");
            companyNameAltiumTextBox.Text = propNameItem.valueString;

            if (settingsDB.GetItem("companyPropNameKiCad") == null)
            {
                propNameItem.name = "companyPropNameKiCad";
                propNameItem.valueString = defaultKiCadCompanyPropName;
                settingsDB.SaveSettingItem(propNameItem);
            }
            else propNameItem = settingsDB.GetItem("companyPropNameKiCad");
            companyNameKiCadTextBox.Text = propNameItem.valueString;

            #endregion
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            // Запись наименования свойства с позиционным обозначением
            propNameItem.name = "designatorPropName";
            propNameItem.valueString = compDesignatorAltiumTextBox.Text;
            settingsDB.SaveSettingItem(propNameItem);

            propNameItem.name = "designatorPropNameKiCad";
            propNameItem.valueString = compDesignatorKiCadTextBox.Text;
            settingsDB.SaveSettingItem(propNameItem);

            // Запись наименования свойства с наименованием компонента
            propNameItem.name = "namePropName";
            propNameItem.valueString = compNameAltiumTextBox.Text;
            settingsDB.SaveSettingItem(propNameItem);

            propNameItem.name = "namePropNameKiCad";
            propNameItem.valueString = compNameKiCadTextBox.Text;
            settingsDB.SaveSettingItem(propNameItem);

            // Запись наименования свойства с документом на поставку компонента
            propNameItem.name = "documPropName";
            propNameItem.valueString = compDocumAltiumTextBox.Text;
            settingsDB.SaveSettingItem(propNameItem);

            propNameItem.name = "documPropNameKiCad";
            propNameItem.valueString = compDocumKiCadTextBox.Text;
            settingsDB.SaveSettingItem(propNameItem);

            // Запись наименования свойства с комментарием
            propNameItem.name = "notePropName";
            propNameItem.valueString = compNoteAltiumTextBox.Text;
            settingsDB.SaveSettingItem(propNameItem);

            propNameItem.name = "notePropNameKiCad";
            propNameItem.valueString = compNoteKiCadTextBox.Text;
            settingsDB.SaveSettingItem(propNameItem);

            // Запись наименования свойства с наименованием децимального номера изделия
            propNameItem.name = "prjDocNumberPropName";
            propNameItem.valueString = prjDocNumberAltiumTextBox.Text;
            settingsDB.SaveSettingItem(propNameItem);

            propNameItem.name = "prjDocNumberPropNameKiCad";
            propNameItem.valueString = prjDocNumberKiCadTextBox.Text;
            settingsDB.SaveSettingItem(propNameItem);

            // Запись наименования свойства с первой строкой наименования изделия
            propNameItem.name = "prjNameStr1PropName";
            propNameItem.valueString = prjNameStr1AltiumTextBox.Text;
            settingsDB.SaveSettingItem(propNameItem);

            propNameItem.name = "prjNameStr1PropNameKiCad";
            propNameItem.valueString = prjNameStr1KiCadTextBox.Text;
            settingsDB.SaveSettingItem(propNameItem);

            // Запись наименования свойства со второй строкой наименования изделия
            propNameItem.name = "prjNameStr2PropName";
            propNameItem.valueString = prjNameStr2AltiumTextBox.Text;
            settingsDB.SaveSettingItem(propNameItem);

            propNameItem.name = "prjNameStr2PropNameKiCad";
            propNameItem.valueString = prjNameStr2KiCadTextBox.Text;
            settingsDB.SaveSettingItem(propNameItem);

            // Запись наименования свойства с наименованием децимального номера платы
            propNameItem.name = "prjPcbNumberPropName";
            propNameItem.valueString = pcbDocNumberAltiumTextBox.Text;
            settingsDB.SaveSettingItem(propNameItem);

            propNameItem.name = "prjPcbNumberPropNameKiCad";
            propNameItem.valueString = pcbDocNumberKiCadTextBox.Text;
            settingsDB.SaveSettingItem(propNameItem);

            // Запись наименования свойства с первой строкой наименования платы
            propNameItem.name = "pcbNameStr1PropName";
            propNameItem.valueString = pcbNameStr1AltiumTextBox.Text;
            settingsDB.SaveSettingItem(propNameItem);

            propNameItem.name = "pcbNameStr1PropNameKiCad";
            propNameItem.valueString = pcbNameStr1KiCadTextBox.Text;
            settingsDB.SaveSettingItem(propNameItem);

            // Запись наименования свойства со второй строкой наименования платы
            propNameItem.name = "pcbNameStr2PropName";
            propNameItem.valueString = pcbNameStr2AltiumTextBox.Text;
            settingsDB.SaveSettingItem(propNameItem);

            propNameItem.name = "pcbNameStr2PropNameKiCad";
            propNameItem.valueString = pcbNameStr2KiCadTextBox.Text;
            settingsDB.SaveSettingItem(propNameItem);

            // Запись наименования свойства с фамилией разработчика
            propNameItem.name = "drawnByPropName";
            propNameItem.valueString = drawnByAltiumTextBox.Text;
            settingsDB.SaveSettingItem(propNameItem);

            propNameItem.name = "drawnByPropNameKiCad";
            propNameItem.valueString = drawnByKiCadTextBox.Text;
            settingsDB.SaveSettingItem(propNameItem);

            // Запись наименования свойства с фамилией проверяющего
            propNameItem.name = "checkedByPropName";
            propNameItem.valueString = checkedByAltiumTextBox.Text;
            settingsDB.SaveSettingItem(propNameItem);

            propNameItem.name = "checkedByPropNameKiCad";
            propNameItem.valueString = checkedByKiCadTextBox.Text;
            settingsDB.SaveSettingItem(propNameItem);

            // Запись наименования свойства с фамилией доп. подписанта
            propNameItem.name = "dopPodpisantPropName";
            propNameItem.valueString = dopPodpisantAltiumTextBox.Text;
            settingsDB.SaveSettingItem(propNameItem);

            propNameItem.name = "dopPodpisantPropNameKiCad";
            propNameItem.valueString = dopPodpisantKiCadTextBox.Text;
            settingsDB.SaveSettingItem(propNameItem);
            
            // Запись наименования свойства с фамилией утверждающего
            propNameItem.name = "approvedByPropName";
            propNameItem.valueString = approvedByAltiumTextBox.Text;
            settingsDB.SaveSettingItem(propNameItem);

            propNameItem.name = "approvedByPropNameKiCad";
            propNameItem.valueString = approvedByKiCadTextBox.Text;
            settingsDB.SaveSettingItem(propNameItem);

            // Запись наименования свойства с наименованием организации
            propNameItem.name = "companyPropName";
            propNameItem.valueString = companyNameAltiumTextBox.Text;
            settingsDB.SaveSettingItem(propNameItem);

            propNameItem.name = "companyPropNameKiCad";
            propNameItem.valueString = companyNameKiCadTextBox.Text;
            settingsDB.SaveSettingItem(propNameItem);

            this.DialogResult = true;
        }
    }
}
