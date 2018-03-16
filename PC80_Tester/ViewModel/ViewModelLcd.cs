using Microsoft.Practices.Prism.Mvvm;

namespace PC80_Tester
{
    public class ViewModelLcd : BindableBase
    {
        //上段左側
        private int _X_UpLeft;
        public int X_UpLeft { get { return _X_UpLeft; } set { SetProperty(ref _X_UpLeft, value); } }

        private int _Y_UpLeft;
        public int Y_UpLeft { get { return _Y_UpLeft; } set { SetProperty(ref _Y_UpLeft, value); } }

        private int _W_UpLeft;
        public int W_UpLeft { get { return _W_UpLeft; } set { SetProperty(ref _W_UpLeft, value); } }

        private int _H_UpLeft;
        public int H_UpLeft { get { return _H_UpLeft; } set { SetProperty(ref _H_UpLeft, value); } }

        //上段右側
        private int _X_UpRight;
        public int X_UpRight { get { return _X_UpRight; } set { SetProperty(ref _X_UpRight, value); } }

        private int _Y_UpRight;
        public int Y_UpRight { get { return _Y_UpRight; } set { SetProperty(ref _Y_UpRight, value); } }

        private int _W_UpRight;
        public int W_UpRight { get { return _W_UpRight; } set { SetProperty(ref _W_UpRight, value); } }

        private int _H_UpRight;
        public int H_UpRight { get { return _H_UpRight; } set { SetProperty(ref _H_UpRight, value); } }

        //下段左側
        private int _X_LoLeft;
        public int X_LoLeft { get { return _X_LoLeft; } set { SetProperty(ref _X_LoLeft, value); } }

        private int _Y_LoLeft;
        public int Y_LoLeft { get { return _Y_LoLeft; } set { SetProperty(ref _Y_LoLeft, value); } }

        private int _W_LoLeft;
        public int W_LoLeft { get { return _W_LoLeft; } set { SetProperty(ref _W_LoLeft, value); } }

        private int _H_LoLeft;
        public int H_LoLeft { get { return _H_LoLeft; } set { SetProperty(ref _H_LoLeft, value); } }

        //下段左側
        private int _X_LoRight;
        public int X_LoRight { get { return _X_LoRight; } set { SetProperty(ref _X_LoRight, value); } }

        private int _Y_LoRight;
        public int Y_LoRight { get { return _Y_LoRight; } set { SetProperty(ref _Y_LoRight, value); } }

        private int _W_LoRight;
        public int W_LoRight { get { return _W_LoRight; } set { SetProperty(ref _W_LoRight, value); } }

        private int _H_LoRight;
        public int H_LoRight { get { return _H_LoRight; } set { SetProperty(ref _H_LoRight, value); } }


        /// <summary>
        /// /////////
        /// </summary>
        private string _ResultUpLeft;
        public string ResultUpLeft { get { return _ResultUpLeft; } set { SetProperty(ref _ResultUpLeft, value); } }

        private string _ResultUpRight;
        public string ResultUpRight { get { return _ResultUpRight; } set { SetProperty(ref _ResultUpRight, value); } }

        private string _ResultLoLeft;
        public string ResultLoLeft { get { return _ResultLoLeft; } set { SetProperty(ref _ResultLoLeft, value); } }

        private string _ResultLoRight;
        public string ResultLoRight { get { return _ResultLoRight; } set { SetProperty(ref _ResultLoRight, value); } }

    }
}
