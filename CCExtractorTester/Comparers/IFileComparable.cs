using System;

namespace CCExtractorTester
{
	public interface IFileComparable
	{
		void CompareAndAddToResult(CompareData data);
		string GetResult(ResultData data);
		string GetReportFileName();
	}
}
