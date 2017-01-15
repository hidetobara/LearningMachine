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
		public int EpochCount = 50;
		public int IterationCount = 10;
		// Dropout
		public int DropoutPadding = 0;
		public double DropoutRate = 0;
		public double NoiseRange = 0.1;
		// 参照設定
		public int OutputReference = 0;

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
			_FrameOut = new LearningFrame() { Height = outHeight, Width = outHeight, Plane = outPlane };
			_MiddleCount = middle;
		}

		public override void Initialize()
		{
			_Network = new DeepBeliefNetwork(Length + 1, new int[] { _MiddleCount, FrameOut.Length + 1 });  // 斉次座標
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

		public override bool Save(string path)
		{
			string dir = System.IO.Path.GetDirectoryName(path);
			if (!System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);
			_Network.Save(path);
			return true;
		}

		public override LearningUnit.LearningStyle Style { get { return LearningStyle.InputOutput; } }
		public override void Learn(List<LearningImagePair> pairs, LearningStyle style)
		{
			for(int e = 0; e < EpochCount; e++)
			{
				List<double[]> learnIn = new List<double[]>();
				List<double[]> learnOut = new List<double[]>();
				List<double[]> testIn = new List<double[]>();
				List<double[]> testOut = new List<double[]>();

				List<int> testList = GetRandomIndex(pairs.Count, pairs.Count / 5 + 1);
				for(int p = 0; p < pairs.Count; p++)
				{
					var pair = pairs[p];
					if (testList.Contains(p))
					{
						testIn.Add(pair.In.Homogenize());
						testOut.Add(pair.Out.Homogenize());
					}
					else
					{
						learnIn.Add(pair.In.Homogenize());
						learnOut.Add(pair.Out.Homogenize());
						// 水増し学習
						if (DropoutRate > 0)
						{
							for (int i = 0; i < DropoutPadding; i++)
							{
								learnIn.Add(pair.In.DropOut(DropoutRate).Homogenize());
								learnOut.Add(pair.Out.Homogenize());
							}
						}
					}
				}

				var dataInPrepared = _Teacher.GetLayerInput(learnIn.ToArray());
				for (int i = 0; i < IterationCount; i++)
				{
					_Teacher.RunEpoch(dataInPrepared, learnOut.ToArray());
					_Network.UpdateVisibleWeights();
				}

				double tested = 0;
				for(int t = 0; t < testIn.Count; t++) tested += TestCompute(testIn[t], testOut[t]);
				tested = tested / testIn.Count;
				if (e % 10 == 0) Log.Instance.Info("[DNN.Learn] epoch=" + e + " diff=" + tested);
				if (tested < 0.03) break;
			}
		}

		public override void Learn(LearningNodeGroup group)
		{
			List<LearningImagePair> pairs = new List<LearningImagePair>();
			List<LearningImage> SlotIn = group.Slots[0];
			List<LearningImage> SlotOut = group.Slots[OutputReference];
			for(int i = 0; i < SlotIn.Count; i++)
			{
				pairs.Add(new LearningImagePair(SlotIn[i], SlotOut[i]));
			}
			Learn(pairs, LearningStyle.InputOutput);
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

		private List<int> GetRandomIndex(int amount, int count)
		{
			int[] array = new int[amount];
			for (int i = 0; i < amount; i++) array[i] = i;
			return array.OrderBy(i => Guid.NewGuid()).Take(count).ToList();
		}
	}
}