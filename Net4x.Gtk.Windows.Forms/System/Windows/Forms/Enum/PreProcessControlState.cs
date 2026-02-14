namespace System.Windows.Forms
{
    //
    // Summary:
    //     Provides options that specify the relationship between the control and preprocessing
    //     messages.
    public enum PreProcessControlState
    {
        //
        // Summary:
        //     Specifies that the message has been processed and no further processing is required.
        MessageProcessed = 0,
        //
        // Summary:
        //     Specifies that the control requires the message and that processing should continue.
        MessageNeeded = 1,
        //
        // Summary:
        //     Specifies that the control does not require the message.
        MessageNotNeeded = 2
    }
}