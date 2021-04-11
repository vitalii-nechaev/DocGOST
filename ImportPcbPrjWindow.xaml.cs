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
        public ImportPcbPrjWindow()
        {
            InitializeComponent();
        }

        public List<List<string[]>> prjParamsVariantList;// Параметры проекта для возможности считывания 


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
                    //nextButton.IsEnabled = false;
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

        private void Next_2_Click(object sender, RoutedEventArgs e)
        {
            List<string> prjNumberPropNameList = new List<string>();

            currentPrjParamsList = new List<string[]>();

            if (prjParamsVariantList.Count > 0)
            for (int i = 0; i < prjParamsVariantList[0].Count; i++)
                {
                    prjNumberPropNameList.Add(prjParamsVariantList[0][i][0]);
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
                        prjNumberPropNameList.Add(prjParamsVariantList[variantSelectionComboBox.SelectedIndex][j][0]);
                    }
                }
            }


            prjNumberSelectionComboBox.ItemsSource = prjNumberPropNameList;

            propertiesGrid.Visibility = Visibility.Hidden;
            moduleDecimalNumberGrid.Visibility = Visibility.Visible;

        }


        private void Next_3_Click(object sender, RoutedEventArgs e)
        {
            List<string> pcbNumberPropNameList = new List<string>();

            currentPrjParamsList = new List<string[]>();

            if (prjParamsVariantList.Count > 0)
                for (int i = 0; i < prjParamsVariantList[0].Count; i++)
                {
                    pcbNumberPropNameList.Add(prjParamsVariantList[0][i][0]);
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
                            pcbNumberPropNameList.Add(prjParamsVariantList[variantSelectionComboBox.SelectedIndex][j][0]);
                        }
                    }
            }


            pcbNumberSelectionComboBox.ItemsSource = pcbNumberPropNameList;
                        
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
