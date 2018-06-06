using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Threading.Tasks;
using System.Media;
using System.Windows.Media;
using OpenCvSharp;
using static System.Threading.Thread;
using static PC80_Tester.State;

namespace PC80_Tester
{


    public static class General
    {

        //インスタンス変数の宣言
        public static SoundPlayer player = null;
        public static SoundPlayer soundPass = null;
        public static SoundPlayer soundFail = null;
        public static SoundPlayer soundAlarm = null;
        public static SoundPlayer soundSuccess = null;
        public static SoundPlayer soundNotice = null;
        public static SoundPlayer soundLabel = null;


        public static SolidColorBrush DialogOnBrush = new SolidColorBrush();
        public static SolidColorBrush OnBrush = new SolidColorBrush();
        public static SolidColorBrush OffBrush = new SolidColorBrush();
        public static SolidColorBrush NgBrush = new SolidColorBrush();


        //インスタンスを生成する必要がある周辺機器
        public static FFT wv;
        public static Camera camLcd;
        public static Camera camLed;
        public static IMultimeter multimeter;

        static General()
        {
            //オーディオリソースを取り出す
            General.soundPass = new SoundPlayer(@"Resources\Wav\Victory.wav");
            General.soundFail = new SoundPlayer(@"Resources\Wav\Fail.wav");
            General.soundAlarm = new SoundPlayer(@"Resources\Wav\Alarm.wav");
            General.soundSuccess = new SoundPlayer(@"Resources\Wav\Success.wav");
            General.soundNotice = new SoundPlayer(@"Resources\Wav\Notice.wav");
            General.soundLabel = new SoundPlayer(@"Resources\Wav\NewLabelSound.wav");

            OffBrush.Color = Colors.Transparent;

            DialogOnBrush.Color = Colors.DodgerBlue;
            DialogOnBrush.Opacity = 0.3;

            OnBrush.Color = Colors.DodgerBlue;
            OnBrush.Opacity = 0.3;

            NgBrush.Color = Colors.HotPink;
            NgBrush.Opacity = 0.3;
        }

        public static void Show()
        {
            var T = 0.3;
            var t = 0.005;

            State.Setting.OpacityTheme = State.VmMainWindow.ThemeOpacity;
            //10msec刻みでT秒で元のOpacityに戻す
            int times = (int)(T / t);

            State.VmMainWindow.ThemeOpacity = 0;
            Task.Run(() =>
            {
                while (true)
                {

                    State.VmMainWindow.ThemeOpacity += State.Setting.OpacityTheme / (double)times;
                    Thread.Sleep((int)(t * 1000));
                    if (State.VmMainWindow.ThemeOpacity >= State.Setting.OpacityTheme) return;

                }
            });
        }

        public static void SetRadius(bool sw)
        {
            var T = 0.45;//アニメーションが完了するまでの時間（秒）
            var t = 0.005;//（秒）

            //5msec刻みでT秒で元のOpacityに戻す
            int times = (int)(T / t);


            Task.Run(() =>
            {
                if (sw)
                {
                    while (true)
                    {
                        State.VmMainWindow.ThemeBlurEffectRadius += 25 / (double)times;
                        Thread.Sleep((int)(t * 1000));
                        if (State.VmMainWindow.ThemeBlurEffectRadius >= 25) return;

                    }
                }
                else
                {
                    var CurrentRadius = State.VmMainWindow.ThemeBlurEffectRadius;
                    while (true)
                    {
                        CurrentRadius -= 25 / (double)times;
                        if (CurrentRadius > 0)
                        {
                            State.VmMainWindow.ThemeBlurEffectRadius = CurrentRadius;
                        }
                        else
                        {
                            State.VmMainWindow.ThemeBlurEffectRadius = 0;
                            return;
                        }
                        Thread.Sleep((int)(t * 1000));
                    }
                }

            });
        }



