using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IconLibrary
{
	public class Log
	{
		private static Log _Instance;
		public static Log Instance
		{
			get { if (_Instance == null) _Instance = new Log(); return _Instance; }
		}

		public bool Updated { private set; get; }
		public int Limit = 50;
		private void Cut() { while (_Strips.Count >= Limit) _Strips.RemoveAt(0); }

		private List<Strip> _Strips = new List<Strip>();
		public void Clear() { lock (_Strips) { _Strips.Clear(); } Updated = true; }
		public void Info(string msg) { lock (_Strips) { _Strips.Add(new Strip() { Type = StripType.INFO, Message = msg }); Cut(); } Updated = true; }
		public void Error(string msg) { lock (_Strips) { _Strips.Add(new Strip() { Type = StripType.ERROR, Message = msg }); Cut(); } Updated = true; }
		public string Get(StripType t = StripType.NONE)
		{
			lock (_Strips)
			{
				List<string> list = new List<string>();
				foreach (var s in _Strips)
				{
					if (t == StripType.NONE || t == s.Type) list.Add(s.Message);
				}
				Updated = false;
				return string.Join(Environment.NewLine, list);
			}
		}

		public enum StripType { NONE, INFO, ERROR }
		struct Strip
		{
			public StripType Type;
			public string Message;
		}
	}
}
