using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Linq;
using System;
using OpenCvSharp;
using System.Collections.Generic;
using static System.Threading.Thread;
using static PC80_Tester.General;

namespace PC80_Tester
{
    /// <summary>
    /// Interaction logic for BasicPage1.xaml
    /// </summary>
    public partial class CameraLcdConf
    {
        private enum RB_LCD_SECTION { NON, UP_L, UP_R, LO_L, LO_R }
        private enum LCD_FIG { NON, FIG1, FIG2 }

        private RB_LCD_SECTION rbState = RB_LCD_SECTION.NON;
        private LCD_FIG figState = LCD_FIG.NON;

        bool CanChangeCnPoint;

        public CameraLcdConf()
        {
            InitializeComponent();
            toggleSw.IsChecked = General.camLcd.Opening;
            RingCnTesting.IsActive = false;
        }

        //フォームイベントいろいろ
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = General.camLcd;
            canvasCnPoint.DataContext = State.VmLcdPoint;

            State.VmMainWindow.MainWinEnable = false;
            await Task.Delay(1200);
            State.VmMainWindow.MainWinEnable = true;
            State.SetCamPoint();//ビューモデルの更新 → 座標が表示される
            State.SetCamPropForLcd();
            General.camLcd.Start();
            tbPoint.Visibility = System.Windows.Visibility.Hidden;
            tbHsv.Visibility = System.Windows.Visibility.Hidden;
            rbNon.IsChecked = true;
            CanChangeCnPoint = true;

        }

        private async void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            CanChangeCnPoint = false;
            rbNon.IsChecked = true;

            await General.camLcd.Stop();
            buttonBin.Background = General.OffBrush;

            BinSw = false;

            buttonBin.IsEnabled = true;

