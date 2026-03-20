/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
/// Specifies the base class used for displaying dialog boxes on the screen.
/// </summary>
[ToolboxItemFilter("System.Windows.Forms")]
public abstract class CommonDialog : Component
{

    private static readonly object s_helpRequestEvent = new object();

    /// <summary>
    /// Occurs when help request.
    /// </summary>
    public event EventHandler HelpRequest
    {
        add => Events.AddHandler(s_helpRequestEvent, value);
        remove => Events.RemoveHandler(s_helpRequestEvent, value);
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.CommonDialog" /> class.
    /// </summary>
    public CommonDialog()
    {

    }

    /// <summary>
    /// Gets or sets the tag.
    /// </summary>
    /// <value>
    /// The tag.
    /// </value>
    public object Tag
    {
        get;
        set;
    }

    /// <summary>
    /// Resets this instance.
    /// </summary>
    public abstract void Reset();

    /// <summary>
    /// Shows the dialog.
    /// </summary>
    /// <returns>
    /// DialogResult.
    /// </returns>
    public virtual DialogResult ShowDialog() => ShowDialog(owner: null);

    /// <summary>
    /// Shows the dialog.
    /// </summary>
    /// <param name="owner">
    /// The owner.
    /// </param>
    /// <returns>
    /// DialogResult.
    /// </returns>
    public virtual DialogResult ShowDialog(IWin32Window owner)
    {
        DialogResult result = DialogResult.Cancel;
        var runresult = RunDialog(owner);
        if (runresult)
        {
            result = DialogResult.OK;
        }

        return result;
    }

    /// <summary>
    /// Handles the
    /// <see cref="T:System.Windows.Forms.CommonDialog.HelpRequest" /> event.
    /// </summary>
    /// <param name="e">
    /// The
    /// <see cref="T:System.EventArgs" /> instance containing the event data.
    /// </param>
    protected virtual void OnHelpRequest(EventArgs e)
    {
        var handler = (EventHandler)Events[s_helpRequestEvent];
        handler?.Invoke(this, e);
    }

    /// <summary>
    /// Runs the dialog.
    /// </summary>
    /// <param name="owner">
    /// The owner.
    /// </param>
    /// <returns>
    /// <c>
    /// true
    /// </c> if XXXX,
    /// <c>
    /// false
    /// </c> otherwise.
    /// </returns>
    protected abstract bool RunDialog(IWin32Window owner);

}