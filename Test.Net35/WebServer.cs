using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace RudderStack.Test
{
    class WebServer : IDisposable
    {
        private HttpListener listener;
        private Thread runningThread;
        private bool runServer;
        private string address;

        public delegate void HandleRequest(HttpListenerRequest req, HttpListenerResponse res);
        public HandleRequest RequestHandler = null;

        public WebServer(string url)
        {
            // Only string ends with '/' is acceptable for web server url
            address = url;
            if (!address.EndsWith("/"))
                address += "/";
        }

        public void HandleIncomingConnections()
        {
            while (runServer)
            {
                try
                {
                    // Will wait here until we hear from a connection
                    HttpListenerContext ctx = listener.GetContext();

                    // Peel out the requests and response objects
                    HttpListenerRequest req = ctx.Request;
                    HttpListenerResponse res = ctx.Response;

                    if (RequestHandler != null)
                    {
                        RequestHandler(req, res);
                    }
                    else
                    {
                        // Write the response info
                        string pageData = "{}";
                        byte[] data = Encoding.UTF8.GetBytes(pageData);
                        res.ContentType = "application/json";
                        res.ContentEncoding = Encoding.UTF8;
                        res.ContentLength64 = data.LongLength;

                        // Write out to the response stream (asynchronously), then close it
                        res.OutputStream.Write(data, 0, data.Length);
                        res.Close();
                    }
                }
                catch (System.Exception)
                {
                    runServer = false;
                    return;
                }
            }
        }

        public void RunAsync()
        {
            // Stop already running server
            Stop();

            // Create new listener
            listener = new HttpListener();
            listener.Prefixes.Add(address);
            listener.Start();

            // Start listening requests
            runServer = true;
            runningThread = new Thread(HandleIncomingConnections);
            runningThread.Start();
        }

        public void Stop()
        {
            runServer = false;

            if (listener != null)
            {
                listener.Close();
                listener = null;
            }
            if (runningThread != null)
            {
                runningThread.Join();
                runningThread = null;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Stop();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
