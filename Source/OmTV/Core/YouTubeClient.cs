using Google.Apis.YouTube.v3;
using System.Threading;
using Android.Content;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using Google.Apis.YouTube.v3.Data;
using System;

namespace OmTV
{
	public class YoutubeClient
	{
        static YoutubeClient()
        {
            PlaylistCollection = new List<Playlist> ();
        }

        public static List<Playlist> _playlistCollection;

        public static List<Playlist> PlaylistCollection
        {
            get
            {
                return _playlistCollection;
            }
            set
            {
                _playlistCollection = value;
                CommonEvents.RaiseOnLstPlaylistsChanged();
            }
        }

        private static List<PlaylistItem> _playlistItemCollection;

        public static List<PlaylistItem> PlaylistItemCollection
        {
            get
            {
                return _playlistItemCollection;
            }
            set
            {
                _playlistItemCollection = value;
                CommonEvents.RaiseOnPlaylistItemCollectionChanged();
            }
        }

		private static YouTubeService _youTubeService;
		private static YouTubeService CreateYoutubeServiceInstance(){
			YouTubeService.Initializer initializer = new YouTubeService.Initializer ();
			initializer.ApiKey = CommonStrings.ApiKeyOmTV;
			return new YouTubeService (initializer); 
		}

        //private static string playlistsDataFilePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonDocuments), "playlists.json");
        private static string playlistsDataFilePath = @"/mnt/sdcard/playlists.json";

		public static YouTubeService YouTubeServiceInstance
        {
            get
            {
                if (_youTubeService == null)
                    _youTubeService = CreateYoutubeServiceInstance();
                return _youTubeService;
            }
        }

		public static void LoadPlaylistItemCollectionAsync(string playlistId)
        {
            PlaylistItemCollection = new List<PlaylistItem>();
            CommonEvents.RaiseOnLoadStarted();
            var nextPageToken = string.Empty;
            var listRequest = YouTubeServiceInstance.PlaylistItems.List(CommonStrings.StrSnippet);
            listRequest.PlaylistId = playlistId;
            listRequest.MaxResults = 20;

            new Thread(new ThreadStart(delegate
            {
                try
                {
                    while (nextPageToken != null)
                    {
                        listRequest.PageToken = nextPageToken;                                         
                        var listResponse = listRequest.Execute();					
                        foreach (var item in listResponse.Items)
                           CommonVoids.GetBitmapFromUrl(item.Snippet.Thumbnails.Default.Url);
                        nextPageToken = listResponse.NextPageToken;
                        PlaylistItemCollection.AddRange(listResponse.Items);
                    }
                    CommonEvents.RaiseOnPlaylistItemCollectionChanged();
                }
                catch
                {
                    CommonEvents.RaiseOnMessage(CommonStrings.StrErrorInternet);                     
                }
                finally
                {
                    CommonEvents.RaiseOnLoadEnded();
                }
            })).Start();
        }        		

        public static void LoadPlaylistCollectionAsync()
        {
            new Thread(new ThreadStart(delegate
            {
                List<Playlist> res = new List<Playlist>();
                var nextPageToken = string.Empty;
                var listRequest = YouTubeServiceInstance.Playlists.List(CommonStrings.StrSnippet);
                listRequest.ChannelId = CommonStrings.ChannelId;
                listRequest.MaxResults = 20;
                CommonEvents.RaiseOnLoadStarted();
                try
                {
                    while (nextPageToken != null)
                    {                    
                        listRequest.PageToken = nextPageToken;                    
                        var listResponse = listRequest.Execute();
                        foreach (var responseItem in listResponse.Items)
                        { 
                            CommonVoids.GetBitmapFromUrl(responseItem.Snippet.Thumbnails.Default.Url);                                             
                            responseItem.ContentDetails = new PlaylistContentDetails();
                            var playListItemRequest = YouTubeServiceInstance.PlaylistItems.List(CommonStrings.StrSnippet);
                            playListItemRequest.PlaylistId = responseItem.Id;
                            playListItemRequest.MaxResults = 1;
                            playListItemRequest.PageToken = string.Empty;
                            var playListItemResponse = playListItemRequest.Execute();
                            responseItem.ContentDetails.ItemCount = playListItemResponse.PageInfo.TotalResults;                               
                            res.Insert(0, responseItem);                            
                        }
                        nextPageToken = listResponse.NextPageToken;
                    }
                    PlaylistCollection = res;
                    SerializeData();                    
                }
                catch
                {                    
                    CommonEvents.RaiseOnMessage(CommonStrings.StrErrorInternet);
                    return;
                }
                finally
                {
                    CommonEvents.RaiseOnLoadEnded();
                }
            })).Start();
        }

        public static List<Playlist> LoadPlaylistCollection()
        {
            List<Playlist> res = new List<Playlist>();
            var nextPageToken = string.Empty;
            var listRequest = YouTubeServiceInstance.Playlists.List(CommonStrings.StrSnippet);
            listRequest.ChannelId = CommonStrings.ChannelId;
            listRequest.MaxResults = 20;
            CommonEvents.RaiseOnLoadStarted();
            try
            {
                while (nextPageToken != null)
                {                    
                    listRequest.PageToken = nextPageToken;                    
                    var listResponse = listRequest.Execute();
                    foreach (var responseItem in listResponse.Items)
                    { 
                        CommonVoids.GetBitmapFromUrl(responseItem.Snippet.Thumbnails.Default.Url);                                             
                        responseItem.ContentDetails = new PlaylistContentDetails();
                        var playListItemRequest = YouTubeServiceInstance.PlaylistItems.List(CommonStrings.StrSnippet);
                        playListItemRequest.PlaylistId = responseItem.Id;
                        playListItemRequest.MaxResults = 1;
                        playListItemRequest.PageToken = string.Empty;
                        var playListItemResponse = playListItemRequest.Execute();
                        responseItem.ContentDetails.ItemCount = playListItemResponse.PageInfo.TotalResults;                               
                        res.Insert(0, responseItem);                            
                    }
                    nextPageToken = listResponse.NextPageToken;
                }                    
            }
            catch
            {                    
                CommonEvents.RaiseOnMessage(CommonStrings.StrErrorInternet);
                return null;
            }
            finally
            {
                CommonEvents.RaiseOnLoadEnded();
            }
            return res;
        }

        public static void UpdatelistCollection()
        {
            new Thread(new ThreadStart(delegate
            {
                List<Playlist> originalList = DeserializeData() ?? new List<Playlist>();
                List<Playlist> loadList = LoadPlaylistCollection();
                if (loadList==null) return;

                foreach (var loadItem in loadList)
                {
                    var foundPlaylist = originalList.Find(obj=>obj.Id==loadItem.Id);
                    if (foundPlaylist != null) continue;
                   
                    loadItem.ContentDetails.ETag = "NEW";
                    originalList.Insert(0,loadItem);
                    PlaylistCollection = originalList;
                }                                                              
            })).Start();
        }

        private static void SerializeData()
        {
            string json = JsonConvert.SerializeObject(PlaylistCollection);
            File.WriteAllText(playlistsDataFilePath, json);
        }

        public static List<Playlist> DeserializeData()
        {
            if (!File.Exists(playlistsDataFilePath))
                return null;                     
            string json = File.ReadAllText(playlistsDataFilePath);
            return JsonConvert.DeserializeObject(json, typeof(List<Playlist>)) as List<Playlist>;
        }
	}
}