extern alias gtk;
using ColorMatrix = gtk::System.Drawing.Imaging.ColorMatrix;
using GtkLogger = gtk::System.Diagnostics.GtkLogger;
using LogLevel = gtk::System.Diagnostics.LogLevel;

namespace System.Drawing;

/// <summary>Behavioural tests for <see cref="ColorMatrix"/> storage and for the logger's dispatch to a
/// custom sink.</summary>
[Collection("NonParallelCollection")]
public class ColorMatrixLoggerFunctionalTests
{
    [Fact]
    public void NewColorMatrix_IsTheIdentityMatrix()
    {
        var cm = new ColorMatrix();

        for (var row = 0; row < 5; row++)
        {
            for (var col = 0; col < 5; col++)
            {
                var expected = row == col ? 1f : 0f;
                Assert.Equal(expected, cm[row, col], 3);
            }
        }
    }

    [Fact]
    public void Indexer_RoundTripsAssignedValues()
    {
        var cm = new ColorMatrix();

        cm[1, 2] = 0.25f;
        cm[4, 0] = -1.5f;

        Assert.Equal(0.25f, cm[1, 2], 3);
        Assert.Equal(-1.5f, cm[4, 0], 3);
        Assert.Equal(1f, cm[0, 0], 3); // untouched entry keeps the identity value
    }

    [Fact]
    public void NamedMatrixProperty_AgreesWithTheIndexer()
    {
        var cm = new ColorMatrix();
        cm[0, 0] = 0.5f;
        cm[3, 3] = 0.75f;

        Assert.Equal(cm[0, 0], cm.Matrix00, 3);
        Assert.Equal(cm[3, 3], cm.Matrix33, 3);
    }

    [Fact]
    public void ArrayConstructor_PreservesSuppliedValues()
    {
        var values = new float[5][];
        for (var i = 0; i < 5; i++)
        {
            values[i] = new float[5];
            values[i][i] = 1f;
        }

        values[2][3] = 0.6f;
        var cm = new ColorMatrix(values);

        Assert.Equal(0.6f, cm[2, 3], 3);
        Assert.Equal(1f, cm[2, 2], 3);
    }

    [Fact]
    public void GtkLogger_ForwardsMessageAndLevelToCustomSink()
    {
        var captured = new List<(string Message, LogLevel Level, Exception? Ex)>();
        try
        {
            GtkLogger.SetCustomLogAction((msg, level, ex) => captured.Add((msg, level, ex)));

            GtkLogger.Info("hello-info");
            GtkLogger.Warn("hello-warn");

            Assert.Contains(captured, c => c.Message.Contains("hello-info"));
            Assert.Contains(captured, c => c.Message.Contains("hello-warn"));
        }
        finally
        {
            GtkLogger.SetCustomLogAction(null);
        }
    }

    [Fact]
    public void GtkLogger_ForwardsTheExceptionItWasGiven()
    {
        var captured = new List<Exception?>();
        var boom = new InvalidOperationException("boom");
        try
        {
            GtkLogger.SetCustomLogAction((msg, level, ex) => captured.Add(ex));

            GtkLogger.Error("failed", boom);

            Assert.Contains(boom, captured);
        }
        finally
        {
            GtkLogger.SetCustomLogAction(null);
        }
    }
}
