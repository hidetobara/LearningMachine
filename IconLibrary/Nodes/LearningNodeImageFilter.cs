using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IconLibrary
{
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
			_Nodes.Add(new LearningNormalize());				// 32-16

			_Nodes.Add(new LearningNodeDuplicator(0, -2));		// コピー
			_Nodes.Add(new LearningIPCA_Slicing(16, 32, 4));	// 32-16
			_Nodes.Add(new LearningPool(2));					// 32-32
			_Nodes.Add(new LearningNormalize());				// 16-32

			_Nodes.Add(new LearningNodeScaler(2));
			var dnnc1 = new LearningDNNConvolution(32, 32, 16, 128);
			dnnc1.OutputReference = -2;
			_Nodes.Add(dnnc1);

			_Nodes.Add(new LearningNodeScaler(2));
			var dnnc2 = new LearningDNNConvolution(64, 16, 3, 256);
			dnnc2.OuterTeacher = new LearningNodeTrueFalse();
			dnnc2.OutputReference = -1;
			_Nodes.Add(dnnc2);
		}
	}
}
