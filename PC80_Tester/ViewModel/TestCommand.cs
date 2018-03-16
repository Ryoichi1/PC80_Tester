
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Effects;
using static PC80_Tester.General;

namespace PC80_Tester
{
    public class TestCommand
    {

        //デリゲートの宣言
        public Action RefreshDataContext;//Test.Xaml内でテスト結果をクリアするために使用すする
        public Action SbRingLoad;
        public Action SbPass;
        public Action SbFail;
        public Action StopButtonBlinkOn;
        public Action StopButtonBlinkOff;

        private bool FlagTestTime;

        DropShadowEffect effect判定表示PASS;
        DropShadowEffect effect判定表示FAIL;

        public TestCommand()
        {
            effect判定表示PASS = new DropShadowEffect();
            effect判定表示PASS.Color = Colors.Aqua;
            effect判定表示PASS.Direction = 0;
            effect判定表示PASS.ShadowDepth = 0;
            effect判定表示PASS.Opacity = 1.0;
            effect判定表示PASS.BlurRadius = 80;

            effect判定表示FAIL = new DropShadowEffect();
            effect判定表示FAIL.Color = Colors.HotPink; ;
            effect判定表示FAIL.Direction = 0;
            effect判定表示FAIL.ShadowDepth = 0;
            effect判定表示FAIL.Opacity = 1.0;
            effect判定表示FAIL.BlurRadius = 40;

        }

        public async Task StartCheck()
        {
            var dis = App.Current.Dispatcher;
            while (true)
            {
                await Task.Run(() =>
                {
                    RETRY:
                    while (true)
                    {
                        if (Flags.OtherPage) break;
                        //Thread.Sleep(200);

                        //作業者名、工番が正しく入力されているかの判定
                        if (!Flags.SetOperator)
                        {
                            State.VmTestStatus.Message = Constants.MessOperator;
                            Flags.EnableTestStart = false;
                            continue;
                        }

                        if (!Flags.SetOpecode)
                        {
                            State.VmTestStatus.Message = Constants.MessOpecode;
                            Flags.EnableTestStart = false;
                            continue;
                        }

                        General.CheckAll周辺機器フラグ();
                        if (!Flags.AllOk周辺機器接続)
                        {
                            State.VmTestStatus.Message = Constants.MessCheckConnectMachine;
                            Flags.EnableTestStart = false;
                            continue;
                        }

                        dis.BeginInvoke(StopButtonBlinkOn);
                        State.VmTestStatus.Message = Constants.MessSet;
                        Flags.EnableTestStart = true;
                        Flags.Click確認Button = false;

                        while (true)
                        {
                            if (Flags.OtherPage || Flags.Click確認Button)
                            {
                                dis.BeginInvoke(StopButtonBlinkOff);
                                return;
                            }

                            if (!Flags.SetOperator || !Flags.SetOpecode)
                            {
                                dis.BeginInvoke(StopButtonBlinkOff);
                                goto RETRY;
                            }
                        }

                    }

                });

                if (Flags.OtherPage)//待機中に他のページに遷移したらメソッドを抜ける
                {
                    return;
                }

                State.VmMainWindow.EnableOtherButton = false;
                State.VmTestStatus.StartButtonContent = Constants.停止;
                State.VmTestStatus.TestSettingEnable = false;
                State.VmMainWindow.OperatorEnable = false;
                await Test();//メインルーチンへ


                //試験合格後、ラベル貼り付けページを表示する場合は下記のステップを追加すること
                if (Flags.ShowLabelPage) return;

                //日常点検合格、一項目試験合格、試験NGの場合は、Whileループを繰り返す
                //通常試験合格の場合は、ラベル貼り付けフォームがロードされた時点で、一旦StartCheckメソッドを終了します
                //その後、ラベル貼り付けフォームが閉じられた後に、Test.xamlがリロードされ、そのフォームロードイベントでStartCheckメソッドがコールされます

            }

        }