            //TODO:
            //LEDを全消灯させる処理
            General.ResetIo();
            State.SetCamPoint();
            await Task.Delay(500);
        }

        private void im_MouseLeave(object sender, MouseEventArgs e)
        {
            tbPoint.Visibility = System.Windows.Visibility.Hidden;
            tbHsv.Visibility = System.Windows.Visibility.Hidden;
            General.camLcd.FlagHsv = false;
        }

        private void im_MouseEnter(object sender, MouseEventArgs e)
        {
            General.camLcd.FlagHsv = true;
            tbHsv.Visibility = System.Windows.Visibility.Visible;
        }

        private void im_MouseMove(object sender, MouseEventArgs e)
        {
            tbPoint.Visibility = System.Windows.Visibility.Visible;
            Point point = e.GetPosition(im);
            tbPoint.Text = "XY=" + ((int)(point.X)).ToString() + "/" + ((int)(point.Y)).ToString();

            General.camLcd.PointX = (int)point.X;
            General.camLcd.PointY = (int)point.Y;

            tbHsv.Text = "HSV=" + General.camLcd.Hdata.ToString() + "," + General.camLcd.Sdata.ToString() + "," + General.camLcd.Vdata.ToString();
        }


        private void SaveLcdPoint()
        {
            State.CamPropLcd.X_UP_L = State.VmLcdPoint.X_UpLeft;
            State.CamPropLcd.Y_UP_L = State.VmLcdPoint.Y_UpLeft;
            State.CamPropLcd.W_UP_L = State.VmLcdPoint.W_UpLeft;
            State.CamPropLcd.H_UP_L = State.VmLcdPoint.H_UpLeft;

            State.CamPropLcd.X_UP_R = State.VmLcdPoint.X_UpRight;
            State.CamPropLcd.Y_UP_R = State.VmLcdPoint.Y_UpRight;
            State.CamPropLcd.W_UP_R = State.VmLcdPoint.W_UpRight;
            State.CamPropLcd.H_UP_R = State.VmLcdPoint.H_UpRight;

            State.CamPropLcd.X_LO_L = State.VmLcdPoint.X_LoLeft;
            State.CamPropLcd.Y_LO_L = State.VmLcdPoint.Y_LoLeft;
            State.CamPropLcd.W_LO_L = State.VmLcdPoint.W_LoLeft;
            State.CamPropLcd.H_LO_L = State.VmLcdPoint.H_LoLeft;

            State.CamPropLcd.X_LO_R = State.VmLcdPoint.X_LoRight;
            State.CamPropLcd.Y_LO_R = State.VmLcdPoint.Y_LoRight;
            State.CamPropLcd.W_LO_R = State.VmLcdPoint.W_LoRight;
            State.CamPropLcd.H_LO_R = State.VmLcdPoint.H_LoRight;
        }

        private void SaveCameraPropForLcd()
        {
            State.CamPropLcd.BinLevel = General.camLcd.BinLevel;
            State.CamPropLcd.Opening = General.camLcd.Opening;
            State.CamPropLcd.OpenCnt = General.camLcd.OpenCnt;
            State.CamPropLcd.CloseCnt = General.camLcd.CloseCnt;
            State.CamPropLcd.Brightness = General.camLcd.Brightness;
            State.CamPropLcd.Contrast = General.camLcd.Contrast;
            State.CamPropLcd.Hue = General.camLcd.Hue;
            State.CamPropLcd.Saturation = General.camLcd.Saturation;
            State.CamPropLcd.Sharpness = General.camLcd.Sharpness;
            State.CamPropLcd.Gamma = General.camLcd.Gamma;
            State.CamPropLcd.Gain = General.camLcd.Gain;
            State.CamPropLcd.Exposure = General.camLcd.Exposure;
            State.CamPropLcd.Whitebalance = General.camLcd.Wb;
            State.CamPropLcd.Theta = General.camLcd.Theta;
        }


        //LED調整用
        bool BinSw = false;
        private void buttonBin_Click(object sender, RoutedEventArgs e)
        {
            General.camLcd.ResetFlag();
            BinSw = !BinSw;
            General.camLcd.FlagBin = BinSw;
            buttonBin.Background = BinSw ? Brushes.DodgerBlue : Brushes.Transparent;
        }


        //カメラプロパティ調整いろいろ
        private void toggleSw_Checked(object sender, RoutedEventArgs e)
        {
            General.camLcd.Opening = true;
        }

        private void toggleSw_Unchecked(object sender, RoutedEventArgs e)
        {
            General.camLcd.Opening = false;
        }

        //コネクタ画像取得いろいろ

        private void SetCnPointCanvas()
        {
            switch (rbState)
            {
                case RB_LCD_SECTION.UP_L:
                    canvasUpLeft.IsEnabled = true;
                    canvasUpRight.IsEnabled = false;
                    canvasLoLeft.IsEnabled = false;
                    canvasLoRight.IsEnabled = false;
                    break;
                case RB_LCD_SECTION.UP_R:
                    canvasUpLeft.IsEnabled = false;
                    canvasUpRight.IsEnabled = true;
                    canvasLoLeft.IsEnabled = false;
                    canvasLoRight.IsEnabled = false;
                    break;
                case RB_LCD_SECTION.LO_L:
                    canvasUpLeft.IsEnabled = false;
                    canvasUpRight.IsEnabled = false;
                    canvasLoLeft.IsEnabled = true;
                    canvasLoRight.IsEnabled = false;
                    break;
                case RB_LCD_SECTION.LO_R:
                    canvasUpLeft.IsEnabled = false;
                    canvasUpRight.IsEnabled = false;
                    canvasLoLeft.IsEnabled = false;
                    canvasLoRight.IsEnabled = true;
                    break;
                case RB_LCD_SECTION.NON:
                    canvasUpLeft.IsEnabled = false;
                    canvasUpRight.IsEnabled = false;
                    canvasLoLeft.IsEnabled = false;
                    canvasLoRight.IsEnabled = false;
                    return;
            }

            SetRect();
        }

        private void SetRect()
        {
            int x = 0, y = 0, w = 0, h = 0;

            switch (rbState)
            {
                case RB_LCD_SECTION.UP_L:
                    x = State.VmLcdPoint.X_UpLeft;
                    y = State.VmLcdPoint.Y_UpLeft;
                    w = State.VmLcdPoint.W_UpLeft;
                    h = State.VmLcdPoint.H_UpLeft;
                    break;
                case RB_LCD_SECTION.UP_R:
                    x = State.VmLcdPoint.X_UpRight;
                    y = State.VmLcdPoint.Y_UpRight;
                    w = State.VmLcdPoint.W_UpRight;
                    h = State.VmLcdPoint.H_UpRight;
                    break;
                case RB_LCD_SECTION.LO_L:
                    x = State.VmLcdPoint.X_LoLeft;
                    y = State.VmLcdPoint.Y_LoLeft;
                    w = State.VmLcdPoint.W_LoLeft;
                    h = State.VmLcdPoint.H_LoLeft;
                    break;
                case RB_LCD_SECTION.LO_R:
                    x = State.VmLcdPoint.X_LoRight;
                    y = State.VmLcdPoint.Y_LoRight;
                    w = State.VmLcdPoint.W_LoRight;
                    h = State.VmLcdPoint.H_LoRight;
                    break;
            }

            General.camLcd.MakeFrame = (img) =>
            {
                img.Rectangle(new CvRect(x, y, w, h), CvColor.Red, 1);
            };

            if (rbState == RB_LCD_SECTION.NON)
                General.camLcd.ResetFlag();//検査枠を非表示に戻す
            else
                General.camLcd.FlagFrame = true;

        }

        private void GetTemplatePic()
        {
            //camの画像を取得する処理
            if (!General.camLcd.GetPic())
            {
                MessageBox.Show("画像取得に失敗しました");
            }
            IplImage src = General.camLcd.imageForTest;

            switch (figState)
            {
                case LCD_FIG.FIG1:
                    var tmpUpLeft1 = General.trimming(src, State.VmLcdPoint.X_UpLeft, State.VmLcdPoint.Y_UpLeft, State.VmLcdPoint.W_UpLeft, State.VmLcdPoint.H_UpLeft);
                    tmpUpLeft1.SaveImage(Constants.filePath_Fig1TempUpLeft);
                    var tmpUpRight1 = General.trimming(src, State.VmLcdPoint.X_UpRight, State.VmLcdPoint.Y_UpRight, State.VmLcdPoint.W_UpRight, State.VmLcdPoint.H_UpRight);
                    tmpUpRight1.SaveImage(Constants.filePath_Fig1TempUpRight);
                    var tmpLoLeft1 = General.trimming(src, State.VmLcdPoint.X_LoLeft, State.VmLcdPoint.Y_LoLeft, State.VmLcdPoint.W_LoLeft, State.VmLcdPoint.H_LoLeft);
                    tmpLoLeft1.SaveImage(Constants.filePath_Fig1TempLoLeft);
                    var tmpLoRight1 = General.trimming(src, State.VmLcdPoint.X_LoRight, State.VmLcdPoint.Y_LoRight, State.VmLcdPoint.W_LoRight, State.VmLcdPoint.H_LoRight);
                    tmpLoRight1.SaveImage(Constants.filePath_Fig1TempLoRight);
                    break;
                case LCD_FIG.FIG2:
                    var tmpUpLeft2 = General.trimming(src, State.VmLcdPoint.X_UpLeft, State.VmLcdPoint.Y_UpLeft, State.VmLcdPoint.W_UpLeft, State.VmLcdPoint.H_UpLeft);
                    tmpUpLeft2.SaveImage(Constants.filePath_Fig2TempUpLeft);
                    var tmpUpRight2 = General.trimming(src, State.VmLcdPoint.X_UpRight, State.VmLcdPoint.Y_UpRight, State.VmLcdPoint.W_UpRight, State.VmLcdPoint.H_UpRight);
                    tmpUpRight2.SaveImage(Constants.filePath_Fig2TempUpRight);
                    var tmpLoLeft2 = General.trimming(src, State.VmLcdPoint.X_LoLeft, State.VmLcdPoint.Y_LoLeft, State.VmLcdPoint.W_LoLeft, State.VmLcdPoint.H_LoLeft);
                    tmpLoLeft2.SaveImage(Constants.filePath_Fig2TempLoLeft);
                    var tmpLoRight2 = General.trimming(src, State.VmLcdPoint.X_LoRight, State.VmLcdPoint.Y_LoRight, State.VmLcdPoint.W_LoRight, State.VmLcdPoint.H_LoRight);
                    tmpLoRight2.SaveImage(Constants.filePath_Fig2TempLoRight);
                    break;
            }

            General.camLcd.ResetFlag();
        }

        private void rbUpLeft_Checked(object sender, RoutedEventArgs e)
        {
            rbState = RB_LCD_SECTION.UP_L;
            SetCnPointCanvas();
        }

        private void rbUpRight_Checked(object sender, RoutedEventArgs e)
        {
            rbState = RB_LCD_SECTION.UP_R;
            SetCnPointCanvas();
        }

        private void rbLoLeft_Checked(object sender, RoutedEventArgs e)
        {
            rbState = RB_LCD_SECTION.LO_L;
            SetCnPointCanvas();
        }

        private void rbLoRight_Checked(object sender, RoutedEventArgs e)
        {
            rbState = RB_LCD_SECTION.LO_R;
            SetCnPointCanvas();
        }



        private void UpLeft_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if (!CanChangeCnPoint) return;
            SetRect();
        }

        private void UpRight_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if (!CanChangeCnPoint) return;
            SetRect();

        }

        private void LoLeft_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if (!CanChangeCnPoint) return;
            SetRect();

        }

        private void LoRight_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if (!CanChangeCnPoint) return;
            SetRect();

        }



        private async void buttonSaveLcd_Click(object sender, RoutedEventArgs e)
        {
            if (figState == LCD_FIG.NON) return;

            buttonSaveLcd.Background = Brushes.DodgerBlue;
            //保存する処理
            SaveLcdPoint();//全セクションの座標を保存（保存ボタン押しで毎回保存する）
            GetTemplatePic();//セクション毎のテンプレート画像を保存
            SaveCameraPropForLcd();//カメラプロパティ（全セクション共通）保存ボタン押しで毎回保存する

            General.PlaySound(General.soundSuccess);
            await Task.Delay(150);
            buttonSaveLcd.Background = Brushes.Transparent;
        }

        private void rbNon_Checked(object sender, RoutedEventArgs e)
        {
            rbState = RB_LCD_SECTION.NON;
            SetCnPointCanvas();
        }

        bool lcdTesting;
        private async void buttonLcdTest_Click(object sender, RoutedEventArgs e)
        {
            if (figState == LCD_FIG.NON) return;
            if (lcdTesting) return;

            rbNon.IsChecked = true;

            lcdTesting = true;
            RingCnTesting.IsActive = true;
            await TestLcd.CheckLcd(figState == LCD_FIG.FIG1 ? TestLcd.FIG_NAME.FIG1 : TestLcd.FIG_NAME.FIG2);
            State.VmLcdPoint.ResultUpLeft = TestLcd.ListSpecs.Find(l => l.name == TestLcd.NAME.UpLeft).一致率.ToString("F2");
            State.VmLcdPoint.ResultUpRight = TestLcd.ListSpecs.Find(l => l.name == TestLcd.NAME.UpRight).一致率.ToString("F2");
            State.VmLcdPoint.ResultLoLeft = TestLcd.ListSpecs.Find(l => l.name == TestLcd.NAME.LoLeft).一致率.ToString("F2");
            State.VmLcdPoint.ResultLoRight = TestLcd.ListSpecs.Find(l => l.name == TestLcd.NAME.LoRight).一致率.ToString("F2");
            RingCnTesting.IsActive = false;
            lcdTesting = false;
        }


        private async void buttonFig1_Click(object sender, RoutedEventArgs e)
        {
            buttonFig1.Background = OnBrush;
            buttonFig2.Background = OffBrush;
            if (!Flags.PowOn)
            {
                General.PowSupply_TestMode(true);
                await Task.Delay(1000);
            }
            Target.SendData(Constants.Fig1Command);
            figState = LCD_FIG.FIG1;
        }

        private async void buttonFig2_Click(object sender, RoutedEventArgs e)
        {
            buttonFig1.Background = OffBrush;
            buttonFig2.Background = OnBrush;
            if (!Flags.PowOn)
            {
                General.PowSupply_TestMode(true);
                await Task.Delay(1000);
            }
            Target.SendData(Constants.Fig2Command);
            figState = LCD_FIG.FIG2;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TestLcd.CheckLcdDEMO();
        }
    }
}
