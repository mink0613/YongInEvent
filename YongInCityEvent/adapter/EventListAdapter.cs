using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using System.Net;

namespace YongInCityEvent.adapter
{
    class EventListAdapter : BaseAdapter
    {
        private Context context;

        private List<string> _eventImageUrl = new List<string>();

        public EventListAdapter(Context context)
        {
            this.context = context;
        }

        public void AddItem(string url)
        {
            _eventImageUrl.Add(url);
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return _eventImageUrl[position];
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;
            EventListAdapterViewHolder holder = null;

            if (view != null)
                holder = view.Tag as EventListAdapterViewHolder;

            if (holder == null)
            {
                holder = new EventListAdapterViewHolder();
                var inflater = context.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
                //replace with your item and your holder items
                //comment back in
                view = inflater.Inflate(Resource.Layout.event_image_view, parent, false);
                //holder.Title = view.FindViewById<TextView>(Resource.Id.text);
                view.Tag = holder;
            }

            //fill in your items
            //holder.Title.Text = "new text here";
            ImageView image = (ImageView) view.FindViewById(Resource.Id.imageView);
            Bitmap bitmap = GetImageBitmapFromUrl(_eventImageUrl[position]);
            image.SetImageBitmap(bitmap);

            return view;
        }

        //Fill in cound here, currently 0
        public override int Count
        {
            get
            {
                return _eventImageUrl.Count;
            }
        }

        private Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;

            using (var webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData(url);
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }

            return imageBitmap;
        }
    }

    class EventListAdapterViewHolder : Java.Lang.Object
    {
        //Your adapter views to re-use
        //public TextView Title { get; set; }
    }
}