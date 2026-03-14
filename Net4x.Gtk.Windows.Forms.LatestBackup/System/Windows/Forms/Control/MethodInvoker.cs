namespace System.Windows.Forms;

/// <summary>
/// MethodInvoker is a delegate that represents a method with no parameters and no return value. It is commonly used in Windows Forms for invoking methods on the UI thread, especially when using the Control.Invoke method to execute code on the thread that owns the control's underlying window handle.
/// </summary>
public delegate void MethodInvoker();