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
				//string fileOutputPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName);
                string fileOutputPath = System.IO.Path.Combine(@"/mnt/sdcard/images/", fileName);
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
			builder.SetTitle (CommonStrings.AppName);
			builder.SetMessage (CommonStrings.StrLoading);
			builder.SetCancelable (false);
			return builder.Create ();
		}

		public static void ExitApp()
		{
			Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
		}

		public static RotateAnimation InitRotateAnimation()
		{
			RotateAnimation anim = new RotateAnimation (0f, 350f, 15f, 15f);
			anim.Interpolator = new LinearInterpolator ();
			anim.RepeatCount = Animation.Infinite;
			anim.Duration = 700;
			return anim;
		}
	}
}

