namespace System.Drawing.Drawing2D;

/// <summary>
/// Encapsulates a 3-by-3 affine matrix that represents a geometric transform. This class cannot be inherited.
/// </summary>
public sealed class Matrix : MarshalByRefObject, IDisposable
{

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> class as the identity matrix.
    /// </summary>
    public Matrix()
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> class.
    /// </summary>
    /// <param name="rect">The rect.
    /// </param>
    /// <param name="plgpts">The PLGPTS.
    /// </param>
    public Matrix(Rectangle rect, Point[] plgpts)
    {

    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> class.
    /// </summary>
    /// <param name="rect">The rect.
    /// </param>
    /// <param name="plgpts">The PLGPTS.
    /// </param>
    public Matrix(RectangleF rect, PointF[] plgpts)
    {

    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> class with the specified elements.
    /// </summary>
    /// <param name="m11">
    /// The value in the first row and first column of the new
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" />.
    /// </param>
    /// <param name="m12">
    /// The value in the first row and second column of the new
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" />.
    /// </param>
    /// <param name="m21">
    /// The value in the second row and first column of the new
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" />.
    /// </param>
    /// <param name="m22">
    /// The value in the second row and second column of the new
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" />.
    /// </param>
    /// <param name="dx">
    /// The value in the third row and first column of the new
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" />.
    /// </param>
    /// <param name="dy">
    /// The value in the third row and second column of the new
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" />.
    /// </param>
    public Matrix(float m11, float m12, float m21, float m22, float dx, float dy)
    {

        this.m11 = m11;
        this.m12 = m12;
        this.m21 = m21;
        this.m22 = m22;
        this.dx = dx;
        this.dy = dy;
    }

    /// <summary>
    /// Gets or sets the angle.
    /// </summary>
    /// <value>
    /// The angle.
    /// </value>
    public float angle
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the dx.
    /// </summary>
    /// <value>
    /// The dx.
    /// </value>
    public float dx
    {
        get;
        set;
    } = 0;

    /// <summary>
    /// Gets or sets the dy.
    /// </summary>
    /// <value>
    /// The dy.
    /// </value>
    public float dy
    {
        get;
        set;
    } = 0;

    /// <summary>
    /// Gets an array of floating-point values that represents the elements of this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" />.
    /// </summary>
    /// <returns>
    /// An array of floating-point values that represents the elements of this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" />.
    /// </returns>
    public float[] Elements
    {
        get
        {
            throw new System.NotImplementedException();
        }
    }

    /// <summary>
    /// Gets or sets the initialize matrix.
    /// </summary>
    /// <value>
    /// The initialize matrix.
    /// </value>
    public Matrix? initMatrix
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the initialize point f.
    /// </summary>
    /// <value>
    /// The initialize point f.
    /// </value>
    public PointF[]? initPointF
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the initialize rectangle.
    /// </summary>
    /// <value>
    /// The initialize rectangle.
    /// </value>
    public RectangleF initRectangle
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> is invert.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if invert; otherwise,
    /// <c>false
    /// </c>.
    /// </value>
    public bool invert
    {
        get;
        set;
    }

    /// <summary>
    /// Gets a value indicating whether this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> is the identity matrix.
    /// </summary>
    /// <returns>
    /// This property is
    /// <see langword="true" /> if this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> is identity; otherwise,
    /// <see langword="false" />.
    /// </returns>
    public bool IsIdentity
    {
        get
        {
            throw new System.NotImplementedException();
        }
    }

    /// <summary>
    /// Gets a value indicating whether this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> is invertible.
    /// </summary>
    /// <returns>
    /// This property is
    /// <see langword="true" /> if this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> is invertible; otherwise,
    /// <see langword="false" />.
    /// </returns>
    public bool IsInvertible
    {
        get
        {
            throw new System.NotImplementedException();
        }
    }

    /// <summary>
    /// Gets or sets the M11.
    /// </summary>
    /// <value>
    /// The M11.
    /// </value>
    public float m11
    {
        get;
        set;
    } = 1;

    /// <summary>
    /// Gets or sets the M12.
    /// </summary>
    /// <value>
    /// The M12.
    /// </value>
    public float m12
    {
        get;
        set;
    } = 0;

    /// <summary>
    /// Gets or sets the M21.
    /// </summary>
    /// <value>
    /// The M21.
    /// </value>
    public float m21
    {
        get;
        set;
    } = 0;

    /// <summary>
    /// Gets or sets the M22.
    /// </summary>
    /// <value>
    /// The M22.
    /// </value>
    public float m22
    {
        get;
        set;
    } = 1;

    /// <summary>
    /// Gets or sets the multiply.
    /// </summary>
    /// <value>
    /// The multiply.
    /// </value>
    public Matrix? multiply
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the x translation value (the dx value, or the element in the third row and first column) of this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" />.
    /// </summary>
    /// <returns>
    /// The x translation value of this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" />.
    /// </returns>
    /// <remarks>
    /// Backed by <c>dx</c>: this used to record only the most recent Translate delta, so it disagreed with
    /// the matrix's actual translation element.
    /// </remarks>
    public float OffsetX => dx;

    /// <summary>
    /// Gets the y translation value (the dy value, or the element in the third row and second column) of this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" />.
    /// </summary>
    /// <returns>
    /// The y translation value of this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" />.
    /// </returns>
    /// <remarks>
    /// Backed by <c>dy</c>: this used to record only the most recent Translate delta, so it disagreed with
    /// the matrix's actual translation element.
    /// </remarks>
    public float OffsetY => dy;

    /// <summary>
    /// Gets or sets the order.
    /// </summary>
    /// <value>
    /// The order.
    /// </value>
    public MatrixOrder order
    {
        get;
        set;
    } = MatrixOrder.Prepend;

    /// <summary>
    /// Gets or sets the rotate at point.
    /// </summary>
    /// <value>
    /// The rotate at point.
    /// </value>
    public PointF rotateAtPoint
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the scale x.
    /// </summary>
    /// <value>
    /// The scale x.
    /// </value>
    public float scaleX
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the scale y.
    /// </summary>
    /// <value>
    /// The scale y.
    /// </value>
    public float scaleY
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the shear x.
    /// </summary>
    /// <value>
    /// The shear x.
    /// </value>
    public float shearX
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the shear y.
    /// </summary>
    /// <value>
    /// The shear y.
    /// </value>
    public float shearY
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the transform points.
    /// </summary>
    /// <value>
    /// The transform points.
    /// </value>
    public PointF[]? transformPoints
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the transform vectors.
    /// </summary>
    /// <value>
    /// The transform vectors.
    /// </value>
    public PointF[]? transformVectors
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the vector transform points.
    /// </summary>
    /// <value>
    /// The vector transform points.
    /// </value>
    public Point[]? vectorTransformPoints
    {
        get;
        set;
    }

    /// <summary>
    /// Creates an exact copy of this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" />.
    /// </summary>
    /// <returns>
    /// The
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> that this method creates.
    /// </returns>
    public Matrix? Clone()
    {
        return null;

        //return m;
    }

    /// <summary>
    /// Releases all resources used by this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" />.
    /// </summary>
    public void Dispose()
    {
        this.initMatrix = null;
        this.initRectangle = new RectangleF();
        this.initPointF = new PointF[0];
        this.vectorTransformPoints = new Point[0];
        this.transformVectors = new PointF[0];
        this.transformPoints = new PointF[0];
        this.shearX = 0;
        this.shearY = 0;
        this.angle = 0;
        this.scaleX = 0;
        this.scaleY = 0;
        this.rotateAtPoint = new PointF();
        this.order = MatrixOrder.Prepend;
        this.dx = 0;
        this.dy = 0;
        this.m11 = 1;
        this.m12 = 0;
        this.m21 = 0;
        this.m22 = 1;
        this.multiply = null;
    }

    /// <summary>
    /// Tests whether the specified object is a
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> and is identical to this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" />.
    /// </summary>
    /// <param name="obj">
    /// The object to test.
    /// </param>
    /// <returns>
    /// This method returns
    /// <see langword="true" /> if
    /// <paramref name="obj" /> is the specified
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> identical to this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" />; otherwise,
    /// <see langword="false" />.
    /// </returns>
    public override bool Equals(object obj)
    {
        return this.GetHashCode() == obj.GetHashCode();
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
    /// </returns>
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    /// <summary>
    /// Inverts this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" />, if it is invertible.
    /// </summary>
    public void Invert()
    {
        this.invert = true;
    }

    /// <summary>
    /// Multiplies this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> by the matrix specified in the
    /// <paramref name="matrix" /> parameter, by prepending the specified
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" />.
    /// </summary>
    /// <param name="matrix">
    /// The
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> by which this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> is to be multiplied.
    /// </param>
    public void Multiply(Matrix matrix)
    {
        this.initMatrix = matrix;
    }

    /// <summary>
    /// Multiplies this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> by the matrix specified in the
    /// <paramref name="matrix" /> parameter, and in the order specified in the
    /// <paramref name="order" /> parameter.
    /// </summary>
    /// <param name="matrix">
    /// The
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> by which this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> is to be multiplied.
    /// </param>
    /// <param name="order">
    /// The
    /// <see cref="T:System.Drawing.Drawing2D.MatrixOrder" /> that represents the order of the multiplication.
    /// </param>
    public void Multiply(Matrix matrix, MatrixOrder order)
    {
        this.multiply = matrix;
        this.order = order;
    }

    /// <summary>
    /// Resets this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> to have the elements of the identity matrix.
    /// </summary>
    public void Reset()
    {
        this.initMatrix = null;
        this.initRectangle = new RectangleF();
        this.initPointF = new PointF[0];
        this.vectorTransformPoints = new Point[0];
        this.transformVectors = new PointF[0];
        this.transformPoints = new PointF[0];
        this.shearX = 0;
        this.shearY = 0;
        this.angle = 0;
        this.scaleX = 0;
        this.scaleY = 0;
        this.rotateAtPoint = new PointF();
        this.order = MatrixOrder.Prepend;
        this.dx = 0;
        this.dy = 0;
        this.m11 = 1;
        this.m12 = 0;
        this.m21 = 0;
        this.m22 = 1;
        this.multiply = null;
    }

    /// <summary>
    /// Prepend to this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> a clockwise rotation, around the origin and by the specified angle.
    /// </summary>
    /// <param name="angle">
    /// The angle of the rotation, in degrees.
    /// </param>
    public void Rotate(float angle)
    {

        this.angle = angle;
    }

    /// <summary>
    /// Applies a clockwise rotation of an amount specified in the
    /// <paramref name="angle" /> parameter, around the origin (zero x and y coordinates) for this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" />.
    /// </summary>
    /// <param name="angle">
    /// The angle (extent) of the rotation, in degrees.
    /// </param>
    /// <param name="order">
    /// A
    /// <see cref="T:System.Drawing.Drawing2D.MatrixOrder" /> that specifies the order (append or prepend) in which the rotation is applied to this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" />.
    /// </param>
    public void Rotate(float angle, MatrixOrder order)
    {
        this.order = order;
        Rotate(angle);
    }

    /// <summary>
    /// Applies a clockwise rotation to this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> around the point specified in the
    /// <paramref name="point" /> parameter, and by prepending the rotation.
    /// </summary>
    /// <param name="angle">
    /// The angle (extent) of the rotation, in degrees.
    /// </param>
    /// <param name="point">
    /// A
    /// <see cref="T:System.Drawing.PointF" /> that represents the center of the rotation.
    /// </param>
    public void RotateAt(float angle, PointF point)
    {
        this.rotateAtPoint = point;
        Rotate(angle);
    }

    /// <summary>
    /// Applies a clockwise rotation about the specified point to this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> in the specified order.
    /// </summary>
    /// <param name="angle">
    /// The angle of the rotation, in degrees.
    /// </param>
    /// <param name="point">
    /// A
    /// <see cref="T:System.Drawing.PointF" /> that represents the center of the rotation.
    /// </param>
    /// <param name="order">
    /// A
    /// <see cref="T:System.Drawing.Drawing2D.MatrixOrder" /> that specifies the order (append or prepend) in which the rotation is applied.
    /// </param>
    public void RotateAt(float angle, PointF point, MatrixOrder order)
    {
        this.rotateAtPoint = point;
        this.order = order;
        Rotate(angle);
    }

    /// <summary>
    /// Applies the specified scale vector to this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> by prepending the scale vector.
    /// </summary>
    /// <param name="scaleX">
    /// The value by which to scale this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> in the x-axis direction.
    /// </param>
    /// <param name="scaleY">
    /// The value by which to scale this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> in the y-axis direction.
    /// </param>
    public void Scale(float scaleX, float scaleY)
    {
        this.scaleX = scaleX;
        this.scaleY = scaleY;
    }

    /// <summary>
    /// Applies the specified scale vector (
    /// <paramref name="scaleX" /> and
    /// <paramref name="scaleY" />) to this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> using the specified order.
    /// </summary>
    /// <param name="scaleX">
    /// The value by which to scale this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> in the x-axis direction.
    /// </param>
    /// <param name="scaleY">
    /// The value by which to scale this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> in the y-axis direction.
    /// </param>
    /// <param name="order">
    /// A
    /// <see cref="T:System.Drawing.Drawing2D.MatrixOrder" /> that specifies the order (append or prepend) in which the scale vector is applied to this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" />.
    /// </param>
    public void Scale(float scaleX, float scaleY, MatrixOrder order)
    {
        this.order = order;
        Scale(scaleX, scaleY);
    }

    /// <summary>
    /// Applies the specified shear vector to this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> by prepending the shear transformation.
    /// </summary>
    /// <param name="shearX">
    /// The horizontal shear factor.
    /// </param>
    /// <param name="shearY">
    /// The vertical shear factor.
    /// </param>
    public void Shear(float shearX, float shearY)
    {
        this.shearX = shearX;
        this.shearY = shearY;
    }

    /// <summary>
    /// Applies the specified shear vector to this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> in the specified order.
    /// </summary>
    /// <param name="shearX">
    /// The horizontal shear factor.
    /// </param>
    /// <param name="shearY">
    /// The vertical shear factor.
    /// </param>
    /// <param name="order">
    /// A
    /// <see cref="T:System.Drawing.Drawing2D.MatrixOrder" /> that specifies the order (append or prepend) in which the shear is applied.
    /// </param>
    public void Shear(float shearX, float shearY, MatrixOrder order)
    {
        this.shearX = shearX;
        this.shearY = shearY;
        this.order = order;
    }

    /// <summary>
    /// Applies the geometric transform represented by this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> to a specified array of points.
    /// </summary>
    /// <param name="pts">
    /// An array of
    /// <see cref="T:System.Drawing.PointF" /> structures that represents the points to transform.
    /// </param>
    public void TransformPoints(PointF[] pts)
    {
        // Previously this only stored the array, so every path/region/point transform was a no-op.
        if (pts == null)
        {
            throw new ArgumentNullException(nameof(pts));
        }

        for (var i = 0; i < pts.Length; i++)
        {
            var x = pts[i].X;
            var y = pts[i].Y;
            pts[i] = new PointF((x * m11) + (y * m21) + dx, (x * m12) + (y * m22) + dy);
        }

        this.transformPoints = pts;
    }

    /// <summary>
    /// Applies the geometric transform represented by this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> to a specified array of points.
    /// </summary>
    /// <param name="pts">
    /// An array of
    /// <see cref="T:System.Drawing.Point" /> structures that represents the points to transform.
    /// </param>
    public void TransformPoints(Point[] pts)
    {
        if (pts == null)
        {
            throw new ArgumentNullException(nameof(pts));
        }

        var transformed = new PointF[pts.Length];
        for (var i = 0; i < pts.Length; i++)
        {
            float x = pts[i].X;
            float y = pts[i].Y;
            var tx = (x * m11) + (y * m21) + dx;
            var ty = (x * m12) + (y * m22) + dy;
            transformed[i] = new PointF(tx, ty);
            pts[i] = new Point((int)Math.Round(tx), (int)Math.Round(ty));
        }

        this.transformPoints = transformed;
    }

    /// <summary>
    /// Multiplies each vector in an array by the matrix. The translation elements of this matrix (third row) are ignored.
    /// </summary>
    /// <param name="pts">
    /// An array of
    /// <see cref="T:System.Drawing.Point" /> structures that represents the points to transform.
    /// </param>
    public void TransformVectors(PointF[] pts)
    {
        // Vectors are transformed without the translation component.
        if (pts == null)
        {
            throw new ArgumentNullException(nameof(pts));
        }

        for (var i = 0; i < pts.Length; i++)
        {
            var x = pts[i].X;
            var y = pts[i].Y;
            pts[i] = new PointF((x * m11) + (y * m21), (x * m12) + (y * m22));
        }

        this.transformVectors = pts;
    }

    /// <summary>
    /// Applies only the scale and rotate components of this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> to the specified array of points.
    /// </summary>
    /// <param name="pts">
    /// An array of
    /// <see cref="T:System.Drawing.Point" /> structures that represents the points to transform.
    /// </param>
    public void TransformVectors(Point[] pts)
    {
        if (pts == null)
        {
            throw new ArgumentNullException(nameof(pts));
        }

        var transformed = new PointF[pts.Length];
        for (var i = 0; i < pts.Length; i++)
        {
            float x = pts[i].X;
            float y = pts[i].Y;
            var tx = (x * m11) + (y * m21);
            var ty = (x * m12) + (y * m22);
            transformed[i] = new PointF(tx, ty);
            pts[i] = new Point((int)Math.Round(tx), (int)Math.Round(ty));
        }

        this.transformVectors = transformed;
    }

    /// <summary>
    /// Applies the specified translation vector (
    /// <paramref name="offsetX" /> and
    /// <paramref name="offsetY" />) to this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> by prepending the translation vector.
    /// </summary>
    /// <param name="offsetX">
    /// The x value by which to translate this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" />.
    /// </param>
    /// <param name="offsetY">
    /// The y value by which to translate this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" />.
    /// </param>
    public void Translate(float offsetX, float offsetY)
    {
        Translate(offsetX, offsetY, this.order);
    }

    /// <summary>
    /// Applies the specified translation vector to this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> in the specified order.
    /// </summary>
    /// <param name="offsetX">
    /// The x value by which to translate this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" />.
    /// </param>
    /// <param name="offsetY">
    /// The y value by which to translate this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" />.
    /// </param>
    /// <param name="order">
    /// A
    /// <see cref="T:System.Drawing.Drawing2D.MatrixOrder" /> that specifies the order (append or prepend) in which the translation is applied to this
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" />.
    /// </param>
    public void Translate(float offsetX, float offsetY, MatrixOrder order)
    {
        this.order = order;

        // Apply the translation to the matrix's translation components. Previously only OffsetX/OffsetY
        // were recorded, so a translated matrix still transformed points as if untranslated.
        if (order == MatrixOrder.Append)
        {
            dx += offsetX;
            dy += offsetY;
        }
        else
        {
            dx += (offsetX * m11) + (offsetY * m21);
            dy += (offsetX * m12) + (offsetY * m22);
        }
    }

    /// <summary>
    /// Multiplies each vector in an array by the matrix. The translation elements of this matrix (third row) are ignored.
    /// </summary>
    /// <param name="pts">
    /// An array of
    /// <see cref="T:System.Drawing.Point" /> structures that represents the points to transform.
    /// </param>
    public void VectorTransformPoints(Point[] pts)
    {

        this.vectorTransformPoints = pts;
    }
}