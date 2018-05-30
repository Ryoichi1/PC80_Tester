using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using OpenCvSharp;
using System.Windows.Input;
using System.Linq;
using System;
using System.Collections.Generic;
using static System.Threading.Thread;

namespace PC80_Tester
{
    /// <summary>
    /// Interaction logic for BasicPage1.xaml
    /// </summary>
    public partial class CameraLedConf
    {
        public CameraLedConf()
        {
            InitializeComponent();
            this.DataContext = General.camLed;
        }


        private void resetView()
        {
            buttonLedOnOff.Background = General.OffBrush;
            buttonBin.Background = General.OffBrush;
            buttonGrid.Background = General.OffBrush;
            buttonLight.Background = General.OffBrush;

            State.VmLedPoint.LED1 = "";
            State.VmLedPoint.LED2 = "";
            State.VmLedPoint.LED3 = "";
            State.VmLedPoint.LED4 = "";

            State.VmLedPoint.LED1Hue = "";
            State.VmLedPoint.LED2Hue = "";
            State.VmLedPoint.LED3Hue = "";
            State.VmLedPoint.LED4Hue = "";
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            canvasLdPoint.DataContext = State.VmLedPoint;

            State.VmMainWindow.MainWinEnable = false;
            await Task.Delay(1200);
            State.VmMainWindow.MainWinEnable = true;
            State.SetCamPoint();//ビューモデルの更新 → 座標が表示される
            General.camLed.Start();
            State.SetCamPropForLed();
            tbPoint.Visibility = System.Windows.Visibility.Hidden;
            tbHsv.Visibility = System.Windows.Visibility.Hidden;
        }

        private async void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            LedOn = false;
            FlagCheckCol = false;
            FlagLight = false;

            BinSw = false;
            GridSw = false;

            buttonBin.IsEnabled = true;
            buttonGrid.IsEnabled = true;
            canvasLdPoint.IsEnabled = true;

            General.SetLight(false);
            General.camLed.ResetFlag();
            await General.camLed.Stop();
            resetView();

            //TODO:
            //LEDを全消灯させる処理
            General.ResetIo();
            State.SetCamPoint();
            await Task.Delay(500);
        }



