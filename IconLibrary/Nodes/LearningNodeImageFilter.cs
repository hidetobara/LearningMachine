using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IconLibrary
{
	/*
	 * 画像を再生成する
	 *  IPCA->DNNC
	 */
	public class LearningNodeImageFilter : LearningNodeConnection
	{
		public override string Filename{ get { return "IF"; } }

		public override int IMAGE_SIZE { get { return 64; } }    // 画像サイズ

		public LearningNodeImageFilter()
		{
			_Nodes = new List<LearningNode>();
			_Nodes.Add(new LearningNodeDuplicator(0, -1));		// コピー
			_Nodes.Add(new LearningIPCA_Slicing(3, 16, 4));		// 64-3
			_Nodes.Add(new LearningPool(2));					// 64-16
			_Nodes.Add(new LearningSigmoid());				// 32-16

			_Nodes.Add(new LearningNodeDuplicator(0, -2));		// コピー
			_Nodes.Add(new LearningIPCA_Slicing(16, 32, 4));	// 32-16
			_Nodes.Add(new LearningPool(2));					// 32-32
			_Nodes.Add(new LearningSigmoid());				// 16-32

			_Nodes.Add(new LearningNodeScaler(2));
			var dnnc1 = new LearningDNNConvolution(32, 32, 16, 64);
			dnnc1.OutputReference = -2;
			_Nodes.Add(dnnc1);

			_Nodes.Add(new LearningNodeScaler(2));
			var dnnc2 = new LearningDNNConvolution(64, 16, 3, 128);
			//dnnc2.OuterTeacher = new LearningNodeTrueFalse();
			dnnc2.OutputReference = -1;
			_Nodes.Add(dnnc2);
		}
	}

	/*
	 * 画像を再生成する
	 *  IPCA->IPCA+DNNC
	 */
	public class LearningNodeImageFilter2 : LearningNodeConnection
	{
		public override string Filename { get { return "IF2"; } }

		public override int IMAGE_SIZE { get { return 64; } }    // 画像サイズ

		public LearningNodeImageFilter2()
		{
			_Nodes = new List<LearningNode>();
			_Nodes.Add(new LearningNodeDuplicator(0, -1));	// コピー
			var ipca1 = new LearningIPCA_Slicing(3, 16, 4);
			_Nodes.Add(ipca1);	// 64-3
			_Nodes.Add(new LearningPool(2));    // 64-16

			var ipca2 = new LearningIPCA_Slicing(16, 32, 4);
			_Nodes.Add(ipca2);  // 32-16
			_Nodes.Add(new LearningPool(2));    // 32-32

			_Nodes.Add(new LearningNodeScaler(2));  // 32-32
			_Nodes.Add(new LearningNodeWrapper(null, ipca2.BackProject));   // 32-16

			_Nodes.Add(new LearningNodeScaler(2));  // 64-16
			_Nodes.Add(new LearningNodeDuplicator(0, -2));  // コピー
			_Nodes.Add(new LearningNodeWrapper(null, ipca1.BackProject));   // 64-3
			_Nodes.Add(new LearningNodeDuplicator(0, -3));  // コピー

			_Nodes.Add(new LearningNodeScaleAdd(-1, 0, 0, 1, -1.0));    // 64-3、理想－生成＝差分
			_Nodes.Add(new LearningNodeGainBias(0.5, 0.5));
			var dnnc = new LearningDNNConvolution(64, 16, 3, 64);
			dnnc.OutputReference = 0;
			dnnc.InputReference = -2;
			_Nodes.Add(dnnc);
			_Nodes.Add(new LearningNodeGainBias(2.0, -1.0));

			_Nodes.Add(new LearningNodeScaleAdd(0, -3, 0));
		}
	}
}
