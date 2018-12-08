using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Windows;
using System;
using DocGOST.Data;

namespace DocGOST
{
    /// <summary>
    /// Логика взаимодействия для OsnNadpisWindow.xaml
    /// </summary>
    public partial class OsnNadpisWindow : Window
    {


        Database project;

        public OsnNadpisWindow(string projecPath)
        {
            
            InitializeComponent();
                        
            project = new Data.Database(projecPath);

            //Заполнение граф окна значениями из базы данных проекта
            gr1aTextBox.Text = project.GetOsnNadpisItem("1a").perechenValue;
            gr1bTextBox.Text = project.GetOsnNadpisItem("1b").perechenValue;
            gr2TextBox.Text = project.GetOsnNadpisItem("2").perechenValue;
            gr3TextBox.Text = project.GetOsnNadpisItem("3").perechenValue;
            gr4aTextBox.Text = project.GetOsnNadpisItem("4a").perechenValue;
            gr4bTextBox.Text = project.GetOsnNadpisItem("4b").perechenValue;
            gr4cTextBox.Text = project.GetOsnNadpisItem("4c").perechenValue;
            gr5TextBox.Text = project.GetOsnNadpisItem("5").perechenValue;
            gr6TextBox.Text = project.GetOsnNadpisItem("6").perechenValue;
            gr7TextBox.Text = project.GetOsnNadpisItem("7").perechenValue;
            gr8TextBox.Text = project.GetOsnNadpisItem("8").perechenValue;
            gr9TextBox.Text = project.GetOsnNadpisItem("9").perechenValue;
            gr10TextBox.Text = project.GetOsnNadpisItem("10").perechenValue;
            gr11aTextBox.Text = project.GetOsnNadpisItem("11a").perechenValue;
            gr11bTextBox.Text = project.GetOsnNadpisItem("11b").perechenValue;
            gr11cTextBox.Text = project.GetOsnNadpisItem("11c").perechenValue;
            gr11dTextBox.Text = project.GetOsnNadpisItem("11d").perechenValue;
            gr11eTextBox.Text = project.GetOsnNadpisItem("11e").perechenValue;
            gr14aTextBox.Text = project.GetOsnNadpisItem("14a").perechenValue;
            gr15aTextBox.Text = project.GetOsnNadpisItem("15a").perechenValue;
            gr16aTextBox.Text = project.GetOsnNadpisItem("16a").perechenValue;
            gr19TextBox.Text = project.GetOsnNadpisItem("19").perechenValue;
            gr21TextBox.Text = project.GetOsnNadpisItem("21").perechenValue;
            gr22TextBox.Text = project.GetOsnNadpisItem("22").perechenValue;
            gr24TextBox.Text = project.GetOsnNadpisItem("24").perechenValue;
            gr25TextBox.Text = project.GetOsnNadpisItem("25").perechenValue;
            gr32TextBox.Text = project.GetOsnNadpisItem("32").perechenValue;

        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            Save();
            this.DialogResult = true;
        }
               
        private void Save()
        {            
            OsnNadpisItem osnNadpisItem = new OsnNadpisItem();
            osnNadpisItem.grapha = "1a";
            osnNadpisItem.perechenValue = gr1aTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "1b";
            osnNadpisItem.perechenValue = gr1bTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "2";
            osnNadpisItem.perechenValue = gr2TextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "3";
            osnNadpisItem.perechenValue = gr3TextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "4a";
            osnNadpisItem.perechenValue = gr4aTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "4b";
            osnNadpisItem.perechenValue = gr4bTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "4c";
            osnNadpisItem.perechenValue = gr4cTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "5";
            osnNadpisItem.perechenValue = gr5TextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "6";
            osnNadpisItem.perechenValue = gr6TextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "7";
            osnNadpisItem.perechenValue = gr7TextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "8";
            osnNadpisItem.perechenValue = gr8TextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "9";
            osnNadpisItem.perechenValue = gr9TextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "10";
            osnNadpisItem.perechenValue = gr10TextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "11a";
            osnNadpisItem.perechenValue = gr11aTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "11b";
            osnNadpisItem.perechenValue = gr11bTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "11c";
            osnNadpisItem.perechenValue = gr11cTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "11d";
            osnNadpisItem.perechenValue = gr11dTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "11e";
            osnNadpisItem.perechenValue = gr11eTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "14a";
            osnNadpisItem.perechenValue = gr14aTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "15a";
            osnNadpisItem.perechenValue = gr15aTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "16a";
            osnNadpisItem.perechenValue = gr16aTextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "19";
            osnNadpisItem.perechenValue = gr19TextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "21";
            osnNadpisItem.perechenValue = gr21TextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "22";
            osnNadpisItem.perechenValue = gr22TextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "24";
            osnNadpisItem.perechenValue = gr24TextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "25";
            osnNadpisItem.perechenValue = gr25TextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
            osnNadpisItem.grapha = "32";
            osnNadpisItem.perechenValue = gr32TextBox.Text;
            project.SaveOsnNadpisItem(osnNadpisItem);
        }
    }
}
