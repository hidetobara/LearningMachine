using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;

using IconLibrary;
using Tweetinvi;


namespace TwitterDesktop
{
	class TwitterManager
	{
		private Tweetinvi.Streaming.ISampleStream _Stream;

		private const int QueueMax = 100;
		private Queue<TwitterTweet> _Tweets = new Queue<TwitterTweet>();
		public int Count()
		{
			lock (_Tweets) { return _Tweets.Count; }
		}
		public bool IsFull()
		{
			lock (_Tweets) { if (_Tweets.Count > QueueMax) return true; return false; }
		}
		public bool Add(TwitterTweet t)
		{
			lock (_Tweets) { if (_Tweets.Count > QueueMax) return false; _Tweets.Enqueue(t); return true; }
		}
		public TwitterTweet Get()
		{
			lock (_Tweets) { if (_Tweets.Count > 0) return _Tweets.Dequeue(); return null; }
		}

		public TwitterManager()
		{
			Auth.SetUserCredentials(Twitter.ConsumerKey, Twitter.ConsumerSecret, Twitter.UserToken, Twitter.UserSecret);
		}

		public void StartStream(object dir)
		{
			string imageDir = dir as string;
			if (imageDir == null) return;

			_Stream = Stream.CreateSampleStream();
			_Stream.AddTweetLanguageFilter(Tweetinvi.Models.LanguageFilter.Japanese);
			_Stream.TweetReceived += (sender, args) =>
			{
				try
				{
					var tweet = args.Tweet;
					var by = tweet.CreatedBy;
					TwitterUser u = new TwitterUser() { Name = by.ScreenName, Description = by.Description };
					TwitterTweet t = new TwitterTweet() { Text = tweet.Text, User = u };
					if (IsFull()) return;

					WebClient client = new WebClient();
					string ext = System.IO.Path.GetExtension(by.ProfileImageUrl400x400);
					string path = System.IO.Path.Combine(imageDir, by.ScreenName + ext);
					client.DownloadFile(tweet.CreatedBy.ProfileImageUrl400x400, path);
					u.IconPath = path;
					Add(t);
				}
				catch(Exception ex)
				{
					Log.Instance.Error(ex.Message + "@" + ex.StackTrace);
				}
			};

			_Stream.StartStreamAsync();
		}

		public void EndStream()
		{
			_Stream.StopStream();
		}
	}

	class TwitterTweet
	{
		public string Text;
		public TwitterUser User;
	}

	class TwitterUser
	{
		public string Name;
		public string Description;
		public string IconPath;
	}
}
