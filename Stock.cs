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

        //for StockDataGrid
        public string symbol { get; private set; }
        public string date { get; private set; }
        public double openPrice { get; private set; }
        public double highPrice { get; private set; }
        public double lowPrice { get; private set; }
        public double closePrice { get; private set; }
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
        //sql Stock constructor
        public Stock(string _smybol,string _date,double _openPrice,double _highPrice,double _lowPrice,double _closePrice)
        {
            symbol = _smybol;
            date = _date;
            openPrice = _openPrice;
            highPrice = _highPrice;
            lowPrice = _lowPrice;
            closePrice = _closePrice;
        }
        public string getSymbolToSql()
        {
            return symbol;
        }
        public string getDateToSql()
        {
            return date;
        }
        public double getOpenPrice()
        {
            return openPrice;
        }
        public double getHighPrice()
        {
            return highPrice;
        }
        public double getLowPrice()
        {
            return lowPrice;
        }
        public double getClosePrice()
        {
            return closePrice;
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
