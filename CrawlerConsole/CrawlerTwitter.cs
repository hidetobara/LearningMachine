using System;
using System.Collections.Generic;
using System.Text;
using CoreTweet;
using MySql.Data.MySqlClient;

namespace CrawlerConsole
{
    class CrawlerTwitter
    {
		private Tokens _Tokens;
		private MySqlConnection _Connection;

		public CrawlerTwitter()
		{
			_Tokens = Tokens.Create(Accounts.CONSUMER_KEY, Accounts.CONSUMER_SECRET, Accounts.OBR_TOKEN, Accounts.OBR_SECRET);

			MySqlConnection conn = new MySqlConnection(Accounts.MYSQL_TWITTER_ENVIRONMENT);
			conn.Open();
			_Connection = conn;
		}

		public void Run()
		{
			try
			{
				var task = _Tokens.Statuses.HomeTimelineAsync(count: 200);
				task.Wait();
				if (task.Exception != null) throw task.Exception;
				var statuses = task.Result;
				foreach (var status in statuses)
				{
					Save(status);
				}
				Console.WriteLine("Have run home.");
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message + "@" + ex.StackTrace);
			}
		}

		private void Save(Status status)
		{
			MySqlCommand command;
			string sql = "";
			try
			{
				sql = string.Format("SELECT `id` FROM `home` WHERE `id` = {0}", status.Id);
				command = new MySqlCommand(sql, _Connection);
				var o = command.ExecuteScalar();
				if (o != null) return;

				string strDate = status.CreatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
				string text = status.Text;
				text = text.Replace("\'", "");
				text = text.Replace("`", "");
				sql = string.Format("INSERT INTO `home` (`id`, `at_created`, `screen_name`, `text`) VALUES ({0}, '{1}', '{2}', '{3}')", status.Id, strDate, status.User.ScreenName, text);
				command = new MySqlCommand(sql, _Connection);
				command.ExecuteNonQuery();
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message + "@" + sql);
			}
		}
    }
}
