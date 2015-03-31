using Google.Apis.YouTube.v3;

namespace OmTV
{
	public class YoutubeClient
	{      
		private static YouTubeService _instance;		

		public static YouTubeService Instance
        {
            get
            {
                if (_instance == null)
                {
                    YouTubeService.Initializer initializer = new YouTubeService.Initializer();
                    initializer.ApiKey = CommonStrings.ApiKeyOmTV;
                    _instance = new YouTubeService(initializer); 

                }
                return _instance;
            }
        }          
	}
}