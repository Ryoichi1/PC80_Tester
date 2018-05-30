using System;
using System.Threading;
using System.Threading.Tasks;
using static System.Threading.Thread;
using static PC80_Tester.General;
using static PC80_Tester.State;

namespace PC80_Tester
{
    public static class Test電流_電圧
    {
        public static async Task<bool> CheckVcc()
        {
            bool result = false;
            Double measData = 0;
            double Max = Spec.VccMax;
            double Min = Spec.VccMin;

            try
            {
                return await Task<bool>.Run(() =>
                {
                    try
                    {
                        ResetIo();
                        SetRelayForVccCheck();
                        PowSupply_TestMode(true);
                        Thread.Sleep(1000);

                        multimeter.GetDcV();
                        measData = multimeter.VoltData;

                        return result = (Min < measData && measData < Max);
                    }
                    catch
                    {
                        return result = false;
                    }

                });
            }
            finally
            {
                ResetIo();

                //ビューモデルの更新
                VmTestResults.Vol5v = measData.ToString("F2") + "V";
                VmTestResults.ColVol5v = result ? OffBrush : NgBrush;

                //NGだった場合、エラー詳細情報の規格値を更新する
                if (!result)
                {
                    VmTestStatus.Spec = $"規格値 : {Min.ToString("F2")} ～ {Max.ToString("F2")}V";
                    VmTestStatus.MeasValue = $"計測値 : {measData.ToString("F2")}V";

                }
            }
        }


        public static async Task<bool> CheckCurr()
        {
            bool result = false;
            Double measData = 0;
            double Max = Spec.CurrMax;
            double Min = Spec.CurrMin;

            try
            {
                return await Task<bool>.Run(() =>
                {
                    try
                    {
                        ResetIo();
                        SetRelayForCurrCheck();
                        Thread.Sleep(1500);

                        multimeter.GetDcA();
                        measData = multimeter.CurrData;

                        return result = (Min < measData && measData < Max);
                    }
                    catch
                    {
                        return result = false;
                    }

                });
            }
            finally
            {

                ResetIo();

                //ビューモデルの更新
                VmTestResults.Curr = measData.ToString("F2") + "mA";
                VmTestResults.ColCurr = result ? OffBrush : NgBrush;

                //NGだった場合、エラー詳細情報の規格値を更新する
                if (!result)
                {
                    VmTestStatus.Spec = $"規格値 : {Min.ToString("F2")} ～ {Max.ToString("F2")}mA";
                    VmTestStatus.MeasValue = $"計測値 : {measData.ToString("F2")}mA";

                }
            }
        }

    }
}
