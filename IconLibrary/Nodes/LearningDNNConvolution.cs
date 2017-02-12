﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

using Accord.Neuro;
using Accord.Neuro.Learning;
using Accord.Neuro.Networks;


namespace IconLibrary
{
	public class LearningDNNConvolution : LearningNode
	{
		private int _MiddleCount = 64;
		private const int SampleLimit = 10000;

		public int EpochCount = 1000;
		public int IterationCount = 50;
		public int OutputReference = 0;
		public int BlockSize = 4;

		private LearningFrame _FrameIn;
		private LearningFrame _FrameOut;
		public override LearningFrame FrameIn { get { return _FrameIn; } }
		public override LearningFrame FrameOut { get { return _FrameOut; } }

		protected DeepBeliefNetwork _Network;
		protected DeepNeuralNetworkLearning _InnerTeacher;

		public LearningNodeConnection OuterTeacher = null;

		public override string Filename { get { return "DNNC_" + FrameIn.Height + "." + FrameIn.Plane + "-" + FrameOut.Height + "." + FrameOut.Plane + ".bin"; } }

		public LearningDNNConvolution(int height, int inPlane, int outPlane, int middle = 64)
		{
			_FrameIn = new LearningFrame() { Height = height, Width = height, Plane = inPlane };
			_FrameOut = new LearningFrame() { Height = height, Width = height, Plane = outPlane };
			_MiddleCount = middle;
#if DEBUG
			EpochCount = 50;
			IterationCount = 20;
#endif
		}

		public override void Initialize()
		{
			_Network = new DeepBeliefNetwork(BlockSize * BlockSize * FrameIn.Plane + 1, new int[] { _MiddleCount, FrameOut.Plane + 1 });  // 斉次座標
			new GaussianWeights(_Network).Randomize();
			_Network.UpdateVisibleWeights();
			InitializeTeacher();

			if (OuterTeacher != null) OuterTeacher.Initialize();
		}

		public override bool Load(string path)
		{
			if (!File.Exists(path)) return false;
			_Network = DeepBeliefNetwork.Load(path);
			InitializeTeacher();
			return true;
		}

