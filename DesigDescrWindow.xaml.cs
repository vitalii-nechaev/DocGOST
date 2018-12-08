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
using DocGOST.Data;
using System.Windows.Controls;

namespace DocGOST
{
    /// <summary>
    /// Логика взаимодействия для DesigDescrWindow.xaml
    /// </summary>
    public partial class DesigDescrWindow : Window
    {
        public DesigDescrWindow()
        {
            InitializeComponent();

           
            DisplayResult();
        }

        /// <summary>
        /// Отображает все позиционные обозначения с наименованиями из базы данных
        /// </summary>
        private void DisplayResult()
        {
            DesignatorDB desDescr = new DesignatorDB();
            int length = desDescr.GetLength();
            //Вывод несгруппированных строк в окно программы:
            List<DesignatorDescriptionItem> result = new List<DesignatorDescriptionItem>(length);

            for (int i = 1; i <= length; i++)
            {
                DesignatorDescriptionItem dd = desDescr.GetItem(i);
                result.Add(dd);
            }

            designatorsListView.ItemsSource = result;
        }

        /// <summary>
        /// Открытие окна редактирования при нажатии на кнопку "Редактировать"
        /// </summary>
        private void EditCategory(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            DesignatorDescriptionItem ddItem = b.CommandParameter as DesignatorDescriptionItem;
            DesignatorAddEditWindow dEditWindow = new DesignatorAddEditWindow();
            dEditWindow.Title = "Редактирование названия группы";
            
            dEditWindow.designatorTextBox.Text = ddItem.Designator;
            dEditWindow.groupTextBox.Text = ddItem.Group;
            dEditWindow.groupPluralTextBox.Text = ddItem.GroupPlural;

            if (dEditWindow.ShowDialog() == true)
            {
                DisplayResult();
            }
        }

        /// <summary>
        /// Удаление выбранного позиционного обозначения после подтверждения пользователем
        /// </summary>
        private void DeleteCategory(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            DesignatorDescriptionItem ddItem = b.CommandParameter as DesignatorDescriptionItem;
            MessageBoxResult dialogResult = MessageBox.Show("Вы действительно хотите удалить позиционные обозначения " + 
                                                            ddItem.Designator + "?", 
                                                            "Маленькое уточнение", 
                                                            MessageBoxButton.YesNo);
            if (dialogResult == MessageBoxResult.Yes)
            {
                DesignatorDB desDescr = new DesignatorDB();
                desDescr.DeliteDesignatorItem(ddItem);
                DisplayResult();
            }

        }

        /// <summary>
        /// Открытие окна добавления записи позиционного обозначения при нажатии на кнопку "Добавить"
        /// </summary>
        private void addItemButton_Click(object sender, RoutedEventArgs e)
        {
            DesignatorAddEditWindow dAddWindow = new DesignatorAddEditWindow();
            dAddWindow.Title = "Добавление названия группы";
            if (dAddWindow.ShowDialog() == true)
            {
                DisplayResult();
            }
        }
        
        /// <summary>
        /// Изменение ширины столбцов ListView при изменении размеров окна программы
        /// </summary>
        private void sizeChanged(object sender, SizeChangedEventArgs e)
        {
            double coef = (window.ActualWidth-205) / 564; //Изначально ширина окна 750, отступ ListView 10 и 2 кнопки в ListView по 83
            designatorColumn.Width = 135 * coef;
            groupColumn.Width = 200 * coef;
            groupPluralColumn.Width = 200 * coef;
        }

    }    
}
