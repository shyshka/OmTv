using Android.Content;
using System;
using Android.Graphics;
using System.IO;
using System.Net;
using Android.App;
using Android.Views.Animations;

namespace OmTV
{
	public static class CommonVoids
	{
        public static void PlayVideo(Context cont, string videoId)
        {
            Intent intent = new Intent(
                Intent.ActionView, 
                Android.Net.Uri.Parse(string.Format("https://www.youtube.com/watch?v={0}", videoId)));
            intent.SetFlags(ActivityFlags.NewTask);
            cont.StartActivity(intent);
        }

		public static void WriteLetter(Context cont)
		{
			Intent emailIntent = new Intent (Android.Content.Intent.ActionSend);
			emailIntent.SetType ("plain/text");
			emailIntent.PutExtra (Android.Content.Intent.ExtraEmail, new String[] { CommonStrings.Email});
			emailIntent.PutExtra (Android.Content.Intent.ExtraSubject, CommonStrings.AppName);
			emailIntent.PutExtra (Android.Content.Intent.ExtraText, string.Empty);
			emailIntent.SetFlags (ActivityFlags.NewTask);
			cont.StartActivity (Intent.CreateChooser (emailIntent, CommonStrings.StrSendLetter));
		}

		public static Bitmap GetBitmapFromUrl(string url)
		{            
			Bitmap imageBitmap = null;
			try
			{
				string fileName = System.IO.Path.Combine(Directory.GetParent(url).Name, System.IO.Path.GetFileName(url));				
                string fileOutputPath =  System.IO.Path.Combine(CommonStrings.StrDataDirPath, fileName);
                System.IO.Directory.CreateDirectory(System.IO.Directory.GetParent(fileOutputPath).FullName);

				if (!File.Exists(fileOutputPath)) 
				{
					Directory.CreateDirectory(Directory.GetParent(fileOutputPath).FullName);
					WebClient webClient = new WebClient();
					webClient.DownloadFile(new Uri(url), fileOutputPath);
					webClient.Dispose();
				}
				imageBitmap = BitmapFactory.DecodeFile(fileOutputPath);
			}
			catch (Exception ex)
            { 
                CommonEvents.RaiseOnMessage(ex.Message);
                return null;
            }
			return imageBitmap;
		}

		public static AlertDialog InitLoadingDialog(Context cont){
			var builder = new AlertDialog.Builder (cont);
            builder.SetIcon(Resource.Drawable.Icon);
			builder.SetTitle (CommonStrings.AppName);
			builder.SetMessage (CommonStrings.StrLoading);
			builder.SetCancelable (false);
			return builder.Create ();
		}

		public static void ExitApp()
		{
			Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
		}

		public static Animation InitRotateAnimation(Context cont)
		{
            var anim = AnimationUtils.LoadAnimation(cont, Resource.Animation.rotate_centre);           
			return anim;
		}

        public static void ShowNotification(Context cont,int count)
        {
            Intent intent = new Intent (cont, typeof(MainActivity));

            const int pendingIntentId = 0;
            PendingIntent pendingIntent = 
                PendingIntent.GetActivity (cont, pendingIntentId, intent, PendingIntentFlags.OneShot);

            Notification.Builder builder = new Notification.Builder(cont)
                .SetContentIntent (pendingIntent)
                    .SetContentTitle (string.Format("{0} +{1}",CommonStrings.AppName,count))
                    .SetContentText (CommonStrings.StrNewVideos)
                    .SetSmallIcon (Resource.Drawable.Notification)
                    .SetAutoCancel(true);

            Notification notification = builder.Notification;

            NotificationManager notificationManager =
                cont.GetSystemService (Context.NotificationService) as NotificationManager;

            const int notificationId = 0;
            notificationManager.Notify (notificationId, notification);
        }
	}
}

