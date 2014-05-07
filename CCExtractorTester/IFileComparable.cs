using System;

namespace CCExtractorTester
{
	public interface IFileComparable
	{
		string Compare(string fileLocation1,string fileLocation2);
	}
}
