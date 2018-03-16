using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Windows.Forms;
using System.Timers;
using System.Threading;
using static System.Threading.Thread;

namespace PC80_Tester
{

    public class Agilent34401A : IMultimeter
    {
        //列挙型の宣言
        public enum ErrorCode { NORMAL, CMD_ERR, TIMEOUT_ERR, OPEN_ERR, SEND_ERR, OTHER }
        public enum MeasMode { DEFAULT, DCV, DCA, RES, FREQ }

        public ErrorCode state { get; set; }
        public MeasMode mode { get; set; }

        //private const string ComName = "通信ポート";
        private const string ComName = "USB Serial Port ";
        private const string ID_34401A = "HEWLETT-PACKARD,34401A";

        //変数の宣言（インスタンスメンバーになります）
        private static SerialPort port;
        private string RecieveData;//34401Aから受信した生データ

        private double voltData;
        private double currData;
        private double resData;
        private double freqData;

        double IMultimeter.VoltData { get { return voltData; } set { voltData = value; } }

        double IMultimeter.CurrData { get { return currData; } set { currData = value; } }

        double IMultimeter.ResData { get { return resData; } set { resData = value; } }

        double IMultimeter.FreqData { get { return freqData; } set { freqData = value; } }



        //コンストラクタ
        public Agilent34401A()
        {
            port = new SerialPort();
            mode = MeasMode.DEFAULT;
        }


        //**************************************************************************
        //34401Aの初期化
        //引数：なし
        //戻値：なし
        //**************************************************************************
        bool IMultimeter.Init()
        {
            try
            {
                //Comポートリストの取得
                var ListComNo = FindSerialPort.GetComNo(ComName);
                if (ListComNo.Count() != 1) return false;

                if (!port.IsOpen)
                {
                    //Agilent34401A用のシリアルポート設定
                    port.PortName = ListComNo[0]; //この時点で既にポートが開いている場合COM番号は設定できず例外となる（イニシャライズは１回のみ有効）
                    port.BaudRate = 9600;
                    port.DataBits = 8;
                    port.Parity = System.IO.Ports.Parity.None;
                    port.StopBits = System.IO.Ports.StopBits.One;
                    port.DtrEnable = true;//これ設定しないとコマンド送るたびにErrorになります！
                    port.NewLine = ("\r\n");
                    port.Open();
                }

                //コマンド送信
                port.WriteLine(":SYST:REM");
                ReadRecieveData(1000);
                port.WriteLine("*CLS");
                ReadRecieveData(1000);
                port.WriteLine("*RST");
                ReadRecieveData(1000);

                //クエリ送信
                port.WriteLine("*IDN?");
                ReadRecieveData(1000);
                if (RecieveData.Contains(ID_34401A))
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

        private void SetRemote()
        {
            //コマンド送信
            port.WriteLine(":SYST:REM");
            ReadRecieveData(1000);
            port.WriteLine("*CLS");
            ReadRecieveData(1000);
            port.WriteLine("*RST");
            ReadRecieveData(1000);

        }

        //**************************************************************************
        //34401Aからの受信データを読み取る
        //引数：指定時間（ｍｓｅｃ）
        //戻値：ErrorCode
        //**************************************************************************
        private bool ReadRecieveData(int time)
        {

            RecieveData = null;//念のため初期化
            string buffer = null;
            port.ReadTimeout = time;
            try
            {
                buffer = port.ReadTo("\r\n");
            }
            catch (TimeoutException)
            {
                return false;
            }

            RecieveData = buffer;
            return true;
        }


        //**************************************************************************
        //COMポートを閉じる
        //引数：なし
        //戻値：bool
        //**************************************************************************   

        bool IMultimeter.ClosePort()
        {
            try
            {
                if (port.IsOpen)
                {
                    port.WriteLine("*RST");
                    port.WriteLine(":SYST:LOC");//ローカル設定に戻してからCOMポート閉じる
                    port.Close();
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
                port.WriteLine(":MEAS:VOLT:DC?");
                Sleep(500);

                bool respons = ReadRecieveData(1000);
                if (!respons)
                {
                    return respons;//falseが返ります
                }

                voltData = Double.Parse(RecieveData);

                return true;
            }
            catch
            {
                return false;

            }
        }

        bool IMultimeter.GetDcA()
        {
            try
            {
                port.WriteLine(":MEAS:CURR:DC?");


                bool respons = ReadRecieveData(2000);
                if (!respons)
                {
                    return respons;//falseが返ります
                }

                currData = Double.Parse(RecieveData);
                return true;
            }
            catch
            {
                return false;

            }
        }

        bool IMultimeter.GetRes()
        {
            try
            {
                port.WriteLine(":MEAS:RES?");
                bool respons = ReadRecieveData(2000);
                if (!respons)
                {
                    return respons;//falseが返ります
                }

                resData = Double.Parse(RecieveData);

                return true;
            }
            catch
            {
                return false;

            }

        }

        bool IMultimeter.SetFreq()
        {
            return true;
        }

        bool IMultimeter.GetFreq()
        {
            try
            {
                port.WriteLine(":MEAS:FREQ?");
                bool respons = ReadRecieveData(5000);
                if (!respons)
                {
                    return respons;//falseが返ります
                }

                freqData = Double.Parse(RecieveData);

                return true;
            }
            catch
            {
                return false;

            }

        }
    }


}