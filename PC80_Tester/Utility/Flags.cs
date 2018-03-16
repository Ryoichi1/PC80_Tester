using System.Windows.Media;
using static PC80_Tester.General;

namespace PC80_Tester
{
    public static class Flags
    {
        public static bool DialogReturn { get; set; }
        public static bool DoGetDeviceName { get; set; }
        public static bool OtherPage { get; set; }

        //試験開始時に初期化が必要なフラグ
        public static bool StopInit周辺機器 { get; set; }
        public static bool Initializing周辺機器 { get; set; }
        public static bool EnableTestStart { get; set; }
        public static bool Testing { get; set; }
        public static bool PowOn { get; set; }//メイン電源ON/OFF
        public static bool ShowErrInfo { get; set; }
        public static bool AddDecision { get; set; }

        public static bool ShowLabelPage { get; set; }
        public static bool ClickStopButton { get; set; }
        public static bool Click確認Button { get; set; }


        //周辺機器ステータス
        private static bool _State1768;
        public static bool State1768
        {
            get { return _State1768; }
            set
            {
                _State1768 = value;
                State.VmTestStatus.Color1768 = value ? OnBrush : NgBrush;
            }
        }

        private static bool _StateMoxa;
        public static bool StateMoxa
        {
            get { return _StateMoxa; }
            set
            {
                _StateMoxa = value;
                State.VmTestStatus.ColorMoxa = value ? OnBrush : NgBrush;
            }
        }


        private static bool _StateMultimeter;
        public static bool StateMultimeter
        {
            get { return _StateMultimeter; }
            set
            {
                _StateMultimeter = value;
                State.VmTestStatus.ColorMultimeter = value ? OnBrush : NgBrush;
            }
        }

        private static bool _StateMic;
        public static bool StateMic
        {
            get { return _StateMic; }
            set
            {
                _StateMic = value;
                State.VmTestStatus.ColorMic = value ? OnBrush : NgBrush;
            }
        }

        private static bool _ThrowException;
        public static bool ThrowException
        {
            get { return _ThrowException; }
            set
            {
                _ThrowException = value;
                State.VmTestStatus.ColorException = value ? OnBrush : NgBrush;
            }
        }


        private static bool _Retry;
        public static bool Retry
        {
            get { return _Retry; }
            set
            {
                _Retry = value;
                State.VmTestStatus.RetryLabelVis = value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            }
        }


        public static bool AllOk周辺機器接続 { get; set; }

        //フラグ
        private static bool _SetOperator;
        public static bool SetOperator
        {
            get { return _SetOperator; }
            set
            {
                _SetOperator = value;
                if (value)
                {
                    if (State.VmMainWindow.Operator == "畔上" || State.VmMainWindow.Operator == "畔上2")
                    {
                        State.VmTestStatus.UnitTestEnable = true;
                    }
                    else
                    {
                        State.VmTestStatus.UnitTestEnable = false;
                        State.VmTestStatus.CheckUnitTest = false;
                    }
                }
                else
                {
                    State.VmMainWindow.Operator = "";
                    State.VmTestStatus.UnitTestEnable = false;
                    State.VmTestStatus.CheckUnitTest = false;
                    State.VmMainWindow.SelectIndex = -1;


                }
            }
        }


        private static bool _SetOpecode;
        public static bool SetOpecode
        {
            get { return _SetOpecode; }

            set
            {
                _SetOpecode = value;

                if (value)
                {
                    State.VmMainWindow.ReadOnlyOpecode = true;
                }
                else
                {
                    State.VmMainWindow.ReadOnlyOpecode = false;
                    State.VmMainWindow.Opecode = "";
                }

            }
        }

    }
}
