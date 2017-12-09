using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAnalyzerText2
{
	class Program
	{
		static void Main(string[] args)
		{
			var analyzer = new Analyzer();
//			analyzer.PickupProfileInHappyMail(new Analyzer.Option() { InputDir = "C:/obara/Data/HappyMail/Profile", OutputDir = "C:/obara/Data/HappyMail/Process" });
			analyzer.CalclateWordClasses(new Analyzer.Option() { InputDir = "C:/obara/Data/HappyMail/Process", OutputDir = "C:/obara/Data/HappyMail/Process" });
		}
	}
}
