using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using NMeCab;
using IconLibrary;

namespace ConsoleAnalyzerText2
{
	public class Analyzer
	{
		const int AXIS_MAX = 32;
		const int WORD_COUNT = 5000;
		const int CUT_LIMIT = 3;
		const int TOP_COUNT = 50;
		const string DIC = "C:/obara/Library/NMeCab0.07/dic/ipadic";

		public void RunWakuwaku()
		{
			PickupProfileInWakuwaku(new Analyzer.Option() { InputDir = "C:/obara/Data/Waku/Crawl", OutputDir = "C:/obara/Data/Waku/Process" });
			CalclateWordClasses(new Analyzer.Option() { DicDir = DIC, InputDir = "C:/obara/Data/Waku/Process", DictionaryPath = "C:/obara/Data/Waku/Process/words.csv" });
			CalclateFrequency(new Analyzer.Option() { DicDir = DIC, Loop = 2, DictionaryPath = "C:/obara/Data/Waku/Process/words.csv", InputDir = "C:/obara/Data/Waku/Process", OutputDir = "C:/obara/Data/Waku/Process" });
			GroupByStatistics(new Analyzer.Option() { DictionaryPath = "C:/obara/Data/Waku/Process/words.csv", InputDir = "C:/obara/Data/Waku/Process", OutputDir = "C:/obara/Data/Waku/Process" });
		}

		public void RunHappyMail()
		{
			PickupProfileInHappyMail(new Analyzer.Option() { InputDir = "C:/obara/Data/HappyMail/Crawl", OutputDir = "C:/obara/Data/HappyMail/Process" });
			CalclateWordClasses(new Analyzer.Option() { DicDir = DIC, InputDir = "C:/obara/Data/HappyMail/Process", DictionaryPath = "C:/obara/Data/HappyMail/Process/words.csv" });
			CalclateFrequency(new Analyzer.Option() { Loop = 2, DictionaryPath = "C:/obara/Data/HappyMail/Process/words.csv", InputDir = "C:/obara/Data/HappyMail/Process", OutputDir = "C:/obara/Data/HappyMail/Process" });
			GroupByStatistics(new Analyzer.Option() { DictionaryPath = "C:/obara/Data/HappyMail/Process/words.csv", InputDir = "C:/obara/Data/HappyMail/Process", OutputDir = "C:/obara/Data/HappyMail/Process" });
			//analyzer.GroupByStatistics(new Analyzer.Option() { ReloadStatistics = true, DictionaryPath = "C:/obara/Data/HappyMail/Process/words.csv", InputDir = "C:/obara/Data/HappyMail/Process", OutputDir = "C:/obara/Data/HappyMail/Process", PickupAxis = 4, PickupLower = -3, PickupUpper = -1.1 });
			//analyzer.GroupByDifference(new Analyzer.Option() { DictionaryPath = "C:/obara/Data/HappyMail/Process/words.csv", InputDir = "C:/obara/Data/HappyMail/Process", OutputDir = "C:/obara/Data/HappyMail/Process" });
			BuildVolatile(new Analyzer.Option() { InputDir = "C:/obara/Data/HappyMail/Process", OutputDir = "C:/obara/Data/HappyMail/Process" });
		}

		public void RunYYC()
		{
			PickupProfileInYYC(new Analyzer.Option() { InputDir = "C:/obara/Data/YYC/Crawl", OutputDir = "C:/obara/Data/YYC/Process" });
			PickupAgesInYYC(new Analyzer.Option() { InputDir = "C:/obara/Data/YYC/Crawl", OutputDir = "C:/obara/Data/YYC/Process" });
			CalclateWordClasses(new Analyzer.Option() { DicDir = DIC, InputDir = "C:/obara/Data/YYC/Process", DictionaryPath = "C:/obara/Data/YYC/Process/words.csv" });
			CalclateFrequency(new Analyzer.Option() { DicDir = DIC, Loop = 2, DictionaryPath = "C:/obara/Data/YYC/Process/words.csv", InputDir = "C:/obara/Data/YYC/Process", OutputDir = "C:/obara/Data/YYC/Process" });
			GroupByStatistics(new Analyzer.Option() { DictionaryPath = "C:/obara/Data/YYC/Process/words.csv", InputDir = "C:/obara/Data/YYC/Process", OutputDir = "C:/obara/Data/YYC/Process" });
		}

