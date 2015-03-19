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
	[Activity (Label = "OmTV")]			
	public class VideosActivity : Activity
	{
		private ListView lView;
		private AlertDialog dlg;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.MainVideos);

			CommonEvents.RaiseNullEvents ();

			CommonEvents.OnMessage += (obj, arg) => {
				Toast.MakeText(this,arg,ToastLength.Short).Show();
			};
			
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

			lView = FindViewById<ListView> (Resource.Id.lViewPlaylist);
			lView.ItemClick += (obj,arg)=> {
				string itemId =  ((obj as ListView).Adapter as VideoItemAdapter).Items[arg.Position].Snippet.ResourceId.VideoId;
				YouTubeClient.PlayVideo(this,itemId);
			};

			var menu = FindViewById<FlyOutContainer> (Resource.Id.FlyOutContainer);

			var tViewTitle = FindViewById<TextView> (Resource.Id.tViewPlaylistTitle);
			tViewTitle.Text = Intent.GetStringExtra (CommonData.StrPlaylistName);

			var menuButton = FindViewById (Resource.Id.MenuButton);
			menuButton.Click += (sender, e) => {
				menu.AnimatedOpened = !menu.AnimatedOpened;
			};

			var layExit = menu.FindViewById<LinearLayout> (Resource.Id.layoutExit);
			layExit.Click += delegate {
				this.Finish ();
				CommonEvents.RaiseOnMessage("exit");
			};

			string playListId = Intent.GetStringExtra (CommonData.StrContent);
			if (string.IsNullOrEmpty (playListId))
				CommonEvents.RaiseOnMessage ("Playlist id is empty");
			else
				YouTubeClient.LoadVideos (playListId);
		}
	}
}

