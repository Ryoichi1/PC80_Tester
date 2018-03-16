using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace PC80_Tester
{
    /// <summary>
    /// Test.xaml の相互作用ロジック
    /// </summary>
    public partial class Test
    {
        private SolidColorBrush ButtonBrush = new SolidColorBrush();
        private const double ButtonOpacity = 0.3;

        public Test()
        {
            this.InitializeComponent();

            //スタートボタンのデザイン
            ButtonBrush.Color = Colors.DodgerBlue;
            ButtonBrush.Opacity = ButtonOpacity;

            // オブジェクト作成に必要なコードをこの下に挿入します。
            this.DataContext = State.VmTestStatus;
            Canvas検査データ.DataContext = State.VmTestResults;
            CanvasImg1.DataContext = General.cam;
            labelCamera.DataContext = General.cam;

            canvasCommIo.DataContext = State.VmComm;
            canvasCommTarget.DataContext = State.VmComm;

            (FindResource("Blink") as Storyboard).Begin();

            //試験合格後（１項目試験 or 日常点検）と試験不合格後に、検査ステータス以外をクリアするための処理
            State.testCommand.RefreshDataContext = (() =>
            {
                Canvas検査データ.DataContext = State.VmTestResults;
                canvasLedTest.DataContext = State.VmTestResults;
                tbTestLog.DataContext = State.VmTestStatus;
            });


            ラベル貼り付け.RefreshDataContextFromLabelForm = (() =>
            {
                Canvas検査データ.DataContext = State.VmTestResults;
                canvasLedTest.DataContext = State.VmTestResults;
                tbTestLog.DataContext = State.VmTestStatus;

            });


            //ストーリーボードの初期化
            State.testCommand.SbRingLoad = (() => { (FindResource("StoryboardRingLoad") as Storyboard).Begin(); });
            State.testCommand.SbPass = (() => { (FindResource("StoryboardDecision") as Storyboard).Begin(); });
            State.testCommand.SbFail = (() => { (FindResource("StoryboardDecision") as Storyboard).Begin(); });
            State.testCommand.StopButtonBlinkOn = (() => { (FindResource("BlinkStopButton") as Storyboard).Begin(); });
            State.testCommand.StopButtonBlinkOff = (() => { (FindResource("BlinkStopButton") as Storyboard).Stop(); });

            //FWバージョンの表示
            State.VmTestStatus.FwVer = State.Spec.FwVer;
            State.VmTestStatus.FwSum = State.Spec.FwSum;
            State.VmTestStatus.StartButtonContent = "開始";

            State.VmTestStatus.RetryLabelVis = System.Windows.Visibility.Hidden;


        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //エラーインフォメーションページからテストページに遷移する場合は、
            //下記のif文を有効にする
            if (Flags.ShowErrInfo)
            {
                Flags.ShowErrInfo = false;
            }
            else
            {
                //フォームの初期化
                SetUnitTest();
                State.VmTestStatus.DecisionVisibility = System.Windows.Visibility.Hidden;
                State.VmTestStatus.ErrInfoVisibility = System.Windows.Visibility.Hidden;
                State.VmTestStatus.StartButtonContent = Constants.開始;
                State.VmTestStatus.StartButtonEnable = true;
                State.VmTestStatus.TestTime = "00:00";
                State.VmTestStatus.IsActiveRing = false;


                await State.testCommand.StartCheck();
            }
            ButtonStop.Focus();
        }

        private void tbTestLog_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbTestLog.ScrollToEnd();
            //tbTestLog.Select(tbTestLog.Text.Length, 0)
        }

        private void canvasUnitTest_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            CheckBoxUnitTest.IsChecked = false;
            cbUnitTest.SelectedIndex = 0;
        }

        private void SetUnitTest()
        {
            var SelectedItem = State.テスト項目.Where(item => item.Key % 100 == 0);
            var list = new List<string>();
            foreach (var t in SelectedItem)
            {
                list.Add(t.Key.ToString() + "_" + t.Value);
            }
            State.VmTestStatus.UnitTestItems = list;

        }




        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            if (State.VmTestStatus.StartButtonContent == Constants.開始)
            {
                if (!Flags.EnableTestStart)
                {
                    return;
                }

                Flags.Click確認Button = true;
            }
            else if (State.VmTestStatus.StartButtonContent == Constants.停止)
            {
                Flags.ClickStopButton = true;
                State.VmTestStatus.StartButtonEnable = false;
            }
            else if (State.VmTestStatus.StartButtonContent == Constants.確認)
            {
                Flags.Click確認Button = true;
                State.VmTestStatus.StartButtonContent = Constants.開始;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //General.MainIo.SendData1768("*IDN?");
        }

        private void ButtonErrInfo_Click(object sender, RoutedEventArgs e)
        {
            Flags.ShowErrInfo = true;
            State.VmMainWindow.TabIndex = 3;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            State.VmMainWindow.Flyout = true;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            State.VmMainWindow.Flyout = false;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            State.VmMainWindow.FlyoutSrc = Constants.PathVR_L;
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            State.VmMainWindow.FlyoutSrc = Constants.PathVR_R;
        }
    }
}
