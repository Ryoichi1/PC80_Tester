using OpenCvSharp;
using OpenCvSharp.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using static PC80_Tester.General;
using static PC80_Tester.State;
using static System.Threading.Thread;

namespace PC80_Tester
{

    public static class TestLed
    {
        public enum NAME
        {
            LED1, LED2, LED3, LED4
        }


        const int WIDTH = 640;
        const int HEIGHT = 360;

        private static IplImage source = new IplImage(WIDTH, HEIGHT, BitDepth.U8, 3);

        public static List<LedSpec> ListLedSpec;

        public class LedSpec
        {
            public NAME name;
            public double x;
            public double y;
            public double Hue;
            public bool resultHue;
        }

        private static void InitList()
        {
            ListLedSpec = new List<LedSpec>();
            foreach (var n in Enum.GetValues(typeof(NAME)))
            {
                var _name = (NAME)n;
                var _x = GetPpoint(_name).x;
                var _y = GetPpoint(_name).y;
                ListLedSpec.Add(new LedSpec { name = _name, x = _x, y = _y });
            }
        }

        private static (int x, int y) GetPpoint(NAME name)
        {
            var X = 0;
            var Y = 0;

            switch (name)
            {
                case NAME.LED1:
                    X = Int32.Parse(State.CamPropLed.Led1.Split('/').ToArray()[0]);
                    Y = Int32.Parse(State.CamPropLed.Led1.Split('/').ToArray()[1]);
                    break;
                case NAME.LED2:
                    X = Int32.Parse(State.CamPropLed.Led2.Split('/').ToArray()[0]);
                    Y = Int32.Parse(State.CamPropLed.Led2.Split('/').ToArray()[1]);
                    break;
                case NAME.LED3:
                    X = Int32.Parse(State.CamPropLed.Led3.Split('/').ToArray()[0]);
                    Y = Int32.Parse(State.CamPropLed.Led3.Split('/').ToArray()[1]);
                    break;
                case NAME.LED4:
                    X = Int32.Parse(State.CamPropLed.Led4.Split('/').ToArray()[0]);
                    Y = Int32.Parse(State.CamPropLed.Led4.Split('/').ToArray()[1]);
                    break;
            }

            return (X, Y);

        }

        public static void CheckColorForDebug()
        {
            var side = 20;
            var X = 0;
            var Y = 0;

            InitList();

            using (IplImage hsv = new IplImage(WIDTH, HEIGHT, BitDepth.U8, 3)) // グレースケール画像格納用の変数
            {
                //cam1の画像を取得する処理
                General.cam.FlagTestPic = true;
                while (General.cam.FlagTestPic) ;
                using (IplImage src = General.cam.imageForTest.Clone()) // グレースケール画像格納用の変数
                {
                    General.cam.ResetFlag();
                    src.SaveImage(@"C:\PC80\Pic\src.jpg");
                    //RGBからHSVに変換
                    Cv.CvtColor(src, hsv, ColorConversion.BgrToHsv);
                    OpenCvSharp.CPlusPlus.Mat mat = new OpenCvSharp.CPlusPlus.Mat(hsv, true);

                    ListLedSpec.ForEach(l =>
                    {
                        var p = GetPpoint(l.name);
                        X = p.x;
                        Y = p.y;

                        var ListH = new List<int>();
                        var ListS = new List<int>();
                        var ListV = new List<int>();

                        foreach (var i in Enumerable.Range(0, side))
                        {
                            foreach (var j in Enumerable.Range(0, side))
                            {
                                var re = mat.At<OpenCvSharp.CPlusPlus.Vec3b>(Y - (side / 2) + i, X - (side / 2) + j);
                                if (re[0] >= 5 && re[1] > 220 && re[2] > 100)
                                {
                                    ListH.Add(re[0]);
                                    ListS.Add(re[1]);
                                    ListV.Add(re[2]);
                                }
                            }
                        }
                        var Hue = (ListH.Count != 0) ? ListH.Average() : 0;
                        var Sat = (ListS.Count != 0) ? ListS.Average() : 0;
                        var Val = (ListV.Count != 0) ? ListV.Average() : 0;

                        var H = Hue.ToString("F0");

                        ColorHSV CurrentHsv = new ColorHSV((float)Hue / 180, (float)Sat / 255, (float)Val / 255); //正規化
                        var rgb = ColorConv.HSV2RGB(CurrentHsv);
                        var color = new SolidColorBrush(Color.FromRgb(rgb.R, rgb.G, rgb.B));
                        color.Opacity = 0.5;
                        color.Freeze();//これ重要！！！  

                        //ビューモデルの更新
                        switch (l.name)
                        {
                            case NAME.LED1:
                                State.VmLedPoint.LED1Hue = H;
                                State.VmLedPoint.ColLED1Hue = color;
                                break;
                            case NAME.LED2:
                                State.VmLedPoint.LED2Hue = H;
                                State.VmLedPoint.ColLED2Hue = color;
                                break;
                            case NAME.LED3:
                                State.VmLedPoint.LED3Hue = H;
                                State.VmLedPoint.ColLED3Hue = color;
                                break;
                            case NAME.LED4:
                                State.VmLedPoint.LED4Hue = H;
                                State.VmLedPoint.ColLED4Hue = color;
                                break;
                        }
                    });
                }
            }
        }

