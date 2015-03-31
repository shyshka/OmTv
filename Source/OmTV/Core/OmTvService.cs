using Android.App;
using System.Threading;


namespace OmTV
{
    [Service]
    public class OmTVService:Service
    {
        private const int interval = 30000;

        public OmTVService ()
        {
        }

        public override void OnStart (Android.Content.Intent intent, int startId)
        {
            base.OnStart (intent, startId);
            DoStuff ();
        }

        public void DoStuff ()
        {
            new Thread(new ThreadStart(delegate
            {
                while (true)
                {
                    Thread.Sleep(interval);
                    int cnt = PlaylistCollection.GetNewVideosCount();
                    if (cnt > 0)
                        CommonVoids.ShowNotification(this, cnt);
                }
            })).Start();
        }

        #region implemented abstract members of Service
        public override Android.OS.IBinder OnBind(Android.Content.Intent intent)
        {
            return null;
        }
        #endregion
    }
}

