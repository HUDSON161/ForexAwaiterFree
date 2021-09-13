using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Awaiter
{
    internal static class Graph//для отрисовки графиков
    {
        public static int LoadedGraphIndex=-1;//индекс выбранного элемента
        public static Graphics g;
        private static Pen p = new Pen(Color.Gold, 2);//ручка для брут части графика
        private static Pen p2 = new Pen(Color.Red, 1);//ручка для небрученой части графика

        public static void ChangeSelected(bool bNext)//меняем индекс выбранного варианта в зависимости от нажатой кнопки
        {
            if ( FB1.Optimizer.VariantOptimalArray.Length > 0)
            {
                if (bNext)
                {
                    if (LoadedGraphIndex == -1 || LoadedGraphIndex == FB1.Optimizer.VariantOptimalArray.Length - 1)
                    {
                        LoadedGraphIndex = 0;
                    }
                    else
                    {
                        LoadedGraphIndex++;
                    }
                }
                else
                {
                    if (LoadedGraphIndex == -1 || LoadedGraphIndex == 0)
                    {
                        LoadedGraphIndex = FB1.Optimizer.VariantOptimalArray.Length - 1;
                    }
                    else
                    {
                        LoadedGraphIndex--;
                    }
                }
            }
        }
        public static void Clean(PictureBox PBox)//очистим график
        {
            PBox.Image = (Image)new Bitmap(PBox.Size.Width, PBox.Size.Height);
            g = Graphics.FromImage(PBox.Image);
        }

        public static void Draw(PictureBox PBox)//нарисуем график
        {
            if (FB1.Optimizer.VariantOptimalArray[LoadedGraphIndex].Orders > 0 && FB1.Optimizer.VariantOptimalArray[LoadedGraphIndex].Quality != 0.0)
            {
                FB1.Optimizer.VariantOptimal.CopyData(FB1.Optimizer.VariantOptimalArray[LoadedGraphIndex]);//изменим сохраненные данные
                FB1.DataWriter.SaveDataOptimized();
                //сначала определим цену деления осей Y и X
                double ScaleProfit = (PBox.Size.Height * 0.85) / Math.Abs(FB1.Optimizer.VariantOptimalArray[LoadedGraphIndex].MaxProfit + FB1.Optimizer.VariantOptimalArray[LoadedGraphIndex].MinProfit);
                double ScaleOrderNum = (PBox.Size.Width * 0.99) / (double)FB1.Optimizer.VariantOptimalArray[LoadedGraphIndex].Orders;
                double Y0 = PBox.Size.Height * 0.02;
                //потом определим стартовую точку (откуда рисовать первый ордер)
                double PrevProfit = 0.0;
                if (FB1.Optimizer.VariantOptimalArray[LoadedGraphIndex].Quality < 0.0) Y0 += (ScaleProfit * FB1.Optimizer.VariantOptimalArray[LoadedGraphIndex].MinProfit);//зададим стартовую точку
                else Y0 += (ScaleProfit * FB1.Optimizer.VariantOptimalArray[LoadedGraphIndex].MaxProfit);//зададим стартовую точку
                if (FB1.Optimizer.VariantOptimalArray[LoadedGraphIndex].Quality < 0.0)
                {
                    for (int i = 0; i < FB1.Optimizer.VariantOptimalArray[LoadedGraphIndex].Order.Count; i++)
                    {
                        g.DrawLine(FB1.Optimizer.VariantOptimalArray[LoadedGraphIndex].Order[i].bInBruteTimeframe ? p : p2, new Point((int)(i * ScaleOrderNum), (int)(Y0 + PrevProfit)), new Point((int)((i + 1) * ScaleOrderNum), (int)(Y0 + PrevProfit + ScaleProfit * FB1.Optimizer.VariantOptimalArray[LoadedGraphIndex].Order[i].Value)));
                        PrevProfit += (ScaleProfit * FB1.Optimizer.VariantOptimalArray[LoadedGraphIndex].Order[i].Value);
                    }
                }
                else
                {
                    for (int i = 0; i < FB1.Optimizer.VariantOptimalArray[LoadedGraphIndex].Order.Count; i++)
                    {
                        g.DrawLine(FB1.Optimizer.VariantOptimalArray[LoadedGraphIndex].Order[i].bInBruteTimeframe ? p : p2, new Point((int)(i * ScaleOrderNum), (int)(Y0 + PrevProfit)), new Point((int)((i + 1) * ScaleOrderNum), (int)(Y0 + PrevProfit - ScaleProfit * FB1.Optimizer.VariantOptimalArray[LoadedGraphIndex].Order[i].Value)));
                        PrevProfit -= (ScaleProfit * FB1.Optimizer.VariantOptimalArray[LoadedGraphIndex].Order[i].Value);
                    }
                }
            }
        }
    }
}

