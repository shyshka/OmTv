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
		public static event EventHandler<string> OnMessage;

		public static event EventHandler OnLoadStarted;
		public static event EventHandler OnLoadEnded;
		public static event EventHandler OnLstPlaylistsChanged;
		public static event EventHandler OnLstPlaylistItemsChanged;

		public static void RaiseOnMessage(string msg)
		{
			if (OnMessage != null)
				OnMessage (null, msg);
		}

		public static void RaiseOnLstPlaylistsChanged()
		{
			if (OnLstPlaylistsChanged != null)
				OnLstPlaylistsChanged (null, EventArgs.Empty);
		}

		public static void RaiseOnLstPlaylistItemsChanged()
		{
			if (OnLstPlaylistItemsChanged != null)
				OnLstPlaylistItemsChanged (null, EventArgs.Empty); 
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
			OnLstPlaylistsChanged = null;
			OnLstPlaylistItemsChanged = null;
		}
	}
}

