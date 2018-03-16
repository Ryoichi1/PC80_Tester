using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Threading.Thread;
using static PC80_Tester.State;
using static PC80_Tester.General;

namespace PC80_Tester
{
    public static class TestLcd
    {
        public enum NAME { UpLeft, UpRight, LoLeft, LoRight }
        public enum FIG_NAME { FIG1, FIG2 }

        public class LcdSpec
        {
            public NAME name;
            public double 角度;
            public double 一致率;
            public bool result;
        }

        public static List<LcdSpec> ListSpecs;// CameraConfクラスからも使用します（カメラ調整のため）

        private static void InitList()
        {
            ListSpecs = new List<LcdSpec>();
            foreach (var n in Enum.GetValues(typeof(NAME)))
            {
                ListSpecs.Add(new LcdSpec { name = (NAME)n });
            }
        }

        public static async Task<bool> CheckLcd(FIG_NAME fig)
        {
            var result = false;
            try
            {
                return result = await Task<bool>.Run(() =>
                {
                    InitList();
                    return result = Check(fig);
                });
            }
            finally
            {
                SetViewModel(fig);
                //if (!result)
                //{
                //    State.uriOtherInfoPage = new Uri("Page/ErrInfo/ErrInfoコネクタチェック.xaml", UriKind.Relative);
                //    State.VmTestStatus.EnableButtonErrInfo = System.Windows.Visibility.Visible;
                //}
            }
        }

        /// <summary>
        /// CameraConfクラスからも使用します（カメラ調整のため）
        /// </summary>
        public static bool Check(FIG_NAME fig)
        {
            IplImage src = null, tmp = null, tmpUP_L = null, tmpUP_R = null, tmpLO_L = null, tmpLO_R = null;

            try
            {
                int x = 0, y = 0, w = 0, h = 0, m = 10;

                State.SetCamPropForLcd();

                switch (fig)
                {
                    case FIG_NAME.FIG1:
                        tmpUP_L = Cv.LoadImage(Constants.filePath_Fig1TempUpLeft);
                        tmpUP_R = Cv.LoadImage(Constants.filePath_Fig1TempUpRight);
                        tmpLO_L = Cv.LoadImage(Constants.filePath_Fig1TempLoLeft);
                        tmpLO_R = Cv.LoadImage(Constants.filePath_Fig1TempLoRight);
                        Target.SendData(Constants.Fig1Command);
                        break;
                    case FIG_NAME.FIG2:
                        tmpUP_L = Cv.LoadImage(Constants.filePath_Fig2TempUpLeft);
                        tmpUP_R = Cv.LoadImage(Constants.filePath_Fig2TempUpRight);
                        tmpLO_L = Cv.LoadImage(Constants.filePath_Fig2TempLoLeft);
                        tmpLO_R = Cv.LoadImage(Constants.filePath_Fig2TempLoRight);
                        Target.SendData(Constants.Fig2Command);
                        break;
                }

                ListSpecs.ForEach(l =>
                {
                    switch (l.name)
                    {
                        case NAME.UpLeft:
                            x = State.CamPropLcd.X_UP_L;
                            y = State.CamPropLcd.Y_UP_L;
                            w = State.CamPropLcd.W_UP_L;
                            h = State.CamPropLcd.H_UP_L;
                            tmp = tmpUP_L;
                            break;
                        case NAME.UpRight:
                            x = State.CamPropLcd.X_UP_R;
                            y = State.CamPropLcd.Y_UP_R;
                            w = State.CamPropLcd.W_UP_R;
                            h = State.CamPropLcd.H_UP_R;
                            tmp = tmpUP_R;
                            break;
                        case NAME.LoLeft:
                            x = State.CamPropLcd.X_LO_L;
                            y = State.CamPropLcd.Y_LO_L;
                            w = State.CamPropLcd.W_LO_L;
                            h = State.CamPropLcd.H_LO_L;
                            tmp = tmpLO_L;
                            break;
                        case NAME.LoRight:
                            x = State.CamPropLcd.X_LO_R;
                            y = State.CamPropLcd.Y_LO_R;
                            w = State.CamPropLcd.W_LO_R;
                            h = State.CamPropLcd.H_LO_R;
                            tmp = tmpLO_R;
                            break;
                    }

                    Sleep(1000);

                    //cam1の画像を取得する処理
                    General.cam.GetPic();
                    src = General.cam.imageForTest.Clone();
                    //
                    x -= m;
                    y -= m;
                    w += 2 * m;
                    h += 2 * m;

                    var _livePic = General.trimming(src, x, y, w, h);

                    double max;
                    double min;

                    var Angles = MakeTheta().ToList();
                    //ローカル関数
                    IEnumerable<double> MakeTheta()
                    {
                        var Val = -3.0;
                        while (true)
                        {
                            yield return Val;
                            Val += 0.1;
                            if (Val > 3.0) yield break;
                        }
                    }


                    var AllData = new List<(double 角度, double 一致率, CvPoint 座標)>();

                    Angles.ForEach(t =>
                    {
                        var livePic = _livePic.Clone();
                        //傾き補正
                        CvPoint2D32f center = new CvPoint2D32f(w / 2, h / 2);
                        CvMat affineMatrix = Cv.GetRotationMatrix2D(center, t, 1.0);
                        Cv.WarpAffine(livePic, livePic, affineMatrix);

                        CvMat result = new CvMat(h - tmp.Height + 1, w - tmp.Width + 1, MatrixType.F32C1);
                        Cv.MatchTemplate(livePic, tmp, result, MatchTemplateMethod.CCoeffNormed);

                        Cv.MinMaxLoc(result, out min, out max);

                        CvPoint minPoint = new CvPoint();
                        CvPoint maxPoint = new CvPoint();
                        Cv.MinMaxLoc(result, out minPoint, out maxPoint);

                        AllData.Add((t, max, maxPoint));
                    });


                    //////////
                    var re = AllData.OrderByDescending(d => d.一致率).ToList()[0];//相関係数の一番高いものを抽出する
                    l.一致率 = re.一致率*100;
                    l.角度 = re.角度;
                    l.result = re.一致率 >= State.Spec.LcdMatchMin;
                    //////////
                });

                return ListSpecs.All(l => l.result);
            }
            catch
            {
                return false;
            }
            finally
            {
                General.cam.ResetFlag();//これ忘れると無限ループにハマる
                src.Dispose();
                tmp.Dispose();
            }
        }

