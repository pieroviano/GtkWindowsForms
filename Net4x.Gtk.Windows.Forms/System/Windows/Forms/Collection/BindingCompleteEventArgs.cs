using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace System.Windows.Forms
{
	public class BindingCompleteEventArgs : CancelEventArgs
	{
		public Binding Binding
		{
			[CompilerGenerated]
			get
			{
				throw new System.NotImplementedException();
			}
		}

		public BindingCompleteState BindingCompleteState
		{
			[CompilerGenerated]
			get
			{
				throw new System.NotImplementedException();
			}
		}

		public BindingCompleteContext BindingCompleteContext
		{
			[CompilerGenerated]
			get
			{
				throw new System.NotImplementedException();
			}
		}

		public string ErrorText
		{
			[CompilerGenerated]
			get
			{
				throw new System.NotImplementedException();
			}
		}

		public Exception Exception
		{
			[CompilerGenerated]
			get
			{
				throw new System.NotImplementedException();
			}
		}

		public BindingCompleteEventArgs(Binding binding, BindingCompleteState state, BindingCompleteContext context, string errorText, Exception exception, bool cancel)
		{
			throw new System.NotImplementedException();
		}

		public BindingCompleteEventArgs(Binding binding, BindingCompleteState state, BindingCompleteContext context, string errorText, Exception exception)
		{
			throw new System.NotImplementedException();
		}

		public BindingCompleteEventArgs(Binding binding, BindingCompleteState state, BindingCompleteContext context, string errorText)
		{
			throw new System.NotImplementedException();
		}

		public BindingCompleteEventArgs(Binding binding, BindingCompleteState state, BindingCompleteContext context)
		{
			throw new System.NotImplementedException();
		}
	}
}