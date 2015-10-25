using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IconLibrary
{
	public class LearningDigits : LearningProcess
	{
		public LearningDigits()
		{
			_Units = new LearningUnit[4];
			_Units[0] = new LearningIPCA_Slicing(1, 16);	// (28,28,1)
			_Units[1] = new LearningPool(4);	// (7,7,16)
			_Units[2] = new LearningNormalize();	// (7,7,16)
			_Units[3] = new LearningDNN(7, 16, 4, 1);
		}
	}
}
