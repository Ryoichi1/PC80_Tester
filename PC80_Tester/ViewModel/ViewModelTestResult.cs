using Microsoft.Practices.Prism.Mvvm;
using System.Windows.Media;


namespace PC80_Tester
{
    public class ViewModelTestResult : BindableBase
    {
        //マイク
        private string _Freq;
        public string Freq
        {
            get { return _Freq; }
            set { this.SetProperty(ref this._Freq, value); }
        }

        private string _VolLev;
        public string VolLev
        {
            get { return _VolLev; }
            set { this.SetProperty(ref this._VolLev, value); }
        }


        //VR調整値
        private string _Vr;
        public string Vr { get { return _Vr; } set { SetProperty(ref _Vr, value); } }

        private Brush _ColVr;
        public Brush ColVr { get { return _ColVr; } set { SetProperty(ref _ColVr, value); } }

        //電源電圧
        private string _Vol5v;
        public string Vol5v { get { return _Vol5v; } set { SetProperty(ref _Vol5v, value); } }

        private Brush _ColVol5v;
        public Brush ColVol5v { get { return _ColVol5v; } set { SetProperty(ref _ColVol5v, value); } }

        //消費電流
        private string _Curr;
        public string Curr { get { return _Curr; } set { SetProperty(ref _Curr, value); } }

        private Brush _ColCurr;
        public Brush ColCurr { get { return _ColCurr; } set { SetProperty(ref _ColCurr, value); } }

        //ブザー検査
        private string _P130;
        public string P130 { get { return _P130; } set { SetProperty(ref _P130, value); } }

        private Brush _ColP130;
        public Brush ColP130 { get { return _ColP130; } set { SetProperty(ref _ColP130, value); } }

        private string _Buz;
        public string Buz { get { return _Buz; } set { SetProperty(ref _Buz, value); } }

        private Brush _ColBuz;
        public Brush ColPBuz { get { return _ColBuz; } set { SetProperty(ref _ColBuz, value); } }

        private string _BuzLev;
        public string BuzLev { get { return _BuzLev; } set { SetProperty(ref _BuzLev, value); } }

        private Brush _ColBuzLev;
        public Brush ColBuzLev { get { return _ColBuzLev; } set { SetProperty(ref _ColBuzLev, value); } }



        /// <summary>
        /// 
        /// </summary>
        private Brush _ColDsw1Exp;
        public Brush ColDsw1Exp { get { return _ColDsw1Exp; } set { SetProperty(ref _ColDsw1Exp, value); } }

        private Brush _ColDsw1Res;
        public Brush ColDsw1Res { get { return _ColDsw1Res; } set { SetProperty(ref _ColDsw1Res, value); } }

        /// <summary>
        /// /
        /// </summary>
        //LED1
        private string _HueLed1;
        public string HueLed1 { get { return _HueLed1; } set { SetProperty(ref _HueLed1, value); } }

        private Brush _ColLed1;
        public Brush ColLed1 { get { return _ColLed1; } set { SetProperty(ref _ColLed1, value); } }

        private string _LumLed1;
        public string LumLed1 { get { return _LumLed1; } set { SetProperty(ref _LumLed1, value); } }

        private Brush _ColLumLed1;
        public Brush ColLumLed1 { get { return _ColLumLed1; } set { SetProperty(ref _ColLumLed1, value); } }

        //LED2
        private string _HueLed2;
        public string HueLed2 { get { return _HueLed2; } set { SetProperty(ref _HueLed2, value); } }

        private Brush _ColLed2;
        public Brush ColLed2 { get { return _ColLed2; } set { SetProperty(ref _ColLed2, value); } }

        private string _LumLed2;
        public string LumLed2 { get { return _LumLed2; } set { SetProperty(ref _LumLed2, value); } }

        private Brush _ColLumLed2;
        public Brush ColLumLed2 { get { return _ColLumLed2; } set { SetProperty(ref _ColLumLed2, value); } }

