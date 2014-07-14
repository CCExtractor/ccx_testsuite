using System;

namespace CCExtractorTester
{
	public class NullProgressReporter: IProgressReportable {
		public static readonly NullProgressReporter Instance = new NullProgressReporter();

		private NullProgressReporter(){}

		#region IProgressReportable implementation
		public void showProgressMessage (string message)
		{
			// do nothing.
		}

		public void showProgramMessage (string message)
		{
			// do nothing
		}
		#endregion
	}
}