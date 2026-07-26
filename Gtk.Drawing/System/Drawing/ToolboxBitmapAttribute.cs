using System.IO;
using System.Reflection;

namespace System.Drawing;

/// <summary>
/// Specifies the image used to represent a component in the Visual Studio toolbox.
/// </summary>
/// <remarks>
/// This attribute was missing, which made it a compile break on the *declaration* of any custom control —
/// <c>[ToolboxBitmap(typeof(MyControl), "icon.bmp")]</c> — rather than at a call site, so an entire
/// custom-control library failed to build. The image is resolved from the declaring type's assembly
/// resources or from disk; a missing image yields <see langword="null" /> rather than an exception, which
/// is what the framework does.
/// </remarks>
[AttributeUsage(AttributeTargets.Class)]
public class ToolboxBitmapAttribute : Attribute
{
    /// <summary>The edge length of the small image the toolbox uses.</summary>
    private const int SmallImageSize = 16;

    private readonly string? _imageFile;

    private readonly Type? _imageType;

    private readonly string? _imageName;

    private Image? _smallImage;

    private Image? _largeImage;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Drawing.ToolboxBitmapAttribute" /> class with
    /// an image loaded from the given file.
    /// </summary>
    /// <param name="imageFile">The path of the image file.</param>
    public ToolboxBitmapAttribute(string imageFile)
    {
        _imageFile = imageFile;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Drawing.ToolboxBitmapAttribute" /> class with
    /// an image loaded from the assembly of the given type.
    /// </summary>
    /// <param name="t">A type whose assembly contains the image resource.</param>
    public ToolboxBitmapAttribute(Type t)
    {
        _imageType = t;
        _imageName = t?.Name;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Drawing.ToolboxBitmapAttribute" /> class with
    /// the named image resource from the assembly of the given type.
    /// </summary>
    /// <param name="t">A type whose assembly contains the image resource.</param>
    /// <param name="name">The name of the embedded resource.</param>
    public ToolboxBitmapAttribute(Type t, string name)
    {
        _imageType = t;
        _imageName = name;
    }

    /// <summary>
    /// The default attribute, carrying no image.
    /// </summary>
    public static readonly ToolboxBitmapAttribute Default = new(string.Empty);

    /// <summary>
    /// Returns the small (16x16) image for the component, or <see langword="null" /> when there is none.
    /// </summary>
    /// <param name="component">The component whose image is wanted.</param>
    public Image? GetImage(object? component) => GetImage(component, true);

    /// <summary>
    /// Returns the image for the component, or <see langword="null" /> when there is none.
    /// </summary>
    /// <param name="component">The component whose image is wanted.</param>
    /// <param name="large">Whether the large image is wanted.</param>
    public Image? GetImage(object? component, bool large) =>
        GetImage(component?.GetType(), large);

    /// <summary>
    /// Returns the small (16x16) image for the type, or <see langword="null" /> when there is none.
    /// </summary>
    /// <param name="type">The type whose image is wanted.</param>
    public Image? GetImage(Type? type) => GetImage(type, false);

    /// <summary>
    /// Returns the image for the type, or <see langword="null" /> when there is none.
    /// </summary>
    /// <param name="type">The type whose image is wanted.</param>
    /// <param name="large">Whether the large image is wanted.</param>
    public Image? GetImage(Type? type, bool large) => GetImage(type, null, large);

    /// <summary>
    /// Returns the named image from the type's assembly, or <see langword="null" /> when there is none.
    /// </summary>
    /// <param name="type">The type whose assembly holds the resource.</param>
    /// <param name="imgName">The name of the resource, or <see langword="null" /> to use the attribute's.</param>
    /// <param name="large">Whether the large image is wanted.</param>
    public Image? GetImage(Type? type, string? imgName, bool large)
    {
        var cached = large ? _largeImage : _smallImage;
        if (cached != null)
        {
            return cached;
        }

        var loaded = Load(type ?? _imageType, imgName ?? _imageName);
        if (loaded == null)
        {
            return null;
        }

        if (large)
        {
            _largeImage = loaded;

            return _largeImage;
        }

        _smallImage = loaded.Width == SmallImageSize && loaded.Height == SmallImageSize
            ? loaded
            : loaded.GetThumbnailImage(SmallImageSize, SmallImageSize, () => false, IntPtr.Zero);

        return _smallImage;
    }

    /// <summary>
    /// Returns the image a type's <see cref="T:System.Drawing.ToolboxBitmapAttribute" /> names, or
    /// <see langword="null" /> when the type carries no such attribute.
    /// </summary>
    /// <param name="type">The type to inspect.</param>
    /// <param name="imageName">The name of the resource, or <see langword="null" /> to use the type name.</param>
    /// <param name="large">Whether the large image is wanted.</param>
    public static Image? GetImageFromResource(Type type, string? imageName, bool large) =>
        new ToolboxBitmapAttribute(type, imageName ?? type?.Name ?? string.Empty).GetImage(type, imageName, large);

    /// <summary>
    /// Loads the image from the type's assembly resources, then from disk, and gives up quietly.
    /// </summary>
    private Image? Load(Type? type, string? name)
    {
        try
        {
            if (!string.IsNullOrEmpty(_imageFile) && File.Exists(_imageFile))
            {
                return Image.FromFile(_imageFile!);
            }

            if (type == null || string.IsNullOrEmpty(name))
            {
                return null;
            }

            var assembly = type.GetTypeInfo().Assembly;
            foreach (var candidate in new[] { name!, type.Namespace + "." + name, type.FullName + ".bmp" })
            {
                var stream = assembly.GetManifestResourceStream(candidate);
                if (stream != null)
                {
                    return Image.FromStream(stream);
                }
            }

            return File.Exists(name) ? Image.FromFile(name!) : null;
        }
        catch (Exception ex)
        {
            // A missing or unreadable toolbox image is not an error worth failing a build or a run over.
            Diagnostics.GtkLogger.Debug("ToolboxBitmapAttribute could not load an image: " + ex.Message);

            return null;
        }
    }

    /// <summary>
    /// Indicates whether the given object is an equivalent attribute.
    /// </summary>
    /// <param name="value">The object to compare with.</param>
    public override bool Equals(object? value) =>
        value is ToolboxBitmapAttribute other
        && other._imageFile == _imageFile
        && other._imageType == _imageType
        && other._imageName == _imageName;

    /// <summary>
    /// Returns a hash code for this attribute.
    /// </summary>
    public override int GetHashCode() =>
        (_imageFile?.GetHashCode() ?? 0) ^ (_imageType?.GetHashCode() ?? 0) ^ (_imageName?.GetHashCode() ?? 0);
}
