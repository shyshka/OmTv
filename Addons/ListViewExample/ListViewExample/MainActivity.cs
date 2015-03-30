using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;

namespace ListViewExample
{
    [Activity(Label = "ListViewExample", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private ListView lview;
        private List<User> items;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            items = new List<User>();
            lview = FindViewById(Resource.Id.listView1) as ListView;
            lview.Adapter = new UserAdapter(this, items);
            lview.ItemClick += (obj, arg) =>
            {
                (lview.Adapter as UserAdapter).items.RemoveAt(arg.Position);
                (lview.Adapter as UserAdapter).NotifyDataSetChanged();
            };

            Button button = FindViewById<Button>(Resource.Id.myButton);
			
            button.Click += delegate
            {
                items.Add(new User{Name = "new item",Age = new Random().Next(25,35)});
                (lview.Adapter as UserAdapter).NotifyDataSetChanged();
            };
        }
    }
}


