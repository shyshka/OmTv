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
	[Activity (Label = "VideosActivity" )]			
	public class VideosActivity : Activity
	{
		private ListView lView;
		private AlertDialog dlg;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Videos);
			lView = FindViewById<ListView> (Resource.Id.lViewPlaylist);
			lView.ItemClick += (obj,arg)=> {
				string itemId =  ((obj as ListView).Adapter as VideoItemAdapter).Items[arg.Position].Snippet.ResourceId.VideoId;
				YouTubeClient.PlayVideo(this,itemId);
			};

			CommonEvents.RaiseNullEvents ();
			CommonEvents.OnLstPlaylistItemsChanged += delegate {
				RunOnUiThread (delegate {
					lView.Adapter = new VideoItemAdapter (this, CommonData.LstPlaylistItems);
				});
			};

			CommonEvents.OnLoadStarted += delegate {
				RunOnUiThread (delegate {
					var builder = new AlertDialog.Builder (this);
					builder.SetTitle ("OmTv");
					builder.SetMessage("Loading data");
					builder.SetCancelable(false);
					dlg = builder.Create ();
					dlg.Show ();		
				});
			};		

			CommonEvents.OnLoadEnded += delegate {
				dlg.Dismiss();
			};

			string playListId = Intent.GetStringExtra (CommonData.StrContent);
			if (string.IsNullOrEmpty (playListId))
				CommonEvents.RaiseOnMessage ("Playlist id is empty");
			else
				YouTubeClient.LoadVideos (playListId);
		}
	}
}

