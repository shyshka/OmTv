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
	[Activity (Label = "OmTV", MainLauncher = true)]			
	public class PlaylistsActivity : Activity
	{
		private ListView lView;
		private AlertDialog dlg;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);

			CommonEvents.RaiseNullEvents ();

			CommonEvents.OnMessage += (obj, arg) => {
				RunOnUiThread(delegate {
					Toast.MakeText(this,arg,ToastLength.Long).Show();
				});
			};

			CommonEvents.OnLstPlaylistsChanged += delegate {
				RunOnUiThread(delegate {
					lView.Adapter = new PlaylistItemAdapter (this, CommonData.LstPlaylists);
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
				try{
					dlg.Dismiss ();
				} catch{}
			};

			var menu = FindViewById<FlyOutContainer> (Resource.Id.FlyOutContainer);
			var layExit = menu.FindViewById<LinearLayout> (Resource.Id.layoutExit);
			layExit.Click += delegate {
				this.Finish ();
			};

			var menuButton = FindViewById (Resource.Id.MenuButton);
			menuButton.Click += (sender, e) => {
				menu.AnimatedOpened = !menu.AnimatedOpened;
			};

			lView = FindViewById<ListView> (Resource.Id.lViewPlaylist);
			lView.ItemClick += (obj, arg) => {
				Intent intent = new Intent (this, typeof(VideosActivity));
				intent.AddFlags (ActivityFlags.NewTask);
				intent.PutExtra(CommonData.StrContent,(lView.Adapter as PlaylistItemAdapter).Items[arg.Position].Id);
				intent.PutExtra(CommonData.StrPlaylistName,(lView.Adapter as PlaylistItemAdapter).Items[arg.Position].Snippet.Title);
				StartActivity (intent);
			};

			YouTubeClient.LoadPlaylists();
		}
	}
}

