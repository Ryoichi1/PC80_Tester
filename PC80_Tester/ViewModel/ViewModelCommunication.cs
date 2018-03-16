using Microsoft.Practices.Prism.Mvvm;
using System.Windows.Media;

namespace PC80_Tester
{

    public class ViewModelCommunication : BindableBase
    {
        //LPC1768
        private string _TX_IO;
        public string TX_IO
        {
            get { return _TX_IO; }
            set { SetProperty(ref _TX_IO, value); }
        }

        private string _RX_IO;
        public string RX_IO
        {
            get { return _RX_IO; }
            set { SetProperty(ref _RX_IO, value); }
        }

        //ターゲット
        private string _TX_TARGET;
        public string TX_TARGET
        {
            get { return _TX_TARGET; }
            set { SetProperty(ref _TX_TARGET, value); }
        }

        private string _RX_TARGET;
        public string RX_TARGET
        {
            get { return _RX_TARGET; }
            set { SetProperty(ref _RX_TARGET, value); }
        }

    }
}
