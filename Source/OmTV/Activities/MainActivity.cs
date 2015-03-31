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
using System.Threading;
using Android.Views.Animations;

namespace OmTV
{
	[Activity (Label = "OmTV", MainLauncher = true)]			
	public class MainActivity : Activity
	{
		private ListView lViewPlaylistCollection;
		private AlertDialog dlg;

		protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.MainContainer);

            dlg = CommonVoids.InitLoadingDialog(this);

            var container = FindViewById<FlyOutContainer>(Resource.Id.FlyOutContainer);

            var btnExit = container.FindViewById<LinearLayout>(Resource.Id.layoutExit);
            btnExit.Click += delegate
            {
                CommonVoids.ExitApp();
            };

            var btnMenu = FindViewById(Resource.Id.MenuButton);
            btnMenu.Click += (sender, e) =>
            {
                container.AnimatedOpened = !container.AnimatedOpened;
            };

            var btnRefresh = FindViewById(Resource.Id.RefreshButton);
            btnRefresh.Click += (sender, e) =>
            {
                var desData = YoutubeClient.DeserializeData();
                if (desData != null)
                    YoutubeClient.PlaylistCollection = desData;
                else
                    YoutubeClient.LoadPlaylistCollectionAsync();   
                YoutubeClient.UpdatelistCollection();          
            };

            var btnWriteLetter = FindViewById(Resource.Id.writeMeBtn);
            btnWriteLetter.Click += delegate
            {
                CommonVoids.WriteLetter(this);
            };	

            var aboutBtn = FindViewById(Resource.Id.aboutBtn);
            aboutBtn.Click += delegate
            {
                Intent intent = new Intent(this, typeof(About));
                intent.SetFlags(ActivityFlags.NewTask);
                StartActivity(intent);
            };

            lViewPlaylistCollection = FindViewById<ListView>(Resource.Id.lViewPlaylist);
            lViewPlaylistCollection.ItemClick += (obj, arg) =>
            {
                Intent intent = new Intent(this, typeof(VideosActivity));
                intent.AddFlags(ActivityFlags.NewTask);
                intent.PutExtra(CommonStrings.StrContent, (lViewPlaylistCollection.Adapter as PlaylistItemAdapter).Items[arg.Position].Id);
                intent.PutExtra(CommonStrings.StrPlaylistName, (lViewPlaylistCollection.Adapter as PlaylistItemAdapter).Items[arg.Position].Snippet.Title);
                StartActivity(intent);
            };

            //CommonEvents.RaiseNullEvents();

            CommonEvents.OnMessage += (obj, arg) =>
            {
                RunOnUiThread(delegate
                {
                    Toast.MakeText(this, arg.Value, ToastLength.Long).Show();
                });
            };

            CommonEvents.OnPlaylistCollectionChanged += delegate
            {
                RunOnUiThread(delegate
                {
                    lViewPlaylistCollection.Adapter = new PlaylistItemAdapter(this, YoutubeClient.PlaylistCollection);
                });
            };

            CommonEvents.OnLoadStarted += delegate
            {
                RunOnUiThread(delegate
                {
                    btnRefresh.StartAnimation(CommonVoids.InitRotateAnimation());	
                });
            };	

            CommonEvents.OnLoadEnded += delegate
            {
                RunOnUiThread(delegate
                {
                    try
                    {
                        btnRefresh.Animation = null;                       
                    }
                    catch
                    {
                    }		
                });
            };

            btnRefresh.CallOnClick();           
        }

		protected override void OnResume ()
		{
			base.OnResume ();
			var menu = FindViewById<FlyOutContainer> (Resource.Id.FlyOutContainer);
			menu.SetOpened (false, false);
		}
    }
}