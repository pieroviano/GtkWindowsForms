using System.Drawing.Drawing2D;

namespace System.Drawing
{
    /// <summary>
    /// Describes the interior of a graphics shape composed of rectangles and paths. This class cannot be inherited.
    /// </summary>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    public sealed class Region : MarshalByRefObject, IDisposable
    {

        private RectangleF rectangle = new RectangleF(0, 0, 0, 0);

        private RegionData regionData = new RegionData();
        private GraphicsPath graphicsPath = new GraphicsPath();
        /// <summary>
        /// Initializes a new
        /// <see cref="T:System.Drawing.Region" />.
        /// </summary>
        public Region()
        {
        }

        /// <summary>
        /// Initializes a new
        /// <see cref="T:System.Drawing.Region" /> with the specified
        /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.
        /// </summary>
        /// <param name="path">A
        /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> that defines the new
        /// <see cref="T:System.Drawing.Region" />.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="path" /> is
        /// <see langword="null" />.
        /// </exception>
        public Region(GraphicsPath path)
        {
            graphicsPath = path;
            rectangle = path.GetBounds();

        }

        /// <summary>
        /// Initializes a new
        /// <see cref="T:System.Drawing.Region" /> from the specified data.
        /// </summary>
        /// <param name="rgnData">A
        /// <see cref="T:System.Drawing.Drawing2D.RegionData" /> that defines the interior of the new
        /// <see cref="T:System.Drawing.Region" />.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="rgnData" /> is
        /// <see langword="null" />.
        /// </exception>
        public Region(RegionData rgnData)
        {
            regionData = rgnData;

        }

        /// <summary>
        /// Initializes a new
        /// <see cref="T:System.Drawing.Region" /> from the specified
        /// <see cref="T:System.Drawing.Rectangle" /> structure.
        /// </summary>
        /// <param name="rect">A
        /// <see cref="T:System.Drawing.Rectangle" /> structure that defines the interior of the new
        /// <see cref="T:System.Drawing.Region" />.
        /// </param>
        public Region(Rectangle rect)
        {
            rectangle = new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <summary>
        /// Initializes a new
        /// <see cref="T:System.Drawing.Region" /> from the specified
        /// <see cref="T:System.Drawing.RectangleF" /> structure.
        /// </summary>
        /// <param name="rect">A
        /// <see cref="T:System.Drawing.RectangleF" /> structure that defines the interior of the new
        /// <see cref="T:System.Drawing.Region" />.
        /// </param>
        public Region(RectangleF rect)
        {
            rectangle = new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <summary>
        /// Creates an exact copy of this
        /// <see cref="System.Drawing.Region" />
        /// </summary>
        /// <returns>The
        /// <see cref="System.Drawing.Region" /> that this method creates.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public Region Clone()
        {
            return new Region(new RectangleF(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height));
        }

        /// <summary>
        /// Updates this
        /// <see cref="System.Drawing.Region" /> to contain the portion of the specified
        /// <see cref="System.Drawing.Drawing2D.GraphicsPath" /> that does not intersect with this
        /// <see cref="System.Drawing.Region" />
        /// </summary>
        /// <param name="path">
        /// The
        /// <see cref="System.Drawing.Drawing2D.GraphicsPath" /> to complement this
        /// <see cref="System.Drawing.Region" />
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="path" /> isnull.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void Complement(GraphicsPath path)
        {
            graphicsPath = path;
            rectangle = path.GetBounds();
        }

        /// <summary>
        /// Updates this
        /// <see cref="System.Drawing.Region" /> to contain the portion of the specified
        /// <see cref="System.Drawing.Rectangle" /> structure that does not intersect with this
        /// <see cref="System.Drawing.Region" />
        /// </summary>
        /// <param name="rect">
        /// The
        /// <see cref="System.Drawing.Rectangle" /> structure to complement this
        /// <see cref="System.Drawing.Region" />
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void Complement(Rectangle rect)
        {
            rectangle = new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <summary>
        /// Updates this
        /// <see cref="System.Drawing.Region" /> to contain the portion of the specified
        /// <see cref="System.Drawing.RectangleF" /> structure that does not intersect with this
        /// <see cref="System.Drawing.Region" />
        /// </summary>
        /// <param name="rect">
        /// The
        /// <see cref="System.Drawing.RectangleF" /> structure to complement this
        /// <see cref="System.Drawing.Region" />
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void Complement(RectangleF rect)
        {
            rectangle = new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <summary>
        /// Updates this
        /// <see cref="System.Drawing.Region" /> to contain the portion of the specified
        /// <see cref="System.Drawing.Region" /> that does not intersect with this
        /// <see cref="System.Drawing.Region" />
        /// </summary>
        /// <param name="region">
        /// The
        /// <see cref="System.Drawing.Region" /> object to complement this
        /// <see cref="System.Drawing.Region" /> object.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="region" /> isnull.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void Complement(Region region)
        {
            rectangle = region.rectangle;
            graphicsPath = region.graphicsPath;
            regionData = region.regionData;
        }

        /// <summary>
        /// Releases all resources used by this
        /// <see cref="System.Drawing.Region" />
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void Dispose()
        {
            rectangle = new RectangleF();
            graphicsPath = new GraphicsPath();
            regionData = new RegionData();
        }

        /// <summary>
        /// Tests whether the specified
        /// <see cref="System.Drawing.Region" /> is identical to this
        /// <see cref="System.Drawing.Region" /> on the specified drawing surface.
        /// </summary>
        /// <returns>
        /// true if the interior of region is identical to the interior of this region when the transformation associated with the
        /// <paramref name="g" /> parameter is applied; otherwise, false.
        /// </returns>
        /// <param name="region">The
        /// <see cref="System.Drawing.Region" /> to test.
        /// </param>
        /// <param name="g">A
        /// <see cref="System.Drawing.Graphics" /> that represents a drawing surface.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="g" /> or
        /// <paramref name="region" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public bool Equals(Region region, Graphics g)
        {
            return rectangle.Equals(region.rectangle) && graphicsPath.Equals(region.graphicsPath) && regionData.Equals(region.regionData);
        }

        /// <summary>
        /// Updates this
        /// <see cref="System.Drawing.Region" /> to contain only the portion of its interior that does not intersect with the specified
        /// <see cref="System.Drawing.Drawing2D.GraphicsPath" />
        /// </summary>
        /// <param name="path">
        /// The
        /// <see cref="System.Drawing.Drawing2D.GraphicsPath" /> to exclude from this
        /// <see cref="System.Drawing.Region" />
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="path" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void Exclude(GraphicsPath path)
        {
        }

        /// <summary>
        /// Updates this
        /// <see cref="System.Drawing.Region" /> to contain only the portion of its interior that does not intersect with the specified
        /// <see cref="System.Drawing.Rectangle" /> structure.
        /// </summary>
        /// <param name="rect">
        /// The
        /// <see cref="System.Drawing.Rectangle" /> structure to exclude from this
        /// <see cref="System.Drawing.Region" />
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void Exclude(Rectangle rect)
        {
        }

        /// <summary>
        /// Updates this
        /// <see cref="System.Drawing.Region" /> to contain only the portion of its interior that does not intersect with the specified
        /// <see cref="System.Drawing.RectangleF" /> structure.
        /// </summary>
        /// <param name="rect">
        /// The
        /// <see cref="System.Drawing.RectangleF" /> structure to exclude from this
        /// <see cref="System.Drawing.Region" />
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void Exclude(RectangleF rect)
        {
        }

        /// <summary>
        /// Updates this
        /// <see cref="System.Drawing.Region" /> to contain only the portion of its interior that does not intersect with the specified
        /// <see cref="System.Drawing.Region" />
        /// </summary>
        /// <param name="region">
        /// The
        /// <see cref="System.Drawing.Region" /> to exclude from this
        /// <see cref="System.Drawing.Region" />
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="region" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void Exclude(Region region)
        {
        }

        /// <summary>
        /// Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.
        /// </summary>
        ~Region()
        {
        }

        /// <summary>
        /// Initializes a new
        /// <see cref="T:System.Drawing.Region" /> from a handle to the specified existing GDI region.
        /// </summary>
        /// <param name="hrgn">A handle to an existing
        /// <see cref="T:System.Drawing.Region" />.
        /// </param>
        /// <returns>The new
        /// <see cref="T:System.Drawing.Region" />.
        /// </returns>
        public static Region FromHrgn(IntPtr hrgn)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets a
        /// <see cref="System.Drawing.RectangleF" /> structure that represents a rectangle that bounds this
        /// <see cref="System.Drawing.Region" /> on the drawing surface of a
        /// <see cref="System.Drawing.Graphics" /> object.
        /// </summary>
        /// <returns>
        /// A
        /// <see cref="System.Drawing.RectangleF" /> structure that represents the bounding rectangle for this
        /// <see cref="System.Drawing.Region" /> on the specified drawing surface.
        /// </returns>
        /// <param name="g">The
        /// <see cref="System.Drawing.Graphics" /> on which this
        /// <see cref="System.Drawing.Region" /> is drawn.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="g" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public RectangleF GetBounds(Graphics g)
        {
            return g.ClipBounds;
        }

        /// <summary>
        /// Returns a Windows handle to this
        /// <see cref="System.Drawing.Region" /> in the specified graphics context.
        /// </summary>
        /// <returns>A Windows handle to this
        /// <see cref="System.Drawing.Region" />
        /// </returns>
        /// <param name="g">The
        /// <see cref="System.Drawing.Graphics" /> on which this
        /// <see cref="System.Drawing.Region" /> is drawn.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="g" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public IntPtr GetHrgn(Graphics g)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Returns a
        /// <see cref="System.Drawing.Drawing2D.RegionData" /> that represents the information that describes this
        /// <see cref="System.Drawing.Region" />
        /// </summary>
        /// <returns>
        /// A
        /// <see cref="System.Drawing.Drawing2D.RegionData" /> that represents the information that describes this
        /// <see cref="System.Drawing.Region" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public RegionData GetRegionData()
        {
            return regionData;
        }

        /// <summary>
        /// Returns an array of
        /// <see cref="System.Drawing.RectangleF" /> structures that approximate this
        /// <see cref="System.Drawing.Region" /> after the specified matrix transformation is applied.
        /// </summary>
        /// <returns>
        /// An array of
        /// <see cref="System.Drawing.RectangleF" /> structures that approximate this
        /// <see cref="System.Drawing.Region" /> after the specified matrix transformation is applied.
        /// </returns>
        /// <param name="matrix">A
        /// <see cref="System.Drawing.Drawing2D.Matrix" /> that represents a geometric transformation to apply to the region.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="matrix" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public RectangleF[] GetRegionScans(Matrix matrix)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Updates this
        /// <see cref="System.Drawing.Region" /> to the intersection of itself with the specified
        /// <see cref="System.Drawing.Drawing2D.GraphicsPath" />
        /// </summary>
        /// <param name="path">
        /// The
        /// <see cref="System.Drawing.Drawing2D.GraphicsPath" /> to intersect with this
        /// <see cref="System.Drawing.Region" />
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void Intersect(GraphicsPath path)
        {
        }

        /// <summary>
        /// Updates this
        /// <see cref="System.Drawing.Region" /> to the intersection of itself with the specified
        /// <see cref="System.Drawing.Rectangle" /> structure.
        /// </summary>
        /// <param name="rect">
        /// The
        /// <see cref="System.Drawing.Rectangle" /> structure to intersect with this
        /// <see cref="System.Drawing.Region" />
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void Intersect(Rectangle rect)
        {
        }

        /// <summary>
        /// Updates this
        /// <see cref="System.Drawing.Region" /> to the intersection of itself with the specified
        /// <see cref="System.Drawing.RectangleF" /> structure.
        /// </summary>
        /// <param name="rect">
        /// The
        /// <see cref="System.Drawing.RectangleF" /> structure to intersect with this
        /// <see cref="System.Drawing.Region" />
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void Intersect(RectangleF rect)
        {
        }

        /// <summary>
        /// Updates this
        /// <see cref="System.Drawing.Region" /> to the intersection of itself with the specified
        /// <see cref="System.Drawing.Region" />
        /// </summary>
        /// <param name="region">
        /// The
        /// <see cref="System.Drawing.Region" /> to intersect with this
        /// <see cref="System.Drawing.Region" />
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void Intersect(Region region)
        {
        }

        /// <summary>
        /// Tests whether this
        /// <see cref="System.Drawing.Region" /> has an empty interior on the specified drawing surface.
        /// </summary>
        /// <returns>
        /// true if the interior of this
        /// <see cref="System.Drawing.Region" /> is empty when the transformation associated with
        /// <paramref name="g" /> is applied; otherwise, false.
        /// </returns>
        /// <param name="g">A
        /// <see cref="System.Drawing.Graphics" /> that represents a drawing surface.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="g" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public bool IsEmpty(Graphics g)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Tests whether this
        /// <see cref="System.Drawing.Region" /> has an infinite interior on the specified drawing surface.
        /// </summary>
        /// <returns>
        /// true if the interior of this
        /// <see cref="System.Drawing.Region" /> is infinite when the transformation associated with
        /// <paramref name="g" /> is applied; otherwise, false.
        /// </returns>
        /// <param name="g">A
        /// <see cref="System.Drawing.Graphics" /> that represents a drawing surface.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="g" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public bool IsInfinite(Graphics g)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Tests whether the specified
        /// <see cref="System.Drawing.Point" /> structure is contained within this
        /// <see cref="System.Drawing.Region" />
        /// </summary>
        /// <returns>true when
        /// <paramref name="point" /> is contained within this
        /// <see cref="System.Drawing.Region" />; otherwise, false.
        /// </returns>
        /// <param name="point">The
        /// <see cref="System.Drawing.Point" /> structure to test.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public bool IsVisible(Point point)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Tests whether the specified
        /// <see cref="System.Drawing.Point" /> structure is contained within this
        /// <see cref="System.Drawing.Region" /> when drawn using the specified
        /// <see cref="System.Drawing.Graphics" />
        /// </summary>
        /// <returns>true when
        /// <paramref name="point" /> is contained within this
        /// <see cref="System.Drawing.Region" />; otherwise, false.
        /// </returns>
        /// <param name="point">The
        /// <see cref="System.Drawing.Point" /> structure to test.
        /// </param>
        /// <param name="g">A
        /// <see cref="System.Drawing.Graphics" /> that represents a graphics context.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public bool IsVisible(Point point, Graphics g)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Tests whether the specified
        /// <see cref="System.Drawing.PointF" /> structure is contained within this
        /// <see cref="System.Drawing.Region" />
        /// </summary>
        /// <returns>true when
        /// <paramref name="point" /> is contained within this
        /// <see cref="System.Drawing.Region" />; otherwise, false.
        /// </returns>
        /// <param name="point">The
        /// <see cref="System.Drawing.PointF" /> structure to test.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public bool IsVisible(PointF point)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Tests whether the specified
        /// <see cref="System.Drawing.PointF" /> structure is contained within this
        /// <see cref="System.Drawing.Region" /> when drawn using the specified
        /// <see cref="System.Drawing.Graphics" />
        /// </summary>
        /// <returns>true when
        /// <paramref name="point" /> is contained within this
        /// <see cref="System.Drawing.Region" />; otherwise, false.
        /// </returns>
        /// <param name="point">The
        /// <see cref="System.Drawing.PointF" /> structure to test.
        /// </param>
        /// <param name="g">A
        /// <see cref="System.Drawing.Graphics" /> that represents a graphics context.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public bool IsVisible(PointF point, Graphics g)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Tests whether any portion of the specified
        /// <see cref="System.Drawing.Rectangle" /> structure is contained within this
        /// <see cref="System.Drawing.Region" />
        /// </summary>
        /// <returns>
        /// This method returns true when any portion of
        /// <paramref name="rect" /> is contained within this
        /// <see cref="System.Drawing.Region" />; otherwise, false.
        /// </returns>
        /// <param name="rect">The
        /// <see cref="System.Drawing.Rectangle" /> structure to test.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public bool IsVisible(Rectangle rect)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Tests whether any portion of the specified
        /// <see cref="System.Drawing.Rectangle" /> structure is contained within this
        /// <see cref="System.Drawing.Region" /> when drawn using the specified
        /// <see cref="System.Drawing.Graphics" />
        /// </summary>
        /// <returns>
        /// true when any portion of the
        /// <paramref name="rect" /> is contained within this
        /// <see cref="System.Drawing.Region" />; otherwise, false.
        /// </returns>
        /// <param name="rect">The
        /// <see cref="System.Drawing.Rectangle" /> structure to test.
        /// </param>
        /// <param name="g">A
        /// <see cref="System.Drawing.Graphics" /> that represents a graphics context.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public bool IsVisible(Rectangle rect, Graphics g)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Tests whether any portion of the specified
        /// <see cref="System.Drawing.RectangleF" /> structure is contained within this
        /// <see cref="System.Drawing.Region" />
        /// </summary>
        /// <returns>
        /// true when any portion of
        /// <paramref name="rect" /> is contained within this
        /// <see cref="System.Drawing.Region" />; otherwise, false.
        /// </returns>
        /// <param name="rect">The
        /// <see cref="System.Drawing.RectangleF" /> structure to test.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public bool IsVisible(RectangleF rect)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Tests whether any portion of the specified
        /// <see cref="System.Drawing.RectangleF" /> structure is contained within this
        /// <see cref="System.Drawing.Region" /> when drawn using the specified
        /// <see cref="System.Drawing.Graphics" />
        /// </summary>
        /// <returns>true when
        /// <paramref name="rect" /> is contained within this
        /// <see cref="System.Drawing.Region" />; otherwise, false.
        /// </returns>
        /// <param name="rect">The
        /// <see cref="System.Drawing.RectangleF" /> structure to test.
        /// </param>
        /// <param name="g">A
        /// <see cref="System.Drawing.Graphics" /> that represents a graphics context.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public bool IsVisible(RectangleF rect, Graphics g)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Tests whether the specified point is contained within this
        /// <see cref="T:System.Drawing.Region" /> object when drawn using the specified
        /// <see cref="T:System.Drawing.Graphics" /> object.
        /// </summary>
        /// <param name="x">The x-coordinate of the point to test.
        /// </param>
        /// <param name="y">The y-coordinate of the point to test.
        /// </param>
        /// <param name="g">A
        /// <see cref="T:System.Drawing.Graphics" /> that represents a graphics context.
        /// </param>
        /// <returns>
        /// <see langword="true" /> when the specified point is contained within this
        /// <see cref="T:System.Drawing.Region" />; otherwise,
        /// <see langword="false" />.
        /// </returns>
        public bool IsVisible(int x, int y, Graphics g)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Tests whether any portion of the specified rectangle is contained within this
        /// <see cref="T:System.Drawing.Region" />.
        /// </summary>
        /// <param name="x">The x-coordinate of the upper-left corner of the rectangle to test.
        /// </param>
        /// <param name="y">The y-coordinate of the upper-left corner of the rectangle to test.
        /// </param>
        /// <param name="width">The width of the rectangle to test.
        /// </param>
        /// <param name="height">The height of the rectangle to test.
        /// </param>
        /// <returns>
        /// <see langword="true" /> when any portion of the specified rectangle is contained within this
        /// <see cref="T:System.Drawing.Region" />; otherwise,
        /// <see langword="false" />.
        /// </returns>
        public bool IsVisible(int x, int y, int width, int height)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Tests whether any portion of the specified rectangle is contained within this
        /// <see cref="T:System.Drawing.Region" /> when drawn using the specified
        /// <see cref="T:System.Drawing.Graphics" />.
        /// </summary>
        /// <param name="x">The x-coordinate of the upper-left corner of the rectangle to test.
        /// </param>
        /// <param name="y">The y-coordinate of the upper-left corner of the rectangle to test.
        /// </param>
        /// <param name="width">The width of the rectangle to test.
        /// </param>
        /// <param name="height">The height of the rectangle to test.
        /// </param>
        /// <param name="g">A
        /// <see cref="T:System.Drawing.Graphics" /> that represents a graphics context.
        /// </param>
        /// <returns>
        /// <see langword="true" /> when any portion of the specified rectangle is contained within this
        /// <see cref="T:System.Drawing.Region" />; otherwise,
        /// <see langword="false" />.
        /// </returns>
        public bool IsVisible(int x, int y, int width, int height, Graphics g)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Tests whether the specified point is contained within this
        /// <see cref="T:System.Drawing.Region" />.
        /// </summary>
        /// <param name="x">The x-coordinate of the point to test.
        /// </param>
        /// <param name="y">The y-coordinate of the point to test.
        /// </param>
        /// <returns>
        /// <see langword="true" /> when the specified point is contained within this
        /// <see cref="T:System.Drawing.Region" />; otherwise,
        /// <see langword="false" />.
        /// </returns>
        public bool IsVisible(float x, float y)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Tests whether the specified point is contained within this
        /// <see cref="T:System.Drawing.Region" /> when drawn using the specified
        /// <see cref="T:System.Drawing.Graphics" />.
        /// </summary>
        /// <param name="x">The x-coordinate of the point to test.
        /// </param>
        /// <param name="y">The y-coordinate of the point to test.
        /// </param>
        /// <param name="g">A
        /// <see cref="T:System.Drawing.Graphics" /> that represents a graphics context.
        /// </param>
        /// <returns>
        /// <see langword="true" /> when the specified point is contained within this
        /// <see cref="T:System.Drawing.Region" />; otherwise,
        /// <see langword="false" />.
        /// </returns>
        public bool IsVisible(float x, float y, Graphics g)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Tests whether any portion of the specified rectangle is contained within this
        /// <see cref="T:System.Drawing.Region" />.
        /// </summary>
        /// <param name="x">The x-coordinate of the upper-left corner of the rectangle to test.
        /// </param>
        /// <param name="y">The y-coordinate of the upper-left corner of the rectangle to test.
        /// </param>
        /// <param name="width">The width of the rectangle to test.
        /// </param>
        /// <param name="height">The height of the rectangle to test.
        /// </param>
        /// <returns>
        /// <see langword="true" /> when any portion of the specified rectangle is contained within this
        /// <see cref="T:System.Drawing.Region" /> object; otherwise,
        /// <see langword="false" />.
        /// </returns>
        public bool IsVisible(float x, float y, float width, float height)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Tests whether any portion of the specified rectangle is contained within this
        /// <see cref="T:System.Drawing.Region" /> when drawn using the specified
        /// <see cref="T:System.Drawing.Graphics" />.
        /// </summary>
        /// <param name="x">The x-coordinate of the upper-left corner of the rectangle to test.
        /// </param>
        /// <param name="y">The y-coordinate of the upper-left corner of the rectangle to test.
        /// </param>
        /// <param name="width">The width of the rectangle to test.
        /// </param>
        /// <param name="height">The height of the rectangle to test.
        /// </param>
        /// <param name="g">A
        /// <see cref="T:System.Drawing.Graphics" /> that represents a graphics context.
        /// </param>
        /// <returns>
        /// <see langword="true" /> when any portion of the specified rectangle is contained within this
        /// <see cref="T:System.Drawing.Region" />; otherwise,
        /// <see langword="false" />.
        /// </returns>
        public bool IsVisible(float x, float y, float width, float height, Graphics g)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Initializes this
        /// <see cref="System.Drawing.Region" /> to an empty interior.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void MakeEmpty()
        {
        }

        /// <summary>
        /// Initializes this
        /// <see cref="System.Drawing.Region" /> object to an infinite interior.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void MakeInfinite()
        {
        }

        /// <summary>
        /// Releases the handle of the
        /// <see cref="T:System.Drawing.Region" />.
        /// </summary>
        /// <param name="regionHandle">The handle to the
        /// <see cref="T:System.Drawing.Region" />.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="regionHandle" /> is
        /// <see langword="null" />.
        /// </exception>
        public void ReleaseHrgn(IntPtr regionHandle)
        {
        }

        /// <summary>
        /// Transforms this
        /// <see cref="System.Drawing.Region" /> by the specified
        /// <see cref="System.Drawing.Drawing2D.Matrix" />
        /// </summary>
        /// <param name="matrix">
        /// The
        /// <see cref="System.Drawing.Drawing2D.Matrix" /> by which to transform this
        /// <see cref="System.Drawing.Region" />
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="matrix" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void Transform(Matrix matrix)
        {
        }

        /// <summary>
        /// Offsets the coordinates of this
        /// <see cref="T:System.Drawing.Region" /> by the specified amount.
        /// </summary>
        /// <param name="dx">The amount to offset this
        /// <see cref="T:System.Drawing.Region" /> horizontally.
        /// </param>
        /// <param name="dy">The amount to offset this
        /// <see cref="T:System.Drawing.Region" /> vertically.
        /// </param>
        public void Translate(int dx, int dy)
        {
        }

        /// <summary>
        /// Offsets the coordinates of this
        /// <see cref="T:System.Drawing.Region" /> by the specified amount.
        /// </summary>
        /// <param name="dx">The amount to offset this
        /// <see cref="T:System.Drawing.Region" /> horizontally.
        /// </param>
        /// <param name="dy">The amount to offset this
        /// <see cref="T:System.Drawing.Region" /> vertically.
        /// </param>
        public void Translate(float dx, float dy)
        {
        }

        /// <summary>
        /// Updates this
        /// <see cref="System.Drawing.Region" /> to the union of itself and the specified
        /// <see cref="System.Drawing.Drawing2D.GraphicsPath" />
        /// </summary>
        /// <param name="path">
        /// The
        /// <see cref="System.Drawing.Drawing2D.GraphicsPath" /> to unite with this
        /// <see cref="System.Drawing.Region" />
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="path" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void Union(GraphicsPath path)
        {
        }

        /// <summary>
        /// Updates this
        /// <see cref="System.Drawing.Region" /> to the union of itself and the specified
        /// <see cref="System.Drawing.Rectangle" /> structure.
        /// </summary>
        /// <param name="rect">
        /// The
        /// <see cref="System.Drawing.Rectangle" /> structure to unite with this
        /// <see cref="System.Drawing.Region" />
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void Union(Rectangle rect)
        {
        }

        /// <summary>
        /// Updates this
        /// <see cref="System.Drawing.Region" /> to the union of itself and the specified
        /// <see cref="System.Drawing.RectangleF" /> structure.
        /// </summary>
        /// <param name="rect">
        /// The
        /// <see cref="System.Drawing.RectangleF" /> structure to unite with this
        /// <see cref="System.Drawing.Region" />
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void Union(RectangleF rect)
        {
        }

        /// <summary>
        /// Updates this
        /// <see cref="System.Drawing.Region" /> to the union of itself and the specified
        /// <see cref="System.Drawing.Region" />
        /// </summary>
        /// <param name="region">
        /// The
        /// <see cref="System.Drawing.Region" /> to unite with this
        /// <see cref="System.Drawing.Region" />
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="region" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void Union(Region region)
        {
        }

        /// <summary>
        /// Updates this
        /// <see cref="System.Drawing.Region" /> to the union minus the intersection of itself with the specified
        /// <see cref="System.Drawing.Drawing2D.GraphicsPath" />
        /// </summary>
        /// <param name="path">
        /// The
        /// <see cref="System.Drawing.Drawing2D.GraphicsPath" /> to
        /// <see cref="!:Overload:System.Drawing.Region.Xor" /> with this
        /// <see cref="System.Drawing.Region" />
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="path" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void Xor(GraphicsPath path)
        {
        }

        /// <summary>
        /// Updates this
        /// <see cref="System.Drawing.Region" /> to the union minus the intersection of itself with the specified
        /// <see cref="System.Drawing.Rectangle" /> structure.
        /// </summary>
        /// <param name="rect">
        /// The
        /// <see cref="System.Drawing.Rectangle" /> structure to
        /// <see cref="!:Overload:System.Drawing.Region.Xor" /> with this
        /// <see cref="System.Drawing.Region" />
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void Xor(Rectangle rect)
        {
        }

        /// <summary>
        /// Updates this
        /// <see cref="System.Drawing.Region" /> to the union minus the intersection of itself with the specified
        /// <see cref="System.Drawing.RectangleF" /> structure.
        /// </summary>
        /// <param name="rect">
        /// The
        /// <see cref="System.Drawing.RectangleF" /> structure to
        /// <see cref="System.Drawing.Region.Xor(System.Drawing.Drawing2D.GraphicsPath)" /> with this
        /// <see cref="System.Drawing.Region" />
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void Xor(RectangleF rect)
        {
        }

        /// <summary>
        /// Updates this
        /// <see cref="System.Drawing.Region" /> to the union minus the intersection of itself with the specified
        /// <see cref="System.Drawing.Region" />
        /// </summary>
        /// <param name="region">
        /// The
        /// <see cref="System.Drawing.Region" /> to
        /// <see cref="!:Overload:System.Drawing.Region.Xor" /> with this
        /// <see cref="System.Drawing.Region" />
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="region" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void Xor(Region region)
        {
        }
    }
}
