namespace System.Runtime.Remoting.Messaging;

public class AsyncResultTests
{
    [Fact]
    public void AsyncState_ShouldBeSettable()
    {
        // Arrange
        var asyncResult = new AsyncResult();
        var state = new object();

        // Act
        asyncResult.AsyncState = state;

        // Assert
        Assert.Same(state, asyncResult.AsyncState);
    }

    [Fact]
    public void AsyncState_Default_ShouldBeNull()
    {
        // Act
        var asyncResult = new AsyncResult();

        // Assert
        Assert.Null(asyncResult.AsyncState);
    }

    [Fact]
    public void CompletedSynchronously_ShouldBeSettable()
    {
        // Arrange
        var asyncResult = new AsyncResult();

        // Act
        asyncResult.CompletedSynchronously = true;

        // Assert
        Assert.True(asyncResult.CompletedSynchronously);
    }

    [Fact]
    public void CompletedSynchronously_Default_ShouldBeFalse()
    {
        // Act
        var asyncResult = new AsyncResult();

        // Assert
        Assert.False(asyncResult.CompletedSynchronously);
    }

    [Fact]
    public void IsCompleted_ShouldBeSettable()
    {
        // Arrange
        var asyncResult = new AsyncResult();

        // Act
        asyncResult.IsCompleted = true;

        // Assert
        Assert.True(asyncResult.IsCompleted);
    }

    [Fact]
    public void IsCompleted_Default_ShouldBeFalse()
    {
        // Act
        var asyncResult = new AsyncResult();

        // Assert
        Assert.False(asyncResult.IsCompleted);
    }

    [Fact]
    public void AsyncWaitHandle_ShouldReturnNewInstanceEachTime()
    {
        // Arrange
        var asyncResult = new AsyncResult();

        // Act
        var handle1 = asyncResult.AsyncWaitHandle;
        var handle2 = asyncResult.AsyncWaitHandle;

        // Assert
        Assert.NotSame(handle1, handle2);
    }

    [Fact]
    public void AsyncWaitHandle_ShouldBeAutoResetEvent()
    {
        // Arrange
        var asyncResult = new AsyncResult();

        // Act
        var handle = asyncResult.AsyncWaitHandle;

        // Assert
        Assert.IsType<EventWaitHandle>(handle);
    }

    [Fact]
    public void AsyncWaitHandle_ShouldNotBeSignaledInitially()
    {
        // Arrange
        var asyncResult = new AsyncResult();

        // Act
        var handle = asyncResult.AsyncWaitHandle;

        // Assert
        // WaitOne with zero timeout should return false if not signaled
        Assert.False(handle.WaitOne(0));
    }

    [Fact]
    public void AsyncResult_ShouldImplementIAsyncResultContract()
    {
        // Arrange
        var asyncResult = new AsyncResult();

        // Act
        var asInterface = (IAsyncResult)asyncResult;

        // Assert
        Assert.NotNull(asInterface.AsyncWaitHandle);
        Assert.False(asInterface.CompletedSynchronously);
        Assert.False(asInterface.IsCompleted);
        Assert.Null(asInterface.AsyncState);
    }
}