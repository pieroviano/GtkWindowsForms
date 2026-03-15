using System;
using System.Drawing;
using System.Windows.Forms;

namespace GtkSystem.Windows.Forms.Utility;

/// <summary>
/// ImageUtility class provides utility methods for handling image drawing and scaling in GTK applications. It includes methods for drawing background images, drawing images with specific alignment, and scaling images based on different display modes such as PictureBoxSizeMode and ImageLayout. The class utilizes Cairo for drawing operations and Gdk.Pixbuf for image manipulation. By using ImageUtility, developers can easily manage image rendering in their GTK applications while maintaining compatibility with the underlying GTK framework.
/// </summary>
public class ImageUtility
{
    /// <summary>
    /// Draws the background image.
    /// </summary>
    /// <param name="ctx">
    /// The CTX.
    /// </param>
    /// <param name="img">
    /// The img.
    /// </param>
    /// <param name="rec">
    /// The record.
    /// </param>
    public static void DrawBackgroundImage(Cairo.Context ctx, Gdk.Pixbuf img, Gdk.Rectangle rec)
    {
        ctx.Save();
        ctx.ResetClip();
        ctx.Rectangle(rec.X, rec.Y, rec.Width, rec.Height);
        ctx.Clip();
        ctx.Translate(rec.Left, rec.Top);
        Gdk.CairoHelper.SetSourcePixbuf(ctx, img, 0, 0);
        using (var p = ctx.GetSource())
        {
            if (p is Cairo.SurfacePattern pattern)
            {
                pattern.Filter = Cairo.Filter.Fast;
            }
        }
        ctx.Paint();
        ctx.Restore();
    }

    /// <summary>
    /// Draws the image.
    /// </summary>
    /// <param name="ctx">
    /// The CTX.
    /// </param>
    /// <param name="img">
    /// The img.
    /// </param>
    /// <param name="rec">
    /// The record.
    /// </param>
    /// <param name="ImageAlign">
    /// The image align.
    /// </param>
    public static void DrawImage(Cairo.Context ctx, Gdk.Pixbuf img, Gdk.Rectangle rec, ContentAlignment ImageAlign)
    {
        ctx.Save();
        if (ImageAlign == ContentAlignment.TopLeft)
            ctx.Translate(rec.X, rec.Y);
        else if (ImageAlign == ContentAlignment.TopCenter)
            ctx.Translate((img.Width - rec.Width) / 2 + rec.X, rec.Y);
        else if (ImageAlign == ContentAlignment.TopRight)
            ctx.Translate((img.Width - rec.Width) + rec.X, rec.Y);
        else if (ImageAlign == ContentAlignment.MiddleLeft)
            ctx.Translate(rec.X, (img.Height - rec.Height) / 2 + rec.Y);
        else if (ImageAlign == ContentAlignment.MiddleCenter)
            ctx.Translate((img.Width - rec.Width) / 2 + rec.X, (img.Height - rec.Height) / 2 + rec.Y);
        else if (ImageAlign == ContentAlignment.MiddleRight)
            ctx.Translate((img.Width - rec.Width) + rec.X, (img.Height - rec.Height) / 2 + rec.Y);
        else if (ImageAlign == ContentAlignment.BottomLeft)
            ctx.Translate(rec.X, (img.Height - rec.Height) + rec.Y);
        else if (ImageAlign == ContentAlignment.BottomCenter)
            ctx.Translate((img.Width - rec.Width) / 2 + rec.X, (img.Height - rec.Height) + rec.Y);
        else if (ImageAlign == ContentAlignment.BottomRight)
            ctx.Translate((img.Width - rec.Width) + rec.X, (img.Height - rec.Height) + rec.Y);
        else
            ctx.Translate(rec.X, rec.Y);

        Gdk.CairoHelper.SetSourcePixbuf(ctx, img, 0, 0);
        using (var p = ctx.GetSource())
        {
            if (p is Cairo.SurfacePattern pattern)
            {
                pattern.Filter = Cairo.Filter.Fast;
            }
        }
        ctx.Paint();
        ctx.Restore();
    }

