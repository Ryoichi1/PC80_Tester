using System.Windows;
using Microsoft.Practices.Prism.Mvvm;
using System.Windows.Media.Animation;
using System;

namespace PC80_Tester
{
    /// <summary>
    /// Interaction logic for BasicPage1.xaml
    /// </summary>
    public partial class ラベル貼り付け
    {
        public static Action RefreshDataContextFromLabelForm;//Test.Xaml内でテスト結果をクリアするために使用すする

        public class vm : BindableBase
        {
            private string _Serial;
            public string Serial { get { return _Serial; } internal set { SetProperty(ref _Serial, value); } }

        }

        private vm viewmodel;

        public ラベル貼り付け()
        {
            this.InitializeComponent();

            State.VmMainWindow.ThemeOpacity = 0.0;

            viewmodel = new vm();
            this.DataContext = viewmodel;
        }

        private void SetLabel()
        {
            //デートコード表記の設定

            viewmodel.Serial = "";
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            SetLabel();
            ButtonReturn.Focus();
            (FindResource("BlinkButton") as Storyboard).Begin();
        }

        private void ButtonReturn_Click(object sender, RoutedEventArgs e)
        {
            General.StopSound();

            //テーマ透過度を元に戻す
            State.VmMainWindow.ThemeOpacity = State.CurrentThemeOpacity;

            General.ResetViewModel();
            Flags.ShowLabelPage = false;
            State.VmMainWindow.TabIndex = 0;

            RefreshDataContextFromLabelForm();

            (FindResource("BlinkButton") as Storyboard).Stop();
            //General.PlaySound(General.soundBattery);
        }
    }
}
