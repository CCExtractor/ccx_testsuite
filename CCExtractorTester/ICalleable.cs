using System;
using System.Collections.Generic;

namespace CCExtractorTester
{
	/// <summary>
	/// Interface for calleable objects.
	/// </summary>
	public interface ICalleable
	{
		/// <summary>
		/// Call the instance back with the specified callbackValues.
		/// </summary>
		/// <param name="callbackValues">Callback values.</param>
		void Call(Dictionary<String,Object> callbackValues);
	}
}