using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using static PC80_Tester.General;

namespace PC80_Tester
{
    /// <summary>
    /// Interaction logic for BasicPage1.xaml
    /// </summary>
    public partial class Mente
    {
        private SolidColorBrush ButtonOnBrush = new SolidColorBrush();
        private SolidColorBrush ButtonOffBrush = new SolidColorBrush();
        private const double ButtonOpacity = 0.4;

        public Mente()
        {
            InitializeComponent();
            CanvasComm.DataContext = State.VmComm;

            ButtonOnBrush.Color = Colors.DodgerBlue;
            ButtonOffBrush.Color = Colors.Transparent;
            ButtonOnBrush.Opacity = ButtonOpacity;
            ButtonOffBrush.Opacity = ButtonOpacity;

            SetCommand();

        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            buttonPow.Background = Brushes.Transparent;
            General.PowSupply_TestMode(false);
        }


        bool FlagPow;
        private void buttonPow_Click(object sender, RoutedEventArgs e)
        {
            if (FlagPow)
            {
                General.PowSupply_TestMode(false);
                buttonPow.Background = ButtonOffBrush;
            }
            else
            {
                General.PowSupply_TestMode(true);
                buttonPow.Background = ButtonOnBrush;
            }

            FlagPow = !FlagPow;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            State.VmComm.TX_IO = "";
            State.VmComm.RX_IO = "";
        }


        private void buttonStamp_Click(object sender, RoutedEventArgs e)
        {
            buttonStamp.Background = ButtonOnBrush;
            General.StampOn();
            buttonStamp.Background = ButtonOffBrush;
        }


        private void SetCommand()
        {
            State.Command.CmdList.ForEach(m =>
            {
                cbCommand.Items.Add(m);
            });

        }

        private void buttonSend_Click(object sender, RoutedEventArgs e)
        {
            if (cbCommand.SelectedIndex == -1) return;
            Target.SendData(cbCommand.SelectedItem.ToString());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ResetIo();
            SetRelayForP130Check();

        }
    }
}
