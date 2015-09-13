using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;


namespace IconLibrary
{
	public class LearningIPCA_Slicing_3to32 : LearningIPCA
	{
		public override LearningFrame FrameIn { get { return new LearningFrame() { Height = 8, Width = 8, Plane = 3 }; } }
		public override LearningFrame FrameOut { get { return new LearningFrame() { Height = 1, Width = 1, Plane = 32 }; } }
		public override string Filename { get { return "IPCA_3to32/"; } }

		public override void Learn(List<LearningImage> images)
		{
			Size s = new Size(Width, Height);
			List<LearningImage> list = new List<LearningImage>();
			foreach (var i in images) list.AddRange(i.MakeSlices(s));
			base.Learn(list);
		}
	}

	public class LearningIPCA_Slicing_32to32 : LearningIPCA
	{
		public override LearningFrame FrameIn { get { return new LearningFrame() { Height = 8, Width = 8, Plane = 32 }; } }
		public override LearningFrame FrameOut { get { return new LearningFrame() { Height = 1, Width = 1, Plane = 32 }; } }
		public override string Filename { get { return "IPCA_32to32/"; } }

		public override void Learn(List<LearningImage> images)
		{
			Size s = new Size(Width, Height);
			List<LearningImage> list = new List<LearningImage>();
			foreach (var i in images) list.AddRange(i.MakeSlices(s));
			base.Learn(list);
		}
	}
}
