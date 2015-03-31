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
	[Activity (Label = "OmTV")]			
	public class VideosActivity : Android.App.Activity
	{
		private ListView lViewPlaylistItemCollection;
		private AlertDialog dlg;
		private string playListId;		

		protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.VideosContainer);

            playListId = Intent.GetStringExtra(CommonStrings.StrContent);
            dlg = CommonVoids.InitLoadingDialog(this);

            lViewPlaylistItemCollection = FindViewById<ListView>(Resource.Id.lViewPlaylist);
            lViewPlaylistItemCollection.ItemClick += (obj, arg) =>
            {
                string itemId = ((obj as ListView).Adapter as VideoItemAdapter).Items[arg.Position].Snippet.ResourceId.VideoId;
                CommonVoids.PlayVideo(this, itemId);
            };

            var container = FindViewById<FlyOutContainer>(Resource.Id.FlyOutContainer);

            var tViewTitle = FindViewById<TextView>(Resource.Id.tViewPlaylistTitle);
            tViewTitle.Text = Intent.GetStringExtra(CommonStrings.StrPlaylistName);

            var btnMenu = FindViewById(Resource.Id.MenuButton);
            btnMenu.Click += (sender, e) =>
            {
                container.AnimatedOpened = !container.AnimatedOpened;
            };


            var btnExit = container.FindViewById<LinearLayout>(Resource.Id.layoutExit);
            btnExit.Click += delegate
            {
                CommonVoids.ExitApp();
            };

            var writeMeBtn = FindViewById(Resource.Id.writeMeBtn);
            writeMeBtn.Click += delegate
            {
                CommonVoids.WriteLetter(this);
            };

            var btnRefresh = FindViewById(Resource.Id.RefreshButton);
            btnRefresh.Click += (sender, e) =>
            {
                YoutubeClient.LoadPlaylistItemCollectionAsync(playListId);
            };


            //CommonEvents.RaiseNullEvents();

            CommonEvents.OnMessage += (obj, arg) =>
            {
                RunOnUiThread(delegate
                {
                    Toast.MakeText(this, arg.Value, ToastLength.Short).Show();   
                });
            };
			
            CommonEvents.OnPlaylistItemCollectionChanged += delegate
            {
                RunOnUiThread(delegate
                {
                    lViewPlaylistItemCollection.Adapter = new VideoItemAdapter(this, YoutubeClient.PlaylistItemCollection);
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
                btnRefresh.Animation = null;
            };
			
            btnRefresh.CallOnClick();
        }
	}
}