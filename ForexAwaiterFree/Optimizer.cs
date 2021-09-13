using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;

namespace FB1
{
    internal class Optimizer//класс для оптимизации
    {
        Data[] Variant;//лучший найденный вариант с первой вкладки
        public static Data VariantOptimal;//лучший найденный вариант
        public static Data[] VariantOptimalArray;//массив лучших вариантов
        public static int IterationsAll;//сколько нужно итераций
        public static int IterationsToOne;//сколько нужно итераций для одного варианта ---------------
        static int IterationsToOnePrev = 0;//сколько нужно итераций для одного варианта предыдущее
        static QUALITY_MODE QualityPrev;//предыдущий режим
        public static int variantnum = 0;//какой вариант сейчас оптимизируется +++++++++++++++
        public static int testnum = 0;//какой вариант в варианте сейчас оптимизируется
        public static long ComplededTests = 0;//прогонов завершено ++++++++++++++
        public static double MinutesNeed = 0.0;//сколько минут надо на тест +++++++++++++
        public static double PercentDone = 0.0;//процентов завершено
        public static int SimulatedBars; //-----------
        public uint BarsTotal = 0;//
        public static int DeepBrute; //--------
        public static double RelativeDeviation; //--------относительное отклонение
        static double DeviationPrev;//предыдущее отклонение
        public static bool bSuper; //--------режим отклонения
        public static bool bPercent;//-------режим процента
        public static bool bSpreadControl;//-------учитывать спред из котировки
        public bool bSpreadNoize;//вкл спредовый шум
        public static bool bCloseFree;//-------закрывать несмотря на спред
        public static bool bScalping;//--------скальпирование
        public static bool bCloseInOutOfTime;//-------закрыть позицию если она не во временном корридоре
        public static int SpreadMax;//макс спред
        public static bool bSpreadControlPrev;
        public int SpreadPrev;
        static bool bSuperPrev; //--------режим отклонения предыдущий
        static bool bPercentPrev;//--------режим процента предыдущий
        public static double Percent;//------процент
        public static double DealsMinR; //--------
        static int PointsDealPrev;//
        public static int PointsDeal = 0; //размер прибыли или убытка в пунктах
        public static QUALITY_MODE QualityModeE;

        public DataTester[] CurrentBars;//текущие бары
        public DataTester[] CurrentBarsDestroyedSpread;//текущие бары c уничтоженым влиянием спреда
        long iteration = 0;
        bool bOpenBuy = false;//открыта ли позиция на покупку
        bool bOpenSell = false;//открыта ли позиция на продажу
        bool PrevbOpenBuy = false;//предыдущее
        bool PrevbOpenSell = false;//предыдущее
        int LastOrders;//вспомогательный счетчик

        public static int OrdersMin;//минимум ордеров ------------------------
        public static int VariantsOptimized;//варианты оптимиации ------------------------
        static int OrdersMinPrev = 0;

        static System.Timers.Timer TimerX;//таймер для внутреннего вычисления скорости прогонов
        static Stopwatch TimerY;//таймер для подсчета времени
        static bool bLoaded;//загружены ли данные прерывания прогона
        static int CurrentIterations;//итерации текущего прогона
        static int StartedTests;//запущеные тесты этого прогона

        public static bool bWork = false;//переменная сигнализирующая прерывание или продолжение оптимизации

        void ReadyValues(ref Data Variant, int num)
        {
            Variant.Orders = 0;
            Variant.Order?.Clear();
            Variant.MA0Down = 0.0;
            Variant.MA0Up = 0.0;
            Variant.MA0DownAbs = 0.0;
            Variant.MA0UpAbs = 0.0;
            Variant.NumMinus = 0;
            Variant.NumPlus = 0;
            Variant.MaxProfit = 0.0;
            Variant.MinProfit = 0.0;
            Variant.PointsDown = 0.0;
            Variant.PointsUp = 0.0;
            Variant.OptimizedOpenValue = (num + 1) * (Variant.MaxValueAbs / (double)(IterationsToOne - 1));
            TempBufferBuy = 0.0;
            TempBufferSell = 0.0;
            bOpenBuy = false;
            bOpenSell = false;
            PrevbOpenBuy = bOpenBuy;
            PrevbOpenSell = bOpenSell;
            TempBufferDown = 0.0;
            TempBufferUp = 0.0;
        }

        public async Task<bool> SearchingAsync(int coreindex)//асинхронно запустить вычисления
        {
            return await Task.Run(() => Searching(coreindex));
        }

        bool Searching(int coreindex)//инициировать вычисления
        {
            PrepareVariants();
            PrepareOptimized();
            if (coreindex == 0) TimerX = new System.Timers.Timer(1000);
            if (coreindex == 0) TimerY = new Stopwatch();
            if (coreindex == 0) TimerX.Elapsed += TimerX_Elapsed;
            if (coreindex == 0) TimerY.Start();
            if (coreindex == 0) TimerX.Start();
            bLoaded = false;
            if (coreindex == 0) CurrentIterations = 0;
            if (coreindex == 0) StartedTests = 0;
            int BasicIteration = 0;

            if (ComplededTests < IterationsAll)
            {
                for (int i = 0; i < Variant.Length; i++)
                {
                    Variant[i].index = i;
                }

                for (int i = 0; i < Variant.Length; i++)
                {
                    LastOrders = OrdersMin;
                    for (int j = 0; j < IterationsToOne; j++)
                    {
                        if (!bLoaded && (testnum != 0 || variantnum != 0))
                        {
                            i = variantnum;
                            j = testnum;
                            bLoaded = true;
                        }

                        if (bWork)
                        {
                            if (BasicIteration >= StartedTests && ComplededTests < IterationsAll)
                            {
                                StartedTests++;
                                if (Variant[i].Orders != 0) LastOrders = Variant[i].Orders;
                                if (LastOrders >= OrdersMin)
                                {
                                    Work(ref Variant[i], j, bSuper);
                                }
                                ComplededTests++;
                                CurrentIterations++;
                            }
                            if (ComplededTests >= IterationsAll)
                            {
                                goto End;
                            }
                        }
                        else
                        {
                            variantnum = i;
                            testnum = j;
                            goto End;
                        }
                        BasicIteration++;
                    }
                }
            }
        End:
            TimerX?.Stop();
            TimerY?.Stop();
            MinutesNeed = 0.0;
            PercentDone = (ComplededTests / (double)IterationsAll) * 100.0;
            IterationsToOnePrev = IterationsToOne;
            OrdersMinPrev = OrdersMin;
            QualityPrev = QualityModeE;
            bSuperPrev = bSuper;
            bPercentPrev = bPercent;
            DeviationPrev = RelativeDeviation;
            PointsDealPrev = PointsDeal;
            bSpreadControlPrev = bSpreadControl;
            SpreadPrev = SpreadMax;
            if (coreindex >= 0) Synchronizer.bDone[coreindex] = true;
            return true;
        }

