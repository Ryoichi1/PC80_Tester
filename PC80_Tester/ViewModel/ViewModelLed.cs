using Microsoft.Practices.Prism.Mvvm;
using System.Windows.Media;

namespace PC80_Tester
{
    public class ViewModelLed : BindableBase
    {

        //LED座標
        private string _LED1;
        public string LED1 { get { return _LED1; } set { SetProperty(ref _LED1, value); } }

        private string _LED2;
        public string LED2 { get { return _LED2; } set { SetProperty(ref _LED2, value); } }

        private string _LED3;
        public string LED3 { get { return _LED3; } set { SetProperty(ref _LED3, value); } }

        private string _LED4;
        public string LED4 { get { return _LED4; } set { SetProperty(ref _LED4, value); } }


        //LED輝度
        private string _LED1Lum;
        public string LED1Lum { get { return _LED1Lum; } set { SetProperty(ref _LED1Lum, value); } }

        private string _LED2Lum;
        public string LED2Lum { get { return _LED2Lum; } set { SetProperty(ref _LED2Lum, value); } }

        private string _LED3Lum;
        public string LED3Lum { get { return _LED3Lum; } set { SetProperty(ref _LED3Lum, value); } }

        private string _LED4Lum;
        public string LED4Lum { get { return _LED4Lum; } set { SetProperty(ref _LED4Lum, value); } }
      

        //LED色相
        private string _LED1Hue;
        public string LED1Hue { get { return _LED1Hue; } set { SetProperty(ref _LED1Hue, value); } }

        private string _LED2Hue;
        public string LED2Hue { get { return _LED2Hue; } set { SetProperty(ref _LED2Hue, value); } }

        private string _LED3Hue;
        public string LED3Hue { get { return _LED3Hue; } set { SetProperty(ref _LED3Hue, value); } }

        private string _LED4Hue;
        public string LED4Hue { get { return _LED4Hue; } set { SetProperty(ref _LED4Hue, value); } }



        private Brush _ColLED1Hue;
        public Brush ColLED1Hue { get { return _ColLED1Hue; } set { SetProperty(ref _ColLED1Hue, value); } }
       
        private Brush _ColLED2Hue;
        public Brush ColLED2Hue { get { return _ColLED2Hue; } set { SetProperty(ref _ColLED2Hue, value); } }
       
        private Brush _ColLED3Hue;
        public Brush ColLED3Hue { get { return _ColLED3Hue; } set { SetProperty(ref _ColLED3Hue, value); } }
       
        private Brush _ColLED4Hue;
        public Brush ColLED4Hue { get { return _ColLED4Hue; } set { SetProperty(ref _ColLED4Hue, value); } }
       
    }
}