		public void BuildVolatile(Option o)
		{
			MorphemeManager.Instance.Initialize("C:/obara/Library/NMeCab0.07/dic/ipadic");
			VolatileManager v = new VolatileManager(o.OutputDir, "happy");
			v.LearningMatrix(o.InputDir);
			v.Save();
			//v.Load();
			string s = v.Generate();
		}

		/*
		 * 逆投影によって、パターン物とそうでないものに分類できないか
		 * 逆投影後、元から大きくずれる物はそれだけパターンでなく、ずれないものはパターン物つまり業者になるのでは
		 * →使えなかった
		 */
		public void GroupByDifference(Option o)
		{
			var dictionary = WordsDictionary.Load(o.DictionaryPath, CUT_LIMIT);
			var tagger = PrepareTagger(o);
			var ipca = new LearningIPCAForWords();
			ipca.Load(o.IpcaDir);

			int index = 0;
			StreamWriter writer = new StreamWriter(Path.Combine(o.OutputDir, "profile_difference.csv"));
			writer.Write("id\t");
			for (int aa = 0; aa < AXIS_MAX; aa++) writer.Write("len" + aa + "\terr" + aa + "\t");
			writer.WriteLine();
			foreach (var p in Directory.GetFiles(o.InputDir, "*.log"))
			{
				foreach (var line in File.ReadLines(p))
				{
					var used = RetrieveWords(tagger, line);
					var image = dictionary.WordsToImage(used);
					var vetor = ipca.Forecast(image);
					List<string> list = new List<string>() { index.ToString() };
					for (int a = 0; a < AXIS_MAX; a++)
					{
						var foot = new LearningImage(1, AXIS_MAX, 1);
						foot.Data[a] = vetor.Data[a];
						var reimaged = ipca.BackForecast(foot);
						LearningImage.Sub(image, reimaged, reimaged);
						double distance = LearningImage.EuclideanLength(reimaged);
						list.Add(foot.Data[a].ToString("F2"));
						list.Add(distance.ToString("F2"));
					}
					writer.WriteLine(string.Join("\t", list) + "\t" + line);

					index++;
					if (index % 1000 == 0) Console.WriteLine("\tindex=" + index);
				}
			}
			Console.WriteLine("grouped.");
		}

