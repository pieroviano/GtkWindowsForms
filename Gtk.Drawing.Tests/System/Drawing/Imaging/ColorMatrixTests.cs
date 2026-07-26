extern alias gtk;
using ColorMatrix = gtk::System.Drawing.Imaging.ColorMatrix;

namespace System.Drawing.Imaging;

public class ColorMatrixTests
{
    #region Constructors

    [Fact]
    public void DefaultConstructor_InitializesIdentityMatrix()
    {
        var matrix = new ColorMatrix();

        Assert.Equal(1f, matrix.Matrix00);
        Assert.Equal(1f, matrix.Matrix11);
        Assert.Equal(1f, matrix.Matrix22);
        Assert.Equal(1f, matrix.Matrix33);
        Assert.Equal(1f, matrix.Matrix44);

        // spot check others are zero
        Assert.Equal(0f, matrix.Matrix01);
        Assert.Equal(0f, matrix.Matrix10);
    }

    [Fact]
    public void Constructor_WithArray_SetsValues()
    {
        var data = CreateMatrix(5f);

        var matrix = new ColorMatrix(data);

        Assert.Equal(5f, matrix.Matrix00);
        Assert.Equal(5f, matrix.Matrix44);
        Assert.Equal(5f, matrix.Matrix23);
    }

    #endregion

    #region Properties

    [Fact]
    public void Properties_SetGet_Works()
    {
        var m = new ColorMatrix
        {
            Matrix12 = 7.5f,
            Matrix34 = 3.2f
        };

        Assert.Equal(7.5f, m.Matrix12);
        Assert.Equal(3.2f, m.Matrix34);
    }

    #endregion

    #region Indexer

    [Fact]
    public void Indexer_Get_ReturnsCorrectValue()
    {
        var matrix = new ColorMatrix
        {
            Matrix21 = 9f
        };

        Assert.Equal(9f, matrix[2, 1]);
    }

    [Fact]
    public void Indexer_Set_UpdatesUnderlyingMatrix()
    {
        var matrix = new ColorMatrix
        {
            [3, 4] = 11f
        };

        Assert.Equal(11f, matrix.Matrix34);
    }

    [Fact]
    public void Indexer_SetThenGet_RoundTrip()
    {
        var matrix = new ColorMatrix
        {
            [1, 2] = 42f
        };

        Assert.Equal(42f, matrix[1, 2]);
    }

    #endregion

    #region GetMatrix

    [Fact]
    public void GetMatrix_ReturnsCorrect2DArray()
    {
        var matrix = new ColorMatrix
        {
            Matrix00 = 1,
            Matrix11 = 2,
            Matrix22 = 3
        };

        var result = matrix.GetMatrix();

        Assert.Equal(1f, result[0][0]);
        Assert.Equal(2f, result[1][1]);
        Assert.Equal(3f, result[2][2]);
    }

    #endregion

    #region SetMatrix

    [Fact]
    public void SetMatrix_UpdatesAllValues()
    {
        var matrix = new ColorMatrix();
        var data = CreateMatrix(7f);

        matrix.SetMatrix(data);

        Assert.Equal(7f, matrix.Matrix00);
        Assert.Equal(7f, matrix.Matrix14);
        Assert.Equal(7f, matrix.Matrix41);
    }

    #endregion

    #region PinnableReference

    [Fact]
    public void GetPinnableReference_ReturnsReferenceToFirstElement()
    {
        var matrix = new ColorMatrix();

        ref float reference = ref matrix.GetPinnableReference();

        reference = 99f;

        Assert.Equal(99f, matrix.Matrix00);
    }

    #endregion

    #region Helpers

    private float[][] CreateMatrix(float value)
    {
        var data = new float[5][];
        for (int i = 0; i < 5; i++)
        {
            data[i] = new float[5];
            for (int j = 0; j < 5; j++)
            {
                data[i][j] = value;
            }
        }
        return data;
    }

    #endregion
}