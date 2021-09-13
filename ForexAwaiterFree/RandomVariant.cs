using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FB1
{
    class RandomVariant
    {
        public static TYPE_RANDOM RandomType;
        Random RandomX = new Random(RandomXS.Next(int.MinValue, int.MaxValue));//рандом для экземпляра
        public static Random RandomXS = new Random();//рандом для статик функции
        ///фрактальный подсчет числа коэффициентов
        public static int NumCAll = 0;//размер массива коэффициентов
        public static double Days;//торговать дней


        void DeepN(int Nums, int deepC = 1)//промежуточный фрактал
        {
            for (int i = 0; i < Nums; i++)
            {
                if (deepC > 1)
                {
                    DeepN(Nums, deepC - 1);
                }
                else
                {
                    NumCAll++;
                }
            }

            for (int i = 0; i < Nums; i++)
            {
                if (deepC > 1)
                {
                    DeepN(Nums, deepC - 1);
                }
                else
                {
                    NumCAll++;
                }
            }

            for (int i = 0; i < Nums; i++)
            {
                if (deepC > 1)
                {
                    DeepN(Nums, deepC - 1);
                }
                else
                {
                    NumCAll++;
                }
            }

            for (int i = 0; i < Nums; i++)
            {
                if (deepC > 1)
                {
                    DeepN(Nums, deepC - 1);
                }
                else
                {
                    NumCAll++;
                }
            }

            for (int i = 0; i < Nums; i++)
            {
                if (deepC > 1)
                {
                    DeepN(Nums, deepC - 1);
                }
                else
                {
                    NumCAll++;
                }
            }
        }

        public void CalcDeepN(int Nums, int deepC = 1)//посчитать число коэффицинтов
        {
            NumCAll = 0;
            for (int i = 0; i < deepC; i++)
            {
                DeepN(Nums, i + 1);
            }
        }

        ///
        public void GenerateC(Tester CoreWorker)
        {
            double RX;
            TYPE_RANDOM RT;
            RX = RandomX.NextDouble();
            if (RandomType == TYPE_RANDOM.RANDOM_TYPE_R) RT = (TYPE_RANDOM)RandomX.Next(0, Enum.GetValues(typeof(TYPE_RANDOM)).Length-1);
            else RT = RandomType;

            for (int i = 0; i < CoreWorker.Variant.ANum; i++)
            {
                if (RT == TYPE_RANDOM.RANDOM_TYPE_0) 
                {
                    if (i > 0) CoreWorker.Variant.Ci[i] = CoreWorker.Variant.Ci[i-1]*RandomX.NextDouble();
                    else CoreWorker.Variant.Ci[0]=1.0;
                }
                if (RT == TYPE_RANDOM.RANDOM_TYPE_5)
                {
                    if (RandomX.NextDouble() >= 0.5)
                    {
                        if (i > 0) CoreWorker.Variant.Ci[i] = CoreWorker.Variant.Ci[i - 1] * RandomX.NextDouble();
                        else CoreWorker.Variant.Ci[0] = 1.0;
                    }
                    else
                    {
                        if (i > 0) CoreWorker.Variant.Ci[i] = CoreWorker.Variant.Ci[i - 1] * (-RandomX.NextDouble());
                        else CoreWorker.Variant.Ci[0] = -1.0;
                    }
                }
                if (RT == TYPE_RANDOM.RANDOM_TYPE_1) CoreWorker.Variant.Ci[i] = RandomX.NextDouble();
                if (RT == TYPE_RANDOM.RANDOM_TYPE_2)
                {
                    if (RandomX.NextDouble() >= 0.5)
                    {
                        CoreWorker.Variant.Ci[i] = RandomX.NextDouble();
                    }
                    else
                    {
                        CoreWorker.Variant.Ci[i] = -RandomX.NextDouble();
                    }
                }
                if (RT == TYPE_RANDOM.RANDOM_TYPE_3)
                {
                    if (RandomX.NextDouble() >= RX)
                    {
                        if (RandomX.NextDouble() >= RX + (1.0 - RX) / 2.0)
                        {
                            CoreWorker.Variant.Ci[i] = RandomX.NextDouble();
                            ///Print(Variants[j].Ci[i]);
                        }
                        else
                        {
                            CoreWorker.Variant.Ci[i] = -RandomX.NextDouble();
                        }
                    }
                    else
                    {
                        CoreWorker.Variant.Ci[i] = 0.0;
                    }
                }
                if (RT == TYPE_RANDOM.RANDOM_TYPE_4)
                {
                    if (RandomX.NextDouble() >= RX)
                    {
                        CoreWorker.Variant.Ci[i] = RandomX.NextDouble();
                    }
                    else
                    {
                        CoreWorker.Variant.Ci[i] = 0.0;
                    }
                }
            }
        }

        private int IntervalLength(Tester CoreWorker)//подсчет длины интервала в минутах
        {
            int BorderMinuteStartTrade = CoreWorker.HourCorrect(CoreWorker.Variant.HourStart) * 60 + CoreWorker.MinuteCorrect(CoreWorker.Variant.MinuteStart);
            int BorderMinuteEndTrade = CoreWorker.HourCorrect(CoreWorker.Variant.HourEnd) * 60 + CoreWorker.MinuteCorrect(CoreWorker.Variant.MinuteEnd);
            if (BorderMinuteStartTrade > BorderMinuteEndTrade)//проверка временного интервала
            {
                return 60 * 24 - (BorderMinuteStartTrade - BorderMinuteEndTrade);
            }
            else
            {
                return BorderMinuteEndTrade - BorderMinuteStartTrade;
            }
        }
        public void GenerateRandomInterval(Tester CoreWorker)//рандомный интервал используя минимальную и максимальную продолжительность интервала
        {
            int ILT;
            do
            {
                CoreWorker.Variant.HourStart = RandomX.Next(0, 23);
                CoreWorker.Variant.MinuteStart = RandomX.Next(0, 59);
                CoreWorker.Variant.HourEnd = RandomX.Next(0, 23);
                CoreWorker.Variant.MinuteEnd = RandomX.Next(0, 59);
                ILT = IntervalLength(CoreWorker);
            }
            while (ILT <= BruteCode.MinIntervalLength || ILT >= BruteCode.MaxIntervalLength);
        }

        public void GenerateRandomDays(Tester CoreWorker)//рандомно сгенерируем массив с днями используя входные данные
        {
            int RandomLength = RandomX.Next(1, BruteCode.DaysToTrade.Length);
            int TR;
            bool bSame=false;
            CoreWorker.Variant.DaysToTrade = new int[RandomLength];
            for (int i = 0; i < RandomLength; i++)
            {
                do
                {
                    TR = RandomX.Next(0, BruteCode.DaysToTrade.Length);
                    bSame = false;
                    for (int j = 0; j < i; j++)
                    {
                        if (BruteCode.DaysToTrade[TR] == CoreWorker.Variant.DaysToTrade[j] ) bSame = true;
                    }
                    if (!bSame) CoreWorker.Variant.DaysToTrade[i] = BruteCode.DaysToTrade[TR];
                }
                while (bSame);
            }
        }

        public void GenerateBasicValues(Tester CoreWorker)//рандомно сгенерируем массив с днями используя входные данные
        {
            CoreWorker.Variant.DaysToTrade = new int[BruteCode.DaysToTrade.Length];
            for (int i = 0; i < BruteCode.DaysToTrade.Length; i++)
            {
                CoreWorker.Variant.DaysToTrade[i] = BruteCode.DaysToTrade[i];
            }
        }

        public void GenerateInterval(Tester CoreWorker)//сгенерируем интервал времени
        {
            CoreWorker.Variant.HourStart = BruteCode.HourStart;
            CoreWorker.Variant.MinuteStart = BruteCode.MinuteStart;
            CoreWorker.Variant.HourEnd = BruteCode.HourEnd;
            CoreWorker.Variant.MinuteEnd = BruteCode.MinuteEnd;
        }

        public void GenerateDays(Tester CoreWorker)//сгенерруем дни на основе входных данных
        {
            CoreWorker.Variant.DaysToTrade = new int[BruteCode.DaysToTrade.Length];
            for (int i = 0; i < BruteCode.DaysToTrade.Length; i++)
            {
                CoreWorker.Variant.DaysToTrade[i] = BruteCode.DaysToTrade[i];
            }
        }

        ///дальше генерация триггеров
        ///

        public static void FillTriggers(ref string st1,bool bOptimized)//заполним все триггеры
        {
            int sizeG = RandomXS.Next(1, 15000);
            string GS = "";
            for (int i = 0; i < sizeG; i++)
            {
                if (i > 0) GS += ("," + RandomXS.NextDouble().ToString().Replace(',', '.'));
                else GS += RandomXS.NextDouble().ToString().Replace(',', '.');
            }

            string CS = "";
            for (int i = 0; i < BruteCode.ReadedVariant.Ci.Length; i++)
            {
                if (i > 0) CS += ("," + BruteCode.ReadedVariant.Ci[i].ToString().Replace(',', '.'));
                else CS += BruteCode.ReadedVariant.Ci[i].ToString().Replace(',', '.');
            }

            string DS = "";//массив дней
            for (int i = 0; i < BruteCode.ReadedVariant.DaysToTrade.Length; i++)//
            {
                if (i > 0) DS += ("," + BruteCode.ReadedVariant.DaysToTrade[i].ToString());
                else DS += BruteCode.ReadedVariant.DaysToTrade[i].ToString();
            }

            string CNum = BruteCode.ReadedVariant.CNum.ToString().Replace(',', '.');
            string DeepBruteX = BruteCode.ReadedVariant.DeepBruteX.ToString().Replace(',', '.');
            string Optimized = BruteCode.ReadedVariant.OptimizedOpenValue.ToString().Replace(',', '.');
            string InvertS;
            if (BruteCode.ReadedVariant.Quality > 0) InvertS = "false";
            else InvertS = "true";
            string LossVal = Math.Abs(BruteCode.ReadedVariant.Quality).ToString().Replace(',', '.');
            string DDeal = DataReader.Data[0].TimeOpen.ToString();
            string Days0 = Days.ToString().Replace(',', '.');

            string Hs = BruteCode.ReadedVariant.HourStart.ToString();
            string Ms = BruteCode.ReadedVariant.MinuteStart.ToString();
            string He = BruteCode.ReadedVariant.HourEnd.ToString();
            string Me = BruteCode.ReadedVariant.MinuteEnd.ToString();
            string Tcb = BruteCode.ReadedVariant.bInitTimeBorders.ToString().ToLower();
            string Meth = BruteCode.Method.ToString().ToUpper();
            string DF = Days.ToString();

            string SI = Optimizer.bSpreadControl.ToString().ToLower();
            string SV = Optimizer.SpreadMax.ToString();
            string b_n = DataWriter.BotName.Replace(" ", "_");
            string B_N = DataWriter.BotName;
            //%%%LOSSVALUE%%% Martin Loss For Multiplier
            //%%%DEALVALUE%%% Deal Profit To Interrupt
            st1 = st1.Replace("%%%METHOD%%%", Meth);
            st1 = st1.Replace("%%%NAMESTRINGFIXED%%%", b_n);
            st1 = st1.Replace("%%%NAMESTRING%%%", B_N);

            st1 = st1.Replace("%%%SPREADINIT%%%", SI);
            st1 = st1.Replace("%%%SPREADVALUE%%%", SV);
            if ( Optimizer.bCloseFree ) st1 = st1.Replace("%%%SPREADVALUECLOSE%%%", SV+"000");
            else st1 = st1.Replace("%%%SPREADVALUECLOSE%%%", SV);

            st1 = st1.Replace("%%%CVALUES%%%", CS);
            st1 = st1.Replace("%%%GVALUES%%%", GS);
            st1 = st1.Replace("%%%DAYSFUTURE%%%", DF);
            st1 = st1.Replace("%%%DAYS%%%", DS);
            st1 = st1.Replace("%%%TIMECORRIDORVALUE%%%", Tcb);
            st1 = st1.Replace("%%%HOURSTARTVALUE%%%", Hs);
            st1 = st1.Replace("%%%MINUTESTARTVALUE%%%", Ms);
            st1 = st1.Replace("%%%HOURENDVALUE%%%", He);
            st1 = st1.Replace("%%%MINUTEENDVALUE%%%", Me);
            st1 = st1.Replace("%%%CNUMVALUE%%%", CNum);
            st1 = st1.Replace("%%%DEEPVALUE%%%", DeepBruteX);
            st1 = st1.Replace("%%%INVERT%%%", InvertS);
            st1 = st1.Replace("%%%LOSSVALUE%%%", LossVal);
            st1 = st1.Replace("%%%DATETIMESTART%%%", DDeal);
            if (bOptimized) st1 = st1.Replace("%%%OPTVALUE%%%", Optimized);
            else st1 = st1.Replace("%%%OPTVALUE%%%", "0.01");
            st1.Replace("%%%CLOSEVALUE%%%", BruteCode.ReadedVariant.OptimizedCloseValue.ToString().Replace(',', '.'));
            st1 = st1.Replace("%%%DAYS%%%", Days0);
        }
    }
}
