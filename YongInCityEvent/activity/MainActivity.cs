using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using System.Collections.Generic;
using System.Net;
using System.Text;
using YongInCityEvent.adapter;
using agi = HtmlAgilityPack;

namespace YongInCityEvent.activity
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private ListView listView;
        private EventListAdapter _adapter;
        private Dictionary<string, string> _imageEventDictionary = new Dictionary<string, string>(); // Image Url / Event Url

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            CrawlWebPage();

            _adapter = new EventListAdapter(this);
            foreach (var imageEventPair in _imageEventDictionary)
            {
                _adapter.AddItem(imageEventPair.Key);
            }

            listView = (ListView)FindViewById(Resource.Id.eventList);
            listView.Adapter = _adapter;

            listView.ItemClick += ListView_ItemClick;
        }

        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            string imageUrl = (string)_adapter.GetItem(e.Position);
            string eventUrl = _imageEventDictionary[imageUrl];

            Intent intent = new Intent(this, typeof(EventViewActivity));
            intent.PutExtra("eventUrl", eventUrl);

            StartActivity(intent);
        }

        private void CrawlWebPage()
        {
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            string html = wc.DownloadString("https://www.yongin.go.kr/user/web/event/BD_selectClturEventPfmcList.do");
            agi.HtmlDocument doc = new agi.HtmlDocument();
            doc.LoadHtml(html);

            string imagePreUrl = "https://www.yongin.go.kr";
            string eventPreUrl = "https://www.yongin.go.kr/user/web/event/";

            // Get nodes of xpath tag "//a"
            var eventNodes = doc.DocumentNode.SelectNodes("//a");
            foreach (var node in eventNodes)
            {
                string value = node.Attributes["href"].Value;
                // If the node is not event node, then skip
                if (value.Contains("BD_selectClturEventPfmc.do") == false)
                {
                    continue;
                }

                // Run through all child nodes and find a relavant image of the event
                var children = node.ChildNodes;
                foreach (var child in children)
                {
                    if (child.Attributes.Count > 0 && child.Attributes.Contains("src"))
                    {
                        string val2 = child.Attributes["src"].Value;
                        // If the image is found, then add image & event url
                        if (val2.Contains("/webcontent/tour/evnet/"))
                        {
                            _imageEventDictionary.Add(imagePreUrl + val2, eventPreUrl + value);
                        }
                    }
                }
            }
        }
    }
}

