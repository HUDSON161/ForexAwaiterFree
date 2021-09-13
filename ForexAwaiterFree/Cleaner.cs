using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awaiter
{
    public static class Cleaner
    {
        public static void CleanFiles()//удалим файлы если они есть
        {
            string FileLine0 = FB1.DataWriter.FileSaveName + " " + FB1.BruteCode.QualityModeE.ToString() + ".txt";
            string FileLineList = FB1.DataWriter.FileSaveName + " " + FB1.BruteCode.QualityModeE.ToString() + " List" + ".txt";
            string FileLineOptimized = FB1.DataWriter.FileSaveName + " " + FB1.Optimizer.QualityModeE.ToString() + " OPTIMIZED.txt";
            if (File.Exists(FileLine0)) File.Delete(FileLine0);
            if (File.Exists(FileLineList)) File.Delete(FileLineList);
            if (File.Exists(FileLineOptimized)) File.Delete(FileLineOptimized);
        }
        public static void CleanArrays()//очистим массивы
        {
            if (FB1.BruteCode.BestVariant != null) FB1.BruteCode.BestVariant = new FB1.Data[FB1.BruteCode.BestVariant.Length];
            if (FB1.BruteCode.ReadedVariant.Quality != 0.0) FB1.BruteCode.ReadedVariant = new FB1.Data();
            if (FB1.Optimizer.VariantOptimal.Quality != 0.0) FB1.Optimizer.VariantOptimal = new FB1.Data();
            if (FB1.Optimizer.VariantOptimalArray != null ) FB1.Optimizer.VariantOptimalArray = new FB1.Data[FB1.Optimizer.VariantOptimalArray.Length];
        }
    }
}
