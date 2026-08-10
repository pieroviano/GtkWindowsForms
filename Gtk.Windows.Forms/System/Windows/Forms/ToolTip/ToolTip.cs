/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 */
using System.ComponentModel;
using System.Drawing;
using Gtk;
using GtkSystem.Windows.Forms.Resources;

namespace System.Windows.Forms;
/// <summary>
/// Defines a set of standardized icons that can be associated with a ToolTip.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
[ProvideProperty(nameof(ToolTip), typeof(Control))]
[DefaultEvent(nameof(Popup))]
[ToolboxItemFilter("System.Windows.Forms")]
public partial class ToolTip : Component, IExtenderProvider
{

    internal const int DefaultDelay = 500;

    private bool _auto = true;

    private Color _backColor = SystemColors.Info;

    private readonly int[] _delayTimes = new int[4];

    private Color _foreColor = SystemColors.InfoText;

    private bool _isBalloon;

    private bool _isDisposing;

    private bool _showAlways;

    private bool _stripAmpersands;

    private ToolTipTimer? _timer;

    private readonly Dictionary<Control, TipInfo> _tools = new();

    private ToolTipIcon _toolTipIcon = ToolTipIcon.None;

    private string _toolTipTitle = string.Empty;

    private Control? _topLevelControl;

    private bool _useAnimation = true;

    private bool _useFading = true;

    private bool active = true;

    private const int AutoPopRatio = 10;

    private const int InfiniteDelay = short.MaxValue;

    private const int ReshowRatio = 5;

    public virtual event DrawToolTipEventHandler? Draw;

    public virtual event PopupEventHandler? Popup;

    public ToolTip(IContainer cont)
        : this()
    {
        if (cont == null)
            throw new ArgumentNullException(nameof(cont));

        cont.Add(this);
    }

    public ToolTip()
    {
        InitialDelay = 500;
    }

    [DefaultValue(true)]
    public bool Active
    {
        get => active;
        set
        {
            if (active != value)
            {
                active = value;

            }
        }
    }

    [DefaultValue(DefaultDelay)]
    public int AutomaticDelay
    {
        get => _delayTimes[0] < 10 ? DefaultDelay : _delayTimes[0];
        set
        {
            // Setting AutomaticDelay derives the other three delays, as WinForms does.
            _delayTimes[0] = value;
            _delayTimes[1] = value * AutoPopRatio;
            _delayTimes[2] = value;
            _delayTimes[3] = value / ReshowRatio;
        }
    }

    public int AutoPopDelay
    {
        get => Math.Max(1000, _delayTimes[1]);
        set
        {
            _delayTimes[1] = value;
        }
    }

    [DefaultValue(typeof(Color), "Info")]
    public Color BackColor
    {
        get => _backColor;
        set
        {
            _backColor = value;
        }
    }

    public Color ForeColor
    {
        get => _foreColor;
        set
        {
            if (value.IsEmpty)
            {
                throw new ArgumentException(string.Format("ToolTip Empty Color {0}:{1}", nameof(ForeColor)), nameof(value));
            }

            _foreColor = value;
        }
    }

    public int InitialDelay
    {
        get => _delayTimes[2];
        set
        {
            // This used to overwrite all four delays, so assigning InitialDelay silently reset
            // AutomaticDelay, AutoPopDelay and ReshowDelay. Only the initial delay belongs here.
            _delayTimes[2] = value;
        }
    }

    public bool IsBalloon
    {
        get => _isBalloon;
        set
        {
            if (_isBalloon != value)
            {
                _isBalloon = value;
            }
        }
    }

    public bool OwnerDraw
    {
        get;
        set;
    }

    public int ReshowDelay
    {
        get => _delayTimes[3];
        set
        {
            _delayTimes[3] = value;
        }
    }

    public bool ShowAlways
    {
        get => _showAlways;
        set
        {
            if (_showAlways != value)
            {
                _showAlways = value;
            }
        }
    }

