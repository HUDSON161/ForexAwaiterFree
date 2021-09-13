using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FB1
{
    struct DataTester//массив котировок полученный из файла
    {
        public double PriceClose;
        public double PriceOpen;
        public double PriceHigh;
        public double PriceLow;
        public int TimeOpen;//секунды от начала истории
        public int HourOpen;
        public int MinuteOpen;
        public int DayOpen;
        public int Spread;//спред на открытии свечи
        public int SpreadClose;//спред на закрытии свечи
        public int SpreadHigh;//спред на High
        public int SpreadLow;//спред на Low
    }
}
