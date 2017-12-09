using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using NMeCab;

namespace ConsoleAnalyzerText2
{
    public class Analyzer
    {
		/*
		 * 有意な品詞だけ取り出す
		 */
		public void CalclateWordClasses(Option o)
		{
			var tagger = PrepareTagger(o);
			Dictionary<string, Word> words = new Dictionary<string, Word>();

			foreach (var path in Directory.GetFiles(o.InputDir, "*.log"))
			{
				foreach (var user in File.ReadAllLines(path))
				{
					List<string> used = new List<string>();
					var node = tagger.ParseToNode(user);
					while(node != null)
					{
						var w = RetrieveWord(node);
						if (w != null)
						{
							if (!words.ContainsKey(w.Text)) words[w.Text] = w;
							words[w.Text].CountWord++;
							if (!used.Contains(w.Text)) used.Add(w.Text);
						}
						node = node.Next;
					}
					foreach (var text in used) words[text].CountUser++;
				}
			}

			int amount = 1;
			List<string> lines = new List<string>();
			foreach(var word in words.Values.OrderByDescending(w => w.CountWord))
			{
				word.ID = amount++;
				lines.Add(word.ToString());
			}
			File.AppendAllLines(Path.Combine(o.OutputDir, "words.csv"), lines);
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
				foreach (var line in File.ReadAllLines(path, Encoding.GetEncoding("shift_jis")))
				{
					Match match = r.Match(line);
					if (!match.Success) continue;

					string replaced = tag.Replace(match.Groups[1].Value, "\t");
					replaced = letter.Replace(replaced, "\t");
					replaced = space.Replace(replaced, "\t");
					replaced = replaced.Replace("ﾒﾓの登録", "");
					replaced = replaced.Replace("⇒", "");
					replaced = replaced.Replace("&nbsp;ﾒﾓ内容&nbsp;", "");
					replaced = tabs.Replace(replaced, "|");
					replaced = replaced.Trim();
					if (replaced.Length == 0) continue;
					pickups.Add(replaced);
				}
			}
			File.WriteAllLines(Path.Combine(o.OutputDir, "profile.log"), pickups.ToArray());
		}

		public class Option
		{
			public string InputDir, OutputDir;
		}

		private class Word
		{
			public MorphemeType Type;
			public int ID;
			public string Text;
			public int CountUser;
			public int CountWord;
			public override string ToString()
			{
				return ID + "\t" + Text + "\t" + Type + "\t" + CountWord + "\t" + CountUser;
			}
		}

		public enum MorphemeType { OTHERS, NOUN /*名詞*/, VERB /*動詞*/, ADJECTIVE /*形容詞*/, PARTICLE /*助詞*/, ADVERB /*副詞*/, EXCLAMATION /*感嘆詞*/, START, END }
	}
}
