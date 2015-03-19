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
using System.Threading.Tasks;
using Google.Apis.YouTube.v3;
using Android.Graphics;
using System.IO;
using System.Net;
using System.Threading;
using Google.Apis.YouTube.v3.Data;

namespace OmTV
{
	public class YouTubeClient
	{
		private static YouTubeService _youTubeService;
		private static YouTubeService CreateYoutubeServiceInstance(){
			YouTubeService.Initializer initializer = new YouTubeService.Initializer ();
			initializer.ApiKey = CommonData.ApiKeyOmTV;
			return new YouTubeService (initializer); 
		}

		public static YouTubeService YouTubeServiceInstance
		{
			get {
				if (_youTubeService == null)
					_youTubeService = CreateYoutubeServiceInstance ();
				return _youTubeService;
			}
		}

		public static void LoadVideos(string playlistId)
		{
			new Thread (new ThreadStart (delegate {
				CommonEvents.RaiseOnLoadStarted();
				var nextPageToken = string.Empty;
				CommonData.LstPlaylistItems.Clear ();
				while (nextPageToken != null) {
					var listRequest = YouTubeServiceInstance.PlaylistItems.List (CommonData.StrSnippet);
					listRequest.PlaylistId = playlistId;
					listRequest.MaxResults = 5;
					listRequest.PageToken = nextPageToken;
					var listResponse = listRequest.Execute ();
					foreach (var item in listResponse.Items)
						GetBitmapFromUrl(item.Snippet.Thumbnails.Default.Url);
					CommonData.LstPlaylistItems.AddRange (listResponse.Items);
					nextPageToken = listResponse.NextPageToken;
					CommonEvents.RaiseOnLstPlaylistItemsChanged ();
				}			
				CommonEvents.RaiseOnLoadEnded();
			})).Start ();
		}

		public static void PlayVideo(Context cont, string videoId)
		{
			Intent intent = new Intent(Intent.ActionView,Android.Net.Uri.Parse(string.Format("https://www.youtube.com/watch?v={0}",videoId)));
			intent.SetFlags(ActivityFlags.NewTask);
			cont.StartActivity(intent);
		}

		public static  void LoadPlaylists()
		{
			new Thread (new ThreadStart (delegate {
				CommonEvents.RaiseOnLoadStarted();
				var nextPageToken = string.Empty;
				CommonData.LstPlaylists.Clear ();
				while (nextPageToken != null) {
					var listRequest = YouTubeServiceInstance.Playlists.List (CommonData.StrSnippet);
					listRequest.ChannelId = CommonData.ChannelId;
					listRequest.MaxResults = 5;
					listRequest.PageToken = nextPageToken;
					var listResponse = listRequest.Execute ();
					CommonData.LstPlaylists.AddRange (listResponse.Items);
					CommonEvents.RaiseOnLstPlaylistsChanged ();
					nextPageToken = listResponse.NextPageToken;
				}			
				CommonEvents.RaiseOnLoadEnded();
			})).Start ();
		}

		public static Bitmap GetBitmapFromUrl(string url)
		{            
			Bitmap imageBitmap = null;
			try
			{
				string fileName = System.IO.Path.Combine(Directory.GetParent(url).Name, System.IO.Path.GetFileName(url));
				string fileOutputPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName);

				if (!File.Exists(fileOutputPath)) 
				{
					Directory.CreateDirectory(Directory.GetParent(fileOutputPath).FullName);
					WebClient webClient = new WebClient();
					webClient.DownloadFile(new Uri(url), fileOutputPath);
					webClient.Dispose();
				}
				imageBitmap = BitmapFactory.DecodeFile(fileOutputPath);
			}
			catch { return null; }
			return imageBitmap;
		}
	}
}