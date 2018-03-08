using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFCustomMessageBox;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for ImportMainPage.xaml
    /// </summary>
    public partial class ImportMainPage : System.Windows.Controls.Page
    {
        private ButtonCommands btnCommand;
        private MainWindow mainWindow;
        private static ImportMainPage instance;
        public bool alwaysAsk
        {
            get
            {
                if(alwaysAskCB.IsChecked.Equals(true))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if(value)
                {
                    neverAskCB.SetCurrentValue(RadioButton.IsCheckedProperty, false);
                }
            }
        }
        public bool neverAsk
        {
            get
            {
                if (neverAskCB.IsChecked.Equals(true))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (value)
                {
                    alwaysAskCB.SetCurrentValue(RadioButton.IsCheckedProperty, false);
                }
            }
        }
        private ImportMainPage(MainWindow mainWindow)
        {
            DataContext = this;
            InitializeComponent();
            neverAskCB.IsChecked = true;
            descriptionComboBox.Visibility = System.Windows.Visibility.Hidden;
            this.mainWindow = mainWindow;
            FolderAddressLabel.Visibility = System.Windows.Visibility.Hidden;
        }
        public void setUserStatistics(User currentUser)
        {
            int numberOfTransactions = 0;
            int totalIncome = 0;
            int totalSpendings = 0;
            string latestImportDate = "";
            string todaysDate = DateTime.Now.ToString("yyyy-MM-dd");
            DateTime todayDate = Convert.ToDateTime(todaysDate);
            usernameLabel.Content = currentUser.getUsername();
            foreach (var transactions in SavedTransactions.getSavedTransactionsBank())
            {
                if(transactions.getAccountNumber().Equals(currentUser.getAccountNumber()))
                {
                    numberOfTransactions++;
                    latestImportDate = transactions.getWriteDate();//always overwrites it --- todo (more logic)
                    if (transactions.getTransactionPrice() > 0)
                    {
                        totalIncome += transactions.getTransactionPrice();
                    }
                    else
                    {
                        totalSpendings += transactions.getTransactionPrice();
                    }
                }
            }
            if (latestImportDate.Length > 12)
            {
                lastImportDateLabel.Content = latestImportDate.Substring(0, 12);
            }
            else
            {
                lastImportDateLabel.Content = latestImportDate;
            }
            noTransactionsLabel.Content = numberOfTransactions;
            DateTime importDate;
            if (latestImportDate.Length > 0)
            {
                importDate = Convert.ToDateTime(latestImportDate);
                float diffTicks = (todayDate - importDate).Days;
                if (diffTicks >= 30)
                {
                    urgencyLabel.Content = "Recommended!";
                    urgencyLabel.Foreground = new SolidColorBrush(Color.FromRgb(217, 30, 24));
                    mainWindow.exclamImage.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    urgencyLabel.Content = "Not urgent";
                    urgencyLabel.Foreground = new SolidColorBrush(Color.FromRgb(46, 204, 113));
                    mainWindow.exclamImage.Visibility = System.Windows.Visibility.Hidden;
                }
            }
            else
            {
                urgencyLabel.Content = "You haven't imported yet!";
                lastImportDateLabel.Content = "You haven't imported yet!";
            }
        }
        private void getTransactions(string importType, List<string> folderAddress)
        {
            new ImportReadIn(importType, folderAddress, mainWindow,false);
        }
        public ButtonCommands OpenFilePushed
        {
            get
            {
                btnCommand = new ButtonCommands(FileBrowser.Content.ToString(),this);
                return btnCommand;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public static ImportMainPage getInstance(MainWindow mainWindow)
        {
            if(instance==null)
            {
                instance = new ImportMainPage(mainWindow);
            }
            return instance;
        }
        public class ButtonCommands : ICommand
        {
            private string buttonContent;
            private ImportMainPage importPage;
            public ButtonCommands(string buttonContent,ImportMainPage importPage)
            {
                this.buttonContent = buttonContent;
                this.importPage = importPage;

                this.importPage.PropertyChanged += new PropertyChangedEventHandler(test_PropertyChanged);
            }
            private void test_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (CanExecuteChanged != null)
                {
                    CanExecuteChanged(this, EventArgs.Empty);
                }
            }
            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                //todo
                return true;
            }
            private void check_if_csv(int fileIndex,ref List<string> fileAddresses)
            {
                string[] fileName = fileAddresses[fileIndex].Split('\\');
                int lastPartIndex = fileName.Length - 1;
                Regex csvPattern = new Regex(@".csv$");
                if (csvPattern.IsMatch(fileName[lastPartIndex]))
                {
                    string newExcelPath = fileAddresses[fileIndex].Substring(0, fileAddresses[fileIndex].Length - 4);
                    string xls = newExcelPath + ".xls";

                    List<List<string>> allWords = new List<List<string>>();
                    IEnumerable<String> all_lines = System.IO.File.ReadLines(fileAddresses[fileIndex], Encoding.Default);
                    foreach (var lines in all_lines)
                    {
                        List<string> words = lines.Split(';').ToList();
                        allWords.Add(words);
                    }
                    Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
                    Workbook wb = app.Workbooks.Open(fileAddresses[fileIndex], Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    Worksheet sheet = wb.Worksheets[1];
                    int row = 1;
                    foreach (List<string> lines in allWords)
                    {
                        int column = 1;
                        for (int itr = 0; itr < lines.Count; itr++)
                        {
                            sheet.Cells[row, column].Value = lines[itr];
                            column++;
                        }
                        row++;
                    }
                    wb.SaveAs(newExcelPath, XlFileFormat.xlOpenXMLWorkbook, Type.Missing, Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    wb.Close();
                    app.Quit();
                    fileAddresses[fileIndex] = newExcelPath; //overwriting the old string
                }
            }
            public void Execute(object parameter)
            {
                if (buttonContent.Equals("Import Transactions"))
                {
                    MessageBoxResult messageBoxResult = CustomMessageBox.ShowYesNo(
                        "\tPlease choose an import type!",
                        "Import type alert!",
                        "Automatized",
                        "User specified");
                    if (messageBoxResult == MessageBoxResult.Yes || messageBoxResult==MessageBoxResult.No)
                    {
                        Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                        dlg.DefaultExt = ".xls,.csv";
                        dlg.Filter = "Excel files (*.xls)|*.xls|Excel Files (*.xlsx)|*.xlsx|Excel Files (*.xlsm)|*.xlsm|CSV Files (*.csv)|*.csv";
                        dlg.Multiselect = true;
                        Nullable<bool> result = dlg.ShowDialog();
                        if (result == true)
                        {
                            List<string> fileAdresses = dlg.FileNames.ToList();
                            for (int i = 0; i < dlg.FileNames.ToList().Count; i++)
                            {
                                check_if_csv(i,ref fileAdresses);
                            }
                            if (messageBoxResult == MessageBoxResult.Yes)
                            {
                                importPage.getTransactions("Bank", fileAdresses);
                            }
                            else if (messageBoxResult == MessageBoxResult.No)
                            {
                                string[] fileName = dlg.FileNames.ToList()[0].Split('\\');
                                int lastPartIndex = fileName.Length - 1; // to see which file the user immporting first
                                SpecifiedImport.getInstance(fileAdresses, importPage.mainWindow).setCurrentFileLabel(fileName[lastPartIndex]);
                                //fájl felismerés
                                StoredColumnChecker columnChecker = new StoredColumnChecker();
                                columnChecker.getDataTableFromSql(importPage.mainWindow);
                                columnChecker.setAnalyseWorksheet(dlg.FileNames.ToList()[0]);
                                columnChecker.setMostMatchesRow(columnChecker.findMostMatchingRow());
                                columnChecker.setSpecifiedImportPageTextBoxes();
                                importPage.mainWindow.MainFrame.Content = SpecifiedImport.getInstance(dlg.FileNames.ToList(), importPage.mainWindow);
                            }
                        }
                    }
                }
            }
        }
        /**
         * in case if it's a csv we have to overwrite the existing filepath to the new converted excel file string
         * so we need the original list of strings
         * that's the reason why it is a reference
         */ 
        public void check_if_csv(int fileIndex,ref List<string> fileAddresses)
        {
            string[] fileName = fileAddresses[fileIndex].Split('\\');
            int lastPartIndex = fileName.Length - 1;
            Regex csvPattern = new Regex(@".csv$");
            if (csvPattern.IsMatch(fileName[lastPartIndex]))
            {
                string newExcelPath = fileAddresses[fileIndex].Substring(0, fileAddresses[fileIndex].Length - 4)+".xls";

                List<List<string>> allWords = new List<List<string>>();
                List<string> all_lines = System.IO.File.ReadAllLines(fileAddresses[fileIndex], Encoding.Default).ToList();
                foreach (var lines in all_lines)
                {
                    List<string> words = lines.Split(';').ToList();
                    Regex reg = new Regex("\"([^\"]*?)\"");
                    for (int i = 0; i < words.Count; i++)
                    {
                        /**
                         * For some reason if there is a Value -> 10,1
                         * it automatically puts it in quotes  ->"10,1"
                         * we have to convert it back..
                         */
                        if(reg.IsMatch(words[i]))
                        {
                            string [] splitted=words[i].Split('"');
                            string word="";
                            for(int j=0;j<splitted.Length;j++)
                            {
                                word += splitted[j];
                            }
                            words[i] = word;
                        }
                    }
                    allWords.Add(words);
                }
                Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
                Workbook wb = app.Workbooks.Open(fileAddresses[fileIndex], Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                Worksheet sheet = wb.Worksheets[1];
                int row = 1;
                foreach (List<string> lines in allWords)
                {
                    int column = 1;
                    for (int itr = 0; itr < lines.Count; itr++)
                    {
                        sheet.Cells[row, column].Value = lines[itr];
                        column++;
                    }
                    row++;
                }
                wb.SaveAs(newExcelPath, XlFileFormat.xlOpenXMLWorkbook, Type.Missing, Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                wb.Close();
                app.Quit();
                fileAddresses[fileIndex] = newExcelPath; //overwriting the old path string
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".xls,.csv";
            dlg.Filter = "Excel files (*.xls)|*.xls|Excel Files (*.xlsx)|*.xlsx|Excel Files (*.xlsm)|*.xlsm|CSV Files (*.csv)|*.csv";
            dlg.Multiselect = true;
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                List<string> fileAdresses = dlg.FileNames.ToList();
                for(int i=0;i<dlg.FileNames.ToList().Count;i++)
                {
                    check_if_csv(i,ref fileAdresses);
                }
                new ImportReadIn("Stock", fileAdresses, mainWindow, false);
                //var Lines = System.IO.File.ReadLines(dlg.FileNames[0], Encoding.Default).Select(a => a.Split(';'));
                /*
                string textToSearch = "Coca-Cola Co.";
                int lineIndex = all_lines.Select((l, ix) => new { line = l, index = ix })
                    .FirstOrDefault(l => l.line.Contains(textToSearch)).index;
                int columnIndex = all_lines[lineIndex].Split(';').Select((c, ix) => new { col = c, index = ix })
                    .FirstOrDefault(c => c.col.Contains(textToSearch)).index;


                Console.WriteLine("Row: {0} , Column: {1}", lineIndex, columnIndex);
                foreach (var lines in all_lines)
                {
                    string[] splittedLine=lines.Split(';');
                    for(int i=0;i<splittedLine.Length;i++)
                    {
                        if(splittedLine[i]!="")
                        {
                        }
                    }
                }
                //new ImportReadIn("Stock", dlg.FileNames.ToList(), mainWindow, false);
                string companyNamesInCSV;
                using (var web = new WebClient())
                {
                    var url = $"http://www.nasdaq.com/screening/companies-by-industry.aspx?render=download";
                    companyNamesInCSV = web.DownloadString(url);
                }
                Regex reg = new Regex("\"([^\"]*?)\"");
                var matches = reg.Matches(companyNamesInCSV).
                    Cast<Match>()
                    .Select(m => m.Value)
                    .ToArray(); ;
                for(int i=9;i<matches.Length;i+=9)
                {
                    Console.WriteLine("Ticker: {0} -> Company name :{1} ", matches[i], matches[i + 1]);
                }
                */
            }
        }
    }
}
