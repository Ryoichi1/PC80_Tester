using System.Threading.Tasks;
using static System.Threading.Thread;
using static PC80_Tester.General;
using static PC80_Tester.State;

namespace PC80_Tester
{
    public static class TestVr
    {
        public static async Task<bool> AdjustVr()
        {
            var result = false;
            try
            {
                return result = await Task<bool>.Run(() =>
                {
                    int okCount = 0;
                    double Value_kOhm = 0;
                    bool DoSound = false;

                    //K1,K2をONする
                    SetRelayForAdjustVr();

                    //お知らせ音
                    PlaySound(soundNotice);
                    VmTestStatus.Message = ((Spec.VrMax + Spec.VrMin) / 2).ToString("F2") + "Vにあわせる";
                    while (true)
                    {
                        if (Flags.ClickStopButton) return false;
                        if (okCount == 10) break;

                        if (!multimeter.GetDcV()) continue;
                        Value_kOhm = multimeter.VoltData;
                        VmTestStatus.Message = ((Spec.VrMax + Spec.VrMin) / 2).ToString("F2") + "Vにあわせる ---> " + Value_kOhm.ToString("F3") + "V";
                        VmMainWindow.VrValue = "計測値 " + Value_kOhm.ToString("F2") + "V";

                        if (Value_kOhm < Spec.VrMin)
                        {
                            VmMainWindow.Flyout = true;
                            VmMainWindow.FlyoutSrc = Constants.PathVR_L;
                            okCount = 0;
                            DoSound = true;
                        }
                        else if (Value_kOhm > Spec.VrMax)
                        {
                            VmMainWindow.Flyout = true;
                            VmMainWindow.FlyoutSrc = Constants.PathVR_R;
                            okCount = 0;
                            DoSound = true;
                        }
                        else
                        {
                            if (DoSound)
                            {
                                PlaySound(soundSuccess);
                                DoSound = false;
                            }
                            VmTestStatus.Message = $"調整OK！ 安定するのを待ちます。。。 {Value_kOhm.ToString("F2")}V";
                            okCount++;
                        }

                        Sleep(300);
                    }

                    VmTestResults.Vr = Value_kOhm.ToString("F2") + "V";
                    return true;
                });
            }
            finally
            {
                ResetIo();
                VmMainWindow.Flyout = false;
                VmTestStatus.Message = Constants.MessWait;
            }
        }

    }
}
