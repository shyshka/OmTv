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
	public class VideoItemAdapter:BaseAdapter
	{
		private Android.App.Activity _activity;
		public List<PlaylistItem> Items{ get; set; }

		public VideoItemAdapter(Android.App.Activity activity, List<PlaylistItem> items)
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
            if (position > Items.Count)
                return null;

            View view = _activity.LayoutInflater.Inflate(Resource.Layout.PlaylistItem, parent, false);
            var item = Items[position];

            var textTitle = view.FindViewById<TextView>(Resource.Id.tViewTitle);
            textTitle.Text = string.Format("{0}. {1}\n{2}", position + 1, item.Snippet.Title ,item.Snippet.PublishedAt.Value.ToShortDateString());

            var image = view.FindViewById(Resource.Id.iView) as ImageView;
            image.SetImageBitmap(CommonVoids.GetBitmapFromUrl(item.Snippet.Thumbnails.Default.Url.ToString()));
		
            return view;
        }

		public override int Count
        {
            get
            {
                return Items.Count;
            }
        }
		#endregion
	}
}