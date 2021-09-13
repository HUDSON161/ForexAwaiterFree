using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace FB1
{
    class Tester: BruteCode
    {
        public Data Variant = new Data();
        public DataTester[] CurrentBars;
        public long iteration = 0;
        public static long ComplededTests = 0;//прогонов завершено
        public static double TestsPerHour = 0.0;//прогонов в час
        public static int SimulatedBars;
        static System.Timers.Timer TimerX;//таймер для внутреннего вычисления скорости прогонов
        static Stopwatch TimerY;//таймер для подсчета времени
        static double PrevTime = 0.0;//вспомогательная переменная
        public uint BarsTotal = 0;//
        public static int StartIteration=0;
        public static bool bWork = false;//переменная сигнализирующая

        public void BuildNewVariant(Tester CoreWorker)//подготовка нового варианта
        {
            BarsTotal = 0;
            ///
            Variant.CNum = Tester.SimulatedBars;
            Variant.ANum = RandomVariant.NumCAll;
            Variant.DeepBruteX = BruteCode.DeepBrute;
            Variant.bInitTimeBorders = BruteCode.bInitTimeBorders;
            Variant.bSpreadNoize = !BruteCode.bSpreadControl;

            Variant.bInvertThis = false;
            Variant.MA0Down = 0.0;
            Variant.MA0DownAbs = 0.0;
            Variant.MA0DownO = 0.0;
            Variant.MA0Up = 0.0;
            Variant.MA0UpAbs = 0.0;
            Variant.MA0UpO = 0.0;
            Variant.MaxProfit = 0.0;
            Variant.MaxValueAbs = 0.0;
            Variant.MinProfit = 0.0;
            Variant.NumBars = 0;
            Variant.NumBarsGood = 0;
            Variant.NumMinus = 0;
            Variant.NumPlus = 0;
            Variant.Quality = 0.0;
            Variant.CurrentDeviation = 0.0;
            Variant.TotalProfit = 0.0;
            Variant.Method = BruteCode.Method;

            CoreWorker.GenerateC(CoreWorker);
            if (CoreWorker.Variant.bInitTimeBorders)
            {
                if (bInitRandomTime) CoreWorker.GenerateRandomInterval(CoreWorker);
                else CoreWorker.GenerateInterval(CoreWorker);
                if (bInitRandomDays) CoreWorker.GenerateRandomDays(CoreWorker);
                else CoreWorker.GenerateDays(CoreWorker);
            }
            else
            {
                CoreWorker.GenerateBasicValues(CoreWorker);
            }
        }

        public void RefreshVariant()//частичный сброс варианта
        {
            BarsTotal = 0;
            Variant.MA0Down = 0.0;
            Variant.MA0DownAbs = 0.0;
            Variant.MA0DownO = 0.0;
            Variant.MA0Up = 0.0;
            Variant.MA0UpAbs = 0.0;
            Variant.MA0UpO = 0.0;
            Variant.MaxProfit = 0.0;
            Variant.MaxValueAbs = 0.0;
            Variant.MinProfit = 0.0;
            Variant.NumBars = 0;
            Variant.NumBarsGood = 0;
            Variant.NumMinus = 0;
            Variant.NumPlus = 0;
            Variant.Quality = 0.0;
            Variant.CurrentDeviation = 0.0;
            Variant.TotalProfit = 0.0;
            Variant.Orders = 0;
        }

        void CalcStartIteration()//посчитаем стартовую итерацию исходя из процента заданного в брут ветке
        {
            if ( BruteCode.PercentBrute != 100 )
            {
                StartIteration= (int)((DataReader.Data.Length - 1 - (SimulatedBars + 2)) * ((100-BruteCode.PercentBrute) / 100.0)) ;
            }
            else
            {
                StartIteration = 0;
            }
        }

        void ReadyStartBars()//стартовое наполнение массива данных
        {
            if (DataReader.Data.Length >= SimulatedBars + 2)
            {
                CalcStartIteration();
                iteration = StartIteration;
                CurrentBars = new DataTester[SimulatedBars + 2];
                if ( bSpreadControl )
                {
                    for (int i = 0; i < CurrentBars.Length; i++)
                    {
                        CurrentBars[i] = DataReader.Data[DataReader.Data.Length - 1 - CurrentBars.Length - (iteration - 1) + i];
                    }
                }
                else
                {
                    for (int i = 0; i < CurrentBars.Length; i++)
                    {
                        CurrentBars[i] = DataReader.SpeadDestroyedData[DataReader.SpeadDestroyedData.Length - 1 - CurrentBars.Length - (iteration - 1) + i];
                    }
                }

            }
        }

        public int HourCorrect(int hour0)
        {
            int rez = 0;
            if (hour0 < 24 && hour0 > 0)
            {
                rez = hour0;
            }
            return rez;
        }

        public int MinuteCorrect(int minute0)
        {
            int rez = 0;
            if (minute0 < 60 && minute0 > 0)
            {
                rez = minute0;
            }
            return rez;
        }
        private bool IsTimeWindow()//находимся ли мы сейчас в требуемом окне
        {
            DataTester LastBar = DataReader.Data[DataReader.Data.Length - 1 - CurrentBars.Length - (iteration - 2)];//
            bool bDayAccess = false;
            for (int i = 0; i < Variant.DaysToTrade.Length; i++)//проверка дней
            {
                if (LastBar.DayOpen == Variant.DaysToTrade[i]) bDayAccess=true;
            }
            if (!bDayAccess) return false;
            int MinuteEquivalent = LastBar.HourOpen * 60 + LastBar.MinuteOpen;
            int BorderMinuteStartTrade = HourCorrect(Variant.HourStart) * 60 + MinuteCorrect(Variant.MinuteStart);
            int BorderMinuteEndTrade = HourCorrect(Variant.HourEnd) * 60 + MinuteCorrect(Variant.MinuteEnd);

            //MessageBox.Show($"{BorderMinuteStartTrade} {BorderMinuteEndTrade} {MinuteEquivalent}");

            if (BorderMinuteStartTrade > BorderMinuteEndTrade)//проверка временного интервала
            {
                if (MinuteEquivalent >= BorderMinuteEndTrade && MinuteEquivalent <= BorderMinuteStartTrade)
                {
                    return false;
                }
            }
            else
            {
                if (!(MinuteEquivalent >= BorderMinuteStartTrade && MinuteEquivalent <= BorderMinuteEndTrade))
                {
                    return false;
                }
            }
            return true;
        }

        bool BuildCurrentBars()//перестроить ссылки на текущие бары
        {
            iteration++;
            if ( bSpreadControl )
            {
                if (DataReader.Data.Length - 1 - CurrentBars.Length - (iteration - 1) >= 0)
                {
                    if (bInitTimeBorders) bInZone = IsTimeWindow();
                    else bInZone = true;
                    if (bInZone)
                    {
                        for (int i = 0; i < CurrentBars.Length; i++)
                        {
                            CurrentBars[i] = DataReader.Data[DataReader.Data.Length - 1 - CurrentBars.Length - (iteration - 1) + i];
                        }
                    }
                    return true;
                }
                else return false;
            }
            else
            {
                if (DataReader.SpeadDestroyedData.Length - 1 - CurrentBars.Length - (iteration - 1) >= 0)
                {
                    if (bInitTimeBorders) bInZone = IsTimeWindow();
                    else bInZone = true;
                    if (bInZone)
                    {
                        for (int i = 0; i < CurrentBars.Length; i++)
                        {
                            CurrentBars[i] = DataReader.SpeadDestroyedData[DataReader.Data.Length - 1 - CurrentBars.Length - (iteration - 1) + i];
                        }
                    }
                    return true;
                }
                else return false;
            }

        }

        bool bInZone;
        void Work(Tester CoreWorker,bool bSuper)//метод для запуска нового прогона
        {
            if ( CoreWorker != null )
            {
                BuildNewVariant(CoreWorker);
                ReadyStartBars();
                //NewBar();

                while (BuildCurrentBars())
                {
                    if (bInZone) NewBar();
                }
                CoreWorker.Variant.NumBars = CoreWorker.BarsTotal;
                if (!bSuper) FinalizeThisVariant();

                if (bSuper) //второй цикл если нужны прямые графики
                {
                    CoreWorker.Variant.KProfit = CoreWorker.Variant.TotalProfit / CoreWorker.Variant.Orders;
                    CoreWorker.Variant.ProfitDelta = Math.Abs(CoreWorker.Variant.TotalProfit * RelativeDeviation);
                    RefreshVariant();
                    ReadyStartBars();
                    NewBar();
                    bool bBreaked = false;
                    while (BuildCurrentBars())
                    {
                        if (bInZone)
                        {
                            NewBar();
                            if (Variant.CurrentDeviation > Variant.ProfitDelta) //если текущее отклонение больше минимального
                            {
                                bBreaked = true;
                            }
                        }

                    }

                    CoreWorker.Variant.Orders = 0;
                    CoreWorker.Variant.TotalProfit = 0.0;
                    if (!bBreaked) FinalizeThisVariant();
                }
            }
        }

        void FinalizeThisVariant()//метод в котором проводим подводим итоги прогона
        {
            CalcQuality();
            CheckBest();
        }

        void CheckBest()//проверям вариант на наличие лучшего значения
        {
            if (Math.Abs(Variant.Quality) > Math.Abs(BruteCode.BestVariant[BruteCode.BestVariant.Length - 1].Quality) && (!bSpreadControl || (bSpreadControl && Variant.Quality > 0.0)) )
            {
                for (int i = 0; i < BruteCode.BestVariant.Length; i++)
                {
                    if (Math.Abs(Variant.Quality) > Math.Abs(BruteCode.BestVariant[i].Quality) && Variant.NumPlus > 0 && Variant.NumMinus > 0 && 2.0 * Math.Abs((double)Variant.NumMinus / (double)(Variant.NumMinus + Variant.NumPlus)) >= BruteCode.DealsMinR && 2.0 * Math.Abs((double)Variant.NumPlus / (double)(Variant.NumMinus + Variant.NumPlus)) >= BruteCode.DealsMinR)
                    {
                        for (int j = BruteCode.BestVariant.Length - 1; j > i; j--)
                        {
                            BruteCode.BestVariant[j] = BruteCode.BestVariant[j - 1];
                        }
                        BruteCode.BestVariant[i].CopyData(Variant);
                        break;
                    }
                }
            }

        }

        void CalcQuality()//посчитаем качество
        {
            if (BruteCode.QualityModeE == QUALITY_MODE.MATH_WAITING) Variant.Quality = ((Variant.MA0Up + Variant.MA0Down) / DataReader.Point) / (double)BarsTotal;
            if (BruteCode.QualityModeE == QUALITY_MODE.P_FACTOR && (Variant.MA0UpAbs + Variant.MA0DownAbs) > 0.0) Variant.Quality = (Variant.MA0Up + Variant.MA0Down) / (Variant.MA0UpAbs + Variant.MA0DownAbs);
        }

        public async Task<bool> SearchingAsync(int coreindex, Tester CoreWorker)//асинхронно запустить вычисления
        {
            return await Task.Run(() => Searching(coreindex, CoreWorker));
        }

        public bool Searching(int coreindex, Tester CoreWorker)//инициировать вычисления
        {
            if (coreindex <= 0)
            {
                TimerX = new System.Timers.Timer(1000);
                TimerY = new Stopwatch();
                PrevTime = 0.0;
                TimerX.Elapsed += Timer_Elapsed;
                TimerY.Start();
                TimerX.Start();
            }
            DataWriter.ReadDataVariants();
            CoreWorker.PreBuildNewVariant(CoreWorker);
            while (bWork)
            {
                Work(CoreWorker,bSuper);
                ComplededTests++;
                //MessageBox.Show(ComplededTests.ToString());
            }
            TimerX.Stop();
            TimerY.Stop();
            TestsPerHour = 0.0;
            ComplededTests = 0;

            if (coreindex >= 0) Synchronizer.bDone[coreindex] = true;
            return true;
        }

        private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)//пересчет всех важных параметров в таймере
        {
            if (PrevTime < TimerY.Elapsed.TotalHours && TimerY.Elapsed.TotalHours > 0.0) TestsPerHour = ComplededTests / TimerY.Elapsed.TotalHours;
            PrevTime = TimerY.Elapsed.TotalHours;
        }

        void NewBar()//метод в котором вызывается внешний код
        {
            Iteration(this);
        }
    }
}
