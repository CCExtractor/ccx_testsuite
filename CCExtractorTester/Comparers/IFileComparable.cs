using System;

namespace CCExtractorTester
{
	public interface IFileComparable
	{
		void CompareAndAddToResult(CompareData data);
		void SaveReport (string pathToFolder, ResultData data);
	}
}
