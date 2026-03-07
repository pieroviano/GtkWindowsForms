namespace System.Windows.Forms;
/// <summary>
/// Displays a message box that can contain text, buttons, and symbols that inform and instruct the user.
/// </summary>
/// <filterpriority>
/// 1
/// </filterpriority>
public class MessageBox
{
    internal static IMessageBox Instance { get; set; } = new NonStaticMessageBox();

    public static IMessageBox ReplaceInstance(int timeout = -1)
    {
        Instance = timeout <= 0 ? new NonStaticMessageBox() : AutoClosingMessageBox.Instance;
        if (timeout > 0)
        {
            AutoClosingMessageBox.Instance.MessageBoxTimeout = timeout;
        }

        return Instance;
    }

    /// <summary>
    /// Displays a message box with the specified text, caption, buttons, icon, default button, options, and Help button.
    /// </summary>
    /// <returns>One of the
    /// <see cref="System.Windows.Forms.DialogResult" /> values.
    /// </returns>
    /// <param name="text">The text to display in the message box.
    /// </param>
    /// <param name="caption">The text to display in the title bar of the message box.
    /// </param>
    /// <param name="buttons">One of the
    /// <see cref="System.Windows.Forms.MessageBoxButtons" /> values that specifies which buttons to display in the message box.
    /// </param>
    /// <param name="icon">One of the
    /// <see cref="System.Windows.Forms.MessageBoxIcon" /> values that specifies which icon to display in the message box.
    /// </param>
    /// <param name="defaultButton">One of the
    /// <see cref="System.Windows.Forms.MessageBoxDefaultButton" /> values that specifies the default button for the message box.
    /// </param>
    /// <param name="options">One of the
    /// <see cref="System.Windows.Forms.MessageBoxOptions" /> values that specifies which display and association options will be used for the message box. You may pass in 0 if you wish to use the defaults.
    /// </param>
    /// <param name="displayHelpButton">true to show the Help button; otherwise, false. The default is false.
    /// </param>
    /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">
    /// <paramref name="buttons" /> is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxButtons" />-or-
    /// <paramref name="icon" /> is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxIcon" />-or- The
    /// <paramref name="defaultButton" /> specified is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxDefaultButton" />
    /// </exception>
    /// <exception cref="System.InvalidOperationException">An attempt was made to display the
    /// <see cref="System.Windows.Forms.MessageBox" /> in a process that is not running in User Interactive mode. This is specified by the
    /// <see cref="System.Windows.Forms.SystemInformation.UserInteractive" /> property.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    /// <paramref name="options" /> specified both
    /// <see cref="System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly" /> and
    /// <see cref="System.Windows.Forms.MessageBoxOptions.ServiceNotification" />-or-
    /// <paramref name="buttons" /> specified an invalid combination of
    /// <see cref="System.Windows.Forms.MessageBoxButtons" />
    /// </exception>
    /// <filterpriority>1
    /// </filterpriority>
    /// <PermissionSet>
    /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
    /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// </PermissionSet>
    public static DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton, MessageBoxOptions options, bool displayHelpButton)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(null, text, caption, buttons, icon);
        }

        return Instance.Show(null, text, caption, buttons, icon);
    }

    /// <summary>
    /// Displays a message box with the specified text, caption, buttons, icon, default button, options, and Help button, using the specified Help file.
    /// </summary>
    /// <returns>One of the
    /// <see cref="System.Windows.Forms.DialogResult" /> values.
    /// </returns>
    /// <param name="text">The text to display in the message box.
    /// </param>
    /// <param name="caption">The text to display in the title bar of the message box.
    /// </param>
    /// <param name="buttons">One of the
    /// <see cref="System.Windows.Forms.MessageBoxButtons" /> values that specifies which buttons to display in the message box.
    /// </param>
    /// <param name="icon">One of the
    /// <see cref="System.Windows.Forms.MessageBoxIcon" /> values that specifies which icon to display in the message box.
    /// </param>
    /// <param name="defaultButton">One of the
    /// <see cref="System.Windows.Forms.MessageBoxDefaultButton" /> values that specifies the default button for the message box.
    /// </param>
    /// <param name="options">One of the
    /// <see cref="System.Windows.Forms.MessageBoxOptions" /> values that specifies which display and association options will be used for the message box. You may pass in 0 if you wish to use the defaults.
    /// </param>
    /// <param name="helpFilePath">The path and name of the Help file to display when the user clicks the Help button.
    /// </param>
    /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">
    /// <paramref name="buttons" /> is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxButtons" />-or-
    /// <paramref name="icon" /> is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxIcon" />-or- The
    /// <paramref name="defaultButton" /> specified is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxDefaultButton" />
    /// </exception>
    /// <exception cref="System.InvalidOperationException">An attempt was made to display the
    /// <see cref="System.Windows.Forms.MessageBox" /> in a process that is not running in User Interactive mode. This is specified by the
    /// <see cref="System.Windows.Forms.SystemInformation.UserInteractive" /> property.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    /// <paramref name="options" /> specified both
    /// <see cref="System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly" /> and
    /// <see cref="System.Windows.Forms.MessageBoxOptions.ServiceNotification" />-or-
    /// <paramref name="buttons" /> specified an invalid combination of
    /// <see cref="System.Windows.Forms.MessageBoxButtons" />
    /// </exception>
    /// <filterpriority>1
    /// </filterpriority>
    /// <PermissionSet>
    /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
    /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// </PermissionSet>
    public static DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(null, text, caption, buttons, icon);
        }

        return Instance.Show(null, text, caption, buttons, icon);
    }

    /// <summary>
    /// Displays a message box with the specified text, caption, buttons, icon, default button, options, and Help button, using the specified Help file.
    /// </summary>
    /// <returns>One of the
    /// <see cref="System.Windows.Forms.DialogResult" /> values.
    /// </returns>
    /// <param name="owner">An implementation of
    /// <see cref="System.Windows.Forms.IWin32Window" /> that will own the modal dialog box.
    /// </param>
    /// <param name="text">The text to display in the message box.
    /// </param>
    /// <param name="caption">The text to display in the title bar of the message box.
    /// </param>
    /// <param name="buttons">One of the
    /// <see cref="System.Windows.Forms.MessageBoxButtons" /> values that specifies which buttons to display in the message box.
    /// </param>
    /// <param name="icon">One of the
    /// <see cref="System.Windows.Forms.MessageBoxIcon" /> values that specifies which icon to display in the message box.
    /// </param>
    /// <param name="defaultButton">One of the
    /// <see cref="System.Windows.Forms.MessageBoxDefaultButton" /> values that specifies the default button for the message box.
    /// </param>
    /// <param name="options">One of the
    /// <see cref="System.Windows.Forms.MessageBoxOptions" /> values that specifies which display and association options will be used for the message box. You may pass in 0 if you wish to use the defaults.
    /// </param>
    /// <param name="helpFilePath">The path and name of the Help file to display when the user clicks the Help button.
    /// </param>
    /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">
    /// <paramref name="buttons" /> is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxButtons" />-or-
    /// <paramref name="icon" /> is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxIcon" />-or- The
    /// <paramref name="defaultButton" /> specified is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxDefaultButton" />
    /// </exception>
    /// <exception cref="System.InvalidOperationException">An attempt was made to display the
    /// <see cref="System.Windows.Forms.MessageBox" /> in a process that is not running in User Interactive mode. This is specified by the
    /// <see cref="System.Windows.Forms.SystemInformation.UserInteractive" /> property.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    /// <paramref name="options" /> specified both
    /// <see cref="System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly" /> and
    /// <see cref="System.Windows.Forms.MessageBoxOptions.ServiceNotification" />-or-
    /// <paramref name="buttons" /> specified an invalid combination of
    /// <see cref="System.Windows.Forms.MessageBoxButtons" />
    /// </exception>
    /// <filterpriority>1
    /// </filterpriority>
    /// <PermissionSet>
    /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
    /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// </PermissionSet>
    public static DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons,
            MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(owner, text, caption, buttons, icon);
        }

        return Instance.Show(owner, text, caption, buttons, icon);
    }

    /// <summary>
    /// Displays a message box with the specified text, caption, buttons, icon, default button, options, and Help button, using the specified Help file and Help keyword.
    /// </summary>
    /// <returns>One of the
    /// <see cref="System.Windows.Forms.DialogResult" /> values.
    /// </returns>
    /// <param name="text">The text to display in the message box.
    /// </param>
    /// <param name="caption">The text to display in the title bar of the message box.
    /// </param>
    /// <param name="buttons">One of the
    /// <see cref="System.Windows.Forms.MessageBoxButtons" /> values that specifies which buttons to display in the message box.
    /// </param>
    /// <param name="icon">One of the
    /// <see cref="System.Windows.Forms.MessageBoxIcon" /> values that specifies which icon to display in the message box.
    /// </param>
    /// <param name="defaultButton">One of the
    /// <see cref="System.Windows.Forms.MessageBoxDefaultButton" /> values that specifies the default button for the message box.
    /// </param>
    /// <param name="options">One of the
    /// <see cref="System.Windows.Forms.MessageBoxOptions" /> values that specifies which display and association options will be used for the message box. You may pass in 0 if you wish to use the defaults.
    /// </param>
    /// <param name="helpFilePath">The path and name of the Help file to display when the user clicks the Help button.
    /// </param>
    /// <param name="keyword">The Help keyword to display when the user clicks the Help button.
    /// </param>
    /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">
    /// <paramref name="buttons" /> is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxButtons" />-or-
    /// <paramref name="icon" /> is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxIcon" />-or- The
    /// <paramref name="defaultButton" /> specified is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxDefaultButton" />
    /// </exception>
    /// <exception cref="System.InvalidOperationException">An attempt was made to display the
    /// <see cref="System.Windows.Forms.MessageBox" /> in a process that is not running in User Interactive mode. This is specified by the
    /// <see cref="System.Windows.Forms.SystemInformation.UserInteractive" /> property.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    /// <paramref name="options" /> specified both
    /// <see cref="System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly" /> and
    /// <see cref="System.Windows.Forms.MessageBoxOptions.ServiceNotification" />-or-
    /// <paramref name="buttons" /> specified an invalid combination of
    /// <see cref="System.Windows.Forms.MessageBoxButtons" />
    /// </exception>
    /// <filterpriority>1
    /// </filterpriority>
    /// <PermissionSet>
    /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
    /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// </PermissionSet>
    public static DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, string keyword)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(null, text, caption, buttons, icon);
        }

        return Instance.Show(null, text, caption, buttons, icon);
    }

    /// <summary>
    /// Displays a message box with the specified text, caption, buttons, icon, default button, options, and Help button, using the specified Help file and Help keyword.
    /// </summary>
    /// <returns>One of the
    /// <see cref="System.Windows.Forms.DialogResult" /> values.
    /// </returns>
    /// <param name="owner">An implementation of
    /// <see cref="System.Windows.Forms.IWin32Window" /> that will own the modal dialog box.
    /// </param>
    /// <param name="text">The text to display in the message box.
    /// </param>
    /// <param name="caption">The text to display in the title bar of the message box.
    /// </param>
    /// <param name="buttons">One of the
    /// <see cref="System.Windows.Forms.MessageBoxButtons" /> values that specifies which buttons to display in the message box.
    /// </param>
    /// <param name="icon">One of the
    /// <see cref="System.Windows.Forms.MessageBoxIcon" /> values that specifies which icon to display in the message box.
    /// </param>
    /// <param name="defaultButton">One of the
    /// <see cref="System.Windows.Forms.MessageBoxDefaultButton" /> values that specifies the default button for the message box.
    /// </param>
    /// <param name="options">One of the
    /// <see cref="System.Windows.Forms.MessageBoxOptions" /> values that specifies which display and association options will be used for the message box. You may pass in 0 if you wish to use the defaults.
    /// </param>
    /// <param name="helpFilePath">The path and name of the Help file to display when the user clicks the Help button.
    /// </param>
    /// <param name="keyword">The Help keyword to display when the user clicks the Help button.
    /// </param>
    /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">
    /// <paramref name="buttons" /> is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxButtons" />-or-
    /// <paramref name="icon" /> is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxIcon" />-or- The
    /// <paramref name="defaultButton" /> specified is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxDefaultButton" />
    /// </exception>
    /// <exception cref="System.InvalidOperationException">An attempt was made to display the
    /// <see cref="System.Windows.Forms.MessageBox" /> in a process that is not running in User Interactive mode. This is specified by the
    /// <see cref="System.Windows.Forms.SystemInformation.UserInteractive" /> property.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    /// <paramref name="options" /> specified both
    /// <see cref="System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly" /> and
    /// <see cref="System.Windows.Forms.MessageBoxOptions.ServiceNotification" />-or-
    /// <paramref name="buttons" /> specified an invalid combination of
    /// <see cref="System.Windows.Forms.MessageBoxButtons" />
    /// </exception>
    /// <filterpriority>1
    /// </filterpriority>
    /// <PermissionSet>
    /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
    /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// </PermissionSet>
    public static DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons,
            MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, string keyword)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(owner, text, caption, buttons, icon);
        }

        return Instance.Show(owner, text, caption, buttons, icon);
    }

    /// <summary>
    /// Displays a message box with the specified text, caption, buttons, icon, default button, options, and Help button, using the specified Help file and HelpNavigator.
    /// </summary>
    /// <returns>One of the
    /// <see cref="System.Windows.Forms.DialogResult" /> values.
    /// </returns>
    /// <param name="text">The text to display in the message box.
    /// </param>
    /// <param name="caption">The text to display in the title bar of the message box.
    /// </param>
    /// <param name="buttons">One of the
    /// <see cref="System.Windows.Forms.MessageBoxButtons" /> values that specifies which buttons to display in the message box.
    /// </param>
    /// <param name="icon">One of the
    /// <see cref="System.Windows.Forms.MessageBoxIcon" /> values that specifies which icon to display in the message box.
    /// </param>
    /// <param name="defaultButton">One of the
    /// <see cref="System.Windows.Forms.MessageBoxDefaultButton" /> values that specifies the default button for the message box.
    /// </param>
    /// <param name="options">One of the
    /// <see cref="System.Windows.Forms.MessageBoxOptions" /> values that specifies which display and association options will be used for the message box. You may pass in 0 if you wish to use the defaults.
    /// </param>
    /// <param name="helpFilePath">The path and name of the Help file to display when the user clicks the Help button.
    /// </param>
    /// <param name="navigator">One of the
    /// <see cref="System.Windows.Forms.HelpNavigator" /> values.
    /// </param>
    /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">
    /// <paramref name="buttons" /> is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxButtons" />-or-
    /// <paramref name="icon" /> is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxIcon" />-or- The
    /// <paramref name="defaultButton" /> specified is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxDefaultButton" />
    /// </exception>
    /// <exception cref="System.InvalidOperationException">An attempt was made to display the
    /// <see cref="System.Windows.Forms.MessageBox" /> in a process that is not running in User Interactive mode. This is specified by the
    /// <see cref="System.Windows.Forms.SystemInformation.UserInteractive" /> property.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    /// <paramref name="options" /> specified both
    /// <see cref="System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly" /> and
    /// <see cref="System.Windows.Forms.MessageBoxOptions.ServiceNotification" />-or-
    /// <paramref name="buttons" /> specified an invalid combination of
    /// <see cref="System.Windows.Forms.MessageBoxButtons" />
    /// </exception>
    /// <filterpriority>1
    /// </filterpriority>
    /// <PermissionSet>
    /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
    /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// </PermissionSet>
    public static DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(null, text, caption, buttons, icon);
        }

        return Instance.Show(null, text, caption, buttons, icon);
    }

    /// <summary>
    /// Displays a message box with the specified text, caption, buttons, icon, default button, options, and Help button, using the specified Help file and HelpNavigator.
    /// </summary>
    /// <returns>One of the
    /// <see cref="System.Windows.Forms.DialogResult" /> values.
    /// </returns>
    /// <param name="owner">An implementation of
    /// <see cref="System.Windows.Forms.IWin32Window" /> that will own the modal dialog box.
    /// </param>
    /// <param name="text">The text to display in the message box.
    /// </param>
    /// <param name="caption">The text to display in the title bar of the message box.
    /// </param>
    /// <param name="buttons">One of the
    /// <see cref="System.Windows.Forms.MessageBoxButtons" /> values that specifies which buttons to display in the message box.
    /// </param>
    /// <param name="icon">One of the
    /// <see cref="System.Windows.Forms.MessageBoxIcon" /> values that specifies which icon to display in the message box.
    /// </param>
    /// <param name="defaultButton">One of the
    /// <see cref="System.Windows.Forms.MessageBoxDefaultButton" /> values that specifies the default button for the message box.
    /// </param>
    /// <param name="options">One of the
    /// <see cref="System.Windows.Forms.MessageBoxOptions" /> values that specifies which display and association options will be used for the message box. You may pass in 0 if you wish to use the defaults.
    /// </param>
    /// <param name="helpFilePath">The path and name of the Help file to display when the user clicks the Help button.
    /// </param>
    /// <param name="navigator">One of the
    /// <see cref="System.Windows.Forms.HelpNavigator" /> values.
    /// </param>
    /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">
    /// <paramref name="buttons" /> is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxButtons" />-or-
    /// <paramref name="icon" /> is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxIcon" />-or- The
    /// <paramref name="defaultButton" /> specified is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxDefaultButton" />
    /// </exception>
    /// <exception cref="System.InvalidOperationException">An attempt was made to display the
    /// <see cref="System.Windows.Forms.MessageBox" /> in a process that is not running in User Interactive mode. This is specified by the
    /// <see cref="System.Windows.Forms.SystemInformation.UserInteractive" /> property.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    /// <paramref name="options" /> specified both
    /// <see cref="System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly" /> and
    /// <see cref="System.Windows.Forms.MessageBoxOptions.ServiceNotification" />-or-
    /// <paramref name="buttons" /> specified an invalid combination of
    /// <see cref="System.Windows.Forms.MessageBoxButtons" />
    /// </exception>
    /// <filterpriority>1
    /// </filterpriority>
    /// <PermissionSet>
    /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
    /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// </PermissionSet>
    public static DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons,
            MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(owner, text, caption, buttons, icon);
        }

        return Instance.Show(owner, text, caption, buttons, icon);
    }

    /// <summary>
    /// Displays a message box with the specified text, caption, buttons, icon, default button, options, and Help button, using the specified Help file, HelpNavigator, and Help topic.
    /// </summary>
    /// <returns>One of the
    /// <see cref="System.Windows.Forms.DialogResult" /> values.
    /// </returns>
    /// <param name="text">The text to display in the message box.
    /// </param>
    /// <param name="caption">The text to display in the title bar of the message box.
    /// </param>
    /// <param name="buttons">One of the
    /// <see cref="System.Windows.Forms.MessageBoxButtons" /> values that specifies which buttons to display in the message box.
    /// </param>
    /// <param name="icon">One of the
    /// <see cref="System.Windows.Forms.MessageBoxIcon" /> values that specifies which icon to display in the message box.
    /// </param>
    /// <param name="defaultButton">One of the
    /// <see cref="System.Windows.Forms.MessageBoxDefaultButton" /> values that specifies the default button for the message box.
    /// </param>
    /// <param name="options">One of the
    /// <see cref="System.Windows.Forms.MessageBoxOptions" /> values that specifies which display and association options will be used for the message box. You may pass in 0 if you wish to use the defaults.
    /// </param>
    /// <param name="helpFilePath">The path and name of the Help file to display when the user clicks the Help button.
    /// </param>
    /// <param name="navigator">One of the
    /// <see cref="System.Windows.Forms.HelpNavigator" /> values.
    /// </param>
    /// <param name="param">The numeric ID of the Help topic to display when the user clicks the Help button.
    /// </param>
    /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">
    /// <paramref name="buttons" /> is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxButtons" />-or-
    /// <paramref name="icon" /> is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxIcon" />-or- The
    /// <paramref name="defaultButton" /> specified is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxDefaultButton" />
    /// </exception>
    /// <exception cref="System.InvalidOperationException">An attempt was made to display the
    /// <see cref="System.Windows.Forms.MessageBox" /> in a process that is not running in User Interactive mode. This is specified by the
    /// <see cref="System.Windows.Forms.SystemInformation.UserInteractive" /> property.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    /// <paramref name="options" /> specified both
    /// <see cref="System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly" /> and
    /// <see cref="System.Windows.Forms.MessageBoxOptions.ServiceNotification" />-or-
    /// <paramref name="buttons" /> specified an invalid combination of
    /// <see cref="System.Windows.Forms.MessageBoxButtons" />
    /// </exception>
    /// <filterpriority>1
    /// </filterpriority>
    /// <PermissionSet>
    /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
    /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// </PermissionSet>
    public static DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator,
            object param)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(null, text, caption, buttons, icon);
        }

        return Instance.Show(null, text, caption, buttons, icon);
    }

    /// <summary>
    /// Displays a message box with the specified text, caption, buttons, icon, default button, options, and Help button, using the specified Help file, HelpNavigator, and Help topic.
    /// </summary>
    /// <returns>One of the
    /// <see cref="System.Windows.Forms.DialogResult" /> values.
    /// </returns>
    /// <param name="owner">An implementation of
    /// <see cref="System.Windows.Forms.IWin32Window" /> that will own the modal dialog box.
    /// </param>
    /// <param name="text">The text to display in the message box.
    /// </param>
    /// <param name="caption">The text to display in the title bar of the message box.
    /// </param>
    /// <param name="buttons">One of the
    /// <see cref="System.Windows.Forms.MessageBoxButtons" /> values that specifies which buttons to display in the message box.
    /// </param>
    /// <param name="icon">One of the
    /// <see cref="System.Windows.Forms.MessageBoxIcon" /> values that specifies which icon to display in the message box.
    /// </param>
    /// <param name="defaultButton">One of the
    /// <see cref="System.Windows.Forms.MessageBoxDefaultButton" /> values that specifies the default button for the message box.
    /// </param>
    /// <param name="options">One of the
    /// <see cref="System.Windows.Forms.MessageBoxOptions" /> values that specifies which display and association options will be used for the message box. You may pass in 0 if you wish to use the defaults.
    /// </param>
    /// <param name="helpFilePath">The path and name of the Help file to display when the user clicks the Help button.
    /// </param>
    /// <param name="navigator">One of the
    /// <see cref="System.Windows.Forms.HelpNavigator" /> values.
    /// </param>
    /// <param name="param">The numeric ID of the Help topic to display when the user clicks the Help button.
    /// </param>
    /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">
    /// <paramref name="buttons" /> is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxButtons" />-or-
    /// <paramref name="icon" /> is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxIcon" />-or- The
    /// <paramref name="defaultButton" /> specified is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxDefaultButton" />
    /// </exception>
    /// <exception cref="System.InvalidOperationException">An attempt was made to display the
    /// <see cref="System.Windows.Forms.MessageBox" /> in a process that is not running in User Interactive mode. This is specified by the
    /// <see cref="System.Windows.Forms.SystemInformation.UserInteractive" /> property.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    /// <paramref name="options" /> specified both
    /// <see cref="System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly" /> and
    /// <see cref="System.Windows.Forms.MessageBoxOptions.ServiceNotification" />-or-
    /// <paramref name="buttons" /> specified an invalid combination of
    /// <see cref="System.Windows.Forms.MessageBoxButtons" />
    /// </exception>
    /// <filterpriority>1
    /// </filterpriority>
    /// <PermissionSet>
    /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
    /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// </PermissionSet>
    public static DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons,
            MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator,
            object param)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(owner, text, caption, buttons, icon);
        }

        return Instance.Show(owner, text, caption, buttons, icon);
    }

    /// <summary>
    /// Displays a message box with the specified text, caption, buttons, icon, default button, and options.
    /// </summary>
    /// <returns>One of the
    /// <see cref="System.Windows.Forms.DialogResult" /> values.
    /// </returns>
    /// <param name="text">The text to display in the message box.
    /// </param>
    /// <param name="caption">The text to display in the title bar of the message box.
    /// </param>
    /// <param name="buttons">One of the
    /// <see cref="System.Windows.Forms.MessageBoxButtons" /> values that specifies which buttons to display in the message box.
    /// </param>
    /// <param name="icon">One of the
    /// <see cref="System.Windows.Forms.MessageBoxIcon" /> values that specifies which icon to display in the message box.
    /// </param>
    /// <param name="defaultButton">One of the
    /// <see cref="System.Windows.Forms.MessageBoxDefaultButton" /> values that specifies the default button for the message box.
    /// </param>
    /// <param name="options">One of the
    /// <see cref="System.Windows.Forms.MessageBoxOptions" /> values that specifies which display and association options will be used for the message box. You may pass in 0 if you wish to use the defaults.
    /// </param>
    /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">
    /// <paramref name="buttons" /> is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxButtons" />-or-
    /// <paramref name="icon" /> is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxIcon" />-or- The
    /// <paramref name="defaultButton" /> specified is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxDefaultButton" />
    /// </exception>
    /// <exception cref="System.InvalidOperationException">An attempt was made to display the
    /// <see cref="System.Windows.Forms.MessageBox" /> in a process that is not running in User Interactive mode. This is specified by the
    /// <see cref="System.Windows.Forms.SystemInformation.UserInteractive" /> property.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    /// <paramref name="options" /> specified both
    /// <see cref="System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly" /> and
    /// <see cref="System.Windows.Forms.MessageBoxOptions.ServiceNotification" />-or-
    /// <paramref name="buttons" /> specified an invalid combination of
    /// <see cref="System.Windows.Forms.MessageBoxButtons" />
    /// </exception>
    /// <filterpriority>1
    /// </filterpriority>
    /// <PermissionSet>
    /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
    /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// </PermissionSet>
    public static DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(null, text, caption, buttons, icon);
        }

        return Instance.Show(null, text, caption, buttons, icon);
    }

    /// <summary>
    /// Displays a message box with the specified text, caption, buttons, icon, and default button.
    /// </summary>
    /// <returns>One of the
    /// <see cref="System.Windows.Forms.DialogResult" /> values.
    /// </returns>
    /// <param name="text">The text to display in the message box.
    /// </param>
    /// <param name="caption">The text to display in the title bar of the message box.
    /// </param>
    /// <param name="buttons">One of the
    /// <see cref="System.Windows.Forms.MessageBoxButtons" /> values that specifies which buttons to display in the message box.
    /// </param>
    /// <param name="icon">One of the
    /// <see cref="System.Windows.Forms.MessageBoxIcon" /> values that specifies which icon to display in the message box.
    /// </param>
    /// <param name="defaultButton">One of the
    /// <see cref="System.Windows.Forms.MessageBoxDefaultButton" /> values that specifies the default button for the message box.
    /// </param>
    /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">
    /// <paramref name="buttons" /> is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxButtons" />-or-
    /// <paramref name="icon" /> is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxIcon" />-or-
    /// <paramref name="defaultButton" /> is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxDefaultButton" />
    /// </exception>
    /// <exception cref="System.InvalidOperationException">An attempt was made to display the
    /// <see cref="System.Windows.Forms.MessageBox" /> in a process that is not running in User Interactive mode. This is specified by the
    /// <see cref="System.Windows.Forms.SystemInformation.UserInteractive" /> property.
    /// </exception>
    /// <filterpriority>1
    /// </filterpriority>
    /// <PermissionSet>
    /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
    /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// </PermissionSet>
    public static DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(null, text, caption, buttons, icon);
        }

        return Instance.Show(null, text, caption, buttons, icon);
    }

    /// <summary>
    /// Displays a message box with specified text, caption, buttons, and icon.
    /// </summary>
    /// <returns>One of the
    /// <see cref="System.Windows.Forms.DialogResult" /> values.
    /// </returns>
    /// <param name="text">The text to display in the message box.
    /// </param>
    /// <param name="caption">The text to display in the title bar of the message box.
    /// </param>
    /// <param name="buttons">One of the
    /// <see cref="System.Windows.Forms.MessageBoxButtons" /> values that specifies which buttons to display in the message box.
    /// </param>
    /// <param name="icon">One of the
    /// <see cref="System.Windows.Forms.MessageBoxIcon" /> values that specifies which icon to display in the message box.
    /// </param>
    /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The
    /// <paramref name="buttons" /> parameter specified is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxButtons" />-or- The
    /// <paramref name="icon" /> parameter specified is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxIcon" />
    /// </exception>
    /// <exception cref="System.InvalidOperationException">An attempt was made to display the
    /// <see cref="System.Windows.Forms.MessageBox" /> in a process that is not running in User Interactive mode. This is specified by the
    /// <see cref="System.Windows.Forms.SystemInformation.UserInteractive" /> property.
    /// </exception>
    /// <filterpriority>1
    /// </filterpriority>
    /// <PermissionSet>
    /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
    /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// </PermissionSet>
    public static DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(null, text, caption, buttons, icon);
        }

        return Instance.Show(null, text, caption, buttons, icon);
    }

    /// <summary>
    /// Displays a message box with specified text, caption, and buttons.
    /// </summary>
    /// <returns>One of the
    /// <see cref="System.Windows.Forms.DialogResult" /> values.
    /// </returns>
    /// <param name="text">The text to display in the message box.
    /// </param>
    /// <param name="caption">The text to display in the title bar of the message box.
    /// </param>
    /// <param name="buttons">One of the
    /// <see cref="System.Windows.Forms.MessageBoxButtons" /> values that specifies which buttons to display in the message box.
    /// </param>
    /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The
    /// <paramref name="buttons" /> parameter specified is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxButtons" />
    /// </exception>
    /// <exception cref="System.InvalidOperationException">An attempt was made to display the
    /// <see cref="System.Windows.Forms.MessageBox" /> in a process that is not running in User Interactive mode. This is specified by the
    /// <see cref="System.Windows.Forms.SystemInformation.UserInteractive" /> property.
    /// </exception>
    /// <filterpriority>1
    /// </filterpriority>
    /// <PermissionSet>
    /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
    /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// </PermissionSet>
    public static DialogResult Show(string? text, string caption, MessageBoxButtons buttons)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(null, text, caption, buttons, MessageBoxIcon.None);
        }

        return Instance.Show(null, text, caption, buttons, MessageBoxIcon.None);
    }

    /// <summary>
    /// Displays a message box with specified text and caption.
    /// </summary>
    /// <returns>One of the
    /// <see cref="System.Windows.Forms.DialogResult" /> values.
    /// </returns>
    /// <param name="text">The text to display in the message box.
    /// </param>
    /// <param name="caption">The text to display in the title bar of the message box.
    /// </param>
    /// <filterpriority>1
    /// </filterpriority>
    /// <PermissionSet>
    /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
    /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// </PermissionSet>
    public static DialogResult Show(string? text, string caption)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(null, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        return Instance.Show(null, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None);
    }

    /// <summary>
    /// Displays a message box with specified text.
    /// </summary>
    /// <returns>One of the
    /// <see cref="System.Windows.Forms.DialogResult" /> values.
    /// </returns>
    /// <param name="text">The text to display in the message box.
    /// </param>
    /// <filterpriority>1
    /// </filterpriority>
    /// <PermissionSet>
    /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
    /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// </PermissionSet>
    public static DialogResult Show(string? text)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(null, text, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        return Instance.Show(null, text, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None);
    }

    /// <summary>
    /// Displays a message box in front of the specified object and with the specified text, caption, buttons, icon, default button, and options.
    /// </summary>
    /// <returns>One of the
    /// <see cref="System.Windows.Forms.DialogResult" /> values.
    /// </returns>
    /// <param name="owner">An implementation of
    /// <see cref="System.Windows.Forms.IWin32Window" /> that will own the modal dialog box.
    /// </param>
    /// <param name="text">The text to display in the message box.
    /// </param>
    /// <param name="caption">The text to display in the title bar of the message box.
    /// </param>
    /// <param name="buttons">One of the
    /// <see cref="System.Windows.Forms.MessageBoxButtons" /> values that specifies which buttons to display in the message box.
    /// </param>
    /// <param name="icon">One of the
    /// <see cref="System.Windows.Forms.MessageBoxIcon" /> values that specifies which icon to display in the message box.
    /// </param>
    /// <param name="defaultButton">One of the
    /// <see cref="System.Windows.Forms.MessageBoxDefaultButton" /> values the specifies the default button for the message box.
    /// </param>
    /// <param name="options">One of the
    /// <see cref="System.Windows.Forms.MessageBoxOptions" /> values that specifies which display and association options will be used for the message box. You may pass in 0 if you wish to use the defaults.
    /// </param>
    /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">
    /// <paramref name="buttons" /> is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxButtons" />-or-
    /// <paramref name="icon" /> is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxIcon" />-or-
    /// <paramref name="defaultButton" /> is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxDefaultButton" />
    /// </exception>
    /// <exception cref="System.InvalidOperationException">An attempt was made to display the
    /// <see cref="System.Windows.Forms.MessageBox" /> in a process that is not running in User Interactive mode. This is specified by the
    /// <see cref="System.Windows.Forms.SystemInformation.UserInteractive" /> property.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    /// <paramref name="options" /> specified both
    /// <see cref="System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly" /> and
    /// <see cref="System.Windows.Forms.MessageBoxOptions.ServiceNotification" />-or-
    /// <paramref name="options" /> specified
    /// <see cref="System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly" /> or
    /// <see cref="System.Windows.Forms.MessageBoxOptions.ServiceNotification" /> and specified a value in the
    /// <paramref name="owner" /> parameter. These two options should be used only if you invoke the version of this method that does not take an
    /// <paramref name="owner" /> parameter.-or-
    /// <paramref name="buttons" /> specified an invalid combination of
    /// <see cref="System.Windows.Forms.MessageBoxButtons" />
    /// </exception>
    /// <filterpriority>1
    /// </filterpriority>
    /// <PermissionSet>
    /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
    /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// </PermissionSet>
    public static DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons,
            MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(owner, text, caption, buttons, icon);
        }

        return Instance.Show(owner, text, caption, buttons, icon);
    }

    /// <summary>
    /// Displays a message box in front of the specified object and with the specified text, caption, buttons, icon, and default button.
    /// </summary>
    /// <returns>One of the
    /// <see cref="System.Windows.Forms.DialogResult" /> values.
    /// </returns>
    /// <param name="owner">An implementation of
    /// <see cref="System.Windows.Forms.IWin32Window" /> that will own the modal dialog box.
    /// </param>
    /// <param name="text">The text to display in the message box.
    /// </param>
    /// <param name="caption">The text to display in the title bar of the message box.
    /// </param>
    /// <param name="buttons">One of the
    /// <see cref="System.Windows.Forms.MessageBoxButtons" /> values that specifies which buttons to display in the message box.
    /// </param>
    /// <param name="icon">One of the
    /// <see cref="System.Windows.Forms.MessageBoxIcon" /> values that specifies which icon to display in the message box.
    /// </param>
    /// <param name="defaultButton">One of the
    /// <see cref="System.Windows.Forms.MessageBoxDefaultButton" /> values that specifies the default button for the message box.
    /// </param>
    /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">
    /// <paramref name="buttons" /> is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxButtons" />-or-
    /// <paramref name="icon" /> is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxIcon" />-or-
    /// <paramref name="defaultButton" /> is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxDefaultButton" />
    /// </exception>
    /// <exception cref="System.InvalidOperationException">An attempt was made to display the
    /// <see cref="System.Windows.Forms.MessageBox" /> in a process that is not running in User Interactive mode. This is specified by the
    /// <see cref="System.Windows.Forms.SystemInformation.UserInteractive" /> property.
    /// </exception>
    /// <filterpriority>1
    /// </filterpriority>
    /// <PermissionSet>
    /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
    /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// </PermissionSet>
    public static DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons,
            MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(owner, text, caption, buttons, icon);
        }

        return Instance.Show(owner, text, caption, buttons, icon);
    }

    /// <summary>
    /// Displays a message box in front of the specified object and with the specified text, caption, buttons, and icon.
    /// </summary>
    /// <returns>One of the
    /// <see cref="System.Windows.Forms.DialogResult" /> values.
    /// </returns>
    /// <param name="owner">An implementation of
    /// <see cref="System.Windows.Forms.IWin32Window" /> that will own the modal dialog box.
    /// </param>
    /// <param name="text">The text to display in the message box.
    /// </param>
    /// <param name="caption">The text to display in the title bar of the message box.
    /// </param>
    /// <param name="buttons">One of the
    /// <see cref="System.Windows.Forms.MessageBoxButtons" /> values that specifies which buttons to display in the message box.
    /// </param>
    /// <param name="icon">One of the
    /// <see cref="System.Windows.Forms.MessageBoxIcon" /> values that specifies which icon to display in the message box.
    /// </param>
    /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">
    /// <paramref name="buttons" /> is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxButtons" />-or-
    /// <paramref name="icon" /> is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxIcon" />
    /// </exception>
    /// <exception cref="System.InvalidOperationException">An attempt was made to display the
    /// <see cref="System.Windows.Forms.MessageBox" /> in a process that is not running in User Interactive mode. This is specified by the
    /// <see cref="System.Windows.Forms.SystemInformation.UserInteractive" /> property.
    /// </exception>
    /// <filterpriority>1
    /// </filterpriority>
    /// <PermissionSet>
    /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
    /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// </PermissionSet>
    public static DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons,
            MessageBoxIcon icon)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(owner, text, caption, buttons, icon);
        }

        return Instance.Show(owner, text, caption, buttons, icon);
    }

    /// <summary>
    /// Displays a message box in front of the specified object and with the specified text, caption, and buttons.
    /// </summary>
    /// <returns>One of the
    /// <see cref="System.Windows.Forms.DialogResult" /> values.
    /// </returns>
    /// <param name="owner">An implementation of
    /// <see cref="System.Windows.Forms.IWin32Window" /> that will own the modal dialog box.
    /// </param>
    /// <param name="text">The text to display in the message box.
    /// </param>
    /// <param name="caption">The text to display in the title bar of the message box.
    /// </param>
    /// <param name="buttons">One of the
    /// <see cref="System.Windows.Forms.MessageBoxButtons" /> values that specifies which buttons to display in the message box.
    /// </param>
    /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">
    /// <paramref name="buttons" /> is not a member of
    /// <see cref="System.Windows.Forms.MessageBoxButtons" />
    /// </exception>
    /// <exception cref="System.InvalidOperationException">An attempt was made to display the
    /// <see cref="System.Windows.Forms.MessageBox" /> in a process that is not running in User Interactive mode. This is specified by the
    /// <see cref="System.Windows.Forms.SystemInformation.UserInteractive" /> property.
    /// </exception>
    /// <filterpriority>1
    /// </filterpriority>
    /// <PermissionSet>
    /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
    /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// </PermissionSet>
    public static DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(owner, text, caption, buttons, MessageBoxIcon.None);
        }

        return Instance.Show(owner, text, caption, buttons, MessageBoxIcon.None);
    }

    /// <summary>
    /// Displays a message box in front of the specified object and with the specified text and caption.
    /// </summary>
    /// <returns>One of the
    /// <see cref="System.Windows.Forms.DialogResult" /> values.
    /// </returns>
    /// <param name="owner">An implementation of
    /// <see cref="System.Windows.Forms.IWin32Window" /> that will own the modal dialog box.
    /// </param>
    /// <param name="text">The text to display in the message box.
    /// </param>
    /// <param name="caption">The text to display in the title bar of the message box.
    /// </param>
    /// <filterpriority>1
    /// </filterpriority>
    /// <PermissionSet>
    /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
    /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// </PermissionSet>
    public static DialogResult Show(IWin32Window? owner, string? text, string caption)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        return Instance.Show(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None);
    }

    /// <summary>
    /// Displays a message box in front of the specified object and with the specified text.
    /// </summary>
    /// <returns>One of the
    /// <see cref="System.Windows.Forms.DialogResult" /> values.
    /// </returns>
    /// <param name="owner">An implementation of
    /// <see cref="System.Windows.Forms.IWin32Window" /> that will own the modal dialog box.
    /// </param>
    /// <param name="text">The text to display in the message box.
    /// </param>
    /// <filterpriority>1
    /// </filterpriority>
    /// <PermissionSet>
    /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
    /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// </PermissionSet>
    public static DialogResult Show(IWin32Window? owner, string? text)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(owner, text, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        return Instance.Show(owner, text, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None);
    }

}