        private static void TimerX_Elapsed(object sender, System.Timers.ElapsedEventArgs e)//подсчет оставшегося времени и прогресса
        {
            if (CurrentIterations > 0) MinutesNeed = (TimerY.Elapsed.TotalHours / CurrentIterations) * 60.0 * (IterationsAll - ComplededTests);
            PercentDone = (ComplededTests / (double)IterationsAll) * 100.0;
        }



        bool BuildCurrentBars(ref Data Variant)//перестроить ссылки на текущие бары
        {
            iteration++;
            if (DataReader.Data.Length - 1 - CurrentBars.Length - (iteration - 1) >= 0)
            {
                if (Variant.bInitTimeBorders) bInZone = IsTimeWindow(ref Variant);
                else bInZone = true;
                for (int i = 0; i < CurrentBars.Length; i++)
                {
                    CurrentBars[i] = DataReader.Data[DataReader.Data.Length - 1 - CurrentBars.Length - (iteration - 1) + i];
                }

                if (Variant.bSpreadNoize || bSpreadNoize)
                {
                    for (int i = 0; i < CurrentBarsDestroyedSpread.Length; i++)
                    {
                        CurrentBarsDestroyedSpread[i] = DataReader.SpeadDestroyedData[DataReader.SpeadDestroyedData.Length - 1 - CurrentBarsDestroyedSpread.Length - (iteration - 1) + i];
                    }
                    return true;
                }
                return true;
            }
            else return false;
        }


        bool bInZone;
        void Work(ref Data Variant, int NumValue, bool bSuper)//метод для запуска нового прогона
        {
            Variant.TotalProfit = 0.0;//обнулим счетчик профита
            Variant.KProfit = 0.0;//обнулим коеффициент профита
            PreBuildNewVariant();
            ReadyValues(ref Variant, NumValue);//подготовим значения***
            ReadyStartBars(ref Variant);
            NewBar(ref Variant);

            while (BuildCurrentBars(ref Variant))
            {
                NewBar(ref Variant);
            }
            FinalClosing(ref Variant);
            Variant.NumBars = BarsTotal;
            //MessageBox.Show(Variant.MA0Down.ToString());
            if (!bSuper) FinalizeThisVariant(ref Variant);

            if (bSuper) //второй цикл если нужны прямые графики
            {
                Variant.KProfit = Variant.TotalProfit / Variant.Orders;
                Variant.ProfitDelta = Math.Abs(Variant.TotalProfit * RelativeDeviation);
                Variant.TotalProfit = 0.0;//обнулим счетчик профита
                Variant.CurrentDeviation = 0.0;
                PreBuildNewVariant();
                ReadyValues(ref Variant, NumValue);//подготовим значения***
                ReadyStartBars(ref Variant);
                NewBar(ref Variant);
                bool bBreaked = false;
                while (BuildCurrentBars(ref Variant))
                {
                    NewBar(ref Variant);
                    if (Variant.CurrentDeviation > Variant.ProfitDelta) //если текущее отклонение больше минимального
                    {
                        bBreaked = true;
                    }
                }

                if (!bBreaked) FinalizeThisVariant(ref Variant);
            }
        }

        void FinalizeThisVariant(ref Data Variant)//метод в котором проводим подводим итоги прогона
        {
            CalcQuality(ref Variant);
            CheckBest(ref Variant);
        }

        public void PrepareVariants()//подготовим массивы
        {
            Variant = new Data[BruteCode.BestVariant.Length];
            for (int i = 0; i < Variant.Length; i++)
            {
                if (BruteCode.BestVariant[i].Ci != null)
                {
                    Variant[i] = new Data();
                    Variant[i].CopyC(BruteCode.BestVariant[i]);
                    Variant[i].CopyT(BruteCode.BestVariant[i]);
                    Variant[i].Order = new List<Awaiter.Order>();//скопируем ордера
                }
            }
        }

        public void PrepareOptimized()//подготовим массивы
        {
            if (VariantOptimalArray == null || VariantOptimalArray.Length != VariantsOptimized || ComplededTests == IterationsAll || (IterationsToOnePrev > 0 && IterationsToOnePrev != IterationsToOne) || (OrdersMinPrev > 0 && OrdersMinPrev != OrdersMin) || (QualityPrev != QualityModeE && OrdersMinPrev > 0) || (bSuperPrev != bSuper) || (bPercentPrev != bPercent) || (DeviationPrev != RelativeDeviation) || (bSpreadControlPrev != bSpreadControl) || (SpreadPrev != SpreadMax))
            {
                ComplededTests = 0;
                variantnum = 0;
                testnum = 0;
                VariantOptimal = new Data();
                VariantOptimalArray = new Data[VariantsOptimized];
                for (int i = 0; i < VariantOptimalArray.Length; i++)
                {
                    VariantOptimalArray[i] = new Data();
                }
            }
        }

        public static void Refresh()//сброс
        {
            ComplededTests = 0;
            variantnum = 0;
            testnum = 0;
            VariantOptimal = new Data();
            VariantOptimalArray = new Data[VariantsOptimized];
            for (int i = 0; i < VariantOptimalArray.Length; i++)
            {
                VariantOptimalArray[i] = new Data();
            }
        }

