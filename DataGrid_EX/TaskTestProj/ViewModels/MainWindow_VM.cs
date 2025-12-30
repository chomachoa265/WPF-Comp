using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTestProj.ViewModels
{
    public partial class MainWindow_VM : ObservableRecipient
    {

        private readonly AsyncCountdownTimer _timer = new();

        [ObservableProperty]
        private string _timerDisplay = "10";

        [ObservableProperty]
        private string _statusText = "準備就緒";
        public bool IsBusy { get; set; } = false;
        public MainWindow_VM()
        {

        }

        [RelayCommand]
        private async Task Start()
        {
            IsBusy = true;
            StatusText = "倒數中...";

            // 2. 呼叫封裝好的 StartAsync
            await _timer.StartAsync(
                totalTime: TimeSpan.FromSeconds(10),
                interval: TimeSpan.FromSeconds(1),

                // 每秒做什麼 (這裡直接寫 Lambda 更新 UI)
                onTick: (timeSpan) =>
                {
                    TimerDisplay = timeSpan.TotalSeconds.ToString("0");
                },

                // 倒數結束做什麼
                onFinished: () =>
                {
                    StatusText = "倒數完成！";
                    TimerDisplay = "Done";
                }
            );

            // 因為 StartAsync 會等待直到結束或被取消，所以這裡代表「整個流程結束」
            if (_timer.IsRunning == false) // 再次確認狀態
            {
                IsBusy = false;
                if (StatusText != "倒數完成！") StatusText = "已由使用者中斷";
            }
        }
        [RelayCommand]
        private void Stop()
        {
            _timer.Stop();
        }
    }
}
