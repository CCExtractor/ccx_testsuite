using System;

namespace CCExtractorTester
{
	public interface IFileComparable
	{
		void CompareAndAddToResult(string fileLocation1,string fileLocation2,string extraHTML="");
		string GetResult();
		string GetReportFileName();
	}
}
