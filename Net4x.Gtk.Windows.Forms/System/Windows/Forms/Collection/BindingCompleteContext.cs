namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies the direction of the binding operation.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public enum BindingCompleteContext
    {
        /// <summary>
        /// An indication that the control property value is being updated from the data source.
        /// </summary>
        ControlUpdate,
        /// <summary>
        /// An indication that the data source value is being updated from the control property.
        /// </summary>
        DataSourceUpdate
    }
}
