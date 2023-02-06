using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RudderStack.Flush;
using RudderStack.Model;
using RudderStack.Request;
using RudderStack.Stats;

namespace RudderStack
{

    /// <summary>
    /// A RudderStack .NET client
    /// </summary>
    public class RudderClient : IRudderAnalyticsClient
    {
#if NET35
        private IFlushHandler _flushHandler;
#else
        private IAsyncFlushHandler _flushHandler;
#endif
        private string _writeKey;
        private RudderConfig _config;

        public Statistics Statistics { get; set; }

        #region Events

        public event FailedHandler Failed;
        public event SucceededHandler Succeeded;

        #endregion

        #region Initialization

        /// <summary>
        /// Creates a new REST client with a specified API writeKey and default config
        /// </summary>
        /// <param name="writeKey"></param>
        public RudderClient(string writeKey) : this(writeKey, new RudderConfig()) { }

        /// <summary>
        /// Creates a new REST client with a specified API writeKey and default config
        /// </summary>
        /// <param name="writeKey"></param>
        /// <param name="config"></param>
        public RudderClient(string writeKey, RudderConfig config) : this(writeKey, config, null)
        {
            if (string.IsNullOrEmpty(writeKey))
                throw new InvalidOperationException("Please supply a valid writeKey to initialize.");

        }

        internal RudderClient(string writeKey, RudderConfig config, IRequestHandler requestHandler)
        {

            this.Statistics = new Statistics();

            this._writeKey = writeKey;
            this._config = config;

            if (requestHandler == null)
            {
                if (config.Send)
                {
                    if (config.MaxRetryTime.HasValue)
                    {
                        requestHandler = new BlockingRequestHandler(this, config.Timeout, new Backo(max: (Convert.ToInt32(config.MaxRetryTime.Value.TotalSeconds) * 1000), jitter: 5000));
                    }
                    else
                    {
                        requestHandler = new BlockingRequestHandler(this, config.Timeout);
                    }

                }
                else
                {
                    requestHandler = new FakeRequestHandler(this);
                }

            }

            IBatchFactory batchFactory = new SimpleBatchFactory(this._writeKey);

            if (config.Async)
            {
                _flushHandler = new AsyncIntervalFlushHandler(batchFactory, requestHandler, config.MaxQueueSize, config.FlushAt, config.FlushIntervalInMillis, config.Threads);
            }
            else
                _flushHandler = new BlockingFlushHandler(batchFactory, requestHandler);
        }

        #endregion

        #region Properties

        public string WriteKey
        {
            get
            {
                return _writeKey;
            }
        }


        public RudderConfig Config
        {
            get
            {
                return _config;
            }
        }

        #endregion

        #region Public Methods

        #region Identify

        /// <inheritdoc />
        public void Identify(string userId, IDictionary<string, object> traits)
        {
            Identify(userId, traits, null);
        }

        /// <inheritdoc />
        public void Identify(string userId, IDictionary<string, object> traits, RudderOptions options)
        {
            if (String.IsNullOrEmpty(userId) && !HasAnonymousId(options))
                throw new InvalidOperationException("Please supply a valid userId to Identify.");

            Enqueue(new Identify(userId, traits, options));
        }

        #endregion

        #region Group

        /// <inheritdoc />
        public void Group(string userId, string groupId, RudderOptions options)
        {
            Group(userId, groupId, null, options);
        }

        /// <inheritdoc />
        public void Group(string userId, string groupId, IDictionary<string, object> traits)
        {
            Group(userId, groupId, traits, null);
        }

        /// <inheritdoc />
        public void Group(string userId, string groupId, IDictionary<string, object> traits, RudderOptions options)
        {
            if (String.IsNullOrEmpty(userId) && !HasAnonymousId(options))
                throw new InvalidOperationException("Please supply a valid userId or anonymousId to call #Group.");

            if (String.IsNullOrEmpty(groupId))
                throw new InvalidOperationException("Please supply a valid groupId to call #Group.");

            Enqueue(new Group(userId, groupId, traits, options));
        }

        #endregion

        #region Track

        /// <inheritdoc />
        public void Track(string userId, string eventName)
        {
            Track(userId, eventName, null, null);
        }

        /// <inheritdoc />
        public void Track(string userId, string eventName, IDictionary<string, object> properties)
        {
            Track(userId, eventName, properties, null);
        }

        /// <inheritdoc />
        public void Track(string userId, string eventName, RudderOptions options)
        {
            Track(userId, eventName, null, options);
        }

        /// <inheritdoc />
        public void Track(string userId, string eventName, IDictionary<string, object> properties, RudderOptions options)
        {
            if (String.IsNullOrEmpty(userId) && !HasAnonymousId(options))
                throw new InvalidOperationException("Please supply a valid userId or anonymousId to call #Track.");

            if (String.IsNullOrEmpty(eventName))
                throw new InvalidOperationException("Please supply a valid event to call #Track.");

            Enqueue(new Track(userId, eventName, properties, options));
        }

