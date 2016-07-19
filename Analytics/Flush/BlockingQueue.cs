//-----------------------------------------------------------------------
// <copyright file="BlockingQueue.cs" company="Segment">
//     Copyright (c) Segment. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Segment.Flush
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Segment.Model;

    /// <summary>
    /// Implementation of a blocking queue.
    /// </summary>
    /// <typeparam name="T">The generic type this queue holds.</typeparam>
    public class BlockingQueue<T> : IDisposable
    {
        /// <summary>
        /// Breaks the waiting state to check whether the queue is disposed on this interval.
        /// </summary>
        public static readonly TimeSpan PulseInterval = TimeSpan.FromMilliseconds(100);

        private Queue<T> queue = new Queue<T>();
        private bool isDisposed = false;

        public int Count
        {
            get { return this.queue.Count; }
        }

        public void Enqueue(T data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("No queue nulls allowed.");
            }

            if (this.isDisposed)
            {
                return;
            }

            lock (this.queue)
            {
                this.queue.Enqueue(data);
                Monitor.Pulse(this.queue);
            }

            Logger.Debug("Enqueued action in queue.", new Dict { { "queue size", this.queue.Count } });
        }

        public T Dequeue()
        {
            lock (this.queue)
            {
                while (this.queue.Count == 0 && this.isDisposed == false)
                {
                    Monitor.Wait(this.queue, PulseInterval);
                }

                if (this.isDisposed)
                {
                    return default(T);
                }

                return this.queue.Dequeue();
            }
        }

        public void Dispose() 
        {
            this.isDisposed = true;
        }
    }
}
