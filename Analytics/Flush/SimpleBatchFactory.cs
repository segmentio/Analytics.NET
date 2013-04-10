using System;
using System.Collections.Generic;

using Segmentio.Model;

namespace Segmentio.Flush
{
	internal class SimpleBatchFactory : IBatchFactory
	{
		private string _secret;

		internal SimpleBatchFactory (string secret)
		{
			this._secret = secret;
		}

		public Batch Create(List<BaseAction> actions) 
		{
			return new Batch(_secret, actions);
		}
	}
}

