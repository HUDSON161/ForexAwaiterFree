using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace FB1
{
    static class DataReader
    {
        public static string CurrentPath;//текущий путь
        public static DataTester[] Data;
        public static DataTester[] SpeadDestroyedData;
        public static double Point=0;

        public static int YearStart = 0;//стартовый год
        public static int MonthStart = 0;//стартовый месяц
        public static int DayStart = 0;//стартовый день

        public static int YearEnd = 0;//конечный год
        public static int MonthEnd = 0;//конечный месяц
        public static int DayEnd = 0;//конечный день

        static List<DataTester> DataBuffer=new List<DataTester>();
        static DataTester TempData;


        public static async Task<bool> ReadDataAs(CheckBox cb1, TextBox tb1)//асинхронно прочитаем данные из текстового файла
        {
            cb1.Checked = false;
            tb1.Text = "";
            try
            {
                await Task.Run(() => ReadData());
                cb1.Checked = true;
                tb1.Text = Convert.ToString(Data.Length);
            }
            catch
            {
                cb1.Checked = false;
                tb1.Text = "";
            }
            return true;
        }
        static bool ReadData()//прочитаем данные из текстового файла
        {
            using (StreamReader sr = new StreamReader(CurrentPath, System.Text.Encoding.Default))
            {
                string line;
                int elements = 0;//счетчик количества элементов
                int dataelems = 0;//счетчик элементов данных в файле

                if ((line = sr.ReadLine()) != null) DataWriter.FileSaveName = line;//читаем имя файла
                if ((line = sr.ReadLine()) != null) Point = Convert.ToDouble(line.Replace('.', ','));//читаем пункты
                if ((line = sr.ReadLine()) != null) YearStart = Convert.ToInt32(line);//читаем стартовый год
                if ((line = sr.ReadLine()) != null) MonthStart = Convert.ToInt32(line);//читаем стартовый месяц
                if ((line = sr.ReadLine()) != null) DayStart = Convert.ToInt32(line);//читаем стартовый день


                DataBuffer.Clear();
                while ((line = sr.ReadLine()) != "EndBars")
                {
                    if (line == string.Empty || line == " ")
                    {
                        dataelems = 0;
                        elements++;
                    }
                    else
                    {
                        if (dataelems == 0) TempData = new DataTester();
                        /////
                        if (dataelems == 0) TempData.PriceClose = Convert.ToDouble(line.Replace('.', ','));
                        if (dataelems == 1) TempData.PriceOpen = Convert.ToDouble(line.Replace('.', ','));
                        if (dataelems == 2) TempData.PriceHigh = Convert.ToDouble(line.Replace('.', ','));
                        if (dataelems == 3) TempData.PriceLow = Convert.ToDouble(line.Replace('.', ','));
                        if (dataelems == 4) TempData.TimeOpen = Convert.ToInt32(line);
                        if (dataelems == 5) TempData.Spread = Convert.ToInt32(line);
                        if (dataelems == 6) TempData.SpreadClose = Convert.ToInt32(line);
                        if (dataelems == 7) TempData.SpreadHigh = Convert.ToInt32(line);
                        if (dataelems == 8) TempData.SpreadLow = Convert.ToInt32(line);
                        if (dataelems == 9) TempData.HourOpen = Convert.ToInt32(line);
                        if (dataelems == 10) TempData.MinuteOpen = Convert.ToInt32(line);
                        if (dataelems == 11) TempData.DayOpen = Convert.ToInt32(line);
                        /////
                        if (dataelems == 11) DataBuffer.Add(TempData);
                        //MessageBox.Show($"{TempData.HourOpen} {TempData.MinuteOpen} {TempData.DayOpen}");
                        dataelems++;
                    }
                }
                if ((line = sr.ReadLine()) != null) YearEnd = Convert.ToInt32(line);//читаем стартовый год
                if ((line = sr.ReadLine()) != null) MonthEnd = Convert.ToInt32(line);//читаем стартовый месяц
                if ((line = sr.ReadLine()) != null) DayEnd = Convert.ToInt32(line);//читаем стартовый день
                if (DataBuffer.Count > 0)
                {
                    Data = new DataTester[DataBuffer.Count];
                    DataBuffer.Reverse();
                    for (int i = 0; i < DataBuffer.Count; i++)
                    {
                        Data[i] = DataBuffer[i];
                    }
                    DataBuffer.Clear();
                    CreateSpreadDestroyedData();
                    return true;
                }
            }
            return false;
        }

        static void CreateSpreadDestroyedData()//создадим массив с данными без спредовых шумов
        {
            SpeadDestroyedData= new DataTester[Data.Length];
            for (int i = 0; i < Data.Length; i++)
            {
                SpeadDestroyedData[i] = Data[i];
                SpeadDestroyedData[i].PriceClose += SpeadDestroyedData[i].SpreadClose/2.0 * Point;
                SpeadDestroyedData[i].PriceHigh += SpeadDestroyedData[i].SpreadHigh/2.0 * Point;
                SpeadDestroyedData[i].PriceLow += SpeadDestroyedData[i].SpreadLow / 2.0 * Point;
                SpeadDestroyedData[i].PriceOpen += SpeadDestroyedData[i].Spread / 2.0 * Point;
            }
        }

    }
}
