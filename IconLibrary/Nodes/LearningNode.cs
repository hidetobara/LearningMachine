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
		public override void Learn(LearningNodeGroup group)
		{
			//group.Slots[_To] = group.Slots[_From];
			base.Learn(group);
		}
		public override LearningNodeGroup Forecast(LearningNodeGroup group)
		{
			group.Slots[_To] = group.Slots[_From];
			return base.Forecast(group);
		}
	}

	public class LearningNodeGroup
	{
		public Dictionary<int, LearningSlot> Slots = new Dictionary<int, LearningSlot>();
		public void Update(LearningImage[] images, int index = 0) { Slots[index] = new LearningSlot(images);  }
	}
}
