#if NET462
namespace System.Runtime.CompilerServices;

/// <summary>
/// The compiler recognises this attribute by name, not by identity, so declaring it here gives
/// <c>net462</c> the same module initializer the modern target frameworks have in the framework itself.
/// </summary>
/// <remarks>
/// Internal on purpose: a public copy would collide with the real type for any consumer that already has
/// one.
/// </remarks>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
internal sealed class ModuleInitializerAttribute : Attribute
{
}
#endif