    public bool StripAmpersands
    {
        get => _stripAmpersands;
        set
        {
            if (_stripAmpersands != value)
            {
                _stripAmpersands = value;
            }
        }
    }

    public object? Tag
    {
        get;
        set;
    }

    public ToolTipIcon ToolTipIcon
    {
        get => _toolTipIcon;
        set
        {
            if (_toolTipIcon != value)
            {
                _toolTipIcon = value;
            }
        }
    }

    public string ToolTipTitle
    {
        get => _toolTipTitle;
        set
        {
            value ??= string.Empty;

            if (_toolTipTitle != value)
            {
                _toolTipTitle = value;
            }
        }
    }

    public bool UseAnimation
    {
        get => _useAnimation;
        set
        {
            if (_useAnimation != value)
            {
                _useAnimation = value;
            }
        }
    }

    public bool UseFading
    {
        get => _useFading;
        set
        {
            if (_useFading != value)
            {
                _useFading = value;
            }
        }
    }

    public bool CanExtend(object target) => target is Control;

    public string? GetToolTip(Control? control)
    {
        if (control is null)
        {
            return string.Empty;
        }

        return _tools.TryGetValue(control, out TipInfo? tipInfo)
            ? tipInfo.Caption
            : string.Empty;
    }

    public void Hide(IWin32Window win)
    {
        //if (win is Control tool)
        //{
        //    if (_tools.ContainsKey(tool))
        //    {
        //    }
        //}
    }

    public void RemoveAll()
    {
        Control[] controls = _tools.Keys.ToArray();
        foreach (Control control in controls)
        {
            control.Widget!.TooltipText = string.Empty;
            control.Widget.HasTooltip = false;
        }
        _tools.Clear();
        _topLevelControl = null;
    }

    public void SetToolTip(Control control, string? caption)
    {
        if (control is null)
        {
            throw new ArgumentNullException(nameof(control));
        }

        // Dictionary.Add threw when a control was given a second caption, and a null caption used to be
        // stored rather than clearing the tooltip.
        var known = _tools.ContainsKey(control);
        if (string.IsNullOrEmpty(caption))
        {
            if (known)
            {
                _tools.Remove(control);
            }

            if (control.Widget != null)
            {
                control.Widget.TooltipText = string.Empty;
                control.Widget.HasTooltip = false;
            }

            return;
        }

        _tools[control] = new TipInfo(caption, TipInfo.Type.Auto);
        if (control.Widget != null)
        {
            control.Widget.TooltipText = caption;
            control.Widget.HasTooltip = true;
            if (!known)
            {
                control.Widget.QueryTooltip += Widget_QueryTooltip;
            }
        }
    }

    public void Show(string? text, IWin32Window window)
    {
        ShowTooltip(text, window, 0);
    }

    public void Show(string? text, IWin32Window window, int duration)
    {
        ShowTooltip(text, window, duration);
    }

    public void Show(string? text, IWin32Window window, Point point)
    {
        ShowTooltip(text, window, 0);
    }

    public void Show(string? text, IWin32Window window, Point point, int duration)
    {
        ShowTooltip(text, window, duration);
    }

    public void Show(string? text, IWin32Window window, int x, int y)
    {
        ShowTooltip(text, window, 0);
    }

    public void Show(string? text, IWin32Window window, int x, int y, int duration)
    {
        ShowTooltip(text, window, duration);
    }

    public override string ToString()
    {
        string s = base.ToString();

        return $"{s} InitialDelay: {InitialDelay}, ShowAlways: {ShowAlways}";
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _isDisposing = true;
            try
            {
                RemoveAll();
            }
            finally
            {
                _isDisposing = false;
            }
        }

