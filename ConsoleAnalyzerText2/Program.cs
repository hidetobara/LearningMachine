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
			const string DIC = "C:/obara/Library/NMeCab0.07/dic/ipadic";
			var analyzer = new Analyzer();
			//analyzer.PickupProfileInHappyMail(new Analyzer.Option() { InputDir = "C:/obara/Data/HappyMail/Crawl", OutputDir = "C:/obara/Data/HappyMail/Process" });
			analyzer.CalclateWordClasses(new Analyzer.Option() { DicDir = DIC, InputDir = "C:/obara/Data/HappyMail/Process", DictionaryPath = "C:/obara/Data/HappyMail/Process/words.csv" });
			//analyzer.CalclateFrequency(new Analyzer.Option() { Loop = 2, DictionaryPath = "C:/obara/Data/HappyMail/Process/words.csv", InputDir = "C:/obara/Data/HappyMail/Process", OutputDir = "C:/obara/Data/HappyMail/Process" });
			//analyzer.GroupByStatistics(new Analyzer.Option() { DictionaryPath = "C:/obara/Data/HappyMail/Process/words.csv", InputDir = "C:/obara/Data/HappyMail/Process", OutputDir = "C:/obara/Data/HappyMail/Process" });
			//analyzer.GroupByStatistics(new Analyzer.Option() { ReloadStatistics = true, DictionaryPath = "C:/obara/Data/HappyMail/Process/words.csv", InputDir = "C:/obara/Data/HappyMail/Process", OutputDir = "C:/obara/Data/HappyMail/Process", PickupAxis = 4, PickupLower = -3, PickupUpper = -1.1 });
			//analyzer.GroupByDifference(new Analyzer.Option() { DictionaryPath = "C:/obara/Data/HappyMail/Process/words.csv", InputDir = "C:/obara/Data/HappyMail/Process", OutputDir = "C:/obara/Data/HappyMail/Process" });
			//analyzer.BuildVolatile(new Analyzer.Option() { InputDir = "C:/obara/Data/HappyMail/Process", OutputDir = "C:/obara/Data/HappyMail/Process" });
		}
	}
}
