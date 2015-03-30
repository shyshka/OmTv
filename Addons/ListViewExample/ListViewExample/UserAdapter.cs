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

namespace ListViewExample
{
    public class UserAdapter:BaseAdapter
    {
        private Context cont;
        public List<User> items;

        public UserAdapter(Context cont, List<User> items)
        {
            this.cont = cont;
            this.items = items;
        }

        #region implemented abstract members of BaseAdapter
        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            return new TextView(cont)
            {
                Text = string.Format("Name:{0}\nAge:{1}", items[position].Name,items[position].Age)
            };
        }
        public override int Count
        {
            get
            {
                return items.Count;
            }
        }
        #endregion
    }
}

