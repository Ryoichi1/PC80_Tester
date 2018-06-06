using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace PC80_Tester
{
    public static class Target
    {
        private const string ComName = "MOXA USB Serial Port ";
        //変数の宣言（インスタンスメンバーになります）
        private static SerialPort port;
        public static string RecieveData { get; set; }//Targetから受信した生データ

        static Target()
        {
            port = new SerialPort();
        }


        //**************************************************************************
        //LPC1768の初期化
        //**************************************************************************
        public static bool Init()
        {
            var result = false;
            try
            {
                var comNum = FindSerialPort.GetComNo(ComName);
                if (comNum.Count() != 1) return false;

                if (!port.IsOpen)
                {
                    //Agilent34401A用のシリアルポート設定
                    port.PortName = comNum[0]; //この時点で既にポートが開いている場合COM番号は設定できず例外となる（イニシャライズは１回のみ有効）
                    port.BaudRate = 19200;
                    port.DataBits = 8;
                    port.Parity = System.IO.Ports.Parity.Even;
                    port.StopBits = System.IO.Ports.StopBits.One;
                    port.DtrEnable = true;
                    port.NewLine = ("\r\n");
                    port.Open();
                }
                return result = true;
            }
            catch
            {
                return result = false;
            }
            finally
            {
                if (!result)
                {
                    ClosePort();
                }
            }
        }

        //**************************************************************************
        //LPC1768を制御する 
        //**************************************************************************
       public static bool SendData(string Data, int Wait = 700, bool setLog = true)
        {
            int retryCount = 0;

            RETRY:
            //送信処理
            try
            {
                State.VmComm.TX_TARGET = "";
                State.VmComm.RX_TARGET = "";

                ClearBuff();//受信バッファのクリア

                port.WriteLine(Data);// \r\n は自動的に付加されます
                if (setLog) State.VmComm.TX_TARGET = Data;
            }
            catch
            {
                State.VmComm.TX_TARGET = "TX_Error";
                return false;
            }

            //受信処理
            try
            {
                RecieveData = "";//初期化
                port.ReadTimeout = Wait;
                RecieveData = port.ReadTo("\r");
                if (setLog) State.VmComm.RX_TARGET = RecieveData;
                return true;
            }
            catch
            {
                State.VmComm.RX_TARGET = "TimeoutErr";
                if (++retryCount == 4)
                    return false;

                Thread.Sleep(400);
                goto RETRY;
            }
            finally
            {
                Thread.Sleep(100);
            }
        }

        public static bool ReadSwData()
        {
            try
            {
                port.ReadTimeout = 1500;
                RecieveData = port.ReadTo("\r");
                State.VmComm.RX_TARGET = RecieveData;
                return true;
            }
            catch
            {
                State.VmComm.RX_TARGET = "TimeOutError";
                return false;
            }
        }


        //**************************************************************************
        //受信バッファをクリアする
        //**************************************************************************
        public static void ClearBuff()
        {
            if (port.IsOpen)
                port.DiscardInBuffer();
        }


        //**************************************************************************
        //COMポートを閉じる
        //引数：なし
        //戻値：bool
        //**************************************************************************   
        public static bool ClosePort()
        {
            try
            {
                //ポートが開いているかどうかの判定
                if (port.IsOpen)
                {
                    port.Close();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }





    }

}

