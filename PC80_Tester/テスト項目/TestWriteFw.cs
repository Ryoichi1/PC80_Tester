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
                SetK7_11(true);//MINICUBE2に接続
                await Task.Delay(400);
                return result = await FlashProgrammer.WriteFirmware(Constants.RwsPath, Spec.FwSum);
            }
            catch
            {
                return false;
            }
            finally
            {
                SetK7_11(false);
            }
        }
    }
}