		protected void InitializeTeacher()
		{
			_InnerTeacher = new DeepNeuralNetworkLearning(_Network)
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

		public override void Learn(LearningNodeGroup group)
		{
			List<double[]> sampleIn = new List<double[]>();
			List<double[]> sampleOut = new List<double[]>();
			for (int i = 0; i < group.Slots[0].Count; i++)
			{
				var pairs = PickupBlocks(group.Slots[0][i], group.Slots[OutputReference][i]);
				foreach (var pair in pairs)
				{
					sampleIn.Add(pair.In.Homogenize());
					sampleOut.Add(pair.Out.Homogenize());
				}
			}
			int sampleCount = sampleIn.Count / 2;
			if (sampleCount > SampleLimit) sampleCount = SampleLimit;

			for (int e = 0; e < EpochCount; e++)
			{
				Log.Instance.Info("[DNNC.Learn] epoch=" + e);

				List<double[]> learnIn = new List<double[]>();
				List<double[]> learnOut = new List<double[]>();
				foreach(int index in GetRandomIndex(sampleIn.Count, sampleCount))
				{
					learnIn.Add(sampleIn[index]);
					learnOut.Add(sampleOut[index]);
				}

				var dataInPrepared = _InnerTeacher.GetLayerInput(learnIn.ToArray());
				for (int i = 0; i < IterationCount; i++)
				{
					_InnerTeacher.RunEpoch(dataInPrepared, learnOut.ToArray());
					_Network.UpdateVisibleWeights();

					if (i % 10 == 0)
					{
						if (OuterTeacher == null) { if (InnerTest(i, sampleIn, sampleOut)) break; }
						else { OuterLearn(group); if (OuterTest(i, group)) break; }
					}
				}
			}
		}

		protected bool InnerTest(int e, List<double[]> sampleIn, List<double[]> sampleOut)
		{
			double tested = 0;
			for (int t = 0; t < sampleIn.Count; t++) tested += InnerTestCompute(sampleIn[t], sampleOut[t]);
			tested = tested / sampleIn.Count;
			//Log.Instance.Info("[DNNC.Learn] epoch=" + e + " diff=" + tested);
			return tested < 0.03;
		}
		protected double InnerTestCompute(double[] input, double[] output)
		{
			var tmpOut = _Network.Compute(input);
			var diff = Accord.Math.Matrix.Subtract(output, tmpOut);
			double lengthOut = Accord.Math.Norm.Euclidean(output);
			double lengthDiff = Accord.Math.Norm.Euclidean(diff);
			return lengthDiff / lengthOut;
		}

		protected List<LearningImagePair> PickupBlocks(LearningImage imageIn, LearningImage imageOut)
		{
			List<LearningImagePair> pairs = new List<LearningImagePair>();
			int half = BlockSize / 2;
			for(int h = 0; h < imageIn.Height; h++)
			{
				for(int w = 0; w < imageIn.Width; w++)
				{
					LearningImage bin = null, bout = null;
					bin = imageIn.Trim(new Rectangle(w - half, h - half, BlockSize, BlockSize));
					if (imageOut != null) bout = imageOut.Trim(new Rectangle(w, h, 1, 1));
					pairs.Add(new LearningImagePair(bin, bout));
				}
			}
			return pairs;
		}

		public override LearningImage Forecast(LearningImage imageIn)
		{
			var pairs = PickupBlocks(imageIn, null);
			LearningImage imageOut = new LearningImage(FrameOut);
			for (int i = 0; i < pairs.Count; i++)
			{
				int h = i / FrameOut.Width;
				int w = i % FrameOut.Width;
				double[] data = _Network.Compute(pairs[i].In.Homogenize());
				imageOut.SetPlane(h, w, new LearningPlane(data));
			}
			return imageOut;
		}

		private List<int> GetRandomIndex(int amount, int count)
		{
			int[] array = new int[amount];
			for (int i = 0; i < amount; i++) array[i] = i;
			return array.OrderBy(i => Guid.NewGuid()).Take(count).ToList();
		}

		const int OUTER_STEP = 5;

		protected void OuterLearn(LearningNodeGroup g)
		{
			List<LearningImage> inputs = new List<LearningImage>();
			List<LearningImage> outputs = new List<LearningImage>();

			for(int i = 0; i < g.Slots[0].Count; i += OUTER_STEP)	// 後で調整
			{
				// 正解
				inputs.Add(g.Slots[OutputReference][i]);
				outputs.Add(new LearningImage(1, 1, 1, new double[] { 1.0 }));
				// 不正解
				LearningImage image = Forecast(g.Slots[0][i]);
				inputs.Add(image);
				outputs.Add(new LearningImage(1, 1, 1, new double[] { 0.0 }));
			}

			LearningNodeGroup group = new LearningNodeGroup() { Name = "DNNC" };
			group.Slots[0] = inputs;
			group.Slots[-1] = outputs;
			OuterTeacher.Learn(group);
		}
		protected bool OuterTest(int ite, LearningNodeGroup g)
		{
			int count = 0, trues = 0;
			for (int i = 0; i < g.Slots[0].Count; i += OUTER_STEP)
			{
				// 正解
				LearningImage forecasted = OuterTeacher.Forecast(g.Slots[OutputReference][i]);
				if (forecasted.Data[0] > 0.5) trues++;
				count++;
				// 不正解
				forecasted = OuterTeacher.Forecast(Forecast(g.Slots[0][i]));
				if (forecasted.Data[0] > 0.5) trues++;
				count++;
			}
			Log.Instance.Info("[DNNC.Learn] ite=" + ite + " trues=" + trues + "/" + count);
			return trues > count * 0.75;
		}
	}
}