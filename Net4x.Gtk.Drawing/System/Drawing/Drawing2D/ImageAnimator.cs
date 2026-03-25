using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Imaging;

namespace System.Drawing;

/// <summary>
/// Animates an image that has time-based frames.
/// </summary>
/// <filterpriority>
/// 1
/// </filterpriority>
public sealed class ImageAnimator
{

    internal const int AnimationDelayMS = 40;

    private static Thread s_animationThread;

    private static bool s_anyFrameDirty;

    private static List<ImageInfo> s_imageInfoList;

    private static readonly ReaderWriterLock s_rwImgListLock = new ReaderWriterLock();

    [ThreadStatic]
    private static int t_threadWriterLockWaitCount;

    private ImageAnimator()
    {
    }

    /// <summary>
    /// Displays a multiple-frame image as an animation.
    /// </summary>
    /// <param name="image">
    /// The
    /// <see cref="T:System.Drawing.Image" /> object to animate.
    /// </param>
    /// <param name="onFrameChangedHandler">
    /// An
    /// <see langword="EventHandler" /> object that specifies the method that is called when the animation frame changes.
    /// </param>
    public static void Animate(Image image, EventHandler onFrameChangedHandler)
    {
        if (image == null)
        {
            return;
        }
        ImageInfo imageInfo = null;
        lock (image)
        {
            imageInfo = new ImageInfo(image);
        }
        StopAnimate(image, onFrameChangedHandler);
        bool isReaderLockHeld = s_rwImgListLock.IsReaderLockHeld;
        var lockCookie = default(LockCookie);
        t_threadWriterLockWaitCount++;
        try
        {
            if (isReaderLockHeld)
            {
                lockCookie = s_rwImgListLock.UpgradeToWriterLock(-1);
            }
            else
            {
                s_rwImgListLock.AcquireWriterLock(-1);
            }
        }
        finally
        {
            t_threadWriterLockWaitCount--;
        }
        try
        {
            if (imageInfo.Animated)
            {
                if (s_imageInfoList == null)
                {
                    s_imageInfoList = new List<ImageInfo>();
                }
                imageInfo.FrameChangedHandler = onFrameChangedHandler;
                s_imageInfoList.Add(imageInfo);
                if (s_animationThread == null)
                {
                    s_animationThread = new Thread(new ThreadStart(AnimateImages))
                    {
                        Name = "ImageAnimator",
                        IsBackground = true
                    };
                    s_animationThread.Start();
                }
            }
        }
        finally
        {
            if (isReaderLockHeld)
            {
                s_rwImgListLock.DowngradeFromWriterLock(ref lockCookie);
            }
            else
            {
                s_rwImgListLock.ReleaseWriterLock();
            }
        }
    }

