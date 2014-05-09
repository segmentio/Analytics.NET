using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Segmentio.Flush;
using Segmentio.Request;
using Segmentio.Exception;
using Segmentio.Model;
using Segmentio.Stats;

namespace Segmentio
{
    /// <summary>
    /// A Segment.io REST client
    /// </summary>
    public class Client : IDisposable
    {
        private IFlushHandler _flushHandler;
        private string _secret;
        private Options _options;

        public Statistics Statistics { get; set; }

        #region Events

        public delegate void FailedHandler(BaseAction action, System.Exception e);
        public delegate void SucceededHandler(BaseAction action);

        public event FailedHandler Failed;
        public event SucceededHandler Succeeded;

        #endregion

        #region Initialization

        /// <summary>
        /// Creates a new REST client with a specified API secret and default options
        /// </summary>
        /// <param name="secret"></param>
        public Client(string secret) : this(secret, new Options()) {}

        /// <summary>
        /// Creates a new REST client with a specified API secret and default options
        /// </summary>
        /// <param name="secret"></param>
        /// <param name="options"></param>
        public Client(string secret, Options options)
        {
            if (String.IsNullOrEmpty(secret))
                throw new InvalidOperationException("Please supply a valid secret to initialize.");

            this.Statistics = new Statistics();

            this._secret = secret;
            this._options = options;

			IRequestHandler requestHandler = new BlockingRequestHandler(this, options.Timeout);
			IBatchFactory batchFactory = new SimpleBatchFactory(this._secret);

			if (options.Async)
				_flushHandler = new AsyncFlushHandler(batchFactory, requestHandler, options.MaxQueueSize);
			else
				_flushHandler = new BlockingFlushHandler(batchFactory, requestHandler);
        }

        #endregion

        #region Properties

        public string Secret
        {
            get
            {
                return _secret;
            }
        }


        public Options Options
        {
            get
            {
                return _options;
            }
        }

        #endregion

        #region Public Methods

		#region Identify

        /// <summary>
        /// Identifying a visitor ties all of their actions to an ID you
        /// recognize and records visitor traits you can segment by.
        /// </summary>
        ///
        /// <param name="userId">The visitor's identifier after they log in, or you know
        /// who they are. By
        /// explicitly identifying a user, you tie all of their actions to their identity.</param>
        ///
        /// <param name="traits">A dictionary with keys like "email", "name", “subscriptionPlan” or
        /// "friendCount”. You can segment your users by any trait you record.
        /// Pass in values in key-value format. String key, then its value
        /// { String, Integer, Boolean, Double, or Date are acceptable types for a value. } </param>
        ///
        public void Identify(string userId, Traits traits)
        {
            Identify(userId, traits, null, null);
        }

        /// <summary>
        /// Identifying a visitor ties all of their actions to an ID you
        /// recognize and records visitor traits you can segment by.
        /// </summary>
        ///
        /// <param name="userId">The visitor's identifier after they log in, or you know
        /// who they are. By
        /// explicitly identifying a user, you tie all of their actions to their identity.</param>
        ///
        /// <param name="traits">A dictionary with keys like "email", "name", “subscriptionPlan” or
        /// "friendCount”. You can segment your users by any trait you record.
        /// Pass in values in key-value format. String key, then its value
        /// { String, Integer, Boolean, Double, or Date are acceptable types for a value. } </param>
        ///
        /// <param name="timestamp">  If this event happened in the past, the timestamp
        /// can be used to designate when the identification happened. Careful with this one,
        /// if it just happened, leave it null.</param>
        ///
        public void Identify(string userId, Traits traits, DateTime? timestamp)
        {
            Identify(userId, traits, timestamp, null);
        }

        /// <summary>
        /// Identifying a visitor ties all of their actions to an ID you
        /// recognize and records visitor traits you can segment by.
        /// </summary>
        ///
        /// <param name="userId">The visitor's identifier after they log in, or you know
        /// who they are. By
        /// explicitly identifying a user, you tie all of their actions to their identity.</param>
        ///
        /// <param name="traits">A dictionary with keys like "email", "name", “subscriptionPlan” or
        /// "friendCount”. You can segment your users by any trait you record.
        /// Pass in values in key-value format. String key, then its value
        /// { String, Integer, Boolean, Double, or Date are acceptable types for a value. } </param>
        ///
        /// <param name="context"> A dictionary with additional information thats related to the visit.
        /// Examples are userAgent, and IP address of the visitor.
        /// Feel free to pass in null if you don't have this information.</param>
        ///
        public void Identify(string userId, Traits traits, Context context)
        {
            Identify(userId, traits, null, context);
        }

