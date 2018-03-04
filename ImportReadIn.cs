using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using _Excel = Microsoft.Office.Interop.Excel;
namespace WpfApp1
{
    public class ImportReadIn
    {
        private List<string> path;
        private string importType = "";
        private string currentFileName;
        private MainWindow mainWindow;
        List<Transaction> transactions;

        _Application excel = new _Excel.Application();
        Workbook ReadWorkbook;
        Worksheet ReadWorksheet;
        public ImportReadIn(string _importType, List<string> _path,MainWindow _mainWindow,bool specifiedByUser)
        {
            path = _path;
            importType = _importType;
            mainWindow = _mainWindow;
            if (path[0] != "FolderAdress")//a path wasn't choosen, useless ( not in use )
            {
                for (int i = 0; i < path.Count; i++)
                {
                    string [] splittedFileName=path[i].Split('\\');
                    int lastSplitIndex = splittedFileName.Length-1;
                    currentFileName = splittedFileName[lastSplitIndex];
                    ReadWorkbook = excel.Workbooks.Open(path[i]);
                    ReadWorksheet = ReadWorkbook.Worksheets[1];
                    if (importType=="Bank")
                    {
                        if (!specifiedByUser)
                        {
                            TemplateBankReadIn templateBank = new TemplateBankReadIn(this, ReadWorkbook, ReadWorksheet, mainWindow, false);
                            //so far we got the Starting Row(of the transactions),Number of Columns, account number
                            templateBank.readOutTransactionColumns(templateBank.getStartingRow(), templateBank.getNumberOfColumns());
                        }
                        else //userSpecified==true
                        {
                            TemplateBankReadIn templateBank = new TemplateBankReadIn(this, ReadWorkbook, ReadWorksheet, mainWindow, true);
                            string startingRow = SpecifiedImport.getInstance(null,mainWindow).transactionsRowTextBox.Text.ToString();
                            string dateColumn = SpecifiedImport.getInstance(null,mainWindow).dateColumnTextBox.Text.ToString();
                            string commentColumn = SpecifiedImport.getInstance(null,mainWindow).commentColumnTextBox.Text.ToString();
                            string accountNumberCB = SpecifiedImport.getInstance(null,mainWindow).accountNumberCB.SelectedItem.ToString();
                            string transactionPriceCB = SpecifiedImport.getInstance(null,mainWindow).priceColumnCB.SelectedItem.ToString();
                            string balanceCB = SpecifiedImport.getInstance(null,mainWindow).balanceColumnCB.SelectedItem.ToString();
                            string balanceComboBocString = SpecifiedImport.getInstance(null, mainWindow).balanceColumnTextBox.Text.ToString();
                            templateBank.readOutUserspecifiedTransactions(startingRow, dateColumn, commentColumn, accountNumberCB, transactionPriceCB, balanceCB, balanceComboBocString);
                        }
                    }
                    else if(importType=="Stock")
                    {
                        TemplateStockReadIn templateStock = new TemplateStockReadIn(this,path[i]);
                        templateStock.analyzeStockTransactionFile();
                    }
                }
                excel.Application.Quit();
                excel.Quit();
            }
        }
        ~ImportReadIn()
        {
            excel.Application.Quit();
            excel.Quit();
        }
        public void addTransactions(List<Transaction> newTransactions)
        {
            this.transactions = newTransactions;
            writeOutTransactions();
        }
        public void writeOutTransactions()
        {
            new ExportTransactions(transactions,mainWindow,currentFileName);
        }
    }
}