		/*
		 * 統計情報からグループ分けする 
		 */
		public void GroupByStatistics(Option o)
		{
			var dictionary = WordsDictionary.Load(o.DictionaryPath, CUT_LIMIT);
			var tagger = PrepareTagger(o);
			var ipca = new LearningIPCAForWords();
			ipca.Load(o.IpcaDir);
			int index = 0;

			// lines
			var lines = LoadLinesUsingUnique(o.InputDir, "*.log");

			// 平均と偏差
			Statistics statitcs = new Statistics();
			if (o.ReloadStatistics) statitcs.Load(Path.Combine(o.OutputDir, "statistics.csv"));
			if (!statitcs.IsCalclated)
			{
				foreach (var line in lines)
				{
					var used = RetrieveWords(tagger, line);
					var image = dictionary.WordsToImage(used);
					var vetor = ipca.Project(image);
					statitcs.Add(vetor.Data);
					index++;
					if (index % 1000 == 0) Console.WriteLine("\tindex=" + index);
				}
				statitcs.Calclate(Path.Combine(o.OutputDir, "statistics.csv"));
			}

			// 偏差によってならした値でグループ化
			index = 0;
			TopList[] tops = new TopList[AXIS_MAX];
			TopList[] bottoms = new TopList[AXIS_MAX];
			for (int a = 0; a < AXIS_MAX; a++)
			{
				tops[a] = new TopList(a, TOP_COUNT);
				bottoms[a] = new TopList(-a, TOP_COUNT);
			}
			TopList pickup = new TopList(o.PickupAxis, 500);
			TopList outer = new TopList(0, 1000);
			LearningImage deviator = statitcs.GenerateDeviator();
			LearningImage bias = statitcs.GenerateBias();

			StreamWriter writer = new StreamWriter(Path.Combine(o.OutputDir, "profile_grouped.csv"));
			writer.Write("CountOut\tMaxIndex\t");
			for (var a = 0; a < AXIS_MAX; a++) writer.Write("Value" + a + "\t");
			writer.WriteLine();
			foreach (var line in lines)
			{
				var trimed = line.Trim().Replace("\t", "|");
				var used = RetrieveWords(tagger, line);
				var image = dictionary.WordsToImage(used);
				var vetor = ipca.Project(image);
				var corrected = new LearningImage(1, AXIS_MAX, 1);
				LearningImage.Add(vetor, bias, corrected);
				LearningImage.Multiply(corrected, deviator, corrected);
				var maxIdx = corrected.Data.Select((val, idx) => new { V = val, I = idx }).Aggregate((max, working) => (max.V > working.V) ? max : working).I;
				var maxVal = corrected.Data[maxIdx];
				var cntOut = corrected.Data.Count(a => a > 1.96);   // 95%信頼区間
				List<string> cells = new List<string>();
				cells.Add(cntOut.ToString());
				cells.Add(maxIdx.ToString());
				for (int i = 0; i < corrected.Data.Length; i++) cells.Add(corrected.Data[i].ToString("F2"));
				cells.Add(trimed);
				writer.WriteLine(string.Join("\t", cells));

				if (o.PickupAxis > 0)
				{
					double value = corrected.Data[o.PickupAxis];
					if (o.PickupLower < value && value < o.PickupUpper) pickup.Add(trimed, value);
				}

				tops[maxIdx].Add(trimed, maxVal);
				bottoms[maxIdx].Add(trimed, -maxVal);
				outer.Add(trimed, cntOut);
				index++;
				if (index % 1000 == 0) Console.WriteLine("\tindex=" + index);
			}
			writer.Close();

			List<string> grouped = new List<string>();
			for (int a = 0; a < AXIS_MAX; a++)
			{
				if (a != 0) grouped.Add("");
				grouped.AddRange(tops[a].ToList());
			}
			WriteLines(Path.Combine(o.OutputDir, "group_by_statistics.csv"), grouped);
			WriteLines(Path.Combine(o.OutputDir, "group_by_outer.csv"), outer.ToList());
			if (o.PickupAxis > 0) WriteLines(Path.Combine(o.OutputDir, "group_by_pickup.csv"), pickup.ToList());

			Console.WriteLine("grouped.");
		}

		private List<string> LoadLinesUsingUnique(string dir, string pattern)
		{
			Dictionary<string, int> table = new Dictionary<string, int>();
			foreach (var p in Directory.GetFiles(dir, "*.log"))
			{
				foreach (var line in File.ReadLines(p)) table[line] = 1;
			}
			return new List<string>(table.Keys);
		}

		private class Statistics
		{
			public int Count;
			public double[] Averages;
			public double[] Deviations;
			public bool IsCalclated { get; private set; }

			public Statistics()
			{
				IsCalclated = false;
				Averages = new double[AXIS_MAX];
				Deviations = new double[AXIS_MAX];
			}
			public bool Load(string path)
			{
				for (int a = 0; a < AXIS_MAX; a++) Deviations[a] = 1;

				var lines = File.ReadAllLines(path);
				if (lines.Length < AXIS_MAX) return false;

				for (int a = 0; a < lines.Length; a++)
				{
					string[] cells = lines[a].Split('\t');
					if (cells.Length < 3) continue;
					double.TryParse(cells[1], out Averages[a]);
					double.TryParse(cells[2], out Deviations[a]);
				}
				IsCalclated = true;
				return true;
			}

			public void Add(double[] v)
			{
				if (IsCalclated) throw new Exception("No more add, already calclated.");

				for (int a = 0; a < AXIS_MAX; a++)
				{
					Averages[a] += v[a];
					Deviations[a] += v[a] * v[a];
				}
				Count++;
			}
			public bool Calclate(string path)
			{
				List<string> lines = new List<string>();
				for (int a = 0; a < AXIS_MAX; a++)
				{
					double average = Averages[a] / Count;
					double distribution = Deviations[a] / Count - average * average;
					double deviation = Math.Sqrt(distribution);
					Averages[a] = average;
					Deviations[a] = deviation;
					lines.Add(a + "\t" + average.ToString("F4") + "\t" + deviation.ToString("F4"));
				}
				File.WriteAllLines(path, lines);
				IsCalclated = true;
				return true;
			}

