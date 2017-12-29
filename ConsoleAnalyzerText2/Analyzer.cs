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
		const int WORD_COUNT = 7000;
		const int CUT_LIMIT = 3;
		const int TOP_COUNT = 30;
		readonly string[] IGNORES = new string[] {"/", ":", "･", ",", "&nbsp;", "なし" };

		/*
		 * 逆投影によって、パターン物とそうでないものに分類できないか
		 * 逆投影後、元から大きくずれる物はそれだけパターンでなく、ずれないものはパターン物つまり業者になるのでは
		 */
		public void GroupByDifference(Option o)
		{
			var dictionary = WordsDictionary.Load(o.DictionaryPath, CUT_LIMIT);
			var tagger = PrepareTagger(o);
			var ipca = new LearningIPCAForWords();
			ipca.Load(o.IpcaDir);

			TopList[] tops = new TopList[AXIS_MAX];
			TopList[] bottoms = new TopList[AXIS_MAX];
			for (int a = 0; a < AXIS_MAX; a++)
			{
				tops[a] = new TopList(a, TOP_COUNT);
				bottoms[a] = new TopList(-a, TOP_COUNT);
			}

			int index = 0;
			foreach (var p in Directory.GetFiles(o.InputDir, "*.log"))
			{
				foreach (var line in File.ReadLines(p))
				{
					var node = tagger.ParseToNode(line);
					var used = RetrieveWords(node);
					var image = dictionary.WordsToImage(used);
					var vetor = ipca.Forecast(image);
					var reimaged = ipca.BackForecast(vetor);
					LearningImage.Sub(image, reimaged, reimaged);
					double distance = LearningImage.EuclideanLength(reimaged);

					var maxIdx = vetor.Data.Select((val, idx) => new { V = val, I = idx }).Aggregate((max, working) => (max.V > working.V) ? max : working).I;
					var context = line.Trim();

					tops[maxIdx].Add(context, distance);
					bottoms[maxIdx].Add(context, -distance);
					index++;
					if (index % 1000 == 0) Console.WriteLine("\tindex=" + index);
				}
			}

			List<string> lines = new List<string>();
			for (int a = 0; a < AXIS_MAX; a++)
			{
				lines.Add("");
				lines.AddRange(tops[a].ToList());
				lines.Add("");
				lines.AddRange(bottoms[a].ToList());
			}
			WriteLines(Path.Combine(o.OutputDir, "group_by_difference.csv"), lines);
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

			// 平均と偏差
			Statistics statitcs = new Statistics();
			if (o.ReloadStatistics) statitcs.Load(Path.Combine(o.OutputDir, "statistics.csv"));
			if (!statitcs.IsCalclated)
			{
				foreach (var p in Directory.GetFiles(o.InputDir, "*.log"))
				{
					foreach (var line in File.ReadLines(p))
					{
						var node = tagger.ParseToNode(line);
						var used = RetrieveWords(node);
						var image = dictionary.WordsToImage(used);
						var vetor = ipca.Project(image);
						statitcs.Add(vetor.Data);
						index++;
						if (index % 1000 == 0) Console.WriteLine("\tindex=" + index);
					}
				}
				statitcs.Calclate(Path.Combine(o.OutputDir, "statistics.csv"));
			}

			// 偏差によってならした値でグループ化
			index = 0;
			TopList[] tops = new TopList[AXIS_MAX];
			for (int a = 0; a < AXIS_MAX; a++) tops[a] = new TopList(a, TOP_COUNT);
			LearningImage deviator = statitcs.GenerateDeviator();

			StreamWriter writer = new StreamWriter(Path.Combine(o.OutputDir, "profile_grouped.csv"));
			foreach (var p in Directory.GetFiles(o.InputDir, "*.log"))
			{
				foreach (var line in File.ReadLines(p))
				{
					var node = tagger.ParseToNode(line);
					var used = RetrieveWords(node);
					var image = dictionary.WordsToImage(used);
					var vetor = ipca.Project(image);
					var corrected = new LearningImage(1, AXIS_MAX, 1);
					LearningImage.Multiply(vetor, deviator, corrected);
					var maxIdx = corrected.Data.Select((val, idx) => new { V = val, I = idx }).Aggregate((max, working) => (max.V > working.V) ? max : working).I;
					var maxVal = corrected.Data[maxIdx];
					var context = line.Trim();
					writer.WriteLine(context + "\t" + maxIdx + "\t" + maxVal.ToString("F4"));

					tops[maxIdx].Add(context, maxVal);
					index++;
					if (index % 1000 == 0) Console.WriteLine("\tindex=" + index);
				}
			}
			writer.Close();

			List<string> lines = new List<string>();
			for (int a = 0; a < AXIS_MAX; a++)
			{
				if (a != 0) lines.Add("");
				lines.AddRange(tops[a].ToList());
			}
			WriteLines(Path.Combine(o.OutputDir, "group_by_statistics.csv"), lines);
			Console.WriteLine("grouped.");
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

			public LearningImage GenerateDeviator()
			{
				LearningImage deviator = new LearningImage(1, AXIS_MAX, 1);
				deviator.Data[0] = 0;
				for (int a = 1; a < AXIS_MAX; a++) deviator.Data[a] = 1 / this.Deviations[a];
				return deviator;
			}
		}

		private double[] LoadDistribution(string path)
		{
			double[] distributions = new double[AXIS_MAX];
			for (int a = 0; a < AXIS_MAX; a++) distributions[a] = 1;

			var lines = File.ReadAllLines(path);
			if (lines.Length < AXIS_MAX) return distributions;

			for(int r = 0; r < lines.Length; r++)
			{
				string[] cells = lines[r].Split('\t');
				if (cells.Length < 4) continue;
				double.TryParse(cells[3], out distributions[r]);
			}
			return distributions;
		}

		private class TopList
		{
			int _Axis;
			int _Max = 30;
			int _Count;
			Dictionary<string, double> _Table = new Dictionary<string, double>();

			public TopList(int axis, int max)
			{
				_Axis = axis;
				_Max = max;
			}
			public void Add(string w, double v)
			{
				_Count++;
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
				List<string> list = new List<string>() { "Axis=" + _Axis.ToString("D2") + "\tCount=" + _Count };
				foreach (var p in _Table.OrderByDescending(p => p.Value)) list.Add(p.Value.ToString("F4") + "\t" + p.Key);
				return list;
			}
		}

		/*
		 * 単語の頻度を計算する
		 */
		public void CalclateFrequency(Option o)
		{
			var dictionary = WordsDictionary.Load(o.DictionaryPath, CUT_LIMIT);
			var tagger = PrepareTagger(o);
			var ipca = new LearningIPCAForWords();
			if (o.ReloadIpca) ipca.Load(o.IpcaDir); else ipca.Initialize();
			int index = 0;
			foreach (var p in Directory.GetFiles(o.InputDir, "*.log"))
			{
				foreach (var line in File.ReadLines(p))
				{
					var node = tagger.ParseToNode(line);
					var used = RetrieveWords(node);
					var image = dictionary.WordsToImage(used);
					ipca.Learn(new List<LearningImage>() { image });
					index++;
					if (index % 1000 == 0) Console.WriteLine("\tindex=" + index);
				}
			}
			ipca.Save(o.IpcaDir);

			var indexDictionary = dictionary.ToIndexDictionary();
			for(int main = 0; main < ipca.MainMax; main++)
			{
				var list = PickupTop(ipca, indexDictionary, main, 50);
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
			private Dictionary<string, Word> _Dictionary;
			public int CountMax { get; private set; }

			public Word Get(string s) { if (!_Dictionary.ContainsKey(s)) return null; return _Dictionary[s]; }

			/*
			 * 単語列からベクトル化する
			 * 1. 短い文章は何も内容が無いということで、分類されたくない→文章の個数の平方根
			 * 2. 頻度が低い単語の優先度を上げたいLog(x, 2.0)に
			 */
			public LearningWords WordsToImage(List<Word> words)
			{
				var image = new LearningWords();
				image.Data[0] = 1.0;
				foreach (var w in words)
				{
					if (!_Dictionary.ContainsKey(w.Text)) continue;
					var word = _Dictionary[w.Text];
					double importance = Math.Log((double)(CountMax + 1) / (double)word.CountLine, 2.0); // 要調整
					image.Data[word.ID] = importance * w.CountWord / Math.Sqrt(words.Count);	// 平方根の方がいいかも
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
			Dictionary<string, Word> words = new Dictionary<string, Word>();

			foreach (var p in Directory.GetFiles(o.InputDir, "*.log"))
			{
				foreach (var user in File.ReadLines(p))
				{
					var node = tagger.ParseToNode(user);
					var used = RetrieveWords(node);
					foreach (var w in used)
					{
						if (!words.ContainsKey(w.Text)) words[w.Text] = w;
						else words[w.Text].CountWord += w.CountWord;
						words[w.Text].CountLine++;
					}
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

		private MeCabTagger PrepareTagger(Option o)
		{
			MeCabParam p = new MeCabParam();
			p.LatticeLevel = MeCabLatticeLevel.Zero;
			p.OutputFormatType = "lattice";
			p.AllMorphs = false;
			p.Partial = false;
			p.DicDir = "C:/obara/Library/NMeCab0.07/dic/ipadic";
			return MeCabTagger.Create(p);
		}

		private Word RetrieveWord(MeCabNode n)
		{
			if (n.CharType == 0) return null;
			if (IGNORES.Contains(n.Surface)) return null;

			if (n.Feature.StartsWith("名詞"))
			{
				return new Word() { Type = MorphemeType.NOUN, Text = n.Surface };
			}
			if (n.Feature.StartsWith("動詞"))
			{
				string[] cells = n.Feature.Split(',');
				return new Word() { Type = MorphemeType.VERB, Text = cells[6] };
			}
			if (n.Feature.StartsWith("形容詞") || n.Feature.StartsWith("形容動詞"))
			{
				string[] cells = n.Feature.Split(',');
				return new Word() { Type = MorphemeType.ADJECTIVE, Text = cells[6] };
			}
			if (n.Feature.StartsWith("副詞"))
			{
				string[] cells = n.Feature.Split(',');
				return new Word() { Type = MorphemeType.ADVERB, Text = cells[6] };
			}
			if (n.Feature.StartsWith("感動詞"))
			{
				string[] cells = n.Feature.Split(',');
				return new Word() { Type = MorphemeType.EXCLAMATION, Text = cells[6] };
			}
			return null;
		}

		private List<Word> RetrieveWords(MeCabNode node)
		{
			Dictionary<string, Word> used = new Dictionary<string, Word>();
			while (node != null)
			{
				var w = RetrieveWord(node);
				if (w != null)
				{
					if (!used.ContainsKey(w.Text)) used[w.Text] = w;
					used[w.Text].CountWord++;
				}
				node = node.Next;
			}
			return new List<Word>(used.Values);
		}

		/*
		 * プロフィールだけを抜き出す
		 */
		public void PickupProfileInHappyMail(Option o)
		{
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
				}
			}
			WriteLines(Path.Combine(o.OutputDir, "profile.log"), pickups);
			Console.WriteLine("pickuped profiles.");
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
