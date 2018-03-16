
namespace PC80_Tester
{
    public interface IMultimeter
    {
        //プロパティの定義
        double VoltData { get; set; }//計測した電圧値
        double CurrData { get; set; }//計測した電流値
        double ResData { get; set; }//計測した抵抗値
        double FreqData { get; set; }//計測した周波数


        //メソッドの定義

        //イニシャライズ
        bool Init();

        //計測
        bool GetDcV();
        bool GetDcA();
        bool GetRes();
        bool SetFreq();
        bool GetFreq();

        //ポートを閉じる処理
        bool ClosePort();
        
    }

}