        private static void SetViewModel(FIG_NAME fig)
        {
            switch (fig)
            {
                case FIG_NAME.FIG1:
                    State.VmTestResults.Fig1UP_L = ListSpecs.Find(l => l.name == NAME.UpLeft).一致率.ToString("F2") + "%";
                    State.VmTestResults.Fig1UP_R = ListSpecs.Find(l => l.name == NAME.UpRight).一致率.ToString("F2") + "%";
                    State.VmTestResults.Fig1LO_L = ListSpecs.Find(l => l.name == NAME.LoLeft).一致率.ToString("F2") + "%";
                    State.VmTestResults.Fig1LO_R = ListSpecs.Find(l => l.name == NAME.LoRight).一致率.ToString("F2") + "%";
                    State.VmTestResults.ColFig1UP_L = ListSpecs.Find(l => l.name == NAME.UpLeft).result? OffBrush : NgBrush;
                    State.VmTestResults.ColFig1UP_R = ListSpecs.Find(l => l.name == NAME.UpRight).result? OffBrush : NgBrush;
                    State.VmTestResults.ColFig1LO_L = ListSpecs.Find(l => l.name == NAME.LoLeft).result? OffBrush : NgBrush;
                    State.VmTestResults.ColFig1LO_R = ListSpecs.Find(l => l.name == NAME.LoRight).result? OffBrush : NgBrush;
                    break;
                case FIG_NAME.FIG2:
                    State.VmTestResults.Fig2UP_L = ListSpecs.Find(l => l.name == NAME.UpLeft).一致率.ToString("F2") + "%";
                    State.VmTestResults.Fig2UP_R = ListSpecs.Find(l => l.name == NAME.UpRight).一致率.ToString("F2") + "%";
                    State.VmTestResults.Fig2LO_L = ListSpecs.Find(l => l.name == NAME.LoLeft).一致率.ToString("F2") + "%";
                    State.VmTestResults.Fig2LO_R = ListSpecs.Find(l => l.name == NAME.LoRight).一致率.ToString("F2") + "%";
                    State.VmTestResults.ColFig2UP_L = ListSpecs.Find(l => l.name == NAME.UpLeft).result? OffBrush : NgBrush;
                    State.VmTestResults.ColFig2UP_R = ListSpecs.Find(l => l.name == NAME.UpRight).result? OffBrush : NgBrush;
                    State.VmTestResults.ColFig2LO_L = ListSpecs.Find(l => l.name == NAME.LoLeft).result? OffBrush : NgBrush;
                    State.VmTestResults.ColFig2LO_R = ListSpecs.Find(l => l.name == NAME.LoRight).result? OffBrush : NgBrush;
                    break;
            }
        }

        public static void CheckLcdDEMO()
        {
            IplImage src = null, tmp = null;

            int x = 0, y = 0, w = 640, h = 480, m = 0;

            x -= m;
            y -= m;
            w += 2 * m;
            h += 2 * m;


            tmp = Cv.LoadImage(@"C:\PC80\temp.bmp");
            src = Cv.LoadImage(@"C:\PC80\Fig3.bmp");




            var _livePic = General.trimming(src, x, y, w, h);

            double max;
            double min;

            var Angles = MakeTheta().ToList();
            //ローカル関数
            IEnumerable<double> MakeTheta()
            {
                var Val = -5.0;
                while (true)
                {
                    yield return Val;
                    Val += 0.1;
                    if (Val > 5.0) yield break;
                }
            }


            var AllData = new List<(double 角度, double 一致率, CvPoint 座標)>();

            Angles.ForEach(t =>
            {
                var livePic = _livePic.Clone();
                //傾き補正
                CvPoint2D32f center = new CvPoint2D32f(w / 2, h / 2);
                CvMat affineMatrix = Cv.GetRotationMatrix2D(center, t, 1.0);
                Cv.WarpAffine(livePic, livePic, affineMatrix);

                CvMat result = new CvMat(h - tmp.Height + 1, w - tmp.Width + 1, MatrixType.F32C1);
                Cv.MatchTemplate(livePic, tmp, result, MatchTemplateMethod.CCoeffNormed);

                Cv.MinMaxLoc(result, out min, out max);

                CvPoint minPoint = new CvPoint();
                CvPoint maxPoint = new CvPoint();
                Cv.MinMaxLoc(result, out minPoint, out maxPoint);

                AllData.Add((t, max, maxPoint));
            });


            //////////
            var re = AllData.OrderByDescending(d => d.一致率).ToList()[0];//相関係数の一番高いものを抽出する
            MessageBox.Show($"{re.一致率.ToString("F2")},{re.角度.ToString("F2")}");


        }
    }
}
