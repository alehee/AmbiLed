using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace PcLedVisualization
{
    public partial class LedLog : Window
    {
        public LedLog()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.Hide();
            e.Cancel = true;
            base.OnClosing(e);
        }

        public void addLog(string log, bool end)
        {
            if (end)
            {
                Dispatcher.BeginInvoke(new Action(() => {
                    TB_Log.Text = "Leds info: ";
                }));
            }
            else
            {
                Dispatcher.BeginInvoke(new Action(() => {
                    TB_Log.Text = TB_Log.Text.ToString() + log + ", ";
                }));
            }
        }
    }
}
