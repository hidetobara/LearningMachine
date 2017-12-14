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
		/*
		 * 単語の頻度を計算する
		 */
		public void CalclateStatistics(Option o)
		{
			var dictionary = LoadDictionary(o.DictionaryPath);
			var tagger = PrepareTagger(o);
			var ipca = new LearningIPCAForWords();
			ipca.Initialize();
			int index = 0;
			int countMax = dictionary.Max(x => x.Value.CountLine);
			foreach (var p in Directory.GetFiles(o.InputDir, "*.log"))
			{
				foreach (var line in File.ReadLines(p))
				{
					var node = tagger.ParseToNode(line);
					var used = RetrieveWords(node);
					var image = new LearningWords();
					image.Data[0] = 1.0;
					foreach (var w in used)
					{
						if (!dictionary.ContainsKey(w.Text)) continue;
						var word = dictionary[w.Text];
						double importance = Math.Pow((double)countMax / (double)word.CountLine, 0.25);	// 要調整
						image.Data[word.ID] = importance * w.CountWord / used.Count;
					}
					ipca.Learn(new List<LearningImage>() { image });
					index++;
					if (index % 1000 == 0) Console.WriteLine("\tindex=" + index);
				}
			}
			ipca.Save(o.OutputDir);

			var invDictionary = new Dictionary<int, string>();
			foreach (var pair in dictionary) invDictionary.Add(pair.Value.ID, pair.Key);
			for(int main = 0; main < ipca.MainMax; main++)
			{
				var list = ipca.PickupTop(invDictionary, main);
				List<string> lines = new List<string>();
				foreach (var item in list) lines.Add(item.ToString());
				string path = Path.Combine(o.OutputDir, "main" + main + ".csv");
				File.WriteAllLines(path, lines);
			}
		}

		private Dictionary<string, Word> LoadDictionary(string path, int limit = 2)
		{
			Dictionary<string, Word> dictionary = new Dictionary<string, Word>();
			foreach(string line in File.ReadLines(path))
			{
				var w = Word.Parse(line);
				if (w == null) continue;
				if (w.CountLine <= limit) continue;
				dictionary[w.Text] = w;
			}
			return dictionary;
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
			WriteLines(Path.Combine(o.OutputDir, "words.csv"), lines);
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
			Regex r = new Regex("<table.*?自己紹介.*?</table>(.*)<table");
			Regex tag = new Regex("<[ :&/a-zA-Z0-9=#%\\?\\.\"\']*>");
			Regex space = new Regex("\\s+");
			Regex letter = new Regex("&#\\d+;");
			Regex tabs = new Regex("\t+");

			List<string> pickups = new List<string>();
			foreach (var path in Directory.GetFiles(o.InputDir, "*.xml"))
			{
				foreach (var line in File.ReadLines(path, Encoding.GetEncoding("shift_jis")))
				{
					Match match = r.Match(line);
					if (!match.Success) continue;

					string replaced = tag.Replace(match.Groups[1].Value, "\t");
					replaced = replaced.Replace('。', '\t');
					replaced = letter.Replace(replaced, "\t");
					replaced = space.Replace(replaced, "\t");
					replaced = replaced.Replace("ﾒﾓの登録", "");
					replaced = replaced.Replace("⇒", "");
					replaced = replaced.Replace("&nbsp;ﾒﾓ内容&nbsp;", "");
					string[] cells = replaced.Split('\t');
					foreach (string cell in cells)
					{
						var trimed = cell.Trim();
						if (trimed.Length <= 5) continue;
						pickups.Add(trimed);
					}
				}
			}
			WriteLines(Path.Combine(o.OutputDir, "profile.log"), pickups);
		}

		private void WriteLines(string path, List<string> lines)
		{
			if (File.Exists(path)) File.Delete(path);
			File.WriteAllLines(path, lines);
		}

		public class Option
		{
			public string InputDir, OutputDir;
			public string DictionaryPath;
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
			public override LearningFrame FrameIn { get { return new LearningFrame(1, 3000, 1); } }
			public override LearningFrame FrameOut { get { return new LearningFrame(1, 1, 16); } }
		}

		private class LearningWords : LearningImage
		{
			public LearningWords() : base(new LearningFrame(1, 3000, 1))
			{
			}
		}

	}
}
