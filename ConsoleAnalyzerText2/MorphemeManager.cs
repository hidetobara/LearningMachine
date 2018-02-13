using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using NMeCab;


namespace ConsoleAnalyzerText2
{
	/*
	 * 形態素解析のラッパークラス
	 */
	public class MorphemeManager
	{
		MeCabTagger _Tagger;

		private static MorphemeManager _Instance;
		public static MorphemeManager Instance
		{
			get
			{
				if (_Instance == null) _Instance = new MorphemeManager();
				return _Instance;
			}
		}
		private MorphemeManager() { }

		public MorphemeManager Initialize(string dir)
		{
			if (_Tagger != null) return this;

			MeCabParam p = new MeCabParam();
			p.LatticeLevel = MeCabLatticeLevel.Zero;
			p.OutputFormatType = "lattice";
			p.AllMorphs = false;
			p.Partial = false;
			p.DicDir = dir;
			_Tagger = MeCabTagger.Create(p);
			return this;
		}

		private List<string> _Codes = new List<string>() { ")", "(", "\"" };

		public List<Word> Parse(string input, bool removeAlphabets = true)
		{
			if (removeAlphabets) input = Regex.Replace(input, "[ @_a-zA-Z0-9!:/#\\.\\-]+", "");

			List<Word> list = new List<Word>() { new Word(MorphemeType.START) };
			MeCabNode node = _Tagger.ParseToNode(input);
			node = node.Next;
			while(node != null)
			{
				if (node.Length > 0 && node.Feature != null)
				{
					if(_Codes.Contains(node.Surface))
					{
						list.Add(new Word(MorphemeType.OTHERS, node.Surface));
					}
					else if (node.Feature.StartsWith("名詞"))
					{
						list.Add(new Word(MorphemeType.NOUN, node.Surface));
					}
					else if (node.Feature.StartsWith("動詞"))
					{
						string[] cells = node.Feature.Split(',');
						list.Add(new Word(MorphemeType.VERB, cells.Length > 6 ? cells[6] : node.Surface, node.Surface));
					}
					else if (node.Feature.StartsWith("副詞"))
					{
						string[] cells = node.Feature.Split(',');
						list.Add(new Word(MorphemeType.ADVERB, cells.Length > 6 ? cells[6] : node.Surface, node.Surface));
					}
					else if (node.Feature.StartsWith("形容詞") || node.Feature.StartsWith("形容動詞"))
					{
						string[] cells = node.Feature.Split(',');
						list.Add(new Word(MorphemeType.ADJECTIVE, cells.Length > 6 ? cells[6] : node.Surface, node.Surface));
					}
					else if (node.Feature.StartsWith("助詞"))
					{
						string[] cells = node.Feature.Split(',');
						list.Add(new Word(MorphemeType.PARTICLE, cells.Length > 6 ? cells[6] : node.Surface, node.Surface));
					}
					else if (node.Feature.StartsWith("感動詞"))
					{
						string[] cells = node.Feature.Split(',');
						list.Add(new Word(MorphemeType.SENSITIVE, cells.Length > 6 ? cells[6] : node.Surface, node.Surface));
					}
					else
					{
						list.Add(new Word(MorphemeType.OTHERS, node.Surface));
					}
				}
				node = node.Next;	// なぜか例外が起きる
			}
			list.Add(new Word(MorphemeType.END));
			return list;
		}

		public enum MorphemeType { NOUN /*名詞*/, VERB /*動詞*/, ADJECTIVE /*形容詞*/, PARTICLE /*助詞*/, ADVERB /*副詞*/, SENSITIVE /* 感動詞 */, OTHERS, START, END }
		public class Word
		{
			public int ID;
			public int CountLine;
			public int CountWord;
			public MorphemeType Type;
			public string Origin;
			public string Variation;
			public Word(MorphemeType t) { Type = t; }
			public Word(MorphemeType t, string origin) { Type = t; Origin = origin; Variation = origin; }
			public Word(MorphemeType t, string origin, string variation) { Type = t; Origin = origin; Variation = variation; }

			public bool IsTerminal() { return Type == MorphemeType.START || Type == MorphemeType.END; }
			public bool IsUsefull()
			{
				switch(Type)
				{
					case MorphemeType.NOUN:
					case MorphemeType.VERB:
					case MorphemeType.ADJECTIVE:
					case MorphemeType.ADVERB:
					case MorphemeType.SENSITIVE:
						return true;
					default:
						return false;
				}
			}

			public override string ToString()
			{
				return ID + "\t" + Origin + "\t" + Type + "\t" + CountWord + "\t" + CountLine;
			}
			public static Word Parse(string line)
			{
				string[] cells = line.Split('\t');
				if (cells.Length < 5) return null;
				var i = new Word(ParseEnum<MorphemeType>(cells[2]));
				i.ID = int.Parse(cells[0]);
				i.Origin = cells[1];
				i.CountWord = int.Parse(cells[3]);
				i.CountLine = int.Parse(cells[4]);
				return i;
			}
			private static T ParseEnum<T>(string s)
			{
				foreach (T t in Enum.GetValues(typeof(T)))
				{
					if (t.ToString() == s) return t;
				}
				return default(T);
			}
		}

		/*
		 * 頻度入り辞書
		 */
		private class WordsDictionary
		{
			const int Length = 5000;
			private Dictionary<string, Word> _Dictionary;
			public int CountMax { get; private set; }

			public Word Get(string s) { if (!_Dictionary.ContainsKey(s)) return null; return _Dictionary[s]; }

			/*
			 * 単語列からベクトル化する
			 * 1. 短い文章は何も内容が無いということで、分類されたくない→文章の個数の平方根
			 * 2. 頻度が低い単語の優先度を上げたいLog(x, 2.0)に
			 */
			public double[] WordsToImage(List<Word> words)
			{
				var data = new double[Length];
				data[0] = 1.0;
				foreach (var w in words)
				{
					if (!_Dictionary.ContainsKey(w.Origin)) continue;
					var word = _Dictionary[w.Origin];
					//					double importance = Math.Log((double)(CountMax + 1) / (double)word.CountLine, Math.E); // 本来
					double importance = Math.Log((double)(CountMax + 1) / (double)word.CountLine, 2.0); // 頻度の高い単語は無視したい
																										//					image.Data[word.ID] = importance * w.CountWord / words.Count;   // 本来
					data[word.ID] = importance * w.CountWord / Math.Sqrt(words.Count);    // 単語の少ない文章は無視したい
				}
				return data;
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
					i._Dictionary[w.Origin] = w;
				}
				i.CountMax = i._Dictionary.Max(x => x.Value.CountLine);
				return i;
			}
		}

	}
}