        public static async Task<bool> Check(NAME name)
        {
            try
            {
                return await Task<bool>.Run(() =>
                {
                    State.SetCamPropForLed();
                    var rePoint = CheckPoint(name);
                    if (!rePoint.result) return false;
                    return CheckColor(name, rePoint.x, rePoint.y);
                });
            }
            catch
            {
                return false;
            }
            finally
            {
                await Task.Delay(700);
            }
        }

        private static bool CheckColor(NAME name, int x, int y)
        {
            const int side = 25;

            bool result = false;
            double Hue = 0;
            double Sat = 0;
            double Val = 0;
            var HueMax = 0.0;
            var HueMin = 0.0;

            try
            {
                using (IplImage hsv = new IplImage(WIDTH, HEIGHT, BitDepth.U8, 3)) // グレースケール画像格納用の変数
                {
                    //cam1の画像を取得する処理
                    if (!cam.GetPic()) return false;
                    using (IplImage src = cam.imageForTest.Clone()) // グレースケール画像格納用の変数
                    {
                        General.cam.ResetFlag();
                        //src.SaveImage(@"C:\Users\TSDP00059\Desktop\src.jpg");
                        //RGBからHSVに変換
                        Cv.CvtColor(src, hsv, ColorConversion.BgrToHsv);
                        OpenCvSharp.CPlusPlus.Mat mat = new OpenCvSharp.CPlusPlus.Mat(hsv, true);

                        var ListH = new List<int>();
                        var ListS = new List<int>();
                        var ListV = new List<int>();

                        foreach (var i in Enumerable.Range(0, side))
                        {
                            foreach (var j in Enumerable.Range(0, side))
                            {
                                var re = mat.At<OpenCvSharp.CPlusPlus.Vec3b>(y - (side / 2) + i, x - (side / 2) + j);
                                if (re[0] >= 5 && re[1] > 220 && re[2] > 100)//白っぽくないとこを抽出
                                {
                                    ListH.Add(re[0]);
                                    ListS.Add(re[1]);
                                    ListV.Add(re[2]);
                                }
                            }
                        }
                        Hue = (ListH.Count != 0) ? ListH.Average() : 0;
                        Sat = (ListS.Count != 0) ? ListS.Average() : 0;
                        Val = (ListV.Count != 0) ? ListV.Average() : 0;


                        switch (name)
                        {
                            case NAME.LED1:
                            case NAME.LED2:
                                HueMax = State.Spec.GreenHueMax;
                                HueMin = State.Spec.GreenHueMin;
                                break;
                            case NAME.LED3:
                                HueMax = State.Spec.RedHueMax;
                                HueMin = State.Spec.RedHueMin;
                                break;
                            case NAME.LED4:
                                HueMax = State.Spec.OrangeHueMax;
                                HueMin = State.Spec.OrangeHueMin;
                                break;
                        }

                        return result = (HueMin <= Hue && Hue <= HueMax);
                    }
                }

            }
            catch
            {
                return result = false;
            }
            finally
            {
                DriveLed(name, false);
                //ビューモデルの更新
                var H = Hue.ToString("F0");

                ColorHSV CurrentHsv = new ColorHSV((float)Hue / 180, (float)Sat / 255, (float)Val / 255); //正規化
                var rgb = ColorConv.HSV2RGB(CurrentHsv);
                var color = new SolidColorBrush(Color.FromRgb(rgb.R, rgb.G, rgb.B));
                color.Opacity = 0.5;
                color.Freeze();//これ重要！！！  

                switch (name)
                {
                    case NAME.LED1:
                        State.VmTestResults.HueLed1 = H;
                        State.VmTestResults.ColLed1 = color;
                        break;
                    case NAME.LED2:
                        State.VmTestResults.HueLed2 = H;
                        State.VmTestResults.ColLed2 = color;
                        break;
                    case NAME.LED3:
                        State.VmTestResults.HueLed3 = H;
                        State.VmTestResults.ColLed3 = color;
                        break;
                    case NAME.LED4:
                        State.VmTestResults.HueLed4 = H;
                        State.VmTestResults.ColLed4 = color;
                        break;
                }

                //NGだった場合、エラー詳細情報の規格値を更新する
                if (!result)
                {
                    VmTestStatus.Spec = $"規格値 : 色相 {HueMin.ToString("F0")} ～ {HueMax.ToString("F0")}";
                    VmTestStatus.MeasValue = $"計測値 : 色相 {H}";

                }
            }
        }

