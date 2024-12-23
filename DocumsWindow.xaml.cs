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
    /// Interaction logic for DocumsWindow.xaml
    /// </summary>
    public partial class DocumsWindow : Window
    {
        bool isDocums4Pcb = false;
        public DocumsWindow(bool isDocums4Pcb)
        {
            InitializeComponent();
            this.isDocums4Pcb = isDocums4Pcb;
            if (isDocums4Pcb) this.Title = "Список документации для печатной платы";
            DisplayResult();           
        }
        /// <summary>
        /// Отображает все позиционные обозначения с наименованиями из базы данных
        /// </summary>
        private void DisplayResult()
        {
            DocumsDB documsDB = new DocumsDB(isDocums4Pcb); 
           
            int length = documsDB.GetLength();
            //Вывод строк в окно программы:
            List<DocumsItem> result = new List<DocumsItem>(length);

            for (int i = 1; i <= length; i++)
            {
                DocumsItem dd = documsDB.GetItem(i);
                result.Add(dd);
            }

            designatorsListView.ItemsSource = result;
        }

        /// <summary>
        /// Открытие окна редактирования при нажатии на кнопку "Редактировать"
        /// </summary>
        private void EditDocum(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            DocumsItem dItem = b.CommandParameter as DocumsItem;
            DocumsAddEditWindow dEditWindow = new DocumsAddEditWindow(isDocums4Pcb);
            dEditWindow.Title = "Редактирование списка документации";

            dEditWindow.codeTextBox.Text = dItem.code;
            dEditWindow.nameTextBox.Text = dItem.name;
            dEditWindow.formatTextBox.Text = dItem.format;
            dEditWindow.noteTextBox.Text = dItem.note;

            if (dEditWindow.ShowDialog() == true)
            {
                DisplayResult();
            }
        }

        /// <summary>
        /// Удаление выбранного позиционного обозначения после подтверждения пользователем
        /// </summary>
        private void DeleteDocum(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            DocumsItem ddItem = b.CommandParameter as DocumsItem;
            MessageBoxResult dialogResult = MessageBox.Show("Вы действительно хотите удалить документ с кодом " +
                                                            ddItem.code + "?",
                                                            "Маленькое уточнение",
                                                            MessageBoxButton.YesNo);
            if (dialogResult == MessageBoxResult.Yes)
            {
                DocumsDB desDescr = new DocumsDB(isDocums4Pcb);
                desDescr.DeliteDocumsItem(ddItem);
                DisplayResult();
            }

        }

        /// <summary>
        /// Открытие окна добавления записи позиционного обозначения при нажатии на кнопку "Добавить"
        /// </summary>
        private void addItemButton_Click(object sender, RoutedEventArgs e)
        {
            DocumsAddEditWindow dAddWindow = new DocumsAddEditWindow(isDocums4Pcb);
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
            double coef = (window.ActualWidth - 205) / 560; //Изначально ширина окна 750, отступ ListView 10 и 2 кнопки в ListView по 85
            codeColumn.Width = 150 * coef;
            nameColumn.Width = 240 * coef;
            formatColumn.Width = 60 * coef;
            noteColumn.Width = 100 * coef;
        }
    }
}
