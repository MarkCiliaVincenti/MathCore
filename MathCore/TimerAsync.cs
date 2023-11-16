﻿#nullable enable
using System.Diagnostics;

namespace MathCore;

public class TimerAsync(int Timeout)
{
    private readonly Lazy<Stopwatch> _Timer = new(Stopwatch.StartNew);

    private async Task<int> WaitAsync()
    {
        var timer   = _Timer.Value;
        var elapsed = timer.ElapsedMilliseconds;
        var delay   = Math.Max(0, (int)(Timeout - elapsed));

        if (delay > 0)
            await Task.Delay(delay).ConfigureAwait(false);

        return delay;
    }
}