        //ローカル関数の定義
        private static void DriveLed(NAME name, bool sw)
        {
            var cmd = "";
            switch (name)
            {
                case NAME.LED1:
                    cmd = sw ? "LED1ON" : "LED1OFF";
                    break;
                case NAME.LED2:
                    cmd = sw ? "LED2ON" : "LED2OFF";
                    break;
                case NAME.LED3:
                    cmd = sw ? "LED3ON" : "LED3OFF";
                    break;
                case NAME.LED4:
                    cmd = sw ? "LED4ON" : "LED4OFF";
                    break;
            }
            Target.SendData(cmd);
        }

        private static (bool result, int x, int y) CheckPoint(NAME name)
        {
            bool result = false;
            double errPoint = 30;

            //InitList();
            try
            {
                cam.ResetFlag();//カメラのフラグを初期化 リトライ時にフラグが初期化できてないとだめ
                                //例 ＮＧリトライ時は、General.cam.FlagFrame = trueになっていてNGフレーム表示の無限ループにいる

                DriveLed(name, true);
                Sleep(600);
                cam.GetBlob(true);
                Sleep(1200);
                var blobInfo = General.cam.blobs.Clone();
                cam.GetBlob(false);
                //点灯しているLEDの座標を抽出する（画面半分より下側のブロブを抽出する）
                var blobLed = blobInfo.Where(b => b.Value.Centroid.Y > 180).ToList();

                if (blobLed.Count() != 1) return (false, 0, 0);//点灯しないか、もしくは２ケ以上点灯してたらダメ

                var x = blobLed[0].Value.Centroid.X;
                var y = blobLed[0].Value.Centroid.Y;

                var xSpec = GetPpoint(name).x;
                var ySpec = GetPpoint(name).y;

                var reX = xSpec - errPoint < x && x < xSpec + errPoint;
                var reY = ySpec - errPoint < y && y < ySpec + errPoint;
                result = reX && reY;//座標が大幅にズレていたらダメ

                return (result, (int)x, (int)y);
            }
            catch
            {
                return (false, 0, 0);
            }
        }

    }

}









