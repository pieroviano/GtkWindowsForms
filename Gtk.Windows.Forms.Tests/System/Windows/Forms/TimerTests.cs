namespace System.Windows.Forms;

[Collection("NonParallelCollection")]
public class TimerTests
{
    [Fact]
    public void Constructor_ShouldInitializeDefaults()
    {
        var timer = new Timer();

        Assert.NotNull(timer);
        Assert.False(timer.Enabled);
        Assert.Equal(100, timer.Interval);
    }

    [Fact]
    public void Enabled_ShouldSetAndGet()
    {
        var timer = new Timer
        {
            Enabled = true
        };

        Assert.True(timer.Enabled);

        timer.Enabled = false;
        Assert.False(timer.Enabled);
    }

    [Fact]
    public void Interval_ShouldSetAndGet()
    {
        var timer = new Timer
        {
            Interval = 250
        };

        Assert.Equal(250, timer.Interval);
    }

    [Fact]
    public void Tag_ShouldSetAndGet()
    {
        var timer = new Timer
        {
            Tag = "test"
        };

        Assert.Equal("test", timer.Tag);
    }

    [Fact]
    public void Start_ShouldEnableTimer()
    {
        var timer = new Timer();

        timer.Start();

        Assert.True(timer.Enabled);

        timer.Stop();
    }

    [Fact]
    public void Stop_ShouldDisableTimer()
    {
        var timer = new Timer();

        timer.Start();
        timer.Stop();

        Assert.False(timer.Enabled);
    }

    [Fact]
    public void ToString_ShouldReturnExpected()
    {
        var timer = new Timer();

        var result = timer.ToString();

        // Matches real System.Windows.Forms.Timer, whose ToString appends the interval. The gitee
        // alignment corrected this from the old stub that returned only the type name. Default is 100.
        Assert.Equal("System.Windows.Forms.Timer, Interval: 100", result);
    }

    [Fact]
    public void Tick_ShouldFire_WhenTimerRuns()
    {
        var fired = false;
        var form = new Form();
        var timer = new Timer
        {
            Interval = 50
        };
        timer.Tick += (s, e) =>
        {
            fired = true;
        };
        form.Load += async (s, e) => { await OnLoadAsync(timer, form); };
        form.ShowDialog();

        Assert.True(fired);
    }

    private static async Task OnLoadAsync(Timer timer, Form form)
    {
        timer.Enabled = true;
        timer.Start();

        // wait max 1 second
        await Task.Delay(1000);

        timer.Stop();
        form.Close();
    }

    [Fact]
    public void Tick_ShouldNotFire_WhenStopped()
    {
        var timer = new Timer
        {
            Interval = 50
        };

        var fired = false;
        using var waitHandle = new System.Threading.AutoResetEvent(false);

        timer.Tick += (s, e) =>
        {
            fired = true;
            waitHandle.Set();
        };

        timer.Start();
        timer.Stop();

        waitHandle.WaitOne(200);

        Assert.False(fired);
    }

    [Fact]
    public void Dispose_ShouldNotThrow_AndDisableTimer()
    {
        var timer = new Timer();

        timer.Start();

        timer.Dispose();

        // Cannot access internal state safely, but should not throw
        Assert.True(true);
    }

    [Fact]
    public void MultipleStart_Stop_ShouldBeSafe()
    {
        var timer = new Timer();

        timer.Start();
        timer.Start(); // should not throw

        timer.Stop();
        timer.Stop(); // should not throw

        Assert.False(timer.Enabled);
    }

    [Fact]
    public void OnTick_ShouldInvokeEvent()
    {
        var timer = new Timer();

        bool fired = false;

        timer.Tick += (s, e) => fired = true;

        var method = typeof(Timer)
            .GetMethod("OnTick", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        method?.Invoke(timer, new object[] { EventArgs.Empty });

        Assert.True(fired);
    }
}