using System;

namespace CCExtractorTester
{
	/// <summary>
	/// Interface for reporting progress.
	/// </summary>
	public interface IProgressReportable
	{
		/// <summary>
		/// Shows the progress message.
		/// </summary>
		/// <param name="message">The progress message to show.</param>
		void showProgressMessage(string message);
	}
}