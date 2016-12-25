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
			var dnn1 = new LearningDNN(4, 32, 16, 16, 32);
			dnn1.OutputReference = -2;
			dnn1.DropoutRate = 0.5;
			dnn1.DropoutPadding = 15;
			_Nodes.Add(dnn1);
			var dnn2 = new LearningDNN(16, 16, 64, 3, 64);
			dnn2.OutputReference = -1;
			dnn2.DropoutRate = 0.5;
			dnn2.DropoutPadding = 15;
			_Nodes.Add(dnn2);
		}
	}
}