    /// <summary>
    /// Returns a Boolean value indicating whether the specified image contains time-based frames.
    /// </summary>
    /// <param name="image">
    /// The
    /// <see cref="T:System.Drawing.Image" /> object to test.
    /// </param>
    /// <returns>
    /// This method returns
    /// <see langword="true" /> if the specified image contains time-based frames; otherwise,
    /// <see langword="false" />.
    /// </returns>
    public static bool CanAnimate([NotNullWhen(true)] Image image)
    {
        if (image == null)
        {
            return false;
        }
        lock (image)
        {
            Guid[] frameDimensionsList = image!.FrameDimensionsList;
            var array = frameDimensionsList;
            foreach (Guid guid in array)
            {
                var frameDimension = new FrameDimension(guid);
                if (frameDimension.Equals(FrameDimension.Time))
                {
                    return image!.GetFrameCount(FrameDimension.Time) > 1;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Terminates a running animation.
    /// </summary>
    /// <param name="image">
    /// The
    /// <see cref="T:System.Drawing.Image" /> object to stop animating.
    /// </param>
    /// <param name="onFrameChangedHandler">
    /// An
    /// <see langword="EventHandler" /> object that specifies the method that is called when the animation frame changes.
    /// </param>
    public static void StopAnimate(Image image, EventHandler onFrameChangedHandler)
    {
        if (image == null || s_imageInfoList == null)
        {
            return;
        }
        bool isReaderLockHeld = s_rwImgListLock.IsReaderLockHeld;
        var lockCookie = default(LockCookie);
        t_threadWriterLockWaitCount++;
        try
        {
            if (isReaderLockHeld)
            {
                lockCookie = s_rwImgListLock.UpgradeToWriterLock(-1);
            }
            else
            {
                s_rwImgListLock.AcquireWriterLock(-1);
            }
        }
        finally
        {
            t_threadWriterLockWaitCount--;
        }
        try
        {
            for (int i = 0; i < s_imageInfoList.Count; i++)
            {
                var imageInfo = s_imageInfoList[i];
                if (image == imageInfo.Image)
                {
                    if ((Delegate?)onFrameChangedHandler == (Delegate?)imageInfo.FrameChangedHandler || (onFrameChangedHandler != null && onFrameChangedHandler.Equals(imageInfo.FrameChangedHandler)))
                    {
                        s_imageInfoList.Remove(imageInfo);
                    }
                    break;
                }
            }
        }
        finally
        {
            if (isReaderLockHeld)
            {
                s_rwImgListLock.DowngradeFromWriterLock(ref lockCookie);
            }
            else
            {
                s_rwImgListLock.ReleaseWriterLock();
            }
        }
    }

    /// <summary>
    /// Advances the frame in the specified image. The new frame is drawn the next time the image is rendered. This method applies only to images with time-based frames.
    /// </summary>
    /// <param name="image">
    /// The
    /// <see cref="T:System.Drawing.Image" /> object for which to update frames.
    /// </param>
    public static void UpdateFrames(Image image)
    {
        if (image == null || s_imageInfoList == null || t_threadWriterLockWaitCount > 0)
        {
            return;
        }
        s_rwImgListLock.AcquireReaderLock(-1);
        try
        {
            var flag = false;
            var flag2 = false;
            foreach (ImageInfo s_imageInfo in s_imageInfoList)
            {
                if (s_imageInfo.Image == image)
                {
                    if (s_imageInfo.FrameDirty)
                    {
                        lock (s_imageInfo.Image)
                        {
                            s_imageInfo.UpdateFrame();
                        }
                    }
                    flag2 = true;
                }
                else if (s_imageInfo.FrameDirty)
                {
                    flag = true;
                }
                if (flag && flag2)
                {
                    break;
                }
            }
            s_anyFrameDirty = flag;
        }
        finally
        {
            s_rwImgListLock.ReleaseReaderLock();
        }
    }

    /// <summary>
    /// Advances the frame in all images currently being animated. The new frame is drawn the next time the image is rendered.
    /// </summary>
    public static void UpdateFrames()
    {
        if (!s_anyFrameDirty || s_imageInfoList == null || t_threadWriterLockWaitCount > 0)
        {
            return;
        }
        s_rwImgListLock.AcquireReaderLock(-1);
        try
        {
            foreach (ImageInfo s_imageInfo in s_imageInfoList)
            {
                lock (s_imageInfo.Image)
                {
                    s_imageInfo.UpdateFrame();
                }
            }
            s_anyFrameDirty = false;
        }
        finally
        {
            s_rwImgListLock.ReleaseReaderLock();
        }
    }

    private static void AnimateImages()
    {
        var stopwatch = Stopwatch.StartNew();
        while (true)
        {
            Thread.Sleep(40);
            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            stopwatch.Restart();
            s_rwImgListLock.AcquireReaderLock(-1);
            try
            {
                for (int i = 0; i < s_imageInfoList.Count; i++)
                {
                    var imageInfo = s_imageInfoList[i];
                    if (imageInfo.Animated)
                    {
                        imageInfo.AdvanceAnimationBy(elapsedMilliseconds);
                        if (imageInfo.FrameDirty)
                        {
                            s_anyFrameDirty = true;
                        }
                    }
                }
            }
            finally
            {
                s_rwImgListLock.ReleaseReaderLock();
            }
        }
    }

    private sealed class ImageInfo
    {

        private readonly bool _animated;

        private int _frame;

        private readonly int _frameCount;

        private bool _frameDirty;

        private readonly long[] _frameEndTimes;

        private long _frameTimer;

        private readonly Image _image;

        private short _loop;

        private readonly short _loopCount;

        private EventHandler _onFrameChangedHandler;

        private readonly long _totalAnimationTime;

        private const int PropertyTagFrameDelay = 20736;

        private const int PropertyTagLoopCount = 20737;

        /// <summary>
        /// Initializes a new instance of
        /// <see cref="T:System.Drawing.Image" /> from a raw image.
        /// </summary>
        /// <param name="image">The raw image.
        /// </param>
        public ImageInfo(Image image)
        {
            _image = image;
            _animated = CanAnimate(image);
            _frameEndTimes = null;
            if (_animated)
            {
                _frameCount = image.GetFrameCount(FrameDimension.Time);
                PropertyItem propertyItem = image.GetPropertyItem(20736);
                if (propertyItem != null)
                {
                    byte[] value = propertyItem.Value;
                    _frameEndTimes = new long[_frameCount];
                    var num = 0L;
                    var num2 = 0;
                    var num3 = 0;
                    while (num2 < _frameCount)
                    {
                        if (num3 >= value.Length)
                        {
                            num3 = 0;
                        }
                        var num4 = BitConverter.ToInt32(value, num3) * 10;
                        num += ((num4 > 0) ? num4 : 40);
                        if (num < _totalAnimationTime)
                        {
                            num = _totalAnimationTime;
                        }
                        else
                        {
                            _totalAnimationTime = num;
                        }
                        _frameEndTimes[num2] = num;
                        num2++;
                        num3 += 4;
                    }
                }
                PropertyItem propertyItem2 = image.GetPropertyItem(20737);
                if (propertyItem2 != null)
                {
                    byte[] value2 = propertyItem2.Value;
                    _loopCount = BitConverter.ToInt16(value2, 0);
                }
                else
                {
                    _loopCount = 0;
                }
            }
            else
            {
                _frameCount = 1;
            }
        }

        public bool Animated => _animated;

        public EventHandler FrameChangedHandler
        {
            get
            {
                return _onFrameChangedHandler;
            }
            set
            {
                _onFrameChangedHandler = value;
            }
        }

        public bool FrameDirty => _frameDirty;

        internal Image Image => _image;

        private bool ShouldAnimate
        {
            get
            {
                if (TotalAnimationTime <= 0)
                {
                    return false;
                }
                if (_loopCount != 0)
                {
                    return _loop <= _loopCount;
                }

                return true;
            }
        }

        private long TotalAnimationTime
        {
            get
            {
                if (!Animated)
                {
                    return 0L;
                }

                return _totalAnimationTime;
            }
        }

        public void AdvanceAnimationBy(long milliseconds)
        {
            if (!ShouldAnimate)
            {
                return;
            }
            var frame = _frame;
            _frameTimer += milliseconds;
            if (_frameTimer > TotalAnimationTime)
            {
                _loop += (short)Math.DivRem(_frameTimer, TotalAnimationTime, out var result);
                _frameTimer = result;
                if (!ShouldAnimate)
                {
                    _frame = _frameCount - 1;
                    _frameTimer = TotalAnimationTime;
                }
                else if (_frame > 0 && _frameTimer < _frameEndTimes[_frame - 1])
                {
                    _frame = 0;
                }
            }
            while (_frameTimer > _frameEndTimes[_frame])
            {
                _frame++;
            }
            if (_frame != frame)
            {
                _frameDirty = true;
                OnFrameChanged(EventArgs.Empty);
            }
        }

        internal void UpdateFrame()
        {
            if (_frameDirty)
            {
                _image.SelectActiveFrame(FrameDimension.Time, _frame);
                _frameDirty = false;
            }
        }

        private void OnFrameChanged(EventArgs e)
        {
            _onFrameChangedHandler?.Invoke(_image, e);
        }
    }
}