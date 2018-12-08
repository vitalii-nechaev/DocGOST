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
using System.Windows.Navigation;
using System.Data.SQLite;
using System.IO;
using Microsoft.Win32;
using iTextSharp.text.pdf;
using iTextSharp.text;
using DocGOST.Data;

namespace DocGOST
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        string projectPath;

        static Database project; //данные для перечня элементов

        private void createProject_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog createDlg = new SaveFileDialog();
            createDlg.Title = "Создание проекта";            
            createDlg.Filter = "Файлы проекта (*.docGOST)|*.docGOST";
            createDlg.OverwritePrompt = true;
            
            if (createDlg.ShowDialog() == true)
            {
                projectPath = createDlg.FileName;
                //Удаляем все ээлементы из дерева проектов
                for (int i=0;i< projectTreeViewItem.Items.Count; i++)
                projectTreeViewItem.Items.RemoveAt(i);                
                //Называем проект в дереве проектов так же, как файл
                int length = createDlg.SafeFileName.Length;
                projectTreeViewItem.Header = createDlg.SafeFileName.Substring(0,length-8);
                TreeViewItem newItem = new TreeViewItem();               
                newItem.Header = "Перечень элементов";               
                projectTreeViewItem.Items.Add(newItem);
                newItem = new TreeViewItem();
                newItem.Header = "Спецификация";                
                projectTreeViewItem.Items.Add(newItem);

                projectTreeViewItem.ExpandSubtree();

                project = new Data.Database(projectPath);
            }          
        }

        private void openProject_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.Title = "Выбор файла проекта";
            openDlg.Multiselect = false;
            openDlg.Filter = "Файлы проекта (*.docGOST)|*.docGOST";
            if (openDlg.ShowDialog() == true)
            {
                projectPath = openDlg.FileName;
                project = new Data.Database(projectPath);
            }
            

            DisplayPerechenValues();
        }


        bool atLeastOneApparatusEntered = false;

        

        private void importBOMfromAD_Click(object sender, RoutedEventArgs e)
        {
            #region Открытие файла с расшифровкой позиционных обозначений
            /*
             * В текстовом файле количество столбцов должно быть 2 или более
             * 
             * В наименование может как входить наименование группы компонента, так и нет. Если наименование группы входит, то нельзя группировать элементы по группе.
             * В намиенование может как входить обозначение документа на поставку, так и нет.
             * 
             * Ожидаемый формат текстового файла:
             *     Столбец 1            Столбец 2       Столбец 3       Столбец 4               Столбец 5           Столбец 6           Столбец N
             *     Поз. обозначение     Наименование    
             *     Поз. обозначение     Наименование    Примечание      
             *     Поз. обозначение     Наименование    Примечание      Документ на поставку    Тип компонента 1    Тип компонента 2    Тип компонента N - 4
             * 
             */
            //Открываем файл для подсчёта количества строк:
            FileStream fileDesDescr = new FileStream("DesignatorDescriptions.txt", FileMode.Open, FileAccess.Read);
            StreamReader readerDesDescr = new StreamReader(fileDesDescr, System.Text.Encoding.Default);

            String strDesDescr;
            int numberOfStrings = 0;

            int numOfStrings = 0;

            while ((strDesDescr = readerDesDescr.ReadLine()) != null)
            {
                numOfStrings++;
            }

            fileDesDescr.Close();

            //Открываем файл для чтения данных:
            fileDesDescr = new FileStream("DesignatorDescriptions.txt", FileMode.Open, FileAccess.Read);
            readerDesDescr = new StreamReader(fileDesDescr, System.Text.Encoding.Default);
            strDesDescr = String.Empty;

            //Создаём массив строк для хранения позиционных обозначений и их описаний
            DesignatorDescriptions[] desDescrs = new DesignatorDescriptions[numOfStrings];

            for (int i = 0; i < numOfStrings; i++)
            {
                strDesDescr = readerDesDescr.ReadLine();
                string[] temp = new string[4];
                if (strDesDescr.Split(new Char[] { '\t' }).Length == 3)
                {
                    temp = strDesDescr.Split(new Char[] { '\t' });
                    desDescrs[i] = new DesignatorDescriptions(temp[0], temp[1], temp[2]);
                }
            }

            fileDesDescr.Close();
            #endregion


            #region Открытие файла BOM и формирование массива строк перечня элементов
            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.Title = "Выбор файла данных для перечня элементов";
            openDlg.Multiselect = false;
            openDlg.Filter = "Файлы BOM (*.txt)|*.txt";
            openDlg.ShowDialog();

            string destinationFilePath = openDlg.FileName;

            //Открываем файл для подсчёта количества строк:
            FileStream file = new FileStream(@destinationFilePath, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(file, System.Text.Encoding.Default);

            String str;

            numberOfStrings = 0;

            while ((str = reader.ReadLine()) != null)
            {
                numberOfStrings++;
            }

            file.Close();
            //Открываем файл для чтения данных:
            file = new FileStream(@destinationFilePath, FileMode.Open, FileAccess.Read);
            reader = new StreamReader(file, System.Text.Encoding.Default);
            str = String.Empty;

            //если данные уже добавлялись, предлагаем ввести наименование нового устройства
            /*if (project.isPerechenDataEmpty() != true)
            {
                //Вставляем назавание первого устройства в список, если этого ещё не произошло
                if (atLeastOneApparatusEntered == false)
                {
                    NewApparatusName apparatNameWindow = new NewApparatusName();
                    apparatNameWindow.captionLabel.Content = "Введите позиционное обозначение, наименование и примечание для уже введённого устройства:";
                    apparatNameWindow.ShowDialog();
                    //Вставляем наименование первого устройства в начало списка
                    PerechenData temp = new PerechenData();
                    temp.designator = apparatNameWindow.pozTextBox.Text;
                    temp.name = apparatNameWindow.nameTextBox.Text;
                    temp.quantity = "1";
                    temp.note = apparatNameWindow.noteTextBox.Text;
                    temp.docum = String.Empty;
                    temp.type = "Составное устройство";
                    temp.group = String.Empty;
                    temp.groupPlural = String.Empty;
                    pData.Insert(0, temp);
                    numberOfValidStrings++;
                    atLeastOneApparatusEntered = true;

                    apparatNameWindow = new NewApparatusName();
                    apparatNameWindow.captionLabel.Content = "Введите позиционное обозначение, наименование и примечание для нового устройства:";
                    apparatNameWindow.ShowDialog();
                     //Добавляем пустую строку
                     //temp = new PerechenData();
                     //pData.Add(temp);
                    temp = new PerechenData();
                    //Добавляем наименование текущего устройства
                    temp.designator = apparatNameWindow.pozTextBox.Text;
                    temp.name = apparatNameWindow.nameTextBox.Text;
                    temp.quantity = "1";
                    temp.note = apparatNameWindow.noteTextBox.Text;
                    temp.docum = String.Empty;
                    temp.type = "Составное устройство";
                    temp.group = String.Empty;
                    temp.groupPlural = String.Empty;
                    pData.Add(temp);
                    numberOfValidStrings++;
                }
                else
                {
                    //Вставляем наименование текущего устройства в список
                    NewApparatusName apparatNameWindow = new NewApparatusName();
                    apparatNameWindow.ShowDialog();
                    PerechenData temp = new PerechenData();
                    temp.designator = apparatNameWindow.pozTextBox.Text;
                    temp.name = apparatNameWindow.nameTextBox.Text;
                    temp.quantity = "1";
                    temp.note = apparatNameWindow.noteTextBox.Text;
                    temp.docum = String.Empty;
                    temp.type = "Составное устройство";
                    temp.group = String.Empty;
                    temp.groupPlural = String.Empty;
                    pData.Add(temp);
                    numberOfValidStrings++;
                }

            }*/

            #region Заполнение массива несгруппированными значениями
            //reader.ReadLine(); //Пропускаем первую строку - строку с названиями колонок
            for (int i = 0; i < numberOfStrings; i++)
            {
                str = reader.ReadLine();
                Data.PerechenItem temp = new Data.PerechenItem();
                if (str.Split(new Char[] { '\t' }).Length > 1)
                    if (str[0] == '\"')
                    {
                        temp.designator = str.Split(new Char[] { '\t' })[0];
                        temp.designator = temp.designator.Substring(1, temp.designator.Length - 2);
                        temp.name = str.Split(new Char[] { '\t' })[1];
                        temp.name = temp.name.Substring(1, temp.name.Length - 2);
                        temp.quantity = "1";
                        temp.note = String.Empty;
                        temp.docum = String.Empty;
                        temp.type = String.Empty;
                        temp.group = String.Empty;
                        temp.groupPlural = String.Empty;

                        int numOfColumns = str.Split(new Char[] { '\t' }).Length;

                        if (numOfColumns == 3)
                        {
                            temp.note = str.Split(new Char[] { '\t' })[2];
                            temp.note = temp.note.Substring(1, temp.note.Length - 2);
                        }
                        else if (numOfColumns > 3)
                        {
                            temp.note = str.Split(new Char[] { '\t' })[2];
                            temp.note = temp.note.Substring(1, temp.note.Length - 2);
                            temp.docum = str.Split(new Char[] { '\t' })[3];
                            temp.docum = temp.docum.Substring(1, temp.docum.Length - 2);


                            if (numOfColumns > 4)
                            {
                                for (int j = 5; j <= numOfColumns; j++)
                                {
                                    if ((temp.type == String.Empty) | (temp.type == "\"\"")) temp.type = str.Split(new Char[] { '\t' })[j - 1];
                                    if (temp.type != String.Empty) temp.type = temp.type.Substring(1, temp.type.Length - 2);
                                }
                            }
                        }

                        //Добавляем документ на поставку к наименованию элемента:
                        //if (addDocumToNameCheckBox.IsChecked == true)
                        //    temp.name += ' ' + temp.docum;

                        //Добавляем тип компонента на основе его позиционного обозначения:
                        foreach (DesignatorDescriptions desDescr in desDescrs)
                            if ((desDescr.Designator == temp.designator.Substring(0, 1)) | (desDescr.Designator == temp.designator.Substring(0, 2)))
                            {
                                temp.group = desDescr.Description.Substring(0, 1).ToUpper() + desDescr.Description.Substring(1, desDescr.Description.Length - 1).ToLower();
                                temp.groupPlural = desDescr.DescriptionPlural.Substring(0, 1).ToUpper() + desDescr.DescriptionPlural.Substring(1, desDescr.DescriptionPlural.Length - 1).ToLower();
                            }

                        project.AddPerechenItem(temp);
                    }

            }
            #endregion

            DisplayPerechenValues();


            #endregion
        }

        private void DisplayPerechenValues()
        {
            int length = project.GetPerechenLength();
            //Вывод несгруппированных строк в окно программы:
            List<PerechenDataToDisplay> result = new List<PerechenDataToDisplay>(length);
            numberOfValidStringsLabel.Content = length;
            //foreach (PerechenItem pd in pData) result.Add(new PerechenDataToDisplay(pd.designator, pd.name, pd.note, pd.docum));
            for (int i = 1; i <= length; i++)
            {
                Data.PerechenItem pd = project.GetPerechenItem(i);
              result.Add(new PerechenDataToDisplay(pd.designator, pd.group + ' ' + pd.name + ' ' + pd.docum, pd.note));
            }

            PerechenDataGrid.ItemsSource = result;

            PerechenDataGrid.Columns[0].Width = new DataGridLength(120, DataGridLengthUnitType.Star);
            PerechenDataGrid.Columns[1].Width = new DataGridLength(380, DataGridLengthUnitType.Star);
            PerechenDataGrid.Columns[2].Width = new DataGridLength(90, DataGridLengthUnitType.Star);
        }

        private void createPdf_Click(object sender, RoutedEventArgs e)
        {
            string pdfPath = "ПЭ3.pdf";
            PdfOperations pdf = new PdfOperations(projectPath);
            int startPage = (startFromSecondCheckBox.IsChecked == false) ? 1 : 2;
            bool addListRegistr = (addListRegistrCheckBox.IsChecked == true);
            pdf.CreatePerechen(pdfPath,startPage, addListRegistr);
           
        }

       
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            

        }

        private void groupByName_Click(object sender, RoutedEventArgs e)
        {
            List<PerechenItem> pData = new List<PerechenItem>();
            int numberOfValidStrings = project.GetPerechenLength();

            for (int i = 1; i <= numberOfValidStrings; i++)
            {
                pData.Add(project.GetPerechenItem(i));
            }

            (new PerechenOperations()).groupPerechenElements(ref pData, ref numberOfValidStrings);

            //Вывод сгруппированных строк в окно программы:
            List<PerechenDataToDisplay> result = new List<PerechenDataToDisplay>(numberOfValidStrings);

            //foreach (PerechenItem pd in pData) result.Add(new PerechenDataToDisplay(pd.designator, pd.name, pd.note, pd.docum));
            for (int i = 0; i < result.Capacity; i++)
            {
                Data.PerechenItem pd = pData[i];
                result.Add(new PerechenDataToDisplay(pd.designator, pd.name + ' ' + pd.docum, pd.note));
            }

            PerechenDataGrid.ItemsSource = result;
            PerechenDataGrid.Columns[0].Width = new DataGridLength(120, DataGridLengthUnitType.Star);
            PerechenDataGrid.Columns[1].Width = new DataGridLength(380, DataGridLengthUnitType.Star);
            PerechenDataGrid.Columns[2].Width = new DataGridLength(90, DataGridLengthUnitType.Star);
        }

        private void osnNadpisButton_Click(object sender, RoutedEventArgs e)
        {
            OsnNadpisWindow osnNadpis;
            osnNadpis = new OsnNadpisWindow(projectPath);
            //Показываем диалоговое окно для заполнения граф основной надписи
            osnNadpis.ShowDialog();
            
            osnNadpis.Close();

        }

        
    }
}
