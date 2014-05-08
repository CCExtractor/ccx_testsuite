using System;
using System.Collections.Generic;

namespace CCExtractorTester.DiffTool
{
	public class SingleSideModel
	{
		public List<LineModel> Lines { get; private set; }

		public SingleSideModel()
		{
			Lines = new List<LineModel>();
		}
	}
}

