extern alias gtk;

using System.ComponentModel;
using gtk::System.Drawing;
using Bitmap = gtk::System.Drawing.Bitmap;

namespace System.Windows.Forms.Design;

[Collection("NonParallelCollection")]
public class PropertyTabTests
{
    [Fact]
    public void Bitmap_Default_IsNull()
    {
        var tab = new TestPropertyTab();

        var result = tab.Bitmap;

        Assert.Null(result);
    }

    [Fact]
    public void Components_CanBeSetAndRetrieved()
    {
        var tab = new TestPropertyTab();
        var components = new object[] { 1, "test" };

        tab.Components = components;

        Assert.Equal(components, tab.Components);
    }

    [Fact]
    public void HelpKeyword_ReturnsTabName()
    {
        var tab = new TestPropertyTab();

        Assert.Equal("TestTab", tab.HelpKeyword);
    }

    [Fact]
    public void CanExtend_AlwaysReturnsTrue()
    {
        var tab = new TestPropertyTab();

        Assert.True(tab.CanExtend(new object()));
        Assert.True(tab.CanExtend(null));
    }

    [Fact]
    public void GetDefaultProperty_ReturnsExpectedProperty()
    {
        var tab = new TestPropertyTab();
        var obj = new TestComponent { Name = "abc" };

        var prop = tab.GetDefaultProperty(obj);

        Assert.NotNull(prop);
        Assert.Equal("Name", prop.Name);
    }

    [Fact]
    public void GetProperties_WithoutAttributes_CallsOverride()
    {
        var tab = new TestPropertyTab();
        var obj = new TestComponent();

        var props = tab.GetProperties(obj);

        Assert.NotNull(props);
        Assert.True(tab.WasGetPropertiesCalled);
    }

    [Fact]
    public void GetProperties_WithContext_DelegatesCorrectly()
    {
        var tab = new TestPropertyTab();
        var obj = new TestComponent();

        var props = tab.GetProperties(null, obj, null);

        Assert.NotNull(props);
        Assert.True(tab.WasGetPropertiesCalled);
    }

    [Fact]
    public void Dispose_DisposesBitmap()
    {
        var tab = new TestPropertyTabWithBitmap();

        var bmp = tab.Bitmap;
        Assert.False(tab.BitmapDisposed);

        tab.Dispose();

        Assert.True(tab.BitmapDisposed);
    }

    // Test helper classes
    private class TestPropertyTab : PropertyTab
    {
        public bool WasGetPropertiesCalled { get; private set; }

        public override string TabName => "TestTab";

        public override PropertyDescriptorCollection GetProperties(object component, Attribute[]? attributes)
        {
            WasGetPropertiesCalled = true;
            return TypeDescriptor.GetProperties(component);
        }
    }

    private class TestPropertyTabWithBitmap : PropertyTab
    {
        private readonly TestBitmap _bmp = new();
        private bool bmpIsDisposed;

        public bool BitmapDisposed
        {
            get
            {
                bmpIsDisposed |= _bmp.IsDisposed;
                return bmpIsDisposed;
            }
            set { bmpIsDisposed = value; }
        }

        public override string TabName => "BitmapTab";

        public override Bitmap Bitmap => _bmp;

        public override PropertyDescriptorCollection GetProperties(object component, Attribute[]? attributes)
        {
            return TypeDescriptor.GetProperties(component);
        }

        public override void Dispose()
        {
            BitmapDisposed = true;
            base.Dispose();
        }
    }

    private class TestBitmap : Bitmap
    {
        public bool IsDisposed { get; private set; }

        public TestBitmap() : base(1, 1) { }

        protected override void Dispose(bool disposing)
        {
            IsDisposed = true;
            base.Dispose(disposing);
        }
    }

    private class TestComponent
    {
        public string Name { get; set; } = string.Empty;
    }
}