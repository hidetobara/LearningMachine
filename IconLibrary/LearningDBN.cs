using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

using Accord.Neuro;
using Accord.Neuro.Learning;
using AForge.Neuro.Learning;
using Accord.Neuro.Networks;


namespace IconLibrary
{
	/*
	 * Deep Brief Network
	 */
	public class LearningDBN : LearningUnit
	{
		const int MiddleCount = 64;
		const int Iterate = 3;

		public override LearningFrame FrameIn { get { return new LearningFrame() { Height = 16, Width = 16, Plane = 16 }; } }
		public override LearningFrame FrameOut { get { return new LearningFrame() { Height = 1, Width = 1, Plane = 16 }; } }

		public int Height { get { return FrameIn.Height; } }
		public int Width { get { return FrameIn.Width; } }
		public int Plane { get { return FrameIn.Plane; } }
		public int Length { get { return FrameIn.Length; } }

		protected DeepBeliefNetwork _Network;
		protected DeepBeliefNetworkLearning _Teacher;

		public override string Filename { get { return "DBN.bin"; } }

		public override void Initialize()
		{
			_Network = new DeepBeliefNetwork(Length, new int[] { MiddleCount, FrameOut.Length });
			new GaussianWeights(_Network).Randomize();
			_Network.UpdateVisibleWeights();
			_Teacher = new DeepBeliefNetworkLearning(_Network)
			{
				Algorithm = (h, v, i) => new ContrastiveDivergenceLearning(h, v)
			};
		}

		public override bool Load(string path)
		{
			if (!File.Exists(path)) return false;
			_Network = DeepBeliefNetwork.Load(path);
			_Teacher = new DeepBeliefNetworkLearning(_Network)
			{
				Algorithm = (h, v, i) => new ContrastiveDivergenceLearning(h, v)
			};
			return true;
		}

		public override void Save(string path)
		{
			string dir = System.IO.Path.GetDirectoryName(path);
			if (!System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);
			_Network.Save(path);
		}

		public override void Learn(List<LearningImage> images)
		{
			List<double[]> inputs = new List<double[]>();
			for (int i = 0; i < images.Count; i++) inputs.Add(images[i].Data);
			var data = _Teacher.GetLayerInput(inputs.ToArray());

			for (int i = 0; i < Iterate; i++) _Teacher.RunEpoch(data);
			_Network.UpdateVisibleWeights();
		}

		public override LearningImage Forecast(LearningImage image)
		{
			double[] data = _Network.Compute(image.Data);
			return new LearningImage(FrameOut, data);
		}
	}
}