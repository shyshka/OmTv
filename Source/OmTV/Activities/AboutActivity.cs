using Android.App;
using System;
using Google.Apis.YouTube.v3.Data;
using Android.OS;
using Android.Widget;
using System.Threading;

namespace OmTV
{
	[Activity (Label = "About")]			
	public class About : Android.App.Activity
	{
		private event EventHandler<Channel> OnFinished;

		protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.AboutContainer);

            var container = FindViewById<FlyOutContainer>(Resource.Id.FlyOutContainer);

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
            			
            var text = FindViewById<TextView>(Resource.Id.tViewAbout);

            OnFinished += (obj, arg) =>
            {
                RunOnUiThread(delegate
                {					
                    text.Text = arg.Snippet.Description;
                });
            };

            new Thread(new ThreadStart(delegate
            {
                try
                {
                    var request = YoutubeClient.Instance.Channels.List(CommonStrings.StrSnippet);
                    request.ForUsername = CommonStrings.UserName;
                    var response = request.Execute();
                    OnFinished(null, response.Items[0]);
                }
                catch
                {
                    CommonEvents.RaiseOnMessage(CommonStrings.StrErrorInternet);
                }
            })).Start();
        }
	}
}