			public LearningImage GenerateBias()
			{
				LearningImage bias = new LearningImage(1, AXIS_MAX, 1);
				bias.Data[0] = 0;
				for (int a = 1; a < AXIS_MAX; a++) bias.Data[a] = -this.Averages[a];
				return bias;
			}

			public LearningImage GenerateDeviator()
			{
				LearningImage deviator = new LearningImage(1, AXIS_MAX, 1);
				deviator.Data[0] = 0;
				for (int a = 1; a < AXIS_MAX; a++) deviator.Data[a] = 1 / this.Deviations[a];
				return deviator;
			}
		}

		private class TopList
		{
			int _Axis;
			int _Max = 30;
			int _Count;
			double _Amount;
			double _Distribution;
			Dictionary<string, double> _Table = new Dictionary<string, double>();

			public TopList(int axis, int max)
			{
				_Axis = axis;
				_Max = max;
			}
			public void Add(string w, double v)
			{
				_Count++;
				_Amount += v;
				_Distribution += v * v;
				if (_Table.Count < _Max)
				{
					_Table[w] = v;
					return;
				}
				var min = _Table.OrderBy(p => p.Value).First();
				if (min.Value < v)
				{
					_Table.Remove(min.Key);
					_Table[w] = v;
				}
			}

			public List<string> ToList()
			{
				double average = _Amount / _Count;
				double distribution = _Distribution / _Count - average * average;
				double deviation = Math.Sqrt(distribution);

				List<string> list = new List<string>() { "Axis=" + _Axis.ToString("D2") + "\tCnt=" + _Count + "\tAve=" + average.ToString("F4") + "\tDev=" + deviation.ToString("F4") };
				foreach (var p in _Table.OrderByDescending(p => p.Value)) list.Add(p.Value.ToString("F4") + "\t" + p.Key);
				return list;
			}
		}

		/*
		 * 単語の頻度を計算する
		 */
		public void CalclateFrequency(Option o)
		{
			if (o.Loop < 1) o.Loop = 1;

			var dictionary = WordsDictionary.Load(o.DictionaryPath, CUT_LIMIT);
			var tagger = PrepareTagger(o);
			var ipca = new LearningIPCAForWords();
			if (o.ReloadIpca) ipca.Load(o.IpcaDir); else ipca.Initialize();
			int index = 0;
			for (int l = 0; l < o.Loop; l++)
			{
				foreach (var p in Directory.GetFiles(o.InputDir, "*.log"))
				{
					foreach (var line in File.ReadLines(p))
					{
						var used = RetrieveWords(tagger, line);
						var image = dictionary.WordsToImage(used);
						ipca.Learn(new List<LearningImage>() { image });
						index++;
						if (index % 1000 == 0) Console.WriteLine("\tindex=" + index);
					}
				}
			}
			ipca.Save(o.IpcaDir);

			var indexDictionary = dictionary.ToIndexDictionary();
			for(int main = 0; main < ipca.MainMax; main++)
			{
				var list = PickupTop(ipca, indexDictionary, main, 100);
				List<string> lines = new List<string>();
				foreach (var item in list) lines.Add(item.ToString());
				string path = Path.Combine(o.IpcaDir, "main" + main + ".csv");
				File.WriteAllLines(path, lines);
			}
			Console.WriteLine("calclarated frequency.");
		}

		private List<Pickup> PickupTop(LearningIPCA ipca,　Dictionary<int, string> dic, int main, int top = 30)
		{
			var image = ipca.MainImages[main];
			List<Pickup> list = new List<Pickup>();
			for (int n = 0; n < image.Data.Length; n++)
			{
				if (!dic.ContainsKey(n)) continue;
				list.Add(new Pickup() { Word = dic[n], Value = image.Data[n] });
			}
			return new List<Pickup>(list.OrderByDescending(x => Math.Abs(x.Value)).Take(top));
		}

		private class Pickup
		{
			public string Word;
			public double Value;
			public override string ToString() { return Value.ToString("F4") + "," + Word; }
		}