    /// <summary>
    /// Scales the image.
    /// </summary>
    /// <param name="width">
    /// The width.
    /// </param>
    /// <param name="height">
    /// The height.
    /// </param>
    /// <param name="imagePixbuf">
    /// The image pixbuf.
    /// </param>
    /// <param name="imagebytes">
    /// The imagebytes.
    /// </param>
    /// <param name="sizeMode">
    /// The size mode.
    /// </param>
    /// <param name="backgroundMode">
    /// The background mode.
    /// </param>
    public static void ScaleImage(int width, int height, ref Gdk.Pixbuf imagePixbuf, byte[] imagebytes, PictureBoxSizeMode sizeMode, ImageLayout backgroundMode)
    {
        if (imagebytes != null)
        {
            var pix = new Gdk.Pixbuf(imagebytes);
            if (width > 0 && height > 0)
            {
                using (var surface = new Cairo.ImageSurface(Cairo.Format.ARGB32, width, height))
                {
                    var showpix = new Gdk.Pixbuf(surface, 0, 0, width, height);
                    if (sizeMode == PictureBoxSizeMode.Normal && backgroundMode == ImageLayout.None)
                    {
                        pix.CopyArea(0, 0, Math.Min(pix.Width, showpix.Width), Math.Min(pix.Height, showpix.Height), showpix, 0, 0);
                    }
                    else if (sizeMode == PictureBoxSizeMode.StretchImage || backgroundMode == ImageLayout.Stretch)
                    { // scale to fill the entire image
                        Gdk.Pixbuf newpix = pix.ScaleSimple(width, height, Gdk.InterpType.Tiles);
                        newpix.CopyArea(0, 0, newpix.Width, newpix.Height, showpix, 0, 0);
                    }
                    else if (sizeMode == PictureBoxSizeMode.CenterImage || backgroundMode == ImageLayout.Center)
                    {
                        // take the center of the original image
                        int offsetx = (pix.Width - showpix.Width) / 2;
                        int offsety = (pix.Height - showpix.Height) / 2;
                        pix.CopyArea(offsetx > 0 ? offsetx : 0, offsety > 0 ? offsety : 0, Math.Min(pix.Width, showpix.Width), Math.Min(pix.Height, showpix.Height), showpix, offsetx < 0 ? -offsetx : 0, offsety < 0 ? -offsety : 0);
                    }
                    else if (sizeMode == PictureBoxSizeMode.Zoom || backgroundMode == ImageLayout.Zoom)
                    {
                        if (pix.Width / width > pix.Height / height)
                        {
                            // the image aspect ratio is greater than the configured aspect ratio， based on width
                            Gdk.Pixbuf newpix = pix.ScaleSimple(width, width * pix.Height / pix.Width, Gdk.InterpType.Tiles);
                            newpix.CopyArea(0, 0, newpix.Width, newpix.Height, showpix, (showpix.Width - newpix.Width) / 2, (showpix.Height - newpix.Height) / 2);
                        }
                        else
                        {
                            Gdk.Pixbuf newpix = pix.ScaleSimple(height * pix.Width / pix.Height, height, Gdk.InterpType.Tiles);
                            newpix.CopyArea(0, 0, newpix.Width, newpix.Height, showpix, (showpix.Width - newpix.Width) / 2, (showpix.Height - newpix.Height) / 2);
                        }
                    }
                    else if (sizeMode == PictureBoxSizeMode.AutoSize)
                    {
                        pix.CopyArea(0, 0, Math.Min(pix.Width, showpix.Width), Math.Min(pix.Height, showpix.Height), showpix, 0, 0);
                    }

                    if (backgroundMode == ImageLayout.Tile)
                    {
                        // tile background image， fill with original image
                        if (pix.Width < width || pix.Height < height)
                        {
                            using (var surface2 = new Cairo.ImageSurface(Cairo.Format.ARGB32, width, height))
                            {
                                var backgroundpix = new Gdk.Pixbuf(surface2, 0, 0, width, height);
                                for (int y = 0; y < height; y += pix.Height)
                                {
                                    for (int x = 0; x < width; x += pix.Width)
                                    {
                                        pix.CopyArea(0, 0, width - x > pix.Width ? pix.Width : width - x, height - y > pix.Height ? pix.Height : height - y, backgroundpix, x, y);
                                    }
                                }
                                imagePixbuf = backgroundpix;
                            }
                        }
                        else
                        {
                            imagePixbuf = showpix;
                        }
                    }
                    else
                    {
                        imagePixbuf = showpix;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Background image display mode
    /// </summary>
    /// <param name="srcImageBytes">
    /// </param>
    /// <param name="width">
    /// </param>
    /// <param name="height">
    /// </param>
    /// <param name="destImage">
    /// </param>
    /// <param name="layoutMode">
    /// </param>
    public static void ScaleImageByImageLayout(byte[] srcImageBytes, int width, int height, out Gdk.Pixbuf destImage, ImageLayout layoutMode)
    {
        var srcPixbuf = new Gdk.Pixbuf(srcImageBytes);
        ScaleImageByImageLayout(srcPixbuf, width, height, out destImage, layoutMode);
    }

    /// <summary>
    /// Background image display mode
    /// </summary>
    /// <param name="srcPixbuf">
    /// </param>
    /// <param name="width">
    /// </param>
    /// <param name="height">
    /// </param>
    /// <param name="destImage">
    /// </param>
    /// <param name="layoutMode">
    /// </param>
    public static void ScaleImageByImageLayout(Gdk.Pixbuf srcPixbuf, int width, int height, out Gdk.Pixbuf destImage, ImageLayout layoutMode)
    {
        using (var surface = new Cairo.ImageSurface(Cairo.Format.ARGB32, width, height))
        {
            destImage = new Gdk.Pixbuf(surface, 0, 0, width, height);

            if (layoutMode == ImageLayout.None)
            {
                // tile the original image starting from the top-left corner， crop excess
                srcPixbuf.Scale(destImage, 0, 0, Math.Min(srcPixbuf.Width, destImage.Width), Math.Min(srcPixbuf.Height, destImage.Height), 0, 0, 1, 1, Gdk.InterpType.Tiles);
            }
            else if (layoutMode == ImageLayout.Stretch)
            { // freely scale to fill the entire image
                destImage = srcPixbuf.ScaleSimple(width, height, Gdk.InterpType.Tiles);
            }
            else if (layoutMode == ImageLayout.Center)
            {
                // take the center of the original image
                int offsetx = (destImage.Width - srcPixbuf.Width) / 2;
                int offsety = (destImage.Height - srcPixbuf.Height) / 2;
                srcPixbuf.Scale(destImage, 0, 0, destImage.Width, destImage.Height, offsetx, offsety, 1, 1, Gdk.InterpType.Tiles);
            }
            else if (layoutMode == ImageLayout.Zoom)
            {
                // scale original image proportionally， display full image
                double scaleX = destImage.Width * 1f / srcPixbuf.Width;
                double scaleY = destImage.Height * 1f / srcPixbuf.Height;
                var scaleR = Math.Min(scaleX, scaleY);
                // scale by minimum ratio
                double srcWidth = scaleX > scaleY ? srcPixbuf.Width * scaleY : srcPixbuf.Width * scaleX;
                double srcHeight = scaleX > scaleY ? srcPixbuf.Height * scaleY : srcPixbuf.Height * scaleX;

                int offsetx = (destImage.Width - (int)srcWidth) / 2;
                int offsety = (destImage.Height - (int)srcHeight) / 2;

                srcPixbuf.Scale(destImage, offsetx > 0 ? offsetx : 0, offsety > 0 ? offsety : 0, (int)Math.Min(destImage.Width, srcWidth), Math.Min(destImage.Height, (int)srcHeight), offsetx > 0 ? offsetx : 0, offsety > 0 ? offsety : 0, scaleR, scaleR, Gdk.InterpType.Tiles);
            }
            else if (layoutMode == ImageLayout.Tile)
            {
                // repeat the original image without scaling until it fills the area
                // tile background image， fill with original image
                if (srcPixbuf.Width < width || srcPixbuf.Height < height)
                {
                    using (var surface2 = new Cairo.ImageSurface(Cairo.Format.ARGB32, width, height))
                    {
                        var backgroundpix = new Gdk.Pixbuf(surface2, 0, 0, width, height);
                        for (int y = 0; y < height; y += srcPixbuf.Height)
                        {
                            for (int x = 0; x < width; x += srcPixbuf.Width)
                            {
                                srcPixbuf.CopyArea(0, 0, width - x > srcPixbuf.Width ? srcPixbuf.Width : width - x, height - y > srcPixbuf.Height ? srcPixbuf.Height : height - y, backgroundpix, x, y);
                            }
                        }
                        destImage = backgroundpix;
                    }
                }
                else
                {
                    srcPixbuf.Scale(destImage, 0, 0, Math.Min(srcPixbuf.Width, destImage.Width), Math.Min(srcPixbuf.Height, destImage.Height), 0, 0, 1, 1, Gdk.InterpType.Tiles);
                }
            }
        }
    }

    /// <summary>
    /// PictureBox image display mode
    /// </summary>
    /// <param name="srcImageBytes">
    /// </param>
    /// <param name="width">
    /// </param>
    /// <param name="height">
    /// </param>
    /// <param name="destImage">
    /// </param>
    /// <param name="sizeMode">
    /// </param>
    public static void ScaleImageByPictureBoxSizeMode(byte[] srcImageBytes, int width, int height, out Gdk.Pixbuf destImage, PictureBoxSizeMode sizeMode)
    {
        var srcPixbuf = new Gdk.Pixbuf(srcImageBytes);
        ScaleImageByPictureBoxSizeMode(srcPixbuf, width, height, out destImage, sizeMode);

    }

    /// <summary>
    /// PictureBox image display mode
    /// </summary>
    /// <param name="srcPixbuf">
    /// </param>
    /// <param name="width">
    /// </param>
    /// <param name="height">
    /// </param>
    /// <param name="destImage">
    /// </param>
    /// <param name="sizeMode">
    /// </param>
    public static void ScaleImageByPictureBoxSizeMode(Gdk.Pixbuf srcPixbuf, int width, int height, out Gdk.Pixbuf destImage, PictureBoxSizeMode sizeMode)
    {
        using (var surface = new Cairo.ImageSurface(Cairo.Format.ARGB32, width, height))
        {
            destImage = new Gdk.Pixbuf(surface, 0, 0, width, height);

            if (sizeMode == PictureBoxSizeMode.Normal)
            {
                // tile the original image starting from the top-left corner， crop excess
                srcPixbuf.Scale(destImage, 0, 0, Math.Min(srcPixbuf.Width, destImage.Width), Math.Min(srcPixbuf.Height, destImage.Height), 0, 0, 1, 1, Gdk.InterpType.Tiles);
            }
            else if (sizeMode == PictureBoxSizeMode.StretchImage)
            { // freely scale to fill the entire image
                destImage = srcPixbuf.ScaleSimple(width, height, Gdk.InterpType.Tiles);
            }
            else if (sizeMode == PictureBoxSizeMode.CenterImage)
            {
                // take the center of the original image
                int offsetx = (destImage.Width - srcPixbuf.Width) / 2;
                int offsety = (destImage.Height - srcPixbuf.Height) / 2;
                srcPixbuf.Scale(destImage, 0, 0, destImage.Width, destImage.Height, offsetx, offsety, 1, 1, Gdk.InterpType.Tiles);
            }
            else if (sizeMode == PictureBoxSizeMode.Zoom)
            {
                // scale original image proportionally， display full image
                double scaleX = destImage.Width * 1f / srcPixbuf.Width;
                double scaleY = destImage.Height * 1f / srcPixbuf.Height;
                var scaleR = Math.Min(scaleX, scaleY);
                // scale by minimum ratio
                double srcWidth = scaleX > scaleY ? srcPixbuf.Width * scaleY : srcPixbuf.Width * scaleX;
                double srcHeight = scaleX > scaleY ? srcPixbuf.Height * scaleY : srcPixbuf.Height * scaleX;

                int offsetx = (destImage.Width - (int)srcWidth) / 2;
                int offsety = (destImage.Height - (int)srcHeight) / 2;

                srcPixbuf.Scale(destImage, offsetx > 0 ? offsetx : 0, offsety > 0 ? offsety : 0, (int)Math.Min(destImage.Width, srcWidth), Math.Min(destImage.Height, (int)srcHeight), offsetx > 0 ? offsetx : 0, offsety > 0 ? offsety : 0, scaleR, scaleR, Gdk.InterpType.Tiles);
            }
            else if (sizeMode == PictureBoxSizeMode.AutoSize)
            {
                srcPixbuf.Scale(destImage, 0, 0, Math.Min(srcPixbuf.Width, destImage.Width), Math.Min(srcPixbuf.Height, destImage.Height), 0, 0, 1, 1, Gdk.InterpType.Tiles);
            }
        }

    }

}