using System;

namespace CCExtractorTester
{
	public interface IFileComparable
	{
		void CompareAndAddToResult(CompareData data);
		string GetResult();
		string GetReportFileName();
	}
}