		/*
		 * 頻度入り辞書
		 */
		private class WordsDictionary
		{
			readonly List<string> Ignores = new List<string>() { "する", "いる", "ある", "ない", "てる", "こと", "れる", "なる", "の", "、", "。", "ー", "､", "｡" };
			private Dictionary<string, Word> _Dictionary;
			public int CountMax { get; private set; }

			public Word Get(string s) { if (!_Dictionary.ContainsKey(s)) return null; return _Dictionary[s]; }

			/*
			 * 単語列からベクトル化する
			 * 1. 短い文章は何も内容が無いということで、分類されたくない→文章の個数の平方根
			 * 2. 頻度が低い単語の優先度を上げたいLog(x, 2.0)に
			 */
			public LearningWords WordsToImage(List<MorphemeManager.Word> words)
			{
				var image = new LearningWords();
				image.Data[0] = 1.0;
				foreach (var w in words)
				{
					if (w.IsTerminal()) continue;	// 終端
					if (!_Dictionary.ContainsKey(w.Origin)) continue;	// 辞書に含まれない
					if (Ignores.Contains(w.Origin)) continue;   // 無視単語
					if (!w.IsUsefull()) continue;
					var word = _Dictionary[w.Origin];
//					double importance = Math.Log((double)(CountMax + 1) / (double)word.CountLine, Math.E); // 本来
					double importance = Math.Log((double)(CountMax + 1) / (double)word.CountLine, 2.0); // 頻度の高い単語は無視したい
//					image.Data[word.ID] = importance * w.CountWord / words.Count;   // 本来
					image.Data[word.ID] += importance / Math.Sqrt(words.Count);	// 単語の少ない文章は無視したい
				}
				return image;
			}

			public Dictionary<int, string> ToIndexDictionary()
			{
				var invDictionary = new Dictionary<int, string>();
				foreach (var pair in _Dictionary) invDictionary.Add(pair.Value.ID, pair.Key);
				return invDictionary;
			}
			
			public static WordsDictionary Load(string path, int limit = 2)
			{
				var i = new WordsDictionary();
				i._Dictionary = new Dictionary<string, Word>();
				foreach (string line in File.ReadLines(path))
				{
					var w = Word.Parse(line);
					if (w == null) continue;
					if (w.CountLine <= limit) continue;
					i._Dictionary[w.Text] = w;
				}
				i.CountMax = i._Dictionary.Max(x => x.Value.CountLine);
				return i;
			}
		}

		/*
		 * 有意な品詞だけ取り出す
		 */
		public void CalclateWordClasses(Option o)
		{
			var tagger = PrepareTagger(o);
			var words = new Dictionary<string, MorphemeManager.Word>();

			foreach (var p in Directory.GetFiles(o.InputDir, "*.log"))
			{
				foreach (var user in File.ReadLines(p))
				{
					List<string> found = new List<string>();
					var used = RetrieveWords(tagger, user);
					foreach (var w in used)
					{
						if (w.IsTerminal()) continue;
						if (!w.IsUsefull()) continue;
						if (!words.ContainsKey(w.Origin)) words[w.Origin] = w;
						words[w.Origin].CountWord++;
						if (!found.Contains(w.Origin)) found.Add(w.Origin);
					}
					foreach (var w in found) words[w].CountLine++;
				}
			}

			int amount = 1;
			List<string> lines = new List<string>();
			foreach (var word in words.Values.OrderByDescending(w => w.CountWord))
			{
				word.ID = amount++;
				lines.Add(word.ToString());
			}
			WriteLines(o.DictionaryPath, lines);
			Console.WriteLine("calclated words classes.");
		}

		private MorphemeManager PrepareTagger(Option o)
		{
			MorphemeManager.Instance.Initialize(o.DicDir);
			return MorphemeManager.Instance;
		}

		private List<MorphemeManager.Word> RetrieveWords(MorphemeManager morpheme, string input)
		{
			return morpheme.Parse(input, false);
		}

