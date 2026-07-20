/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */

using Gtk;

namespace System.Windows.Forms.GtkRender;

/// <summary>
/// Class CellReandererUtility.
/// </summary>
public class CellReandererUtility
{
    internal string? GetFormatText(object? text, DataGridViewCellStyle? cellstyle)
    {
        if (text == null && cellstyle != null)
            text = Convert.ToString(cellstyle.NullValue) ?? string.Empty;
        if (cellstyle != null && string.IsNullOrWhiteSpace(cellstyle.Format) == false)
            return string.Format(cellstyle.Format??string.Empty, text);
        else
            return text?.ToString();
    }

    internal DataGridViewCellStyle? GetStyle(DataGridViewColumn col, DataGridViewCell cell)
    {
        DataGridViewCellStyle? style = cell.Style;
        if (style == null)
            style = cell.RowStyle;

        if (style == null)
        {
            if (cell.DataGridView?.AlternatingRowsDefaultCellStyle != null && cell.OwningRow?.Index % 2 == 1)
                style = cell.DataGridView.AlternatingRowsDefaultCellStyle;
            else
                style = cell.DataGridView?.RowsDefaultCellStyle;
        }
        if (style == null)
        {
            style = col.DefaultCellStyle ?? cell.DataGridView?.DefaultCellStyle;
        }

        return style;
    }

    internal void SetControlWithStyle(CellRenderer cell, DataGridViewCellStyle? cellstyle)
    {
        if (cellstyle != null)
        {
            if (cellstyle.BackColor.Name != "0")
                cell.CellBackgroundRgba = new Gdk.RGBA() { Alpha = cellstyle.BackColor.A / 255f, Blue = cellstyle.BackColor.B / 255f, Green = cellstyle.BackColor.G / 255f, Red = cellstyle.BackColor.R / 255f };
            if (cellstyle.Alignment == DataGridViewContentAlignment.TopLeft)
                cell.SetAlignment(0, 0);
            else if (cellstyle.Alignment == DataGridViewContentAlignment.TopCenter)
                cell.SetAlignment(0.5f, 0);
            else if (cellstyle.Alignment == DataGridViewContentAlignment.TopRight)
                cell.SetAlignment(1.0f, 0);
            else if (cellstyle.Alignment == DataGridViewContentAlignment.MiddleLeft)
                cell.SetAlignment(0, 0.5f);
            else if (cellstyle.Alignment == DataGridViewContentAlignment.MiddleCenter)
                cell.SetAlignment(0.5f, 0.5f);
            else if (cellstyle.Alignment == DataGridViewContentAlignment.MiddleRight)
                cell.SetAlignment(1.0f, 0.5f);
            else if (cellstyle.Alignment == DataGridViewContentAlignment.BottomLeft)
                cell.SetAlignment(0, 1f);
            else if (cellstyle.Alignment == DataGridViewContentAlignment.BottomCenter)
                cell.SetAlignment(0.5f, 1f);
            else if (cellstyle.Alignment == DataGridViewContentAlignment.BottomRight)
                cell.SetAlignment(1.0f, 1f);
        }
    }

    internal void SetTextWithStyle(CellRendererText cell, DataGridViewCellStyle? cellstyle, object? value)
    {
        SetTextWithStyle(cell, cellstyle);
        cell.Text = GetFormatText(value, cellstyle);
    }

    internal void SetTextWithStyle(CellRendererText cell, DataGridViewCellStyle? cellstyle)
    {
        if (cellstyle != null)
        {
            if (cellstyle.WrapMode == DataGridViewTriState.True)
            {
                cell.WrapMode = Pango.WrapMode.WordChar;
                cell.WrapWidth = cell.WidthChars;
            }
            if (cellstyle.ForeColor.Name != "0")
                if (cellstyle != null)
                {
                    cell.ForegroundRgba = new Gdk.RGBA()
                    {
                        Alpha = cellstyle.ForeColor.A / 255f, Blue = cellstyle.ForeColor.B / 255f,
                        Green = cellstyle.ForeColor.G / 255f, Red = cellstyle.ForeColor.R / 255f
                    };
                }

            if (cellstyle != null && cellstyle.BackColor.Name != "0")
                cell.CellBackgroundRgba = new Gdk.RGBA() { Alpha = cellstyle.BackColor.A / 255f, Blue = cellstyle.BackColor.B / 255f, Green = cellstyle.BackColor.G / 255f, Red = cellstyle.BackColor.R / 255f };
            if (cellstyle != null && cellstyle.Alignment == DataGridViewContentAlignment.TopLeft)
                cell.SetAlignment(0, 0);
            else if (cellstyle != null && cellstyle.Alignment == DataGridViewContentAlignment.TopCenter)
                cell.SetAlignment(0.5f, 0);
            else if (cellstyle != null && cellstyle.Alignment == DataGridViewContentAlignment.TopRight)
                cell.SetAlignment(1.0f, 0);
            else if (cellstyle != null && cellstyle.Alignment == DataGridViewContentAlignment.MiddleLeft)
                cell.SetAlignment(0, 0.5f);
            else if (cellstyle != null && cellstyle.Alignment == DataGridViewContentAlignment.MiddleCenter)
                cell.SetAlignment(0.5f, 0.5f);
            else if (cellstyle != null && cellstyle.Alignment == DataGridViewContentAlignment.MiddleRight)
                cell.SetAlignment(1.0f, 0.5f);
            else if (cellstyle != null && cellstyle.Alignment == DataGridViewContentAlignment.BottomLeft)
                cell.SetAlignment(0, 1f);
            else if (cellstyle != null && cellstyle.Alignment == DataGridViewContentAlignment.BottomCenter)
                cell.SetAlignment(0.5f, 1f);
            else if (cellstyle != null && cellstyle.Alignment == DataGridViewContentAlignment.BottomRight)
                cell.SetAlignment(1.0f, 1f);
        }
    }
}