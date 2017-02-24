using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IconLibrary
{
	class LearningNormalize : LearningUnit
	{
		public override string Filename { get { return "Normailize.bin"; } }
		public override bool Load(string path) { return true; }

		public override LearningImage Project(LearningImage image)
		{
			LearningImage o = new LearningImage(image);
			double[] maxs = new double[image.Plane];
			for (int h = 0; h < o.Height; h++)
			{
				for (int w = 0; w < o.Width; w++)
				{
					for (int p = 0; p < o.Plane; p++)
					{
						double v = o.Data[(o.Width * h + w) * o.Plane + p];
						if (maxs[p] < Math.Abs(v)) maxs[p] = Math.Abs(v);
					}
				}
			}

			for (int h = 0; h < o.Height; h++)
			{
				for (int w = 0; w < o.Width; w++)
				{
					for (int p = 0; p < o.Plane; p++)
					{
						if (maxs[p] == 0) continue;
						o.Data[(o.Width * h + w) * o.Plane + p] /= maxs[p];
					}
				}
			}
			return o;
		}
	}

	class LearningSigmoid : LearningUnit
	{
		const double GAIN = 10.0;
		public override string Filename { get { return "Sigmoid.bin"; } }
		public override bool Load(string path) { return true; }

		public override LearningImage Project(LearningImage image)
		{
			LearningImage o = new LearningImage(image);
			double[] maxs = new double[image.Plane];
			for (int h = 0; h < o.Height; h++)
			{
				for (int w = 0; w < o.Width; w++)
				{
					for (int p = 0; p < o.Plane; p++)
					{
						double v = o.Data[(o.Width * h + w) * o.Plane + p];
						if (maxs[p] < Math.Abs(v)) maxs[p] = Math.Abs(v);
					}
				}
			}

			for (int h = 0; h < o.Height; h++)
			{
				for (int w = 0; w < o.Width; w++)
				{
					for (int p = 0; p < o.Plane; p++)
					{
						if (maxs[p] == 0) continue;
						int i = (o.Width * h + w) * o.Plane + p;
						double x = o.Data[i] * GAIN / maxs[p];
						o.Data[i] = 1.0 / (1.0 + Math.Exp(-x));
					}
				}
			}
			return o;
		}
	}

}