        private async void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            buttonSave.Background = Brushes.DodgerBlue;
            SaveCameraProp();
            await Task.Delay(150);
            General.PlaySound(General.soundSuccess);
            buttonSave.Background = Brushes.Transparent;
        }

        private void SaveCameraProp()
        {
            //すべてのデータを保存する
            State.CamPropLed.Brightness = General.camLed.Brightness;
            State.CamPropLed.Contrast = General.camLed.Contrast;
            State.CamPropLed.Hue = General.camLed.Hue;
            State.CamPropLed.Saturation = General.camLed.Saturation;
            State.CamPropLed.Sharpness = General.camLed.Sharpness;
            State.CamPropLed.Gamma = General.camLed.Gamma;
            State.CamPropLed.Gain = General.camLed.Gain;
            State.CamPropLed.Exposure = General.camLed.Exposure;
            State.CamPropLed.Whitebalance = General.camLed.Wb;
            State.CamPropLed.Theta = General.camLed.Theta;
            State.CamPropLed.BinLevel = General.camLed.BinLevel;

            State.CamPropLed.Opening = General.camLed.Opening;
            State.CamPropLed.OpenCnt = General.camLed.OpenCnt;
            State.CamPropLed.CloseCnt = General.camLed.CloseCnt;


            State.CamPropLed.PointLed1 = State.VmLedPoint.LED1;
            State.CamPropLed.PointLed2 = State.VmLedPoint.LED2;
            State.CamPropLed.PointLed3 = State.VmLedPoint.LED3;
            State.CamPropLed.PointLed4 = State.VmLedPoint.LED4;

            State.CamPropLed.LumLed1 = State.VmLedPoint.LED1Lum;
            State.CamPropLed.LumLed2 = State.VmLedPoint.LED2Lum;
            State.CamPropLed.LumLed3 = State.VmLedPoint.LED3Lum;
            State.CamPropLed.LumLed4 = State.VmLedPoint.LED4Lum;

            State.CamPropLed.HueLed1 = State.VmLedPoint.LED1Hue;
            State.CamPropLed.HueLed2 = State.VmLedPoint.LED2Hue;
            State.CamPropLed.HueLed3 = State.VmLedPoint.LED3Hue;
            State.CamPropLed.HueLed4 = State.VmLedPoint.LED4Hue;

        }

        private CvPoint GetCenter(string data)
        {
            var re = data.Split('/').ToArray();
            return new CvPoint(Int32.Parse(re[0]), Int32.Parse(re[1]));
        }

        private void im_MouseLeave(object sender, MouseEventArgs e)
        {
            tbPoint.Visibility = System.Windows.Visibility.Hidden;
            tbHsv.Visibility = System.Windows.Visibility.Hidden;
            General.camLed.FlagHsv = false;
        }

        private void im_MouseEnter(object sender, MouseEventArgs e)
        {
            General.camLed.FlagHsv = true;
            tbHsv.Visibility = System.Windows.Visibility.Visible;
        }

        private void im_MouseMove(object sender, MouseEventArgs e)
        {
            tbPoint.Visibility = System.Windows.Visibility.Visible;
            Point point = e.GetPosition(im);
            tbPoint.Text = "XY=" + ((int)(point.X)).ToString() + "/" + ((int)(point.Y)).ToString();

            General.camLed.PointX = (int)point.X;
            General.camLed.PointY = (int)point.Y;

            tbHsv.Text = "HSV=" + General.camLed.Hdata.ToString() + "," + General.camLed.Sdata.ToString() + "," + General.camLed.Vdata.ToString();
        }

        bool LedOn;
        private async void buttonLedOnOff_Click(object sender, RoutedEventArgs e)
        {
            LedOn = !LedOn;
            if (LedOn)
            {
                buttonLedOnOff.Background = General.OnBrush;
                await Task.Run(() =>
                {
                    General.PowSupply_TestMode(true);
                    Sleep(1000);
                    General.LedAllOn();
                });
            }
            else
            {
                buttonLedOnOff.Background = General.OffBrush;
                General.ResetIo();
            }
        }

        bool GridSw = false;
        private void buttonGrid_Click(object sender, RoutedEventArgs e)
        {
            General.camLed.ResetFlag();
            GridSw = !GridSw;
            General.camLed.FlagGrid = GridSw;
            buttonGrid.Background = GridSw ? Brushes.DodgerBlue : Brushes.Transparent;

            buttonBin.IsEnabled = !GridSw;
            canvasLdPoint.IsEnabled = !GridSw;
        }

        bool BinSw = false;
        private void buttonBin_Click(object sender, RoutedEventArgs e)
        {
            General.camLed.ResetFlag();
            BinSw = !BinSw;
            General.camLed.FlagBin = BinSw;
            buttonBin.Background = BinSw ? Brushes.DodgerBlue : Brushes.Transparent;

            buttonGrid.IsEnabled = !BinSw;
            canvasLdPoint.IsEnabled = !BinSw;
        }


        public enum SEG_NAME
        {
            LED1, LED2, LED3, LED4, LED5, LED6, LED7
        }

        private List<Tuple<CvPoint, SEG_NAME>> TestpointForGetLum = new List<Tuple<CvPoint, SEG_NAME>>();

        private List<Tuple<CvPoint, SEG_NAME>> GetTestPoint()
        {
            var list = new List<Tuple<CvPoint, SEG_NAME>>();
            list.Add(Tuple.Create(GetCenter(State.VmLedPoint.LED1), SEG_NAME.LED1));
            list.Add(Tuple.Create(GetCenter(State.VmLedPoint.LED2), SEG_NAME.LED2));
            list.Add(Tuple.Create(GetCenter(State.VmLedPoint.LED3), SEG_NAME.LED3));
            list.Add(Tuple.Create(GetCenter(State.VmLedPoint.LED4), SEG_NAME.LED4));

            return list;
        }

        private void toggleSw_Checked(object sender, RoutedEventArgs e)
        {
            General.camLed.Opening = true;
        }

        private void toggleSw_Unchecked(object sender, RoutedEventArgs e)
        {
            General.camLed.Opening = false;
        }


        bool FlagLight;
        private async void buttonLight_Click(object sender, RoutedEventArgs e)
        {
            if (FlagLight)
            {
                General.SetLight(false);
                buttonLight.Background = General.OffBrush;
            }
            else
            {
                General.SetLight(true);
                buttonLight.Background = General.OnBrush;
            }

            FlagLight = !FlagLight;
        }

        bool FlagCheckCol;
        private void CheckColor()
        {
            IplImage source = new IplImage(640, 360, BitDepth.U8, 3);

            const int TEST_FRAME = 36;

            var ListH = new List<int>();

            int side = (int)Math.Sqrt(TEST_FRAME);//検査枠の１辺の長さ

            try
            {
                Task.Run(() =>
                {
                    while (FlagCheckCol)
                    {
                        try
                        {
                            //画像を取得する処理
                            if (!General.camLed.GetPic())
                                continue;

                            source = General.camLed.imageForTest;

                            using (var hsv = new IplImage(640, 360, BitDepth.U8, 3)) // グレースケール画像格納用の変数
                            {
                                //RGBからHSVに変換
                                Cv.CvtColor(source, hsv, ColorConversion.BgrToHsv);

                                OpenCvSharp.CPlusPlus.Mat mat = new OpenCvSharp.CPlusPlus.Mat(hsv, true);

                                TestpointForGetLum = GetTestPoint();
                                TestpointForGetLum.ForEach(l =>
                                {
                                    ListH.Clear();
                                    foreach (var i in Enumerable.Range(0, side))
                                    {
                                        foreach (var j in Enumerable.Range(0, side))
                                        {
                                            var re = mat.At<OpenCvSharp.CPlusPlus.Vec3b>(l.Item1.Y - (side / 2) + i, l.Item1.X - (side / 2) + j);
                                            if (re[0] != 0)
                                            {
                                                ListH.Add(re[0]);
                                            }
                                        }
                                    }

                                    string Hue = (ListH.Count != 0) ? ListH.Average().ToString("F0") : "0";

                                    switch (l.Item2)
                                    {
                                        case SEG_NAME.LED1:
                                            State.VmLedPoint.LED1Hue = Hue;
                                            break;

                                        case SEG_NAME.LED2:
                                            State.VmLedPoint.LED2Hue = Hue;
                                            break;

                                        case SEG_NAME.LED3:
                                            State.VmLedPoint.LED3Hue = Hue;
                                            break;

                                        case SEG_NAME.LED4:
                                            State.VmLedPoint.LED4Hue = Hue;
                                            break;

                                    }

                                });
                            }
                        }
                        catch
                        {
                            FlagCheckCol = false;
                        }


                    }
                });


            }
            finally
            {
                source.Dispose();
            }

        }

        private void labeling()
        {
            Task.Run(() =>
            {
                while (FlagLabeling)
                {
                    if (General.camLed.blobs == null) continue;
                    var blobInfo = General.camLed.blobs.Clone();
                    if (blobInfo.Count() != 4) continue;

                    var SortLed3_4RectBlob = blobInfo.OrderBy(b => b.Value.Centroid.X).Take(2).OrderBy(b => b.Value.Centroid.Y).ToList();//７セグ(LD1)のドットが点灯してしまっているため削除する

                    State.VmLedPoint.LED4 = SortLed3_4RectBlob[0].Value.Centroid.X.ToString("F0") + "/" + SortLed3_4RectBlob[0].Value.Centroid.Y.ToString("F0");
                    State.VmLedPoint.LED4Lum = SortLed3_4RectBlob[0].Value.Area.ToString();

                    State.VmLedPoint.LED3 = SortLed3_4RectBlob[1].Value.Centroid.X.ToString("F0") + "/" + SortLed3_4RectBlob[1].Value.Centroid.Y.ToString("F0");
                    State.VmLedPoint.LED3Lum = SortLed3_4RectBlob[1].Value.Area.ToString();

                    var SortLed1_2RectBlob = blobInfo.OrderBy(b => b.Value.Centroid.X).Skip(2).OrderBy(b => b.Value.Centroid.Y).ToList();//７セグ(LD1)のドットが点灯してしまっているため削除する

                    State.VmLedPoint.LED2 = SortLed1_2RectBlob[0].Value.Centroid.X.ToString("F0") + "/" + SortLed1_2RectBlob[0].Value.Centroid.Y.ToString("F0");
                    State.VmLedPoint.LED2Lum = SortLed1_2RectBlob[0].Value.Area.ToString();

                    State.VmLedPoint.LED1 = SortLed1_2RectBlob[1].Value.Centroid.X.ToString("F0") + "/" + SortLed1_2RectBlob[1].Value.Centroid.Y.ToString("F0");
                    State.VmLedPoint.LED1Lum = SortLed1_2RectBlob[1].Value.Area.ToString();



                    //State.VmLedPoint.LED2 = SortRectBlob[2].Value.Centroid.X.ToString("F0") + "/" + SortRectBlob[2].Value.Centroid.Y.ToString("F0");
                    //State.VmLedPoint.LED2Lum = SortRectBlob[2].Value.Area.ToString();

                    ////TODO: 現物と比較して座標を確認すること
                    //State.VmLedPoint.LED3 = SortRectBlob[0].Value.Centroid.X.ToString("F0") + "/" + SortRectBlob[0].Value.Centroid.Y.ToString("F0");
                    //State.VmLedPoint.LED3Lum = SortRectBlob[0].Value.Area.ToString();

                    //State.VmLedPoint.LED4 = SortRectBlob[1].Value.Centroid.X.ToString("F0") + "/" + SortRectBlob[1].Value.Centroid.Y.ToString("F0");
                    //State.VmLedPoint.LED4Lum = SortRectBlob[1].Value.Area.ToString();


                }

            });
        }

        bool FlagLabeling;
        private void buttonLabeling_Click(object sender, RoutedEventArgs e)
        {
            FlagLabeling = !FlagLabeling;

            buttonBin.IsEnabled = !FlagLabeling;
            buttonGrid.IsEnabled = !FlagLabeling;

            buttonLabeling.Background = FlagLabeling ? General.OnBrush : General.OffBrush;

            if (FlagLabeling)
            {
                General.camLed.ResetFlag();
                General.camLed.FlagLabeling = true;

                labeling();
            }
            else
            {
                General.camLed.ResetFlag();
            }

        }

        private void buttonHue_Click(object sender, RoutedEventArgs e)
        {
            FlagCheckCol = !FlagCheckCol;

            buttonLabeling.IsEnabled = !FlagCheckCol;
            buttonBin.IsEnabled = !FlagCheckCol;
            buttonGrid.IsEnabled = !FlagCheckCol;

            if (FlagCheckCol)
            {
                FlagCheckCol = true;
                CheckColor();
            }
            else
            {
                FlagCheckCol = false;
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
                General.camLed.SetWb();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            State.CamPropLed.Opening = false;

        }
    }
}
