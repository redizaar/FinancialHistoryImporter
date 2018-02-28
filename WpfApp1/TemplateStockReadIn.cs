using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Data.SqlClient;
using System.Data;
namespace WpfApp1
{
    public class TemplateStockReadIn
    {
        private List<string> folderAddresses;
        private ImportReadIn stockHandler;
        public TemplateStockReadIn(ImportReadIn _stockHandler,List<string> _folderAddresses)
        {
            stockHandler = _stockHandler;
            folderAddresses = _folderAddresses;
        }
    }
}