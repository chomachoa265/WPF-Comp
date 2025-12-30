using System;
using System.Threading;
using System.Threading.Tasks;

public class AsyncCountdownTimer
{
    private CancellationTokenSource? _cts;

    // 公開屬性讓人知道目前是否在跑
    public bool IsRunning { get; private set; }

    /// <summary>
    /// 啟動倒數
    /// </summary>
    /// <param name="totalTime">總時間</param>
    /// <param name="interval">更新頻率 (例如 1秒)</param>
    /// <param name="onTick">每次 Tick 執行的動作 (回傳剩餘時間)</param>
    /// <param name="onFinished">倒數自然結束時執行 (可選)</param>
    public async Task StartAsync(
        TimeSpan totalTime,
        TimeSpan interval,
        Action<TimeSpan> onTick,
        Action? onFinished = null)
    {
        // 防止重複啟動
        if (IsRunning) return;

        IsRunning = true;
        _cts = new CancellationTokenSource();
        var remaining = totalTime;

        // 立即執行第一次回報 (讓 UI 馬上顯示起始時間)
        onTick(remaining);

        using var timer = new PeriodicTimer(interval);

        try
        {
            // 只要沒被取消，且還有剩餘時間，就繼續等下一個 Tick
            while (await timer.WaitForNextTickAsync(_cts.Token))
            {
                remaining = remaining.Subtract(interval);

                // 確保不小於 0
                if (remaining < TimeSpan.Zero) remaining = TimeSpan.Zero;

                // 回報進度
                onTick(remaining);

                // 時間到，跳出迴圈
                if (remaining == TimeSpan.Zero)
                {
                    onFinished?.Invoke();
                    break;
                }
            }
        }
        catch (OperationCanceledException)
        {
            // 被 Stop() 強制中斷
        }
        finally
        {
            IsRunning = false;
            _cts?.Dispose();
            _cts = null;
        }
    }

    /// <summary>
    /// 停止計時
    /// </summary>
    public void Stop()
    {
        _cts?.Cancel();
    }
}