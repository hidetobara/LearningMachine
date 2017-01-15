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
			_Nodes.Add(new LearningNodeDuplicator(0, -1));	// コピー
			_Nodes.Add(new LearningIPCA_Slicing(3, 16));    // 64,64,3
			_Nodes.Add(new LearningPool(4));                // 64,64,16
			_Nodes.Add(new LearningNormalize());            // 16,16,16
			_Nodes.Add(new LearningNodeDuplicator(0, -2));  // コピー
			_Nodes.Add(new LearningIPCA_Slicing(16, 32));    // 16,16,32
			_Nodes.Add(new LearningPool(4));                // 4,4,32
			_Nodes.Add(new LearningNormalize());            // 4,4,32

			_Nodes.Add(new LearningNodeScaler(4));
			var dnnc1 = new LearningDNNConvolution(16, 32, 16, 64);
			dnnc1.OutputReference = -2;
			_Nodes.Add(dnnc1);
			_Nodes.Add(new LearningNodeScaler(4));
			var dnnc2 = new LearningDNNConvolution(64, 16, 3, 128);
#if !DEBUG
			dnnc2.EpochCount = 300;
#endif
			dnnc2.OutputReference = -1;
			_Nodes.Add(dnnc2);
		}
	}
}