        /// <summary>
        /// Identifying a visitor ties all of their actions to an ID you
        /// recognize and records visitor traits you can segment by.
        /// </summary>
        ///
        /// <param name="userId">The visitor's identifier after they log in, or you know
        /// who they are. By
        /// explicitly identifying a user, you tie all of their actions to their identity.</param>
        ///
        /// <param name="traits">A dictionary with keys like "email", "name", “subscriptionPlan” or
        /// "friendCount”. You can segment your users by any trait you record.
        /// Pass in values in key-value format. String key, then its value
        /// { String, Integer, Boolean, Double, or Date are acceptable types for a value. } </param>
        ///
        /// <param name="timestamp">  If this event happened in the past, the timestamp
        /// can be used to designate when the identification happened. Careful with this one,
        /// if it just happened, leave it null.</param>

        /// <param name="context"> A dictionary with additional information thats related to the visit.
        /// Examples are userAgent, and IP address of the visitor.
        /// Feel free to pass in null if you don't have this information.</param>
        ///
        ///
        public void Identify(string userId, Traits traits,
            DateTime? timestamp, Context context)
        {

            if (String.IsNullOrEmpty(userId))
                throw new InvalidOperationException("Please supply a valid userId to Identify.");

			Identify identify = new Identify(userId, traits, timestamp, context);

            Enqueue(identify);
        }

		#endregion

		#region Track

        /// <summary>
        /// Whenever a user triggers an event on your site, you’ll want to track it.
        /// </summary>
        ///
        /// <param name="userId">The visitor's identifier after they log in, or you know
        /// who they are. </param>
        ///
        /// <param name="eventName">The event name you are tracking. It is recommended
        /// that it is in human readable form. For example, "Bought T-Shirt"
        /// or "Started an exercise"</param>
        ///
        public void Track(string userId, string eventName)
        {
            Track(userId, eventName, null, null, null);
        }

        /// <summary>
        /// Whenever a user triggers an event on your site, you’ll want to track it.
        /// </summary>
        ///
        /// <param name="userId">The visitor's identifier after they log in, or you know
        /// who they are. </param>
        ///
        /// <param name="eventName">The event name you are tracking. It is recommended
        /// that it is in human readable form. For example, "Bought T-Shirt"
        /// or "Started an exercise"</param>
        ///
        /// <param name="properties"> A dictionary with items that describe the event
        /// in more detail. This argument is optional, but highly recommended —
        /// you’ll find these properties extremely useful later.</param>
        ///
        public void Track(string userId, string eventName, Properties properties)
        {
            Track(userId, eventName, properties, null, null);
        }


        /// <summary>
        /// Whenever a user triggers an event on your site, you’ll want to track it
        /// so that you can analyze and segment by those events later.
        /// </summary>
        ///
        /// <param name="userId">The visitor's identifier after they log in, or you know
        /// who they are. By
        /// explicitly identifying a user, you tie all of their actions to their identity.
        /// This makes it possible for you to run things like segment-based email campaigns.</param>
        ///
        /// <param name="eventName">The event name you are tracking. It is recommended
        /// that it is in human readable form. For example, "Bought T-Shirt"
        /// or "Started an exercise"</param>
        ///
        /// <param name="properties"> A dictionary with items that describe the event
        /// in more detail. This argument is optional, but highly recommended —
        /// you’ll find these properties extremely useful later.</param>
        ///
        /// <param name="context"> A dictionary with additional information thats related to the visit.
        /// Examples are userAgent, and IP address of the visitor.
        /// Feel free to pass in null if you don't have this information.</param>
        ///
        /// <param name="timestamp">  If this event happened in the past, the timestamp
        /// can be used to designate when the identification happened. Careful with this one,
        /// if it just happened, leave it null.</param>
        ///
        public void Track(string userId, string eventName, Properties properties,
           DateTime? timestamp)
        {
            Track(userId, eventName, properties, timestamp, null);
        }