        void CheckBest(ref Data Variant)//проверям вариант на наличие лучшего значения
        {
            if (Math.Abs(Variant.Quality) > Math.Abs(VariantOptimalArray[VariantOptimalArray.Length - 1].Quality) && (!bSpreadControl || (bSpreadControl && Variant.Quality > 0.0)) && (double)(EndTimeX - LastOpenTime) / (double)(EndTimeX-StartTimeX ) <= 0.03 )
            {

                bool bSameIndex = false;
                for (int i = 0; i < VariantOptimalArray.Length; i++)
                {
                    if (VariantOptimalArray[i].index == Variant.index && VariantOptimalArray[i].Quality != 0.0) bSameIndex = true;

                    if (VariantOptimalArray[i].index == Variant.index && Math.Abs(VariantOptimalArray[i].Quality) < Math.Abs(Variant.Quality) && VariantOptimalArray[i].Quality != 0.0)
                    {
                        if (Variant.Quality != 0.0 && (Math.Abs(Variant.Quality) > Math.Abs(VariantOptimalArray[i].Quality) && VariantOptimalArray[i].Quality != 0.0) && Variant.Orders > OrdersMin && Variant.NumPlus > 0 && Variant.NumMinus > 0 && 2.0 * Math.Abs((double)Variant.NumMinus / (double)(Variant.NumMinus + Variant.NumPlus)) > 0 && 2.0 * Math.Abs((double)Variant.NumPlus / (double)(Variant.NumMinus + Variant.NumPlus)) > 0)
                        {

                            for (int j = i; j < VariantOptimalArray.Length - 1; j++)
                            {
                                VariantOptimalArray[j] = VariantOptimalArray[j + 1];
                            }
                            VariantOptimalArray[VariantOptimalArray.Length - 1].Quality = 0.0;

                            for (int k = 0; k < VariantOptimalArray.Length - 1; k++)
                            {
                                if (Variant.Quality != 0.0 && (Math.Abs(Variant.Quality) > Math.Abs(VariantOptimalArray[k].Quality) || VariantOptimalArray[k].Quality == 0.0) && Variant.Orders > OrdersMin && Variant.NumPlus > 0 && Variant.NumMinus > 0 && 2.0 * Math.Abs((double)Variant.NumMinus / (double)(Variant.NumMinus + Variant.NumPlus)) > 0 && 2.0 * Math.Abs((double)Variant.NumPlus / (double)(Variant.NumMinus + Variant.NumPlus)) > 0)
                                {
                                    for (int j = VariantOptimalArray.Length - 1; j > k; j--)
                                    {
                                        VariantOptimalArray[j] = VariantOptimalArray[j - 1];
                                    }
                                    VariantOptimalArray[k].CopyDataOptimized(Variant);
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    if (VariantOptimalArray[i].index == Variant.index && Math.Abs(VariantOptimalArray[i].Quality) >= Math.Abs(Variant.Quality) && VariantOptimalArray[i].Quality != 0.0)
                    {
                        break;
                    }
                }

                if (!bSameIndex)
                {
                    for (int i = 0; i < VariantOptimalArray.Length; i++)
                    {
                        if (Variant.Quality != 0.0 && (Math.Abs(Variant.Quality) > Math.Abs(VariantOptimalArray[i].Quality) || VariantOptimalArray[i].Quality == 0.0) && Variant.Orders > OrdersMin && Variant.NumPlus > 0 && Variant.NumMinus > 0 && 2.0 * Math.Abs((double)Variant.NumMinus / (double)(Variant.NumMinus + Variant.NumPlus)) > 0 && 2.0 * Math.Abs((double)Variant.NumPlus / (double)(Variant.NumMinus + Variant.NumPlus)) > 0)
                        {
                            for (int j = VariantOptimalArray.Length - 1; j > i; j--)
                            {
                                VariantOptimalArray[j] = VariantOptimalArray[j - 1];
                            }
                            VariantOptimalArray[i].CopyDataOptimized(Variant);
                            break;
                        }
                    }
                }

                if (Math.Abs(Variant.Quality) > Math.Abs(VariantOptimal.Quality) && Variant.Orders > OrdersMin && Variant.NumPlus > 0 && Variant.NumMinus > 0 && 2.0 * Math.Abs((double)Variant.NumMinus / (double)(Variant.NumMinus + Variant.NumPlus)) >= DealsMinR && 2.0 * Math.Abs((double)Variant.NumPlus / (double)(Variant.NumMinus + Variant.NumPlus)) >= DealsMinR)
                {
                    VariantOptimal = Variant;
                }
            }
        }

        void CalcQuality(ref Data Variant)//посчитаем качество
        {
            if (!bPercent)
            {
                if (QualityModeE == QUALITY_MODE.MATH_WAITING && Variant.Orders > 0) Variant.Quality = ((Variant.MA0Up + Variant.MA0Down) / DataReader.Point) / (double)Variant.Orders;
                if (QualityModeE == QUALITY_MODE.P_FACTOR && Variant.Orders > 0 && (Variant.MA0UpAbs + Variant.MA0DownAbs) > 0.0) Variant.Quality = (Variant.MA0Up + Variant.MA0Down) / (Variant.MA0UpAbs + Variant.MA0DownAbs);
            }
            else
            {
                int SumIn = 0;
                int SumOut = 0;
                for (int i = 0; i < Variant.Order.Count; i++)
                {
                    if (Variant.Order[i].bInBruteTimeframe)
                    {
                        SumIn++;
                    }
                    else
                    {
                        SumOut++;
                    }
                }

                if ((SumIn > 0 || SumOut > 0) && (SumOut * 100) / (double)(SumIn + SumOut) >= Percent)
                {
                    if (QualityModeE == QUALITY_MODE.MATH_WAITING && Variant.Orders > 0) Variant.Quality = ((Variant.MA0Up + Variant.MA0Down) / DataReader.Point) / (double)Variant.Orders;
                    if (QualityModeE == QUALITY_MODE.P_FACTOR && Variant.Orders > 0 && (Variant.MA0UpAbs + Variant.MA0DownAbs) > 0.0) Variant.Quality = (Variant.MA0Up + Variant.MA0Down) / (Variant.MA0UpAbs + Variant.MA0DownAbs);
                }
                else
                {
                    Variant.Quality = 0.0;
                }
            }
            if (!Variant.bSpreadNoize && Variant.Quality < 0.0) Variant.Quality = 0.0;
            //MessageBox.Show(Variant.MA0Up.ToString());
        }

        private int StartTimeX;//стартовая точка времени
        private int EndTimeX;//конечная точка времени
        private int LastOpenTime;//время последнего открытия
        void ReadyStartBars(ref Data Variant)//стартовое наполнение массива данных
        {
            if (DataReader.Data.Length >= SimulatedBars + 2)
            {
                iteration = 0;
                CurrentBars = new DataTester[SimulatedBars + 2];
                for (int i = CurrentBars.Length - 1; i >= 0; i--)
                {
                    CurrentBars[i] = DataReader.Data[DataReader.Data.Length - 1 - ((CurrentBars.Length - 1) - i)];
                }

                if (Variant.bSpreadNoize || bSpreadNoize)
                {
                    CurrentBarsDestroyedSpread = new DataTester[SimulatedBars + 2];
                    for (int i = CurrentBars.Length - 1; i >= 0; i--)
                    {
                        CurrentBarsDestroyedSpread[i] = DataReader.SpeadDestroyedData[DataReader.SpeadDestroyedData.Length - 1 - ((CurrentBarsDestroyedSpread.Length - 1) - i)];
                    }
                }
                StartTimeX = DataReader.Data[DataReader.Data.Length - 1].TimeOpen;
                EndTimeX = DataReader.Data[0].TimeOpen;
            }
        }

        void NewBar(ref Data Variant)//метод в котором вызывается внешний код
        {
            Iteration(ref Variant);
        }

        void FinalClosing(ref Data Variant)//финальное закрытие всех ордеров
        {
            if (!bOpenBuy && !bOpenSell && TempBufferDown != 0)
            {
                if (Variant.PointsDown <= TempBufferDown) Variant.PointsDown = TempBufferDown;
            }

            if (!bOpenBuy && !bOpenSell && TempBufferUp != 0)
            {
                if (Variant.PointsUp <= TempBufferUp) Variant.PointsUp = TempBufferUp;
            }

            if (bOpenSell)
            {
                bOpenSell = false;
                Variant.MA0Up += TempBufferBuy;
                Variant.MA0UpAbs += Math.Abs(TempBufferBuy);
                Variant.NumMinus++;
                Variant.Orders++;
                Variant.Order.Add(new Awaiter.Order(TempBufferBuy, iteration >= Tester.StartIteration ? true : false));

                Variant.TotalProfit += TempBufferBuy;
                Variant.CurrentDeviation = Math.Abs(Variant.TotalProfit - Variant.KProfit * Variant.Orders);

                if (TempBufferBuy / DataReader.Point >= PointsDeal)
                {
                    if (Variant.PointsDown <= TempBufferDown) Variant.PointsDown = TempBufferDown;
                    TempBufferDown = 0;
                }
                else
                {
                    TempBufferDown -= TempBufferBuy / DataReader.Point;
                    if (Variant.PointsDown <= TempBufferDown) Variant.PointsDown = TempBufferDown;
                }
                if (TempBufferBuy / DataReader.Point <= -PointsDeal)
                {
                    if (Variant.PointsUp <= TempBufferUp) Variant.PointsUp = TempBufferUp;
                    TempBufferUp = 0;
                }
                else
                {
                    TempBufferUp += TempBufferBuy / DataReader.Point;
                    if (Variant.PointsUp <= TempBufferUp) Variant.PointsUp = TempBufferUp;
                }
            }
            if (bOpenBuy)
            {
                bOpenBuy = false;
                Variant.MA0Down += TempBufferSell;
                Variant.MA0DownAbs += Math.Abs(TempBufferSell);
                Variant.NumPlus++;
                Variant.Orders++;
                Variant.Order.Add(new Awaiter.Order(TempBufferSell, iteration >= Tester.StartIteration ? true : false));

                Variant.TotalProfit += TempBufferSell;
                Variant.CurrentDeviation = Math.Abs(Variant.TotalProfit - Variant.KProfit * Variant.Orders);

                if (TempBufferSell / DataReader.Point >= PointsDeal)
                {
                    if (Variant.PointsDown <= TempBufferDown) Variant.PointsDown = TempBufferDown;
                    TempBufferDown = 0;
                }
                else
                {
                    TempBufferDown -= TempBufferSell / DataReader.Point;
                    if (Variant.PointsDown <= TempBufferDown) Variant.PointsDown = TempBufferDown;
                }
                if (TempBufferSell / DataReader.Point <= -PointsDeal)
                {
                    if (Variant.PointsUp <= TempBufferUp) Variant.PointsUp = TempBufferUp;
                    TempBufferUp = 0;
                }
                else
                {
                    TempBufferUp += TempBufferSell / DataReader.Point;
                    if (Variant.PointsUp <= TempBufferUp) Variant.PointsUp = TempBufferUp;
                }
            }

        }

        //////////////
        ///
        public void PreBuildNewVariant()//предварительная подготовка нового варианта
        {
            BarsTotal = 0;
        }

        ///
        double ValA;
        int iterator;
        double PolinomValue(ref Data Variant)
        {
            ValA = 0;
            iterator = 0;
            if (!Variant.bSpreadNoize)
            {
                if (BruteCode.Method == EQUATION.TEYLOR)
                {
                    if (Variant.DeepBruteX <= 1)
                    {
                        for (int i = 0; i < Variant.CNum; i++)
                        {
                            ValA += Variant.Ci[iterator] * (CurrentBars[i + 2].PriceClose - CurrentBars[i + 2].PriceOpen) / DataReader.Point;
                            iterator++;
                        }

                        for (int i = 0; i < Variant.CNum; i++)
                        {
                            ValA += Variant.Ci[iterator] * (CurrentBars[i + 2].PriceHigh - CurrentBars[i + 2].PriceOpen) / DataReader.Point;
                            iterator++;
                        }

                        for (int i = 0; i < Variant.CNum; i++)
                        {
                            ValA += Variant.Ci[iterator] * (CurrentBars[i + 2].PriceOpen - CurrentBars[i + 2].PriceLow) / DataReader.Point;
                            iterator++;
                        }

                        for (int i = 0; i < Variant.CNum; i++)
                        {
                            ValA += Variant.Ci[iterator] * (CurrentBars[i + 2].PriceHigh - CurrentBars[i + 2].PriceClose) / DataReader.Point;
                            iterator++;
                        }

                        for (int i = 0; i < Variant.CNum; i++)
                        {
                            ValA += Variant.Ci[iterator] * (CurrentBars[i + 2].PriceClose - CurrentBars[i + 2].PriceLow) / DataReader.Point;
                            iterator++;
                        }

                        return ValA;
                    }
                    else
                    {
                        CalcDeep0(ref Variant, ref Variant.Ci, Variant.CNum, Variant.DeepBruteX);
                        return ValStart0;
                    }
                }
                else if (BruteCode.Method == EQUATION.FOURIER)
                {
                    for (int i = 0; i < Variant.CNum; i++)
                    {
                        ValA += Variant.Ci[iterator] * Math.Sin(Variant.Ci[iterator + 1] * (CurrentBars[i + 2].PriceClose - CurrentBars[i + 2].PriceOpen) / DataReader.Point)
                            + Variant.Ci[iterator + 2] * Math.Cos(Variant.Ci[iterator + 3] * (CurrentBars[i + 2].PriceClose - CurrentBars[i + 2].PriceOpen) / DataReader.Point);
                        iterator = +4;
                    }

                    for (int i = 0; i < Variant.CNum; i++)
                    {
                        ValA += Variant.Ci[iterator] * Math.Sin((Variant.Ci[iterator + 1] * CurrentBars[i + 2].PriceHigh - CurrentBars[i + 2].PriceOpen) / DataReader.Point)
                            + Variant.Ci[iterator + 2] * Math.Cos((Variant.Ci[iterator + 3] * CurrentBars[i + 2].PriceHigh - CurrentBars[i + 2].PriceOpen) / DataReader.Point);
                        iterator = +4;
                    }

                    for (int i = 0; i < Variant.CNum; i++)
                    {
                        ValA += Variant.Ci[iterator] * Math.Sin(Variant.Ci[iterator + 1] * (CurrentBars[i + 2].PriceOpen - CurrentBars[i + 2].PriceLow) / DataReader.Point)
                            + Variant.Ci[iterator + 2] * Math.Cos(Variant.Ci[iterator + 3] * (CurrentBars[i + 2].PriceOpen - CurrentBars[i + 2].PriceLow) / DataReader.Point);
                        iterator = +4;
                    }

                    for (int i = 0; i < Variant.CNum; i++)
                    {
                        ValA += Variant.Ci[iterator] * Math.Sin(Variant.Ci[iterator + 1] * (CurrentBars[i + 2].PriceHigh - CurrentBars[i + 2].PriceClose) / DataReader.Point)
                            + Variant.Ci[iterator + 2] * Math.Cos(Variant.Ci[iterator + 3] * (CurrentBars[i + 2].PriceHigh - CurrentBars[i + 2].PriceClose) / DataReader.Point);
                        iterator = +4;
                    }

                    for (int i = 0; i < Variant.CNum; i++)
                    {
                        ValA += Variant.Ci[iterator] * Math.Sin(Variant.Ci[iterator + 1] * (CurrentBars[i + 2].PriceClose - CurrentBars[i + 2].PriceLow) / DataReader.Point)
                            + Variant.Ci[iterator + 2] * Math.Cos(Variant.Ci[iterator + 3] * (CurrentBars[i + 2].PriceClose - CurrentBars[i + 2].PriceLow) / DataReader.Point);
                        iterator = +4;
                    }

                    return ValA;
                }
            }
            else
            {
                if (BruteCode.Method == EQUATION.TEYLOR)
                {
                    if (Variant.DeepBruteX <= 1)
                    {
                        for (int i = 0; i < Variant.CNum; i++)
                        {
                            ValA += Variant.Ci[iterator] * (CurrentBarsDestroyedSpread[i + 2].PriceClose - CurrentBarsDestroyedSpread[i + 2].PriceOpen) / DataReader.Point;
                            iterator++;
                        }

                        for (int i = 0; i < Variant.CNum; i++)
                        {
                            ValA += Variant.Ci[iterator] * (CurrentBarsDestroyedSpread[i + 2].PriceHigh - CurrentBarsDestroyedSpread[i + 2].PriceOpen) / DataReader.Point;
                            iterator++;
                        }

                        for (int i = 0; i < Variant.CNum; i++)
                        {
                            ValA += Variant.Ci[iterator] * (CurrentBarsDestroyedSpread[i + 2].PriceOpen - CurrentBarsDestroyedSpread[i + 2].PriceLow) / DataReader.Point;
                            iterator++;
                        }

                        for (int i = 0; i < Variant.CNum; i++)
                        {
                            ValA += Variant.Ci[iterator] * (CurrentBarsDestroyedSpread[i + 2].PriceHigh - CurrentBarsDestroyedSpread[i + 2].PriceClose) / DataReader.Point;
                            iterator++;
                        }

                        for (int i = 0; i < Variant.CNum; i++)
                        {
                            ValA += Variant.Ci[iterator] * (CurrentBarsDestroyedSpread[i + 2].PriceClose - CurrentBarsDestroyedSpread[i + 2].PriceLow) / DataReader.Point;
                            iterator++;
                        }

                        return ValA;
                    }
                    else
                    {
                        CalcDeep0(ref Variant, ref Variant.Ci, Variant.CNum, Variant.DeepBruteX);
                        return ValStart0;
                    }
                }
                else if (BruteCode.Method == EQUATION.FOURIER)
                {
                    for (int i = 0; i < Variant.CNum; i++)
                    {
                        ValA += Variant.Ci[iterator] * Math.Sin(Variant.Ci[iterator + 1] * (CurrentBarsDestroyedSpread[i + 2].PriceClose - CurrentBarsDestroyedSpread[i + 2].PriceOpen) / DataReader.Point)
                            + Variant.Ci[iterator + 2] * Math.Cos(Variant.Ci[iterator + 3] * (CurrentBarsDestroyedSpread[i + 2].PriceClose - CurrentBarsDestroyedSpread[i + 2].PriceOpen) / DataReader.Point);
                        iterator = +4;
                    }

                    for (int i = 0; i < Variant.CNum; i++)
                    {
                        ValA += Variant.Ci[iterator] * Math.Sin((Variant.Ci[iterator + 1] * CurrentBarsDestroyedSpread[i + 2].PriceHigh - CurrentBarsDestroyedSpread[i + 2].PriceOpen) / DataReader.Point)
                            + Variant.Ci[iterator + 2] * Math.Cos((Variant.Ci[iterator + 3] * CurrentBarsDestroyedSpread[i + 2].PriceHigh - CurrentBarsDestroyedSpread[i + 2].PriceOpen) / DataReader.Point);
                        iterator = +4;
                    }

                    for (int i = 0; i < Variant.CNum; i++)
                    {
                        ValA += Variant.Ci[iterator] * Math.Sin(Variant.Ci[iterator + 1] * (CurrentBarsDestroyedSpread[i + 2].PriceOpen - CurrentBarsDestroyedSpread[i + 2].PriceLow) / DataReader.Point)
                            + Variant.Ci[iterator + 2] * Math.Cos(Variant.Ci[iterator + 3] * (CurrentBarsDestroyedSpread[i + 2].PriceOpen - CurrentBarsDestroyedSpread[i + 2].PriceLow) / DataReader.Point);
                        iterator = +4;
                    }

                    for (int i = 0; i < Variant.CNum; i++)
                    {
                        ValA += Variant.Ci[iterator] * Math.Sin(Variant.Ci[iterator + 1] * (CurrentBarsDestroyedSpread[i + 2].PriceHigh - CurrentBarsDestroyedSpread[i + 2].PriceClose) / DataReader.Point)
                            + Variant.Ci[iterator + 2] * Math.Cos(Variant.Ci[iterator + 3] * (CurrentBarsDestroyedSpread[i + 2].PriceHigh - CurrentBarsDestroyedSpread[i + 2].PriceClose) / DataReader.Point);
                        iterator = +4;
                    }

                    for (int i = 0; i < Variant.CNum; i++)
                    {
                        ValA += Variant.Ci[iterator] * Math.Sin(Variant.Ci[iterator + 1] * (CurrentBarsDestroyedSpread[i + 2].PriceClose - CurrentBarsDestroyedSpread[i + 2].PriceLow) / DataReader.Point)
                            + Variant.Ci[iterator + 2] * Math.Cos(Variant.Ci[iterator + 3] * (CurrentBarsDestroyedSpread[i + 2].PriceClose - CurrentBarsDestroyedSpread[i + 2].PriceLow) / DataReader.Point);
                        iterator = +4;
                    }

                    return ValA;
                }
            }

            return 0.0;

        }
        ///

        ///фрактальный подсчет чисел
        double ValW0;//число куда умножаем все(а потом его прибавляем к ValStart)
        uint NumC0;//текущее число для коэффициента
        double ValStart0;//число куда суммируем все
        void Deep0(ref Data Variant, ref double[] Ci0, int Nums, int deepC, int deepStart, double Val0 = 1.0)//промежуточный фрактал
        {
            if (!Variant.bSpreadNoize)
            {
                for (int i = 0; i < Nums; i++)
                {
                    if (deepC > 1)
                    {
                        ValW0 = (CurrentBars[i + 2].PriceClose - CurrentBars[i + 2].PriceOpen) * Val0;
                        Deep0(ref Variant, ref Ci0, Nums, deepC - 1, deepStart, ValW0);
                    }
                    else
                    {
                        ValStart0 += (Ci0[NumC0]) * (CurrentBars[i + 2].PriceClose - CurrentBars[i + 2].PriceOpen) * Val0 / DataReader.Point;
                        NumC0++;
                    }
                }

                for (int i = 0; i < Nums; i++)
                {
                    if (deepC > 1)
                    {
                        ValW0 = (CurrentBars[i + 2].PriceHigh - CurrentBars[i + 2].PriceOpen) * Val0;
                        Deep0(ref Variant, ref Ci0, Nums, deepC - 1, deepStart, ValW0);
                    }
                    else
                    {
                        ValStart0 += (Ci0[NumC0]) * (CurrentBars[i + 2].PriceHigh - CurrentBars[i + 2].PriceOpen) * Val0 / DataReader.Point;
                        NumC0++;
                    }
                }

                for (int i = 0; i < Nums; i++)
                {
                    if (deepC > 1)
                    {
                        ValW0 = (CurrentBars[i + 2].PriceOpen - CurrentBars[i + 2].PriceLow) * Val0;
                        Deep0(ref Variant, ref Ci0, Nums, deepC - 1, deepStart, ValW0);
                    }
                    else
                    {
                        ValStart0 += (Ci0[NumC0]) * (CurrentBars[i + 2].PriceOpen - CurrentBars[i + 2].PriceLow) * Val0 / DataReader.Point;
                        NumC0++;
                    }
                }

                for (int i = 0; i < Nums; i++)
                {
                    if (deepC > 1)
                    {
                        ValW0 = (CurrentBars[i + 2].PriceHigh - CurrentBars[i + 2].PriceClose) * Val0;
                        Deep0(ref Variant, ref Ci0, Nums, deepC - 1, deepStart, ValW0);
                    }
                    else
                    {
                        ValStart0 += (Ci0[NumC0]) * (CurrentBars[i + 2].PriceHigh - CurrentBars[i + 2].PriceClose) * Val0 / DataReader.Point;
                        NumC0++;
                    }
                }

                for (int i = 0; i < Nums; i++)
                {
                    if (deepC > 1)
                    {
                        ValW0 = (CurrentBars[i + 2].PriceClose - CurrentBars[i + 2].PriceLow) * Val0;
                        Deep0(ref Variant, ref Ci0, Nums, deepC - 1, deepStart, ValW0);
                    }
                    else
                    {
                        ValStart0 += (Ci0[NumC0]) * (CurrentBars[i + 2].PriceClose - CurrentBars[i + 2].PriceLow) * Val0 / DataReader.Point;
                        NumC0++;
                    }
                }
            }
            else
            {
                for (int i = 0; i < Nums; i++)
                {
                    if (deepC > 1)
                    {
                        ValW0 = (CurrentBarsDestroyedSpread[i + 2].PriceClose - CurrentBarsDestroyedSpread[i + 2].PriceOpen) * Val0;
                        Deep0(ref Variant, ref Ci0, Nums, deepC - 1, deepStart, ValW0);
                    }
                    else
                    {
                        ValStart0 += (Ci0[NumC0]) * (CurrentBarsDestroyedSpread[i + 2].PriceClose - CurrentBarsDestroyedSpread[i + 2].PriceOpen) * Val0 / DataReader.Point;
                        NumC0++;
                    }
                }

                for (int i = 0; i < Nums; i++)
                {
                    if (deepC > 1)
                    {
                        ValW0 = (CurrentBarsDestroyedSpread[i + 2].PriceHigh - CurrentBarsDestroyedSpread[i + 2].PriceOpen) * Val0;
                        Deep0(ref Variant, ref Ci0, Nums, deepC - 1, deepStart, ValW0);
                    }
                    else
                    {
                        ValStart0 += (Ci0[NumC0]) * (CurrentBarsDestroyedSpread[i + 2].PriceHigh - CurrentBarsDestroyedSpread[i + 2].PriceOpen) * Val0 / DataReader.Point;
                        NumC0++;
                    }
                }

                for (int i = 0; i < Nums; i++)
                {
                    if (deepC > 1)
                    {
                        ValW0 = (CurrentBarsDestroyedSpread[i + 2].PriceOpen - CurrentBarsDestroyedSpread[i + 2].PriceLow) * Val0;
                        Deep0(ref Variant, ref Ci0, Nums, deepC - 1, deepStart, ValW0);
                    }
                    else
                    {
                        ValStart0 += (Ci0[NumC0]) * (CurrentBarsDestroyedSpread[i + 2].PriceOpen - CurrentBarsDestroyedSpread[i + 2].PriceLow) * Val0 / DataReader.Point;
                        NumC0++;
                    }
                }

                for (int i = 0; i < Nums; i++)
                {
                    if (deepC > 1)
                    {
                        ValW0 = (CurrentBarsDestroyedSpread[i + 2].PriceHigh - CurrentBarsDestroyedSpread[i + 2].PriceClose) * Val0;
                        Deep0(ref Variant, ref Ci0, Nums, deepC - 1, deepStart, ValW0);
                    }
                    else
                    {
                        ValStart0 += (Ci0[NumC0]) * (CurrentBarsDestroyedSpread[i + 2].PriceHigh - CurrentBarsDestroyedSpread[i + 2].PriceClose) * Val0 / DataReader.Point;
                        NumC0++;
                    }
                }

                for (int i = 0; i < Nums; i++)
                {
                    if (deepC > 1)
                    {
                        ValW0 = (CurrentBarsDestroyedSpread[i + 2].PriceClose - CurrentBarsDestroyedSpread[i + 2].PriceLow) * Val0;
                        Deep0(ref Variant, ref Ci0, Nums, deepC - 1, deepStart, ValW0);
                    }
                    else
                    {
                        ValStart0 += (Ci0[NumC0]) * (CurrentBarsDestroyedSpread[i + 2].PriceClose - CurrentBarsDestroyedSpread[i + 2].PriceLow) * Val0 / DataReader.Point;
                        NumC0++;
                    }
                }
            }
        }

        void CalcDeep0(ref Data Variant, ref double[] Ci0, int Nums, int deepC = 1)
        {
            NumC0 = 0;
            ValStart0 = 0.0;
            for (int i = 0; i < deepC; i++)
            {
                Deep0(ref Variant, ref Ci0, Nums, i + 1, i + 1);
            }
        }

        private int HourCorrect(int hour0)
        {
            int rez = 0;
            if (hour0 < 24 && hour0 > 0)
            {
                rez = hour0;
            }
            return rez;
        }

        private int MinuteCorrect(int minute0)
        {
            int rez = 0;
            if (minute0 < 60 && minute0 > 0)
            {
                rez = minute0;
            }
            return rez;
        }
        private bool IsTimeWindow(ref Data Variant)//находимся ли мы сейчас в требуемом окне
        {
            DataTester LastBar = DataReader.Data[DataReader.Data.Length - 1 - CurrentBars.Length - (iteration - 2)];//
            bool bDayAccess = false;
            for (int i = 0; i < Variant.DaysToTrade.Length; i++)//проверка дней
            {
                if (LastBar.DayOpen == Variant.DaysToTrade[i]) bDayAccess = true;
            }
            if (!bDayAccess) return false;
            int MinuteEquivalent = LastBar.HourOpen * 60 + LastBar.MinuteOpen;
            int BorderMinuteStartTrade = HourCorrect(Variant.HourStart) * 60 + MinuteCorrect(Variant.MinuteStart);
            int BorderMinuteEndTrade = HourCorrect(Variant.HourEnd) * 60 + MinuteCorrect(Variant.MinuteEnd);
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

        ///////      
        double TempBufferBuy = 0.0;//для суммирования профита сделки
        double TempBufferSell = 0.0;//для суммирования профита сделки
        double TempBufferDown = 0.0;//
        double TempBufferUp = 0.0;//
        public void Iteration(ref Data Variant)//прохождение одного бара вариантом
        {
            double TempV = PolinomValue(ref Variant);
            BarsTotal++;
            //Print(TempV);

            if ( bOpenBuy && ( ( ( (TempV > 0 && bScalping) || (TempV > Variant.OptimizedOpenValue && !bScalping) ) && !bCloseInOutOfTime) || ( bCloseInOutOfTime && !bInZone ) ) )
            {
                if (!bSpreadControl)
                {
                    bOpenSell = false;
                    bOpenBuy = false;
                    Variant.MA0Up += TempBufferBuy;
                    Variant.MA0UpAbs += Math.Abs(TempBufferBuy);
                    Variant.NumMinus++;
                    Variant.Orders++;
                    Variant.Order.Add(new Awaiter.Order(TempBufferBuy, iteration >= Tester.StartIteration ? true : false));

                    Variant.TotalProfit += TempBufferBuy;
                    Variant.CurrentDeviation = Math.Abs(Variant.TotalProfit - Variant.KProfit * Variant.Orders);

                    if (TempBufferBuy / DataReader.Point >= PointsDeal)
                    {
                        if (Variant.PointsDown <= TempBufferDown) Variant.PointsDown = TempBufferDown;
                        TempBufferDown = 0;
                    }
                    else if (TempBufferBuy < 0.0)
                    {
                        TempBufferDown -= TempBufferBuy / DataReader.Point;
                    }
                    if (TempBufferBuy / DataReader.Point <= -PointsDeal)
                    {
                        if (Variant.PointsUp <= TempBufferUp) Variant.PointsUp = TempBufferUp;
                        TempBufferUp = 0;
                    }
                    else if (TempBufferBuy > 0.0)
                    {
                        TempBufferUp += TempBufferBuy / DataReader.Point;
                    }

                    TempBufferSell = 0.0;
                    TempBufferBuy = 0.0;
                    if ((Variant.MA0Up + Variant.MA0Down) > 0.0 && (Variant.MA0Up + Variant.MA0Down) > Variant.MaxProfit) Variant.MaxProfit = Variant.MA0Up + Variant.MA0Down;
                    if ((Variant.MA0Up + Variant.MA0Down) < 0.0 && -(Variant.MA0Up + Variant.MA0Down) > Variant.MinProfit) Variant.MinProfit = -(Variant.MA0Up + Variant.MA0Down);
                }
                else if ( bCloseFree || (!bCloseFree && CurrentBars[1].Spread <= SpreadMax) )
                {
                    bOpenSell = false;
                    bOpenBuy = false;
                    Variant.MA0Up += TempBufferBuy;
                    Variant.MA0UpAbs += Math.Abs(TempBufferBuy);
                    Variant.NumMinus++;
                    Variant.Orders++;
                    Variant.Order.Add(new Awaiter.Order(TempBufferBuy, iteration >= Tester.StartIteration ? true : false));

                    Variant.TotalProfit += TempBufferBuy;
                    Variant.CurrentDeviation = Math.Abs(Variant.TotalProfit - Variant.KProfit * Variant.Orders);

                    if (TempBufferBuy / DataReader.Point >= PointsDeal)
                    {
                        if (Variant.PointsDown <= TempBufferDown) Variant.PointsDown = TempBufferDown;
                        TempBufferDown = 0;
                    }
                    else if (TempBufferBuy < 0.0)
                    {
                        TempBufferDown -= TempBufferBuy / DataReader.Point;
                    }
                    if (TempBufferBuy / DataReader.Point <= -PointsDeal)
                    {
                        if (Variant.PointsUp <= TempBufferUp) Variant.PointsUp = TempBufferUp;
                        TempBufferUp = 0;
                    }
                    else if (TempBufferBuy > 0.0)
                    {
                        TempBufferUp += TempBufferBuy / DataReader.Point;
                    }

                    TempBufferSell = 0.0;
                    TempBufferBuy = 0.0;
                    if ((Variant.MA0Up + Variant.MA0Down) > 0.0 && (Variant.MA0Up + Variant.MA0Down) > Variant.MaxProfit) Variant.MaxProfit = Variant.MA0Up + Variant.MA0Down;
                    if ((Variant.MA0Up + Variant.MA0Down) < 0.0 && -(Variant.MA0Up + Variant.MA0Down) > Variant.MinProfit) Variant.MinProfit = -(Variant.MA0Up + Variant.MA0Down);
                }
                //MessageBox.Show(Variant.MA0Up.ToString());
            }
            if ( bOpenSell && ( ( ( ( TempV < 0 && bScalping ) || (TempV < -Variant.OptimizedOpenValue && !bScalping)) && !bCloseInOutOfTime) || (bCloseInOutOfTime && !bInZone) ) )
            {
                if (!bSpreadControl)
                {
                    bOpenBuy = false;
                    bOpenSell = false;
                    Variant.MA0Down += TempBufferSell;
                    Variant.MA0DownAbs += Math.Abs(TempBufferSell);
                    Variant.NumPlus++;
                    Variant.Orders++;
                    Variant.Order.Add(new Awaiter.Order(TempBufferSell, iteration >= Tester.StartIteration ? true : false));

                    Variant.TotalProfit += TempBufferSell;
                    Variant.CurrentDeviation = Math.Abs(Variant.TotalProfit - Variant.KProfit * Variant.Orders);

                    if (TempBufferSell / DataReader.Point >= PointsDeal)
                    {
                        if (Variant.PointsDown <= TempBufferDown) Variant.PointsDown = TempBufferDown;
                        TempBufferDown = 0;
                    }
                    else if (TempBufferSell < 0.0)
                    {
                        TempBufferDown -= TempBufferSell / DataReader.Point;
                    }
                    if (TempBufferSell / DataReader.Point <= -PointsDeal)
                    {
                        if (Variant.PointsUp <= TempBufferUp) Variant.PointsUp = TempBufferUp;
                        TempBufferUp = 0;
                    }
                    else if (TempBufferSell > 0.0)
                    {
                        TempBufferUp += TempBufferSell / DataReader.Point;
                    }

                    TempBufferSell = 0.0;
                    TempBufferBuy = 0.0;
                    if ((Variant.MA0Up + Variant.MA0Down) > 0.0 && (Variant.MA0Up + Variant.MA0Down) > Variant.MaxProfit) Variant.MaxProfit = Variant.MA0Up + Variant.MA0Down;
                    if ((Variant.MA0Up + Variant.MA0Down) < 0.0 && -(Variant.MA0Up + Variant.MA0Down) > Variant.MinProfit) Variant.MinProfit = -(Variant.MA0Up + Variant.MA0Down);
                }
                else if (bCloseFree || (!bCloseFree && CurrentBars[1].Spread <= SpreadMax))
                {
                    bOpenBuy = false;
                    bOpenSell = false;
                    TempBufferSell -= CurrentBars[1].Spread * DataReader.Point;//на продажу открываем без спреда, а закроем со спредом
                    Variant.MA0Down += TempBufferSell;
                    Variant.MA0DownAbs += Math.Abs(TempBufferSell);
                    Variant.NumPlus++;
                    Variant.Orders++;
                    Variant.Order.Add(new Awaiter.Order(TempBufferSell, iteration >= Tester.StartIteration ? true : false));

                    Variant.TotalProfit += TempBufferSell;
                    Variant.CurrentDeviation = Math.Abs(Variant.TotalProfit - Variant.KProfit * Variant.Orders);

                    if (TempBufferSell / DataReader.Point >= PointsDeal)
                    {
                        if (Variant.PointsDown <= TempBufferDown) Variant.PointsDown = TempBufferDown;
                        TempBufferDown = 0;
                    }
                    else if (TempBufferSell < 0.0)
                    {
                        TempBufferDown -= TempBufferSell / DataReader.Point;
                    }
                    if (TempBufferSell / DataReader.Point <= -PointsDeal)
                    {
                        if (Variant.PointsUp <= TempBufferUp) Variant.PointsUp = TempBufferUp;
                        TempBufferUp = 0;
                    }
                    else if (TempBufferSell > 0.0)
                    {
                        TempBufferUp += TempBufferSell / DataReader.Point;
                    }

                    TempBufferSell = 0.0;
                    TempBufferBuy = 0.0;
                    if ((Variant.MA0Up + Variant.MA0Down) > 0.0 && (Variant.MA0Up + Variant.MA0Down) > Variant.MaxProfit) Variant.MaxProfit = Variant.MA0Up + Variant.MA0Down;
                    if ((Variant.MA0Up + Variant.MA0Down) < 0.0 && -(Variant.MA0Up + Variant.MA0Down) > Variant.MinProfit) Variant.MinProfit = -(Variant.MA0Up + Variant.MA0Down);
                }
                //MessageBox.Show(Variant.MA0Down.ToString());
            }


            if ( bInZone && TempV < -Variant.OptimizedOpenValue )
            {
                if ( !bSpreadControl )
                {
                    bOpenBuy = true;
                }
                else if( CurrentBars[1].Spread <= SpreadMax && !bOpenBuy )
                {
                    bOpenBuy = true;
                    TempBufferBuy = -CurrentBars[1].Spread * DataReader.Point;//сразу задаем убыток в виде спреда, а закроем без спреда
                }
                    
            }
            if ( bInZone && TempV > Variant.OptimizedOpenValue)
            {
                if (!bSpreadControl)
                {
                    bOpenSell = true;
                    LastOpenTime = CurrentBars[0].TimeOpen;
                }
                else if (CurrentBars[1].Spread <= SpreadMax && !bOpenSell)
                {
                    bOpenSell = true;
                    LastOpenTime = CurrentBars[0].TimeOpen;
                }
            }

            if (bOpenBuy == true)
            {
                if ( !bSpreadNoize ) TempBufferBuy += (CurrentBars[1].PriceClose - CurrentBars[2].PriceClose);
                else TempBufferBuy += (CurrentBarsDestroyedSpread[1].PriceClose - CurrentBarsDestroyedSpread[2].PriceClose);
            }
            if (bOpenSell == true)
            {
                if ( !bSpreadNoize ) TempBufferSell += (CurrentBars[2].PriceClose - CurrentBars[1].PriceClose);
                else TempBufferSell += (CurrentBarsDestroyedSpread[2].PriceClose - CurrentBarsDestroyedSpread[1].PriceClose);
            }

            PrevbOpenBuy = bOpenBuy;
            PrevbOpenSell = bOpenSell;
        }
    }
}
