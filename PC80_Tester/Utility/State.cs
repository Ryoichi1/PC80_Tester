using System;
using System.Collections.Generic;

namespace PC80_Tester
{

    public class TestDetail
    {
        public int Key;
        public string Value;
        public bool PowSw;

        public TestDetail(int key, string value, bool powSW = true)
        {
            this.Key = key;
            this.Value = value;
            this.PowSw = powSW;

        }
    }

    public static class State
    {

        //データソース（バインディング用）
        public static ViewModelMainWindow VmMainWindow = new ViewModelMainWindow();
        public static ViewModelTestStatus VmTestStatus = new ViewModelTestStatus();
        public static ViewModelTestResult VmTestResults = new ViewModelTestResult();
        public static TestCommand testCommand = new TestCommand();
        public static ViewModelLed VmLedPoint = new ViewModelLed();
        public static ViewModelLcd VmLcdPoint = new ViewModelLcd();
        public static ViewModelCommunication VmComm = new ViewModelCommunication();

        //パブリックメンバ
        public static Configuration Setting { get; set; }
        public static TestSpec Spec { get; set; }
        public static Command Command { get; set; }

        public static CameraPropertyForLcd CamPropLcd { get; set; }

        public static CameraPropertyForLed CamPropLed { get; set; }
        public static CameraPropertyForDefault CamPropDef { get; set; }

        public static string CurrDir { get; set; }

        public static string AssemblyInfo { get; set; }

        public static double CurrentThemeOpacity { get; set; }

        public static Uri uriOtherInfoPage { get; set; }


        //リトライ履歴保存用リスト
        public static List<string> RetryLogList = new List<string>();


        public static List<TestDetail> テスト項目 = new List<TestDetail>()
        {
            new TestDetail(100, "FW書き込み", false),
            new TestDetail(200, "VR1調整", true),
            new TestDetail(300, "DSW1 ONチェック"),
            new TestDetail(301, "DSW1 OFFチェック"),
            new TestDetail(400, "消費電流 チェック", false),
            new TestDetail(500, "電源電圧Vcc チェック", false),
            new TestDetail(600, "P130発振周波数 チェック",false),
            new TestDetail(601, "ブザー周波数 チェック"),
            new TestDetail(602, "ブザー音圧 チェック"),
            new TestDetail(700, "タクトスイッチ チェック"),
            new TestDetail(800, "LED1（緑） チェック"),
            new TestDetail(801, "LED2（緑） チェック"),
            new TestDetail(802, "LED3（赤） チェック"),
            new TestDetail(803, "LED4（橙） チェック"),
            new TestDetail(900, "LCD(FIG1) チェック"),
            new TestDetail(901, "LCD(FIG2) チェック"),
        };

        //個別設定のロード
        public static void LoadConfigData()
        {
            //Configファイルのロード
            Setting = Deserialize<Configuration>(Constants.filePath_Configuration);

            VmMainWindow.Opecode = Setting.Opecode;
            VmMainWindow.ListOperator = Setting.作業者リスト;
            VmMainWindow.Theme = Setting.PathTheme;
            VmMainWindow.ThemeOpacity = Setting.OpacityTheme;

            if (Setting.日付 != DateTime.Now.ToString("yyyyMMdd"))
            {
                Setting.日付 = DateTime.Now.ToString("yyyyMMdd");
                Setting.TodayOkCount = 0;
                Setting.TodayNgCount = 0;
            }

            VmTestStatus.OkCount = Setting.TodayOkCount.ToString() + "台";
            VmTestStatus.NgCount = Setting.TodayNgCount.ToString() + "台";

            //TestSpecファイルのロード
            Spec = Deserialize<TestSpec>(Constants.filePath_TestSpec);

            //Commandファイルのロード
            Command = Deserialize<Command>(Constants.filePath_Command);

            //コネクタ画像検査 カメラプロパティファイルのロード
            CamPropLcd = Deserialize<CameraPropertyForLcd>(Constants.filePath_CamPropLcd);
            //LEDプロパティファイルのロード
            CamPropLed = Deserialize<CameraPropertyForLed>(Constants.filePath_CamPropLed);
            CamPropDef = Deserialize<CameraPropertyForDefault>(Constants.filePath_CamPropDef);

        }

        public static void LoadTestSpec()//リトライ前にコールすると便利（デバッグ時、試験スペックを調整するときに使用する）
        {
            //TestSpecファイルのロード
            Spec = Deserialize<TestSpec>(Constants.filePath_TestSpec);
        }

        //インスタンスをXMLデータに変換する
        public static bool Serialization<T>(T obj, string xmlFilePath)
        {
            try
            {
                //XmlSerializerオブジェクトを作成
                //オブジェクトの型を指定する
                System.Xml.Serialization.XmlSerializer serializer =
                    new System.Xml.Serialization.XmlSerializer(typeof(T));
                //書き込むファイルを開く（UTF-8 BOM無し）
                System.IO.StreamWriter sw = new System.IO.StreamWriter(xmlFilePath, false, new System.Text.UTF8Encoding(false));
                //シリアル化し、XMLファイルに保存する
                serializer.Serialize(sw, obj);
                //ファイルを閉じる
                sw.Close();

                return true;

            }
            catch
            {
                return false;
            }

        }

