using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IconLibrary
{
	using LearningSlot = List<LearningImage>;

	public class LearningNode
	{
		protected List<LearningFrame> _FramesIn = new List<LearningFrame>();
		public virtual LearningFrame FrameIn { get { return _FramesIn[0]; } }
		public virtual LearningFrame FrameOut { get; }

		public int LearningLimit;

		protected ParallelOptions GetParallelOptions()
		{
			ParallelOptions o = new ParallelOptions();
#if DEBUG
			o.MaxDegreeOfParallelism = 1;
#else
			// デフォルトのまま
#endif
			return o;
		}

		public virtual string Filename { get { return "./"; } }
		public virtual void Initialize() { }
		public virtual bool Load(string path) { return false; }
		public virtual bool Save(string path) { return false; }

		public virtual void Learn(List<LearningImage> images) { }
		public virtual void Learn(LearningNodeGroup group)
		{
			Learn(group.Slots[0]);
		}

		public virtual LearningImage Forecast(LearningImage i) { return i; }
		public virtual LearningNodeGroup Forecast(LearningNodeGroup group)
		{
			LearningSlot slot = group.Slots[0];
			LearningImage[] list = new LearningImage[slot.Count];
			Parallel.For(0, slot.Count, GetParallelOptions(), i => { list[i] = Forecast(slot[i]); });
			group.Update(list);
			return group;
		}
		public virtual LearningImage BackForecast(LearningImage i) { return i; }
	}

	/*
	 * スロットをコピーする
	 */
	public class LearningNodeDuplicator : LearningNode
	{
		private int _From, _To;
		public LearningNodeDuplicator(int from, int to)
		{
			_From = from;
			_To = to;
		}
		public override void Learn(LearningNodeGroup group) { }
		public override LearningNodeGroup Forecast(LearningNodeGroup group)
		{
			group.Slots[_To] = group.Slots[_From];
			return base.Forecast(group);
		}
	}

	public class LearningNodeScaleAdd : LearningNode
	{
		private int _A, _B, _To;
		private double _ScaleA, _ScaleB;
		public LearningNodeScaleAdd(int a, int b, int to, double scaleA = 1, double scaleB = 1)
		{
			_A = a;
			_B = b;
			_To = to;
			_ScaleA = scaleA;
			_ScaleB = scaleB;
		}
		public override void Learn(LearningNodeGroup group) { }
		public override LearningNodeGroup Forecast(LearningNodeGroup group)
		{
			LearningSlot slot = new LearningSlot();
			for (int i = 0; i < group.Slots[_A].Count; i++)
			{
				var ai = new LearningImage(group.Slots[_A][i]);
				var bi = new LearningImage(group.Slots[_B][i]);
				if (_ScaleA != 1) LearningImage.Multiply(ai, ai, _ScaleA);
				if (_ScaleB != 1) LearningImage.Multiply(bi, bi, _ScaleB);
				LearningImage.Add(ai, bi, ai);
				slot.Add(ai);
			}
			group.Slots[_To] = slot;
			return group;
		}
	}

	public class LearningNodeGainBias : LearningNode
	{
		int _From, _To;
		double _Gain, _Bias;
		public LearningNodeGainBias(double gain, double bias, int from = 0, int to = 0)
		{
			_Gain = gain;
			_Bias = bias;
			_From = from;
			_To = to;
		}
		public override void Learn(LearningNodeGroup group) { }
		public override LearningNodeGroup Forecast(LearningNodeGroup group)
		{
			LearningSlot slot = new LearningSlot();
			for (int i = 0; i < group.Slots[_From].Count; i++)
			{
				var a = new LearningImage(group.Slots[0][i]);
				LearningImage.Multiply(a, a, _Gain, _Bias);
				slot.Add(a);
			}
			group.Slots[_To] = slot;
			return group;
		}
	}

	/*
	 * 画像サイズを等倍する
	 */
	public class LearningNodeScaler : LearningNode
	{
		int _Scale = 1;
		int _From, _To;
		public bool IsStrictly = true;

		public LearningNodeScaler(int scale, int from = 0, int to = 0)
		{
			_Scale = scale;
			_From = from;
			_To = to;
		}
		public override LearningNodeGroup Forecast(LearningNodeGroup group)
		{
			LearningSlot slot = new LearningSlot();
			for (int i = 0; i < group.Slots[_From].Count; i++)
			{
				var a = group.Slots[0][i].ScaleImage(_Scale, IsStrictly);
				slot.Add(a);
			}
			group.Slots[_To] = slot;
			return group;
		}
	}

	/*
	 * 他の関数を動かせるようにする
	 */
	public class LearningNodeWrapper : LearningNode
	{
		LearnHandler _Learn;
		ForecastHandler _Forecast;

		public LearningNodeWrapper(LearnHandler learn, ForecastHandler handler)
		{
			_Learn = learn;
			_Forecast = handler;
		}

		public override void Learn(LearningNodeGroup group)
		{
			if (_Learn == null) return;
			_Learn(group);
		}
		public override LearningImage Forecast(LearningImage i)
		{
			if (_Forecast == null) return i;
			return _Forecast(i);
		}
	}

	public class LearningNodeGroup
	{
		public string Name = "";
		public Dictionary<int, LearningSlot> Slots = new Dictionary<int, LearningSlot>();
		public void Update(LearningImage[] images, int index = 0) { Slots[index] = new LearningSlot(images);  }
	}

	public delegate void LearnHandler(LearningNodeGroup group);
	public delegate LearningImage ForecastHandler(LearningImage image);
}
