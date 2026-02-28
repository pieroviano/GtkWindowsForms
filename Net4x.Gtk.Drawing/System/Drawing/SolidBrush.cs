using System.Collections;

namespace System.Drawing
{
    /// <summary>
    /// Defines a brush of a single color. Brushes are used to fill graphics shapes, such as rectangles, ellipses, pies, polygons, and paths. This class cannot be inherited.
    /// </summary>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    public sealed class SolidBrush : Brush
    {
        /// <summary>
        /// Gets or sets the color of this
        /// <see cref="System.Drawing.SolidBrush" /> object.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Color" /> structure that represents the color of this brush.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// The
        /// <see cref="System.Drawing.SolidBrush.Color" /> property is set on an immutable
        /// <see cref="System.Drawing.SolidBrush" />
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public Color Color
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new
        /// <see cref="T:System.Drawing.SolidBrush" /> object of the specified color.
        /// </summary>
        /// <param name="color">A
        /// <see cref="T:System.Drawing.Color" /> structure that represents the color of this brush.
        /// </param>
        public SolidBrush(Color color)
        {
            this.Color = color;
        }

        /// <summary>
        /// Creates an exact copy of this
        /// <see cref="System.Drawing.SolidBrush" /> object.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Drawing.SolidBrush" /> object that this method creates.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public override object Clone()
        {
            return null;
        }

        protected override void Dispose(bool disposing)
        {

        }
    }
}
