using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterDesktop
{
	class CategoryManager
	{
		Dictionary<string, int> _Table = new Dictionary<string, int>();

		public CategoryManager()
		{
			_Table["漫画"] = 1;
			_Table["マンガ"] = 1;
			_Table["アニメ"] = 2;
			_Table["ゲーム"] = 3;

			_Table["学生"] = 4;
			_Table["中学"] = 4;
			_Table["高校"] = 4;
			_Table["大学"] = 4;
			_Table["中学"] = 5;
			_Table["高校"] = 6;
			_Table["大学"] = 7;

			_Table["男"] = 8;
			_Table["女"] = 9;
		}

		public List<int> GetCategories(string text)
		{
			List<int> list = new List<int>();
			if (string.IsNullOrEmpty(text)) return list;

			foreach(var pair in _Table)
			{
				if (text.Contains(pair.Key)) list.Add(pair.Value);
			}
			return list.Distinct().ToList();
		}
	}
}
