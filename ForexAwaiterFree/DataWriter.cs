using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace FB1
{
    static class DataWriter//для сохранеия прогресса
    {
        public static string FileSaveName="Saved.txt";
        public static string BotName;

        public static bool BasicSaved = true;//для того чтобы не было конфликтов при записи
        public static bool ListSaved = true;//для того чтобы не было конфликтов при записи
        public static bool OptimizedSaved = true;//для того чтобы не было конфликтов при записи

        public static void SaveData()//проверка найденного варианта и сохранение
        {
            ReadData();
            string FileLine = FileSaveName + " " + BruteCode.QualityModeE.ToString() + ".txt";
            if (Math.Abs(BruteCode.ReadedVariant.Quality) < Math.Abs(BruteCode.BestVariant[0].Quality))
            {
                if (BasicSaved)
                {
                    BasicSaved = false;
                    using (StreamWriter sw = new StreamWriter(FileLine, false))
                    {
                        bool[] TempDays;
                        sw.WriteLine(BruteCode.BestVariant[0].Quality.ToString().Replace(',', '.'));
                        sw.WriteLine(BruteCode.BestVariant[0].NumBars.ToString().Replace(',', '.'));
                        sw.WriteLine(BruteCode.SpreadMax.ToString().Replace(',', '.'));
                        sw.WriteLine(0.000000001.ToString().Replace(',', '.'));
                        sw.WriteLine(BruteCode.BestVariant[0].CNum.ToString().Replace(',', '.'));
                        sw.WriteLine(BruteCode.BestVariant[0].ANum.ToString().Replace(',', '.'));
                        sw.WriteLine(BruteCode.BestVariant[0].DeepBruteX.ToString().Replace(',', '.'));
                        sw.WriteLine(DataReader.Data[0].TimeOpen.ToString().Replace(',', '.'));
                        //////блок временных интервалов и дней
                        sw.WriteLine(BruteCode.BestVariant[0].bInitTimeBorders.ToString());
                        sw.WriteLine(true.ToString());
                        sw.WriteLine(true.ToString());//CloseFree
                        ///для записи дней
                        TempDays = new bool[7];
                        for (int i = 0; i < 7; i++)
                        {
                            TempDays[i] = false;
                        }
                        for (int i = 0; i < BruteCode.BestVariant[0].DaysToTrade.Length; i++)
                        {
                            TempDays[BruteCode.BestVariant[0].DaysToTrade[i]] = true;
                        }
                        for (int i = 0; i < 7; i++)
                        {
                            sw.WriteLine(TempDays[i].ToString());
                        }
                        ///конец записи дней
                        sw.WriteLine(BruteCode.BestVariant[0].HourStart.ToString());
                        sw.WriteLine(BruteCode.BestVariant[0].HourEnd.ToString());
                        sw.WriteLine(BruteCode.BestVariant[0].MinuteStart.ToString());
                        sw.WriteLine(BruteCode.BestVariant[0].MinuteEnd.ToString());
                        //////конец блока временных интервалов и дней
                        sw.WriteLine(BruteCode.BestVariant[0].Method.ToString().ToUpper());
                        sw.WriteLine(BruteCode.BestVariant[0].bSpreadNoize.ToString());
                        sw.WriteLine(0.001.ToString().Replace(',', '.'));
                        for (int i = 0; i < BruteCode.BestVariant[0].ANum; i++)
                        {
                            sw.WriteLine(BruteCode.BestVariant[0].Ci[i].ToString().Replace(',', '.'));
                        }
                    }
                    BasicSaved = true;
                }

            }
        }

        public static void SaveDataOptimized()//сохранение оптимизированного варианта
        {
            ReadData();
            string FileLine = FileSaveName + " " + Optimizer.QualityModeE.ToString() + " OPTIMIZED.txt";
            if (OptimizedSaved)
            {
                OptimizedSaved = false;
                using (StreamWriter sw = new StreamWriter(FileLine, false))
                {
                    sw.WriteLine(Optimizer.VariantOptimal.Quality.ToString().Replace(',', '.'));
                    sw.WriteLine(Optimizer.VariantOptimal.NumBars.ToString().Replace(',', '.'));
                    sw.WriteLine(Optimizer.SpreadMax.ToString().Replace(',', '.'));
                    sw.WriteLine(Optimizer.VariantOptimal.OptimizedOpenValue.ToString().Replace(',', '.'));
                    sw.WriteLine(Optimizer.VariantOptimal.CNum.ToString().Replace(',', '.'));
                    sw.WriteLine(Optimizer.VariantOptimal.ANum.ToString().Replace(',', '.'));
                    sw.WriteLine(Optimizer.VariantOptimal.DeepBruteX.ToString().Replace(',', '.'));
                    sw.WriteLine(DataReader.Data[0].TimeOpen.ToString().Replace(',', '.'));
                    //////блок временных интервалов и дней
                    sw.WriteLine(Optimizer.VariantOptimal.bInitTimeBorders.ToString());
                    sw.WriteLine(Optimizer.bCloseInOutOfTime.ToString());
                    sw.WriteLine(Optimizer.bCloseFree.ToString());
                    ///для записи дней
                    bool[] TempDays = new bool[7];
                    for (int i = 0; i < 7; i++)
                    {
                        TempDays[i] = false;
                    }
                    for (int i = 0; i < Optimizer.VariantOptimal.DaysToTrade.Length; i++)
                    {
                        TempDays[Optimizer.VariantOptimal.DaysToTrade[i]] = true;
                    }
                    for (int i = 0; i < 7; i++)
                    {
                        sw.WriteLine(TempDays[i].ToString());
                    }
                    ///конец записи дней
                    sw.WriteLine(Optimizer.VariantOptimal.HourStart.ToString());
                    sw.WriteLine(Optimizer.VariantOptimal.HourEnd.ToString());
                    sw.WriteLine(Optimizer.VariantOptimal.MinuteStart.ToString());
                    sw.WriteLine(Optimizer.VariantOptimal.MinuteEnd.ToString());
                    //////конец блока временных интервалов и дней
                    sw.WriteLine(Optimizer.VariantOptimal.Method.ToString().ToUpper());
                    sw.WriteLine(Optimizer.VariantOptimal.bSpreadNoize.ToString());
                    if ( !Optimizer.bScalping )
                    {
                        sw.WriteLine(Optimizer.VariantOptimal.OptimizedOpenValue.ToString().Replace(',', '.'));
                    }
                    else
                    {
                        sw.WriteLine(0.001.ToString().Replace(',', '.'));
                    }
                    for (int i = 0; i < Optimizer.VariantOptimal.ANum; i++)
                    {
                        sw.WriteLine(Optimizer.VariantOptimal.Ci[i].ToString().Replace(',', '.'));
                    }
                }
                OptimizedSaved = true;
            }

        }

        public static void SaveDataList()//сохранение всего списка вариантов
        {
            string FileLine = FileSaveName + " " + BruteCode.QualityModeE.ToString() + " List" + ".txt";
            if (ListSaved)
            {
                ListSaved = false;
                using (StreamWriter sw = new StreamWriter(FileLine, false))
                {
                    for (int i = 0; i < BruteCode.BestVariant.Length; i++)
                    {
                        if (BruteCode.BestVariant[i].Ci != null)
                        {
                            sw.WriteLine(BruteCode.BestVariant[i].Quality.ToString().Replace(',', '.'));
                            sw.WriteLine(BruteCode.BestVariant[i].NumBars.ToString().Replace(',', '.'));
                            sw.WriteLine(BruteCode.BestVariant[i].CNum.ToString().Replace(',', '.'));
                            sw.WriteLine(BruteCode.BestVariant[i].ANum.ToString().Replace(',', '.'));
                            sw.WriteLine(BruteCode.BestVariant[i].DeepBruteX.ToString().Replace(',', '.'));
                            sw.WriteLine(BruteCode.BestVariant[i].MaxValueAbs.ToString().Replace(',', '.'));
                            //////блок временных интервалов и дней
                            sw.WriteLine(BruteCode.BestVariant[i].bInitTimeBorders.ToString());
                            sw.WriteLine(true.ToString());
                            sw.WriteLine(true.ToString());
                            ///для записи дней
                            bool[] TempDays = new bool[7];
                            for (int j = 0; j < 7; j++)
                            {
                                TempDays[j] = false;
                            }
                            for (int j = 0; j < BruteCode.BestVariant[i].DaysToTrade.Length; j++)
                            {
                                TempDays[BruteCode.BestVariant[i].DaysToTrade[j]] = true;
                            }
                            for (int j = 0; j < 7; j++)
                            {
                                sw.WriteLine(TempDays[j].ToString());
                            }
                            ///конец записи дней
                            sw.WriteLine(BruteCode.BestVariant[i].HourStart.ToString());
                            sw.WriteLine(BruteCode.BestVariant[i].HourEnd.ToString());
                            sw.WriteLine(BruteCode.BestVariant[i].MinuteStart.ToString());
                            sw.WriteLine(BruteCode.BestVariant[i].MinuteEnd.ToString());
                            //////конец блока временных интервалов и дней
                            sw.WriteLine(BruteCode.BestVariant[i].Method.ToString().ToUpper());
                            sw.WriteLine(BruteCode.BestVariant[i].bSpreadNoize.ToString());
                            sw.WriteLine(0.001.ToString().Replace(',', '.'));
                            for (int j = 0; j < BruteCode.BestVariant[0].ANum; j++)
                            {
                                sw.WriteLine(BruteCode.BestVariant[i].Ci[j].ToString().Replace(',', '.'));
                            }
                            if (i < BruteCode.BestVariant.Length - 1) sw.WriteLine();
                        }
                    }
                }
                ListSaved = true;
            }
        }

        public static void ReadData()//чтение показателя качества из сохранения
        {
            string FileLine = FileSaveName + " " + BruteCode.QualityModeE.ToString() + ".txt";
            if (File.Exists(FileLine))
            {
                using (StreamReader sr = new StreamReader(FileLine, System.Text.Encoding.Default))
                {
                    string line;
                    if ((line = sr.ReadLine()) != null) BruteCode.ReadedVariant.Quality = double.Parse(line == "" ? "0,0" : line.Replace('.', ','));//читаем имя файла
                }
            }
        }

        static void ReadDataFull(bool bOptimized)//чтение данных для робота
        {
            if ( !bOptimized )
            {
                string FileLine = FileSaveName + " " + BruteCode.QualityModeE.ToString() + ".txt";
                if (File.Exists(FileLine))
                {
                    BruteCode.ReadedVariant = new Data();
                    using (StreamReader sr = new StreamReader(FileLine, System.Text.Encoding.Default))
                    {
                        string line;
                        if ((line = sr.ReadLine()) != null) BruteCode.ReadedVariant.Quality = double.Parse(line == "" ? "0,0" : line.Replace('.', ','));//читаем качество
                        sr.ReadLine();
                        sr.ReadLine();
                        BruteCode.ReadedVariant.OptimizedOpenValue = Convert.ToDouble(sr.ReadLine().Replace('.', ','));
                        BruteCode.ReadedVariant.CNum = Convert.ToInt32(sr.ReadLine());
                        BruteCode.ReadedVariant.ANum = Convert.ToInt32(sr.ReadLine());
                        BruteCode.ReadedVariant.DeepBruteX = Convert.ToInt32(sr.ReadLine());
                        sr.ReadLine();
                        //////блок временных интервалов и дней
                        BruteCode.ReadedVariant.bInitTimeBorders = Convert.ToBoolean(sr.ReadLine());
                        BruteCode.ReadedVariant.bCloseInOutOfTime = Convert.ToBoolean(sr.ReadLine());
                        BruteCode.ReadedVariant.bCloseFree = Convert.ToBoolean(sr.ReadLine());
                        ///для чтения дней
                        bool[] TempDays = new bool[7];
                        for (int i = 0; i < 7; i++) TempDays[i] = Convert.ToBoolean(sr.ReadLine());
                        int count = 0;
                        for (int i = 0; i < 7; i++)
                        {
                            if (TempDays[i]) count++;
                        }
                        BruteCode.ReadedVariant.DaysToTrade = new int[count];
                        count = 0;
                        for (int i = 0; i < 7; i++)
                        {
                            if (TempDays[i])
                            {
                                BruteCode.ReadedVariant.DaysToTrade[count] = i;
                                count++;
                            }
                        }
                        ///конец чтения дней
                        BruteCode.ReadedVariant.HourStart = Convert.ToInt32(sr.ReadLine());
                        BruteCode.ReadedVariant.HourEnd = Convert.ToInt32(sr.ReadLine());
                        BruteCode.ReadedVariant.MinuteStart = Convert.ToInt32(sr.ReadLine());
                        BruteCode.ReadedVariant.MinuteEnd = Convert.ToInt32(sr.ReadLine());
                        //////конец блока временных интервалов и дней
                        BruteCode.ReadedVariant.Method = (EQUATION)Enum.Parse(BruteCode.ReadedVariant.Method.GetType(), sr.ReadLine());
                        BruteCode.ReadedVariant.bSpreadNoize = Convert.ToBoolean(sr.ReadLine());
                        BruteCode.ReadedVariant.OptimizedCloseValue = Convert.ToDouble(sr.ReadLine().Replace('.', ','));
                        BruteCode.ReadedVariant.Ci = new double[BruteCode.ReadedVariant.ANum];
                        for (int i = 0; i < BruteCode.ReadedVariant.ANum; i++)
                        {
                            BruteCode.ReadedVariant.Ci[i] = Convert.ToDouble(sr.ReadLine().Replace('.', ','));
                        }
                    }
                }
            }
            else
            {
                string FileLine = FileSaveName + " " + Optimizer.QualityModeE.ToString()+ " OPTIMIZED.txt";
                if (File.Exists(FileLine))
                {
                    BruteCode.ReadedVariant = new Data();
                    using (StreamReader sr = new StreamReader(FileLine, System.Text.Encoding.Default))
                    {
                        string line;
                        if ((line = sr.ReadLine()) != null) BruteCode.ReadedVariant.Quality = double.Parse(line == "" ? "0,0" : line.Replace('.', ','));//читаем качество
                        sr.ReadLine();
                        sr.ReadLine();
                        BruteCode.ReadedVariant.OptimizedOpenValue= double.Parse(sr.ReadLine().Replace('.', ','));
                        BruteCode.ReadedVariant.CNum = Convert.ToInt32(sr.ReadLine());
                        BruteCode.ReadedVariant.ANum = Convert.ToInt32(sr.ReadLine());
                        BruteCode.ReadedVariant.DeepBruteX = Convert.ToInt32(sr.ReadLine());
                        sr.ReadLine();
                        //////блок временных интервалов и дней
                        BruteCode.ReadedVariant.bInitTimeBorders = Convert.ToBoolean(sr.ReadLine());
                        BruteCode.ReadedVariant.bCloseInOutOfTime = Convert.ToBoolean(sr.ReadLine());
                        BruteCode.ReadedVariant.bCloseFree = Convert.ToBoolean(sr.ReadLine());
                        ///для чтения дней
                        bool[] TempDays = new bool[7];
                        for (int i = 0; i < 7; i++) TempDays[i] = Convert.ToBoolean(sr.ReadLine());
                        int count = 0;
                        for (int i = 0; i < 7; i++)
                        {
                            if (TempDays[i]) count++;
                        }
                        BruteCode.ReadedVariant.DaysToTrade = new int[count];
                        count = 0;
                        for (int i = 0; i < 7; i++)
                        {
                            if (TempDays[i])
                            {
                                BruteCode.ReadedVariant.DaysToTrade[count] = i;
                                count++;
                            }
                        }
                        ///конец чтения дней
                        BruteCode.ReadedVariant.HourStart = Convert.ToInt32(sr.ReadLine());
                        BruteCode.ReadedVariant.HourEnd = Convert.ToInt32(sr.ReadLine());
                        BruteCode.ReadedVariant.MinuteStart = Convert.ToInt32(sr.ReadLine());
                        BruteCode.ReadedVariant.MinuteEnd = Convert.ToInt32(sr.ReadLine());
                        //////конец блока временных интервалов и дней
                        BruteCode.ReadedVariant.Method = (EQUATION)Enum.Parse(BruteCode.ReadedVariant.Method.GetType(), sr.ReadLine());
                        BruteCode.ReadedVariant.bSpreadNoize = Convert.ToBoolean(sr.ReadLine());
                        BruteCode.ReadedVariant.OptimizedCloseValue = Convert.ToDouble(sr.ReadLine().Replace('.', ','));

                        BruteCode.ReadedVariant.Ci = new double[BruteCode.ReadedVariant.ANum];
                        for (int i = 0; i < BruteCode.ReadedVariant.ANum; i++)
                        {
                            BruteCode.ReadedVariant.Ci[i] = Convert.ToDouble(sr.ReadLine().Replace('.', ','));
                        }
                    }
                }
            }
        }

        public static void ReadDataVariants()//чтение данных всех вариантов
        {
            string FileLine = FileSaveName + " " + BruteCode.QualityModeE.ToString()+" List" + ".txt";
            if (File.Exists(FileLine))
            {
                List<Data> TempVar = new List<Data>();
                Data NewVar;
                using (StreamReader sr = new StreamReader(FileLine, System.Text.Encoding.Default))
                {
                    string line;
                    bool[] TempDays;
                    while ((line = sr.ReadLine()) != null )
                    {
                        NewVar = new Data();
                        NewVar.Quality = double.Parse(line == "" ? "0,0" : line.Replace('.', ','));//читаем имя файла
                        NewVar.NumBars= (uint)Convert.ToInt32(sr.ReadLine());
                        NewVar.CNum = Convert.ToInt32(sr.ReadLine());
                        NewVar.ANum = Convert.ToInt32(sr.ReadLine());
                        NewVar.DeepBruteX = Convert.ToInt32(sr.ReadLine());
                        line = sr.ReadLine();
                        NewVar.MaxValueAbs= double.Parse(line == "" ? "0,0" : line.Replace('.', ','));
                        //////блок временных интервалов и дней
                        NewVar.bInitTimeBorders = Convert.ToBoolean(sr.ReadLine());
                        NewVar.bCloseInOutOfTime = Convert.ToBoolean(sr.ReadLine());
                        NewVar.bCloseFree = Convert.ToBoolean(sr.ReadLine());
                        ///для чтения дней
                        TempDays = new bool[7];
                        for (int i = 0; i < 7; i++) TempDays[i] = Convert.ToBoolean(sr.ReadLine());
                        int count = 0;
                        for (int i = 0; i < 7; i++)
                        {
                            if (TempDays[i]) count++;
                        }
                        NewVar.DaysToTrade = new int[count];
                        count = 0;
                        for (int i = 0; i < 7; i++)
                        {
                            if (TempDays[i])
                            {
                                NewVar.DaysToTrade[count] = i;
                                count++;
                            }
                        }
                        ///конец чтения дней
                        NewVar.HourStart = Convert.ToInt32(sr.ReadLine());
                        NewVar.HourEnd = Convert.ToInt32(sr.ReadLine());
                        NewVar.MinuteStart = Convert.ToInt32(sr.ReadLine());
                        NewVar.MinuteEnd = Convert.ToInt32(sr.ReadLine());
                        //////конец блока временных интервалов и дней
                        NewVar.Method = (EQUATION)Enum.Parse(BruteCode.ReadedVariant.Method.GetType(), sr.ReadLine());
                        NewVar.bSpreadNoize = Convert.ToBoolean(sr.ReadLine());
                        NewVar.OptimizedCloseValue = Convert.ToDouble(sr.ReadLine().Replace('.', ','));
                        NewVar.Ci = new double[NewVar.ANum];
                        for (int i = 0; i < NewVar.ANum; i++)
                        {
                            NewVar.Ci[i] = Convert.ToDouble(sr.ReadLine().Replace('.', ','));
                        }
                        TempVar.Add(NewVar);
                        sr.ReadLine();
                    }
                    if ( BruteCode.BestVariant.Length < TempVar.Count ) BruteCode.BestVariant = new Data[TempVar.Count];
                    for (int i = 0; i < TempVar.Count; i++) BruteCode.BestVariant[i] = TempVar[i];
                }
            }
        }
    }
}
