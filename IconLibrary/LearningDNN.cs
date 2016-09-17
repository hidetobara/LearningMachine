using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Drawing;

using Accord.Neuro;
using Accord.Neuro.Learning;
using AForge.Neuro.Learning;
using Accord.Neuro.Networks;


namespace IconLibrary
{
	/*
	 * Deep Neural Network
	 */
	public class LearningDNN : LearningUnit
	{
		private int _MiddleCount = 64;

		private LearningFrame _FrameIn;
		private LearningFrame _FrameOut;
		public override LearningFrame FrameIn { get { return _FrameIn; } }
		public override LearningFrame FrameOut { get { return _FrameOut; } }

		protected DeepBeliefNetwork _Network;
		protected DeepNeuralNetworkLearning _Teacher;

		public override string Filename { get { return "DNN_" + FrameIn.Height + "." + FrameIn.Plane + "-" + FrameOut.Height + "." + FrameOut.Plane + ".bin"; } }

		public LearningDNN(int inHeight, int inPlane, int outHeight, int outPlane, int middle = 64)
		{
			_FrameIn = new LearningFrame() { Height = inHeight, Width = inHeight, Plane = inPlane };
			_FrameOut = new LearningFrame(){ Height = outHeight, Width = outHeight, Plane = outPlane };
			_MiddleCount = middle;
		}

		public override void Initialize()
		{
			_Network = new DeepBeliefNetwork(Length + 1, new int[] { _MiddleCount, FrameOut.Length + 1 });	// 斉次座標
			new GaussianWeights(_Network).Randomize();
			_Network.UpdateVisibleWeights();
			InitializeTeacher();
		}

		public override bool Load(string path)
		{
			if (!File.Exists(path)) return false;
			_Network = DeepBeliefNetwork.Load(path);
			InitializeTeacher();
			return true;
		}

		private void InitializeTeacher()
		{
			_Teacher = new DeepNeuralNetworkLearning(_Network)
			{
				Algorithm = (ann, i) => new ParallelResilientBackpropagationLearning(ann),
				LayerIndex = _Network.Machines.Count - 1,
			};
		}

		public override void Save(string path)
		{
			string dir = System.IO.Path.GetDirectoryName(path);
			if (!System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);
			_Network.Save(path);
		}

		public override LearningUnit.LearningStyle Style { get { return LearningStyle.InputOutput; } }
		public override void Learn(List<LearningImagePair> pairs, LearningStyle style)
		{
			Log.Instance.Info("[DNN.Learn]");
			List<double[]> dataIn = new List<double[]>();
			List<double[]> dataOut = new List<double[]>();
			foreach (var p in pairs)
			{
				dataIn.Add(p.In.Homogenize());
				dataOut.Add(p.Out.Homogenize());
			}
			var dataInPrepared = _Teacher.GetLayerInput(dataIn.ToArray());

			for (int i = 0; i < 200; i++)
			{
				_Teacher.RunEpoch(dataInPrepared, dataOut.ToArray());
				_Network.UpdateVisibleWeights();
				var tmpOut = _Network.Compute(dataIn[0]);
				var diff = Accord.Math.Matrix.Subtract(dataOut[0], tmpOut);
				double lengthOut = Accord.Math.Norm.Euclidean(dataOut[0]);
				double lengthDiff = Accord.Math.Norm.Euclidean(diff);
				if (i % 10 == 0) Log.Instance.Info("[DNN.Learn]" + i + " " + lengthDiff + "/" + lengthOut);
				if (lengthDiff / lengthOut < 0.05) break;
			}
		}

		public override LearningImage Project(LearningImage image)
		{
			double[] data = _Network.Compute(image.Homogenize());
			return new LearningImage(FrameOut, data);
		}
	}
}