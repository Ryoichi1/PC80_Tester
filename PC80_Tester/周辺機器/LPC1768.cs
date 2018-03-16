using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace PC80_Tester
{
    public static class LPC1768
    {
        private const string ID_LPC1768 = "PC80_TESTER_V";
        private const string ComName = "mbed Serial Port ";
        //変数の宣言（インスタンスメンバーになります）
        private static SerialPort port;
        public static string RecieveData { get; set; }//LPC1768から受信した生データ

        static LPC1768()
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
                    port.BaudRate = 115200;
                    port.DataBits = 8;
                    port.Parity = System.IO.Ports.Parity.None;
                    port.StopBits = System.IO.Ports.StopBits.One;
                    //port.DtrEnable = true;
                    port.NewLine = ("\r\n");
                    port.Open();
                }


                //クエリ送信
                if (!SendData("*IDN?")) return false;
                return result = RecieveData.Contains(ID_LPC1768);
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
        public static bool SendData(string Data, int Wait = 1000, bool setLog = true)
        {
            //送信処理
            try
            {
                State.VmComm.TX_IO = "";
                State.VmComm.RX_IO = "";

                ClearBuff();//受信バッファのクリア

                port.WriteLine(Data);// \r\n は自動的に付加されます
                if (setLog) State.VmComm.TX_IO = Data;
            }
            catch
            {
                State.VmComm.TX_IO = "TX_Error";
                return false;
            }

            //受信処理
            try
            {
                RecieveData = "";//初期化
                port.ReadTimeout = Wait;
                RecieveData = port.ReadLine();
                if (setLog) State.VmComm.RX_IO = RecieveData;
                return true;
            }
            catch
            {
                State.VmComm.RX_IO = "TimeoutErr";
                return false;
            }
            finally
            {
                Thread.Sleep(20);
            }
        }


        //**************************************************************************
        //受信バッファをクリアする
        //**************************************************************************
        private static void ClearBuff()
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
                    SendData("ResetIo");//製品を初期化して終了
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

