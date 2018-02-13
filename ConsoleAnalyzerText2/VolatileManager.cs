using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using IconLibrary;
using MiniJSON;

namespace ConsoleAnalyzerText2
{
	using Hash = Dictionary<string, object>;

	public class VolatileManager
	{
		const int START = -1;
		const int END = -2;

		private List<string> _Ignores = new List<string>() { "、", "。", "「", "」", "(", ")", "[", "]", "{", "}" };

		private string _NetworkDir;
		private string _Name;

		private Dictionary<string, int> _TextTable = new Dictionary<string ,int>();
		private Dictionary<int, Flow> _FlowMatrix = new Dictionary<int, Flow>();

		public VolatileManager(string networkDir, string name)
		{
			_NetworkDir = networkDir;
			_Name = name;
		}

		public void LearningMatrix(string inputDir)
		{
			foreach(var path in Directory.GetFiles(inputDir, "*.log"))
			{
				foreach(string line in File.ReadLines(path))
				{
					var cells = line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
					foreach (var cell in cells) Learn(cell);
				}
			}
		}

		public void Load()
		{
			LoadJson(JsonPath);
		}
		public void Save()
		{
			SaveJson(JsonPath);
		}

		private string TextsPath { get { return Path.Combine(_NetworkDir, _Name + ".txt"); } }
		private string MatrixPath { get { return Path.Combine(_NetworkDir, _Name + ".mtx"); } }
		private string JsonPath { get { return Path.Combine(_NetworkDir, _Name + ".flow.json"); } }

		private bool LoadTexts(string path)
		{
			if (!File.Exists(path)) return false;

			using(StreamReader reader = new StreamReader(path))
			{
				_TextTable.Clear();
				while(!reader.EndOfStream)
				{
					string line = reader.ReadLine();
					string[] cells = line.Split('\t');
					if (cells.Length < 2) continue;
					int count;
					if (string.IsNullOrEmpty(cells[1]) || !int.TryParse(cells[1], out count)) continue;
					_TextTable[cells[0]] = count;
				}
			}
			return true;
		}
		private bool LoadMatrix(string path)
		{
			if (!File.Exists(path)) return false;

			using (StreamReader reader = new StreamReader(path))
			{
				_FlowMatrix.Clear();
				while (!reader.EndOfStream)
				{
					string line = reader.ReadLine();
					string[] cells = line.Split('\t');
					if (cells.Length < 2) continue;
					int num;
					if (!int.TryParse(cells[0], out num)) continue;
					Flow flow = new Flow();
					flow.Retrieve(cells, 1);
					_FlowMatrix[num] = flow;
				}
			}
			return true;
		}
		private bool SaveTexts(string path)
		{
			using(StreamWriter writer = new StreamWriter(path))
			{
				foreach (var pair in _TextTable) writer.WriteLine(pair.Key + "\t" + pair.Value);
				if (writer != null) writer.Close();
			}
			return true;
		}
		private bool SaveMatrix(string path)
		{
			using(StreamWriter writer = new StreamWriter(path))
			{
				foreach(var pair in _FlowMatrix)
				{
					writer.WriteLine(pair.Key + pair.Value.ToString());
				}
				if (writer != null) writer.Close();
			}
			return true;
		}

		private bool LoadJson(string path)
		{
			if (!File.Exists(path)) return false;

			using (StreamReader reader = new StreamReader(path))
			{
				_TextTable.Clear();
				_FlowMatrix.Clear();
				while (!reader.EndOfStream)
				{
					string line = reader.ReadLine();
					Hash hash = MiniJSON.Json.Deserialize(line) as Hash;
					int id;
					if (!int.TryParse(hash["id"].ToString(), out id)) continue;
					if (hash.ContainsKey("text")) { _TextTable[hash["text"].ToString()] = id; }
					_FlowMatrix[id] = Flow.Retrieve(hash);
				}
			}
			return true;

		}
		private bool SaveJson(string path)
		{
			Dictionary<int, string> reverse = new Dictionary<int, string>();
			foreach (var pair in _TextTable) reverse[pair.Value] = pair.Key;

			using (StreamWriter writer = new StreamWriter(path))
			{
				foreach (var pair in _FlowMatrix)
				{
					Hash hash = new Hash();
					hash["id"] = pair.Key;
					if (reverse.ContainsKey(pair.Key)) hash["text"] = reverse[pair.Key];
					hash["flow"] = pair.Value.Texts;
					writer.WriteLine(MiniJSON.Json.Serialize(hash));
				}
				if (writer != null) writer.Close();
			}
			return true;
		}

