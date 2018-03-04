using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    class Stock
    {
        private string stockName;
        private float stockPrice;
        private string StockTimeID;
        public Stock(string name,float price,string time_id)
        {
            stockName = name;
            stockPrice = price;
            StockTimeID = time_id;
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
        public float getStockPrice()
        {
            return stockPrice;
        }
    }
}
