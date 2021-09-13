using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FB1
{
    enum EQUATION//тип формулы
    {
        TEYLOR = 0,
        FOURIER = 1,
    };
    enum TYPE_RANDOM//тип формулы
    {
        RANDOM_TYPE_0 = 0,
        RANDOM_TYPE_1 = 1,
        RANDOM_TYPE_2 = 2,
        RANDOM_TYPE_3 = 3,
        RANDOM_TYPE_4 = 4,
        RANDOM_TYPE_5 = 5,
        RANDOM_TYPE_R = 6
    };

    enum QUALITY_MODE//тип поиска
    {
        MATH_WAITING = 1,
        P_FACTOR = 2
    };
    abstract class BruteCode:RandomVariant//содержит основной код бруттера
    {
        public static int MinIntervalLength;//минимальный размер интервала в минутах
        public static int MaxIntervalLength;//максимальный размер интервала в минутах
        public static bool bInitRandomTime;//включить рандомное время
        public static bool bInitRandomDays;//включить рандомные дни
        public static bool bInitTimeBorders;//включить контроль временных зон
        public static int HourStart;
        public static int MinuteStart;
        public static int HourEnd;
        public static int MinuteEnd;
        public static int[] DaysToTrade;
        /////////
        public static double RelativeDeviation;//относительное отклонение
        public static bool bSuper;//режим отклонения
        public static bool bSpreadControl;//учитывать спред из котировки
        public static int SpreadMax;//макс спред
        public static int DeepBrute;
        public static int PercentBrute;//***
        public static double DealsMinR=0.5;
        public static Data[] BestVariant;
        public static Data ReadedVariant;//вариант который читаем из файла
        public static QUALITY_MODE QualityModeE;
        public static EQUATION Method;//вид уравнения


        public void PreBuildNewVariant(Tester Worker)//предварительная подготовка нового варианта
        {
            Worker.BarsTotal = 0;
            ///
            if (DeepBrute > 1 && Method == EQUATION.TEYLOR ) CalcDeepN(Tester.SimulatedBars, DeepBrute);
            else if ( Method == EQUATION.TEYLOR ) NumCAll = Tester.SimulatedBars*5;
            else if ( Method == EQUATION.FOURIER ) NumCAll = Tester.SimulatedBars * 5 * 4;
            Worker.Variant.Ci = new double[NumCAll];
            Worker.Variant.bInitTimeBorders = bInitTimeBorders;
        }

        ///
        double ValA;
        int iterator;
        double PolinomValue(Tester CoreWorker)
        {
            ValA = 0;
            iterator = 0;

            if ( Method == EQUATION.TEYLOR )
            {
                if (CoreWorker.Variant.DeepBruteX <= 1)
                {
                    for (int i = 0; i < CoreWorker.Variant.CNum; i++)
                    {
                        ValA += CoreWorker.Variant.Ci[iterator] * (CoreWorker.CurrentBars[i + 2].PriceClose - CoreWorker.CurrentBars[i + 2].PriceOpen) / DataReader.Point;
                        iterator++;
                    }

                    for (int i = 0; i < CoreWorker.Variant.CNum; i++)
                    {
                        ValA += CoreWorker.Variant.Ci[iterator] * (CoreWorker.CurrentBars[i + 2].PriceHigh - CoreWorker.CurrentBars[i + 2].PriceOpen) / DataReader.Point;
                        iterator++;
                    }

                    for (int i = 0; i < CoreWorker.Variant.CNum; i++)
                    {
                        ValA += CoreWorker.Variant.Ci[iterator] * (CoreWorker.CurrentBars[i + 2].PriceOpen - CoreWorker.CurrentBars[i + 2].PriceLow) / DataReader.Point;
                        iterator++;
                    }

                    for (int i = 0; i < CoreWorker.Variant.CNum; i++)
                    {
                        ValA += CoreWorker.Variant.Ci[iterator] * (CoreWorker.CurrentBars[i + 2].PriceHigh - CoreWorker.CurrentBars[i + 2].PriceClose) / DataReader.Point;
                        iterator++;
                    }

                    for (int i = 0; i < CoreWorker.Variant.CNum; i++)
                    {
                        ValA += CoreWorker.Variant.Ci[iterator] * (CoreWorker.CurrentBars[i + 2].PriceClose - CoreWorker.CurrentBars[i + 2].PriceLow) / DataReader.Point;
                        iterator++;
                    }

                    return ValA;
                }
                else
                {
                    CalcDeep0(CoreWorker, CoreWorker.Variant.Ci, CoreWorker.Variant.CNum, CoreWorker.Variant.DeepBruteX);
                    return ValStart0;
                }
            }
            else if ( Method == EQUATION.FOURIER )
            {
                for (int i = 0; i < CoreWorker.Variant.CNum; i++)
                {
                    ValA += CoreWorker.Variant.Ci[iterator] * Math.Sin(CoreWorker.Variant.Ci[iterator+1] * (CoreWorker.CurrentBars[i + 2].PriceClose - CoreWorker.CurrentBars[i + 2].PriceOpen) / DataReader.Point)
                        + CoreWorker.Variant.Ci[iterator+2] * Math.Cos(CoreWorker.Variant.Ci[iterator + 3] * (CoreWorker.CurrentBars[i + 2].PriceClose - CoreWorker.CurrentBars[i + 2].PriceOpen) / DataReader.Point);
                    iterator=+4;
                }

                for (int i = 0; i < CoreWorker.Variant.CNum; i++)
                {
                    ValA += CoreWorker.Variant.Ci[iterator] * Math.Sin((CoreWorker.Variant.Ci[iterator+1] * CoreWorker.CurrentBars[i + 2].PriceHigh - CoreWorker.CurrentBars[i + 2].PriceOpen) / DataReader.Point)
                        + CoreWorker.Variant.Ci[iterator + 2] * Math.Cos((CoreWorker.Variant.Ci[iterator + 3] * CoreWorker.CurrentBars[i + 2].PriceHigh - CoreWorker.CurrentBars[i + 2].PriceOpen) / DataReader.Point);
                    iterator = +4;
                }

                for (int i = 0; i < CoreWorker.Variant.CNum; i++)
                {
                    ValA += CoreWorker.Variant.Ci[iterator] * Math.Sin(CoreWorker.Variant.Ci[iterator+1] * (CoreWorker.CurrentBars[i + 2].PriceOpen - CoreWorker.CurrentBars[i + 2].PriceLow) / DataReader.Point)
                        + CoreWorker.Variant.Ci[iterator+2] * Math.Cos(CoreWorker.Variant.Ci[iterator +3] * (CoreWorker.CurrentBars[i + 2].PriceOpen - CoreWorker.CurrentBars[i + 2].PriceLow) / DataReader.Point);
                    iterator = +4;
                }

                for (int i = 0; i < CoreWorker.Variant.CNum; i++)
                {
                    ValA += CoreWorker.Variant.Ci[iterator] * Math.Sin(CoreWorker.Variant.Ci[iterator + 1] * (CoreWorker.CurrentBars[i + 2].PriceHigh - CoreWorker.CurrentBars[i + 2].PriceClose) / DataReader.Point)
                        + CoreWorker.Variant.Ci[iterator+2] * Math.Cos(CoreWorker.Variant.Ci[iterator + 3] * (CoreWorker.CurrentBars[i + 2].PriceHigh - CoreWorker.CurrentBars[i + 2].PriceClose) / DataReader.Point);
                    iterator = +4;
                }

                for (int i = 0; i < CoreWorker.Variant.CNum; i++)
                {
                    ValA += CoreWorker.Variant.Ci[iterator] * Math.Sin(CoreWorker.Variant.Ci[iterator + 1] * (CoreWorker.CurrentBars[i + 2].PriceClose - CoreWorker.CurrentBars[i + 2].PriceLow) / DataReader.Point)
                        + CoreWorker.Variant.Ci[iterator+2] * Math.Cos(CoreWorker.Variant.Ci[iterator + 3] * (CoreWorker.CurrentBars[i + 2].PriceClose - CoreWorker.CurrentBars[i + 2].PriceLow) / DataReader.Point);
                    iterator = +4;
                }

                return ValA;
            }
            return 0.0;
        }
        ///

        ///фрактальный подсчет чисел
        double ValW0;//число куда умножаем все(а потом его прибавляем к ValStart)
        uint NumC0;//текущее число для коэффициента
        double ValStart0;//число куда суммируем все
        void Deep0(Tester CoreWorker,double[] Ci0, int Nums, int deepC , int deepStart, double Val0 = 1.0)//промежуточный фрактал
        {
            for (int i = 0; i < Nums; i++)
            {
                if (deepC > 1)
                {
                    ValW0 = (CoreWorker.CurrentBars[i + 2].PriceClose - CoreWorker.CurrentBars[i + 2].PriceOpen) * Val0;
                    Deep0(CoreWorker,Ci0, Nums, deepC - 1,deepStart, ValW0);
                }
                else
                {
                    ValStart0 += Ci0[NumC0] * (CoreWorker.CurrentBars[i + 2].PriceClose - CoreWorker.CurrentBars[i + 2].PriceOpen) * Val0 / DataReader.Point;
                    NumC0++;
                }
            }

            for (int i = 0; i < Nums; i++)
            {
                if (deepC > 1)
                {
                    ValW0 = (CoreWorker.CurrentBars[i + 2].PriceHigh - CoreWorker.CurrentBars[i + 2].PriceOpen) * Val0;
                    Deep0(CoreWorker, Ci0, Nums, deepC - 1,deepStart, ValW0);
                }
                else
                {
                    ValStart0 += (Ci0[NumC0]) * (CoreWorker.CurrentBars[i + 2].PriceHigh - CoreWorker.CurrentBars[i + 2].PriceOpen) * Val0 / DataReader.Point;
                    NumC0++;
                }
            }

            for (int i = 0; i < Nums; i++)
            {
                if (deepC > 1)
                {
                    ValW0 = (CoreWorker.CurrentBars[i + 2].PriceOpen - CoreWorker.CurrentBars[i + 2].PriceLow) * Val0;
                    Deep0(CoreWorker, Ci0, Nums, deepC - 1,deepStart, ValW0);
                }
                else
                {
                    ValStart0 += (Ci0[NumC0]) * (CoreWorker.CurrentBars[i + 2].PriceOpen - CoreWorker.CurrentBars[i + 2].PriceLow) * Val0 / DataReader.Point;
                    NumC0++;
                }
            }

            for (int i = 0; i < Nums; i++)
            {
                if (deepC > 1)
                {
                    ValW0 = (CoreWorker.CurrentBars[i + 2].PriceHigh - CoreWorker.CurrentBars[i + 2].PriceClose) * Val0;
                    Deep0(CoreWorker, Ci0, Nums, deepC - 1,deepStart, ValW0);
                }
                else
                {
                    ValStart0 += (Ci0[NumC0]) * (CoreWorker.CurrentBars[i + 2].PriceHigh - CoreWorker.CurrentBars[i + 2].PriceClose) * Val0 / DataReader.Point;
                    NumC0++;
                }
            }

            for (int i = 0; i < Nums; i++)
            {
                if (deepC > 1)
                {
                    ValW0 = (CoreWorker.CurrentBars[i + 2].PriceClose - CoreWorker.CurrentBars[i + 2].PriceLow) * Val0;
                    Deep0(CoreWorker, Ci0, Nums, deepC - 1,deepStart, ValW0);
                }
                else
                {
                    ValStart0 += (Ci0[NumC0]) * (CoreWorker.CurrentBars[i + 2].PriceClose - CoreWorker.CurrentBars[i + 2].PriceLow) * Val0 / DataReader.Point;
                    NumC0++;
                }
            }
        }

        void CalcDeep0(Tester CoreWorker,double[] Ci0, int Nums, int deepC = 1)
        {
            NumC0 = 0;
            ValStart0 = 0.0;
            for (int i = 0; i < deepC; i++)
            {
                Deep0(CoreWorker,Ci0, Nums,i + 1, i + 1);
            }
        }
        ///      

        public void Iteration(Tester CoreWorker)//прохождение одного бара вариантом
        {
            double TempV = PolinomValue(CoreWorker);
            CoreWorker.BarsTotal++;
            //Print(TempV);
            if (TempV < 0)
            {
                if (!bSpreadControl)
                {
                    CoreWorker.Variant.MA0Up += (CoreWorker.CurrentBars[1].PriceClose - CoreWorker.CurrentBars[1].PriceOpen);
                    CoreWorker.Variant.MA0UpAbs += Math.Abs(CoreWorker.CurrentBars[1].PriceClose - CoreWorker.CurrentBars[1].PriceOpen);
                    CoreWorker.Variant.MA0UpO += -TempV;
                    CoreWorker.Variant.NumMinus++;
                    if (bSuper) CoreWorker.Variant.Orders++;
                    if (bSuper) CoreWorker.Variant.TotalProfit += (CoreWorker.CurrentBars[1].PriceClose - CoreWorker.CurrentBars[1].PriceOpen);
                    if (bSuper) CoreWorker.Variant.CurrentDeviation = Math.Abs(CoreWorker.Variant.TotalProfit - CoreWorker.Variant.KProfit * CoreWorker.Variant.Orders);
                }
                else if ( CoreWorker.CurrentBars[1].Spread <= SpreadMax )
                {
                    CoreWorker.Variant.MA0Up += (CoreWorker.CurrentBars[1].PriceClose - CoreWorker.CurrentBars[1].PriceOpen - CoreWorker.CurrentBars[1].Spread * DataReader.Point);
                    CoreWorker.Variant.MA0UpAbs += Math.Abs(CoreWorker.CurrentBars[1].PriceClose - CoreWorker.CurrentBars[1].PriceOpen - CoreWorker.CurrentBars[1].Spread * DataReader.Point);
                    CoreWorker.Variant.MA0UpO += -TempV;
                    CoreWorker.Variant.NumMinus++;
                    if (bSuper) CoreWorker.Variant.Orders++;
                    if (bSuper) CoreWorker.Variant.TotalProfit += (CoreWorker.CurrentBars[1].PriceClose - CoreWorker.CurrentBars[1].PriceOpen);
                    if (bSuper) CoreWorker.Variant.CurrentDeviation = Math.Abs(CoreWorker.Variant.TotalProfit - CoreWorker.Variant.KProfit * CoreWorker.Variant.Orders);
                }
            }
            if (TempV > 0)
            {
                if (!bSpreadControl)
                {
                    CoreWorker.Variant.MA0Down += (CoreWorker.CurrentBars[1].PriceOpen - CoreWorker.CurrentBars[1].PriceClose);
                    CoreWorker.Variant.MA0DownAbs += Math.Abs(CoreWorker.CurrentBars[1].PriceOpen - CoreWorker.CurrentBars[1].PriceClose);
                    CoreWorker.Variant.MA0DownO += TempV;
                    CoreWorker.Variant.NumPlus++;
                    if (bSuper) CoreWorker.Variant.Orders++;
                    if (bSuper) CoreWorker.Variant.TotalProfit += (CoreWorker.CurrentBars[1].PriceOpen - CoreWorker.CurrentBars[1].PriceClose);
                    if (bSuper) CoreWorker.Variant.CurrentDeviation = Math.Abs(CoreWorker.Variant.TotalProfit - CoreWorker.Variant.KProfit * CoreWorker.Variant.Orders);
                }
                else if (CoreWorker.CurrentBars[1].Spread <= SpreadMax)
                {
                    CoreWorker.Variant.MA0Down += (CoreWorker.CurrentBars[1].PriceOpen - CoreWorker.CurrentBars[1].PriceClose - CoreWorker.CurrentBars[0].Spread * DataReader.Point);
                    CoreWorker.Variant.MA0DownAbs += Math.Abs(CoreWorker.CurrentBars[1].PriceOpen - CoreWorker.CurrentBars[1].PriceClose - CoreWorker.CurrentBars[0].Spread * DataReader.Point);
                    CoreWorker.Variant.MA0DownO += TempV;
                    CoreWorker.Variant.NumPlus++;
                    if (bSuper) CoreWorker.Variant.Orders++;
                    if (bSuper) CoreWorker.Variant.TotalProfit += (CoreWorker.CurrentBars[1].PriceOpen - CoreWorker.CurrentBars[1].PriceClose);
                    if (bSuper) CoreWorker.Variant.CurrentDeviation = Math.Abs(CoreWorker.Variant.TotalProfit - CoreWorker.Variant.KProfit * CoreWorker.Variant.Orders);
                }

            }
            if (Math.Abs(TempV) > CoreWorker.Variant.MaxValueAbs)
            {
                CoreWorker.Variant.MaxValueAbs = Math.Abs(TempV);
            }
        }
    }
}