        #endregion

        #region Alias

        /// <inheritdoc />
        public void Alias(string previousId, string userId)
        {
            Alias(previousId, userId, null);
        }

        /// <inheritdoc />
        public void Alias(string previousId, string userId, RudderOptions options)
        {
            if (String.IsNullOrEmpty(previousId))
                throw new InvalidOperationException("Please supply a valid 'previousId' to Alias.");

            if (String.IsNullOrEmpty(userId))
                throw new InvalidOperationException("Please supply a valid 'userId' to Alias.");

            Enqueue(new Alias(previousId, userId, options));
        }

        #endregion

        #region Page

        /// <inheritdoc />
        public void Page(string userId, string name)
        {
            Page(userId, name, null, null, null);
        }

        /// <inheritdoc />
        public void Page(string userId, string name, RudderOptions options)
        {
            Page(userId, name, null, null, options);
        }

        /// <inheritdoc />
        public void Page(string userId, string name, string category)
        {
            Page(userId, name, category, null, null);
        }

        /// <inheritdoc />
        public void Page(string userId, string name, IDictionary<string, object> properties)
        {
            Page(userId, name, null, properties, null);
        }

        /// <inheritdoc />
        public void Page(string userId, string name, IDictionary<string, object> properties, RudderOptions options)
        {
            Page(userId, name, null, properties, options);
        }

        /// <inheritdoc />
        public void Page(string userId, string name, string category, IDictionary<string, object> properties, RudderOptions options)
        {
            if (String.IsNullOrEmpty(userId) && !HasAnonymousId(options))
                throw new InvalidOperationException("Please supply a valid userId or anonymousId to call #Page.");

            if (String.IsNullOrEmpty(name))
                throw new InvalidOperationException("Please supply a valid name to call #Page.");

            Enqueue(new Page(userId, name, category, properties, options));
        }

        #endregion

        #region Screen

        /// <inheritdoc />
        public void Screen(string userId, string name)
        {
            Screen(userId, name, null, null, null);
        }

        /// <inheritdoc />
        public void Screen(string userId, string name, RudderOptions options)
        {
            Screen(userId, name, null, null, options);
        }

        /// <inheritdoc />
        public void Screen(string userId, string name, string category)
        {
            Screen(userId, name, category, null, null);
        }

        /// <inheritdoc />
        public void Screen(string userId, string name, IDictionary<string, object> properties)
        {
            Screen(userId, name, null, properties, null);
        }

        /// <inheritdoc />
        public void Screen(string userId, string name, IDictionary<string, object> properties, RudderOptions options)
        {
            Screen(userId, name, null, properties, options);
        }

        /// <inheritdoc />
        public void Screen(string userId, string name, string category, IDictionary<string, object> properties, RudderOptions options)
        {
            if (String.IsNullOrEmpty(userId) && !HasAnonymousId(options))
                throw new InvalidOperationException("Please supply a valid userId or anonymousId to call #Screen.");

            if (String.IsNullOrEmpty(name))
                throw new InvalidOperationException("Please supply a valid name to call #Screen.");

            Enqueue(new Screen(userId, name, category, properties, options));
        }

        #endregion

        #region Other

        /// <inheritdoc />
        public void Flush()
        {
            _flushHandler.Flush();
        }
#if !NET35
        public Task FlushAsync()
        {
            return _flushHandler.FlushAsync();
        }
#endif

        /// <inheritdoc />
        public void Dispose()
        {
            _flushHandler.Dispose();
        }

        #endregion

        #endregion

        #region Private Methods

        private void Enqueue(BaseAction action)
        {
            _flushHandler.Process(action).GetAwaiter().GetResult();
            this.Statistics.IncrementSubmitted();
        }

        protected void ensureId(String userId, RudderOptions options)
        {
            if (String.IsNullOrEmpty(userId) && String.IsNullOrEmpty(options.AnonymousId))
                throw new InvalidOperationException("Please supply a valid id (either userId or anonymousId.");
        }

        #endregion

        #region Event API

        internal void RaiseSuccess(BaseAction action)
        {
            if (Succeeded != null) Succeeded(action);
        }

        internal void RaiseFailure(BaseAction action, System.Exception e)
        {
            if (Failed != null) Failed(action, e);
        }

        /// <summary>
        /// Determines whether an anonymous identifier is defined in the specified options.
        /// </summary>
        /// <returns><c>true</c> if the specified options have an anonymous identifier; otherwise, <c>false</c>.</returns>
        /// <param name="options">Options.</param>
        internal static bool HasAnonymousId(RudderOptions options)
        {
            return options != null && !String.IsNullOrEmpty(options.AnonymousId);
        }

        #endregion
    }
}
