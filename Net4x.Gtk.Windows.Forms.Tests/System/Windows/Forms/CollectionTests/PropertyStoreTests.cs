using System.Drawing;
using System.Reflection;

namespace System.Windows.Forms.CollectionTests;

public class PropertyStoreTests
{
    [Fact]
    public void ContainsInteger_ReturnsCorrectValue()
    {
        var store = CreateStore();
        var key = PropertyStore.CreateKey();

        Assert.False(store.ContainsInteger(key));

        store.SetInteger(key, 10);

        Assert.True(store.ContainsInteger(key));
    }

    [Fact]
    public void ContainsObject_ReturnsCorrectValue()
    {
        var store = CreateStore();
        var key = PropertyStore.CreateKey();

        Assert.False(store.ContainsObject(key));

        store.SetObject(key, new object());

        Assert.True(store.ContainsObject(key));
    }

    [Fact]
    public void ContainsObjectThatIsNotNull_ReturnsFalseForNull()
    {
        var store = CreateStore();
        var key = PropertyStore.CreateKey();

        store.SetObject(key, null);

        Assert.True(store.ContainsObject(key));
        Assert.False(store.ContainsObjectThatIsNotNull(key));
    }

    [Fact]
    public void CreateKey_ShouldReturnUniqueKeys()
    {
        var k1 = PropertyStore.CreateKey();
        var k2 = PropertyStore.CreateKey();

        Assert.NotEqual(k1, k2);
    }

    [Fact]
    public void GetColor_NotFound_ReturnsEmpty()
    {
        var store = CreateStore();
        var key = PropertyStore.CreateKey();

        var result = store.GetColor(key, out var found);

        Assert.False(found);
        Assert.Equal(Color.Empty, result);
    }

    [Fact]
    public void GetInteger_NotFound_ReturnsDefaultAndFalse()
    {
        var store = CreateStore();
        var key = PropertyStore.CreateKey();

        var value = store.GetInteger(key, out var found);

        Assert.False(found);
        Assert.Equal(0, value);
    }

    [Fact]
    public void GetObject_NotFound_ReturnsNullAndFalse()
    {
        var store = CreateStore();
        var key = PropertyStore.CreateKey();

        var obj = store.GetObject(key, out var found);

        Assert.False(found);
        Assert.Null(obj);
    }

    [Fact]
    public void RemoveObject_RemovesValue()
    {
        var store = CreateStore();
        var key = PropertyStore.CreateKey();

        store.SetObject(key, new object());
        store.RemoveObject(key);

        var found = store.ContainsObject(key);

        Assert.False(found);
    }

    [Fact]
    public void SetColor_ThenGetColor_ReturnsValue()
    {
        var store = CreateStore();
        var key = PropertyStore.CreateKey();
        var color = Color.Red;

        store.SetColor(key, color);

        var result = store.GetColor(key, out var found);

        Assert.True(found);
        Assert.Equal(color, result);
    }

    [Fact]
    public void SetInteger_ThenGetInteger_ReturnsValue()
    {
        var store = CreateStore();
        var key = PropertyStore.CreateKey();

        store.SetInteger(key, 42);

        var value = store.GetInteger(key, out var found);

        Assert.True(found);
        Assert.Equal(42, value);
    }

    [Fact]
    public void SetObject_ThenGetObject_ReturnsSameInstance()
    {
        var store = CreateStore();
        var key = PropertyStore.CreateKey();
        var value = new object();

        store.SetObject(key, value);

        var result = store.GetObject(key, out var found);

        Assert.True(found);
        Assert.Same(value, result);
    }

    [Fact]
    public void SetPadding_ThenGetPadding_ReturnsValue()
    {
        var store = CreateStore();
        var key = PropertyStore.CreateKey();
        var padding = new Padding(1, 2, 3, 4);

        store.SetPadding(key, padding);

        var result = store.GetPadding(key, out var found);

        Assert.True(found);
        Assert.Equal(padding, result);
    }

    [Fact]
    public void SetRectangle_ThenGetRectangle_ReturnsValue()
    {
        var store = CreateStore();
        var key = PropertyStore.CreateKey();
        var rect = new Rectangle(1, 2, 3, 4);

        store.SetRectangle(key, rect);

        var result = store.GetRectangle(key, out var found);

        Assert.True(found);
        Assert.Equal(rect, result);
    }

    [Fact]
    public void SetSize_ThenGetSize_ReturnsValue()
    {
        var store = CreateStore();
        var key = PropertyStore.CreateKey();
        var size = new Size(5, 6);

        store.SetSize(key, size);

        var result = store.GetSize(key, out var found);

        Assert.True(found);
        Assert.Equal(size, result);
    }

    [Fact]
    public void SplitKey_SplitsCorrectly()
    {
        var key = 0b_101100; // example
        var entry = InvokeSplitKey(key, out var element);

        Assert.Equal((short)(key & 0xFFFFFFFC), entry);
        Assert.Equal((short)(key & 0x3), element);
    }

    [Fact]
    public void TryGetObject_WithReferenceType_Works()
    {
        var store = CreateStore();
        var key = PropertyStore.CreateKey();
        var value = "hello";

        store.SetObject(key, value);

        var success = store.TryGetObject<string>(key, out var result);

        Assert.True(success);
        Assert.Equal(value, result);
    }

    [Fact]
    public void TryGetObject_WithValueType_Works()
    {
        var store = CreateStore();
        var key = PropertyStore.CreateKey();

        store.SetObject(key, 123);

        var success = store.TryGetObject<int>(key, out var result);

        Assert.True(success);
        Assert.Equal(123, result);
    }

    private PropertyStore CreateStore() => new();

    // helper via reflection since SplitKey is private
    private static short InvokeSplitKey(int key, out short element)
    {
        var method = typeof(PropertyStore)
            .GetMethod("SplitKey", BindingFlags.NonPublic | BindingFlags.Static)!;

        object[] args = { key, null! };
        var result = (short)method.Invoke(null, args)!;
        element = (short)args[1];
        return result;
    }
}