        public static bool SaveRetryLog()
        {
            if (State.RetryLogList.Count() == 0) return true;

            //出力用のファイルを開く appendをtrueにすると既存のファイルに追記、falseにするとファイルを新規作成する
            using (var sw = new System.IO.StreamWriter(Constants.fileName_RetryLog, true, Encoding.GetEncoding("Shift_JIS")))
            {
                try
                {
                    State.RetryLogList.ForEach(d =>
                    {
                        sw.WriteLine(d);
                    });

                    return true;
                }
                catch
                {
                    return false;
                }
            }

        }



        private static List<string> MakePassTestData()//TODO:
        {
            var ListData = new List<string>
            {
                "AssemblyVer " + State.AssemblyInfo,
                "TestSpecVer " + State.Spec.TestSpecVer,
                System.DateTime.Now.ToString("yyyy年MM月dd日(ddd) HH:mm:ss"),
                VmMainWindow.Operator,
                VmTestStatus.FwVer,
                VmTestStatus.FwSum,

                VmTestResults.Vol5v,
                VmTestResults.Curr,

                VmTestResults.P130,
                VmTestResults.Buz,
                VmTestResults.BuzLev,

                VmTestResults.HueLed1,
                VmTestResults.HueLed2,
                VmTestResults.HueLed3,
                VmTestResults.HueLed4,

                VmTestResults.Fig1UP_L,
                VmTestResults.Fig1UP_R,
                VmTestResults.Fig1LO_L,
                VmTestResults.Fig1LO_R,

                VmTestResults.Fig2UP_L,
                VmTestResults.Fig2UP_R,
                VmTestResults.Fig2LO_L,
                VmTestResults.Fig2LO_R,
            };

            return ListData;
        }

