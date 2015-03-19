using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace OmTV
{
	[Activity (Label = "OmTV")]
	public class MainActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);

			CommonEvents.OnMessage += (obj, arg) => {
				RunOnUiThread(delegate {
					Toast.MakeText (this, arg, ToastLength.Short).Show ();
				});
			};
			Button btnShowPlaylists = FindViewById<Button> (Resource.Id.btnShowPlaylists);
			btnShowPlaylists.Click += delegate {
				Intent intent = new Intent(this,typeof(PlaylistsActivity));
				intent.AddFlags(ActivityFlags.NewTask);
				StartActivity(intent);
			};

			Button btnShowVideos = FindViewById<Button> (Resource.Id.btnShowVideos);
			btnShowVideos.Click += delegate {
				Intent intent = new Intent (this, typeof(VideosActivity));
				intent.AddFlags (ActivityFlags.NewTask);
				StartActivity (intent);
			};
		}
	}
}