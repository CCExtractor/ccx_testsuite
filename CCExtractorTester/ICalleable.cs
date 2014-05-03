using System;
using System.Collections.Generic;

namespace CCExtractorTester
{
	public interface ICalleable
	{
		void Call(Dictionary<String,Object> callbackValues);
	}
}