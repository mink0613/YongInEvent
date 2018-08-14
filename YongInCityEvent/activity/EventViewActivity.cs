using Android.App;
using Android.Content;
using Android.OS;
using Android.Webkit;
using Android.Widget;
using System;
using System.Net;
using System.Text;
using agi = HtmlAgilityPack;

namespace YongInCityEvent.activity
{
    [Activity(Label = "EventViewActivity")]
    public class EventViewActivity : Activity
    {
        private float _startPositionX;
        private float _viewWidth;
        private LinearLayout _buttonLayout;
        private WebView _webView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_event_view);

            string eventUrl = Intent.GetStringExtra("eventUrl");

            _buttonLayout = (LinearLayout)FindViewById(Resource.Id.shareButtons);
            _buttonLayout.Visibility = Android.Views.ViewStates.Gone;

            _webView = (WebView)FindViewById(Resource.Id.eventWebView);
            _webView.Settings.LoadWithOverviewMode = true;
            _webView.Settings.UseWideViewPort = true;
            
            _webView.SetWebViewClient(new WebViewClient());

            string innerHtml = GetContents(eventUrl);
            int index = innerHtml.IndexOf("<!-- 작업");
            innerHtml = innerHtml.Remove(0, index); // remove unnecessary info
            _webView.Settings.MinimumFontSize = 16;
            _webView.SetScrollContainer(false);
            _webView.LoadDataWithBaseURL("https://www.yongin.go.kr", innerHtml, "text /html; charset=UTF-8", "UTF-8", null);
            _webView.TextAlignment = Android.Views.TextAlignment.Center;
            //webView.LoadUrl(eventUrl);

            int scale = (int)(100 * _webView.ScaleX);
            _webView.SetInitialScale(scale);

            _webView.Touch += WebView_Touch;
        }

        private void ShowShareButtons()
        {
            _buttonLayout.Visibility = Android.Views.ViewStates.Visible;
        }

        private void HideShareButtons()
        {
            _buttonLayout.Visibility = Android.Views.ViewStates.Gone;
        }

        private void WebView_Touch(object sender, Android.Views.View.TouchEventArgs e)
        {
            WebView webView = sender as WebView;
            if (e.Event.Action == Android.Views.MotionEventActions.Down)
            {
                _startPositionX = e.Event.GetX();
                _viewWidth = webView.Width;
                HideShareButtons();
            }

            if (e.Event.Action == Android.Views.MotionEventActions.Up)
            {
                float movement = e.Event.GetX() - _startPositionX;
                float offset = _viewWidth / 4;

                if (Math.Abs(movement) > offset)
                {
                    if (movement < 0)
                    {
                        Console.WriteLine("Left swipe");
                        ShowShareButtons();
                    }
                    else
                    {
                        Console.WriteLine("Right swipe");
                        if (_startPositionX < 5)
                        {
                            Finish();
                        }
                    }
                }
            }
            e.Handled = false;
        }

        private string GetContents(string url)
        {
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            string html = wc.DownloadString(url);
            agi.HtmlDocument doc = new agi.HtmlDocument();
            doc.LoadHtml(html);

            var element = doc.GetElementbyId("contents");
            return element.InnerHtml;
        }
    }
}