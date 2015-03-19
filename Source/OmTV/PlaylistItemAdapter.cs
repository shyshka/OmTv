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
using System.Threading;
using System.Threading.Tasks;

namespace OmTV
{
	public class PlaylistItemAdapter:BaseAdapter
	{
		private Android.App.Activity _activity;
		public List<Playlist> Items{ get; set; }

		public PlaylistItemAdapter(Android.App.Activity activity, List<Playlist> items)
		{
			_activity = activity;
			Items = items;
		}

		#region implemented abstract members of BaseAdapter
		public override Java.Lang.Object GetItem (int position)
		{
			return null;
		}
		public override long GetItemId (int position)
		{
			return position;
		}
		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			View view = _activity.LayoutInflater.Inflate (Resource.Layout.PlaylistItem, parent, false);
			var item = Items [position];

			var textTitle = view.FindViewById<TextView> (Resource.Id.tViewTitle);
			textTitle.Text = string.Format ("{0}. {1}", position + 1, item.Snippet.Title);

			var image = view.FindViewById (Resource.Id.iView) as ImageView;
			image.SetImageBitmap (YouTubeClient.GetBitmapFromUrl (item.Snippet.Thumbnails.Default.Url.ToString ()));

			if (position == Items.Count - 1)
				CommonEvents.RaiseOnMessage ("load more");
			return view;
		}

		public override int Count {
			get {
				return Items.Count;
			}
		}
		#endregion
	}
}

