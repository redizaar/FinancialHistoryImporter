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
        }

        public int getDateColumn()
        {
            Regex dateRegex1 = new Regex(@"^20\d{2}.\d{2}.\d{2}");
            Regex dateRegex2 = new Regex(@"^20\d{2}-\d{2}-\d{2}");
            Regex dateRegex3 = new Regex(@"^20\d{2}.\s\d{2}.\s\d{2}");
            Regex dateRegex4 = new Regex(@"^\d{2}-[a-zA-Z]{1}[\u0000-\u00FF]{1}[a-zA-Z]{2}.-\d{4}$");
            if(dateRegex4.IsMatch(stockWorksheet.Cells[2,1].Value.ToString()))
            {
                Console.WriteLine("Match");
                return 0;
            }
            else
            {
                Console.WriteLine("Not matching");
            }
            return 0;
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
                    //Console.WriteLine(row+++ ":" + column + " -> " + stockWorksheet.Cells[row++, column].Value.ToString());
                    blank_cell_counter = 0;
                    row++;
                }
                else
                {
                    Console.WriteLine(row + 1 + ":" + column + " -> null");
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
            workbook.Close();
            excel.Quit();
        }
    }
}