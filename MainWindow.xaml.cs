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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using Microsoft.Win32;
using DocGOST.Data;

namespace DocGOST
{
    /// <summary>
    /// В программе производится работа с проектом, который представляет собой базу данных SQLite, сохранённую с расширением .DocGOST.
    /// Эта база данных состоит из нескольких талиц.
    /// 1) Таблица класса PerechenItem предназначена для хранения данных перечня элементов.
    /// 2) Табилца класса SpecificationItem предназначена для хранения данных спецификации.
    /// В этих двух таблицах ключевые элементы id представляют собой переменную типа int,
    /// которая комбинирует номер строки документа (первые 20 бит) и номер временного сохранения (последние 12 бит).
    /// Номер временного сохранения больше или равен 1 и нужен для функционирования кнопок "Отмена" и "Возврат".
    /// При сохранении проекта данные текущего временного сохранения записываются с номером сохранения 0.
    /// При закрытии проекта все сохранения, кроме нулевого, удаляются из базы данных прокета.
    /// 3) Таблица класса OsnNadpis предназначена для хранения граф основной надписи проекта.
    /// 
    /// Кроме того, программа использует другую базу данных, в которой есть таблица с данными класса DesignatorDescriptionItem.
    /// Эта база данных должна находиться в одной папке с exe-файлом программы и её данные не зависят от отрытого в данный момент проекта.
    /// Она предназначена для расшифровки позиционных обозначений компонентов (например, строка "C" "Конденсатор" "Конденсаторы"). При
    /// изменении этой базы данных эти изменения коснутся не только текущего проекта, но и всех проектов, которые будут открыты после её изменения.
    /// </summary>
    public partial class MainWindow : Window
    {
        TempSaves perTempSave, specTempSave, vedomostTempSave; //переменные для работы с номерами временных сохранений (для работы кнопок "Отменить"/"Вернуть")
        Global id; //Переменная для работы с id данных проекта (т.е. для того, чтобы создавать и расшифровывать id,
                   //т.к. id сгруппирован из номера текущего сохранённого состояния и номера строки записи - подробнее в Data.PerechenItem.cs и Data.SpecificationItem.cs)

        public MainWindow()
        {
            InitializeComponent();

            Application.Current.MainWindow.WindowState = WindowState.Maximized;

            id = new Global();

            CommandBinding undoBinding = new CommandBinding(ApplicationCommands.Undo);
            undoBinding.Executed += UndoButton_Click;
            this.CommandBindings.Add(undoBinding);

            CommandBinding redoBinding = new CommandBinding(ApplicationCommands.Redo);
            redoBinding.Executed += RedoButton_Click;
            this.CommandBindings.Add(redoBinding);

            CommandBinding saveBinding = new CommandBinding(ApplicationCommands.Save);
            saveBinding.Executed += SaveProject_Click;
            this.CommandBindings.Add(saveBinding);

            CommandBinding openBinding = new CommandBinding(ApplicationCommands.Open);
            openBinding.Executed += OpenProject_Click;
            this.CommandBindings.Add(openBinding);

            CommandBinding newBinding = new CommandBinding(ApplicationCommands.New);
            newBinding.Executed += CreateProject_Click;
            this.CommandBindings.Add(newBinding);

            CommandBinding closeBinding = new CommandBinding(ApplicationCommands.Close);
            closeBinding.Executed += ExitMenuItem_Click;
            this.CommandBindings.Add(closeBinding);
            
        }

        string projectPath;

        static ProjectDB project; //данные для перечня элементов

        /// <summary> Создание нового проекта </summary>
        private void CreateProject_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog createDlg = new SaveFileDialog();
            createDlg.Title = "Создание проекта";
            createDlg.Filter = "Файлы проекта (*.docGOST)|*.docGOST";
            createDlg.OverwritePrompt = true;
            createDlg.FileName = "Проект";

            if (createDlg.ShowDialog() == true)
            {
                projectPath = createDlg.FileName;
                //Удаляем все элементы из дерева проектов
                for (int i = 0; i < projectTreeViewItem.Items.Count; i++)
                    projectTreeViewItem.Items.RemoveAt(i);
                //Называем проект в дереве проектов так же, как файл
                int length = createDlg.SafeFileName.Length;
                projectTreeViewItem.Header = createDlg.SafeFileName.Substring(0, length - 8);

                TreeViewItem apparatItem = new TreeViewItem();
                apparatItem.Header = "Устройство 1";
                apparatItem.Name = "apparatusTereeItem1";

                TreeViewItem docItem = new TreeViewItem();
                docItem.Header = "Перечень элементов";
                docItem.IsSelected = true;
                docItem.Selected += treeSelectionChanged;
                apparatItem.Items.Add(docItem);
                docItem = new TreeViewItem();
                docItem.Header = "Спецификация";
                docItem.Selected += treeSelectionChanged;
                apparatItem.Items.Add(docItem);
                docItem = new TreeViewItem();
                docItem.Header = "Ведомость";
                docItem.Selected += treeSelectionChanged;
                apparatItem.Items.Add(docItem);
                projectTreeViewItem.Items.Add(apparatItem);

                projectTreeViewItem.ExpandSubtree();

                if (File.Exists(projectPath))
                {
                    File.Delete(projectPath);
                }

                project = new ProjectDB(projectPath);

                //Очищаем рабочую область от предыдущих значений
                DisplayPerValues(null);
                DisplaySpecValues(null);

                importMenuItem.IsEnabled = true;
                osnNadpisMenuItem.IsEnabled = true;
                osnNadpisButton.IsEnabled = true;
            }

        }

        /// <summary> Переключение между спецификацией и перечнем в дереве проекта </summary>
        private void treeSelectionChanged(object sender, RoutedEventArgs e)
        {
            string header = (sender as TreeViewItem).Header.ToString();
            if (header == "Перечень элементов")
            {
                perechenListView.Visibility = Visibility.Visible;
                specificationTabControl.Visibility = Visibility.Hidden;
                vedomostListView.Visibility = Visibility.Hidden;
            }
            else if (header == "Спецификация")
            {
                perechenListView.Visibility = Visibility.Hidden;
                specificationTabControl.Visibility = Visibility.Visible;
                vedomostListView.Visibility = Visibility.Hidden;
            }
            else if (header == "Ведомость")
            {
                perechenListView.Visibility = Visibility.Hidden;
                specificationTabControl.Visibility = Visibility.Hidden;
                vedomostListView.Visibility = Visibility.Visible;
            }
        }

