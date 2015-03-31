using System;

namespace OmTV
{
	public static class CommonEvents
	{
		public static event EventHandler<StringEventArgs> OnMessage;
		public static event EventHandler OnLoadStarted;
		public static event EventHandler OnLoadEnded;

        public class StringEventArgs : EventArgs
        {
            public string Value;
            public StringEventArgs(string val)
            {
                Value = val;
            }
        }

		public static void RaiseOnMessage(string msg)
		{
			if (OnMessage != null)
				OnMessage (null, new StringEventArgs(msg));
		}        	

		public static void RaiseOnLoadStarted()
		{
			if (OnLoadStarted != null)
				OnLoadStarted (null, EventArgs.Empty); 
		}

		public static void RaiseOnLoadEnded()
		{
			if (OnLoadEnded != null)
				OnLoadEnded (null, EventArgs.Empty); 
		}

		public static void RaiseNullEvents()
		{
			OnLoadStarted = null;
			OnLoadEnded = null;			     
		}      
	}
}

