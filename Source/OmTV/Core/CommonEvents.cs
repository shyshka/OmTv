using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace OmTV
{
	public static class CommonEvents
	{
		public static event EventHandler<StringEventArgs> OnMessage;
		public static event EventHandler OnLoadStarted;
		public static event EventHandler OnLoadEnded;
		public static event EventHandler OnPlaylistCollectionChanged;
        public static event EventHandler OnPlaylistItemCollectionChanged;

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

		public static void RaiseOnLstPlaylistsChanged()
		{
			if (OnPlaylistCollectionChanged != null)
				OnPlaylistCollectionChanged (null, EventArgs.Empty);
		}

		public static void RaiseOnPlaylistItemCollectionChanged()
		{
			if (OnPlaylistItemCollectionChanged != null)
                OnPlaylistItemCollectionChanged (null, EventArgs.Empty); 
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
			OnPlaylistCollectionChanged = null;
            OnPlaylistItemCollectionChanged = null;
		}
	}
}