		/*
		 * HappyMail内のプロフィールだけを抜き出す
		 */
		public void PickupProfileInHappyMail(Option o)
		{
			Console.WriteLine("pickuping profiles.");
			string[] removes = new string[] { "ﾒﾓの登録", "⇒", "ﾒﾓ内容", "&nbsp;" };
			Regex r = new Regex("<table.*?自己紹介.*?</table>(.*)<table");
			Regex tag = new Regex("<[ :&/a-zA-Z0-9=#%\\?\\.\"\']*>");
			Regex space = new Regex("\\s+");
			Regex letter = new Regex("&#\\d+;");
			Regex tabs = new Regex("\t+");

			List<string> pickups = new List<string>();
			foreach (var path in Directory.GetFiles(o.InputDir, "*.xml", SearchOption.AllDirectories))
			{
				foreach (var line in File.ReadLines(path, Encoding.GetEncoding("shift_jis")))
				{
					Match match = r.Match(line);
					if (!match.Success) continue;

					string replaced = tag.Replace(match.Groups[1].Value, "\t");
					replaced = replaced.Replace('。', '\t');
					replaced = letter.Replace(replaced, "\t");
					replaced = space.Replace(replaced, "\t");
					foreach (var s in removes) replaced = replaced.Replace(s, "");

					pickups.Add(replaced);
					if (pickups.Count % 1000 == 0) Console.WriteLine("\tcount pickups=" + pickups.Count);
				}
			}
			WriteLines(Path.Combine(o.OutputDir, "profile.log"), pickups);
		}

		/*
		 * Wakuwaku内のプロフィールだけを抜き出す
		 */
		public void PickupProfileInWakuwaku(Option o)
		{
			Console.WriteLine("pickuping profiles.");
			string[] removes = new string[] { "&amp;", "&nbsp;", "&gt;", "&lt;", "自由ｺﾒﾝﾄ", "自由コメント", "ﾏｲﾌﾞｰﾑ", "マイブーム", "誰似", "髪型", "性格", "望む相手", "特技", "休日", "趣味", "恋人", "車", "夢",
				 "-", "_", ",", ".", "/", ":", "：", ";", "；", "･", "m", "^", "o", "(", ")", "*", "`", "'", "＼", "／", "♪", "?" };
			Regex r = new Regex("<p class=\"p-profile-comment__content\">(.*?)</p>");
			Regex tag = new Regex("<[^>]*?>");
			Regex space = new Regex("[。\\s\r\n]+");
			Regex letter = new Regex("&#\\d+;");
			Regex tabs = new Regex("\t+");

			List<string> pickups = new List<string>();
			foreach (var path in Directory.GetFiles(o.InputDir, "*.xml", SearchOption.AllDirectories))
			{
				var context = File.ReadAllText(path, Encoding.GetEncoding("utf-8"));
				context = context.Replace("\r\n", "");
				Match match = r.Match(context);
				if (!match.Success) continue;

				string replaced = match.Groups[1].Value;
				replaced = tag.Replace(replaced, "\t");
				replaced = letter.Replace(replaced, "\t");
				replaced = space.Replace(replaced, "\t");
				foreach (var s in removes) replaced = replaced.Replace(s, "");
				pickups.Add(replaced);
				if (pickups.Count % 1000 == 0) Console.WriteLine("\tcount pickups=" + pickups.Count);
			}
			WriteLines(Path.Combine(o.OutputDir, "profile.log"), pickups);
		}

		public void PickupProfileInYYC(Option o)
		{
			string[] removes = new string[] { "&amp;", "&nbsp;", "&gt;", "&lt;", "自己紹介",
				"-", "о", "_", ",", ".", "/", ":", "：", ";", "；", "･", "m", "^", "o", "(", ")", "*", "`", "'", "＼", "／", "♪", "?", "!" };
			Regex r = new Regex("<div class=\"pr-title\">(.*?)<div class=\"hope-title\">");
			Regex tag = new Regex("<[^>]*?>");
			Regex space = new Regex("[。\\s\r\n]+");
			Regex letter = new Regex("&#\\d+;");
			Regex tabs = new Regex("\t+");

			Dictionary<string, int> pickups = new Dictionary<string, int>();
			foreach (var path in Directory.GetFiles(o.InputDir, "*.xml", SearchOption.AllDirectories))
			{
				var context = File.ReadAllText(path, Encoding.GetEncoding("utf-8"));
				context = context.Replace("\r\n", "");
				Match match = r.Match(context);
				if (!match.Success) continue;

				string replaced = match.Groups[1].Value;
				replaced = tag.Replace(replaced, "\t");
				replaced = letter.Replace(replaced, "\t");
				replaced = space.Replace(replaced, "\t");
				foreach (var s in removes) replaced = replaced.Replace(s, "");
				if (replaced == "\t") continue;
				pickups[replaced] = 1;
				if (pickups.Count % 1000 == 0) Console.WriteLine("\tcount pickups=" + pickups.Count);
			}
			WriteLines(Path.Combine(o.OutputDir, "profile.log"), pickups.Keys.ToList());
		}