        //LED3
        private string _HueLed3;
        public string HueLed3 { get { return _HueLed3; } set { SetProperty(ref _HueLed3, value); } }

        private Brush _ColLed3;
        public Brush ColLed3 { get { return _ColLed3; } set { SetProperty(ref _ColLed3, value); } }

        private string _LumLed3;
        public string LumLed3 { get { return _LumLed3; } set { SetProperty(ref _LumLed3, value); } }

        private Brush _ColLumLed3;
        public Brush ColLumLed3 { get { return _ColLumLed3; } set { SetProperty(ref _ColLumLed3, value); } }

        //LED4
        private string _HueLed4;
        public string HueLed4 { get { return _HueLed4; } set { SetProperty(ref _HueLed4, value); } }

        private Brush _ColLed4;
        public Brush ColLed4 { get { return _ColLed4; } set { SetProperty(ref _ColLed4, value); } }

        private string _LumLed4;
        public string LumLed4 { get { return _LumLed4; } set { SetProperty(ref _LumLed4, value); } }

        private Brush _ColLumLed4;
        public Brush ColLumLed4 { get { return _ColLumLed4; } set { SetProperty(ref _ColLumLed4, value); } }

        //LCD検査

        //FIG1
        private string _Fig1UP_L;
        public string Fig1UP_L { get { return _Fig1UP_L; } set { SetProperty(ref _Fig1UP_L, value); } }

        private Brush _ColFig1UP_L;
        public Brush ColFig1UP_L { get { return _ColFig1UP_L; } set { SetProperty(ref _ColFig1UP_L, value); } }

        private string _Fig1UP_R;
        public string Fig1UP_R { get { return _Fig1UP_R; } set { SetProperty(ref _Fig1UP_R, value); } }

        private Brush _ColFig1UP_R;
        public Brush ColFig1UP_R { get { return _ColFig1UP_R; } set { SetProperty(ref _ColFig1UP_R, value); } }

        private string _Fig1LO_L;
        public string Fig1LO_L { get { return _Fig1LO_L; } set { SetProperty(ref _Fig1LO_L, value); } }

        private Brush _ColFig1LO_L;
        public Brush ColFig1LO_L { get { return _ColFig1LO_L; } set { SetProperty(ref _ColFig1LO_L, value); } }

        private string _Fig1LO_R;
        public string Fig1LO_R { get { return _Fig1LO_R; } set { SetProperty(ref _Fig1LO_R, value); } }

        private Brush _ColFig1LO_R;
        public Brush ColFig1LO_R { get { return _ColFig1LO_R; } set { SetProperty(ref _ColFig1LO_R, value); } }

        //FIG2
        private string _Fig2UP_L;
        public string Fig2UP_L { get { return _Fig2UP_L; } set { SetProperty(ref _Fig2UP_L, value); } }

        private Brush _ColFig2UP_L;
        public Brush ColFig2UP_L { get { return _ColFig2UP_L; } set { SetProperty(ref _ColFig2UP_L, value); } }

        private string _Fig2UP_R;
        public string Fig2UP_R { get { return _Fig2UP_R; } set { SetProperty(ref _Fig2UP_R, value); } }

        private Brush _ColFig2UP_R;
        public Brush ColFig2UP_R { get { return _ColFig2UP_R; } set { SetProperty(ref _ColFig2UP_R, value); } }

        private string _Fig2LO_L;
        public string Fig2LO_L { get { return _Fig2LO_L; } set { SetProperty(ref _Fig2LO_L, value); } }

        private Brush _ColFig2LO_L;
        public Brush ColFig2LO_L { get { return _ColFig2LO_L; } set { SetProperty(ref _ColFig2LO_L, value); } }

        private string _Fig2LO_R;
        public string Fig2LO_R { get { return _Fig2LO_R; } set { SetProperty(ref _Fig2LO_R, value); } }

        private Brush _ColFig2LO_R;
        public Brush ColFig2LO_R { get { return _ColFig2LO_R; } set { SetProperty(ref _ColFig2LO_R, value); } }


    }

}








