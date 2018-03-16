

namespace PC80_Tester
{
    public class CameraPropertyForLcd
    {
        //ノイズ処理
        public int BinLevel { get; set; }
        public bool Opening { get; set; }//オープニング処理 or クロージング処理
        public int OpenCnt { get; set; }//クロージング処理時の拡張回数
        public int CloseCnt { get; set; }//クロージング処理時の収縮回数

        //カメラプロパティ
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

        //マスター画像の位置と大きさ
        //上段左
        public int X_UP_L { get; set; }
        public int Y_UP_L { get; set; }
        public int W_UP_L { get; set; }
        public int H_UP_L { get; set; }
        //上段右
        public int X_UP_R { get; set; }
        public int Y_UP_R { get; set; }
        public int W_UP_R { get; set; }
        public int H_UP_R { get; set; }

        //下段左
        public int X_LO_L { get; set; }
        public int Y_LO_L { get; set; }
        public int W_LO_L { get; set; }
        public int H_LO_L { get; set; }
        //下段右
        public int X_LO_R { get; set; }
        public int Y_LO_R { get; set; }
        public int W_LO_R { get; set; }
        public int H_LO_R { get; set; }




    }
}