        /// <summary> Открытие существующего проекта </summary>
        private void OpenProject_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.Title = "Выбор файла проекта";
            openDlg.Multiselect = false;
            openDlg.Filter = "Файлы проекта (*.docGOST)|*.docGOST";
            if (openDlg.ShowDialog() == true)
            {

                MessageBoxResult closingDialogResult = MessageBoxResult.No;
                if ((perTempSave != null) & (specTempSave != null)&(vedomostTempSave != null))
                {
                    if ((perTempSave.GetCurrent() != perTempSave.GetLastSavedState()) |
                    (specTempSave.GetCurrent() != specTempSave.GetLastSavedState()) |
                    (vedomostTempSave.GetCurrent() != vedomostTempSave.GetLastSavedState()))
                    {
                        closingDialogResult = MessageBox.Show("Проект не сохранён. Сохранить проект перед закрытием?", "Сохранение проекта", MessageBoxButton.YesNoCancel);
                        if (closingDialogResult == MessageBoxResult.Yes) project.Save(perTempSave.GetCurrent(), specTempSave.GetCurrent(), vedomostTempSave.GetCurrent());
                    }
                    project.DeleteTempData();
                }
                   

                if (closingDialogResult != MessageBoxResult.Cancel)
                {
                    projectPath = openDlg.FileName;
                    project = new Data.ProjectDB(projectPath);

                    perTempSave = new TempSaves();
                    specTempSave = new TempSaves();
                    vedomostTempSave = new TempSaves();

                    //Удаляем все элементы из дерева проектов
                    for (int i = 0; i < projectTreeViewItem.Items.Count; i++)
                        projectTreeViewItem.Items.RemoveAt(i);
                    //Называем проект в дереве проектов так же, как файл
                    int length = openDlg.SafeFileName.Length;
                    projectTreeViewItem.Header = openDlg.SafeFileName.Substring(0, length - 8);

                    TreeViewItem apparatItem = new TreeViewItem();
                    apparatItem.Header = "Устройство 1";
                    apparatItem.Name = "apparatusTereeItem1";

                    TreeViewItem docItem = new TreeViewItem();
                    docItem.Header = "Перечень элементов";
                    docItem.IsSelected = true;
                    docItem.Selected += treeSelectionChanged;
                    apparatItem.Items.Add(docItem);
                    docItem = new TreeViewItem();
                    docItem.Header = "Спецификация";
                    docItem.Selected += treeSelectionChanged;
                    apparatItem.Items.Add(docItem);
                    docItem = new TreeViewItem();
                    docItem.Header = "Ведомость";
                    docItem.Selected += treeSelectionChanged;
                    apparatItem.Items.Add(docItem);
                    projectTreeViewItem.Items.Add(apparatItem);

                    projectTreeViewItem.ExpandSubtree();

                    //Копируем данные во временные
                    perTempSave = new TempSaves();
                    for (int i = 1; i <= project.GetPerechenLength(0); i++)
                    {
                        PerechenItem perItem = new PerechenItem();
                        perItem = project.GetPerechenItem(i, 0);
                        perItem.id = id.makeID(i, perTempSave.GetCurrent());
                        project.AddPerechenItem(perItem);
                    }
                    perTempSave.ProjectSaved();

                    for (int i = 1; i <= project.GetSpecLength(0); i++)
                    {
                        SpecificationItem specItem = new SpecificationItem();
                        specItem = project.GetSpecItem(i, 0);
                        specItem.id = id.makeID(i, specTempSave.GetCurrent());
                        project.AddSpecItem(specItem);
                    }
                    specTempSave.ProjectSaved();

                    for (int i = 1; i <= project.GetVedomostLength(0); i++)
                    {
                        VedomostItem vedomostItem = new VedomostItem();
                        vedomostItem = project.GetVedomostItem(i, 0);
                        vedomostItem.id = id.makeID(i, specTempSave.GetCurrent());
                        project.AddVedomostItem(vedomostItem);
                    }
                    vedomostTempSave.ProjectSaved();
                                        
                    DisplayAllValues();

                    importMenuItem.IsEnabled = true;
                    saveProjectMenuItem.IsEnabled = true;
                    undoMenuItem.IsEnabled = true;
                    redoMenuItem.IsEnabled = true;
                    createPdfMenuItem.IsEnabled = true;
                    osnNadpisMenuItem.IsEnabled = true;
                    osnNadpisButton.IsEnabled = true;
                }                
            }
        }

        /// <summary> Импорт данных из проекта Altium Designer (.PrjPcb) </summary>
        private void ImportPrjPcbfromAD_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.Title = "Выбор файла проекта AltiumDesigner";
            openDlg.Multiselect = false;
            openDlg.Filter = "Файлы проекта AD (*.PrjPcb)|*.PrjPcb";
            if (openDlg.ShowDialog() == true)
            {
                perTempSave = new TempSaves();
                specTempSave = new TempSaves();
                vedomostTempSave = new TempSaves();

                #region Открытие и парсинг файла проекта Altium
                string pcbPrjFilePath = openDlg.FileName;
                string pcbPrjFolderPath = Path.GetDirectoryName(pcbPrjFilePath);

                //Открываем файл проекта AD для поиска имён файлов схемы:
                FileStream pcbPrjFile = new FileStream(pcbPrjFilePath, FileMode.Open, FileAccess.Read);
                StreamReader pcbPrjReader = new StreamReader(pcbPrjFile, System.Text.Encoding.Default);

                String prjStr;

                int numberOfStrings = 0;

                List<List<ComponentProperties>> componentsList = new List<List<ComponentProperties>>();
                List<ComponentProperties> componentPropList = new List<ComponentProperties>();

                List<ComponentProperties> otherPropList = new List<ComponentProperties>();

                SpecificationItem plataSpecItem = new SpecificationItem(); //для хранения записи печатной платы для добавления в раздел "Детали" спецификации

                plataSpecItem.position = "Авто";               
                plataSpecItem.name = "Плата печатная";                
                plataSpecItem.quantity = "1";
                plataSpecItem.spSection = (int)Global.SpSections.Details;

                while ((prjStr = pcbPrjReader.ReadLine()) != null)
                {
                    string schPath = String.Empty;
                    if (prjStr.Length > 13)
                        if ((prjStr.Substring(0, 13) == "DocumentPath=") & (prjStr.Substring(prjStr.Length - 6, 6) == "SchDoc"))
                        {
                            schPath = Path.Combine(pcbPrjFolderPath, prjStr.Substring(13));

                            //Открываем файл схемы AD для получения списка параметров электронных компонентов:
                            FileStream schFile = new FileStream(schPath, FileMode.Open, FileAccess.Read);
                            StreamReader schReader = new StreamReader(schFile, System.Text.Encoding.Default);

                            string schStr = String.Empty;

                            bool isNoBom = false;
                            bool isComponent = false;

                            while ((schStr = schReader.ReadLine()) != null)
                            {
                                string[] schStrArray = schStr.Split(new Char[] { '|' });
                                for (int i = 0; i < schStrArray.Length - 1; i++)
                                {                                    
                                    if (isComponent == true)
                                    {
                                        if (schStrArray[i].Length >= 23)
                                            if (schStrArray[i].Substring(0, 23) == "COMPONENTKINDVERSION2=5") isNoBom = true;

                                        if ((schStrArray[i].Length > 5) & (schStrArray[i + 1].Length > 5))
                                            if ((schStrArray[i].Substring(0, 5) == "TEXT=") & (schStrArray[i + 1].Substring(0, 5) == "NAME="))
                                            {
                                                ComponentProperties prop = new ComponentProperties();
                                                prop.Name = schStrArray[i + 1].Substring(5);
                                                prop.Text = schStrArray[i].Substring(5);
                                                if (isNoBom == false)
                                                //if ((prop.Name == "Designator") | (prop.Name == "SType") | (prop.Name == "Docum") | (prop.Name == "Note"))
                                                componentPropList.Add(prop);
                                            }

                                        if (schStrArray[i].Length >= 8)
                                            if ((schStrArray[i].Substring(0, 7) == "HEADER=") |((schStrArray[i].Substring(0, 8) == "RECORD=1")&(schStrArray[i].Length==8))) //Считаем, что описание каждого компонента заканчивается этой фразой
                                            {
                                                if ((isNoBom == false) & (componentPropList.Count > 0))
                                                {
                                                    componentsList.Add(componentPropList);
                                                    
                                                    numberOfStrings++;
                                                }

                                                isNoBom = false;
                                                isComponent = false;

                                                componentPropList = new List<ComponentProperties>();
                                            }
                                    }
                                    else if (isComponent == false)
                                    {
                                        //Запись полей основной надписи в базу данных проекта, если они встречаются в файле Sch:
                                        if ((schStrArray[i].Length > 5) & (schStrArray[i + 1].Length > 5))
                                            if ((schStrArray[i].Substring(0, 5) == "TEXT=") & (schStrArray[i + 1].Substring(0, 5) == "NAME="))
                                            {
                                                ComponentProperties prop = new ComponentProperties();
                                                prop.Name = schStrArray[i + 1].Substring(5);
                                                prop.Text = schStrArray[i].Substring(5);

                                                otherPropList.Add(prop);
                                            }
                                    }
                                    //Теперь ищем все записи компонента и сохраняем их
                                    if (schStrArray[i].Length == 8)
                                        if (schStrArray[i].Substring(0, 8) == "RECORD=1") //Считаем, что описание каждого компонента начинается этой фразой
                                            isComponent = true;
                                }
                            }

                            //Работа с составными именами типа ='prop1'+'prop2'
                            for (int i = 0; i < otherPropList.Count; i++)
                            {
                                otherPropList[i].Text = MakeComplexStringIfItIs(otherPropList[i].Text, otherPropList);
                            }

                            schFile.Close();
                        }
                        else if ((prjStr.Substring(0, 13) == "DocumentPath=") & (prjStr.Substring(prjStr.Length - 6, 6) == "PcbDoc"))
                        {
                            //Добавляем плату в спецификацию ВРЕМЕННО! в раздел "Детали"                            
                            plataSpecItem.oboznachenie = prjStr.Substring(13, prjStr.Length - 20);
                            plataSpecItem.oboznachenie = plataSpecItem.oboznachenie.Split(new char[] { '\\' }).Last();
                            plataSpecItem.name = "Плата печатная";                            

                        }

                }

                pcbPrjFile.Close();
                #endregion



                //Записываем в новый список названия всех свойств компонентов
                List<string> propNames = new List<string>();
                propNames.Add("<Не задано>");

                for (int i = 0; i < numberOfStrings; i++)
                {
                    for (int j = 0; j < (componentsList[i]).Count; j++)
                    {
                        string name = componentsList[i][j].Name;
                        if (propNames.Contains(name) == false) propNames.Add(name);
                    }
                }

                ImportPcbPrjWindow importPcbPrjWindow = new ImportPcbPrjWindow();

                importPcbPrjWindow.designatorComboBox.ItemsSource = propNames;
                if (propNames.Contains("Designator")) importPcbPrjWindow.designatorComboBox.SelectedIndex = propNames.FindIndex(x => x == "Designator");
                else
                {
                    importPcbPrjWindow.designatorComboBox.SelectedIndex = 0;
                    importPcbPrjWindow.nextButton.IsEnabled = false;
                    importPcbPrjWindow.nextButton.ToolTip = "Свойства \"Поз. обозначение\" и \"Наименование должны быть заданы\"";
                }
                importPcbPrjWindow.nameComboBox.ItemsSource = propNames;
                if (propNames.Contains("SType")) importPcbPrjWindow.nameComboBox.SelectedIndex = propNames.FindIndex(x => x == "SType");
                else
                {
                    importPcbPrjWindow.nameComboBox.SelectedIndex = 0;
                    importPcbPrjWindow.nextButton.IsEnabled = false;
                    importPcbPrjWindow.nextButton.ToolTip = "Свойства \"Поз. обозначение\" и \"Наименование должны быть заданы\"";
                }

                importPcbPrjWindow.documComboBox.ItemsSource = propNames;
                if (propNames.Contains("Docum")) importPcbPrjWindow.documComboBox.SelectedIndex = propNames.FindIndex(x => x == "Docum");
                else importPcbPrjWindow.documComboBox.SelectedIndex = 0;
                importPcbPrjWindow.noteComboBox.ItemsSource = propNames;
                if (propNames.Contains("Note")) importPcbPrjWindow.noteComboBox.SelectedIndex = propNames.FindIndex(x => x == "Note");
                else importPcbPrjWindow.noteComboBox.SelectedIndex = 0;

                if (importPcbPrjWindow.ShowDialog() == true)
                {
                    #region Заполнение списков для базы данных проекта
                    createPdfMenuItem.IsEnabled = true;

                    List<PerechenItem> perechenList = new List<PerechenItem>();
                    List<SpecificationItem> specList = new List<SpecificationItem>();
                    List<VedomostItem> vedomostList = new List<VedomostItem>();

                    int numberOfValidStrings = 0;

                    SpecificationItem tempSpecItem = new SpecificationItem();
                    numberOfValidStrings++;
                    tempSpecItem.id = id.makeID(numberOfValidStrings, specTempSave.GetCurrent());
                    tempSpecItem.name = "Схема электрическая принципиальная";
                    tempSpecItem.quantity = String.Empty;
                    tempSpecItem.spSection = (int)Global.SpSections.Documentation;
                    specList.Add(tempSpecItem);

                    tempSpecItem = new SpecificationItem();
                    numberOfValidStrings++;
                    tempSpecItem.id = id.makeID(numberOfValidStrings, specTempSave.GetCurrent());
                    tempSpecItem.name = "Перечень элементов";
                    tempSpecItem.quantity = String.Empty;
                    tempSpecItem.spSection = (int)Global.SpSections.Documentation;
                    specList.Add(tempSpecItem);

                    tempSpecItem = new SpecificationItem();
                    numberOfValidStrings++;
                    tempSpecItem.id = id.makeID(numberOfValidStrings, specTempSave.GetCurrent());
                    tempSpecItem.name = "Ведомость покупных изделий";
                    tempSpecItem.quantity = String.Empty;
                    tempSpecItem.spSection = (int)Global.SpSections.Documentation;
                    specList.Add(tempSpecItem);

                    numberOfValidStrings++;
                    plataSpecItem.id = id.makeID(numberOfValidStrings, specTempSave.GetCurrent());
                    specList.Add(plataSpecItem);


                    for (int i = 0; i < numberOfStrings; i++)
                    {
                        PerechenItem tempPerechen = new PerechenItem();
                        SpecificationItem tempSpecification = new SpecificationItem();
                        VedomostItem tempVedomost = new VedomostItem();

                        numberOfValidStrings++;
                        tempPerechen.id = id.makeID(numberOfValidStrings, perTempSave.GetCurrent());
                        tempPerechen.designator = string.Empty;
                        tempPerechen.name = string.Empty;
                        tempPerechen.quantity = "1";
                        tempPerechen.note = String.Empty;
                        tempPerechen.docum = String.Empty;
                        tempPerechen.type = String.Empty;
                        tempPerechen.group = String.Empty;
                        tempPerechen.groupPlural = String.Empty;
                        tempPerechen.isNameUnderlined = false;
                        
                        for (int j = 0; j < (componentsList[i]).Count; j++)
                        {
                            ComponentProperties prop;
                            try
                            {
                                prop = (componentsList[i])[j];

                                string designatorName = importPcbPrjWindow.designatorComboBox.SelectedItem.ToString();
                                string nameName = importPcbPrjWindow.nameComboBox.SelectedItem.ToString();
                                string documName = importPcbPrjWindow.documComboBox.SelectedItem.ToString();
                                string noteName = importPcbPrjWindow.noteComboBox.SelectedItem.ToString();

                                if (prop.Name == designatorName) tempPerechen.designator = prop.Text;
                                else if (prop.Name == nameName) tempPerechen.name = prop.Text;
                                else if (prop.Name == documName) tempPerechen.docum = prop.Text;
                                else if (prop.Name == noteName) tempPerechen.note = prop.Text;
                            }
                            catch
                            {
                                MessageBox.Show(j.ToString() + "/" + ((componentsList[i]).Capacity - 1).ToString() + ' ' + i.ToString() + "/" + numberOfStrings);
                            }


                        }

                        tempSpecification.id = tempPerechen.id;
                        tempSpecification.spSection = (int)Global.SpSections.Other;
                        tempSpecification.format = String.Empty;
                        tempSpecification.zona = String.Empty;
                        tempSpecification.position = String.Empty;
                        tempSpecification.oboznachenie = String.Empty;
                        tempSpecification.name = tempPerechen.name;
                        tempSpecification.quantity = "1";
                        tempSpecification.note = tempPerechen.designator;
                        tempSpecification.group = String.Empty;
                        tempSpecification.docum = tempPerechen.docum;
                        tempSpecification.designator = tempPerechen.designator;
                        tempSpecification.isNameUnderlined = false;

                        tempVedomost.id = tempPerechen.id;
                        tempVedomost.designator = tempPerechen.designator;
                        tempVedomost.name = tempPerechen.name;
                        tempVedomost.kod = String.Empty;
                        tempVedomost.docum = tempPerechen.docum;
                        tempVedomost.supplier = String.Empty;
                        tempVedomost.belongs = String.Empty;
                        tempVedomost.quantityIzdelie = "1";
                        tempVedomost.quantityComplects = String.Empty;
                        tempVedomost.quantityTotal = "1";
                        tempVedomost.note = String.Empty;
                        tempVedomost.isNameUnderlined = false;

                        string group = string.Empty;

                        DesignatorDB designDB = new DesignatorDB();
                        int descrDBLength = designDB.GetLength();

                        for (int j = 0; j < descrDBLength; j++)
                        {
                            DesignatorDescriptionItem desDescr = designDB.GetItem(j + 1);

                          if ((desDescr.Designator == tempPerechen.designator.Substring(0, 1)) | (desDescr.Designator == tempPerechen.designator.Substring(0, 2)))
                            {
                                tempPerechen.group = desDescr.Group.Substring(0, 1).ToUpper() + desDescr.Group.Substring(1, desDescr.Group.Length - 1).ToLower();
                                tempPerechen.groupPlural = desDescr.GroupPlural.Substring(0, 1).ToUpper() + desDescr.GroupPlural.Substring(1, desDescr.GroupPlural.Length - 1).ToLower();

                                group = tempPerechen.group;

                                tempSpecification.group = group;
                                tempVedomost.group = group;
                                tempVedomost.groupPlural = tempPerechen.groupPlural;
                            }
                        }


                        tempSpecification.name = group + " " + tempSpecification.name + " " + tempSpecification.docum;

                        perechenList.Add(tempPerechen);
                        specList.Add(tempSpecification);
                        vedomostList.Add(tempVedomost);

                    }

                    //Сортировка по поз. обозначению
                    List<PerechenItem> perechenListSorted = new List<PerechenItem>();
                    List<SpecificationItem> specOtherListSorted = new List<SpecificationItem>();
                    List<VedomostItem> vedomostListSorted = new List<VedomostItem>();

                    perechenListSorted = perechenList.OrderBy(x => MakeDesignatorForOrdering(x.designator)).ToList();
                    specOtherListSorted = specList.Where(x => x.spSection == ((int)Global.SpSections.Other)).OrderBy(x => MakeDesignatorForOrdering(x.designator)).ToList();
                    vedomostListSorted = vedomostList.OrderBy(x => MakeDesignatorForOrdering(x.designator)).ToList();

                    for (int i = 0; i < specOtherListSorted.Count; i++)
                    {
                        perechenListSorted[i].id = id.makeID(i + 1, perTempSave.GetCurrent());
                        specOtherListSorted[i].id = id.makeID(i + 1 + (numberOfValidStrings - specOtherListSorted.Count), specTempSave.GetCurrent());
                        vedomostListSorted[i].id = id.makeID(i + 1, vedomostTempSave.GetCurrent());
                    }

                    saveProjectMenuItem.IsEnabled = true;
                    undoMenuItem.IsEnabled = true;
                    redoMenuItem.IsEnabled = true;
                    #endregion
                    groupByName(perechenListSorted, specList.Where(x => x.spSection != ((int)Global.SpSections.Other)).ToList(), specOtherListSorted, vedomostListSorted);
                }
            }
        }

        /// <summary> Формирование числа типа int для правильной сортировки позиционных обозначений,
        /// так как в случае простой сортировки по алфавиту результат неправильный, например,
        /// сортируется C1, C15, C2 вместо C1, C2< C15.<</summary>
        private int MakeDesignatorForOrdering(string designator)
        {
            int result = 0;
            if (designator.Length>1)
            {
                if (Char.IsDigit(designator[1]))
                {
                    result = ((designator[0])<<24) + (int.Parse(designator.Substring(1,designator.Length - 1))<<8);
                }
                else
                {
                    result = ((designator[0]) << 24) + (int.Parse(designator.Substring(2, designator.Length - 2)) << 8) + designator[1];
                }
            }
                       
            return result;
        }

        /// <summary> Чтение строк вида ='Str1'+'Str2' при чтении данных из проекта Altium Designer (.PrjPcb) </summary>
        private string MakeComplexStringIfItIs(string text, List<ComponentProperties> componentsList)
        {
            bool isComplexName = false;
            string result = text;

            if (text[0] == '=')
            {
                isComplexName = true;
                string tempStr = String.Empty;
                string origStr = text.Substring(1); //Удаляем пробелы и знак равно в начале строки
                string[] otherPropNames = origStr.Split(new char[] { '+' });
                for (int j = 0; j < otherPropNames.Length; j++)
                {
                    if ((otherPropNames[j].First() == '\'') & (otherPropNames[j].Last() == '\''))
                        tempStr += otherPropNames[j].Substring(1, otherPropNames[j].Length - 2);
                    else
                    {
                        try
                        {
                            tempStr += componentsList.Where(x => x.Name == otherPropNames[j]).First().Text;
                        }
                        catch
                        {
                            isComplexName = false;
                        }
                    }
                }
                if (isComplexName) result = tempStr;
            }
            return result;
        }

        /// <summary> Отображение данных спецификации и перечня пользователю из текущего временного сохранения проекта </summary>
        private void DisplayAllValues()
        {
            //Вывод данных перечня в окно программы:
            int length = project.GetPerechenLength(perTempSave.GetCurrent());            
            List<PerechenItem> resultPer = new List<PerechenItem>(length);

            for (int i = 1; i <= length; i++)
            {
                resultPer.Add(project.GetPerechenItem(i, perTempSave.GetCurrent()));
            }

            DisplayPerValues(resultPer);

            //Вывод данных спецификации в окно программы:
            length = project.GetSpecLength(specTempSave.GetCurrent());
            List<SpecificationItem> resultSpec = new List<SpecificationItem>(length);

            for (int i = 1; i <= length; i++)
            {
                resultSpec.Add(project.GetSpecItem(i, specTempSave.GetCurrent()));
            }

            DisplaySpecValues(resultSpec);
            
            //Вывод данных ведомости в окно программы:
            length = project.GetVedomostLength(vedomostTempSave.GetCurrent());
            List<VedomostItem> resultVedomost = new List<VedomostItem>(length);

            for (int i = 1; i <= length; i++)
            {
                resultVedomost.Add(project.GetVedomostItem(i, vedomostTempSave.GetCurrent()));
            }

            DisplayVedomostValues(resultVedomost);
        }

        /// <summary>
        /// Отображение данных спецификации для пользователя
        /// </summary>
        /// <param name="pData"> Список с данными спецификации для отображения</param>
        private void DisplaySpecValues(List<SpecificationItem> sData)
        {
            //Вывод данных в окно программы:
            if (sData == null)
            {
                documSpecListView.ItemsSource = null;
                compleksiSpecListView.ItemsSource = null;
                sborEdSpecListView.ItemsSource = null;
                detailsSpecListView.ItemsSource = null;
                standartSpecListView.ItemsSource = null;
                otherSpecListView.ItemsSource = null;
                materialsSpecListView.ItemsSource = null;
                complectsSpecListView.ItemsSource = null;
            }
            else
            {
                
                //Заполнение раздела "Документация"
                
                List<SpecificationItem> documRazdelList = (sData.Where(x => x.spSection == (int)Global.SpSections.Documentation)).ToList();               
                documSpecListView.ItemsSource = documRazdelList;

                //Заполнение раздела "Комплексы"
                 List<SpecificationItem> compleksiRazdelList = (sData.Where(x => x.spSection == (int)Global.SpSections.Compleksi)).ToList();                
                compleksiSpecListView.ItemsSource = compleksiRazdelList;

                //Заполнение раздела "Сборочные единицы"
                List<SpecificationItem> sborEdRazdelList = (sData.Where(x => x.spSection == (int)Global.SpSections.SborEd)).ToList();
               sborEdSpecListView.ItemsSource = sborEdRazdelList;

                //Заполнение раздела "Детали"
                List<SpecificationItem> detailsRazdelList = (sData.Where(x => x.spSection == (int)Global.SpSections.Details)).ToList();
                detailsSpecListView.ItemsSource = detailsRazdelList;

                //Заполнение раздела "Стандартные изделия"
                List<SpecificationItem> standartRazdelList = (sData.Where(x => x.spSection == (int)Global.SpSections.Standard)).ToList();
                standartSpecListView.ItemsSource = standartRazdelList;

                //Заполнение раздела "Прочие"
                List<SpecificationItem> otherRazdelList = (sData.Where(x => x.spSection == (int)Global.SpSections.Other)).ToList();
                otherRazdelList = sData.Where(x => x.spSection == (int)Global.SpSections.Other).ToList();
               otherSpecListView.ItemsSource = otherRazdelList;

                //Заполнение раздела "Материалы"
                List<SpecificationItem> materialsRazdelList = (sData.Where(x => x.spSection == (int)Global.SpSections.Materials)).ToList();
                materialsSpecListView.ItemsSource = materialsRazdelList;

                //Заполнение раздела "Комплекты"
                List<SpecificationItem> complectsRazdelList = (sData.Where(x => x.spSection == (int)Global.SpSections.Compleсts)).ToList();
                complectsSpecListView.ItemsSource = complectsRazdelList;
            }

        }

        /// <summary>
        /// Отображение данных перечня для пользователя
        /// </summary>
        /// <param name="pData"> Список с данными перечня элементов для отображения</param>
        private void DisplayPerValues(List<PerechenItem> pData)
        {
            //Вывод данных в окно программы: 
            perechenListView.ItemsSource = pData;
            
            //Вставляем подчёркивания:
           /* foreach (ListViewItem item in perechenListView.Items)
            {
                //if (item.Name)
                TextBlock[] tb = new TextBlock[2];
                tb[0].TextDecorations = TextDecorations.Underline;
                tb[0].Text = "fdfdf";
                tb[1].TextDecorations = TextDecorations.Underline;
                tb[1].Text = "asdfasdf";
                item.Content = tb;
            }*/
        }

        /// <summary>
        /// Отображение данных ведомости для пользователя
        /// </summary>
        /// <param name="vData"> Список с данными ведомости для отображения</param>
        private void DisplayVedomostValues(List<VedomostItem> vData)
        {
            //Вывод данных в окно программы: 
            vedomostListView.ItemsSource = vData;            
        }

        /// <summary> Создание PDF-файлов перечня и спецификации </summary>
        private void CreatePdf_Click(object sender, RoutedEventArgs e)
        {
            string pdfPath = "ПЭ3.pdf";
            PdfOperations pdf = new PdfOperations(projectPath);
            int startPage = (startFromSecondCheckBox.IsChecked == false) ? 1 : 2;
            bool addListRegistr = (addListRegistrCheckBox.IsChecked == true);
            pdf.CreatePerechen(pdfPath, startPage, addListRegistr, perTempSave.GetCurrent());
            System.Diagnostics.Process.Start(pdfPath); //открываем pdf файл

            pdfPath = "Спецификация.pdf";
            pdf = new PdfOperations(projectPath);
            pdf.CreateSpecification(pdfPath, startPage, addListRegistr, specTempSave.GetCurrent());
            System.Diagnostics.Process.Start(pdfPath); //открываем pdf файл

            pdfPath = "ВП.pdf";
            pdf = new PdfOperations(projectPath);
            pdf.CreateVedomost(pdfPath, startPage, addListRegistr, vedomostTempSave.GetCurrent());
            System.Diagnostics.Process.Start(pdfPath); //открываем pdf файл
        }

        /// <summary> При закрытии окна программы выполняется проверка, сохранён ли проект, и показывается соответствующее диалоговое окно </summary>        
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult closingDialogResult = new MessageBoxResult();
            if ((perTempSave != null) & (specTempSave != null) & (vedomostTempSave != null))
            {
                if ((perTempSave.GetCurrent() != perTempSave.GetLastSavedState()) |
                    (specTempSave.GetCurrent() != specTempSave.GetLastSavedState()) |
                    (vedomostTempSave.GetCurrent() != vedomostTempSave.GetLastSavedState()))
                {
                    closingDialogResult = MessageBox.Show("Проект не сохранён. Сохранить проект перед закрытием?", "Сохранение проекта", MessageBoxButton.YesNo);
                    if (closingDialogResult == MessageBoxResult.Yes) project.Save(perTempSave.GetCurrent(), specTempSave.GetCurrent(), vedomostTempSave.GetCurrent());
                }
                project.DeleteTempData();
            }                
        }

        /// <summary> Группировка элементов перечня и спецификации, их запись в базу данных проекта и отображение пользователю </summary>
        /// <remarks> Вызывается после импорта новых данных из файла </remarks>
        /// <param name="pData"> Список с данными для перечня элементов, которые будут сгруппированы</param>
        /// <param name="sData"> Список с данными спецификации без данных раздела "Прочие изделия"</param>
        /// <param name="sOtherData"> Список с данными спецификации из раздела данных "Прочие изделия", которые будут сгруппированы</param>
        /// <param name="vData"> Список с данными для ведомости, которые будут сгруппированы</param>
        private void groupByName(List<PerechenItem> pData, List<SpecificationItem> sData, List<SpecificationItem> sOtherData, List<VedomostItem> vData)
        {

            int numOfPerechenValidStrings = pData.Count;
            int numOfSpecificationStrings = sOtherData.Count;
            int numOfVedomostValidStrings = vData.Count;
                        

            (new PerechenOperations()).groupPerechenElements(ref pData, ref numOfPerechenValidStrings);
            
            sOtherData = (new SpecificationOperations()).groupSpecificationElements(sOtherData, ref numOfSpecificationStrings);

            vData = (new VedomostOperations()).groupVedomostElements(vData, ref numOfVedomostValidStrings);

            //Запись значений в файл и вывод сгруппированных строк в окно программы:

            List<PerechenItem> perResult = new List<PerechenItem>(numOfPerechenValidStrings);
            List<SpecificationItem> specResult = new List<SpecificationItem>(numOfSpecificationStrings + sData.Count);
            List<VedomostItem> vedomostResult = new List<VedomostItem>(numOfVedomostValidStrings);
            
            for (int i = 0; i < numOfPerechenValidStrings; i++)
            {
                PerechenItem pd = pData[i];
                pd.name += " " + pd.docum;
                pd.id = id.makeID(i + 1, perTempSave.GetCurrent());
                perResult.Add(pd);
                project.AddPerechenItem(pd);
            }

            for (int i = 0; i < numOfSpecificationStrings + sData.Count; i++)
            {
                if (i < sData.Count())
                {
                    SpecificationItem sd = sData[i];
                    sd.id = id.makeID(i + 1, specTempSave.GetCurrent());
                    specResult.Add(sd);
                    project.AddSpecItem(sd);
                }
                else
                {
                    SpecificationItem sd = sOtherData[i - sData.Count()];
                    sd.id = id.makeID(i + 1, specTempSave.GetCurrent());
                    specResult.Add(sd);
                    project.AddSpecItem(sd);
                }
            }

            for (int i = 0; i < numOfVedomostValidStrings; i++)
            {
                VedomostItem vd = vData[i];               
                vd.id = id.makeID(i + 1, perTempSave.GetCurrent());
                vedomostResult.Add(vd);
                project.AddVedomostItem(vd);
            }

            DisplayPerValues(perResult);
            DisplaySpecValues(specResult);
            DisplayVedomostValues(vedomostResult);
        }

        /// <summary> Редактирование основной надписи </summary>
        private void osnNadpisButton_Click(object sender, RoutedEventArgs e)
        {
            OsnNadpisWindow osnNadpis;
            osnNadpis = new OsnNadpisWindow(projectPath);
            //Показываем диалоговое окно для заполнения граф основной надписи
            osnNadpis.ShowDialog();

            osnNadpis.Close();
        }

        /// <summary> Редактирование расшифровок позиционных обозначений </summary>
        private void desigDescrButton_Click(object sender, RoutedEventArgs e)
        {
            DesigDescrWindow ddWindow = new DesigDescrWindow();
            ddWindow.ShowDialog();
        }

        /// <summary> Изменение ширины столбцов элементов ListView при изменении размеров окна </summary>
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double coef = (perechenListView.ActualWidth - 270) / 742; //742 - сумма ширин всех столбцов, кроме 3 кнопок постоянной ширины по 90 = 270
            designatorPerechenColumn.Width = 120 * coef;
            namePerechenColumn.Width = 410 * coef;
            quantityPerechenColumn.Width = 70 * coef;
            notePerechenColumn.Width = 120 * coef;


            formatSpecColumn.Width = 30 * coef;
            zonaSpecColumn.Width = 30 * coef;
            positionSpecColumn.Width = 30 * coef;
            oboznachSpecColumn.Width = 280 * coef;
            nameSpecColumn.Width = 280 * coef;
            quantitySpecColumn.Width = 30 * coef;
            noteSpecColumn.Width = 40 * coef;

            coef = (vedomostListView.ActualWidth - 210) / 1230; //1230 - сумма ширин всех столбцов, кроме 3 кнопок постоянной ширины по 90 + 60 +60 = 210
            nameVedomostColumn.Width = 210 * coef;
            kodVedomostColumn.Width = 70 * coef;
            documVedomostColumn.Width = 180 * coef;
            supplierVedomostColumn.Width = 110 * coef;
            belongsVedomostColumn.Width = 110 * coef;
            quantityIzdelieVedomostColumn.Width = 120 * coef;
            quantityComplectsVedomostColumn.Width = 120 * coef;
            quantityRegulVedomostColumn.Width = 120 * coef;
            quantityTotalVedomostColumn.Width = 50 * coef;
            noteVedomostColumn.Width = 140 * coef;
        }

        /// <summary> Правка текущей строки перечня </summary>
        private void PerechenEdit_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;

            // Создаём список элементов перечня, хранящийся в оперативной памяти для ускорения работы программы:
            int perLength = project.GetPerechenLength(perTempSave.GetCurrent());
            List<PerechenItem> tempPerList = new List<PerechenItem>(perLength);

            for (int i = 1; i <= perLength; i++)
            {
                tempPerList.Add(project.GetPerechenItem(i, perTempSave.GetCurrent()));
            }

            int strNum = id.getStrNum((b.CommandParameter as PerechenItem).id);


            EditPerechenItemWindow editWindow = new EditPerechenItemWindow(projectPath, strNum, perTempSave.GetCurrent(), perTempSave.GetCurrent() + 1);
            if (editWindow.ShowDialog() == true)
            {
                project.DeletePerechenTempData(perTempSave.SetNext()); // Увеличиваем номер текущего сохранения и одновременно удаляем все последующие сохранения

                for (int i = 1; i <= perLength; i++)
                {
                    if (i != strNum)
                    {
                        tempPerList[i - 1].id = id.makeID(i, perTempSave.GetCurrent());
                        project.AddPerechenItem(tempPerList[i - 1]);
                    }

                }

                tempPerList[strNum - 1] = project.GetPerechenItem(strNum, perTempSave.GetCurrent());
                project.AddPerechenItem(tempPerList[strNum - 1]);

                DisplayPerValues(tempPerList);
            }


        }

        /// <summary> Добавление к перечню пустой строки сверху </summary>
        private void PerechenAdd_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            int strNum = id.getStrNum((b.CommandParameter as PerechenItem).id);
            int length = project.GetPerechenLength(perTempSave.GetCurrent());
            List<PerechenItem> perList = new List<PerechenItem>();

            int prevTempSave = perTempSave.GetCurrent();
            project.DeletePerechenTempData(perTempSave.GetCurrent()); // Удаляем все последующие сохранения
            int currentTempSave = perTempSave.SetNext(); // Увеличиваем номер текущего сохранения

            for (int i = 1; i <= length + 1; i++)
            {
                PerechenItem perItem = new PerechenItem();

                if (i < strNum)
                {
                    perItem = project.GetPerechenItem(i, prevTempSave);
                    perItem.id = id.makeID(i, currentTempSave);
                    perList.Add(perItem);
                }
                else if (i == strNum)
                {
                    perItem.id = id.makeID(i, currentTempSave);
                    perList.Add(perItem);
                }
                else if (i > strNum)
                {
                    perItem = project.GetPerechenItem(i - 1, prevTempSave);
                    perItem.id = id.makeID(i, currentTempSave);
                    perList.Add(perItem);
                }

            }

            for (int i = 0; i < perList.Count; i++) project.AddPerechenItem(perList[i]);

            DisplayPerValues(perList);
        }

        /// <summary> Удаление из перечня текущей строки </summary>
        private void PerechenDelete_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            int strNum = id.getStrNum((b.CommandParameter as PerechenItem).id);
            int length = project.GetPerechenLength(perTempSave.GetCurrent());
            List<PerechenItem> perList = new List<PerechenItem>();

            int prevTempSave = perTempSave.GetCurrent();
            project.DeletePerechenTempData(perTempSave.GetCurrent()); // Удаляем все последующие сохранения
            perTempSave.SetNext(); // Увеличиваем номер текущего сохранения
            int currentTempSave = perTempSave.GetCurrent();

            for (int i = 1; i <= length; i++)
            {
                PerechenItem perItem = new PerechenItem();

                if (i < strNum)
                {
                    perItem = project.GetPerechenItem(i, prevTempSave);
                    perItem.id = id.makeID(i, currentTempSave);
                    perList.Add(perItem);
                }
                else if (i > strNum)
                {
                    perItem = project.GetPerechenItem(i, prevTempSave);
                    perItem.id = id.makeID(i - 1, currentTempSave);
                    perList.Add(perItem);
                }

            }

            for (int i = 0; i < perList.Count; i++) project.AddPerechenItem(perList[i]);

            DisplayPerValues(perList);
        }

        /// <summary> Правка текущей строки спецификации </summary>
        private void SpecEdit_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;

            // Создаём список элементов перечня, хранящийся в оперативной памяти для ускорения работы программы:
            int length = project.GetSpecLength(specTempSave.GetCurrent());
            List<SpecificationItem> tempList = new List<SpecificationItem>(length);

            for (int i = 1; i <= length; i++)
            {
                tempList.Add(project.GetSpecItem(i, specTempSave.GetCurrent()));
            }

            int strNum = id.getStrNum((b.CommandParameter as SpecificationItem).id);
            int tempNum = specTempSave.GetCurrent();

            EditSpecItemWindow editWindow = new EditSpecItemWindow(projectPath, strNum, tempNum, specTempSave.GetCurrent() + 1);
            if (editWindow.ShowDialog() == true)
            {
                project.DeleteSpecTempData(specTempSave.SetNext()); // Увеличиваем номер текущего сохранения и одновременно удаляем все последующие сохранения               

                for (int i = 1; i <= length; i++)
                {
                    if (i != strNum)
                    {
                        tempList[i - 1].id = id.makeID(i, specTempSave.GetCurrent());
                        project.AddSpecItem(tempList[i - 1]);
                    }

                }

                tempList[strNum - 1] = project.GetSpecItem(strNum, specTempSave.GetCurrent());

                DisplaySpecValues(tempList);
            }
        }

        /// <summary>
        /// Возвращает номер строки элемента, следующего за последним элементом в разделе спецификации
        /// Используется при добавлении строки спецификации снизу
        /// </summary>
        /// <param name="section">Раздел спецификации</param>
        private int GetSpecNextNumInSection(Global.SpSections section)
        {
            int strNum = 1;
            
            List<SpecificationItem> sData = project.GetSpecificationList(specTempSave.GetCurrent());
            for (int i = (int)Global.SpSections.Documentation; i <= (int)section; i++)
                strNum += (sData.Where(x => x.spSection == i)).Count();
           
            return strNum;
        }

        /// <summary> Добавление к спецификации пустой строки сверху </summary>
        private void SpecAdd_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            int strNum;

            int specSection = 0;

            Global.SpSections section;
           
            if (b.Tag as string == "Документация")
            {
                //Определяем номер добавляемой строки
                strNum = GetSpecNextNumInSection(Global.SpSections.Documentation);
                specSection = (int)Global.SpSections.Documentation;
            }
            else
            if (b.Tag as string == "Комплексы")
            {
                //Определяем номер добавляемой строки
                strNum = GetSpecNextNumInSection(Global.SpSections.Compleksi);
                specSection = (int)Global.SpSections.Compleksi;
            }
            else
            if (b.Tag as string == "Сборочные единицы")
            {
                //Определяем номер добавляемой строки
                strNum = GetSpecNextNumInSection(Global.SpSections.SborEd);
                specSection = (int)Global.SpSections.SborEd;
            }
            else
            if (b.Tag as string == "Детали")
            {
                //Определяем номер добавляемой строки
                strNum = GetSpecNextNumInSection(Global.SpSections.Details);
                specSection = (int)Global.SpSections.Details;
            }
            else
            if (b.Tag as string == "Стандартные изделия")
            {
                //Определяем номер добавляемой строки
                strNum = GetSpecNextNumInSection(Global.SpSections.Standard);
                specSection = (int)Global.SpSections.Standard;
            }
            else
            if (b.Tag as string == "Прочие изделия")
            {
                //Определяем номер добавляемой строки
                strNum = GetSpecNextNumInSection(Global.SpSections.Other);
                specSection = (int)Global.SpSections.Other;
            }
            else
            if (b.Tag as string == "Материалы")
            {
                //Определяем номер добавляемой строки
                strNum = GetSpecNextNumInSection(Global.SpSections.Materials);
                specSection = (int)Global.SpSections.Materials;
            }
            else
            if (b.Tag as string == "Комплекты")
            {
                //Определяем номер добавляемой строки
                strNum = GetSpecNextNumInSection(Global.SpSections.Compleсts);
                specSection = (int)Global.SpSections.Compleсts;
            }
            else
            {
                strNum = id.getStrNum((b.CommandParameter as SpecificationItem).id);
                specSection = (b.CommandParameter as SpecificationItem).spSection;
            }

            int length = project.GetSpecLength(specTempSave.GetCurrent());
            List<SpecificationItem> specList = new List<SpecificationItem>();

            int prevTempSave = specTempSave.GetCurrent();
            project.DeleteSpecTempData(specTempSave.GetCurrent()); // Удаляем все последующие сохранения
            int currentTempSave = specTempSave.SetNext(); // Увеличиваем номер текущего сохранения 

            for (int i = 1; i <= length + 1; i++)
            {
                SpecificationItem specItem = new SpecificationItem();

                if (i < strNum)
                {
                    specItem = project.GetSpecItem(i, prevTempSave);
                    specItem.id = id.makeID(i, currentTempSave);
                }
                else if (i == strNum)
                {
                    specItem = new SpecificationItem();                    
                    specItem.id = id.makeID(i, currentTempSave);
                    specItem.spSection = specSection;
                    specItem.zona = String.Empty;
                    specItem.position = String.Empty;
                    specItem.oboznachenie = String.Empty;
                    specItem.name = String.Empty;
                    specItem.quantity = String.Empty;
                    specItem.note = String.Empty;
                }
                else if (i > strNum)
                {
                    specItem = project.GetSpecItem(i - 1, prevTempSave);
                    specItem.id = id.makeID(i, currentTempSave);
                }
                specList.Add(specItem);
            }

            for (int i = 0; i < specList.Count; i++) project.AddSpecItem(specList[i]);

            DisplaySpecValues(specList);
        }
        
        /// <summary> Удаление текущей строки спецификации </summary>
        private void SpecDelete_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            int strNum = id.getStrNum((b.CommandParameter as SpecificationItem).id);
            int length = project.GetSpecLength(specTempSave.GetCurrent());
            List<SpecificationItem> specList = new List<SpecificationItem>();

            int prevTempSave = specTempSave.GetCurrent();
            project.DeleteSpecTempData(specTempSave.GetCurrent()); // Удаляем все последующие сохранения
            int currentTempSave = specTempSave.SetNext(); // Увеличиваем номер текущего сохранения

            for (int i = 1; i <= length; i++)
            {
                SpecificationItem specItem = new SpecificationItem();

                if (i < strNum)
                {
                    specItem = project.GetSpecItem(i, prevTempSave);
                    specItem.id = id.makeID(i, currentTempSave);
                }
                else if (i > strNum)
                {
                    specItem = project.GetSpecItem(i, prevTempSave);
                    specItem.id = id.makeID(i - 1, currentTempSave);
                }
                if (i != strNum) specList.Add(specItem);

            }

            for (int i = 0; i < specList.Count; i++) project.AddSpecItem(specList[i]);

            DisplaySpecValues(specList);
        }

        /// <summary> Правка текущей строки ведомости </summary>
        private void VedomostEdit_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;

            // Создаём список элементов ведомости, хранящийся в оперативной памяти для ускорения работы программы:
            int vedomostLength = project.GetVedomostLength(perTempSave.GetCurrent());
            List<VedomostItem> tempVedomostList = new List<VedomostItem>(vedomostLength);

            for (int i = 1; i <= vedomostLength; i++)
            {
                tempVedomostList.Add(project.GetVedomostItem(i, vedomostTempSave.GetCurrent()));
            }

            int strNum = id.getStrNum((b.CommandParameter as VedomostItem).id);


            EditVedomostItemWindow editWindow = new EditVedomostItemWindow(projectPath, strNum, vedomostTempSave.GetCurrent(), vedomostTempSave.GetCurrent() + 1);
            if (editWindow.ShowDialog() == true)
            {
                project.DeleteVedomostTempData(vedomostTempSave.SetNext()); // Увеличиваем номер текущего сохранения и одновременно удаляем все последующие сохранения

                for (int i = 1; i <= vedomostLength; i++)
                {
                    if (i != strNum)
                    {
                        tempVedomostList[i - 1].id = id.makeID(i, vedomostTempSave.GetCurrent());
                        project.AddVedomostItem(tempVedomostList[i - 1]);
                    }

                }

                tempVedomostList[strNum - 1] = project.GetVedomostItem(strNum, vedomostTempSave.GetCurrent());
                project.AddVedomostItem(tempVedomostList[strNum - 1]);

                DisplayVedomostValues(tempVedomostList);
            }
    
        }

        /// <summary> Добавление к ведомости пустой строки сверху </summary>
        private void VedomostAdd_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            int strNum = id.getStrNum((b.CommandParameter as VedomostItem).id);
            int length = project.GetVedomostLength(vedomostTempSave.GetCurrent());
            List<VedomostItem> vedomostList = new List<VedomostItem>();

            int prevTempSave = vedomostTempSave.GetCurrent();
            project.DeleteVedomostTempData(vedomostTempSave.GetCurrent()); // Удаляем все последующие сохранения
            int currentTempSave = vedomostTempSave.SetNext(); // Увеличиваем номер текущего сохранения

            for (int i = 1; i <= length + 1; i++)
            {
                VedomostItem vedomostItem = new VedomostItem();

                if (i < strNum)
                {
                    vedomostItem = project.GetVedomostItem(i, prevTempSave);
                    vedomostItem.id = id.makeID(i, currentTempSave);
                    vedomostList.Add(vedomostItem);
                }
                else if (i == strNum)
                {
                    vedomostItem.id = id.makeID(i, currentTempSave);
                    vedomostList.Add(vedomostItem);
                }
                else if (i > strNum)
                {
                    vedomostItem = project.GetVedomostItem(i - 1, prevTempSave);
                    vedomostItem.id = id.makeID(i, currentTempSave);
                    vedomostList.Add(vedomostItem);
                }

            }

            for (int i = 0; i < vedomostList.Count; i++) project.AddVedomostItem(vedomostList[i]);

            DisplayVedomostValues(vedomostList);
        }

        /// <summary> Удаление из ведомости текущей строки </summary>
        private void VedomostDelete_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            int strNum = id.getStrNum((b.CommandParameter as VedomostItem).id);
            int length = project.GetVedomostLength(vedomostTempSave.GetCurrent());
            List<VedomostItem> vedomostList = new List<VedomostItem>();

            int prevTempSave = vedomostTempSave.GetCurrent();
            project.DeleteVedomostTempData(vedomostTempSave.GetCurrent()); // Удаляем все последующие сохранения
            vedomostTempSave.SetNext(); // Увеличиваем номер текущего сохранения
            int currentTempSave = vedomostTempSave.GetCurrent();

            for (int i = 1; i <= length; i++)
            {
                VedomostItem vedomostItem = new VedomostItem();

                if (i < strNum)
                {
                    vedomostItem = project.GetVedomostItem(i, prevTempSave);
                    vedomostItem.id = id.makeID(i, currentTempSave);
                    vedomostList.Add(vedomostItem);
                }
                else if (i > strNum)
                {
                    vedomostItem = project.GetVedomostItem(i, prevTempSave);
                    vedomostItem.id = id.makeID(i - 1, currentTempSave);
                    vedomostList.Add(vedomostItem);
                }

            }

            for (int i = 0; i < vedomostList.Count; i++) project.AddVedomostItem(vedomostList[i]);

            DisplayVedomostValues(vedomostList);
        }

        /// <summary> Сохранение проекта </summary>
        private void SaveProject_Click(object sender, RoutedEventArgs e)
        {
            if (saveProjectMenuItem.IsEnabled == true)
            {
                project.Save(perTempSave.GetCurrent(), specTempSave.GetCurrent(), vedomostTempSave.GetCurrent());

                perTempSave.ProjectSaved();
                specTempSave.ProjectSaved();
                vedomostTempSave.ProjectSaved();
            }
        }

        /// <summary> Отмена действия </summary>
        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {
            if (undoMenuItem.IsEnabled == true)
            {
                if (perechenListView.Visibility == Visibility.Visible) perTempSave.SetPrevIfExist();
                if (specificationTabControl.Visibility == Visibility.Visible) specTempSave.SetPrevIfExist();
                if (vedomostListView.Visibility == Visibility.Visible) vedomostTempSave.SetPrevIfExist();
                DisplayAllValues();
            }
        }

        /// <summary> Возврат действия </summary>
        private void RedoButton_Click(object sender, RoutedEventArgs e)
        {
            if (redoMenuItem.IsEnabled == true)
            {
                if (perechenListView.Visibility == Visibility.Visible) perTempSave.SetNextIfExist();
                if (specificationTabControl.Visibility == Visibility.Visible) specTempSave.SetNextIfExist();
                if (vedomostListView.Visibility == Visibility.Visible) vedomostTempSave.SetNextIfExist();
                DisplayAllValues();
            }

        }

        /// <summary> Выход из программы </summary>
        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary> Открытие справки </summary>
        private void helpMenuItem_Click (object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.электроника-и-программирование.рф/docgost/");
        }
    }
}