        private void Timer()
        {
            var t = Task.Run(() =>
            {
                //Stopwatchオブジェクトを作成する
                var sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                while (FlagTestTime)
                {
                    Thread.Sleep(200);
                    State.VmTestStatus.TestTime = sw.Elapsed.ToString().Substring(3, 5);
                }
                sw.Stop();
            });
        }

        //メインルーチン
        public async Task Test()
        {
            Flags.Click確認Button = false;
            Flags.Testing = true;

            State.VmTestStatus.Message = Constants.MessWait;

            //現在のテーマ透過度の保存
            State.CurrentThemeOpacity = State.VmMainWindow.ThemeOpacity;
            //テーマ透過度を最小にする
            General.SetRadius(true);

            General.cam.ImageOpacity = 1.0;
            State.SetCamPoint();
            State.SetCamPropForDef();
            General.cam.Start();//非同期メソッドの中で実行すると、カメラがうまく動作しないので注意！！！
            await Task.Delay(2500);

            FlagTestTime = true;
            Timer();

            int FailStepNo = 0;
            int RetryCnt = 0;//リトライ用に使用する
            string FailTitle = "";


            var テスト項目最新 = new List<TestDetail>();
            if (State.VmTestStatus.CheckUnitTest == true)
            {
                //チェックしてある項目の百の桁の解析
                var re = Int32.Parse(State.VmTestStatus.UnitTestName.Split('_').ToArray()[0]);
                int 上位桁 = Int32.Parse(State.VmTestStatus.UnitTestName.Substring(0, (re >= 1000) ? 2 : 1));
                var 抽出データ = State.テスト項目.Where(p => (p.Key / 100) == 上位桁);
                foreach (var p in 抽出データ)
                {
                    テスト項目最新.Add(new TestDetail(p.Key, p.Value, p.PowSw));
                }
            }
            else
            {
                テスト項目最新 = State.テスト項目;
            }



            try
            {
                //IO初期化
                General.ResetIo();
                Thread.Sleep(400);


                foreach (var d in テスト項目最新.Select((s, i) => new { i, s }))
                {
                    Retry:
                    State.VmTestStatus.Spec = "規格値 : ---";
                    State.VmTestStatus.MeasValue = "計測値 : ---";
                    Flags.AddDecision = true;

                    SetTestLog(d.s.Key.ToString() + "_" + d.s.Value);

                    if (d.s.PowSw)
                    {
                        if (!Flags.PowOn)
                        {
                            General.PowSupply_TestMode(true);
                        }
                    }
                    else
                    {
                        General.PowSupply_TestMode(false);
                    }

                    switch (d.s.Key)
                    {
                        case 100://FW書き込み
                            if (State.VmTestStatus.CheckWriteTestFwPass == true) break;
                            if (await TestWriteFw.WriteFw()) break;
                            RetryCnt = Constants.RetryCount;
                            goto case 5000;

                        case 200://VR1調整
                            if (await TestVr.AdjustVr()) break;
                            goto case 5000;

                        case 300://DSW1 ONチェック
                            if (await TestSw.CheckDsw1(true)) break;
                            goto case 5000;

                        case 301://DSW1 OFFチェック
                            if (await TestSw.CheckDsw1(false)) break;
                            goto case 5000;

                        case 400://消費電流チェック
                            if (await Test電流_電圧.CheckCurr()) break;
                            goto case 5000;

                        case 500://電源電圧Vccチェック
                            if (await Test電流_電圧.CheckVcc()) break;
                            goto case 5000;

                        case 600://P130発振周波数 チェック
                            if (await TestBuz.CheckP130()) break;
                            goto case 5000;

                        case 601://ブザー周波数 チェック
                            if (await TestBuz.CheckBuzFreq()) break;
                            goto case 5000;

                        case 602://ブザー音圧 チェック
                            if (await TestBuz.CheckBuzLev()) break;
                            goto case 5000;

                        case 700://タクトスイッチチェック
                            if (await TestSw.CheckSw()) break;
                            goto case 5000;

                        case 800://LED1(緑) チェック
                            if (await TestLed.Check(TestLed.NAME.LED1)) break;
                            goto case 5000;

                        case 801://LED2(緑) チェック
                            if (await TestLed.Check(TestLed.NAME.LED2)) break;
                            goto case 5000;

                        case 802://LED3(赤) チェック
                            if (await TestLed.Check(TestLed.NAME.LED3)) break;
                            goto case 5000;

                        case 803://LED4(橙) チェック
                            if (await TestLed.Check(TestLed.NAME.LED4)) break;
                            goto case 5000;

                        case 900://LCD(FIG1) チェック
                            if (await TestLcd.CheckLcd(TestLcd.FIG_NAME.FIG1)) break;
                            goto case 5000;

                        case 901://LCD(FIG2) チェック
                            if (await TestLcd.CheckLcd(TestLcd.FIG_NAME.FIG2)) break;
                            goto case 5000;

                        case 5000://NGだっときの処理
                            Flags.Retry = true;
                            if (Flags.AddDecision) SetTestLog("---- FAIL\r\n");
                            FailStepNo = d.s.Key;
                            FailTitle = d.s.Value;

                            General.ResetIo();
                            State.VmTestStatus.IsActiveRing = false;//リング表示してる可能性があるので念のため消す処理

                            if (Flags.ClickStopButton) goto FAIL;

                            if (RetryCnt++ != Constants.RetryCount)
                            {
                                //リトライ履歴リスト更新
                                State.RetryLogList.Add(FailStepNo.ToString() + "," + FailTitle + "," + System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                                General.cam.ResetFlag();//LEDテストでNGになったときは、カメラのフラグを初期化しないとNG枠が出たままになる
                                goto Retry;

                            }

                            var dialog = new DialogPic();
                            dialog.ShowDialog();

                            //何が選択されたか調べる
                            if (Flags.DialogReturn)
                            {
                                State.LoadTestSpec();
                                RetryCnt = 0;
                                await Task.Delay(1000);
                                goto Retry;
                            }

                            goto FAIL;//自動リトライ後の作業者への確認はしない


                    }
                    //↓↓各ステップが合格した時の処理です↓↓
                    if (Flags.AddDecision) SetTestLog("---- PASS\r\n");

                    State.VmTestStatus.IsActiveRing = false;

                    //リトライステータスをリセットする
                    RetryCnt = 0;
                    Flags.Retry = false;

                    await Task.Run(() =>
                    {
                        var CurrentProgValue = State.VmTestStatus.進捗度;
                        var NextProgValue = (int)(((d.i + 1) / (double)テスト項目最新.Count()) * 100);
                        var 変化量 = NextProgValue - CurrentProgValue;
                        foreach (var p in Enumerable.Range(1, 変化量))
                        {
                            State.VmTestStatus.進捗度 = CurrentProgValue + p;
                            if (State.VmTestStatus.CheckUnitTest == false)
                            {
                                Thread.Sleep(5);
                            }
                        }
                    });
                    if (Flags.ClickStopButton) goto FAIL;
                }


                //↓↓すべての項目が合格した時の処理です↓↓
                General.ResetIo();
                await Task.Delay(500);
                State.VmTestStatus.StartButtonContent = Constants.確認;
                await General.cam.Stop();

                FlagTestTime = false;

                State.VmTestStatus.Colorlabel判定 = Brushes.AntiqueWhite;
                State.VmTestStatus.Decision = "PASS";
                State.VmTestStatus.ColorDecision = effect判定表示PASS;

                ResetRing();
                SetDecision();
                SbPass();


                //通しで試験が合格したときの処理です(検査データを保存して、シリアルナンバーをインクリメントする)
                if (State.VmTestStatus.CheckUnitTest != true) //null or False アプリ立ち上げ時はnullになっている！
                {
                    General.SaveTestData();
                    General.StampOn();//合格印押し

                    //当日試験合格数をインクリメント ビューモデルはまだ更新しない
                    State.Setting.TodayOkCount++;
                    Flags.ShowLabelPage = true;
                    General.PlaySound(General.soundPass);
                    await Task.Delay(3900);
                    State.VmTestStatus.StartButtonEnable = true;
                    return;
                }
                else
                {
                    State.VmTestStatus.Message = Constants.MessRemove;
                    Flags.ShowLabelPage = false;

                    StopButtonBlinkOn();
                    State.VmTestStatus.StartButtonEnable = true;
                    State.VmTestStatus.StartButtonContent = Constants.確認;
                    await Task.Run(() =>
                    {
                        while (true)
                        {
                            if (Flags.Click確認Button)
                            {
                                break;
                            }
                            Thread.Sleep(100);
                        }
                    });
                    StopButtonBlinkOff();
                    return;
                }


                //不合格時の処理
                FAIL:

                General.ResetIo();
                await Task.Delay(500);


                FlagTestTime = false;
                State.VmTestStatus.Message = Constants.MessRemove;


                //当日試験不合格数をインクリメント ビューモデルはまだ更新しない
                State.Setting.TodayNgCount++;
                await Task.Delay(100);

                State.VmTestStatus.Colorlabel判定 = Brushes.AliceBlue;
                State.VmTestStatus.Decision = "FAIL";
                State.VmTestStatus.ColorDecision = effect判定表示FAIL;

                SetErrorMessage(FailStepNo, FailTitle);

                var NgDataList = new List<string>()
                                    {
                                        State.VmMainWindow.Opecode,
                                        State.VmMainWindow.Operator,
                                        System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                                        State.VmTestStatus.FailInfo,
                                        State.VmTestStatus.Spec,
                                        State.VmTestStatus.MeasValue
                                    };

                General.SaveNgData(NgDataList);


                ResetRing();
                SetDecision();
                SetErrInfo();
                SbFail();

                General.PlaySound(General.soundFail);
                StopButtonBlinkOn();
                State.VmTestStatus.StartButtonEnable = true;
                State.VmTestStatus.StartButtonContent = Constants.確認;
                await Task.Run(() =>
                {
                    while (true)
                    {
                        if (Flags.Click確認Button)
                        {
                            break;
                        }
                        Thread.Sleep(100);
                    }
                });
                StopButtonBlinkOff();

                return;


            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("想定外の例外発生DEATH！！！\r\n申し訳ありませんが再起動してください");
                Environment.Exit(0);

            }
            finally
            {
                General.ResetIo();
                SbRingLoad();
                General.cam.ResetFlag();
                await General.cam.Stop();

                if (Flags.ShowLabelPage)
                {
                    State.uriOtherInfoPage = new Uri("Page/Test/ラベル貼り付け.xaml", UriKind.Relative);
                    State.VmMainWindow.TabIndex = 3;
                }
                else
                {
                    General.ResetViewModel();
                    RefreshDataContext();
                }
            }

        }

        //フォームきれいにする処理いろいろ
        private void ClearForm()
        {
            SbRingLoad();
            RefreshDataContext();
        }

        private void SetErrorMessage(int stepNo, string title)
        {
            if (Flags.ClickStopButton)
            {
                State.VmTestStatus.FailInfo = "エラーコード ---     強制停止";
            }
            else
            {
                State.VmTestStatus.FailInfo = "エラーコード " + stepNo.ToString("00") + "   " + title + "異常";
            }
        }

        //テストログの更新
        private void SetTestLog(string addData)
        {
            State.VmTestStatus.TestLog += addData;
        }

        private void ResetRing()
        {
            State.VmTestStatus.RingVisibility = System.Windows.Visibility.Hidden;

        }

        private void SetDecision()
        {
            State.VmTestStatus.DecisionVisibility = System.Windows.Visibility.Visible;
        }

        private void SetErrInfo()
        {
            State.VmTestStatus.ErrInfoVisibility = System.Windows.Visibility.Visible;
        }



    }
}
