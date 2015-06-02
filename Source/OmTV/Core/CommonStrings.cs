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
using Google.Apis.YouTube.v3.Data;

namespace OmTV
{
	public static class CommonStrings
	{				
        public static readonly string ApiKeyOmTV = @"AIzaSyDtpfszHVJUH-G0NS0f6UgREjQVHfOdBbQ";
		public static readonly string ChannelId = @"UCkp0Tc7ll67bChomTyB1ezQ";
        public static readonly string Email = @"omelchuktv@gmail.com";
			
		public static readonly string UserName = "OmelchukTV";
		public static readonly string StrSnippet = "snippet";
		public static readonly string StrContent = "content";
		public static readonly string StrPlaylistName = "playlistName";       
        public static string StrDataDirPath { get { return 
                    System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); } }

        public static readonly string StrUpdate = "Дані оновлено";
		public static readonly string StrErrorInternet = "Не вдалося завантажити дані.\nПеревірте з'єднання з Internet";
		public static readonly string StrSendLetter = "Відправка листа...";
		public static readonly string StrLoading = "Завантаження даних...";
        public static readonly string StrNewVideos = "На каналі OmTV з'явилися нові відео...";
		public static readonly string AppName = "OmTV";
	}
}