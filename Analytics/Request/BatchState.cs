using System.Threading;
using Segment.Model;

namespace Segmentio.Request
{
    internal class BatchState
    {
        internal HttpWebRequest Request { get; set; }
        internal Batch Batch { get; set; }
		internal ManualResetEvent Event { get; set; }

        internal BatchState(HttpWebRequest request, Batch batch)
        {
            this.Request = request;
            this.Batch = batch;

			this.Event = new ManualResetEvent(false);
        }
    }
}
