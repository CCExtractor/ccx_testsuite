using System;

namespace CCExtractorTester
{
	public interface IReportable : IFileComparable
	{
		void AddInformation (string htmlData);
	}
}
