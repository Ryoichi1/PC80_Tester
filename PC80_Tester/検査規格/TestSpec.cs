
namespace PC80_Tester
{
    public class TestSpec
    {
        //テストスペックVER
        public string TestSpecVer { get; set; }

        //ファームウェア
        public string FwVer { get; set; }
        public string FwSum { get; set; }

        //電源電圧5V
        public double VccMax { get; set; }
        public double VccMin { get; set; }

        //消費電流
        public double CurrMax { get; set; }
        public double CurrMin { get; set; }

        //P130周波数
        public double P130Max { get; set; }
        public double P130Min { get; set; }

        //ブザー音
        public double BzMin { get; set; }

        //VR調整時のVo電圧（LCDのVoピンに入力される電圧）
        public double VrMax { get; set; }
        public double VrMin { get; set; }

        //LED 色相
        public int RedHueMax { get; set; }
        public int RedHueMin { get; set; }

        public int OrangeHueMax { get; set; }
        public int OrangeHueMin { get; set; }

        public int GreenHueMax { get; set; }
        public int GreenHueMin { get; set; }

        //LCD一致率
        public double LcdMatchMin { get; set; }

    }
}
