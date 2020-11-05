using System;
using System.Threading.Tasks;
using RudderStack.Model;

namespace RudderStack.Flush
{
    /// <summary>
    /// A component responsibe for flushing an action to the server
    /// </summary>
    public interface IFlushHandler : IDisposable
    {
        /// <summary>
        /// Validates an action and begins the process of flushing it to the server
        /// </summary>
        /// <param name="action">Action.</param>
        Task Process(BaseAction action);

        /// <summary>
        /// Blocks until all processing messages are flushed to the server
        /// </summary>
        void Flush();
    }

    public interface IAsyncFlushHandler : IFlushHandler
    {
        Task FlushAsync();
    }
}
