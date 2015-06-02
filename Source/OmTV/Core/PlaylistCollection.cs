using Google.Apis.YouTube.v3.Data;
using System.Collections.Generic;
using System;
using System.Threading;
using Newtonsoft.Json;
using System.IO;

namespace OmTV
{
    public class PlaylistCollection:List<Playlist>
    {
        public event EventHandler DataChanged;

        private PlaylistCollection(){
        }

        public void GetDataAsync()
        {
            this.Clear();
            var nextPageToken = string.Empty;
            var listRequest = YoutubeClient.Instance.Playlists.List(CommonStrings.StrSnippet);
            listRequest.ChannelId = CommonStrings.ChannelId;
            listRequest.MaxResults = 20;
            CommonEvents.RaiseOnLoadStarted();
            new Thread(new ThreadStart(delegate
            {
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
                            var playListItemRequest = YoutubeClient.Instance.PlaylistItems.List(CommonStrings.StrSnippet);
                            playListItemRequest.PlaylistId = responseItem.Id;
                            playListItemRequest.MaxResults = 1;
                            playListItemRequest.PageToken = string.Empty;
                            var playListItemResponse = playListItemRequest.Execute();
                            responseItem.ContentDetails.ItemCount = playListItemResponse.PageInfo.TotalResults;                               
                            this.Add(responseItem);                            
                            DataChanged(this, EventArgs.Empty);
                        }
                        nextPageToken = listResponse.NextPageToken;
                    }
                    SaveData(); 
                    CommonEvents.RaiseOnMessage(CommonStrings.StrUpdate);                           
                }
                catch (Exception ex)
                {
                    CommonEvents.RaiseOnMessage(string.Format("{0}\n{1}", CommonStrings.StrErrorInternet, ex.Message));
                    return;
                }
                finally
                {
                    CommonEvents.RaiseOnLoadEnded();
                }
            })).Start();
        }

        private  static PlaylistCollection GetData(bool notify)
        {
            PlaylistCollection res = new PlaylistCollection();
            var nextPageToken = string.Empty;
            var listRequest = YoutubeClient.Instance.Playlists.List(CommonStrings.StrSnippet);
            listRequest.ChannelId = CommonStrings.ChannelId;
            listRequest.MaxResults = 20;
            if (notify) CommonEvents.RaiseOnLoadStarted();
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
                        var playListItemRequest = YoutubeClient.Instance.PlaylistItems.List(CommonStrings.StrSnippet);
                        playListItemRequest.PlaylistId = responseItem.Id;
                        playListItemRequest.MaxResults = 1;
                        playListItemRequest.PageToken = string.Empty;
                        var playListItemResponse = playListItemRequest.Execute();
                        responseItem.ContentDetails.ItemCount = playListItemResponse.PageInfo.TotalResults;                               
                        res.Add(responseItem);                            
                    }
                    nextPageToken = listResponse.NextPageToken;
                }                    
            }
            catch (Exception ex)
            {
                CommonEvents.RaiseOnMessage(string.Format("{0}\n{1}", CommonStrings.StrErrorInternet, ex.Message));
                return null;
            }
            finally
            {
                if (notify) CommonEvents.RaiseOnLoadEnded();
            }
            return res;
        }

        public void UpdateDataAsync()
        {
            new Thread(new ThreadStart(delegate
            {
                List<Playlist> loadList = GetData(true);
                if (loadList == null)
                    return;

                foreach (var loadItem in loadList)
                {
                    var foundItem = this.Find(obj => obj.Id == loadItem.Id);
                    if (foundItem != null)
                    {
                        int def =(int) (loadItem.ContentDetails.ItemCount- foundItem.ContentDetails.ItemCount);
                        if (def != 0)
                        {
                            foundItem.ContentDetails.ETag = string.Format("+{0}", def);
                            DataChanged(this, EventArgs.Empty);
                        }
                    }
                    else
                    {
                        loadItem.ContentDetails.ETag = string.Format("+{0}",loadItem.ContentDetails.ItemCount);
                        this.Insert(0, loadItem);
                        DataChanged(this, EventArgs.Empty);
                    }
                }
                CommonEvents.RaiseOnMessage(CommonStrings.StrUpdate);                                                          
            })).Start();
        }

        public static PlaylistCollection CreateNewInstance()
        {
            return new PlaylistCollection();
        }

        private void SaveData()
        {
            string json = JsonConvert.SerializeObject(this as List<Playlist>);
            Directory.CreateDirectory(Directory.GetParent(CommonStrings.StrDataDirPath).FullName);
            File.WriteAllText(Path.Combine(CommonStrings.StrDataDirPath, "playlists.json"), json);
        }
            
        public static List<Playlist> LoadInstance()
        {
            if (!File.Exists(Path.Combine(CommonStrings.StrDataDirPath, "playlists.json")))
                return null;                     
            string json = File.ReadAllText(Path.Combine(CommonStrings.StrDataDirPath, "playlists.json"));
            return JsonConvert.DeserializeObject(json, typeof(List<Playlist>)) as List<Playlist>;
        }

        public static int GetNewVideosCount()
        {
            int res = 0;
            var origList = LoadInstance();
            var loadList = GetData(false);

            if (origList == null || loadList == null)
                return 0;

            foreach (var loadItem in loadList)
            {
                var foundItem = origList.Find(obj => obj.Id == loadItem.Id);
                if (foundItem != null)
                    res += (int)(loadItem.ContentDetails.ItemCount - foundItem.ContentDetails.ItemCount);
                else
                    res += (int)loadItem.ContentDetails.ItemCount;
            }
            return res;
        }
    }
}

