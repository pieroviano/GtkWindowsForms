// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Drawing;

namespace System.Windows.Forms
{
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

            ~NativeImageList()
            {
                // There are certain code paths where we are unable to track the lifetime of the object,
                // for example in the following scenarios:
                //
                //      this.imageList1.ImageStream = (System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream"));
                // or
                //      resources.ApplyResources(this.listView1, "listView1");
                //
                // In those cases the loose instances will be collected by the GC.
                Dispose(disposing: false);
            }

            //#if DEBUG
            //            Debug.Fail($"{nameof(NativeImageList)} could not be duplicated. Originating stack:\n{_callStack}");
            //#endif
            //            throw new InvalidOperationException(SR.ImageListDuplicateFailed);
            //}
        }
    }
}