        //XMLデータからインスタンスを生成する
        public static T Deserialize<T>(string xmlFilePath)
        {
            System.Xml.Serialization.XmlSerializer serializer;
            using (var sr = new System.IO.StreamReader(xmlFilePath, new System.Text.UTF8Encoding(false)))
            {
                serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(sr);
            }
        }

        //********************************************************
        //個別設定データの保存
        //********************************************************
        public static bool Save個別データ()
        {
            try
            {
                //Configファイルの保存
                Setting.Opecode = VmMainWindow.Opecode;
                Setting.作業者リスト = VmMainWindow.ListOperator;
                Setting.PathTheme = VmMainWindow.Theme;
                Setting.OpacityTheme = VmMainWindow.ThemeOpacity;

                Serialization<Configuration>(Setting, Constants.filePath_Configuration);

                //コネクタ画像検査 Camプロパティの保存
                Serialization<CameraPropertyForLcd>(State.CamPropLcd, Constants.filePath_CamPropLcd);
                //Ledプロパティの保存
                Serialization<CameraPropertyForLed>(State.CamPropLed, Constants.filePath_CamPropLed);

                return true;
            }
            catch
            {
                return false;

            }

        }

        public static void SetCamPropForDef()
        {
            General.cam.Brightness = CamPropDef.Brightness;
            General.cam.Contrast = CamPropDef.Contrast;
            General.cam.Hue = CamPropDef.Hue;
            General.cam.Saturation = CamPropDef.Saturation;
            General.cam.Sharpness = CamPropDef.Sharpness;
            General.cam.Gamma = CamPropDef.Gamma;
            General.cam.Gain = CamPropDef.Gain;
            General.cam.Exposure = CamPropDef.Exposure;
            General.cam.Wb = CamPropDef.Whitebalance;
            General.cam.Theta = CamPropDef.Theta;
        }
        public static void SetCamPropForLcd()
        {
            General.cam.BinLevel = CamPropLcd.BinLevel;
            General.cam.Opening = CamPropLcd.Opening;
            General.cam.openCnt = CamPropLcd.OpenCnt;
            General.cam.closeCnt = CamPropLcd.CloseCnt;

            General.cam.Brightness = CamPropLcd.Brightness;
            General.cam.Contrast = CamPropLcd.Contrast;
            General.cam.Hue = CamPropLcd.Hue;
            General.cam.Saturation = CamPropLcd.Saturation;
            General.cam.Sharpness = CamPropLcd.Sharpness;
            General.cam.Gamma = CamPropLcd.Gamma;
            General.cam.Gain = CamPropLcd.Gain;
            General.cam.Exposure = CamPropLcd.Exposure;
            General.cam.Wb = CamPropLcd.Whitebalance;
            General.cam.Theta = CamPropLcd.Theta;
        }

        public static void SetCamPropForLed()
        {
            General.cam.Opening = CamPropLed.Opening;
            General.cam.openCnt = CamPropLed.OpenCnt;
            General.cam.closeCnt = CamPropLed.CloseCnt;

            General.cam.Brightness = CamPropLed.Brightness;
            General.cam.Contrast = CamPropLed.Contrast;
            General.cam.Hue = CamPropLed.Hue;
            General.cam.Saturation = CamPropLed.Saturation;
            General.cam.Sharpness = CamPropLed.Sharpness;
            General.cam.Gamma = CamPropLed.Gamma;
            General.cam.Gain = CamPropLed.Gain;
            General.cam.Exposure = CamPropLed.Exposure;
            General.cam.Wb = CamPropLed.Whitebalance;
            General.cam.Theta = CamPropLed.Theta;
            General.cam.BinLevel = CamPropLed.BinLevel;
        }

        public static void SetCamPoint()
        {
            VmLedPoint.LED1 = CamPropLed.Led1;
            VmLedPoint.LED2 = CamPropLed.Led2;
            VmLedPoint.LED3 = CamPropLed.Led3;
            VmLedPoint.LED4 = CamPropLed.Led4;

            VmLcdPoint.X_UpLeft = CamPropLcd.X_UP_L;
            VmLcdPoint.Y_UpLeft = CamPropLcd.Y_UP_L;
            VmLcdPoint.W_UpLeft = CamPropLcd.W_UP_L;
            VmLcdPoint.H_UpLeft = CamPropLcd.H_UP_L;

            VmLcdPoint.X_UpRight = CamPropLcd.X_UP_R;
            VmLcdPoint.Y_UpRight = CamPropLcd.Y_UP_R;
            VmLcdPoint.W_UpRight = CamPropLcd.W_UP_R;
            VmLcdPoint.H_UpRight = CamPropLcd.H_UP_R;

            VmLcdPoint.X_LoLeft = CamPropLcd.X_LO_L;
            VmLcdPoint.Y_LoLeft = CamPropLcd.Y_LO_L;
            VmLcdPoint.W_LoLeft = CamPropLcd.W_LO_L;
            VmLcdPoint.H_LoLeft = CamPropLcd.H_LO_L;

            VmLcdPoint.X_LoRight = CamPropLcd.X_LO_R;
            VmLcdPoint.Y_LoRight = CamPropLcd.Y_LO_R;
            VmLcdPoint.W_LoRight = CamPropLcd.W_LO_R;
            VmLcdPoint.H_LoRight = CamPropLcd.H_LO_R;
        }

    }

}
