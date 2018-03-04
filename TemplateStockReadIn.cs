using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using _Excel = Microsoft.Office.Interop.Excel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Net;

namespace WpfApp1
{
    public class TemplateStockReadIn
    {
        private string folderAddresses;
        private ImportReadIn stockHandler;
        private Workbook workbook;
        private Worksheet stockWorksheet;
        private _Application excel = new _Excel.Application();
        private string temporaryExcel="";
        private bool isCSV;
        public TemplateStockReadIn(ImportReadIn _stockHandler,string filePath)
        {
            stockHandler = _stockHandler;
            folderAddresses = filePath;
        }
        public void analyzeStockTransactionFile()
        {
            workbook = excel.Workbooks.Open(folderAddresses);
            stockWorksheet = workbook.Worksheets[1];
            int companyName = getCompanyColumn();
            int transactionDate = getDateColumn();
            getHistoricalStockPrice(companyName,transactionDate);
        }
        //right know olny works for NASDAQ,NYSE
        private void getHistoricalStockPrice(int companyColumn, int dateColumn)
        {
            List<string> companyNames = collectCompanyNames(companyColumn);
            Dictionary<string, string> companyToDate = collectOldestShareDates(companyNames,companyColumn,dateColumn);
            Dictionary<string, string> companyToTicker = new Dictionary<string, string>();
            string companyNamesCSV;
            using (var web = new WebClient())
            {
                var url = $"http://www.nasdaq.com/screening/companies-by-industry.aspx?render=download";
                companyNamesCSV = web.DownloadString(url);
            }
            Regex reg = new Regex("\"([^\"]*?)\"");
            var matches = reg.Matches(companyNamesCSV).
                Cast<Match>()
                .Select(m => m.Value)
                .ToArray();
            for (int i = 9; i < matches.Length; i += 9)
            {
                for(int j=0;j<companyNames.Count;j++)
                {
                    if(matches[i+1].Contains(companyNames[j]) || levenshteinDistance(companyNames[j],matches[i+1])==1)
                    {
                        string [] splitted = matches[i].Split('"');
                        string ticker = "";
                        for (int k = 0; k < splitted.Length; k++)
                            ticker += splitted[k];
                        companyToTicker.Add(companyNames[j], matches[i]);
                        companyNames.Remove(companyNames[j]);
                    }
                }
                //Console.WriteLine("Ticker: {0} -> Company name :{1} ", matches[i], matches[i + 1]);
            }
            /*
             * minimum eltérés keresése
            string amazon = "Amazon.com Inc.";
            int temp;
            int minimum=10;
            int minIdx=10;
            if (companyNames.Count>0)
            {
                for(int i=10;i<matches.Length;i+=9)
                {
                    temp = levenshteinDistance(amazon,matches[i]);
                    if (temp<minimum)
                    {
                        minimum = temp;
                        minIdx = i;
                    }
                }
            }
            */
        }

        private Dictionary<string, string> collectOldestShareDates(List<string> companyNames,int companyColumn, int dateColumn)
        {
            int blank_cell_counter = 0;
            int row=1;
            Dictionary<string, string> companyToOldestDate = new Dictionary<string, string>();
            while(blank_cell_counter<2)
            {
                if(stockWorksheet.Cells[row,1].Value!=null)
                {
                    blank_cell_counter = 0;
                }
                else
                {
                    blank_cell_counter++;
                }
                row++;
            }
            int lastRow = row-2;
            return companyToOldestDate;
        }

