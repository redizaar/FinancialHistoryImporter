using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using _Excel = Microsoft.Office.Interop.Excel;
using System.Windows;
using System.IO;
using System.Globalization;
using System.Threading;
using System.Data.SqlClient;

namespace WpfApp1
{
    class ExportTransactions
    {
        Workbook WriteWorkbook;
        Worksheet WriteWorksheet;
        _Application excel = new _Excel.Application();
        private MainWindow mainWindow;
        private string importerAccountNumber;
        public ExportTransactions(List<Transaction> transactions,MainWindow mainWindow,string currentFileName)
        {
            for (int i = 0; i < transactions.Count; i++)
            {
                string [] spaceSplitted=transactions[i].getTransactionDate().Split(' ');
                string dateString="";
                for (int j = 0; j < spaceSplitted.Length; j++)
                    dateString += spaceSplitted[j];
                Console.WriteLine(dateString.Substring(0,11));
            }
            this.mainWindow = mainWindow;
            MessageBox.Show("Exporting data from: " + currentFileName, "", MessageBoxButton.OK);
                                                    //BUT FIRST - check if the transaction is already exported or not 
            List<Transaction> neededTransactions = newTransactions(transactions);
            SavedTransactions.addToSavedTransactionsBank(neededTransactions);//adding the freshyl imported transactions to the saved 
            WriteWorkbook = excel.Workbooks.Open(@"C:\Users\Tocki\Desktop\Kimutatas.xlsx");
            WriteWorksheet = WriteWorkbook.Worksheets[1];
            if (neededTransactions != null)
            {
                string todaysDate = DateTime.Now.ToString("yyyy-MM-dd");
                int row_number = 1;
                while (WriteWorksheet.Cells[row_number, 1].Value != null)
                {
                    row_number++; // get the current last row
                }
                foreach (var transctn in neededTransactions)
                {

                    WriteWorksheet.Cells[row_number, 1].Value = todaysDate;
                    WriteWorksheet.Cells[row_number, 2].Value = transctn.getTransactionDate();
                    WriteWorksheet.Cells[row_number, 3].Value = transctn.getBalance_rn();
                    WriteWorksheet.Cells[row_number, 7].Value = transctn.getTransactionPrice();
                    if (transctn.getTransactionPrice() < 0)
                    {
                        WriteWorksheet.Cells[row_number, 9].Value = transctn.getTransactionPrice();
                        WriteWorksheet.Cells[row_number, 11].Value = transctn.getTransactionPrice();
                        WriteWorksheet.Cells[row_number, 15].Value = "havi";
                    }
                    else
                    {
                        WriteWorksheet.Cells[row_number, 8].Value = transctn.getTransactionPrice();
                        WriteWorksheet.Cells[row_number, 10].Value = transctn.getTransactionPrice();
                        WriteWorksheet.Cells[row_number, 11].Value = transctn.getTransactionPrice();
                        WriteWorksheet.Cells[row_number, 15].Value = "havi";
                    }
                    WriteWorksheet.Cells[row_number, 14].Value = transctn.getTransactionDescription();
                    WriteWorksheet.Cells[row_number, 16].Value = transctn.getAccountNumber();
                    row_number++;
                    Range line = (Range)WriteWorksheet.Rows[row_number];
                    line.Insert();
                }
                try
                {
                    //WriteWorkbook.SaveAs(@"C:\Users\Tocki\Desktop\Kimutatas.xlsx");
                    excel.ActiveWorkbook.Save();
                    excel.Workbooks.Close();
                    excel.Quit();
                }
                catch(Exception e)
                {

                }
                ImportPageBank.getInstance(mainWindow).setUserStatistics(mainWindow.getCurrentUser());
            }
            else
            {
                return;
            }
        }
        private List<Transaction> newTransactions(List<Transaction> importedTransactions) //check if the transaction is already exported or not
        {
            List<Transaction> savedTransactions = SavedTransactions.getSavedTransactionsBank();
            List<Transaction> neededTransactions=new List<Transaction>();
            importerAccountNumber = importedTransactions[0].getAccountNumber();//account number is the same for all
            ThreadStart threadStart = delegate
            {
                writeAccountNumberToSql(importerAccountNumber);
            };
            Thread sqlThread = new Thread(threadStart);
            sqlThread.IsBackground = true;
            sqlThread.Start();
            sqlThread.Join();
            mainWindow.setAccountNumber(importerAccountNumber);
            if (savedTransactions.Count != 0)//if the export file was not empty we scan the list
            {
                List<Transaction> tempTransactions = new List<Transaction>();
                foreach (var saved in savedTransactions)
                {
                   //egy külön listába tesszük azokat az elemeket a már elmentet tranzakciókból ahol a bankszámlaszám
                   //megegyezik az importálandó bankszámlaszámmal
                   if(saved.getAccountNumber().Equals(importerAccountNumber))
                    {
                        tempTransactions.Add(saved);
                    }
                }
                if (tempTransactions.Count != 0)//ha van olyan már elmentett tranzakció aminek az  a bankszámlaszáma mint amit importálni akarunk
                {
                    int explicitImported=0;
                    StreamWriter logFile =new StreamWriter("C:\\Users\\Tocki\\Desktop\\transactionsLog.txt", append:true);
                    foreach (var imported in importedTransactions)
                    {
                        bool redundant = false;
                        foreach (var saved in tempTransactions)
                        {
                            if (saved.getTransactionDate().Equals(imported.getTransactionDate()) &&
                                    saved.getTransactionPrice().Equals(imported.getTransactionPrice()) &&
                                    saved.getBalance_rn() == imported.getBalance_rn())
                            {
                                redundant = true;
                                if (ImportPageBank.getInstance(mainWindow).alwaysAsk==true)
                                {
                                    if (MessageBox.Show("This transaction is most likely to be in the Databse already!\n -- Transaction date: " + imported.getTransactionDate() + "\n-- Transaction price: " + imported.getTransactionPrice()
                                        + "\n-- Imported on: " + saved.getWriteDate().Substring(0,12)+"\nWould you like to import it anyways?",
                                     "Imprt alert!",
                                        MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                                    {
                                        neededTransactions.Add(imported);
                                        explicitImported++;
                                        logFile.WriteLine("AccountNumber: " + imported.getAccountNumber() +
                                            "\n ImportDate: " + imported.getTransactionDate() +
                                            "\n TransactionPrice: " 
                                            + imported.getTransactionPrice()+" *");
                                    }
                                }
                                break;
                            }
                        }
                        if (redundant == false)
                        {
                            neededTransactions.Add(imported);;
                        }
                    }
                    logFile.Close();
                    if (MessageBox.Show("You have imported "+neededTransactions.Count+" new transaction(s)!\n" +
                        "("+(tempTransactions.Count-explicitImported)+" was already imported)", "Import alert!",
                         MessageBoxButton.OK, MessageBoxImage.Information) == MessageBoxResult.OK)
                    {
                        return neededTransactions;
                    }
                    return neededTransactions;
                }
                else //nincs olyan elmentett tranzakció aminek az lenne a bankszámlaszáma mint amit importálni akarunk
                    //tehát az összeset importáljuk
                {
                    //mainWindow.setTableAttributes(importedTransactions,"empty");
                    if (MessageBox.Show("You have imported " + importedTransactions.Count + " new transaction(s)!\n", "Import alert!",
                         MessageBoxButton.OK, MessageBoxImage.Information) == MessageBoxResult.OK)
                    {
                        return importedTransactions;
                    }
                    return importedTransactions;
                }
            }
            else // még nincs elmentett tranzakció
                 // tehát az összeset importáljuk
            {
                //mainWindow.setTableAttributes(importedTransactions,"empty");
                if (MessageBox.Show("You have imported " + importedTransactions.Count + " new transaction(s)!\n", "Import alert!",
                         MessageBoxButton.OK, MessageBoxImage.Information) == MessageBoxResult.OK)
                {
                    return importedTransactions;
                }
                return importedTransactions;
            }
        }
        private void writeAccountNumberToSql(string accountNumber)
        {
            string storedAccountNumber="-"; //alapértelmezett érték ha valaki regisztrált és még nem importált
            SqlConnection sqlConn = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=LoginDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            string accountNumberQuery = "Select * From [UserDatas] where Username = '" + mainWindow.getCurrentUser().getUsername() + "'";
            SqlDataAdapter sda = new SqlDataAdapter(accountNumberQuery, sqlConn);
            System.Data.DataTable dtb = new System.Data.DataTable();
            sda.Fill(dtb);
            if (dtb.Rows.Count == 1)
            {
                foreach(System.Data.DataRow row in dtb.Rows)
                {
                    storedAccountNumber = row["AccountNumber"].ToString();
                }
            }
            string []splittedAccountNumber = storedAccountNumber.Split(',');
            bool stored = false;
            for (int i = 0; i < splittedAccountNumber.Length; i++)
            {
                if (splittedAccountNumber[i]==accountNumber)
                {
                    stored = true;
                    break;
                }
            }
            if (!stored && storedAccountNumber!="-")
            {
                storedAccountNumber += "," + accountNumber;
                using (SqlCommand command = sqlConn.CreateCommand())
                {
                    command.CommandText = "UPDATE UserDatas SET AccountNumber = '" + storedAccountNumber + "' Where Username = '" + mainWindow.getCurrentUser().getUsername() + "'";
                    sqlConn.Open();
                    command.ExecuteNonQuery();
                    sqlConn.Close();
                }
            }
            else if(storedAccountNumber=="-")
            {
                storedAccountNumber = accountNumber;
                using (SqlCommand command = sqlConn.CreateCommand())
                {
                    command.CommandText = "UPDATE UserDatas SET AccountNumber = '" + storedAccountNumber + "' Where Username = '" + mainWindow.getCurrentUser().getUsername() + "'";
                    sqlConn.Open();
                    command.ExecuteNonQuery();
                    sqlConn.Close();
                }
            }
        }
        public ExportTransactions(List<Stock> transactions, MainWindow mainWindow,string currentFileName)
        {

        }
        public string geImporterAccountNumber()
        {
            return importerAccountNumber;
        }
        public void setimporterAccountNumber(string value)
        {
            importerAccountNumber = value;
        }
        ~ExportTransactions()
        {
            excel.Application.Quit();
            excel.Quit();
        }
    }
}
