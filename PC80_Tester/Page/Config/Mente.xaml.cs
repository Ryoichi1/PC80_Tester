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
            labelCamChange.Visibility = Visibility.Hidden;
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
            General.SetRelayForCurrCheck();
        }

        private async void buttonSw1_Click(object sender, RoutedEventArgs e)
        {
            General.SetSw1(true);
            await Task.Delay(300);
            General.SetSw1(false);
        }

        private async void buttonSw2_Click(object sender, RoutedEventArgs e)
        {
            General.SetSw2(true);
            await Task.Delay(300);
            General.SetSw2(false);
        }

        private async void buttonSw3_Click(object sender, RoutedEventArgs e)
        {
            General.SetSw3(true);
            await Task.Delay(300);
            General.SetSw3(false);
        }

        private async void buttonSw4_Click(object sender, RoutedEventArgs e)
        {
            General.SetSw4(true);
            await Task.Delay(300);
            General.SetSw4(false);
        }

        private async void buttonSw5_Click(object sender, RoutedEventArgs e)
        {
            General.SetSw5(true);
            await Task.Delay(300);
            General.SetSw5(false);
        }

        private async void buttonSw6_Click(object sender, RoutedEventArgs e)
        {
            General.SetSw6(true);
            await Task.Delay(300);
            General.SetSw6(false);
        }

        private async void buttonSw7_Click(object sender, RoutedEventArgs e)
        {
            General.SetSw7(true);
            await Task.Delay(300);
            General.SetSw7(false);
        }

        private async void buttonSw8_Click(object sender, RoutedEventArgs e)
        {
            General.SetSw8(true);
            await Task.Delay(300);
            General.SetSw8(false);
        }

        private async void buttonSw9_Click(object sender, RoutedEventArgs e)
        {
            General.SetSw9(true);
            await Task.Delay(300);
            General.SetSw9(false);
        }

        private void Button_Unloaded(object sender, RoutedEventArgs e)
        {
            ResetIo();
        }

        private async void buttonChangeCamNo_Click(object sender, RoutedEventArgs e)
        {
            buttonChangeCamNo.Background = ButtonOnBrush;

            await Task.Run(() =>
            {

                var CurrentCamLcdNum = State.CamPropLcd.CamNumber;
                var CurrentCamLedNum = State.CamPropLed.CamNumber;
                State.CamPropLcd.CamNumber = CurrentCamLedNum;
                State.CamPropLed.CamNumber = CurrentCamLcdNum;

                camLcd = new Camera(State.CamPropLcd.CamNumber, Constants.filePath_CamLedCalFilePath, 640, 360);
                camLcd.InitCamera();

                camLed = new Camera(State.CamPropLed.CamNumber, Constants.filePath_CamLedCalFilePath, 640, 360);
                camLed.InitCamera();

            });

            buttonChangeCamNo.Background = ButtonOffBrush;
            labelCamChange.Visibility = Visibility.Visible;
            await Task.Delay(2000);
            labelCamChange.Visibility = Visibility.Hidden;
        }
    }
}
