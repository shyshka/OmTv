using System.Collections.Generic;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Threading;
using System.IO;
using Newtonsoft.Json;

namespace OmTV
{
    public class PlaylistitemCollection:List<PlaylistItem>
    {
        public string PlaylistId;
        public event EventHandler DataChanged;

        private PlaylistitemCollection(){}

        private static PlaylistitemCollection GetData(string playlistId){
            PlaylistitemCollection res = new PlaylistitemCollection();           
            CommonEvents.RaiseOnLoadStarted();
            var nextPageToken = string.Empty;
            var listRequest = YoutubeClient.Instance.PlaylistItems.List(CommonStrings.StrSnippet);
            listRequest.PlaylistId = playlistId;
            listRequest.MaxResults = 20;          
            try
            {
                while (nextPageToken != null)
                {
                    listRequest.PageToken = nextPageToken;                                         
                    var listResponse = listRequest.Execute();                   
                    foreach (var item in listResponse.Items)
                    {
                        CommonVoids.GetBitmapFromUrl(item.Snippet.Thumbnails.Default.Url);
                        item.ContentDetails = new PlaylistItemContentDetails();
                        res.Add(item);
                    }
                    nextPageToken = listResponse.NextPageToken;
                }              
            }
            catch
            {
                CommonEvents.RaiseOnMessage(CommonStrings.StrErrorInternet);                     
            }
            finally
            {
                CommonEvents.RaiseOnLoadEnded();
            }
            return res;
        }    

        public void GetDataAsync()
        {
            this.Clear();
            CommonEvents.RaiseOnLoadStarted();
            var nextPageToken = string.Empty;
            var listRequest = YoutubeClient.Instance.PlaylistItems.List(CommonStrings.StrSnippet);
            listRequest.PlaylistId = PlaylistId;
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
                        {
                            CommonVoids.GetBitmapFromUrl(item.Snippet.Thumbnails.Default.Url);
                            item.ContentDetails = new PlaylistItemContentDetails();
                            this.Add(item);
                            DataChanged(this,EventArgs.Empty);
                        }
                        nextPageToken = listResponse.NextPageToken;
                    }
                    SaveData();
                    CommonEvents.RaiseOnMessage(CommonStrings.StrUpdate);  
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

        public void UpdateDataAsync()
        {
            new Thread(new ThreadStart(delegate
            {
                List<PlaylistItem> loadList = GetData(PlaylistId);
                if (loadList == null)
                    return;

                foreach (var loadItem in loadList)
                {
                    var foundItem = this.Find(obj => obj.Id == loadItem.Id);
                    if (foundItem != null)
                        continue;
                    loadItem.ContentDetails.ETag = "NEW";
                    this.Insert(0,loadItem);
                    DataChanged(this, EventArgs.Empty);
                }
                CommonEvents.RaiseOnMessage(CommonStrings.StrUpdate);                                                          
            })).Start();
        }

        public static PlaylistitemCollection CreateNewInstance(string playlistId)
        {
            var res = new PlaylistitemCollection();
            res.PlaylistId = playlistId;
            return res;
        }

        public void SaveData()
        {
            string filePath = Path.Combine(CommonStrings.StrDataDirPath, PlaylistId);
            string json = JsonConvert.SerializeObject(this as List<PlaylistItem>);
            Directory.CreateDirectory(CommonStrings.StrDataDirPath);
            File.WriteAllText(filePath, json);
        }         

        public static List<PlaylistItem> LoadInstance(string playlistId)
        {
            string filePath = Path.Combine(CommonStrings.StrDataDirPath, playlistId);
            if (!File.Exists(filePath))
                return null;                     
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject(json, typeof(List<PlaylistItem>)) as List<PlaylistItem>;
        }
    }
}