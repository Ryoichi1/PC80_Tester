using System.Threading.Tasks;
using static PC80_Tester.General;
using static PC80_Tester.State;

namespace PC80_Tester
{
    public static class TestWriteFw
    {
        public static async Task<bool> WriteFw()
        {
            bool result = false;
            try
            {
                ResetIo();
                SetK7_RL2(true);//MINICUBE2に接続
                await Task.Delay(400);
                General.PowSupply(true);
                await Task.Delay(500);
                var re = await FlashProgrammer.WriteFirmware(Constants.RwsPath, Spec.FwSum);

                VmTestStatus.Spec = $"規格値 : チェックサム 0x{Spec.FwSum}";
                VmTestStatus.MeasValue = $"計測値 : チェックサム 0x{re.readSum}";

                return result = re.result;

            }
            catch
            {
                return false;
            }
            finally
            {
                General.PowSupply(false);
                SetK7_RL2(false);
            }
        }
    }
}
