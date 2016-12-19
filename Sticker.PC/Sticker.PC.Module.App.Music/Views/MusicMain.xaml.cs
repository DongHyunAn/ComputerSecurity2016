using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Sticker.PC.Module.App.Music.Views
{
    /// <summary>
    /// MusicMain.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MusicMain : UserControl
    {
        public MusicMain()
        {
            InitializeComponent();
            FlowText.TargetUpdated += FlowText_TargetUpdated;
            datagrid.SelectionChanged += Datagrid_SelectionChanged;
        }

        private void Datagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            datagrid.ScrollIntoView(datagrid.SelectedItem); 
        }

        private void FlowText_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = 900;
            doubleAnimation.To = -(FlowText.Text.Length * 20);
            doubleAnimation.RepeatBehavior = RepeatBehavior.Forever;
            doubleAnimation.Duration = new Duration(TimeSpan.Parse("0:0:10"));
            FlowText.BeginAnimation(Canvas.LeftProperty, doubleAnimation);
        }
    }
}