        public static int levenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];
            if (n == 0)
            {
                return m;
            }
            if (m == 0)
            {
                return n;
            }
            for (int i = 0; i <= n; d[i, 0] = i++)
                ;
            for (int j = 0; j <= m; d[0, j] = j++)
                ;
            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            return d[n, m];
        }
        private List<string> collectCompanyNames(int companyColumn)
        {
            List<string> returnValue = new List<string>();
            int blank_cell_counter = 0;
            int row = 1;
            while(blank_cell_counter<2)
            {
                if(stockWorksheet.Cells[row,companyColumn].Value!=null)
                {
                    blank_cell_counter = 0;
                    if(!returnValue.Contains(stockWorksheet.Cells[row, companyColumn].Value.ToString()))
                        returnValue.Add(stockWorksheet.Cells[row, companyColumn].Value.ToString());
                }
                else
                {
                    blank_cell_counter++;
                }
                row++;
            }
            return returnValue;
        }

        public int getDateColumn()
        {
            Regex dateRegex1 = new Regex(@"^20\d{2}.\d{2}.\d{2}");
            Regex dateRegex2 = new Regex(@"^20\d{2}-\d{2}-\d{2}");
            Regex dateRegex3 = new Regex(@"^20\d{2}.\s\d{2}.\s\d{2}");
            Regex dateRegex4 = new Regex(@"^\d{2}-[\u0000-\u00FF]{3}.-\d{4}$"); // pl. 28-ápr-2018
            Regex dateRegex5 = new Regex(@"^\d{2}-[\u0000-\u00FF]{4}.-\d{4}$"); // pl. 28-márc-2018
            Regex dateRegex6 = new Regex(@"^\d{4}-[\u0000-\u00FF]{4}.-\d{2}$");
            Regex dateRegex7 = new Regex(@"^\d{4}-[\u0000-\u00FF]{3}.-\d{2}$");
            int blank_cell_counter = 0;
            int row = 2;
            int column = 1;
            while (true)
            {
                while (blank_cell_counter < 2)
                {
                    if (stockWorksheet.Cells[row, column].Value != null)
                    {
                        blank_cell_counter=0;
                        if (dateRegex1.IsMatch(stockWorksheet.Cells[row, column].Value.ToString()) ||
                            dateRegex2.IsMatch(stockWorksheet.Cells[row, column].Value.ToString()) ||
                            dateRegex3.IsMatch(stockWorksheet.Cells[row, column].ToString()) ||
                            dateRegex4.IsMatch(stockWorksheet.Cells[row, column].Value.ToString()) ||
                            dateRegex5.IsMatch(stockWorksheet.Cells[row, column].Value.ToString()) ||
                            dateRegex6.IsMatch(stockWorksheet.Cells[row, column].Value.ToString()) ||
                            dateRegex7.IsMatch(stockWorksheet.Cells[row, column].Value.ToString()))
                        {
                            /*
                             * It can be in the first column, but it got to have other values in that row
                             * It can happen that it is the last value un the row
                             * but in that case it isn't in the first column
                             */
                            if((stockWorksheet.Cells[row,column+1].Value!=null) || column!=1)
                            {
                                return column;
                            }
                        }
                        else
                        {
                            column++;
                        }
                    }
                    else
                    {
                        blank_cell_counter++;
                    }
                    row++;
                }
                column = 1;
                if (stockWorksheet.Cells[row++, column].Value != null)
                {
                    blank_cell_counter = 0;
                    row++;
                }
                else
                {
                    return 0;
                }
            }
        }

        public int getCompanyColumn()
        {
            int blank_cell_counter = 0;
            int row = 2;
            int column = 1;
            string companyRegex1 = "Co.";
            string companyRegex2 = "AG";
            string companyRegex3 = "Inc.";
            string companyRegex4 = "Corp.";
            string companyRegex5 = "Ltd.";
            string companyRegex6 = "Nyrt.";
            while (true)
            {
                while (blank_cell_counter < 2)
                {
                    if (stockWorksheet.Cells[row, column].Value != null)
                    {
                        blank_cell_counter = 0;
                        if (stockWorksheet.Cells[row, column].Value.ToString().Contains(companyRegex1) ||
                            stockWorksheet.Cells[row, column].Value.ToString().Contains(companyRegex2) ||
                            stockWorksheet.Cells[row, column].Value.ToString().Contains(companyRegex3) ||
                            stockWorksheet.Cells[row, column].Value.ToString().Contains(companyRegex4) ||
                            stockWorksheet.Cells[row, column].Value.ToString().Contains(companyRegex5) ||
                            stockWorksheet.Cells[row, column].Value.ToString().Contains(companyRegex6))
                        {
                            int matchingCells = 1;
                            for(int i=row;i<row+3;i++)
                            {
                                if (stockWorksheet.Cells[row, column].Value.ToString().Contains(companyRegex1) ||
                                    stockWorksheet.Cells[row, column].Value.ToString().Contains(companyRegex2) ||
                                    stockWorksheet.Cells[row, column].Value.ToString().Contains(companyRegex3) ||
                                    stockWorksheet.Cells[row, column].Value.ToString().Contains(companyRegex4) ||
                                    stockWorksheet.Cells[row, column].Value.ToString().Contains(companyRegex5) ||
                                    stockWorksheet.Cells[row, column].Value.ToString().Contains(companyRegex6))
                                {
                                    matchingCells++;
                                }
                            }
                            if(matchingCells>1)
                            {
                                return column;
                            }
                        }
                    }
                    else
                    {
                        blank_cell_counter++;
                    }
                    column++;
                }
                column = 1;
                if(stockWorksheet.Cells[row++,column].Value!=null)
                {
                    blank_cell_counter = 0;
                    row++;
                }
                else
                {
                    return 0;
                }
            }
        }
        public void deleteTemporaryExcel()
        {
            if (File.Exists(temporaryExcel))
            {
                File.Delete(temporaryExcel);
            }
        }
        public string[] WriteSafeReadAllLines(String path)
        {
            using (var csv = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(csv))
            {
                List<string> file = new List<string>();
                while (!sr.EndOfStream)
                {
                    file.Add(sr.ReadLine());
                }
                return file.ToArray();
            }
        }
        ~TemplateStockReadIn()
        {
            /*
            if(temporaryExcel!="")
            {
                deleteTemporaryExcel();
            }
            */
            //workbook.Close();
            //excel.Quit();
        }
    }
}