		public void PickupAgesInYYC(Option o)
		{
			Regex r = new Regex(@"(\d+)歳");
			Dictionary<string, int> table = new Dictionary<string, int>();
			foreach (var path in Directory.GetFiles(o.InputDir, "*.xml", SearchOption.AllDirectories))
			{
				var context = File.ReadAllText(path, Encoding.GetEncoding("utf-8"));
				context = context.Replace("\r\n", "");
				Match match = r.Match(context);
				if (!match.Success) continue;

				int age = 0;
				if (!int.TryParse(match.Groups[1].Value, out age)) continue;
				string name = Path.GetFileNameWithoutExtension(path);
				string[] cells = name.Split('_');
				table[cells[0]] = age;
			}
			Dictionary<int, int> ages = new Dictionary<int, int>();
			for (int i = 1; i < 100; i++) ages[i] = 0;
			foreach (var age in table.Values) if (ages.ContainsKey(age)) ages[age]++;
			List<string> lines = new List<string>();
			for (int i = 1; i < 100; i++) lines.Add("age=" + i + ",count=" + ages[i]);
			WriteLines(Path.Combine(o.OutputDir, "ages.csv"), lines);
		}
		private void WriteLines(string path, List<string> lines)
		{
			if (File.Exists(path)) File.Delete(path);
			File.WriteAllLines(path, lines);
		}

		public class Option
		{
			public string InputDir, OutputDir;
			public bool ReloadIpca;
			public string DictionaryPath;
			public string IpcaDir { get { return Path.Combine(OutputDir, "ipca"); } }
			public bool ReloadStatistics;
			public int Loop;
			public int PickupAxis;
			public double PickupLower, PickupUpper;
			public string DicDir;
		}

		public enum MorphemeType { OTHERS, NOUN /*名詞*/, VERB /*動詞*/, ADJECTIVE /*形容詞*/, PARTICLE /*助詞*/, ADVERB /*副詞*/, EXCLAMATION /*感嘆詞*/, START, END }
		private class Word
		{
			public MorphemeType Type;
			public int ID;
			public string Text;
			public int CountLine;
			public int CountWord;
			public override string ToString()
			{
				return ID + "\t" + Text + "\t" + Type + "\t" + CountWord + "\t" + CountLine;
			}
			public static Word Parse(string line)
			{
				string[] cells = line.Split('\t');
				if (cells.Length < 5) return null;
				var i = new Word();
				i.ID = int.Parse(cells[0]);
				i.Text = cells[1];
				i.Type = ParseEnum<MorphemeType>(cells[2]);
				i.CountWord = int.Parse(cells[3]);
				i.CountLine = int.Parse(cells[4]);
				return i;
			}
			public bool haveSense() { return Type == MorphemeType.NOUN || Type == MorphemeType.VERB || Type == MorphemeType.ADJECTIVE || Type == MorphemeType.ADVERB || Type == MorphemeType.EXCLAMATION; }
			public bool haveBorder() { return Type == MorphemeType.START || Type == MorphemeType.END; }
		}
		private static T ParseEnum<T>(string s)
		{
			foreach(T t in Enum.GetValues(typeof(T)))
			{
				if (t.ToString() == s) return t;
			}
			return default(T);
		}

		private class LearningIPCAForWords : LearningIPCA
		{
			public override LearningFrame FrameIn { get { return new LearningFrame(1, WORD_COUNT, 1); } }
			public override LearningFrame FrameOut { get { return new LearningFrame(1, 1, AXIS_MAX); } }
		}

		private class LearningWords : LearningImage
		{
			public LearningWords() : base(new LearningFrame(1, WORD_COUNT, 1))
			{
			}
		}

	}
}