        /// <summary>
        /// Whenever a user triggers an event on your site, you’ll want to track it
        /// so that you can analyze and segment by those events later.
        /// </summary>
        ///
        /// <param name="userId">The visitor's identifier after they log in, or you know
        /// who they are. By
        /// explicitly identifying a user, you tie all of their actions to their identity.
        /// This makes it possible for you to run things like segment-based email campaigns.</param>
        ///
        /// <param name="eventName">The event name you are tracking. It is recommended
        /// that it is in human readable form. For example, "Bought T-Shirt"
        /// or "Started an exercise"</param>
        ///
        /// <param name="properties"> A dictionary with items that describe the event
        /// in more detail. This argument is optional, but highly recommended —
        /// you’ll find these properties extremely useful later.</param>
        ///
        /// <param name="timestamp">  If this event happened in the past, the timestamp
        /// can be used to designate when the identification happened. Careful with this one,
        /// if it just happened, leave it null.</param>
        ///
        /// <param name="context"> A dictionary with additional information thats related to the visit.
        /// Examples are userAgent, and IP address of the visitor.
        /// Feel free to pass in null if you don't have this information.</param>
        ///
        ///
        public void Track(string userId, string eventName, Properties properties,
            DateTime? timestamp, Context context)
        {
            if (String.IsNullOrEmpty(userId))
                throw new InvalidOperationException("Please supply a valid userId to Track.");

            if (String.IsNullOrEmpty(eventName))
                throw new InvalidOperationException("Please supply a valid eventName to Track.");

			Track track = new Track(userId, eventName, properties, timestamp, context);

            Enqueue(track);
        }

		#endregion

		#region Alias

		
		/// <summary>
		/// Aliases an anonymous user into an identified user.
		/// </summary>
		/// 
		/// <param name="from">The anonymous user's id before they are logged in.</param>
		/// 
		/// <param name="to">the identified user's id after they're logged in.</param>
		
		/// <param name="timestamp">  If this event happened in the past, the timestamp
		/// can be used to designate when the identification happened. Careful with this one,
		/// if it just happened, leave it null.</param>
		/// 
		public void Alias(string from, string to)
		{
			Alias(from, to, null, null);
		}

		
		/// <summary>
		/// Aliases an anonymous user into an identified user.
		/// </summary>
		/// 
		/// <param name="from">The anonymous user's id before they are logged in.</param>
		/// 
		/// <param name="to">the identified user's id after they're logged in.</param>
		
		/// <param name="timestamp">  If this event happened in the past, the timestamp
		/// can be used to designate when the identification happened. Careful with this one,
		/// if it just happened, leave it null.</param>
		/// 
		public void Alias(string from, string to, DateTime? timestamp)
		{
			Alias(from, to, timestamp, null);
		}

		/// <summary>
		/// Aliases an anonymous user into an identified user.
		/// </summary>
		/// 
		/// <param name="from">The anonymous user's id before they are logged in.</param>
		/// 
		/// <param name="to">the identified user's id after they're logged in.</param>
		
		/// <param name="context"> A dictionary with additional information thats related to the visit.
		/// Examples are userAgent, and IP address of the visitor.
		/// Feel free to pass in null if you don't have this information.</param>
		/// 
		public void Alias(string from, string to, Context context)
		{
			Alias(from, to, null, context);
		}

		/// <summary>
		/// Aliases an anonymous user into an identified user.
		/// </summary>
		/// 
		/// <param name="from">The anonymous user's id before they are logged in.</param>
		/// 
		/// <param name="to">the identified user's id after they're logged in.</param>
		
		/// <param name="timestamp">  If this event happened in the past, the timestamp
		/// can be used to designate when the identification happened. Careful with this one,
		/// if it just happened, leave it null.</param>
		///
		/// <param name="context"> A dictionary with additional information thats related to the visit.
		/// Examples are userAgent, and IP address of the visitor.
		/// Feel free to pass in null if you don't have this information.</param>
		/// 
		public void Alias(string from, string to, DateTime? timestamp, Context context)
		{
			if (String.IsNullOrEmpty(from))
				throw new InvalidOperationException("Please supply a valid 'from' to Alias.");
			
			if (String.IsNullOrEmpty(to))
				throw new InvalidOperationException("Please supply a valid 'to' to Alias.");
			
			Alias alias = new Alias(from, to, timestamp, context);

            Enqueue(alias);
		}

		#endregion


        #region Other

        /// <summary>
        /// Blocks until all messages are flushed
        /// </summary>
        public void Flush()
        {
			_flushHandler.Flush();
        }

		/// <summary>
		/// Disposes of the flushing thread and the message queue. Note, this does not call Flush() first.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="Segmentio.Client"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="Segmentio.Client"/> in an unusable state. After calling
		/// <see cref="Dispose"/>, you must release all references to the <see cref="Segmentio.Client"/> so the garbage
		/// collector can reclaim the memory that the <see cref="Segmentio.Client"/> was occupying.</remarks>
		public void Dispose() 
		{
			_flushHandler.Dispose();
		}

        #endregion

        #endregion

        #region Private Methods

        private void Enqueue(BaseAction action)
        {
            _flushHandler.Process(action);

            this.Statistics.Submitted += 1;
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

        #endregion
    }
}
