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
			SetContentView (Resource.Layout.Playlists);

			lView = FindViewById<ListView> (Resource.Id.lViewPlaylist);
			lView.ItemClick += (obj, arg) => {
				Intent intent = new Intent (this, typeof(VideosActivity));
				intent.AddFlags (ActivityFlags.NewTask);
				intent.PutExtra(CommonData.StrContent,(lView.Adapter as PlaylistItemAdapter).Items[arg.Position].Id);
				StartActivity (intent);
			};

			CommonEvents.RaiseNullEvents ();

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
				dlg.Dismiss();
			};

			YouTubeClient.LoadPlaylists();
		}
	}
}