		public bool Learn(string input)
		{
			if (string.IsNullOrWhiteSpace(input)) return false;

			List<string> blocks = new List<string>();
			MorphemeManager.MorphemeType previousType = MorphemeManager.MorphemeType.OTHERS;
			string block = "";
			foreach (var item in MorphemeManager.Instance.Parse(input))
			{
				if (item.Origin == null || _Ignores.Contains(item.Origin)) continue;

				switch (item.Type)
				{
					case MorphemeManager.MorphemeType.PARTICLE:
					case MorphemeManager.MorphemeType.ADJECTIVE:
					case MorphemeManager.MorphemeType.ADVERB:
						block += item.Variation;
						blocks.Add(block);
						block = "";
						break;
					case MorphemeManager.MorphemeType.NOUN:
						if (previousType == MorphemeManager.MorphemeType.NOUN) { blocks.Add(block); block = ""; }
						block += item.Variation;
						break;
					default:
						block += item.Variation;
						break;

				}
				previousType = item.Type;
			}
			if (!string.IsNullOrEmpty(block)) blocks.Add(block);
			if (blocks.Count == 0) return false;

			int previous = START;
			foreach(var b in blocks)
			{
				if (!_TextTable.ContainsKey(b)) _TextTable[b] = _TextTable.Count;
				int number = _TextTable[b];
				if (!_FlowMatrix.ContainsKey(previous)) _FlowMatrix[previous] = new Flow();
				Flow f = _FlowMatrix[previous];
				if (!f.Texts.ContainsKey(number)) f.Texts[number] = 1; else f.Texts[number]++;
				previous = number;
			}
			if (!_FlowMatrix.ContainsKey(previous)) _FlowMatrix[previous] = new Flow();
			Flow fend = _FlowMatrix[previous];
			if (!fend.Texts.ContainsKey(END)) fend.Texts[END] = 1; else fend.Texts[END]++;
			return true;
		}

		public string Generate(int textLimit = 128, int useLimit = 3)
		{
			Dictionary<int, string> reverse = new Dictionary<int, string>();
			foreach (var pair in _TextTable) reverse[pair.Value] = pair.Key;

			int index = START;
			int next = START;
			Dictionary<int, int> used = new Dictionary<int, int>();
			string result = "";
			while(index != END)
			{
				if (reverse.ContainsKey(index)) result += reverse[index];
				if (!used.ContainsKey(index)) used[index] = 1; else used[index]++;
				if (used[index] > useLimit) break;
				if (result.Length > textLimit) break;
				next = _FlowMatrix[index].Invoke();				
				index = next;
			}
			Log.Instance.Info("Generate(): " + result);
			return result;
		}

		class Flow
		{
			public Dictionary<int, int> Texts = new Dictionary<int, int>();
			public override string ToString()
			{
				StringBuilder builder = new StringBuilder();
				foreach (var kv in Texts) builder.Append("\t" + kv.Key + "=" + kv.Value);
				return builder.ToString();
			}
			public void Retrieve(string[] cells, int offset)
			{
				for(int i = offset; i < cells.Length; i++)
				{
					string[] kv = cells[i].Split('=');
					if (kv.Length != 2) continue;
					int key, value;
					if (int.TryParse(kv[0], out key) && int.TryParse(kv[1], out value)) Texts[key] = value;
				}
			}
			public static Flow Retrieve(Hash hash)
			{
				Flow i = new Flow();
				if (!hash.ContainsKey("flow")) return null;
				foreach(var pair in hash["flow"] as Hash)
				{
					int key, value;
					if (int.TryParse(pair.Key, out key) && int.TryParse(pair.Value.ToString(), out value)) i.Texts[key] = value;
				}
				return i;
			}
			public int Invoke()
			{
				int amount = Texts.Sum(x => x.Value);
				Random r = new Random();
				int value = r.Next(0, amount);
				int sum = 0;
				foreach(var pair in Texts)
				{
					sum += pair.Value;
					if (value <= sum) return pair.Key;
				}
				return END;
			}
		}
	}
}
