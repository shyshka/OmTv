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
using Google.Apis.YouTube.v3.Data;

namespace OmTV
{
	public static class CommonData
	{
		static CommonData()
		{
			LstPlaylists = new List<Playlist> ();
			LstPlaylistItems = new List<PlaylistItem> ();
		}

		public static readonly string ApiKeyOmTV = "AIzaSyDFPzm4iSdU-JK9ykZVL1NMK6Iw73-E3C4";
		public static readonly string ChannelId = "UCkp0Tc7ll67bChomTyB1ezQ";		
		public static readonly string StrSnippet = "snippet";
		public static readonly string StrContent = "content";
		public static readonly string StrPlaylistName = "playlistName";

		public static List<Playlist> LstPlaylists{ get; set; }		
		public static List<PlaylistItem> LstPlaylistItems{ get; set; }

	}
}

