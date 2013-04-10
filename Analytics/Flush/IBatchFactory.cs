using System;
using System.Collections.Generic;

using Segmentio.Model;

namespace Segmentio.Flush
{
	internal interface IBatchFactory
	{
		Batch Create(List<BaseAction> actions);
	}
}

