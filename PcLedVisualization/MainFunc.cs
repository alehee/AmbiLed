using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.IO.Ports;

namespace PcLedVisualization
{
    public partial class MainWindow : Window
    {
        /// *** PROCESS SCREEN FUNCTIONS AREA ***

        /// F: screen capture timer event
        async Task screenCapture()
        {
            Bitmap bitmap_Screen = new Bitmap(screenWidth, screenHeight);
            Graphics g = Graphics.FromImage(bitmap_Screen);
            g.CopyFromScreen(screenLeft, screenTop, 0, 0, bitmap_Screen.Size);

            if (!sendingLedsToArduino)
            {
                var source = Task.Run(async () => await ConvertToImage(bitmap_Screen)).Result;
                I_Screen.Source = source;
            }

            Dispatcher.BeginInvoke(new Action(() => {
                for (int i = 0; i < ledsTotal; i++)
                {
                    System.Drawing.Rectangle screenRectangle = new System.Drawing.Rectangle(0, 0, 10, 10);
                    if (i < ledsHorizontal)
                    {
                        // Bottom
                        screenRectangle = new System.Drawing.Rectangle((int)(screenWidth - (i + 1) * screenHorizontalWidth), (int)(screenHeight - screenCaptureThickness), (int)(screenHorizontalWidth), screenCaptureThickness);
                    }
                    else if (i < (ledsHorizontal + ledsVertical))
                    {
                        // Left
                        screenRectangle = new System.Drawing.Rectangle((int)screenLeft, (int)(screenHeight - (i - ledsHorizontal + 1) * screenVerticalHeight), screenCaptureThickness, (int)(screenVerticalHeight));
                    }
                    else if (i < (ledsHorizontal * 2 + ledsVertical))
                    {
                        // Top
                        screenRectangle = new System.Drawing.Rectangle((int)(screenLeft + (i - ledsHorizontal - ledsVertical) * screenHorizontalWidth), (int)(screenTop), (int)(screenHorizontalWidth), screenCaptureThickness);
                    }
                    else
                    {
                        // Right
                        screenRectangle = new System.Drawing.Rectangle((int)(screenWidth - screenCaptureThickness), (int)(screenTop + (i - ledsVertical - ledsHorizontal * 2) * screenVerticalHeight), screenCaptureThickness, (int)(screenVerticalHeight));
                    }

                    string ledColors = Task.Run(async () => await leds[i].changeColor(bitmap_Screen.Clone(screenRectangle, bitmap_Screen.PixelFormat), sendingLedsToArduino)).Result;

                    if (sendingLedsToArduino)
                        serialSend += ledColors;
                }
            }));

            if (sendingLedsToArduino)
            {
                try
                {
                    port.Write(serialSend + "\n");
                }
                catch { }

                serialSend = "";
                timerScreenCapture.Stop();
                while (dataRead == false)
                {
                    int data = port.ReadByte();
                    if (data != null)
                    {
                        dataRead = true;
                    }
                }
                dataRead = false;
                timerScreenCapture.Start();
            }
        }
        /// ==========

        /// F: screen capture convert
        public async Task<BitmapImage> ConvertToImage(Bitmap src)
        {
            MemoryStream ms = new MemoryStream();
            ((System.Drawing.Bitmap)src).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }
        /// ==========

        /// F: test led timer event
        void ledCheckReturn(Object sender, EventArgs e)
        {
            byte r, g, b;
            int ledToCheckIndex = 0;
            Dispatcher.BeginInvoke(new Action(() => {
                ledToCheckIndex = Convert.ToInt32(TB_TestLed.Text);
                switch (ledToCheckIndex % 3)
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
                leds[ledToCheckIndex].rectangle.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(r, g, b));
            }));
        }
        /// ==========

        /// ==========




        /// *** SMALL FUNCTIONS AREA ***

        /// F: output error log
        private async Task logOutput(string msg)
        {
            this.L_Error.Content = msg;
        }
        /// ==========

    /// ==========
    }
}
