using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;

namespace OmTV
{
	[Activity (Label = "OmTV")]			
	public class VideosActivity : Android.App.Activity
	{
		private ListView lViewCollection;
		private AlertDialog dlg;
        private PlaylistitemCollection items;		

		protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.VideosContainer);

            items = PlaylistitemCollection.CreateNewInstance(
                Intent.GetStringExtra(CommonStrings.StrContent));
            items.DataChanged += delegate
            {
                RunOnUiThread(delegate
                {
                    (lViewCollection.Adapter as BaseAdapter).NotifyDataSetChanged();
                });
            };

            dlg = CommonVoids.InitLoadingDialog(this);

            lViewCollection = FindViewById<ListView>(Resource.Id.lViewPlaylist);
            lViewCollection.Adapter = new PlaylistitemAdapter(this, items);
            lViewCollection.ItemClick += (obj, arg) =>
            {
                items[arg.Position].ContentDetails.ETag = string.Empty;
                (lViewCollection.Adapter as BaseAdapter).NotifyDataSetChanged();
                items.SaveData();
                CommonVoids.PlayVideo(this, items [arg.Position].Snippet.ResourceId.VideoId);
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
                this.Finish();
            };

            var btnWriteLetter = FindViewById(Resource.Id.writeMeBtn);
            btnWriteLetter.Click += delegate
            {
                CommonVoids.WriteLetter(this);
            };

            var btnAbout = FindViewById(Resource.Id.aboutBtn);
            btnAbout.Click += delegate
            {
                Intent intent = new Intent(this, typeof(About));
                intent.SetFlags(ActivityFlags.NewTask);
                StartActivity(intent);
            };

            var btnRefresh = FindViewById(Resource.Id.RefreshButton);
            btnRefresh.Click += (sender, e) =>
            {
                var res = PlaylistitemCollection.LoadInstance(items.PlaylistId);
                if (res == null)
                    items.GetDataAsync();
                else
                {
                    items.Clear();
                    items.AddRange(res);
                    (lViewCollection.Adapter as BaseAdapter).NotifyDataSetChanged();
                    items.UpdateDataAsync();          
                }   
            };

            CommonEvents.OnMessage += (obj, arg) =>
            {
                RunOnUiThread(delegate
                {
                    Toast.MakeText(this, arg.Value, ToastLength.Short).Show();   
                });
            };

            CommonEvents.OnLoadStarted += delegate
            {
                RunOnUiThread(delegate
                {                   
                    dlg.Show();
                    btnRefresh.StartAnimation(CommonVoids.InitRotateAnimation(this));
                });
            };

            CommonEvents.OnLoadEnded += delegate
            {
                RunOnUiThread(delegate {
                    btnRefresh.Animation = null;
                    dlg.Hide();   
                });
            };
			
            btnRefresh.PerformClick();
        }
	}
}