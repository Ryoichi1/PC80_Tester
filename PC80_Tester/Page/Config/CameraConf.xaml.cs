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
    public partial class CameraConf
    {
        private enum RB_CN { NON, UP_L, UP_R, LO_L, LO_R }
        private enum LCD_FIG { NON, FIG1, FIG2 }

        private RB_CN rbState = RB_CN.NON;
        private LCD_FIG figState = LCD_FIG.NON;

        bool CanChangeCnPoint;

        public CameraConf()
        {
            InitializeComponent();
            this.DataContext = General.cam;
            canvasCnPoint.DataContext = State.VmLcdPoint;
            canvasLdPoint.DataContext = State.VmLedPoint;
            toggleSw.IsChecked = General.cam.Opening;
            RingCnTesting.IsActive = false;
        }

        //フォームイベントいろいろ
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LedOn = false;
            State.VmMainWindow.MainWinEnable = false;
            await Task.Delay(1200);
            State.VmMainWindow.MainWinEnable = true;
            State.SetCamPoint();
            General.cam.Start();
            tbPoint.Visibility = System.Windows.Visibility.Hidden;
            tbHsv.Visibility = System.Windows.Visibility.Hidden;
            rbNon.IsChecked = true;
            CanChangeCnPoint = true;

        }

        private async void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            CanChangeCnPoint = false;
            rbNon.IsChecked = true;

            await General.cam.Stop();
            buttonLedOn.Background = General.OffBrush;
            buttonLabeling.Background = General.OffBrush;
            buttonBin.Background = General.OffBrush;
            resetView();

            FlagLabeling = false;
            BinSw = false;

            buttonLabeling.IsEnabled = true;
            buttonBin.IsEnabled = true;
            canvasLdPoint.IsEnabled = true;

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
            General.cam.FlagHsv = false;
        }

        private void im_MouseEnter(object sender, MouseEventArgs e)
        {
            General.cam.FlagHsv = true;
            tbHsv.Visibility = System.Windows.Visibility.Visible;
        }

        private void im_MouseMove(object sender, MouseEventArgs e)
        {
            tbPoint.Visibility = System.Windows.Visibility.Visible;
            Point point = e.GetPosition(im);
            tbPoint.Text = "XY=" + ((int)(point.X)).ToString() + "/" + ((int)(point.Y)).ToString();

            General.cam.PointX = (int)point.X;
            General.cam.PointY = (int)point.Y;

            tbHsv.Text = "HSV=" + General.cam.Hdata.ToString() + "," + General.cam.Sdata.ToString() + "," + General.cam.Vdata.ToString();
        }



        //データ保存いろいろ
        private void resetView()
        {
            State.VmLedPoint.LED1 = "";
            State.VmLedPoint.LED2 = "";
            State.VmLedPoint.LED3 = "";
            State.VmLedPoint.LED4 = "";

            State.VmLedPoint.LED1Lum = "";
            State.VmLedPoint.LED2Lum = "";
            State.VmLedPoint.LED3Lum = "";
            State.VmLedPoint.LED4Lum = "";

            State.VmLedPoint.LED1Hue = "";
            State.VmLedPoint.LED2Hue = "";
            State.VmLedPoint.LED3Hue = "";
            State.VmLedPoint.LED4Hue = "";

            State.VmLedPoint.ColLED1Hue = General.OffBrush;
            State.VmLedPoint.ColLED2Hue = General.OffBrush;
            State.VmLedPoint.ColLED3Hue = General.OffBrush;
            State.VmLedPoint.ColLED4Hue = General.OffBrush;
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
            State.CamPropLcd.BinLevel = General.cam.BinLevel;
            State.CamPropLcd.Opening = General.cam.Opening;
            State.CamPropLcd.OpenCnt = General.cam.openCnt;
            State.CamPropLcd.CloseCnt = General.cam.closeCnt;
            State.CamPropLcd.Brightness = General.cam.Brightness;
            State.CamPropLcd.Contrast = General.cam.Contrast;
            State.CamPropLcd.Hue = General.cam.Hue;
            State.CamPropLcd.Saturation = General.cam.Saturation;
            State.CamPropLcd.Sharpness = General.cam.Sharpness;
            State.CamPropLcd.Gamma = General.cam.Gamma;
            State.CamPropLcd.Gain = General.cam.Gain;
            State.CamPropLcd.Exposure = General.cam.Exposure;
            State.CamPropLcd.Whitebalance = General.cam.Wb;
            State.CamPropLcd.Theta = General.cam.Theta;
        }

        private void SaveCameraPropForLed()
        {
            State.CamPropLed.Brightness = General.cam.Brightness;
            State.CamPropLed.Contrast = General.cam.Contrast;
            State.CamPropLed.Hue = General.cam.Hue;
            State.CamPropLed.Saturation = General.cam.Saturation;
            State.CamPropLed.Sharpness = General.cam.Sharpness;
            State.CamPropLed.Gamma = General.cam.Gamma;
            State.CamPropLed.Gain = General.cam.Gain;
            State.CamPropLed.Exposure = General.cam.Exposure;
            State.CamPropLed.Whitebalance = General.cam.Wb;
            State.CamPropLed.Theta = General.cam.Theta;
            State.CamPropLed.BinLevel = General.cam.BinLevel;

            State.CamPropLed.Opening = General.cam.Opening;
            State.CamPropLed.OpenCnt = General.cam.openCnt;
            State.CamPropLed.CloseCnt = General.cam.closeCnt;
        }

        private void SaveLedPoint()
        {
            State.CamPropLed.Led1 = State.VmLedPoint.LED1;
            State.CamPropLed.Led2 = State.VmLedPoint.LED2;
            State.CamPropLed.Led3 = State.VmLedPoint.LED3;
            State.CamPropLed.Led4 = State.VmLedPoint.LED4;
        }

        private void SaveLedLum()
        {
            State.CamPropLed.LumLed1 = Double.Parse(State.VmLedPoint.LED1Lum);
            State.CamPropLed.LumLed2 = Double.Parse(State.VmLedPoint.LED2Lum);
            State.CamPropLed.LumLed3 = Double.Parse(State.VmLedPoint.LED3Lum);
            State.CamPropLed.LumLed4 = Double.Parse(State.VmLedPoint.LED4Lum);
        }


        //LED調整用
        bool BinSw = false;
        private void buttonBin_Click(object sender, RoutedEventArgs e)
        {
            General.cam.ResetFlag();
            BinSw = !BinSw;
            General.cam.FlagBin = BinSw;
            buttonBin.Background = BinSw ? Brushes.DodgerBlue : Brushes.Transparent;

            buttonLabeling.IsEnabled = !BinSw;

        }

        bool CanSaveLedPpint = false;
        private void Labeling()
        {
            Task.Run(() =>
            {
                resetView();
                while (FlagLabeling)
                {
                    if (General.cam.blobs == null) continue;
                    var blobs = General.cam.blobs.Clone();

                    //画面上側のLCD表示が写り込んでしまうため、マスク処理（Y座標240以上（半分から下） のものを抽出）
                    var blobInfo = blobs.Where(b => b.Value.Centroid.Y > 240);

                    //LED1～4の座標を抽出する
                    var blobLed1_4 = blobInfo.OrderBy(b => b.Value.Centroid.X).ToList();

                    if (blobLed1_4.Count() != 4)
                    {
                        resetView();
                        CanSaveLedPpint = false;
                        continue;
                    }

                    CanSaveLedPpint = true;
                    //ビューモデルの更新
                    State.VmLedPoint.LED1 = blobLed1_4[0].Value.Centroid.X.ToString("F0") + "/" + blobLed1_4[0].Value.Centroid.Y.ToString("F0");
                    State.VmLedPoint.LED1Lum = blobLed1_4[0].Value.Area.ToString();

                    State.VmLedPoint.LED2 = blobLed1_4[1].Value.Centroid.X.ToString("F0") + "/" + blobLed1_4[1].Value.Centroid.Y.ToString("F0");
                    State.VmLedPoint.LED2Lum = blobLed1_4[1].Value.Area.ToString();

                    State.VmLedPoint.LED3 = blobLed1_4[2].Value.Centroid.X.ToString("F0") + "/" + blobLed1_4[2].Value.Centroid.Y.ToString("F0");
                    State.VmLedPoint.LED3Lum = blobLed1_4[2].Value.Area.ToString();

                    State.VmLedPoint.LED4 = blobLed1_4[3].Value.Centroid.X.ToString("F0") + "/" + blobLed1_4[3].Value.Centroid.Y.ToString("F0");
                    State.VmLedPoint.LED4Lum = blobLed1_4[3].Value.Area.ToString();

                }
                CanSaveLedPpint = false;
            });
        }

        bool FlagLabeling;
        private void buttonLabeling_Click(object sender, RoutedEventArgs e)
        {
            if (!LedOn) return;

            FlagLabeling = !FlagLabeling;
            SetLabeling();
        }

        void SetLabeling()
        {
            buttonBin.IsEnabled = !FlagLabeling;
            buttonHue.IsEnabled = !FlagLabeling;

            buttonLabeling.Background = FlagLabeling ? General.OnBrush : General.OffBrush;

            if (FlagLabeling)
            {
                General.cam.ResetFlag();
                General.cam.FlagLabeling = true;

                Labeling();
            }
            else
            {
                resetView();
                General.cam.ResetFlag();
            }
        }


        private async void buttonSaveLed_Click(object sender, RoutedEventArgs e)
        {
            if (!CanSaveLedPpint)
                return;
            buttonSaveLed.Background = Brushes.DodgerBlue;

            SaveLedPoint();
            SaveLedLum();
            SaveCameraPropForLed();
            General.PlaySound(General.soundSuccess);
            await Task.Delay(150);
            buttonSaveLed.Background = Brushes.Transparent;
        }

        bool LedOn;
        private async void buttonLedOn_Click(object sender, RoutedEventArgs e)
        {
            rbNon.IsChecked = true;
            figState = LCD_FIG.NON;


            State.SetCamPropForLed();//LEDチェック用にカメラプロパティを切り替える
            LedOn = !LedOn;

            SetLed();
        }

        async void SetLed()
        {
            if (LedOn)
            {
                buttonLedOn.Background = General.OnBrush;
                await Task.Run(() => General.PowSupply_TestMode(true));//電源投入
                await Task.Delay(2000);
                await Task.Run(() => General.LedAllOn());
            }
            else
            {
                buttonLedOn.Background = General.OffBrush;
                General.ResetIo();
            }
        }

        //カメラプロパティ調整いろいろ
        private void toggleSw_Checked(object sender, RoutedEventArgs e)
        {
            General.cam.Opening = true;
        }

        private void toggleSw_Unchecked(object sender, RoutedEventArgs e)
        {
            General.cam.Opening = false;
        }

        //コネクタ画像取得いろいろ

        private void SetCnPointCanvas()
        {
            switch (rbState)
            {
                case RB_CN.UP_L:
                    canvasUpLeft.IsEnabled = true;
                    canvasUpRight.IsEnabled = false;
                    canvasLoLeft.IsEnabled = false;
                    canvasLoRight.IsEnabled = false;
                    break;
                case RB_CN.UP_R:
                    canvasUpLeft.IsEnabled = false;
                    canvasUpRight.IsEnabled = true;
                    canvasLoLeft.IsEnabled = false;
                    canvasLoRight.IsEnabled = false;
                    break;
                case RB_CN.LO_L:
                    canvasUpLeft.IsEnabled = false;
                    canvasUpRight.IsEnabled = false;
                    canvasLoLeft.IsEnabled = true;
                    canvasLoRight.IsEnabled = false;
                    break;
                case RB_CN.LO_R:
                    canvasUpLeft.IsEnabled = false;
                    canvasUpRight.IsEnabled = false;
                    canvasLoLeft.IsEnabled = false;
                    canvasLoRight.IsEnabled = true;
                    break;
                case RB_CN.NON:
                    canvasUpLeft.IsEnabled = false;
                    canvasUpRight.IsEnabled = false;
                    canvasLoLeft.IsEnabled = false;
                    canvasLoRight.IsEnabled = false;
                    General.cam.ResetFlag();
                    return;
            }
            SetRect();
        }

        private void SetRect()
        {
            int x = 0, y = 0, w = 0, h = 0;

            switch (rbState)
            {
                case RB_CN.UP_L:
                    x = State.VmLcdPoint.X_UpLeft;
                    y = State.VmLcdPoint.Y_UpLeft;
                    w = State.VmLcdPoint.W_UpLeft;
                    h = State.VmLcdPoint.H_UpLeft;
                    break;
                case RB_CN.UP_R:
                    x = State.VmLcdPoint.X_UpRight;
                    y = State.VmLcdPoint.Y_UpRight;
                    w = State.VmLcdPoint.W_UpRight;
                    h = State.VmLcdPoint.H_UpRight;
                    break;
                case RB_CN.LO_L:
                    x = State.VmLcdPoint.X_LoLeft;
                    y = State.VmLcdPoint.Y_LoLeft;
                    w = State.VmLcdPoint.W_LoLeft;
                    h = State.VmLcdPoint.H_LoLeft;
                    break;
                case RB_CN.LO_R:
                    x = State.VmLcdPoint.X_LoRight;
                    y = State.VmLcdPoint.Y_LoRight;
                    w = State.VmLcdPoint.W_LoRight;
                    h = State.VmLcdPoint.H_LoRight;
                    break;
            }

            General.cam.MakeFrame = (img) =>
            {
                img.Rectangle(new CvRect(x, y, w, h), CvColor.Red, 1);
            };
            General.cam.FlagFrame = true;

        }

        private void GetTemplatePic()
        {
            //camの画像を取得する処理
            General.cam.FlagTestPic = true;
            while (General.cam.FlagTestPic) ;
            IplImage src = General.cam.imageForTest;

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

            General.cam.ResetFlag();
        }

        private void ChangeLcdSetting()
        {
            if (ShowHue)
            {
                ShowHue = false;
                SetShowHue();
            }

            if (FlagLabeling)
            {
                FlagLabeling = false;
                SetLabeling();
            }

            if (LedOn)
            {
                LedOn = false;
                SetLed();
            }
        }

        private void rbUpLeft_Checked(object sender, RoutedEventArgs e)
        {
            ChangeLcdSetting();

            rbState = RB_CN.UP_L;
            State.SetCamPropForLcd();
            SetCnPointCanvas();
        }

        private void rbUpRight_Checked(object sender, RoutedEventArgs e)
        {
            ChangeLcdSetting();

            rbState = RB_CN.UP_R;
            State.SetCamPropForLcd();
            SetCnPointCanvas();
        }

        private void rbLoLeft_Checked(object sender, RoutedEventArgs e)
        {
            ChangeLcdSetting();

            rbState = RB_CN.LO_L;
            State.SetCamPropForLcd();
            SetCnPointCanvas();
        }

        private void rbLoRight_Checked(object sender, RoutedEventArgs e)
        {
            ChangeLcdSetting();

            rbState = RB_CN.LO_R;
            State.SetCamPropForLcd();
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
            SaveLcdPoint();//セクション毎の座標を保存
            GetTemplatePic();//セクション毎のテンプレート画像を保存
            SaveCameraPropForLcd();//カメラプロパティ（全セクション共通）保存ボタン押しで毎回取得する

            General.PlaySound(General.soundSuccess);
            await Task.Delay(150);
            buttonSaveLcd.Background = Brushes.Transparent;
        }

        private void rbNon_Checked(object sender, RoutedEventArgs e)
        {
            rbState = RB_CN.NON;
            SetCnPointCanvas();
        }

        bool cnTesting;
        private async void buttonLcdTest_Click(object sender, RoutedEventArgs e)
        {
            if (figState == LCD_FIG.NON) return;
            if (cnTesting) return;

            rbNon.IsChecked = true;

            cnTesting = true;
            RingCnTesting.IsActive = true;
            await TestLcd.CheckLcd(figState == LCD_FIG.FIG1 ? TestLcd.FIG_NAME.FIG1 : TestLcd.FIG_NAME.FIG2);
            State.VmLcdPoint.ResultUpLeft = TestLcd.ListSpecs.Find(l => l.name == TestLcd.NAME.UpLeft).一致率.ToString("F2");
            State.VmLcdPoint.ResultUpRight = TestLcd.ListSpecs.Find(l => l.name == TestLcd.NAME.UpRight).一致率.ToString("F2");
            State.VmLcdPoint.ResultLoLeft = TestLcd.ListSpecs.Find(l => l.name == TestLcd.NAME.LoLeft).一致率.ToString("F2");
            State.VmLcdPoint.ResultLoRight = TestLcd.ListSpecs.Find(l => l.name == TestLcd.NAME.LoRight).一致率.ToString("F2");
            RingCnTesting.IsActive = false;
            cnTesting = false;
            State.VmTestStatus.EnableButtonErrInfo = Visibility.Hidden;//コネクタチェックでエラーになるとエラー詳細ボタンが表示されてしまう！！！
        }

        bool ShowHue;
        private void buttonHue_Click(object sender, RoutedEventArgs e)
        {
            if (!LedOn) return;
            ShowHue = !ShowHue;
            SetShowHue();

        }

        private void SetShowHue()
        {
            buttonBin.IsEnabled = !ShowHue;
            buttonLabeling.IsEnabled = !ShowHue;

            buttonHue.Background = ShowHue ? General.OnBrush : General.OffBrush;

            if (ShowHue)
            {
                General.cam.ResetFlag();

                Task.Run(() =>
                {
                    while (ShowHue)
                    {
                        TestLed.CheckColorForDebug();
                        Sleep(100);
                    }
                    //resetView();
                });
            }
            else
            {
                General.cam.ResetFlag();
            }
        }

        private async void buttonFig1_Click(object sender, RoutedEventArgs e)
        {
            
            if (LedOn) return;

            State.SetCamPropForLcd();
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
            if (LedOn) return;

            State.SetCamPropForLcd();
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
