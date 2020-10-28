using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PcLedVisualization
{
    class Led
    {
        public System.Windows.Shapes.Rectangle rectangle;
        public int index;
        public bool isVertical;
        public double height;
        public double width;
        public byte r;
        public byte g;
        public byte b;

        public Led(int iniIndex, bool iniIsVertical, double iniHeight, double iniWidth)
        {
            rectangle = new System.Windows.Shapes.Rectangle();
            rectangle.Name = "led" + index;
            index = iniIndex;
            isVertical = iniIsVertical;
            height = iniHeight;
            width = iniWidth;

            switch (iniIndex % 3)
            {
                case 0:
                    r = 255;
                    g = 0;
                    b = 0;
                    break;
                case 1:
                    r = 0;
                    g = 255;
                    b = 0;
                    break;
                case 2:
                    r = 0;
                    g = 0;
                    b = 255;
                    break;
                default:
                    r = 0;
                    g = 0;
                    b = 0;
                    break;
            }

            rectangle.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(r, g, b));
            rectangle.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
            rectangle.Width = width;
            rectangle.Height = height;
        }

        public string changeColor(Bitmap map, bool skipVisual)
        {
            string ledColorsLocal = "";

            BitmapData srcData = map.LockBits(
            new System.Drawing.Rectangle(0, 0, map.Width, map.Height),
            ImageLockMode.ReadOnly,
            System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            int stride = srcData.Stride;

            IntPtr Scan0 = srcData.Scan0;

            long[] totals = new long[] { 0, 0, 0 };

            int width = map.Width;
            int height = map.Height;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        for (int color = 0; color < 3; color++)
                        {
                            int idx = (y * stride) + x * 4 + color;

                            totals[color] += p[idx];
                        }
                    }
                }
            }

            long avgB = totals[0] / (width * height);
            long avgG = totals[1] / (width * height);
            long avgR = totals[2] / (width * height);

            long maxMargin = 0;

            if (avgR < 200 && avgG < 200 && avgB < 200 && (avgR > 60 || avgG > 60 || avgB > 60))
            {
                if (avgB >= avgG && avgB >= avgR)
                {
                    maxMargin = 200 - avgB;
                }

                else if (avgG >= avgR && avgG >= avgB)
                {
                    maxMargin = 200 - avgG;
                }

                else if (avgR >= avgG && avgR >= avgB)
                {
                    maxMargin = 200 - avgR;
                }

                avgR += maxMargin;
                avgG += maxMargin;
                avgB += maxMargin;
            }
            



            if (!skipVisual)
                this.rectangle.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb((byte)avgR, (byte)avgG, (byte)avgB));

                if (avgR < 10)
                {
                    ledColorsLocal += "00" + avgR;
                }
                else if(avgR < 100)
                {
                    ledColorsLocal += "0" + avgR;
                }
                else
                {
                    ledColorsLocal += "" + avgR;
                }

                if (avgG < 10)
                {
                    ledColorsLocal += "00" + avgG;
                }
                else if (avgG < 100)
                {
                    ledColorsLocal += "0" + avgG;
                }
                else
                {
                    ledColorsLocal += "" + avgG;
                }

                if (avgB < 10)
                {
                    ledColorsLocal += "00" + avgB;
                }
                else if (avgB < 100)
                {
                    ledColorsLocal += "0" + avgB;
                }
                else
                {
                    ledColorsLocal += "" + avgB;
                }

            return ledColorsLocal;
        }
    }
}
