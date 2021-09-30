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
        Led []leds = new Led[300];

        int screenLeft = 0;
        int screenTop = 0;
        int screenWidth = 0;
        int screenHeight = 0;
        System.Timers.Timer timerScreenCapture;
        bool isCaptured = false;

        int ledsVertical;
        int ledsHorizontal;
        int ledsTotal = 0;
        double ledsVerticalHeight;
        const int ledsVerticalWidth = 20;
        const int ledsHorizontalHeight = 20;
        double ledsHorizontalWidth;

        double screenHorizontalWidth;
        double screenVerticalHeight;

        int screenCaptureThickness = 40;

        bool sendingLedsToArduino = false;
        bool dataRead = false;

        String[] ports;
        SerialPort port;

        String serialSend = "000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";

        public MainWindow()
        {
            InitializeComponent();

            // Checking for older calibration
            if(Properties.Settings.Default.LedsVertical != "0")
            {
                this.TB_LedsVertical.Text = Properties.Settings.Default.LedsVertical.ToString();
                this.TB_LedsHorizontal.Text = Properties.Settings.Default.LedsHorizontal.ToString();
                this.TB_ScreenStartX.Text = Properties.Settings.Default.ScreenLeft.ToString();
                this.TB_ScreenStartY.Text = Properties.Settings.Default.ScreenTop.ToString();
                this.TB_ScreenEndX.Text = Properties.Settings.Default.ScreenWidth.ToString();
                this.TB_ScreenEndY.Text = Properties.Settings.Default.ScreenHeight.ToString();
            }

            // Initialize the screen capture timer
            timerScreenCapture = new System.Timers.Timer(60); // 20 klatek / sekundę
            timerScreenCapture.Elapsed += screenCapture;
            timerScreenCapture.AutoReset = true;
            timerScreenCapture.Enabled = true;
            timerScreenCapture.Stop();

            // Getting ports and adding to ComboBox
            ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                CB_Serial.Items.Add(port);
                Console.WriteLine(port);
                if (ports[0] != null)
                {
                    CB_Serial.SelectedItem = ports[0];
                }
            }
        }

        // Screen Capture Timer Event
        void screenCapture(Object sender, EventArgs e)
        {
            Bitmap bitmap_Screen = new Bitmap(screenWidth, screenHeight);
            Graphics g = Graphics.FromImage(bitmap_Screen);
            g.CopyFromScreen(screenLeft, screenTop, 0, 0, bitmap_Screen.Size);

            if (!sendingLedsToArduino)
            {
                Dispatcher.BeginInvoke(new Action(() => {
                    I_Screen.Source = ConvertToImage(bitmap_Screen);
                }));
            }

            Dispatcher.BeginInvoke(new Action(() => {
                for (int i = 0; i < ledsTotal; i++)
                {
                    System.Drawing.Rectangle screenRectangle = new System.Drawing.Rectangle(0, 0, 10, 10);
                    if (i < ledsHorizontal)
                        screenRectangle = new System.Drawing.Rectangle((int)(screenWidth - (i+1) * screenHorizontalWidth), (int)(screenHeight - screenCaptureThickness), (int)(screenHorizontalWidth), screenCaptureThickness);
                    else if (i < (ledsHorizontal + ledsVertical))
                        screenRectangle = new System.Drawing.Rectangle((int)screenLeft, (int)(screenHeight - (i - ledsHorizontal + 1) * screenVerticalHeight), screenCaptureThickness, (int)(screenVerticalHeight));
                    else if (i < (ledsHorizontal * 2 + ledsVertical))
                        screenRectangle = new System.Drawing.Rectangle((int)(screenLeft + (i - ledsHorizontal - ledsVertical) * screenHorizontalWidth), (int)(screenTop), (int)(screenHorizontalWidth), screenCaptureThickness);
                    else
                        screenRectangle = new System.Drawing.Rectangle((int)(screenWidth - screenCaptureThickness), (int)(screenTop + (i - ledsVertical - ledsHorizontal * 2) * screenVerticalHeight), screenCaptureThickness, (int)(screenVerticalHeight));

                    string ledColors = leds[i].changeColor(bitmap_Screen.Clone(screenRectangle, bitmap_Screen.PixelFormat), sendingLedsToArduino);

                    if (sendingLedsToArduino)
                        serialSend += ledColors;
                }
            }));

            if (sendingLedsToArduino)
            {
                try
                {
                    port.Write(serialSend + "\n");
                }catch { }

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

        // Timer function to checking leds

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

        // Screen Capture Convert 
        public BitmapImage ConvertToImage(Bitmap src)
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

        // Initializing the leds
        void ledsInitialize(int vertical, int horizontal)
        {
            ledsVertical = vertical;
            ledsHorizontal = horizontal;
            ledsTotal = vertical * 2 + horizontal * 2;

            ledsVerticalHeight = 270 / (double)ledsVertical;
            ledsHorizontalWidth = 480 / (double)ledsHorizontal;
            screenHorizontalWidth = screenWidth / (double)ledsHorizontal;
            screenVerticalHeight = screenHeight / (double)ledsVertical;

            for (int i=0; i<ledsTotal; i++)
            {
                bool isVertical = false;
                double ledHeight = 0;
                double ledWidth = 0;

                if (i < ledsHorizontal)
                {
                    isVertical = false;
                }
                else if (i < (ledsHorizontal + ledsVertical))
                {
                    isVertical = true;
                }
                else if (i < (ledsHorizontal*2 + ledsVertical))
                {
                    isVertical = false;
                }
                else
                {
                    isVertical = true;
                }

                if(isVertical == false)
                {
                    ledWidth = ledsHorizontalWidth;
                    ledHeight = ledsHorizontalHeight;
                }
                else
                {
                    ledWidth = ledsVerticalWidth;
                    ledHeight = ledsVerticalHeight;
                }

                leds[i] = new Led(i, isVertical, ledHeight, ledWidth);

                if (i < ledsHorizontal)
                {
                    // Bottom
                    leds[i].rectangle.Margin = new Thickness(0, 290, (-480 + ledsHorizontalWidth + i*ledsHorizontalWidth*2), 0);
                }
                else if (i < (ledsHorizontal + ledsVertical))
                {
                    // Left
                    leds[i].rectangle.Margin = new Thickness(0, 0, 500, (-270 + ledsVerticalHeight + (i-ledsHorizontal)*ledsVerticalHeight*2));
                }
                else if (i < (ledsHorizontal * 2 + ledsVertical))
                {
                    // Top
                    leds[i].rectangle.Margin = new Thickness((-480 + ledsHorizontalWidth + (i - ledsHorizontal - ledsVertical)*ledsHorizontalWidth * 2), 0, 0, 290);
                }
                else
                {
                    // Right
                    leds[i].rectangle.Margin = new Thickness(500, (-270 + ledsVerticalHeight + (i - ledsHorizontal*2 - ledsVertical) * ledsVerticalHeight * 2), 0, 0);
                }

                MainGrid.Children.Add(leds[i].rectangle);
            }

            Dispatcher.BeginInvoke(new Action(() => {
                this.L_Error.Content = "Leds initialized as " + ledsVertical + " Vertical, " + ledsHorizontal + " Horizontal!";
            }));
        }

        // Setting the leds
        private void B_Calibrate_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() => {
                this.L_Error.Content = "";
            }));

            try
            {
                if (this.TB_LedsVertical.Text.ToString() != "" && Convert.ToInt32(this.TB_LedsVertical.Text.ToString()) > 0 && Convert.ToInt32(this.TB_LedsVertical.Text.ToString()) < 50 &&
                    this.TB_LedsHorizontal.Text.ToString() != "" && Convert.ToInt32(this.TB_LedsHorizontal.Text.ToString()) > 0 && Convert.ToInt32(this.TB_LedsHorizontal.Text.ToString()) < 50 &&
                    this.TB_ScreenStartX.Text.ToString() != "" && Convert.ToInt32(this.TB_ScreenStartX.Text.ToString()) >= 0 &&
                    this.TB_ScreenStartY.Text.ToString() != "" && Convert.ToInt32(this.TB_ScreenStartY.Text.ToString()) >= 0 &&
                    this.TB_ScreenEndX.Text.ToString() != "" && Convert.ToInt32(this.TB_ScreenEndX.Text.ToString()) > 0 &&
                    this.TB_ScreenEndY.Text.ToString() != "" && Convert.ToInt32(this.TB_ScreenEndY.Text.ToString()) > 0
                    )
                {
                    // Saving calibration data
                    Properties.Settings.Default.LedsVertical = this.TB_LedsVertical.Text.ToString();
                    Properties.Settings.Default.LedsHorizontal = this.TB_LedsHorizontal.Text.ToString();
                    Properties.Settings.Default.ScreenWidth = this.TB_ScreenEndX.Text.ToString();
                    Properties.Settings.Default.ScreenHeight = this.TB_ScreenEndY.Text.ToString();
                    Properties.Settings.Default.ScreenLeft = this.TB_ScreenStartX.Text.ToString();
                    Properties.Settings.Default.ScreenTop = this.TB_ScreenStartY.Text.ToString();
                    Properties.Settings.Default.Save();

                    // Getting the Screen Coords
                    screenWidth = Convert.ToInt32(this.TB_ScreenEndX.Text.ToString());
                    screenHeight = Convert.ToInt32(this.TB_ScreenEndY.Text.ToString());
                    screenLeft = Convert.ToInt32(this.TB_ScreenStartX.Text.ToString());
                    screenTop = Convert.ToInt32(this.TB_ScreenStartY.Text.ToString());

                    ledsInitialize(Convert.ToInt32(this.TB_LedsVertical.Text.ToString()), Convert.ToInt32(this.TB_LedsHorizontal.Text.ToString()));
                }
                else
                {
                    Dispatcher.BeginInvoke(new Action(() => {
                        this.L_Error.Content = "Leds/screen coords data input error!";
                    }));
                }
            }
            catch (Exception exception)
            {
                Dispatcher.BeginInvoke(new Action(() => {
                    this.L_Error.Content = exception.ToString();
                }));
            }
            
        }

        private void B_Capture_Click(object sender, RoutedEventArgs e)
        {
            if (isCaptured)
            {
                timerScreenCapture.Stop();
                isCaptured = false;
                Dispatcher.BeginInvoke(new Action(() => {
                    B_Calibrate.IsEnabled = true;
                    B_TestLed.IsEnabled = true;
                    B_Capture.Content = "Capture";
                    I_Screen.Opacity = 0d;
                    B_SendLeds.IsEnabled = false;
                }));
            }
            else
            {
                if (screenHeight == 0 && screenLeft == 0 && screenTop == 0 && screenWidth == 0)
                {
                    Dispatcher.BeginInvoke(new Action(() => {
                        this.L_Error.Content = "Calibrate your application first!";
                    }));
                }
                else
                {
                    timerScreenCapture.Start();
                    isCaptured = true;
                    Dispatcher.BeginInvoke(new Action(() => {
                        B_Calibrate.IsEnabled = false;
                        B_TestLed.IsEnabled = false;
                        B_Capture.Content = "Stop";
                        I_Screen.Opacity = 1d;
                        B_SendLeds.IsEnabled = true;
                    }));
                }
            }
        }

        private void B_TestLed_Click(object sender, RoutedEventArgs e)
        {
            if(Convert.ToInt32(TB_TestLed.Text) < ledsTotal && Convert.ToInt32(TB_TestLed.Text) >= 0)
            {
                 int ledToTestIndex = Convert.ToInt32(TB_TestLed.Text);

                Dispatcher.BeginInvoke(new Action(() => {
                    this.L_Error.Content = "Testing led " + ledToTestIndex;
                }));

                SolidColorBrush colorOriginal = new SolidColorBrush(System.Windows.Media.Color.FromRgb(leds[ledToTestIndex].r, leds[ledToTestIndex].g, leds[ledToTestIndex].b));
                leds[ledToTestIndex].rectangle.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));

                System.Timers.Timer timerLedCheck;
                timerLedCheck = new System.Timers.Timer(1000);
                timerLedCheck.Elapsed += ledCheckReturn;
                timerLedCheck.AutoReset = false;
                timerLedCheck.Enabled = true;
                timerLedCheck.Start();
            }
            else
            {
                Dispatcher.BeginInvoke(new Action(() => {
                    this.L_Error.Content = "Test led error, check your calibration!";
                }));
            }
        }

        private void B_SendLeds_Click(object sender, RoutedEventArgs e)
        {
            if(sendingLedsToArduino == true)
            {
                sendingLedsToArduino = false;
                Dispatcher.BeginInvoke(new Action(() => {
                    R_SendLeds.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 0));
                    B_SendLeds.Content = "Send Leds";
                    B_Capture.IsEnabled = true;
                    CB_Serial.IsEnabled = true;
                }));
                port.Close();
                Dispatcher.BeginInvoke(new Action(() => {
                    L_Error.Content = "Closed connection";
                }));
            }
            else
            {
                sendingLedsToArduino = true;
                Dispatcher.BeginInvoke(new Action(() => {
                    R_SendLeds.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 255, 0));
                    B_SendLeds.Content = "Stop Leds";
                    B_Capture.IsEnabled = false;
                    CB_Serial.IsEnabled = false;
                }));
                string selectedPort = CB_Serial.SelectedItem.ToString();
                port = new SerialPort(selectedPort, 250000, Parity.None, 8, StopBits.One);
                port.Open();
                Dispatcher.BeginInvoke(new Action(() => {
                    L_Error.Content = "Tried to connect on " + selectedPort;
                }));
            }
        }
    }
}
