using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;

namespace OmTV
{
	[Activity (Label = "OmTV", MainLauncher = true)]			
	public class MainActivity : Activity
	{
		private ListView lViewCollection;
		private AlertDialog dlg;
        private PlaylistCollection items;

		protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.MainContainer);

            this.StartService (new Intent (this, typeof(OmTVService)));

            items = PlaylistCollection.CreateNewInstance();
            items.DataChanged += delegate
            {
                RunOnUiThread(delegate {
                    (lViewCollection.Adapter as BaseAdapter).NotifyDataSetChanged();   
                });
            };

            dlg = CommonVoids.InitLoadingDialog(this);

            var container = FindViewById<FlyOutContainer>(Resource.Id.FlyOutContainer);

            var btnExit = container.FindViewById<LinearLayout>(Resource.Id.layoutExit);
            btnExit.Click += delegate
            {
                this.Finish();
            };

            var btnMenu = FindViewById(Resource.Id.MenuButton);
            btnMenu.Click += (sender, e) =>
            {
                container.AnimatedOpened = !container.AnimatedOpened;
            };

            var btnRefresh = FindViewById(Resource.Id.RefreshButton);
            btnRefresh.Click += (sender, e) =>
            {
                var res = PlaylistCollection.LoadInstance();
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

            lViewCollection = FindViewById<ListView>(Resource.Id.lViewPlaylist);
            lViewCollection.Adapter = new PlaylistAdapter(this, items);
            lViewCollection.ItemClick += (obj, arg) =>
            {
                Intent intent = new Intent(this, typeof(VideosActivity));
                intent.AddFlags(ActivityFlags.NewTask);
                intent.PutExtra(CommonStrings.StrContent, (lViewCollection.Adapter as PlaylistAdapter).Items[arg.Position].Id);
                intent.PutExtra(CommonStrings.StrPlaylistName, (lViewCollection.Adapter as PlaylistAdapter).Items[arg.Position].Snippet.Title);
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

            CommonEvents.OnLoadStarted += delegate
            {
                RunOnUiThread(delegate
                {
                    dlg.Show();
                    btnRefresh.StartAnimation(CommonVoids.InitRotateAnimation());	
                });
            };	

            CommonEvents.OnLoadEnded += delegate
            {
                RunOnUiThread(delegate
                {
                    dlg.Hide();
                    btnRefresh.Animation = null;                                           		
                });
            };           

            btnRefresh.PerformClick();           
        }

		protected override void OnResume ()
		{
			base.OnResume ();
			var menu = FindViewById<FlyOutContainer> (Resource.Id.FlyOutContainer);
			menu.SetOpened (false, false);
		}
    }
}