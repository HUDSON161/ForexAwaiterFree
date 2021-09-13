using Awaiter.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FB1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)//Open File Button
        {
            openFileDialog1.ShowDialog();
        }

        private async void openFileDialog1_FileOk(object sender, CancelEventArgs e)//когда выбрали файл в диалоговом окне
        {
            textBox9.Text = openFileDialog1.FileName;
            DataReader.CurrentPath = textBox9.Text;
            BruteCode.QualityModeE = (QUALITY_MODE)Enum.Parse(BruteCode.QualityModeE.GetType(), comboBox2.Text);
            Optimizer.QualityModeE = (QUALITY_MODE)Enum.Parse(Optimizer.QualityModeE.GetType(), comboBox5.Text);
            Enabled = false;
            await DataReader.ReadDataAs(checkBox1, textBox2);//асинхронно прочитаем данные
            Enabled = true;
        }

        private async void button2_Click(object sender, EventArgs e)//кнопка запуска вычислений
        {
            if (Tester.bWork == false)
            {
                Tester.bWork = true;
                button1.Enabled = false;
                button2.Enabled = false;
                button4.Enabled = false;
                button6.Enabled = false;
                button7.Enabled = false;
                button9.Enabled = false;
                button12.Enabled = false;
                pictureBox2.Visible = true;
                label27.Visible = true;
                textBox1.Text = "Brutting...";
                timer2.Start();
                ///
                BruteCode.Method = (EQUATION)Enum.Parse(BruteCode.Method.GetType(), comboBox4.Text.ToUpper());
                BruteCode.BestVariant = new Data[int.Parse(textBox11.Text == "" ? "1" : textBox11.Text)];//зададим размер банка памяти
                for (int i = 0; i < BruteCode.BestVariant.Length; i++)//создам массив вариантов
                {
                    BruteCode.BestVariant[i] = new Data();
                    BruteCode.BestVariant[i].Method = BruteCode.Method;
                }
                Optimizer.variantnum = 0;
                Optimizer.testnum = 0;
                Optimizer.ComplededTests = 0;
                Optimizer.MinutesNeed = 0.0;
                Optimizer.PercentDone = 0.0;

                ///дни и временные корридоры****
                BruteCode.bInitRandomDays = checkBox8.Checked;
                BruteCode.bInitRandomTime = checkBox9.Checked;
                BruteCode.bInitTimeBorders = checkBox10.Checked;
                int checkedI = 0;
                for (int i = 0; i < 7; i++) if (checkedListBox1.GetItemChecked(i)) checkedI++;//считаем сколько галочек
                BruteCode.DaysToTrade = new int[checkedI];
                checkedI = 0;
                for (int i = 0; i < 7; i++) if (checkedListBox1.GetItemChecked(i))//формируем массив дней
                    {
                        if ( i != 6 ) BruteCode.DaysToTrade[checkedI] = i+1;
                        else BruteCode.DaysToTrade[checkedI] = 0;
                        checkedI++;
                    }
                BruteCode.HourStart = int.Parse(textBox28.Text == "" ? "0" : textBox28.Text);
                BruteCode.MinuteStart = int.Parse(textBox29.Text == "" ? "0" : textBox29.Text);
                BruteCode.HourEnd = int.Parse(textBox31.Text == "" ? "0" : textBox31.Text);
                BruteCode.MinuteEnd = int.Parse(textBox30.Text == "" ? "0" : textBox30.Text);
                BruteCode.MinIntervalLength = int.Parse(textBox32.Text == "" ? "1" : textBox32.Text);
                BruteCode.MaxIntervalLength = int.Parse(textBox33.Text == "" ? "1" : textBox33.Text);
                ///
                BruteCode.RelativeDeviation = double.Parse(textBox27.Text == "" ? "0.0" : textBox27.Text.Replace('.', ','));
                if (BruteCode.RelativeDeviation < 0.0 || BruteCode.RelativeDeviation > 1.0) { BruteCode.RelativeDeviation = 1.0; textBox27.Text = "1.0"; }
                BruteCode.bSuper = checkBox7.Checked;
                BruteCode.bSpreadControl = checkBox11.Checked;
                BruteCode.SpreadMax= int.Parse(textBox34.Text == "" ? "0" : textBox34.Text);
                BruteCode.PercentBrute= int.Parse(textBox23.Text == "" || textBox23.Text == "0" ? "100" : textBox23.Text);
                BruteCode.DeepBrute = int.Parse(comboBox3.Text);
                Tester.SimulatedBars = int.Parse(textBox5.Text == "" ? "0" : textBox5.Text);
                BruteCode.DealsMinR = double.Parse(textBox6.Text == "" ? "0,5" : textBox6.Text.Replace('.',','));
                if (Tester.SimulatedBars < 1) { Tester.SimulatedBars = 1; textBox5.Text = "1"; }
                if (BruteCode.DealsMinR < 0.0 || BruteCode.DealsMinR > 1.0) { BruteCode.DealsMinR = 0.5; textBox6.Text = "0.5"; }
                BruteCode.QualityModeE = (QUALITY_MODE)Enum.Parse(BruteCode.QualityModeE.GetType(), comboBox2.Text);
                RandomVariant.RandomType = (TYPE_RANDOM)Enum.Parse(RandomVariant.RandomType.GetType(), comboBox1.Text);
                ///
                int Cores = int.Parse(textBox26.Text);
                Tester[] CoreWorkers=new Tester[Cores];
                for (int i = 0; i < CoreWorkers.Length; i++) CoreWorkers[i] = new Tester();
                if ( Cores == 1 ) _ = await CoreWorkers[0].SearchingAsync(-1, CoreWorkers[0]);
                else _ = await Synchronizer.AsyncSearchMultiCore(Cores, CoreWorkers);
                Tester.bWork = false;
                timer2_Tick(null, null);
                textBox1.Text = "Stopped";
                Enabled = true;
                button1.Enabled = true;
                button2.Enabled = true;
                button4.Enabled = true;
                button6.Enabled = true;
                button7.Enabled = true;
                button9.Enabled = true;
                button12.Enabled = true;
                pictureBox2.Visible = false;
                label27.Visible = false;
                timer2.Stop();
            }
        }

        private void button3_Click(object sender, EventArgs e)//кнопка остановки вычислений
        {
            if (Tester.bWork)
            {
                Tester.bWork = false;
                Enabled = false;
                textBox1.Text = "Finalizing...";
                button6.Enabled = true;
                button7.Enabled = true;
                button9.Enabled = true;
                button10.Enabled = true;
                button11.Enabled = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)//таймер обновления состояния формы
        {
            textBox3.Text = Convert.ToString(Tester.ComplededTests);
            textBox4.Text = Convert.ToString(Tester.TestsPerHour);
            string TempStr = "";
            for (int i = 0; i < BruteCode.BestVariant.Length; i++) TempStr+= Convert.ToString(BruteCode.BestVariant[i].Quality)+Environment.NewLine;
            textBox7.Text = TempStr;
            progressBar1.Value = (int)Optimizer.PercentDone;
            textBox12.Text = Optimizer.MinutesNeed.ToString();
            textBox13.Text = Optimizer.VariantOptimal.Quality.ToString();
            textBox15.Text = Optimizer.VariantOptimal.Orders.ToString();
            TempStr = "";
            if (Optimizer.VariantOptimalArray != null) for (int i = 0; i < Optimizer.VariantOptimalArray.Length; i++) TempStr += Convert.ToString(Optimizer.VariantOptimalArray[i].Quality) + Environment.NewLine;
            textBox18.Text = TempStr;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 2;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;
            comboBox5.SelectedIndex = 0;
            textBox28.Enabled = false;
            textBox29.Enabled = false;
            textBox30.Enabled = false;
            textBox31.Enabled = false;
            textBox32.Enabled = false;
            textBox33.Enabled = false;
            checkBox8.Enabled = false;
            checkBox9.Enabled = false;
            checkedListBox1.Enabled = false;
            button6.Enabled = false;
            button7.Enabled = false;
            button9.Enabled = false;
            button10.Enabled = false;
            button11.Enabled = false;
            pictureBox2.Visible = false;
            pictureBox3.Visible = false;
            label27.Visible = false;
            label30.Visible = false;
            for (int i = 0; i < 5; i++) checkedListBox1.SetItemChecked(i, true);//активируем все с понедельника по пятницу
            Configuration.LoadConfiguration(
                comboBox1, comboBox2, comboBox3,comboBox4,
                textBox5, textBox6, textBox11,textBox26,
                textBox14,
                textBox16,
                textBox9,textBox17, textBox27, textBox21,textBox23,textBox24,
                comboBox5,
                checkBox7, checkBox5, checkBox6, checkBox11, checkBox12,checkBox14, checkBox17, checkBox18,
                checkedListBox1, checkBox10, checkBox8, checkBox9, textBox32, textBox33, textBox28, textBox31, textBox29, textBox30, textBox34, textBox22
                );
            checkBox11_CheckedChanged(checkBox11, null);//вызываем проверку состояния
            checkBox12_CheckedChanged(checkBox12, null);//проверка состояния на вкладке оптимизации
            if ( textBox9.Text != "" ) button4_Click(null, null);
            DataWriter.BotName = textBox9.Text;
            BruteCode.BestVariant = new Data[1] {new Data()};
            timer1.Start();
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)//фильтруем на числа
        {
            if ((e.KeyChar <= 47 || e.KeyChar >= 58) && e.KeyChar != 8)
                e.Handled = true;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)//при закрытии формы
        {
            timer1.Stop();
        }

        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)//фильтруем на числа
        {
            if ((e.KeyChar <= 47 || e.KeyChar >= 58 ) && e.KeyChar != 8 && e.KeyChar != '.')
                e.Handled = true;
        }

        private void timer2_Tick(object sender, EventArgs e)//таймер для сохранения данных
        {
            DataWriter.SaveData();
            DataWriter.SaveDataList();
            textBox8.Text = Convert.ToString(BruteCode.ReadedVariant.Quality);
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            DataReader.CurrentPath = textBox9.Text;
            BruteCode.QualityModeE = (QUALITY_MODE)Enum.Parse(BruteCode.QualityModeE.GetType(), comboBox2.Text);
            Optimizer.QualityModeE = (QUALITY_MODE)Enum.Parse(Optimizer.QualityModeE.GetType(), comboBox5.Text);
            Enabled = false;
            await DataReader.ReadDataAs(checkBox1, textBox2);//асинхронно прочитаем данные
            Enabled = true;
        }

        private void textBox11_KeyPress(object sender, KeyPressEventArgs e)//ввод количества вариантов
        {
            if ((e.KeyChar <= 47 || e.KeyChar >= 58) && e.KeyChar != 8)
                e.Handled = true;
        }

        private void textBox7_KeyPress(object sender, KeyPressEventArgs e)//заблокируем нажатие на текст бокс с вариантами
        {
            e.Handled = true;
        }

        private async void button6_Click(object sender, EventArgs e)//старт оптимизации
        {
            Optimizer.bWork = true;
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button6.Enabled = false;
            button9.Enabled = false;
            button10.Enabled = false;
            button11.Enabled = false;
            button12.Enabled = false;
            pictureBox3.Visible = true;
            label30.Visible = true;
            textBox1.Text = "Optimizing...";
            timer3.Start();
            Optimizer.IterationsToOne = int.Parse(textBox16.Text == "" ? "2" : textBox16.Text);
            if (Optimizer.IterationsToOne < 2) { Optimizer.IterationsToOne = 2; textBox16.Text = "2"; }
            Optimizer.DealsMinR = double.Parse(textBox6.Text == "" ? "0,5" : textBox6.Text.Replace('.', ','));
            if (Tester.SimulatedBars < 1) { Tester.SimulatedBars = 1; textBox5.Text = "1"; }
            if (Optimizer.DealsMinR < 0.0 || Optimizer.DealsMinR > 1.0) { Optimizer.DealsMinR = 0.5; textBox6.Text = "0.5"; }
            Optimizer.DeepBrute = BruteCode.DeepBrute;
            Optimizer.SimulatedBars = Tester.SimulatedBars;
            Optimizer.IterationsAll = Optimizer.IterationsToOne * BruteCode.BestVariant.Length;
            Optimizer.QualityModeE = (QUALITY_MODE)Enum.Parse(Optimizer.QualityModeE.GetType(), comboBox5.Text);
            Optimizer.OrdersMin= int.Parse(textBox14.Text == "" ? "0" : textBox14.Text);
            if (Optimizer.OrdersMin < 1) { Optimizer.OrdersMin = 1; textBox14.Text = "1"; }
            Optimizer.VariantsOptimized= int.Parse(textBox17.Text == "" ? "0" : textBox17.Text);
            if (Optimizer.VariantsOptimized < 1) { Optimizer.VariantsOptimized = 1; textBox17.Text = "1"; }
            Optimizer.RelativeDeviation= double.Parse(textBox21.Text == "" ? "0.0" : textBox21.Text.Replace('.', ','));
            if (Optimizer.RelativeDeviation < 0.0 || Optimizer.RelativeDeviation > 1.0) { Optimizer.RelativeDeviation = 1.0; textBox21.Text = "1.0"; }
            Optimizer.bSuper = checkBox5.Checked;
            Optimizer.bSpreadControl = checkBox12.Checked;
            Optimizer.SpreadMax = int.Parse(textBox22.Text == "" ? "0" : textBox22.Text);
            Optimizer.bPercent = checkBox6.Checked;
            Optimizer.Percent = int.Parse(textBox24.Text == "" || textBox24.Text == "0" ? "100" : textBox24.Text);
            Optimizer.bScalping = checkBox17.Checked;
            Optimizer.bCloseInOutOfTime = checkBox18.Checked;

            int Cores = int.Parse(textBox26.Text);
            Optimizer[] CoreWorkers = new Optimizer[Cores];
            for (int i = 0; i < CoreWorkers.Length; i++)
            {
                CoreWorkers[i] = new Optimizer();
                CoreWorkers[i].bSpreadNoize = !Optimizer.bSpreadControl;
            }
            if (Cores == 1) _ = await CoreWorkers[0].SearchingAsync(0);
            else
            _ = await Synchronizer.AsyncOptimizeMultiCore(Cores,CoreWorkers);
            ///
            timer2_Tick(null, null);
            Optimizer.bWork = false;
            textBox1.Text = "Stopped";
            if (FB1.Optimizer.VariantOptimalArray[0].DaysToTrade != null) FB1.Optimizer.VariantOptimal.CopyData(FB1.Optimizer.VariantOptimalArray[0]);
            if (FB1.Optimizer.VariantOptimalArray[0].DaysToTrade != null) DataWriter.SaveDataOptimized();
            Enabled = true;
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button6.Enabled = true;
            button9.Enabled = true;
            button10.Enabled = true;
            button11.Enabled = true;
            button12.Enabled = true;
            pictureBox3.Visible = false;
            label30.Visible = false;
            timer3.Stop();
        }

        private void button7_Click(object sender, EventArgs e)//стоп оптимизация
        {
            if (Optimizer.bWork)
            {
                Optimizer.bWork = false;
                Enabled = false;
                textBox1.Text = "Finalizing ...";
            }
        }

        private void timer3_Tick(object sender, EventArgs e)//таймер сохранения оптимизации
        {
            if (FB1.Optimizer.VariantOptimalArray[0].DaysToTrade != null) FB1.Optimizer.VariantOptimal.CopyData(FB1.Optimizer.VariantOptimalArray[0]);
            if (FB1.Optimizer.VariantOptimalArray[0].DaysToTrade != null) DataWriter.SaveDataOptimized();
        }

        private void textBox14_KeyPress(object sender, KeyPressEventArgs e)//мин ордеров
        {
            if ((e.KeyChar <= 47 || e.KeyChar >= 58) && e.KeyChar != 8)
                e.Handled = true;
        }

        private void button8_Click(object sender, EventArgs e)//сохранение настроек
        {
            Enabled = false;
            Configuration.SaveConfiguration(
                comboBox1, comboBox2, comboBox3, comboBox4,
                textBox5, textBox6, textBox11, textBox26,
                textBox14,
                textBox16,
                textBox9, textBox17, textBox27, textBox21, textBox23, textBox24,
                comboBox5,
                checkBox7, checkBox5, checkBox6,checkBox11, checkBox12, checkBox14, checkBox17, checkBox18,
                checkedListBox1, checkBox10, checkBox8, checkBox9, textBox32, textBox33, textBox28, textBox31, textBox29, textBox30, textBox34, textBox22
                );
            Enabled = true;
        }

        private void TabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            /*
            if (tabControl1.SelectedTab.Text == "WAIT") {
                Width = 479;
                Height = 507;

                tabControl1.Width = 463; 
                tabControl1.Height = 436;

            }
            
            if (tabControl1.SelectedTab.Text == "WAIT AGAIN") {
                Width = 479;
                Height = 507;

                tabControl1.Width = 463;
                tabControl1.Height = 436;

            }
            
            if (tabControl1.SelectedTab.Text == "GENERATE AND WAIT"){
                Width = 479;
                Height = 230;

                tabControl1.Width = 463;
                tabControl1.Height = 159;
            }
            */
 
        }

        private void textBox16_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar <= 47 || e.KeyChar >= 58) && e.KeyChar != 8)
                e.Handled = true;
        }

        private void textBox17_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar <= 47 || e.KeyChar >= 58) && e.KeyChar != 8)
                e.Handled = true;
        }

        private void textBox18_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void textBox20_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar <= 47 || e.KeyChar >= 58) && e.KeyChar != 8)
                e.Handled = true;
        }


        private void button9_Click(object sender, EventArgs e)//рисование графика по выбранному элементу
        {
            button9.Enabled = false;
            button10.Enabled = false;
            button11.Enabled = false;
            button6.Enabled = false;
            Awaiter.Graph.LoadedGraphIndex = int.Parse(textBox20.Text == "" ? "0" : textBox20.Text);
            if (Awaiter.Graph.LoadedGraphIndex < 0) { Awaiter.Graph.LoadedGraphIndex = 0; textBox20.Text = "0"; }
            if (Awaiter.Graph.LoadedGraphIndex >= Optimizer.VariantOptimalArray.Length) { Awaiter.Graph.LoadedGraphIndex = Optimizer.VariantOptimalArray.Length - 1; textBox20.Text = (Optimizer.VariantOptimalArray.Length - 1).ToString(); }
            textBox19.Text = Optimizer.VariantOptimalArray[Awaiter.Graph.LoadedGraphIndex].Quality.ToString();
            textBox13.Text = Optimizer.VariantOptimalArray[Awaiter.Graph.LoadedGraphIndex].Quality.ToString();
            textBox15.Text = Optimizer.VariantOptimalArray[Awaiter.Graph.LoadedGraphIndex].Orders.ToString();
            Awaiter.Graph.Clean(pictureBox1);
            Awaiter.Graph.Draw(pictureBox1);
            button6.Enabled = true;
            button9.Enabled = true;
            button10.Enabled = true;
            button11.Enabled = true;
        }

        private void button10_Click(object sender, EventArgs e)//Next
        {
            button9.Enabled = false;
            button10.Enabled = false;
            button11.Enabled = false;
            Awaiter.Graph.ChangeSelected(true);
            textBox13.Text = Optimizer.VariantOptimalArray[Awaiter.Graph.LoadedGraphIndex].Quality.ToString();
            textBox19.Text = Optimizer.VariantOptimalArray[Awaiter.Graph.LoadedGraphIndex].Quality.ToString();
            textBox20.Text = Awaiter.Graph.LoadedGraphIndex.ToString();
            textBox15.Text = Optimizer.VariantOptimalArray[Awaiter.Graph.LoadedGraphIndex].Orders.ToString();
            Awaiter.Graph.Clean(pictureBox1);
            Awaiter.Graph.Draw(pictureBox1);
            button9.Enabled = true;
            button10.Enabled = true;
            button11.Enabled = true;
        }

        private void button11_Click(object sender, EventArgs e)//Back
        {
            button9.Enabled = false;
            button10.Enabled = false;
            button11.Enabled = false;
            Awaiter.Graph.ChangeSelected(false);
            textBox13.Text = Optimizer.VariantOptimalArray[Awaiter.Graph.LoadedGraphIndex].Quality.ToString();
            textBox19.Text = Optimizer.VariantOptimalArray[Awaiter.Graph.LoadedGraphIndex].Quality.ToString();
            textBox20.Text = Awaiter.Graph.LoadedGraphIndex.ToString();
            textBox15.Text = Optimizer.VariantOptimalArray[Awaiter.Graph.LoadedGraphIndex].Orders.ToString();
            Awaiter.Graph.Clean(pictureBox1);
            Awaiter.Graph.Draw(pictureBox1);
            button9.Enabled = true;
            button10.Enabled = true;
            button11.Enabled = true;
        }

        private void textBox21_KeyPress(object sender, KeyPressEventArgs e)//текст бокс относительного отклонения
        {
            if ((e.KeyChar <= 47 || e.KeyChar >= 58) && e.KeyChar != 8 && e.KeyChar != '.')
                e.Handled = true;
        }

        private void textBox22_KeyPress(object sender, KeyPressEventArgs e)//текст бокс  профита прерывающей сделки
        {
            if ((e.KeyChar <= 47 || e.KeyChar >= 58) && e.KeyChar != 8)
                e.Handled = true;
        }

        private void textBox23_KeyPress(object sender, KeyPressEventArgs e)//текст бокс процента брута загруженной котировки
        {
            if ((e.KeyChar <= 47 || e.KeyChar >= 58) && e.KeyChar != 8)
                e.Handled = true;
        }

        private void textBox23_TextChanged(object sender, EventArgs e)//текст изменился в текст боксе процентов
        {
            if  (textBox23.Text != "" && int.Parse(textBox23.Text) > 100) textBox23.Text="100";
            if (textBox23.Text != "" && int.Parse(textBox23.Text) <= 0) textBox23.Text = "1";
        }

        private void textBox24_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar <= 47 || e.KeyChar >= 58) && e.KeyChar != 8)
                e.Handled = true;
        }

        private void textBox24_TextChanged(object sender, EventArgs e)
        {
            if (textBox24.Text != "" && int.Parse(textBox24.Text) > 100) textBox24.Text = "100";
            if (textBox24.Text != "" && int.Parse(textBox24.Text) <= 0) textBox24.Text = "1";
        }

        private void textBox25_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar <= 47 || e.KeyChar >= 58) && e.KeyChar != 8 && e.KeyChar != '.')
                e.Handled = true;
        }

        private void textBox26_KeyPress(object sender, KeyPressEventArgs e)//cores
        {
            if ((e.KeyChar <= 47 || e.KeyChar >= 58) && e.KeyChar != 8 )
                e.Handled = true;
        }

        private void textBox26_TextChanged(object sender, EventArgs e)//cores
        {
            if (textBox24.Text != "" && int.Parse(textBox26.Text) <= 0) textBox26.Text = "1";
        }

        private void textBox27_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar <= 47 || e.KeyChar >= 58) && e.KeyChar != 8 && e.KeyChar != '.')
                e.Handled = true;
        }

        private void textBox32_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar <= 47 || e.KeyChar >= 58) && e.KeyChar != 8)
                e.Handled = true;
        }

        private void textBox32_TextChanged(object sender, EventArgs e)
        {
            if (textBox32.Text != "" && int.Parse(textBox32.Text) <= 0) textBox32.Text = "1";
        }

        private void textBox33_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar <= 47 || e.KeyChar >= 58) && e.KeyChar != 8)
                e.Handled = true;
        }

        private void textBox33_TextChanged(object sender, EventArgs e)
        {
            if (textBox33.Text != "" && int.Parse(textBox33.Text) <= 0) textBox33.Text = "1";
        }

        private void textBox28_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar <= 47 || e.KeyChar >= 58) && e.KeyChar != 8)
                e.Handled = true;
        }

        private void textBox28_TextChanged(object sender, EventArgs e)
        {
            if (textBox28.Text != "" && int.Parse(textBox28.Text) <= 0) textBox28.Text = "0";
        }

        private void textBox31_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar <= 47 || e.KeyChar >= 58) && e.KeyChar != 8)
                e.Handled = true;
        }

        private void textBox31_TextChanged(object sender, EventArgs e)
        {
            if (textBox31.Text != "" && int.Parse(textBox31.Text) <= 0) textBox31.Text = "0";
        }

        private void textBox29_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar <= 47 || e.KeyChar >= 58) && e.KeyChar != 8)
                e.Handled = true;
        }

        private void textBox29_TextChanged(object sender, EventArgs e)
        {
            if (textBox29.Text != "" && int.Parse(textBox29.Text) <= 0) textBox29.Text = "0";
        }

        private void textBox30_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar <= 47 || e.KeyChar >= 58) && e.KeyChar != 8)
                e.Handled = true;
        }

        private void textBox30_TextChanged(object sender, EventArgs e)
        {
            if (textBox30.Text != "" && int.Parse(textBox30.Text) <= 0) textBox30.Text = "0";
        }

        private void timer4_Tick(object sender, EventArgs e)//срабатывание таймера контроля сессии
        {

        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)//проверка на чек девиации первой вкладки
        {
            if (((CheckBox)sender).Checked) textBox27.Enabled = true;
            else textBox27.Enabled = false;
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {
                textBox28.Enabled = false;
                textBox29.Enabled = false;
                textBox30.Enabled = false;
                textBox31.Enabled = false;
                textBox32.Enabled = true;
                textBox33.Enabled = true;
            }
            else
            {
                textBox28.Enabled = true;
                textBox29.Enabled = true;
                textBox30.Enabled = true;
                textBox31.Enabled = true;
                textBox32.Enabled = false;
                textBox33.Enabled = false;
            }
        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {
            if ( ((CheckBox)sender).Checked )
            {
                checkBox8.Enabled = true;
                checkBox9.Enabled = true;
                checkedListBox1.Enabled = true;
                if (checkBox9.Checked)
                {
                    textBox28.Enabled = false;
                    textBox29.Enabled = false;
                    textBox30.Enabled = false;
                    textBox31.Enabled = false;
                    textBox32.Enabled = true;
                    textBox33.Enabled = true;
                }
                else
                {
                    textBox28.Enabled = true;
                    textBox29.Enabled = true;
                    textBox30.Enabled = true;
                    textBox31.Enabled = true;
                    textBox32.Enabled = false;
                    textBox33.Enabled = false;
                }
            }
            else
            {
                textBox28.Enabled = false;
                textBox29.Enabled = false;
                textBox30.Enabled = false;
                textBox31.Enabled = false;
                textBox32.Enabled = false;
                textBox33.Enabled = false;
                checkBox8.Enabled = false;
                checkBox9.Enabled = false;
                checkedListBox1.Enabled = false;
            }
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked) textBox21.Enabled = true;
            else textBox21.Enabled = false;
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked) textBox24.Enabled = true;
            else textBox24.Enabled = false;
        }

        private void checkBox12_CheckedChanged(object sender, EventArgs e)//спред контроль оптимизация
        {
            textBox22.Enabled= (sender as CheckBox).Checked ? true : false;
            checkBox16.Checked = !checkBox12.Checked;
        }

        private void checkBox11_CheckedChanged(object sender, EventArgs e)//спред контроль брут
        {
            textBox34.Enabled = (sender as CheckBox).Checked ? true : false;
            checkBox15.Checked = !checkBox11.Checked;
        }

        private void textBox34_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar <= 47 || e.KeyChar >= 58) && e.KeyChar != 8)
                e.Handled = true;
        }

        private void textBox22_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar <= 47 || e.KeyChar >= 58) && e.KeyChar != 8)
                e.Handled = true;
        }

        private void button12_Click(object sender, EventArgs e)//очистка
        {
            Awaiter.Cleaner.CleanArrays();
            Awaiter.Cleaner.CleanFiles();
            textBox3.Text = "0";
            textBox4.Text = "0";
            textBox8.Text = "0";
            textBox7.Text = "";

            textBox15.Text = "0";
            textBox13.Text = "0";
            textBox12.Text = "0";
            textBox19.Text = "0";
            textBox18.Text = "";

            button6.Enabled = false;
            button7.Enabled = false;
            button9.Enabled = false;
            button10.Enabled = false;
            button11.Enabled = false;
            Awaiter.Graph.Clean(pictureBox1);
            Optimizer.Refresh();
        }

        private void checkBox15_CheckedChanged(object sender, EventArgs e)
        {
            checkBox11.Checked = !checkBox15.Checked;
        }

        private void checkBox16_CheckedChanged(object sender, EventArgs e)
        {
            checkBox12.Checked = !checkBox16.Checked;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)//я
        {
            System.Diagnostics.Process.Start("https://www.mql5.com/ru/users/w.hudson");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)//MetaTrader 4
        {
            System.Diagnostics.Process.Start("https://www.mql5.com/en/market/product/61516");
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.mql5.com/en/market/product/61653");
        }
    }
}
