using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
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
				var task = _Tokens.Statuses.HomeTimelineAsync(count: 200, exclude_replies: true);
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
			Save(status.Id, status.CreatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"), status.User.ScreenName, status.Text);
		}
		private void Save(long tid, string date, string name, string text)
		{
			MySqlCommand command;
			string sql = "";
			try
			{
				sql = string.Format("SELECT `id` FROM `home` WHERE `id` = {0}", tid);
				command = new MySqlCommand(sql, _Connection);
				var o = command.ExecuteScalar();
				if (o != null) return;

				text = text.Replace("\'", "");
				text = text.Replace("`", "");
				text = text.Replace("\\", "");
				sql = string.Format("INSERT INTO `home` (`id`, `at_created`, `screen_name`, `text`) VALUES ({0}, '{1}', '{2}', '{3}')", tid, date, name, text);
				command = new MySqlCommand(sql, _Connection);
				command.ExecuteNonQuery();
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message + "@" + sql);
			}
		}

		/**
		 * type0の読み込み
		 */
		public void LoadType0(string userPath, string srcDir)
		{
			Dictionary<int, string> users = new Dictionary<int, string>();
			foreach(string line in File.ReadLines(userPath))
			{
				string screenName = null;
				int uid = 0;
				foreach(var cell in line.Split(','))
				{
					var kv = cell.Split('=');
					if (kv.Length != 2) continue;
					if (kv[0] == "user_screen_name") screenName = kv[1];
					if (kv[0] == "user_id") int.TryParse(kv[1], out uid);
				}
				if (uid > 0 && screenName != null) users[uid] = screenName;
			}

			foreach(string path in Directory.GetFiles(srcDir, "*.log", SearchOption.AllDirectories))
			{
				foreach (string line in File.ReadLines(path))
				{
					var cells = line.Split(',');
					if (cells.Length < 3) continue;

					long tid = 0;
					long.TryParse(cells[0], out tid);
					string date = cells[1] + ":00";
					var blocks = cells[2].Split(':');
					if (blocks.Length < 2) continue;
					int uid = 0;
					int.TryParse(blocks[0], out uid);
					string text = blocks[1];

					if (!users.ContainsKey(uid)) continue;
					string name = users[uid];

					Save(tid, date, name, text);
				}
			}
			Console.WriteLine("loaded type0");
		}

		public void LoadType1or2(string srcDir)
		{
			foreach (string path in Directory.GetFiles(srcDir, "*.log", SearchOption.AllDirectories))
			{
				foreach (string line in File.ReadLines(path))
				{
					long tid = 0;
					string name = null;
					string date = null;
					string text = null;
					foreach (string cell in line.Split(','))
					{
						var kv = cell.Split('=');
						if (kv.Length == 1)
						{
							long.TryParse(cell, out tid);
							continue;
						}
						if (kv.Length == 2)
						{
							if (kv[0] == "id") long.TryParse(kv[1], out tid);
							if (kv[0] == "user_screen_name") name = kv[1];
							if (kv[0] == "created_at" || kv[0] == "create_at") date = kv[1];
							if (kv[0] == "text") text = kv[1];
						}
					}
					if (tid > 0 && date != null && name != null && text != null) Save(tid, date, name, text);
				}
			}
			Console.WriteLine("loaded type1 or 2");
		}
	}
}
