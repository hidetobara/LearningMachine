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
			Reconnect();
		}

		private void Reconnect()
		{
			if (_Connection != null) _Connection.Close();

			MySqlConnection conn = new MySqlConnection(Accounts.MYSQL_TWITTER_ENVIRONMENT);
			conn.Open();
			_Connection = conn;
		}

		public void RunHome()
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
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message + "@" + ex.StackTrace);
			}
		}

		private void Save(Status status, string table = "home")
		{
			Save(status.Id, status.CreatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"), status.User.ScreenName, status.Text, table);
		}
		private void Save(long tid, string date, string name, string text, string table = "home")
		{
			text = text.Replace("\'", "");
			text = text.Replace("`", "");
			text = text.Replace("\\", "");

			string sql = "";
			for (int r = 0; r < 3; r++)
			{
				try
				{
					sql = string.Format("SELECT `id` FROM `{1}` WHERE `id` = {0}", tid, table);
					MySqlCommand command = new MySqlCommand(sql, _Connection);
					var o = command.ExecuteScalar();
					if (o != null) return;

					sql = string.Format("INSERT INTO `{4}` (`id`, `at_created`, `screen_name`, `text`) VALUES ({0}, '{1}', '{2}', '{3}')", tid, date, name, text, table);
					command = new MySqlCommand(sql, _Connection);
					command.ExecuteNonQuery();
					break;  // 最後まで実行出来たら終了
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message + "@" + sql + " [" + r + "]");
					if (_Connection.State != System.Data.ConnectionState.Open) Reconnect();
				}
			}
		}

		/**
		 * type0の読み込み
		 */
		public void LoadType0(string userPath, string srcDir)
		{
			Dictionary<int, string> users = new Dictionary<int, string>();
			foreach (string line in File.ReadLines(userPath))
			{
				string screenName = null;
				int uid = 0;
				foreach (var cell in line.Split(','))
				{
					var kv = cell.Split('=');
					if (kv.Length != 2) continue;
					if (kv[0] == "user_screen_name") screenName = kv[1];
					if (kv[0] == "user_id") int.TryParse(kv[1], out uid);
				}
				if (uid > 0 && screenName != null) users[uid] = screenName;
			}

			foreach (string path in Directory.GetFiles(srcDir, "*.log", SearchOption.AllDirectories))
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
			int count = 0;
			foreach (string path in Directory.GetFiles(srcDir, "*.log", SearchOption.AllDirectories))
			{
				foreach (string line in File.ReadLines(path))
				{
					long tid = 0;
					string name = null;
					string date = null;
					string text = null;
					var cells = line.Split(',');
					for (int i = 0; i < cells.Length; i++)
					{
						if (i == 0)
						{
							long.TryParse(cells[0], out tid);
							continue;
						}
						var kv = cells[i].Split('=');
						if (kv.Length != 2) continue;

						if (kv[0] == "id") long.TryParse(kv[1], out tid);
						if (kv[0] == "user_screen_name") name = kv[1];
						if (kv[0] == "created_at" || kv[0] == "create_at") date = kv[1].Replace("Z", "");
						if (kv[0] == "text") text = kv[1];
					}
					if (tid > 0 && date != null && name != null && text != null)
					{
						Save(tid, date, name, text);
						count++;

						if (count % 1000 == 0) { Reconnect(); Console.WriteLine("\trecord=" + count); }
					}
				}
			}
			Console.WriteLine("loaded type1 or 2");
		}

		public void LoadCsvs(string srcDir)
		{
			int count = 0;
			foreach (string path in Directory.GetFiles(srcDir, "*.csv", SearchOption.AllDirectories))
			{
				string name = Path.GetFileNameWithoutExtension(path);
				foreach (string line in File.ReadLines(path))
				{
					long tid = 0;
					var cells = line.Split(',');
					if (cells.Length < 6) continue;
					if (!long.TryParse(cells[0].Trim('"'), out tid)) continue;
					string date = cells[3].Trim('"').Replace(" +0000", "");
					string text = cells[5].Trim('"');
					Save(tid, date, name, text);

					count++;
					if (count % 1000 == 0) { Reconnect(); Console.WriteLine("\trecord=" + count); }
				}
			}
			Console.WriteLine("loaded csvs.");
		}

		/**
		 * ハッシュタグを検索したい、でも同じタグのノイズが入りやすい
		 */
		public void SearchTags()
		{
			try
			{
				var task = _Tokens.Search.TweetsAsync(q: "#.* -RT", count: 100, lang: "jp");
				task.Wait();
				if (task.Exception != null) throw task.Exception;
				var statuses = task.Result;
				foreach (var status in statuses)
				{
					Save(status, "tags");
				}
				Console.WriteLine("Have run search.");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message + "@" + ex.StackTrace);
			}
		}
	}
}
