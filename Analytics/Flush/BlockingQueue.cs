using System;

using System.Collections.Generic;
using System.Threading;

namespace Segmentio.Flush
{
	/// <summary>
	/// Implementation of a blocking queue
	/// </summary>
	public class BlockingQueue<T> : IDisposable
	{
		private Queue<T> _queue = new Queue<T>();

		private bool _continue = true;

		/// <summary>
		/// Breaks the waiting state to check whether the queue is disposed on this interval
		/// </summary>
		public TimeSpan PulseInterval = TimeSpan.FromMilliseconds(250);

		public void Enqueue(T data)
		{
			if (data == null) throw new ArgumentNullException("No queue nulls allowed.");
			if (!_continue) return;

			lock (_queue)
			{
				_queue.Enqueue(data);
				Monitor.Pulse(_queue);
			}
		}

		public T Dequeue()
		{
			lock (_queue)
			{
				while (_queue.Count == 0 && _continue) Monitor.Wait(_queue, PulseInterval);
				if (!_continue) return default(T);
				return _queue.Dequeue();
			}
		}

		public int Count { get { return _queue.Count; } }

		public void Dispose() 
		{
			_continue = false;
		}
	}
}

