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
using System.Text.RegularExpressions;

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
            string earningMethod = ImportPageStock.getInstance(mainWindow).getMethod();
            switch(earningMethod)
            {
                case "FIFO":
                    stockExportFIFO(transactions);
                    break;
                case "LIFO":
                    break;
                case "CUSTOM":
                    break;
            }
        }

        private void stockExportFIFO(List<Stock> transactions)
        {
            Dictionary<Stock, int> transactionMap = new Dictionary<Stock, int>();
            List<string> companies = new List<string>();
            foreach(var transaction in transactions)
            {
                if (!companies.Contains(transaction.getStockName()))
                    companies.Add(transaction.getStockName()); 
            }
            //while (companies.Count != 0)
            //{
            string companyName = companies[0];//removing the already scanned companies
            List<Stock> tempTransactions = new List<Stock>();
            for(int i=0;i<transactions.Count;i++)
            {
                if(companyName==transactions[i].getStockName())
                {
                    tempTransactions.Add(transactions[i]);
                    transactionMap.Add(transactions[i], i);
                }
            }
            if (tempTransactions.Count > 1)
            {
                Stock soldStock=null;
                Stock boughtStock=null;
                int soldIndex = -1;
                for (int i = tempTransactions.Count-1; i >= 0; i--)
                {
                    Regex quantityRegex1 = new Regex(@"Eladott");
                    Regex quantityRegex2 = new Regex(@"Sold");
                    Regex quantityRegex3 = new Regex(@"Sell");
                    if( quantityRegex1.IsMatch(tempTransactions[i].getTransactionType()) ||
                        quantityRegex2.IsMatch(tempTransactions[i].getTransactionType()) ||
                        quantityRegex3.IsMatch(tempTransactions[i].getTransactionType()))
                    {
                        soldStock = tempTransactions[i];
                        soldIndex = i;
                        break;
                    }
                }
                if (soldStock != null)
                {
                    int boughtIndex = -1;
                    for (int i = tempTransactions.Count-1; i > soldIndex; i--)
                    {
                        Regex quantityRegex1 = new Regex(@"Vásárolt");
                        Regex quantityRegex2 = new Regex(@"Bought");
                        Regex quantityRegex3 = new Regex(@"Buy");
                        if (quantityRegex1.IsMatch(tempTransactions[i].getTransactionType()) ||
                            quantityRegex2.IsMatch(tempTransactions[i].getTransactionType()) ||
                            quantityRegex3.IsMatch(tempTransactions[i].getTransactionType()))
                        {
                            boughtStock = tempTransactions[i];
                            boughtIndex = i;
                            break;
                        }
                    }
                    if (boughtStock != null)
                    {
                        while (true)
                        {
                            double profit = 0;
                            if ((boughtStock.getQuantity() - soldStock.getQuantity()) == 0)
                            {
                                profit = (soldStock.getStockPrice() - boughtStock.getStockPrice()) * soldStock.getQuantity();
                                int index = transactionMap[soldStock];
                                transactions[index].setProfit(profit);
                                if (boughtIndex - 1 != soldIndex)
                                    boughtIndex--;
                            }
                            else if ((boughtStock.getQuantity() - soldStock.getQuantity()) < 0)
                            {
                                //it's important to multiple it by the boughtStock,
                                //because the soldStock quantity is higher than the bought
                                profit = (soldStock.getStockPrice() - boughtStock.getStockPrice()) * boughtStock.getQuantity();
                                int leftQuantity = soldStock.getQuantity() - boughtStock.getQuantity();
                                soldStock.setQuantity(leftQuantity);
                                while (soldStock.getQuantity() != 0)
                                {
                                    if (boughtIndex - 1 != soldIndex)
                                    {
                                        for (int i = boughtIndex; i >= soldIndex; i--)
                                        {
                                            Regex quantityRegex1 = new Regex(@"Vásárolt");
                                            Regex quantityRegex2 = new Regex(@"Bought");
                                            Regex quantityRegex3 = new Regex(@"Buy");
                                            if (quantityRegex1.IsMatch(tempTransactions[i].getTransactionType()) ||
                                                quantityRegex2.IsMatch(tempTransactions[i].getTransactionType()) ||
                                                quantityRegex3.IsMatch(tempTransactions[i].getTransactionType()))
                                            {
                                                boughtStock = tempTransactions[i];
                                                boughtIndex = i;
                                                break;
                                            }
                                        }
                                        /**
                                         * We change the bought Stocks quantity because if we have other SOLD stocks we dont want it to count
                                         * with the full quantity (There would be a mistake)
                                         */
                                        if (soldStock.getQuantity() > boughtStock.getQuantity())
                                        {
                                            leftQuantity = soldStock.getQuantity() - boughtStock.getQuantity();
                                            profit += (soldStock.getStockPrice() - boughtStock.getStockPrice()) * boughtStock.getQuantity();
                                        }
                                        else if (boughtStock.getQuantity() > soldStock.getQuantity())
                                        {
                                            leftQuantity = 0;
                                            profit += (soldStock.getStockPrice() - boughtStock.getStockPrice()) * soldStock.getQuantity();
                                            int leftBoughtQuantity = boughtStock.getQuantity() - soldStock.getQuantity();
                                            boughtStock.setQuantity(leftBoughtQuantity);
                                        }
                                        else if (boughtStock.getQuantity() == soldStock.getQuantity())
                                        {
                                            leftQuantity = 0;
                                            profit += (soldStock.getStockPrice() - boughtStock.getStockPrice()) * soldStock.getQuantity();
                                        }
                                        soldStock.setQuantity(leftQuantity);
                                    }
                                    else//we reached the sell transaction but the quantity is still not zero, ? to do in that case
                                    {
                                        soldStock.setQuantity(0);
                                    }
                                }

                            }
                            else if ((boughtStock.getQuantity() - soldStock.getQuantity()) > 0)
                            {
                                profit = (soldStock.getStockPrice() - boughtStock.getStockPrice()) * soldStock.getQuantity();
                                int leftBoughtStock = boughtStock.getQuantity() - soldStock.getQuantity();
                                tempTransactions.Find(i => i == boughtStock).setQuantity(leftBoughtStock);
                            }

                        }
                    }
                }
            }
            //}
        }
        private void stockExportFIFO2(List<Stock> allCompany)
        {
            /*Need to add the SavedStocks to this List*/
            /*getting the company names*/
            List<string> companies = new List<string>();
            foreach(var transaction in allCompany)
            {
                if (!companies.Contains(transaction.getStockName()))
                    companies.Add(transaction.getStockName());
            }
            /*getting the company names*/

            Dictionary<Stock, int> transactionMap = new Dictionary<Stock, int>();
            List<Stock> company = new List<Stock>();
            bool allFinished = false;
            while (!allFinished)
            {
                //removing the companies we done calculation
                string companyName = companies[0];

                /*Separating the Stocks based on CompanyNames*/
                /*If we add it to the separate list we also save the original index,because we most likely to change it*/
                for (int i = 0; i < allCompany.Count; i++)
                {
                    if(allCompany[i].getStockName()==companyName)
                    {
                        company.Add(allCompany[i]);
                        transactionMap.Add(allCompany[i], i);
                    }
                }
                /*Separating the Stocks based on CompanyNames*/
                /*If we add it to the separate list we also save the original index,because we most likely to change it*/

                //Megtaláljuk Hátulról a legelső eladást
                Stock soldStock = null;
                Stock boughtStock = null;
                int totalCount = company.Count - 1;
                int soldIndex = -1;
                int boughtIndex = -1;
                bool finished = false;
                while (!finished)
                {
                    for (int i = totalCount; i >= 0; i--)
                    {
                        Regex quantityRegex1 = new Regex(@"Eladott");
                        Regex quantityRegex2 = new Regex(@"Sold");
                        Regex quantityRegex3 = new Regex(@"Sell");
                        if (quantityRegex1.IsMatch(company[i].getTransactionType()) ||
                                quantityRegex2.IsMatch(company[i].getTransactionType()) ||
                                quantityRegex3.IsMatch(company[i].getTransactionType()))
                        {
                            soldStock = company[i];
                            soldIndex = i;
                            break;
                        }
                    }
                    if(soldStock!=null)//ha újra belép akkor nem jó
                    {
                        for (int i = totalCount; i >= soldIndex+1; i--)
                        {
                            Regex quantityRegex1 = new Regex(@"Vásárolt");
                            Regex quantityRegex2 = new Regex(@"Bought");
                            Regex quantityRegex3 = new Regex(@"Buy");
                            if (quantityRegex1.IsMatch(company[i].getTransactionType()) ||
                                quantityRegex2.IsMatch(company[i].getTransactionType()) ||
                                quantityRegex3.IsMatch(company[i].getTransactionType()))
                            {
                                boughtStock = company[i];
                                boughtIndex = i;
                                break;
                            }
                        }
                        if(boughtStock!=null)//ha újra belép nem jó ?
                        {
                            double profit = 0;
                            if ((boughtStock.getQuantity() - soldStock.getQuantity()) == 0)
                            {
                                profit = (soldStock.getStockPrice() - boughtStock.getStockPrice()) * soldStock.getQuantity();
                                int index = transactionMap[soldStock];
                                allCompany[index].setProfit(profit);
                                totalCount = soldIndex--;
                                boughtStock.setQuantity(0);
                                soldStock.setQuantity(0);
                            }
                            else if ((boughtStock.getQuantity() - soldStock.getQuantity()) < 0)
                            {
                                //it's important to multiple it by the boughtStock,
                                //because the soldStock quantity is higher than the bought
                                profit = (soldStock.getStockPrice() - boughtStock.getStockPrice()) * boughtStock.getQuantity();
                                int leftQuantity = soldStock.getQuantity() - boughtStock.getQuantity();
                                soldStock.setQuantity(leftQuantity);
                                while (soldStock.getQuantity() != 0)
                                {
                                    /*this if means that, we "run" out of bought quantity, and the next Stock would be the SoldStock*/
                                    if (boughtIndex - 1 != soldIndex)
                                    {
                                        for (int i = boughtIndex-1; i > soldIndex; i--)
                                        {
                                            Regex quantityRegex1 = new Regex(@"Vásárolt");
                                            Regex quantityRegex2 = new Regex(@"Bought");
                                            Regex quantityRegex3 = new Regex(@"Buy");
                                            if (quantityRegex1.IsMatch(company[i].getTransactionType()) ||
                                                quantityRegex2.IsMatch(company[i].getTransactionType()) ||
                                                quantityRegex3.IsMatch(company[i].getTransactionType()))
                                            {
                                                boughtStock = company[i];
                                                boughtIndex = i;
                                                break;
                                            }
                                        }
                                        /**
                                         * We change the bought Stocks quantity because if we have other SOLD stocks we dont want it to count
                                         * with the full quantity (There would be a mistake)
                                         */
                                        if (soldStock.getQuantity() > boughtStock.getQuantity())
                                        {
                                            leftQuantity = soldStock.getQuantity() - boughtStock.getQuantity();
                                            profit += (soldStock.getStockPrice() - boughtStock.getStockPrice()) * boughtStock.getQuantity();
                                        }
                                        else if (boughtStock.getQuantity() > soldStock.getQuantity())
                                        {
                                            leftQuantity = 0;
                                            profit += (soldStock.getStockPrice() - boughtStock.getStockPrice()) * soldStock.getQuantity();
                                            int leftBoughtQuantity = boughtStock.getQuantity() - soldStock.getQuantity();
                                            boughtStock.setQuantity(leftBoughtQuantity);
                                        }
                                        else if (boughtStock.getQuantity() == soldStock.getQuantity())
                                        {
                                            leftQuantity = 0;
                                            profit += (soldStock.getStockPrice() - boughtStock.getStockPrice()) * soldStock.getQuantity();
                                            boughtStock.setQuantity(0);
                                        }
                                        soldStock.setQuantity(leftQuantity);
                                    }
                                    else//we reached the sell transaction but the quantity is still not zero, ? to do in that case
                                    {
                                        soldStock.setQuantity(0);
                                        totalCount = soldIndex--;
                                    }
                                }
                            }
                            else if ((boughtStock.getQuantity() - soldStock.getQuantity()) > 0)
                            {
                                profit = (soldStock.getStockPrice() - boughtStock.getStockPrice()) * soldStock.getQuantity();
                                int leftBoughtStock = boughtStock.getQuantity() - soldStock.getQuantity();
                                company.Find(i => i == boughtStock).setQuantity(leftBoughtStock);
                                totalCount = soldIndex--;
                            }
                        }
                        else
                        {
                            finished=true;
                        }
                    }
                    else
                    {
                        finished = true;
                    }
                }
            }
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
