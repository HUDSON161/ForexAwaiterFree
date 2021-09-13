using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.ComponentModel;

namespace FB1
{
    static class Synchronizer//для управления потоками
    {
        public static bool[] bDone= new bool[1];
        static bool bDoneAll = false;
        public static async Task<bool> AsyncSearchMultiCore(int cores, Tester[] Testers)
        {
            return await Task.Run(() => SyncSearchMultiCore(cores, Testers));
        }

        public static async Task<bool> AsyncOptimizeMultiCore(int cores, Optimizer[] Optimizers)
        {
            return await Task.Run(() => SyncOptimizeMultiCore(cores, Optimizers));
        }

        private static bool SyncSearchMultiCore(int cores,Tester[] Testers)
        {
            bDoneAll = false;
            bDone = new bool[cores];
            //Workers = new BackgroundWorker[cores];
            for (int i = 0; i < cores; i++) bDone[i]=false;
            for (int i = 0; i < cores; i++) 
            {
                _ = Testers[i].SearchingAsync(i, Testers[i]);
            }
            while (!bDoneAll)
            {
                Thread.Sleep(1000);
                bDoneAll = true;
                for (int i = 0; i < cores; i++)
                {
                    if (bDone[i] == false) 
                    {
                        bDoneAll = false;
                        break;
                    }
                }
            }
            return true;
        }

        private static bool SyncOptimizeMultiCore(int cores, Optimizer[] Optimizers)
        {
            bDoneAll = false;
            bDone = new bool[cores];
            //Workers = new BackgroundWorker[cores];
            for (int i = 0; i < cores; i++) bDone[i] = false;
            for (int i = 0; i < cores; i++)
            {
                _ = Optimizers[i].SearchingAsync(i);
            }
            while (!bDoneAll)
            {
                Thread.Sleep(1000);
                bDoneAll = true;
                for (int i = 0; i < cores; i++)
                {
                    if (bDone[i] == false)
                    {
                        bDoneAll = false;
                        break;
                    }
                }
            }
            return true;
        }

        private static void Synchronizer_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)//поток завершил работу
        {
            for (int i = 0; i < bDone.Length; i++)
            {
                bDone[i] = true;
            }
        }
    }
}
