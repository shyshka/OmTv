using Android.Widget;
using Android.Views;

namespace OmTV
{
	public class PlaylistitemAdapter:BaseAdapter
	{
		private Android.App.Activity _activity;
		public PlaylistitemCollection Items{ get; set; }

        public PlaylistitemAdapter(Android.App.Activity activity, PlaylistitemCollection collection)
		{
			_activity = activity;
			Items = collection;
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
                layout.SetBackgroundColor(Android.Graphics.Color.Argb(255,40,40,40));
            }

            var textTitle = view.FindViewById<TextView>(Resource.Id.tViewTitle);
            textTitle.Text = string.Format("{0}. {1}\n{2}", 
                                           position + 1,
                                           item.Snippet.Title,
                                           item.Snippet.PublishedAt.Value.ToString());

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