using System;
using System.Collections.Generic;

using Segmentio.Model;

namespace Segmentio.Flush
{
	internal class SimpleBatchFactory : IBatchFactory
	{
		private string _writeKey;

		internal SimpleBatchFactory (string writeKey)
		{
			this._writeKey = writeKey;
		}

		public Batch Create(List<BaseAction> actions) 
		{
			return new Batch(_writeKey, actions);
		}
	}
}

