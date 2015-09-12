using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;


namespace IconLibrary
{
	public class LearningIPCA_Slicing : LearningIPCA
	{
		public override int Height { get { return 16; } }
		public override int Width { get { return 16; } }
		public override int Plane { get { return 3; } }

		public override void Learn(List<LearningImage> images)
		{
			Size s = new Size(Width, Height);
			List<LearningImage> list = new List<LearningImage>();
			foreach (var i in images) list.AddRange(i.MakeSlices(s));
			base.Learn(list);
		}
	}
}
