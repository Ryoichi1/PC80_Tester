
namespace PC80_Tester
{
    public static class Constants
    {
        //スタートボタンの表示
        public const string 開始 = "開始";
        public const string 停止 = "停止";
        public const string 確認 = "確認";

        //作業者へのメッセージ
        public const string MessOpecode = "工番を入力してください";
        public const string MessOperator = "作業者名を選択してください";
        public const string MessSet = "製品をセットしてレバーを下げてください";
        public const string MessRemove = "製品を取り外してください";
        public const string MessLabel = "ラベルを貼り付けたら、確認ボタンを押してください";

        public const string MessWait = "検査中！　しばらくお待ちください・・・";
        public const string MessCheckConnectMachine = "周辺機器の接続を確認してください！";

        public static readonly string filePath_TestSpec = @"C:\PC80\ConfigData\TestSpec.config";
        public static readonly string filePath_Configuration = @"C:\PC80\ConfigData\Configuration.config";
        public static readonly string filePath_Command = @"C:\PC80\ConfigData\Command.config";
        public static readonly string filePath_CamPropLcd = @"C:\PC80\ConfigData\CameraPropertyForLcd.config";
        public static readonly string filePath_CamPropLed = @"C:\PC80\ConfigData\CameraPropertyForLed.config";
        public static readonly string filePath_CamLcdCalFilePath = @"C:\PC80\ConfigData\CameraLcd.xml";
        public static readonly string filePath_CamLedCalFilePath = @"C:\PC80\ConfigData\CameraLed.xml";

        public static readonly string filePath_Fig1TempUpLeft = @"C:\PC80\Pic\Fig1tempUpLeft.bmp";
        public static readonly string filePath_Fig1TempUpRight = @"C:\PC80\Pic\Fig1tempUpRight.bmp";
        public static readonly string filePath_Fig1TempLoLeft = @"C:\PC80\Pic\Fig1tempLoLeft.bmp";
        public static readonly string filePath_Fig1TempLoRight = @"C:\PC80\Pic\Fig1tempLoRight.bmp";
        public static readonly string filePath_Fig2TempUpLeft = @"C:\PC80\Pic\Fig2tempUpLeft.bmp";
        public static readonly string filePath_Fig2TempUpRight = @"C:\PC80\Pic\Fig2tempUpRight.bmp";
        public static readonly string filePath_Fig2TempLoLeft = @"C:\PC80\Pic\Fig2tempLoLeft.bmp";
        public static readonly string filePath_Fig2TempLoRight = @"C:\PC80\Pic\Fig2tempLoRight.bmp";

        public static readonly string Fig1Command = "LED2ON";
        public static readonly string Fig2Command = "BZOFF";
        public static readonly string PathVR_R = "Resources/Pic/VR_R.jpg";
        public static readonly string PathVR_L = "Resources/Pic/VR_L.jpg";


        public static readonly string RwsPath = @"C:\PC80\WRITE_FW\WRITE_FW.rws";

        public static readonly string Path_Manual = @"C:\PC80\Manual.pdf";

        //検査データフォルダのパス
        public static readonly string PassDataFolderPath = @"C:\PC80\検査データ\合格品データ\";
        public static readonly string FailDataFolderPath = @"C:\PC80\検査データ\不良品データ\";
        public static readonly string fileName_RetryLog = @"C:\PC80\検査データ\不良品データ\" + "リトライ履歴.txt";

        //Imageの透明度
        public const double OpacityImgMin = 0.0;

        //リトライ回数
        public static readonly int RetryCount = 2;












    }
}
