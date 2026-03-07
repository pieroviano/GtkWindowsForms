// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    /// The ImageList is an object that stores a collection of Images, most
    /// commonly used by other controls, such as the ListView, TreeView, or
    /// Toolbar. You can add either bitmaps or Icons to the ImageList, and the
    /// other controls will be able to use the Images as they desire.
    /// </summary>
    /// <filterpriority>2
    /// </filterpriority>
    /// The ImageList is an object that stores a collection of Images, most
    /// commonly used by other controls, such as the ListView, TreeView, or
    /// Toolbar. You can add either bitmaps or Icons to the ImageList, and the
    /// other controls will be able to use the Images as they desire.
    /// </summary>
    /// <filterpriority>2
    /// </filterpriority>
    /// The ImageList is an object that stores a collection of Images, most
    /// commonly used by other controls, such as the ListView, TreeView, or
    /// Toolbar. You can add either bitmaps or Icons to the ImageList, and the
    /// other controls will be able to use the Images as they desire.
    /// </summary>
    /// <filterpriority>2
    /// </filterpriority>
    /// The ImageList is an object that stores a collection of Images, most
    /// commonly used by other controls, such as the ListView, TreeView, or
    /// Toolbar. You can add either bitmaps or Icons to the ImageList, and the
    /// other controls will be able to use the Images as they desire.
    /// </summary>
    /// <filterpriority>2
    /// </filterpriority>
    /// The ImageList is an object that stores a collection of Images, most
    /// commonly used by other controls, such as the ListView, TreeView, or
    /// Toolbar. You can add either bitmaps or Icons to the ImageList, and the
    /// other controls will be able to use the Images as they desire.
    /// </summary>
    /// <filterpriority>2
    /// </filterpriority>
    /// The ImageList is an object that stores a collection of Images, most
    /// commonly used by other controls, such as the ListView, TreeView, or
    /// Toolbar. You can add either bitmaps or Icons to the ImageList, and the
    /// other controls will be able to use the Images as they desire.
    /// </summary>
    /// <filterpriority>2
    /// </filterpriority>
    /// The ImageList is an object that stores a collection of Images, most
    /// commonly used by other controls, such as the ListView, TreeView, or
    /// Toolbar. You can add either bitmaps or Icons to the ImageList, and the
    /// other controls will be able to use the Images as they desire.
    /// </summary>
    /// <filterpriority>2
    /// </filterpriority>
    public sealed partial class ImageList
    {
        internal class NativeImageList : IDisposable
        {
#if DEBUG
            private readonly string _callStack = new StackTrace().ToString();
#endif

            private const int GrowBy = 4;

            private const int InitialCapacity = 4;

            private static readonly object s_syncLock = new object();

            //private NativeImageList(HIMAGELIST himl)
            //{
            //    HIMAGELIST = himl;
            //}

            //        private void Init(HIMAGELIST himl)
            //        {
            //            if (!himl.IsNull)
            //            {
            //                HIMAGELIST = himl;
            //                return;
            //            }

            //#if DEBUG
            //            Debug.Fail($"{nameof(NativeImageList)} could not be created. Originating stack:\n{_callStack}");
            //#endif
            //            throw new InvalidOperationException(SR.ImageListCreateFailed);
            //        }

            //        HIMAGELIST IHandle<HIMAGELIST>.Handle => HIMAGELIST;

            //        internal HIMAGELIST HIMAGELIST { get; private set; }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {

                //    PInvoke.ImageList.Destroy(this);
                //    HIMAGELIST = HIMAGELIST.Null;
                //}
            }

            //#if DEBUG
            //            Debug.Fail($"{nameof(NativeImageList)} could not be duplicated. Originating stack:\n{_callStack}");
            //#endif
            //            throw new InvalidOperationException(SR.ImageListDuplicateFailed);
            //}
        }
    }
}
