using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for SpecifiedImport.xaml
    /// </summary>
    public partial class SpecifiedImportBank : Page
    {
        private static SpecifiedImportBank instance;
        public MainWindow mainWindow;
        public static List<string> folderPath;
        public int numberofFile;
        //binding
        private ButtonCommands btnCommand;
        public List<string> accountNumberChoices { get; set; }
        public string accountNumberChoice { get; set; }
        public List<string> priceColumnChoices { get; set; }
        public string priceColumnChoice { get; set; }
        public List<string> balanceColumnChoices { get; set; }
        public string balanceColumnChoice { get; set; }
        public string commentColumnHelp { get; set; }
        public ButtonCommands importPushed
        {
            get
            {
                btnCommand = new ButtonCommands(this, folderPath[numberofFile]);
                return btnCommand;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        //flags
        private bool accNum_Column;
        private bool accNum_Cell;
        private bool accNum_SheetName;

        private bool priceSingleColumn;
        private bool priceMultipleColumn;

        private bool balanceColumn;
        private bool noBalanceColumn;
        private SpecifiedImportBank(MainWindow mainWindow)
        {
            numberofFile = 0;
            this.mainWindow = mainWindow;
            InitializeComponent();
            DataContext = this;
            commentColumnHelp = "Multiple comment columns can be separated by commas (i.e. A,B,C)!";
            string[] splitedFileName = folderPath[numberofFile].Split('\\');
            int lastSplitIndex = splitedFileName.Length - 1;
            currentFileLabel.Content = "File: " + splitedFileName[lastSplitIndex];
            accountNumberChoices = new List<string>();
            accountNumberChoices.Add("Column");
            accountNumberChoices.Add("Cell");
            accountNumberChoices.Add("Sheet name");
            priceColumnChoices = new List<string>();
            priceColumnChoices.Add("One column");
            priceColumnChoices.Add("Income,Spending");
            balanceColumnChoices = new List<string>();
            balanceColumnChoices.Add("Column");
            balanceColumnChoices.Add("None");
            accountNumberTextBox.Visibility = Visibility.Hidden;
            priceColumnTextBox_1.Visibility = Visibility.Hidden;
            priceColumnTextBox_2.Visibility = Visibility.Hidden;
            balanceColumnTextBox.Visibility = Visibility.Hidden;
        }
        public static SpecifiedImportBank getInstance(List<string> newfoldetPath, MainWindow mainWindow)
        {
            if (newfoldetPath != null)
            {
                folderPath = newfoldetPath;
            }
            if (instance == null)
            {
                instance = new SpecifiedImportBank(mainWindow);
            }
            return instance;
        }
        private void accountNumberCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            accountNumberTextBox.Visibility = Visibility.Hidden;
            accNum_Column = false;
            accNum_Cell = false;
            accNum_SheetName = false;
            if (accountNumberChoice == "Column")
            {
                accountNumberTextBox.Visibility = Visibility.Visible;
                accNum_Column = true;
            }
            else if (accountNumberChoice == "Cell")
            {
                accountNumberTextBox.Visibility = Visibility.Visible;
                accNum_Cell = true;
            }
            else if (accountNumberChoice == "Sheet name")
            {
                accNum_SheetName = true;
            }
        }
        public void setCurrentFileLabel(string currentFile)
        {
            currentFileLabel.Content = "File: " + currentFile;
        }
        private void priceColumnCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            priceSingleColumn = false;
            priceMultipleColumn = false;
            if (priceColumnChoice == "One column")
            {
                priceSingleColumn = true;
                priceColumnTextBox_1.Visibility = Visibility.Visible;
                priceColumnTextBox_2.Visibility = Visibility.Hidden;
            }
            else if (priceColumnChoice == "Income,Spending")
            {
                priceMultipleColumn = false;
                priceColumnTextBox_1.Visibility = Visibility.Visible;
                priceColumnTextBox_2.Visibility = Visibility.Visible;
            }
        }

        private void balanceColumnCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            balanceColumn = false;
            noBalanceColumn = false;
            if (balanceColumnChoice == "Column")
            {
                balanceColumn = true;
                balanceColumnTextBox.Visibility = Visibility.Visible;
            }
            else if (balanceColumnChoice == "None")
            {
                noBalanceColumn = true;
                balanceColumnTextBox.Visibility = Visibility.Hidden;
            }
        }
        public void incrementNumberofFile()
        {
            numberofFile++;
        }
        public int getCurrentFileIndex()
        {
            return numberofFile;
        }
        public bool getAccNum_Column()
        {
            return accNum_Column;
        }
        public bool getAccNum_Cell()
        {
            return accNum_Cell;
        }
        public bool getAccNum_SheetName()
        {
            return accNum_SheetName;
        }
        public bool getPriceSingleColumn()
        {
            return priceSingleColumn;
        }
        public bool getPriceMultipleColumn()
        {
            return priceMultipleColumn;
        }
        public bool getBalanceColumn()
        {
            return balanceColumn;
        }
        public bool getNoBalanceColumn()
        {
            return noBalanceColumn;
        }
        public class ButtonCommands : ICommand
        {
            private SpecifiedImportBank specifiedImport;
            private string currentFileName;
            public ButtonCommands(SpecifiedImportBank specifiedImport,string fileName)
            {
                this.specifiedImport = specifiedImport;
                currentFileName = fileName;
                specifiedImport.PropertyChanged += new PropertyChangedEventHandler(test_PropertyChanged);
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
            private void set_box_values_to_zero()
            {
                specifiedImport.accountNumberCB.SelectedItem = null;
                specifiedImport.priceColumnCB.SelectedItem = null;
                specifiedImport.balanceColumnCB.SelectedItem = null;

                specifiedImport.transactionsRowTextBox.Text = null;
                specifiedImport.accountNumberTextBox.Text = null;
                specifiedImport.dateColumnTextBox.Text = null;
                specifiedImport.priceColumnTextBox_1.Text = null;
                specifiedImport.priceColumnTextBox_2.Text = null;
                specifiedImport.balanceColumnTextBox.Text = null;
                specifiedImport.commentColumnTextBox.Text = null;
            }
            public void Execute(object parameter)
            {
                List<string> currentFile = new List<string>();
                currentFile.Add(currentFileName);
                new ImportReadIn("Bank", currentFile, specifiedImport.mainWindow, true);
                if (SpecifiedImportBank.folderPath.Count < specifiedImport.getCurrentFileIndex())
                {
                    specifiedImport.incrementNumberofFile();
                    string nextFileName = SpecifiedImportBank.folderPath[specifiedImport.getCurrentFileIndex()];
                    string[] splittedFileName = nextFileName.Split('\\');
                    int lastSplitIndex = nextFileName.Length - 1;
                    specifiedImport.currentFileLabel.Content = "File: " + splittedFileName[lastSplitIndex];
                    set_box_values_to_zero();
                    StoredColumnChecker columnChecker = new StoredColumnChecker();
                    columnChecker.getDataTableFromSql(specifiedImport.mainWindow);
                    columnChecker.setAnalyseWorksheet(nextFileName);
                    columnChecker.setMostMatchesRow(columnChecker.findMostMatchingRow());
                    columnChecker.setSpecifiedImportPageTextBoxes();
                }
            }
        }
    }
}
