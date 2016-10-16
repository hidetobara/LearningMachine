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
		// Dropout
		public int DropoutPadding = 0;
		public double DropoutRate = 0;
		public double NoiseRange = 0.1;

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
				// 水増し学習
				if (DropoutRate > 0)
				{
					for (int i = 0; i < DropoutPadding; i++)
					{
						dataIn.Add(p.In.DropOut(DropoutRate).Homogenize());
						dataOut.Add(p.Out.Homogenize());
					}
				}
			}
			var dataInPrepared = _Teacher.GetLayerInput(dataIn.ToArray());

			for (int i = 0; i < 500; i++)
			{
				_Teacher.RunEpoch(dataInPrepared, dataOut.ToArray());
				_Network.UpdateVisibleWeights();

				double amount = 0;
				int count = 0;
				for(int j = 0; j < dataIn.Count; j += DropoutPadding * 10)	// 適当に省いて評価
				{
					amount += TestCompute(dataIn[j], dataOut[j]);
					count++;
				}
				if (i % 10 == 0) Log.Instance.Info("[DNN.Learn]" + i + " " + (amount / count));
				if (amount / count < 0.05) break;
			}
		}

		private double TestCompute(double[] input, double[] output)
		{
			var tmpOut = _Network.Compute(input);
			var diff = Accord.Math.Matrix.Subtract(output, tmpOut);
			double lengthOut = Accord.Math.Norm.Euclidean(output);
			double lengthDiff = Accord.Math.Norm.Euclidean(diff);
			return lengthDiff / lengthOut;
		}

		public override LearningImage Project(LearningImage image)
		{
			double[] data = _Network.Compute(image.Homogenize());
			return new LearningImage(FrameOut, data);
		}
	}
}