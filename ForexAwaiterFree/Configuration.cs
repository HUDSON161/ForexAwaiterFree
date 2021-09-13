using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace FB1
{
    internal static class Configuration
    {
        public static void SaveConfiguration(
            ComboBox ReandomTypeBox,ComboBox SearchTypeBox, ComboBox EquationDeepBox,ComboBox EquationBox,
            TextBox BarsBox, TextBox DealsAsymmetryBox, TextBox VariantsMemoryBox,TextBox CoresBox,
            TextBox MinOrdersBox,
            TextBox VariantsOptimizationBox,
            TextBox PathBox,TextBox OptimizedBox, TextBox DeviationBoxBrute, TextBox DeviationBox,TextBox PercentBox,TextBox PercentBoxOptimizer,
            ComboBox ModeBox,
            CheckBox CheckBoxLineBrute, CheckBox CheckBoxLine, CheckBox CheckBoxPercent, CheckBox CheckSpreadFirst, CheckBox CheckSpreadSecond, CheckBox SpreadCloseBox, CheckBox ScalpingBox, CheckBox OutOfTimeBox,
            CheckedListBox DaysBox, CheckBox InitTimeAndDaysBox, CheckBox RandomDaysBox, CheckBox RandomTimeBox,
            TextBox MinutesMinBox,TextBox MinutesMaxBox, TextBox HourStartBox, TextBox HourEndBox,
            TextBox MinuteStartBox, TextBox MinuteEndBox, TextBox SpreadBoxFirst, TextBox SpreadBoxSecond
            )//сохраним конфигурацию
        {
            string FileLine = "BrutterXXXConfig.ini";
            using (StreamWriter sw = new StreamWriter(FileLine, false))
            {
                sw.WriteLine(ReandomTypeBox.Text);
                sw.WriteLine(SearchTypeBox.Text);
                sw.WriteLine(EquationDeepBox.Text);
                sw.WriteLine(EquationBox.Text);
                sw.WriteLine(BarsBox.Text);
                sw.WriteLine(DealsAsymmetryBox.Text);
                sw.WriteLine(VariantsMemoryBox.Text);
                sw.WriteLine(CoresBox.Text);
                sw.WriteLine(MinOrdersBox.Text);
                sw.WriteLine(VariantsOptimizationBox.Text);
                sw.WriteLine(PathBox.Text);
                sw.WriteLine(OptimizedBox.Text);
                sw.WriteLine(DeviationBox.Text);
                sw.WriteLine(DeviationBoxBrute.Text);
                sw.WriteLine(ModeBox.Text);
                sw.WriteLine(PercentBox.Text);
                sw.WriteLine(PercentBoxOptimizer.Text);
                sw.WriteLine(CheckBoxLine.Checked.ToString());
                sw.WriteLine(CheckBoxLineBrute.Checked.ToString());
                sw.WriteLine(CheckBoxPercent.Checked.ToString());
                sw.WriteLine(CheckSpreadFirst.Checked.ToString());
                sw.WriteLine(CheckSpreadSecond.Checked.ToString());
                sw.WriteLine(SpreadCloseBox.Checked.ToString());
                sw.WriteLine(ScalpingBox.Checked.ToString());
                sw.WriteLine(OutOfTimeBox.Checked.ToString());
                sw.WriteLine(SpreadBoxFirst.Text);
                sw.WriteLine(SpreadBoxSecond.Text);
                for (int i = 0; i < 7; i++) sw.WriteLine(DaysBox.GetItemChecked(i).ToString());
                sw.WriteLine(InitTimeAndDaysBox.Checked.ToString());
                sw.WriteLine(RandomDaysBox.Checked.ToString());
                sw.WriteLine(RandomTimeBox.Checked.ToString());
                sw.WriteLine(MinutesMinBox.Text);
                sw.WriteLine(MinutesMaxBox.Text);
                sw.WriteLine(HourStartBox.Text);
                sw.WriteLine(HourEndBox.Text);
                sw.WriteLine(MinuteStartBox.Text);
                sw.WriteLine(MinuteEndBox.Text);
            }
        }

        public static void LoadConfiguration(
            ComboBox ReandomTypeBox, ComboBox SearchTypeBox, ComboBox EquationDeepBox, ComboBox EquationBox,
            TextBox BarsBox, TextBox DealsAsymmetryBox, TextBox VariantsMemoryBox,TextBox CoresBox,
            TextBox MinOrdersBox,
            TextBox VariantsOptimizationBox,
            TextBox PathBox, TextBox OptimizedBox, TextBox DeviationBoxBrute, TextBox DeviationBox,TextBox PercentBox,TextBox PercentBoxOptimizer,
            ComboBox ModeBox,
            CheckBox CheckBoxLineBrute, CheckBox CheckBoxLine, CheckBox CheckBoxPercent, CheckBox CheckSpreadFirst, CheckBox CheckSpreadSecond, CheckBox SpreadCloseBox, CheckBox ScalpingBox, CheckBox OutOfTimeBox,
            CheckedListBox DaysBox, CheckBox InitTimeAndDaysBox, CheckBox RandomDaysBox, CheckBox RandomTimeBox,
            TextBox MinutesMinBox, TextBox MinutesMaxBox, TextBox HourStartBox, TextBox HourEndBox,
            TextBox MinuteStartBox, TextBox MinuteEndBox, TextBox SpreadBoxFirst, TextBox SpreadBoxSecond
            )//восстановим конфигурацию
        {
            string FileLine = "BrutterXXXConfig.ini";
            if (File.Exists(FileLine))
            {
                using (StreamReader sr = new StreamReader(FileLine, System.Text.Encoding.UTF8))
                {
                    ReandomTypeBox.Text = sr.ReadLine();
                    SearchTypeBox.Text = sr.ReadLine();
                    EquationDeepBox.Text = sr.ReadLine();
                    EquationBox.Text = sr.ReadLine();
                    BarsBox.Text = sr.ReadLine();
                    DealsAsymmetryBox.Text = sr.ReadLine();
                    VariantsMemoryBox.Text = sr.ReadLine();
                    CoresBox.Text = sr.ReadLine();
                    MinOrdersBox.Text = sr.ReadLine();
                    VariantsOptimizationBox.Text = sr.ReadLine();
                    PathBox.Text = sr.ReadLine();
                    OptimizedBox.Text = sr.ReadLine();
                    DeviationBox.Text = sr.ReadLine();
                    DeviationBoxBrute.Text = sr.ReadLine();
                    ModeBox.Text = sr.ReadLine();
                    PercentBox.Text = sr.ReadLine();
                    PercentBoxOptimizer.Text = sr.ReadLine();
                    CheckBoxLine.Checked = bool.Parse(sr.ReadLine());
                    CheckBoxLineBrute.Checked = bool.Parse(sr.ReadLine());
                    CheckBoxPercent.Checked = bool.Parse(sr.ReadLine());
                    CheckSpreadFirst.Checked = bool.Parse(sr.ReadLine());
                    CheckSpreadSecond.Checked = bool.Parse(sr.ReadLine());
                    SpreadCloseBox.Checked = bool.Parse(sr.ReadLine());
                    ScalpingBox.Checked = bool.Parse(sr.ReadLine());
                    OutOfTimeBox.Checked = bool.Parse(sr.ReadLine());
                    SpreadBoxFirst.Text= sr.ReadLine();
                    SpreadBoxSecond.Text = sr.ReadLine();
                    for (int i = 0; i < 7; i++) DaysBox.SetItemChecked(i, bool.Parse(sr.ReadLine()));
                    InitTimeAndDaysBox.Checked = bool.Parse(sr.ReadLine());
                    RandomDaysBox.Checked = bool.Parse(sr.ReadLine());
                    RandomTimeBox.Checked = bool.Parse(sr.ReadLine());
                    MinutesMinBox.Text = sr.ReadLine();
                    MinutesMaxBox.Text = sr.ReadLine();
                    HourStartBox.Text = sr.ReadLine();
                    HourEndBox.Text = sr.ReadLine();
                    MinuteStartBox.Text = sr.ReadLine();
                    MinuteEndBox.Text = sr.ReadLine();
                }
            }
        }
    }
}
