using System;

namespace CCExtractorTester
{
	public interface IProgressReportable
	{
		void showProgressMessage(string message);
	}
}