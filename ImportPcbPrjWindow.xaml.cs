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
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;


namespace DocGOST
{
    /// <summary>
    /// Логика взаимодействия для ImportPcbPrjWindow.xaml
    /// </summary>
    public partial class ImportPcbPrjWindow : Window
    {
        SettingsItem propNameItem = new SettingsItem();
        SettingsDB settingsDB = new SettingsDB();

        public ImportPcbPrjWindow()
        {
            InitializeComponent();
        }

        public List<List<string[]>> prjParamsVariantList;// Параметры проекта для возможности считывания 
        public bool isAltiumProject = true;
        public string prjName = "Наименование"; // Наименование изделия
        public string pcbName = "Плата печатная"; // Наименование платы
        public string drawnBy = String.Empty; // Разраб.
        public string checkedBy = String.Empty; // Пров.
        public string dopPodpisant = String.Empty; // Доп. подписант
        public string normInspection = String.Empty; // Нормоконтролёр
        public string approvedBy=String.Empty; // Утв.
        public string companyName = String.Empty; // Наименование организации

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
                    nextButton.ToolTip = "Свойства \"Поз. обозначение\" и \"Наименование\" должны быть заданы";
                    nextButton.IsEnabled = false;
                }
                else
                {
                    nextButton.ToolTip = "Приступить к импорту данных";
                    nextButton.IsEnabled = true;
                }
            }
            
                
        }

        private void Next_1_Click(object sender, RoutedEventArgs e)
        {
            
            variantsGrid.Visibility = Visibility.Hidden;
            propertiesGrid.Visibility = Visibility.Visible;
            
        }

        List<string[]> currentPrjParamsList = new List<string[]>();

        //Децимальный номер изделия
        private void Next_2_Click(object sender, RoutedEventArgs e)
        {
            

            List<string> propNameList = new List<string>();

            currentPrjParamsList = new List<string[]>();

            if (prjParamsVariantList.Count > 0)
            for (int i = 0; i < prjParamsVariantList[0].Count; i++)
                {
                    propNameList.Add(prjParamsVariantList[0][i][0]);
                    currentPrjParamsList.Add(prjParamsVariantList[0][i]);
                }


            if (variantSelectionComboBox.SelectedIndex > 0)
            {
                if (prjParamsVariantList.Count>1) //если в варианте изменяются параметры
                for (int j = 0; j < prjParamsVariantList[variantSelectionComboBox.SelectedIndex].Count; j++) 
                {
                    bool isNewName = true;
                    for (int i = 0; i < prjParamsVariantList[0].Count; i++)
                    {
                        if (prjParamsVariantList[0][i][0] == prjParamsVariantList[variantSelectionComboBox.SelectedIndex][j][0])
                        {
                            currentPrjParamsList[i] = prjParamsVariantList[variantSelectionComboBox.SelectedIndex][j];
                            isNewName = false;
                        }
                    }
                    if (isNewName) 
                    {
                        currentPrjParamsList.Add(prjParamsVariantList[variantSelectionComboBox.SelectedIndex][j]);
                        propNameList.Add(prjParamsVariantList[variantSelectionComboBox.SelectedIndex][j][0]);
                    }
                }
            }

            prjNumberSelectionComboBox.ItemsSource = propNameList;
           
            //Децимальный номер изделия
            string defaultPrjDocNumberPropName = "PrjDocNumber";
            SettingsItem propNameItem = new SettingsItem();
            SettingsDB settingsDB = new SettingsDB();

            if (isAltiumProject)
            {
                if (settingsDB.GetItem("prjDocNumberPropName") == null)
                {
                    propNameItem.name = "prjDocNumberPropName";
                    propNameItem.valueString = defaultPrjDocNumberPropName;
                    settingsDB.SaveSettingItem(propNameItem);
                }
                else propNameItem = settingsDB.GetItem("prjDocNumberPropName");               
            }
            else // if KiCad Project
            {
                if (settingsDB.GetItem("prjDocNumberPropNameKiCad") == null)
                {
                    propNameItem.name = "prjDocNumberPropNameKiCad";
                    propNameItem.valueString = defaultPrjDocNumberPropName;
                    settingsDB.SaveSettingItem(propNameItem);
                }
                else propNameItem = settingsDB.GetItem("prjDocNumberPropNameKiCad");               
            }
            defaultPrjDocNumberPropName = propNameItem.valueString;

            if (propNameList.Contains(defaultPrjDocNumberPropName)) prjNumberSelectionComboBox.SelectedIndex = propNameList.FindIndex(x => x == defaultPrjDocNumberPropName);
            else prjNumberSelectionComboBox.SelectedIndex = 0;

            //Наименование изделия:
            // Чтение наименования свойства с первой строкой наименования изделия
            string defaultPrjNameStr1PropName = "TitleStr1";         

            string nameStr1 = "Наименование";           

            if (isAltiumProject)
            {
                if (settingsDB.GetItem("prjNameStr1PropName") == null)
                {
                    propNameItem.name = "prjNameStr1PropName";
                    propNameItem.valueString = defaultPrjNameStr1PropName;
                    settingsDB.SaveSettingItem(propNameItem);
                }
                else propNameItem = settingsDB.GetItem("prjNameStr1PropName");                
            }
            else // if KiCad Project
            {
                if (settingsDB.GetItem("prjNameStr1PropNameKiCad") == null)
                {
                    propNameItem.name = "prjNameStr1PropNameKiCad";
                    propNameItem.valueString = defaultPrjNameStr1PropName;
                    settingsDB.SaveSettingItem(propNameItem);
                }
                else propNameItem = settingsDB.GetItem("prjNameStr1PropNameKiCad");
            }

            defaultPrjNameStr1PropName = propNameItem.valueString;

            if (propNameList.Contains(defaultPrjNameStr1PropName))
            {
                int str1Index = propNameList.FindIndex(x => x == defaultPrjNameStr1PropName);
                nameStr1 = currentPrjParamsList[str1Index][1];
            }

            // Чтение наименования свойства со второй строкой наименования изделия

            string defaultPrjNameStr2PropName = "TitleStr2";
            string nameStr2 = String.Empty;

            if (isAltiumProject)
            {
                if (settingsDB.GetItem("prjNameStr2PropName") == null)
                {
                    propNameItem.name = "prjNameStr2PropName";
                    propNameItem.valueString = defaultPrjNameStr2PropName;
                    settingsDB.SaveSettingItem(propNameItem);
                }
                else propNameItem = settingsDB.GetItem("prjNameStr2PropName");
                defaultPrjNameStr2PropName = propNameItem.valueString;
            }
            else
            {
                if (settingsDB.GetItem("prjNameStr2PropNameKiCad") == null)
                {
                    propNameItem.name = "prjNameStr2PropNameKiCad";
                    propNameItem.valueString = defaultPrjNameStr2PropName;
                    settingsDB.SaveSettingItem(propNameItem);
                }
                else propNameItem = settingsDB.GetItem("prjNameStr2PropNameKiCad");
                defaultPrjNameStr2PropName = propNameItem.valueString;
            }

            if (propNameList.Contains(defaultPrjNameStr2PropName))
            {
                int str2Index = propNameList.FindIndex(x => x == defaultPrjNameStr2PropName);
                nameStr2 = currentPrjParamsList[str2Index][1];
            }

            //Комбинирование строк 1 и 2 наименования изделия           
           
            if (nameStr2.Trim().Length > 0)
            {
                prjName = nameStr1.Trim() + " " + nameStr2.Trim();
            }
            else prjName = nameStr1.Trim();

            //Разраб.
            string defaultDrawnByPropName = "DrawnBy";
            

            if (isAltiumProject)
            {
                if (settingsDB.GetItem("drawnByPropName") == null)
                {
                    propNameItem.name = "drawnByPropName";
                    propNameItem.valueString = defaultDrawnByPropName;
                    settingsDB.SaveSettingItem(propNameItem);
                }
                else propNameItem = settingsDB.GetItem("drawnByPropName");
            }
            else // if KiCad Project
            {
                if (settingsDB.GetItem("drawnByPropNameKiCad") == null)
                {
                    propNameItem.name = "drawnByPropNameKiCad";
                    propNameItem.valueString = defaultDrawnByPropName;
                    settingsDB.SaveSettingItem(propNameItem);
                }
                else propNameItem = settingsDB.GetItem("drawnByPropNameKiCad");
            }
            defaultDrawnByPropName = propNameItem.valueString;

            if (propNameList.Contains(defaultDrawnByPropName))
            {
                int drawnByIndex = propNameList.FindIndex(x => x == defaultDrawnByPropName);
                drawnBy = currentPrjParamsList[drawnByIndex][1];
            }

            //Пров.
            string defaultCheckedByPropName = "DrawnBy";


            if (isAltiumProject)
            {
                if (settingsDB.GetItem("checkedByPropName") == null)
                {
                    propNameItem.name = "checkedByPropName";
                    propNameItem.valueString = defaultCheckedByPropName;
                    settingsDB.SaveSettingItem(propNameItem);
                }
                else propNameItem = settingsDB.GetItem("checkedByPropName");
            }
            else // if KiCad Project
            {
                if (settingsDB.GetItem("checkedByPropNameKiCad") == null)
                {
                    propNameItem.name = "checkedByPropNameKiCad";
                    propNameItem.valueString = defaultCheckedByPropName;
                    settingsDB.SaveSettingItem(propNameItem);
                }
                else propNameItem = settingsDB.GetItem("checkedByPropNameKiCad");
            }
            defaultCheckedByPropName = propNameItem.valueString;

            if (propNameList.Contains(defaultCheckedByPropName))
            {
                int checkedByIndex = propNameList.FindIndex(x => x == defaultCheckedByPropName);
                checkedBy = currentPrjParamsList[checkedByIndex][1];
            }

            //Доп. подписант
            string defaultDopPodpisantPropName = "DrawnBy";


            if (isAltiumProject)
            {
                if (settingsDB.GetItem("dopPodpisantPropName") == null)
                {
                    propNameItem.name = "dopPodpisantPropName";
                    propNameItem.valueString = defaultDopPodpisantPropName;
                    settingsDB.SaveSettingItem(propNameItem);
                }
                else propNameItem = settingsDB.GetItem("dopPodpisantPropName");
            }
            else // if KiCad Project
            {
                if (settingsDB.GetItem("dopPodpisantPropNameKiCad") == null)
                {
                    propNameItem.name = "dopPodpisantPropNameKiCad";
                    propNameItem.valueString = defaultDopPodpisantPropName;
                    settingsDB.SaveSettingItem(propNameItem);
                }
                else propNameItem = settingsDB.GetItem("dopPodpisantPropNameKiCad");
            }
            defaultDopPodpisantPropName = propNameItem.valueString;

            if (propNameList.Contains(defaultDopPodpisantPropName))
            {
                int dopPodpisantIndex = propNameList.FindIndex(x => x == defaultDopPodpisantPropName);
                dopPodpisant = currentPrjParamsList[dopPodpisantIndex][1];
            }

            //Нормоконтролёр
            string defaultNormInspectionPropName = "DrawnBy";


            if (isAltiumProject)
            {
                if (settingsDB.GetItem("normInspectionPropName") == null)
                {
                    propNameItem.name = "normInspectionPropName";
                    propNameItem.valueString = defaultNormInspectionPropName;
                    settingsDB.SaveSettingItem(propNameItem);
                }
                else propNameItem = settingsDB.GetItem("normInspectionPropName");
            }
            else // if KiCad Project
            {
                if (settingsDB.GetItem("normInspectionPropNameKiCad") == null)
                {
                    propNameItem.name = "normInspectionPropNameKiCad";
                    propNameItem.valueString = defaultNormInspectionPropName;
                    settingsDB.SaveSettingItem(propNameItem);
                }
                else propNameItem = settingsDB.GetItem("normInspectionPropNameKiCad");
            }
            defaultNormInspectionPropName = propNameItem.valueString;

            if (propNameList.Contains(defaultNormInspectionPropName))
            {
                int normInspectionIndex = propNameList.FindIndex(x => x == defaultNormInspectionPropName);
                normInspection = currentPrjParamsList[normInspectionIndex][1];
            }

            // Утв.
            string defaultApprovedByPropName = "DrawnBy";


            if (isAltiumProject)
            {
                if (settingsDB.GetItem("approvedByPropName") == null)
                {
                    propNameItem.name = "approvedByPropName";
                    propNameItem.valueString = defaultApprovedByPropName;
                    settingsDB.SaveSettingItem(propNameItem);
                }
                else propNameItem = settingsDB.GetItem("approvedByPropName");
            }
            else // if KiCad Project
            {
                if (settingsDB.GetItem("approvedByPropNameKiCad") == null)
                {
                    propNameItem.name = "approvedByPropNameKiCad";
                    propNameItem.valueString = defaultApprovedByPropName;
                    settingsDB.SaveSettingItem(propNameItem);
                }
                else propNameItem = settingsDB.GetItem("approvedByPropNameKiCad");
            }
            defaultApprovedByPropName = propNameItem.valueString;

            if (propNameList.Contains(defaultApprovedByPropName))
            {
                int approvedByIndex = propNameList.FindIndex(x => x == defaultApprovedByPropName);
                approvedBy = currentPrjParamsList[approvedByIndex][1];
            }

            // Наименование организации
            string defaultCompanyPropName = "CompanyName";


            if (isAltiumProject)
            {
                if (settingsDB.GetItem("companyPropName") == null)
                {
                    propNameItem.name = "companyPropName";
                    propNameItem.valueString = defaultCompanyPropName;
                    settingsDB.SaveSettingItem(propNameItem);
                }
                else propNameItem = settingsDB.GetItem("companyPropName");
            }
            else // if KiCad Project
            {
                if (settingsDB.GetItem("companyPropNameKiCad") == null)
                {
                    propNameItem.name = "companyPropNameKiCad";
                    propNameItem.valueString = defaultCompanyPropName;
                    settingsDB.SaveSettingItem(propNameItem);
                }
                else propNameItem = settingsDB.GetItem("companyPropNameKiCad");
            }
            defaultCompanyPropName = propNameItem.valueString;

            if (propNameList.Contains(defaultCompanyPropName))
            {
                int companyIndex = propNameList.FindIndex(x => x == defaultCompanyPropName);
                companyName = currentPrjParamsList[companyIndex][1];
            }

            propertiesGrid.Visibility = Visibility.Hidden;
            moduleDecimalNumberGrid.Visibility = Visibility.Visible;

        }


        private void Next_3_Click(object sender, RoutedEventArgs e)
        {
            List<string> propNameList = new List<string>();

            currentPrjParamsList = new List<string[]>();

            if (prjParamsVariantList.Count > 0)
                for (int i = 0; i < prjParamsVariantList[0].Count; i++)
                {
                    propNameList.Add(prjParamsVariantList[0][i][0]);
                    currentPrjParamsList.Add(prjParamsVariantList[0][i]);
                }


            if (variantSelectionComboBox.SelectedIndex > 0)
            {
                if (prjParamsVariantList.Count > 1) //если в варианте изменяются параметры
                    for (int j = 0; j < prjParamsVariantList[variantSelectionComboBox.SelectedIndex].Count; j++)
                    {
                        bool isNewName = true;
                        for (int i = 0; i < prjParamsVariantList[0].Count; i++)
                        {
                            if (prjParamsVariantList[0][i][0] == prjParamsVariantList[variantSelectionComboBox.SelectedIndex][j][0])
                            {
                                currentPrjParamsList[i] = prjParamsVariantList[variantSelectionComboBox.SelectedIndex][j];
                                isNewName = false;
                            }
                        }
                        if (isNewName)
                        {
                            currentPrjParamsList.Add(prjParamsVariantList[variantSelectionComboBox.SelectedIndex][j]);
                            propNameList.Add(prjParamsVariantList[variantSelectionComboBox.SelectedIndex][j][0]);
                        }
                    }
            }


            pcbNumberSelectionComboBox.ItemsSource = propNameList;

            //Децимальный номер платы
            string defaultPcbDocNumberPropName = "PcbDocNumber";
            SettingsItem propNameItem = new SettingsItem();
            SettingsDB settingsDB = new SettingsDB();

            if (isAltiumProject)
            {
                if (settingsDB.GetItem("pcbDocNumberPropName") == null)
                {
                    propNameItem.name = "pcbDocNumberPropName";
                    propNameItem.valueString = defaultPcbDocNumberPropName;
                    settingsDB.SaveSettingItem(propNameItem);
                }
                else propNameItem = settingsDB.GetItem("pcbDocNumberPropName");
            }
            else // if KiCad Project
            {
                if (settingsDB.GetItem("pcbDocNumberPropNameKiCad") == null)
                {
                    propNameItem.name = "pcbDocNumberPropNameKiCad";
                    propNameItem.valueString = defaultPcbDocNumberPropName;
                    settingsDB.SaveSettingItem(propNameItem);
                }
                else propNameItem = settingsDB.GetItem("pcbDocNumberPropNameKiCad");
            }
            defaultPcbDocNumberPropName = propNameItem.valueString;

            if (propNameList.Contains(defaultPcbDocNumberPropName)) pcbNumberSelectionComboBox.SelectedIndex = propNameList.FindIndex(x => x == defaultPcbDocNumberPropName);
            else prjNumberSelectionComboBox.SelectedIndex = 0;

            // Наименование платы:
            // Чтение наименования свойства с первой строкой наименования платы
            string defaultPcbNameStr1PropName = "PcbTitle";

            string nameStr1 = "Наименование";

            if (isAltiumProject)
            {
                if (settingsDB.GetItem("pcbNameStr1PropName") == null)
                {
                    propNameItem.name = "pcbNameStr1PropName";
                    propNameItem.valueString = defaultPcbNameStr1PropName;
                    settingsDB.SaveSettingItem(propNameItem);
                }
                else propNameItem = settingsDB.GetItem("pcbNameStr1PropName");
            }
            else // if KiCad Project
            {
                if (settingsDB.GetItem("pcbNameStr1PropNameKiCad") == null)
                {
                    propNameItem.name = "pcbNameStr1PropNameKiCad";
                    propNameItem.valueString = defaultPcbNameStr1PropName;
                    settingsDB.SaveSettingItem(propNameItem);
                }
                else propNameItem = settingsDB.GetItem("pcbNameStr1PropNameKiCad");
            }

            defaultPcbNameStr1PropName = propNameItem.valueString;

            if (propNameList.Contains(defaultPcbNameStr1PropName))
            {
                int str1Index = propNameList.FindIndex(x => x == defaultPcbNameStr1PropName);
                nameStr1 = currentPrjParamsList[str1Index][1];
            }

            // Чтение наименования свойства со второй строкой наименования платы

            string defaultPcbNameStr2PropName = String.Empty;
            string nameStr2 = String.Empty;

            if (isAltiumProject)
            {
                if (settingsDB.GetItem("pcbNameStr2PropName") == null)
                {
                    propNameItem.name = "pcbNameStr2PropName";
                    propNameItem.valueString = defaultPcbNameStr2PropName;
                    settingsDB.SaveSettingItem(propNameItem);
                }
                else propNameItem = settingsDB.GetItem("pcbNameStr2PropName");
                defaultPcbNameStr2PropName = propNameItem.valueString;
            }
            else
            {
                if (settingsDB.GetItem("pcbNameStr2PropNameKiCad") == null)
                {
                    propNameItem.name = "pcbNameStr2PropNameKiCad";
                    propNameItem.valueString = defaultPcbNameStr2PropName;
                    settingsDB.SaveSettingItem(propNameItem);
                }
                else propNameItem = settingsDB.GetItem("pcbNameStr2PropNameKiCad");
                defaultPcbNameStr2PropName = propNameItem.valueString;
            }

            if (propNameList.Contains(defaultPcbNameStr2PropName))
            {
                int str2Index = propNameList.FindIndex(x => x == defaultPcbNameStr2PropName);
                nameStr2 = currentPrjParamsList[str2Index][1];
            }

            //Комбинирование строк 1 и 2 наименования платы       

            if (nameStr2.Trim().Length > 0)
            {
                pcbName = nameStr1.Trim() + " " + nameStr2.Trim();
            }
            else pcbName = nameStr1.Trim();

            moduleDecimalNumberGrid.Visibility = Visibility.Hidden;
            pcbDecimalNumberGrid.Visibility = Visibility.Visible;

        }

        private void prjNumberSelectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (prjNumberSelectionComboBox.HasItems)
            {
                prjNumberTextBox.Text = currentPrjParamsList[prjNumberSelectionComboBox.SelectedIndex][1];
            }
        }

        private void prjNumberRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (prjNumberSelectionComboBox.HasItems)
            {
                if ((sender as RadioButton).Name == "manualPrjNumberRadioButton")
                {
                    prjNumberSelectionComboBox.IsEnabled = false;
                    prjNumberTextBox.IsEnabled = true;
                }
                else
                {
                    prjNumberSelectionComboBox.IsEnabled = true;
                    prjNumberTextBox.IsEnabled = false;
                }
            }
        }

        private void pcbNumberSelectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (pcbNumberSelectionComboBox.HasItems)
            {
                pcbNumberTextBox.Text = currentPrjParamsList[pcbNumberSelectionComboBox.SelectedIndex][1];
            }
        }

        private void pcbNumberRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (pcbNumberSelectionComboBox.HasItems)
            {
                if ((sender as RadioButton).Name == "manualPcbNumberRadioButton")
                {
                    pcbNumberSelectionComboBox.IsEnabled = false;
                    pcbNumberTextBox.IsEnabled = true;
                }
                else
                {
                    pcbNumberSelectionComboBox.IsEnabled = true;
                    pcbNumberTextBox.IsEnabled = false;
                }
            }
        }
    }
}
