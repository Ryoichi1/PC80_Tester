using System;
using System.IO.Ports;
using System.Linq;
using static System.Threading.Thread;

namespace PC80_Tester
{
    class R6441B : IMultimeter
    {
        //列挙型の宣言
        public enum ErrorCode { NORMAL, CMD_ERR, TIMEOUT_ERR, OPEN_ERR, SEND_ERR, OTHER }
        public enum MeasMode { DEFAULT, DCV, DCA, RES, FREQ }

        public ErrorCode state { get; set; }
        public MeasMode mode { get; set; }

        //private const string ComName = "通信ポート";
        private const string ComName = "USB Serial Port ";

        private SerialPort port;

        private string RecieveData;//マルチメータから受信した生データ

        private double voltData;
        private double currData;
        private double resData;
        private double freqData;



        double IMultimeter.VoltData { get { return voltData; } set { voltData = value; } }

        double IMultimeter.CurrData { get { return currData; } set { currData = value; } }

        double IMultimeter.ResData { get { return resData; } set { resData = value; } }

        double IMultimeter.FreqData { get { return freqData; } set { freqData = value; } }

        public R6441B()
        {
            port = new SerialPort();
            mode = MeasMode.DEFAULT;
        }


        bool IMultimeter.ClosePort()
        {
            try
            {
                if (port.IsOpen) port.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

        bool IMultimeter.Init()
        {
            try
            {
                //Comポートリストの取得
                var ListComNo = FindSerialPort.GetComNo(ComName);
                if (ListComNo.Count() != 1) return false;

                //Agilent34401A用のシリアルポート設定
                port.PortName = ListComNo[0];
                port.BaudRate = 9600;
                port.DataBits = 8;
                port.Parity = System.IO.Ports.Parity.None;
                port.StopBits = System.IO.Ports.StopBits.One;
                port.DtrEnable = true;//これ設定しないとコマンド送るたびにErrorになります！
                port.NewLine = ("\r\n");
                port.Open();
                //クエリ送信
                Sleep(500);
                if (SendData("Z,F1,R5,PR2") && ReadRecieveData(1000)) //Z：初期化 コマンドを送信して=>の返信があるかチェックする
                {
                    return true;
                }
                else
                {
                    //開いたポートが間違っているのでいったん閉じる
                    port.Close();
                    state = ErrorCode.OPEN_ERR;
                    return false;
                }
            }
            catch
            {
                port.Close();
                state = ErrorCode.OPEN_ERR;
                return false;
            }
        }

        bool IMultimeter.GetDcA()
        {
            try
            {
                if (mode != MeasMode.DCA)
                {
                    //if (!SendData("Z,F5,R6,PR2")) return false; //Z：初期化,F5：直流電流測定,R6：200mAレンジ,PR2：サンプリングレートMid
                    if (!SendData("Z,F5")) return false; //オートレンジ設定（マルチメータの電流端子H/Lどちらにも対応 デバッグ中にL側のFUSEが切れたためH側に変更）
                    if (!ReadRecieveData(1000)) return false;
                    Sleep(2000);//モード変えたらウェイト
                    mode = MeasMode.DCA;
                }

                Sleep(1000);
                SendData("MD?"); //'MD?:測定データ出力要求
                if (!ReadRecieveData(1000))
                {
                    return false;
                }

                currData = Double.Parse(RecieveData.Substring(8, 10)) * 1000;
                return true;
            }
            catch
            {
                mode = MeasMode.DEFAULT;
                return false;
            }
        }

        bool IMultimeter.GetRes()
        {
            try
            {
                if (mode != MeasMode.RES)
                {
                    if (!SendData("Z,F3,R5,PR2")) return false; // Z：初期化,F3：抵抗測定,R5：**レンジ,PR2：サンプリングレートMid
                    if (!ReadRecieveData(1000)) return false;
                    Sleep(2000);//モード変えたらウェイト
                    mode = MeasMode.DCV;
                }
                SendData("MD?"); //'MD?:測定データ出力要求
                if (!ReadRecieveData(1000))
                {
                    return false;
                }

                resData = Double.Parse(RecieveData.Substring(8, 10)) / 1000;
                return true;
            }
            catch
            {
                mode = MeasMode.DEFAULT;
                return false;
            }

        }

        bool IMultimeter.GetFreq()
        {
            try
            {
                SendData("MD?"); //'MD?:測定データ出力要求
                if (!ReadRecieveData(1000))
                {
                    return false;
                }

                freqData = Double.Parse(RecieveData.Substring(9, 9));
                return true;
            }
            catch
            {
                return false;
            }
        }
        bool IMultimeter.SetFreq()
        {
            try
            {
                if (mode != MeasMode.FREQ)
                {
                    if (!SendData("Z,F50,R5,PR1")) return false; //Z：初期化,F50：周波数測定,R5：20kHzレンジ,PR1：サンプリングレートFAST
                    if (!ReadRecieveData(1000)) return false;
                    Sleep(1000);//モード変えたらウェイト
                    mode = MeasMode.FREQ;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }


        bool IMultimeter.GetDcV()
        {
            try
            {
                if (mode != MeasMode.DCV)
                {
                    if (!SendData("Z,F1,R5,PR2")) return false; // Z：初期化,F1：直流電圧測定,R5：20Vレンジ,PR2：サンプリングレートMid
                    if (!ReadRecieveData(1000)) return false;
                    Sleep(2000);//モード変えたらウェイト
                    mode = MeasMode.DCV;
                }
                SendData("MD?"); //'MD?:測定データ出力要求
                if (!ReadRecieveData(1000))
                {
                    return false;
                }

                voltData = Double.Parse(RecieveData.Substring(8, 10));
                return true;
            }
            catch
            {
                mode = MeasMode.DEFAULT;
                return false;
            }
        }

        //**************************************************************************
        //ターゲットにコマンドを送る
        //引数：なし
        //戻値：bool
        //**************************************************************************
        private bool SendData(string cmd)
        {
            try
            {
                port.DiscardInBuffer();//COM受信バッファクリア
                port.WriteLine(cmd);
                return true;
            }
            catch
            {
                return false;
            }
        }


        //**************************************************************************
        //R6441Bからの受信データを読み取る
        //引数：指定時間（ｍｓｅｃ）
        //戻値：ErrorCode
        //**************************************************************************
        private bool ReadRecieveData(int time)
        {
            RecieveData = null;//念のため初期化
            port.ReadTimeout = time;
            try
            {
                RecieveData = port.ReadTo("=>\r\n");
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
