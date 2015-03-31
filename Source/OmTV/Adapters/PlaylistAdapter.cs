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
	public class PlaylistAdapter:BaseAdapter
	{
		private Android.App.Activity _activity;
        public PlaylistCollection Items{ get; set; }

        public PlaylistAdapter(Android.App.Activity activity, PlaylistCollection items)
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
            View view = _activity.LayoutInflater.Inflate(Resource.Layout.PlaylistItem, parent, false);
            var item = Items[position];

            if (!string.IsNullOrEmpty(item.ContentDetails.ETag))
            {
                var layout = view.FindViewById<LinearLayout>(Resource.Id.layoutPlaylistitem);
                layout.Background = 
                    new Android.Graphics.Drawables.ColorDrawable(Android.Graphics.Color.Argb(255,40,40,40));
            }

            var textTitle = view.FindViewById<TextView>(Resource.Id.tViewTitle);
            textTitle.Text = string.Format("{0}\n{1}\n{2} відео",
                                           item.Snippet.Title,
                                           item.Snippet.PublishedAt.Value.ToLongDateString(),
                                           item.ContentDetails.ItemCount.Value);

            var tViewNewCnt = view.FindViewById<TextView>(Resource.Id.tViewNewCnt);
            tViewNewCnt.Text = string.Format("{0}", item.ContentDetails.ETag);

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

