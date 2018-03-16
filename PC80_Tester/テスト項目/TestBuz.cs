using System.Threading.Tasks;
using static System.Threading.Thread;
using static PC80_Tester.General;
using static PC80_Tester.State;

namespace PC80_Tester
{
    public static class TestBuz
    {

        public static async Task<bool> CheckP130()
        {
            var result = false;
            double measData = 0;
            double Max = Spec.P130Max;
            double Min = Spec.P130Min;
            try
            {
                return result = await Task<bool>.Run(() =>
                {
                    multimeter.SetFreq();
                    ResetIo();
                    SetRelayForP130Check();
                    PowSupply_TestMode(true);

                    //ターゲットにブザーオン指令を出す
                    if (!Target.SendData("BZON")) return false;
                    Sleep(5000);
                    if (!multimeter.GetFreq()) return false;
                    measData = multimeter.FreqData;
                    //周波数の判定
                    return result = (Min < measData && measData < Max);

                });
            }
            finally
            {
                VmTestResults.P130 = measData.ToString("F2") + "Hz";
                VmTestResults.ColP130 = result ? OffBrush : NgBrush;


                //NGだった場合、エラー詳細情報の規格値を更新する
                if (!result)
                {
                    VmTestStatus.Spec = $"規格値 : {Min.ToString("F2")} ～ {Max.ToString("F2")}Hz";
                    VmTestStatus.MeasValue = $"計測値 : {measData.ToString("F2")}Hz";

                }
            }
        }

        public static async Task<bool> CheckBuzFreq()
        {
            var result = false;
            double measData = 0;
            double Max = Spec.P130Max;
            double Min = Spec.P130Min;
            try
            {
                return result = await Task<bool>.Run(() =>
                {
                    if (Flags.Retry)
                    {
                        ResetIo();
                        PowSupply_TestMode(true);
                        Sleep(2000);
                        //ターゲットにブザーオン指令を出す
                        if (!Target.SendData("BZON")) return false;
                        Sleep(2000);
                    }

                    measData = wv.Freq;

                    //周波数の判定
                    return result = (Min < measData && measData < Max);
                });
            }
            finally
            {
                VmTestResults.Buz = measData.ToString("F2") + "Hz";
                VmTestResults.ColPBuz = result ? OffBrush : NgBrush;

                //NGだった場合、エラー詳細情報の規格値を更新する
                if (!result)
                {
                    wv.Close();
                    wv = new FFT();
                    wv.Init();
                    
                    VmTestStatus.Spec = $"規格値 : {Min.ToString("F2")} ～ {Max.ToString("F2")}Hz";
                    VmTestStatus.MeasValue = $"計測値 : {measData.ToString("F2")}Hz";

                }
            }
        }

        public static async Task<bool> CheckBuzLev()
        {
            var result = false;
            double measData = 0;
            double Min = Spec.BzMin;
            try
            {
                return result = await Task<bool>.Run(() =>
                {
                    if (Flags.Retry)
                    {
                        ResetIo();
                        PowSupply_TestMode(true);
                        Sleep(2000);
                        //ターゲットにブザーオン指令を出す
                        if (!Target.SendData("BZON")) return false;
                        Sleep(2000);
                    }

                    measData = wv.Vol;

                    //周波数の判定
                    return result = measData > Min;
                });
            }
            finally
            {
                ResetIo();
                VmTestResults.BuzLev = measData.ToString("F2");
                VmTestResults.ColBuzLev = result ? OffBrush : NgBrush;

                //NGだった場合、エラー詳細情報の規格値を更新する
                if (!result)
                {
                    VmTestStatus.Spec = $"規格値 : {Min.ToString("F2")}以上";
                    VmTestStatus.MeasValue = $"計測値 : {measData.ToString("F2")}";

                }
            }
        }

    }
}