        public static bool SaveTestData()
        {
            try
            {
                var dataList = new List<string>();
                dataList = MakePassTestData();

                var OkDataFilePath = Constants.PassDataFolderPath + State.VmMainWindow.Opecode + ".csv";

                if (!System.IO.File.Exists(OkDataFilePath))
                {
                    //既存検査データがなければ新規作成
                    File.Copy(Constants.PassDataFolderPath + "Format.csv", OkDataFilePath);
                }

                // リストデータをすべてカンマ区切りで連結する
                string stCsvData = string.Join(",", dataList);

                // appendをtrueにすると，既存のファイルに追記
                // falseにすると，ファイルを新規作成する
                var append = true;

                // 出力用のファイルを開く
                using (var sw = new System.IO.StreamWriter(OkDataFilePath, append, Encoding.GetEncoding("Shift_JIS")))
                {
                    sw.WriteLine(stCsvData);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }


        //**************************************************************************
        //検査データの保存　　　　
        //引数：なし
        //戻値：なし
        //**************************************************************************

        public static bool SaveNgData(List<string> dataList)
        {
            try
            {
                var NgDataFilePath = Constants.FailDataFolderPath + State.VmMainWindow.Opecode + ".csv";
                if (!File.Exists(NgDataFilePath))
                {
                    //既存検査データがなければ新規作成
                    File.Copy(Constants.FailDataFolderPath + "FormatNg.csv", NgDataFilePath);
                }

                var stArrayData = dataList.ToArray();
                // リストデータをすべてカンマ区切りで連結する
                string stCsvData = string.Join(",", stArrayData);

                // appendをtrueにすると，既存のファイルに追記
                //         falseにすると，ファイルを新規作成する
                var append = true;

                // 出力用のファイルを開く
                using (var sw = new System.IO.StreamWriter(NgDataFilePath, append, Encoding.GetEncoding("Shift_JIS")))
                {
                    sw.WriteLine(stCsvData);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        //**************************************************************************
        //EPX64のリセット
        //引数：なし
        //戻値：なし
        //**************************************************************************
        public static void ResetIo()
        {
            //IOを初期化する処理（出力をすべてＬに落とす）
            LPC1768.SendData("ResetIo");
            Flags.PowOn = false;
            Flags.LightOn = false;
        }
        public static bool CheckPressOpen()
        {
            LPC1768.SendData("R,P25");
            return (LPC1768.RecieveData == "H");
        }


        public static void PowSupply_TestMode(bool sw)
        {
            if (Flags.PowOn == sw) return;
            if (sw)
            {
                SetK6(true);
                Sleep(500);
                SetK1(true);
                Sleep(500);
                SetK6(false);
            }
            else
            {
                SetK1(false);
            }

            Flags.PowOn = sw;
        }
        public static void PowSupply(bool sw)
        {
            if (Flags.PowOn == sw) return;
            if (sw)
            {
                SetK1(true);
            }
            else
            {
                SetK1(false);
            }

            Flags.PowOn = sw;
        }

        public static void SetRelayForAdjustVr()
        {
            SetK4(true);
            Sleep(250);
        }

        public static void SetRelayForVccCheck()
        {
            SetK3(true);
            Sleep(200);
        }

        public static void SetRelayForCurrCheck()
        {
            SetK2(true);
            Sleep(800);
            SetRL1(true);
        }

        public static void SetRelayForP130Check()
        {
            SetK5(true);
            Sleep(200);
        }


        //**************************************************************************
        //WAVEファイルを再生する
        //引数：なし
        //戻値：なし
        //**************************************************************************  

        //WAVEファイルを再生する（非同期で再生）
        public static void PlaySound(SoundPlayer p)
        {
            //再生されているときは止める
            if (player != null)
                player.Stop();

            //waveファイルを読み込む
            player = p;
            //最後まで再生し終えるまで待機する
            player.Play();
        }
        //WAVEファイルを再生する（同期で再生）
        public static void PlaySound2(SoundPlayer p)
        {
            //再生されているときは止める
            if (player != null)
                player.Stop();

            //waveファイルを読み込む
            player = p;
            //最後まで再生し終えるまで待機する
            player.PlaySync();

        }

        public static void PlaySoundLoop(SoundPlayer p)
        {
            //再生されているときは止める
            if (player != null)
                player.Stop();

            //waveファイルを読み込む
            player = p;
            //最後まで再生し終えるまで待機する
            player.PlayLooping();
        }

        //再生されているWAVEファイルを止める
        public static void StopSound()
        {
            if (player != null)
            {
                player.Stop();
                player.Dispose();
                player = null;
            }
        }



        public static void ResetViewModel()//TODO:
        {
            //ViewModel OK台数、NG台数、Total台数の更新
            VmTestStatus.OkCount = Setting.TodayOkCount.ToString() + "台";
            VmTestStatus.NgCount = Setting.TodayNgCount.ToString() + "台";
            VmTestStatus.Message = Constants.MessSet;
            camLcd.ImageOpacity = Constants.OpacityImgMin;
            camLed.ImageOpacity = Constants.OpacityImgMin;


            VmTestStatus.DecisionVisibility = System.Windows.Visibility.Hidden;
            VmTestStatus.ErrInfoVisibility = System.Windows.Visibility.Hidden;
            VmTestStatus.RingVisibility = System.Windows.Visibility.Visible;

            VmTestStatus.TestTime = "00:00";
            VmTestStatus.進捗度 = 0;
            VmTestStatus.TestLog = "";

            VmTestStatus.FailInfo = "";
            VmTestStatus.Spec = "";
            VmTestStatus.MeasValue = "";


            //試験結果のクリア
            VmTestResults = new ViewModelTestResult();

            //通信ログのクリア
            VmComm.TX_IO = "";
            VmComm.RX_IO = "";
            VmComm.TX_TARGET = "";
            VmComm.RX_TARGET = "";

            VmMainWindow.EnableOtherButton = true;

            //各種フラグの初期化
            Flags.PowOn = false;
            Flags.ClickStopButton = false;
            Flags.Testing = false;


            //テーマ透過度を元に戻す
            General.SetRadius(false);

            VmTestStatus.RetryLabelVis = System.Windows.Visibility.Hidden;
            VmTestStatus.TestSettingEnable = true;
            VmMainWindow.OperatorEnable = true;

            VmTestStatus.CheckWriteTestFwPass = false;


        }


        public static void CheckAll周辺機器フラグ()
        {
            Flags.AllOk周辺機器接続 = (Flags.State1768 && Flags.StateMoxa && camLcd.CamState && camLed.CamState && Flags.StateMic && Flags.StateMultimeter);
        }


        public static void Init周辺機器()//TODO:
        {
            Flags.Initializing周辺機器 = true;

            Task.Run(() =>
            {
                Flags.DoGetDeviceName = true;
                while (Flags.DoGetDeviceName)
                {
                    FindSerialPort.GetDeviceNames();
                    Sleep(400);
                }

            });

            //LPC1768の初期化
            bool Stop1768 = false;
            Task.Run(() =>
            {
                while (true)
                {
                    if (Flags.StopInit周辺機器)
                    {
                        break;
                    }

                    Flags.State1768 = LPC1768.Init();
                    if (Flags.State1768)
                    {
                        //IOボードのリセット（出力をすべてLする）
                        ResetIo();
                        break;
                    }

                    Thread.Sleep(500);
                }
                Stop1768 = true;
            });

            //MOXA1130の初期化
            bool StopMoxa = false;
            Task.Run(() =>
            {
                while (true)
                {
                    if (Flags.StopInit周辺機器)
                    {
                        break;
                    }

                    Flags.StateMoxa = Target.Init();
                    if (Flags.StateMoxa)
                    {
                        break;
                    }

                    Thread.Sleep(500);
                }
                StopMoxa = true;
            });


            //カメラ1（LCD撮影）の初期化
            bool StopCAMERA1 = false;
            //カメラ2（LED撮影）の初期化
            bool StopCAMERA2 = false;
            
            Task.Run(() =>
            {
                camLcd = new Camera(State.CamPropLcd.CamNumber, Constants.filePath_CamLcdCalFilePath, 640, 360);
                while (true)
                {
                    if (Flags.StopInit周辺機器)
                    {
                        break;
                    }

                    camLcd.InitCamera();
                    if (camLcd.CamState) break;

                    Thread.Sleep(500);
                }
                StopCAMERA1 = true;

                camLed = new Camera(State.CamPropLed.CamNumber, Constants.filePath_CamLedCalFilePath, 640, 360);
                while (true)
                {
                    if (Flags.StopInit周辺機器)
                    {
                        break;
                    }

                    camLed.InitCamera();
                    if (camLed.CamState) break;

                    Thread.Sleep(500);
                }
                StopCAMERA2 = true;
            });


            //アドバンテスト R6441Bの初期化
            bool StopMulti = false;
            Task.Run(() =>
            {
                IMultimeter multimeter1 = new R6441B();
                IMultimeter multimeter2 = new Agilent34401A();
                while (true)
                {
                    if (Flags.StopInit周辺機器)
                    {
                        break;
                    }

                    Flags.StateMultimeter = multimeter1.Init();
                    if (Flags.StateMultimeter)
                    {
                        multimeter = multimeter1;
                        break;
                    }

                    Flags.StateMultimeter = multimeter2.Init();
                    if (Flags.StateMultimeter)
                    {
                        multimeter = multimeter2;
                        break;
                    }
                }
                StopMulti = true;
            });



            Task.Run(() =>
            {
                while (true)
                {
                    CheckAll周辺機器フラグ();
                    var IsAllStopped = Stop1768 && StopMoxa && StopMulti && StopCAMERA1 && StopCAMERA2;

                    if (Flags.AllOk周辺機器接続 || IsAllStopped) break;
                    Thread.Sleep(400);

                }
                Flags.DoGetDeviceName = false;
                Sleep(500);
                Flags.Initializing周辺機器 = false;
            });

        }

        public static async Task CheckNextButton(bool ExtraSound = false)
        {
            var tm = new GeneralTimer(500);

            await Task.Run(() =>
            {
                try
                {
                    bool sw = true;
                    SetSwLed(sw);
                    tm.start();
                    while (true)
                    {
                        if (tm.FlagTimeout)
                        {
                            sw = !sw;
                            SetSwLed(sw);
                            tm.start();
                        }
                        LPC1768.SendData("R,P26");
                        if (LPC1768.RecieveData == "L" || Flags.DialogPushed)
                        {
                            tm.stop();
                            return;
                        }
                    }
                }
                finally
                {
                    SetSwLed(false);
                }
            });
        }





        public static IplImage trimming(IplImage src, int x, int y, int width, int height)
        {
            IplImage dest = new IplImage(width, height, src.Depth, src.NChannels);
            Cv.SetImageROI(src, new CvRect(x, y, width, height));
            dest = src.Clone();
            Cv.ResetImageROI(src);
            return dest;
        }

        public static void LedAllOn()
        {
            //TODO: LED全点灯処理
            Target.SendData("LED1ON");
            Sleep(1000);
            Target.SendData("LED2ON");
            Sleep(1000);
            Target.SendData("LED3ON");
            Sleep(1000);
            Target.SendData("LED4ON");
        }

        public static void GetBuzzData()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    State.VmTestResults.VolLev = General.wv.Vol.ToString("F2");
                    State.VmTestResults.Freq = General.wv.Freq.ToString("F0") + "Hz";
                    State.VmTestStatus.DataList1 = General.wv.VolPoints;
                    State.VmTestStatus.DataList2 = General.wv.FreqPoints;
                    Thread.Sleep(125);
                }
            });
        }



        //試験機リレー制御
        public static void SetK1(bool sw) { LPC1768.SendData("W,P05," + (sw ? "1" : "0")); }
        public static void SetK2(bool sw) { LPC1768.SendData("W,P06," + (sw ? "1" : "0")); }
        public static void SetRL1(bool sw) { LPC1768.SendData("W,P07," + (sw ? "1" : "0")); }
        public static void SetK3(bool sw) { LPC1768.SendData("W,P08," + (sw ? "1" : "0")); }
        public static void SetK4(bool sw) { LPC1768.SendData("W,P09," + (sw ? "1" : "0")); }
        public static void SetK5(bool sw) { LPC1768.SendData("W,P10," + (sw ? "1" : "0")); }
        public static void SetK6(bool sw) { LPC1768.SendData("W,P11," + (sw ? "1" : "0")); }
        public static void SetK7_RL2(bool sw) { LPC1768.SendData("W,P12," + (sw ? "1" : "0")); }

        //ソレノイド制御
        public static void SetSw1(bool sw) { LPC1768.SendData("W,P13," + (sw ? "0" : "1")); }
        public static void SetSw2(bool sw) { LPC1768.SendData("W,P14," + (sw ? "0" : "1")); }
        public static void SetSw3(bool sw) { LPC1768.SendData("W,P15," + (sw ? "0" : "1")); }
        public static void SetSw4(bool sw) { LPC1768.SendData("W,P16," + (sw ? "0" : "1")); }
        public static void SetSw5(bool sw) { LPC1768.SendData("W,P17," + (sw ? "0" : "1")); }
        public static void SetSw6(bool sw) { LPC1768.SendData("W,P18," + (sw ? "0" : "1")); }
        public static void SetSw7(bool sw) { LPC1768.SendData("W,P19," + (sw ? "0" : "1")); }
        public static void SetSw8(bool sw) { LPC1768.SendData("W,P20," + (sw ? "0" : "1")); }
        public static void SetSw9(bool sw) { LPC1768.SendData("W,P21," + (sw ? "0" : "1")); }

        //照明制御
        public static void SetLight(bool sw)
        {
            LPC1768.SendData("W,P23," + (sw ? "0" : "1"));
            Flags.LightOn = sw;
        }
        public static void SetSwLed(bool sw) { LPC1768.SendData("W,P24," + (sw ? "0" : "1")); }

        //合格印制御
        public static void StampOn() { LPC1768.SendData("STAMP"); }//mbed側でON⇒OFFの処理を行う

    }

}

