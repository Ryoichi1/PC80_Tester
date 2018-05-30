

namespace PC80_Tester
{
    public class CameraPropertyForLed
    {
        public int BinLevel { get; set; }
        public bool Opening { get; set; }//オープニング処理 or クロージング処理
        public int OpenCnt { get; set; }//クロージング処理時の拡張回数
        public int CloseCnt { get; set; }//クロージング処理時の収縮回数

        //カメラプロパティ
        public int CamNumber { get; set; }
        public double Brightness { get; set; }
        public double Contrast { get; set; }
        public double Hue { get; set; }
        public double Saturation { get; set; }
        public double Sharpness { get; set; }
        public double Gamma { get; set; }
        public double Gain { get; set; }
        public double Exposure { get; set; }
        public int Whitebalance { get; set; }
        public double Theta { get; set; }


        //LEDの座標
        public string PointLed1 { get; set; }
        public string PointLed2 { get; set; }
        public string PointLed3 { get; set; }
        public string PointLed4 { get; set; }

        //LEDの輝度
        public string LumLed1 { get; set; }
        public string LumLed2 { get; set; }
        public string LumLed3 { get; set; }
        public string LumLed4 { get; set; }

        //LEDの色相
        public string HueLed1 { get; set; }
        public string HueLed2 { get; set; }
        public string HueLed3 { get; set; }
        public string HueLed4 { get; set; }




    }
}
