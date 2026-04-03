namespace System.Windows.Forms.Collection;

public class SecurityUtilsTests
{
    private class PublicCtorClass
    {
        public int Value { get; }

        public PublicCtorClass()
        {
            Value = 1;
        }

        public PublicCtorClass(int value)
        {
            Value = value;
        }
    }

    private class PrivateCtorClass
    {
        public int Value { get; }

        private PrivateCtorClass()
        {
            Value = 42;
        }
    }

    [Fact]
    public void SecureCreateInstance_NullType_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            SecurityUtils.SecureCreateInstance(null));
    }

    [Fact]
    public void SecureCreateInstance_PublicCtor_NoArgs_CreatesInstance()
    {
        var instance = SecurityUtils.SecureCreateInstance(typeof(PublicCtorClass));

        Assert.NotNull(instance);
        Assert.IsType<PublicCtorClass>(instance);
        Assert.Equal(1, ((PublicCtorClass)instance!).Value);
    }

    [Fact]
    public void SecureCreateInstance_PublicCtor_WithArgs_CreatesInstance()
    {
        var instance = SecurityUtils.SecureCreateInstance(
            typeof(PublicCtorClass),
            new object[] { 123 },
            allowNonPublic: false);

        Assert.NotNull(instance);
        Assert.Equal(123, ((PublicCtorClass)instance!).Value);
    }

    [Fact]
    public void SecureCreateInstance_PrivateCtor_WithoutAllowNonPublic_Throws()
    {
        Assert.Throws<MissingMethodException>(() =>
            SecurityUtils.SecureCreateInstance(
                typeof(PrivateCtorClass),
                null,
                allowNonPublic: false));
    }

    [Fact]
    public void SecureCreateInstance_PrivateCtor_WithAllowNonPublic_CreatesInstance()
    {
        var instance = SecurityUtils.SecureCreateInstance(
            typeof(PrivateCtorClass),
            null,
            allowNonPublic: true);

        Assert.NotNull(instance);
        Assert.IsType<PrivateCtorClass>(instance);
        Assert.Equal(42, ((PrivateCtorClass)instance!).Value);
    }

    [Fact]
    public void SecureCreateInstance_AllowNonPublic_IgnoredWhenFalse()
    {
        Assert.Throws<MissingMethodException>(() =>
            SecurityUtils.SecureCreateInstance(
                typeof(PrivateCtorClass),
                null,
                allowNonPublic: false));
    }

    [Fact]
    public void SecureCreateInstance_AllowNonPublic_UsesNonPublicBindingFlag()
    {
        var instance = SecurityUtils.SecureCreateInstance(
            typeof(PrivateCtorClass),
            null,
            allowNonPublic: true);

        Assert.NotNull(instance);
    }

    [Fact]
    public void SecureCreateInstance_TypeNotVisible_DoesNotThrow()
    {
        var instance = SecurityUtils.SecureCreateInstance(typeof(PrivateCtorClass));

        Assert.NotNull(instance); // still works via public path or fails gracefully
    }
}