using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class Stock
    {
        public string stockName { get; private set; }
        public double stockPrice { get; private set; }
        public int quantity { get; private set; }
        public string transactionDate { get; private set; }
        public string transactionType { get; private set; }
        public string writeDate { get; private set; }
        public Stock(string _stockName,double _stockPrice,int _quantity,string _transactionDate,string _transactionType)
        {
            stockName = _stockName;
            stockPrice = _stockPrice;
            quantity = _quantity;
            transactionDate = _transactionDate;
            transactionType = _transactionType;
        }
        public Stock(string _writeDate, string _stockName, double _stockPrice, int _quantity, string _transactionDate, string _transactionType)
        {
            writeDate = _writeDate;
            stockName = _stockName;
            stockPrice = _stockPrice;
            quantity = _quantity;
            transactionDate = _transactionDate;
            transactionType = _transactionType;
        }
        public Stock(string name,float price)
        {
            stockName = name;
            stockPrice = price;
        }
        public string getStockName()
        {
            return stockName;
        }
        public double getStockPrice()
        {
            return stockPrice;
        }
        public string getTransactionDate()
        {
            return transactionDate;
        }
        public string getTransactionType()
        {
            return transactionType;
        }
        public int getQuantity()
        {
            return quantity;
        }
    }
}
