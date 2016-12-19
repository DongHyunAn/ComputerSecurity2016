using System;
using Prism.Events;
using Prism.Mvvm;
using System.Windows;
using Prism.Commands;
using System.Windows.Forms;
using System.Windows.Input;
using Sticker.PC.Infra.Events;
using System.Threading.Tasks;

namespace Sticker.PC.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        IEventAggregator _eventAggregator;

        public ICommand RemoteCommand { get; set; }

        private string _notifyText;
        public string NotifyText
        {
            get { return _notifyText; }
            set { SetProperty(ref _notifyText, value); }
        }

        public MainWindowViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<ProgramShutDownEvent>().Subscribe(ProgramShutDownImply, ThreadOption.UIThread);
            _eventAggregator.GetEvent<NotifyToastEvent>().Subscribe(NotifyToastImply, ThreadOption.UIThread);
        }

        private async void ProgramShutDownImply(object obj)
        {
            NotifyToastImply("프로그램을 종료합니다...");
            await Task.Delay(TimeSpan.FromSeconds(3));
            System.Windows.Application.Current.Shutdown();
        }

        private void NotifyToastImply(string obj)
        {
            NotifyText = "";
            NotifyText = obj;
        }
    }
}
