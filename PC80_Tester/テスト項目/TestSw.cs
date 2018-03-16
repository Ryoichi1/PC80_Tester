using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Threading.Thread;
using static PC80_Tester.General;
using static PC80_Tester.State;

namespace PC80_Tester
{
    public static class TestSw
    {
        private enum SW_NAME
        {
            SW1,
            SW2,
            SW3,
            SW4,
            SW5,
            SW6,
            SW7,
            SW8,
            SW9,
        }

        public static async Task<bool> CheckSw()
        {
            bool result = false;
            SW_NAME name;

            try
            {
                return result = await Task<bool>.Run(() =>
                {
                    Sleep(1000);
                    Flags.AddDecision = false;
                    VmTestStatus.TestLog += "\r\n";
                    foreach (var n in Enum.GetValues(typeof(SW_NAME)))
                    {
                        name = (SW_NAME)n;
                        VmTestStatus.TestLog += $"{name.ToString()} ONチェック";
                        Target.ClearBuff();

                        //該当のスイッチをオンする
                        SetSw(name, true);

                        result = Target.ReadSwData() && Target.RecieveData.Contains($"@{name.ToString()}");

                        SetSw(name, false);

                        if (result)
                        {
                            VmTestStatus.TestLog += "---PASS\r\n";
                        }
                        else
                        {
                            VmTestStatus.TestLog += "---FAIL\r\n";
                            return false;
                        }
                        Sleep(500);
                    }
                    return true;
                });
            }
            finally
            {

            }

            //ローカル関数の定義
            void SetSw(SW_NAME n, bool sw)
            {
                switch (n)
                {
                    case SW_NAME.SW1:
                        SetSw1(sw ? true : false);
                        break;
                    case SW_NAME.SW2:
                        SetSw2(sw ? true : false);
                        break;
                    case SW_NAME.SW3:
                        SetSw3(sw ? true : false);
                        break;
                    case SW_NAME.SW4:
                        SetSw4(sw ? true : false);
                        break;
                    case SW_NAME.SW5:
                        SetSw5(sw ? true : false);
                        break;
                    case SW_NAME.SW6:
                        SetSw6(sw ? true : false);
                        break;
                    case SW_NAME.SW7:
                        SetSw7(sw ? true : false);
                        break;
                    case SW_NAME.SW8:
                        SetSw8(sw ? true : false);
                        break;
                    case SW_NAME.SW9:
                        SetSw9(sw ? true : false);
                        break;
                    default:
                        break;
                }
            }
        }

        public static async Task<bool> CheckDsw1(bool sw)
        {
            bool result = false;

            try
            {
                return result = await Task<bool>.Run(() =>
                {
                    if (sw)
                    {
                        Target.SendData("DSW");
                        if (Target.RecieveData.Contains("DSWON"))
                        {
                            return true;
                        }
                    }
                    //お知らせ音
                    PlaySound(soundNotice);
                    State.VmTestStatus.Message = sw ? "DSW1の1番をON（↓）してください" : "DSW1の1番をOFF（↑）してください";
                    while (true)
                    {
                        if (Flags.ClickStopButton) return false;
                        Target.SendData("DSW");
                        if (Target.RecieveData.Contains(sw? "DSWON" :"DSWOFF"))
                        {
                            return true;
                        }
                        Sleep(400);
                    }
                });
            }
            finally
            {
                if (!sw)
                    VmTestStatus.Message = Constants.MessWait;
            }
        }

    }
}
