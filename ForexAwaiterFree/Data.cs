using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FB1
{
    struct Data
    {
        public uint NumBars;//число баров
        public double MA0Up;//матожидание величины реальной свечи вверх
        public double MA0Down;//матожидание величины реальной свечи вниз
        public double MA0UpAbs;//матожидание абсолютной величины реальной свечи вверх
        public double MA0DownAbs;//матожидание абсолютной величины реальной свечи вниз      
        public double MA0UpO;//матожидание предсказания вверх
        public double MA0DownO;//матожидание предсказания вниз

        public double MaxProfit;//максимальный текущий профит
        public double MinProfit;//максимальный текущий проигрыш

        public double TotalProfit;//вспомогательная переменная для оптимизации кривизны
        public double ProfitDelta;//максимальная величина отклонения
        public double KProfit;//коэффициент наклона для оптимизации кривизны
        public double CurrentDeviation;//текущее отклонение от прямой

        public double PointsDown;//просадка вниз до первого профита
        public double PointsUp;//просадка вверх до первого проигрыша

        public double MaxValueAbs;//максимальное значение критерия
        public int NumBarsGood;//число хороших баров
        public bool bInvertThis;//знак сигнала
        public int CNum;//количество свечей в формуле
        public int ANum;//количество коэффициентов в формуле
        public int DeepBruteX;//глубина формулы
        public uint NumPlus;//число сигналов в плюс
        public uint NumMinus;//число сигналов в плюс

        public double OptimizedOpenValue;//оптимизированное число для открытия
        public int Orders;//ордеров в варианте
        public double Quality;//качество предсказания

        public bool bInitTimeBorders;//включить контроль временных зон
        public bool bCloseInOutOfTime;//закрывать вне временной зоны
        public bool bCloseFree;//закрыть свободно
        public bool bSpreadNoize;//включить подавление спредовых шумов
        public int HourStart;//час начала торговли
        public int MinuteStart;//минута начала торговли
        public int HourEnd;//час конца торговли
        public int MinuteEnd;//минута конца торговли
        public int[] DaysToTrade;
        public int index;//индекс в списке первой вкладки
        public EQUATION Method;//метод
        public double OptimizedCloseValue;//значение для закрытия

        public double[] Ci;//коэффициенты
        public List<Awaiter.Order> Order;//ордера
        //дальше проценты правильного предсказания
        public void CopyData(Data outer)
        {
            NumBars = outer.NumBars;
            MA0Up = outer.MA0Up;
            MA0Down = outer.MA0Down;
            MA0UpAbs = outer.MA0UpAbs;
            MA0DownAbs = outer.MA0DownAbs;
            MA0UpO = outer.MA0UpO;
            MA0DownO = outer.MA0DownO;

            MaxProfit = outer.MaxProfit;
            MinProfit = outer.MinProfit;

            MaxValueAbs = outer.MaxValueAbs;
            NumBarsGood = outer.NumBarsGood;
            bInvertThis = outer.bInvertThis;
            CNum = outer.CNum;
            ANum = outer.ANum;
            DeepBruteX = outer.DeepBruteX;
            NumPlus = outer.NumPlus;
            NumMinus = outer.NumMinus;
            Orders = outer.Orders;

            OptimizedOpenValue = outer.OptimizedOpenValue;

            Quality = outer.Quality;

            bInitTimeBorders = outer.bInitTimeBorders;//включить контроль временных зон
            bCloseInOutOfTime = outer.bCloseInOutOfTime;
            bCloseFree = outer.bCloseFree;
            bSpreadNoize = outer.bSpreadNoize;//включить подавление спредовых шумов
            HourStart = outer.HourStart;//час начала торговли
            MinuteStart = outer.MinuteStart;//минута начала торговли
            HourEnd = outer.HourEnd;//час конца торговли
            MinuteEnd = outer.MinuteEnd;//минута конца торговли
            DaysToTrade = outer.DaysToTrade;
            index = outer.index;
            Method = outer.Method;
            OptimizedCloseValue = outer.OptimizedCloseValue;//для закрытия

            Ci = outer.Ci;
        }

        public void CopyDataOptimized(Data outer)
        {
            Ci = outer.Ci;
            Order = new List<Awaiter.Order>(outer.Order);//скопируем ордера

            NumBars = outer.NumBars;
            MA0Up = outer.MA0Up;
            MA0Down = outer.MA0Down;
            MA0UpAbs = outer.MA0UpAbs;
            MA0DownAbs = outer.MA0DownAbs;
            MA0UpO = outer.MA0UpO;
            MA0DownO = outer.MA0DownO;

            MaxProfit = outer.MaxProfit;
            MinProfit = outer.MinProfit;

            MaxValueAbs = outer.MaxValueAbs;
            NumBarsGood = outer.NumBarsGood;
            bInvertThis = outer.bInvertThis;
            CNum = outer.CNum;
            ANum = outer.ANum;
            DeepBruteX = outer.DeepBruteX;
            NumPlus = outer.NumPlus;
            NumMinus = outer.NumMinus;
            Orders = outer.Orders;

            OptimizedOpenValue = outer.OptimizedOpenValue;

            Quality = outer.Quality;

            bInitTimeBorders = outer.bInitTimeBorders;
            bCloseInOutOfTime = outer.bCloseInOutOfTime;
            bCloseFree = outer.bCloseFree;
            bSpreadNoize = outer.bSpreadNoize;
            HourStart = outer.HourStart;
            MinuteStart = outer.MinuteStart;
            HourEnd = outer.HourEnd;
            MinuteEnd = outer.MinuteEnd;
            DaysToTrade = outer.DaysToTrade;
            index = outer.index;
            Method = outer.Method;
        }

        public void CopyC(Data outer)
        {
            NumBars = outer.NumBars;
            CNum = outer.CNum;
            ANum = outer.ANum;
            DeepBruteX = outer.DeepBruteX;
            OptimizedOpenValue = outer.OptimizedOpenValue;
            MaxValueAbs = outer.MaxValueAbs;
            index = outer.index;
            Method = outer.Method;
            bSpreadNoize = outer.bSpreadNoize;//включить подавление спредовых шумов
            bCloseInOutOfTime = outer.bCloseInOutOfTime;
            bCloseFree = outer.bCloseFree;

            Ci = new double[outer.Ci.Length];
            for (int i = 0; i < outer.Ci.Length; i++)
            {
                Ci[i] = outer.Ci[i];
            }
        }

        public void CopyT(Data outer)
        {
            bInitTimeBorders = outer.bInitTimeBorders;
            bCloseInOutOfTime = outer.bCloseInOutOfTime;
            bCloseFree = outer.bCloseFree;
            HourStart = outer.HourStart;
            MinuteStart = outer.MinuteStart;
            HourEnd = outer.HourEnd;
            MinuteEnd = outer.MinuteEnd;

            DaysToTrade = new int[outer.DaysToTrade.Length];
            for (int i = 0; i < outer.DaysToTrade.Length; i++)
            {
                DaysToTrade[i] = outer.DaysToTrade[i];
            }
        }

    }
}