        base.Dispose(disposing);
    }

    /// <summary>
    /// Raises the Draw event.
    /// </summary>
    protected virtual void OnDraw(DrawToolTipEventArgs e)
    {
        Draw?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the Popup event.
    /// </summary>
    protected virtual void OnPopup(PopupEventArgs e)
    {
        Popup?.Invoke(this, e);
    }

    private void ShowTooltip(string? text, IWin32Window window, int duration)
    {
        if (window is Control form && form.Widget?.IsFocus == true)
        {
            foreach (var tip in _tools)
            {
                tip.Key.Widget?.TriggerTooltipQuery();
            }
        }
    }

    private void TimerHandler(object? source, EventArgs args)
    {
        Hide(((ToolTipTimer)source!).Host);
    }

    private void Widget_QueryTooltip(object o, QueryTooltipArgs args)
    {
        foreach (var tip in _tools)
        {
            if (IsBalloon == false)
            {
                var tooltiptitle = ToolTipTitle;
                if (!string.IsNullOrEmpty(tooltiptitle))
                {
                    if (ToolTipIcon == ToolTipIcon.Info)
                        tooltiptitle = $"<span foreground=\"#0069ee\" size=\"large\">🔔 {tooltiptitle}</span>{Environment.NewLine}";
                    else if (ToolTipIcon == ToolTipIcon.Warning)
                        tooltiptitle = $"<span foreground=\"#e0a500\" size=\"large\">🔺 {tooltiptitle}</span>{Environment.NewLine}";
                    else if (ToolTipIcon == ToolTipIcon.Error)
                        tooltiptitle = $"<span foreground=\"#ff0000\" size=\"large\">🚫 {tooltiptitle}</span>{Environment.NewLine}";
                    else
                        tooltiptitle = $"<span foreground=\"#0069ee\" size=\"large\">{tooltiptitle}</span>{Environment.NewLine}";
                }
                args.Tooltip.Markup = $"{tooltiptitle}<span foreground=\"#555555\" >{tip.Value.Caption}</span>";
                args.RetVal = true;
            }
            else
            {
                var tooltiptitle = ToolTipTitle;
                if (!string.IsNullOrEmpty(tooltiptitle))
                {
                    if (ToolTipIcon == ToolTipIcon.Info)
                    {
                        args.Tooltip.Icon = new Gdk.Texture(new Gdk.Pixbuf(AssemblyResources.CurrentAssembly, AssemblyResources.ToSystemUri("dialog-question.png"))); // Gtk4: Tooltip.Icon is IPaintable
                        tooltiptitle = $"<span foreground=\"#0069ee\" size=\"large\">{tooltiptitle}</span>{Environment.NewLine}";
                    }
                    else if (ToolTipIcon == ToolTipIcon.Warning)
                    {
                        args.Tooltip.Icon = new Gdk.Texture(new Gdk.Pixbuf(AssemblyResources.CurrentAssembly, AssemblyResources.ToSystemUri("dialog-warning.png"))); // Gtk4: Tooltip.Icon is IPaintable
                        tooltiptitle = $"<span foreground=\"#e0a500\" size=\"large\">{tooltiptitle}</span>{Environment.NewLine}";
                    }
                    else if (ToolTipIcon == ToolTipIcon.Error)
                    {
                        args.Tooltip.Icon = new Gdk.Texture(new Gdk.Pixbuf(AssemblyResources.CurrentAssembly, AssemblyResources.ToSystemUri("dialog-error.png"))); // Gtk4: Tooltip.Icon is IPaintable
                        tooltiptitle = $"<span foreground=\"#ff0000\" size=\"large\">{tooltiptitle}</span>{Environment.NewLine}";
                    }
                    else
                        tooltiptitle = $"<span foreground=\"#0069ee\" size=\"large\">{tooltiptitle}</span>{Environment.NewLine}";
                }
                args.Tooltip.Markup = $"{tooltiptitle}<span foreground=\"#555555\" >{tip.Value.Caption}</span>";
                args.RetVal = true;
            }
        }
    }
}