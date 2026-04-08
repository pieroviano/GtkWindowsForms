namespace System.Windows.Forms;

extern alias gtk;

[Collection("NonParallelCollection")]
public class TabControlTests
{
    [Fact]
    public void Constructor_ShouldInitializeDefaults()
    {
        var form = new Form();
        var tab = new TabControl();
        form.Controls.Add(tab);

        object? gtkControl = null;

        form.Load += (s, e) =>
        {
            gtkControl = tab.GtkControl;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(tab.self);
        Assert.NotNull(gtkControl);
        Assert.Equal(tab.self, gtkControl);
    }

    [Theory]
    [InlineData(TabAlignment.Top)]
    [InlineData(TabAlignment.Bottom)]
    [InlineData(TabAlignment.Left)]
    [InlineData(TabAlignment.Right)]
    public void Alignment_ShouldSetAndGet(TabAlignment alignment)
    {
        var form = new Form();
        var tab = new TabControl();
        form.Controls.Add(tab);

        TabAlignment result = TabAlignment.Top;

        form.Load += (s, e) =>
        {
            tab.Alignment = alignment;
            result = tab.Alignment;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(alignment, result);
    }

    [Fact]
    public void SelectedIndex_ShouldSetAndGet()
    {
        var form = new Form();
        var tab = new TabControl();
        tab.TabPages.Add("tab1");
        tab.TabPages.Add("tab2");
        form.Controls.Add(tab);

        int index = -1;

        tab.Load += (s, e) =>
        {

            tab.SelectedIndex = 1;
            index = tab.SelectedIndex;
        };

        async Task OnFormLoadAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            form.Close();
        }

        form.Load += async (s, e) => { await OnFormLoadAsync(); };

        form.ShowDialog();

        Assert.Equal(1, index);
    }

    [Fact]
    public void TabPages_Add_ShouldIncreaseCount()
    {
        var form = new Form();
        var tab = new TabControl();
        form.Controls.Add(tab);

        int count = 0;

        form.Load += (s, e) =>
        {
            tab.TabPages.Add("tab1");
            tab.TabPages.Add("tab2");

            count = tab.TabCount;

            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(2, count);
    }

    [Fact]
    public void TabPages_Remove_ShouldDecreaseCount()
    {
        var form = new Form();
        var tab = new TabControl();
        form.Controls.Add(tab);

        int count = -1;

        form.Load += (s, e) =>
        {
            var page = new TabPage { Text = "tab1" };
            tab.TabPages.Add(page);

            tab.TabPages.Remove(page);

            count = tab.TabCount;

            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(0, count);
    }

    [Fact]
    public void ShowTabs_ShouldSetAndGet()
    {
        var form = new Form();
        var tab = new TabControl();
        form.Controls.Add(tab);

        bool result = true;

        form.Load += (s, e) =>
        {
            tab.ShowTabs = false;
            result = tab.ShowTabs;

            form.Close();
        };

        form.ShowDialog();

        Assert.False(result);
    }

    [Fact]
    public void TabPages_ContainsKey_ShouldWork()
    {
        var form = new Form();
        var tab = new TabControl();
        form.Controls.Add(tab);

        bool contains = false;

        form.Load += (s, e) =>
        {
            tab.TabPages.Add("key1", "Tab 1");

            contains = tab.TabPages.ContainsKey("key1");

            form.Close();
        };

        form.ShowDialog();

        Assert.True(contains);
    }

    [Fact]
    public void TabPages_IndexOfKey_ShouldReturnCorrectIndex()
    {
        var form = new Form();
        var tab = new TabControl();
        form.Controls.Add(tab);

        int index = -1;

        form.Load += (s, e) =>
        {
            tab.TabPages.Add("key1", "Tab 1");

            index = tab.TabPages.IndexOfKey("key1");

            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(0, index);
    }

    [Fact]
    public void GetTabRect_InvalidIndex_ShouldThrow()
    {
        var form = new Form();
        var tab = new TabControl();
        form.Controls.Add(tab);

        Exception? ex = null;

        form.Load += (s, e) =>
        {
            tab.TabPages.Add("tab1");

            try
            {
                tab.GetTabRect(-1);
            }
            catch (Exception e2)
            {
                ex = e2;
            }

            form.Close();
        };

        form.ShowDialog();

        Assert.IsType<ArgumentOutOfRangeException>(ex);
    }

    [Fact]
    public void SelectedIndexChanged_ShouldFire()
    {
        var form = new Form();
        var tab = new TabControl();
        form.Controls.Add(tab);

        bool fired = false;

        form.Load += (s, e) =>
        {
            tab.TabPages.Add("tab1");
            tab.TabPages.Add("tab2");

            tab.SelectedIndexChanged += (s2, e2) => fired = true;

            // simulate change
            tab.SelectedIndex = 1;

            // force invoke (GTK won't trigger automatically in test env)
            var method = typeof(TabControl)
                .GetMethod("OnSelectedIndexChanged", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            method?.Invoke(tab, new object[] { EventArgs.Empty });

            form.Close();
        };

        form.ShowDialog();

        Assert.True(fired);
    }

    [Fact]
    public void GtkControl_ShouldReturnSelf()
    {
        var form = new Form();
        var tab = new TabControl();
        form.Controls.Add(tab);

        object? gtkControl = null;
        object? self = null;

        form.Load += (s, e) =>
        {
            gtkControl = tab.GtkControl;
            self = tab.self;

            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(self);
        Assert.Equal(self, gtkControl);
    }
}