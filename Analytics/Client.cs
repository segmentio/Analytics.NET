using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Segment.Flush;
using Segment.Request;
using Segment.Exception;
using Segment.Model;
using Segment.Stats;

namespace Segment
{
    /// <summary>
    /// A Segment.io .NET client
    /// </summary>
    public class Client : IDisposable
    {
        private IFlushHandler _flushHandler;
        private string _writeKey;
		private Config _config;

        public Statistics Statistics { get; set; }

        #region Events

        public delegate void FailedHandler(BaseAction action, System.Exception e);
        public delegate void SucceededHandler(BaseAction action);

        public event FailedHandler Failed;
        public event SucceededHandler Succeeded;

        #endregion

        #region Initialization

        /// <summary>
        /// Creates a new REST client with a specified API writeKey and default config
        /// </summary>
        /// <param name="writeKey"></param>
        public Client(string writeKey) : this(writeKey, new Config()) {}

        /// <summary>
        /// Creates a new REST client with a specified API writeKey and default config
        /// </summary>
        /// <param name="writeKey"></param>
        /// <param name="config"></param>
		public Client(string writeKey, Config config)
        {
            if (String.IsNullOrEmpty(writeKey))
                throw new InvalidOperationException("Please supply a valid writeKey to initialize.");

            this.Statistics = new Statistics();

            this._writeKey = writeKey;
			this._config = config;

			IRequestHandler requestHandler = new BlockingRequestHandler(this, config.Timeout);
			IBatchFactory batchFactory = new SimpleBatchFactory(this._writeKey);

			if (config.Async)
				_flushHandler = new AsyncFlushHandler(batchFactory, requestHandler, config.MaxQueueSize);
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


		public Config Config
        {
            get
            {
				return _config;
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
            Identify(userId, traits, null);
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
		/// <param name="options">Options allowing you to set timestamp, anonymousId, target integrations,
		/// and the context of th emessage.</param>
        ///
		public void Identify(string userId, Traits traits, Options options)
        {
			if (String.IsNullOrEmpty(userId) && !HasAnonymousId(options))
				throw new InvalidOperationException("Please supply a valid userId to Identify.");

			Enqueue(new Identify(userId, traits, options));
        }

		#endregion

		#region Group

		/// <summary>
		/// The `group` method lets you associate a user with a group. Be it a company, 
		/// organization, account, project, team or whatever other crazy name you came up 
		/// with for the same concept! It also lets you record custom traits about the 
		/// group, like industry or number of employees.
		/// </summary>
		///
		/// <param name="userId">The visitor's database identifier after they log in, or you know
		/// who they are. By explicitly grouping a user, you tie all of their actions to their group.</param>
		///
		/// <param name="groupId">The group's database identifier after they log in, or you know
		/// who they are.</param>
		///
		/// <param name="options">Options allowing you to set timestamp, anonymousId, target integrations,
		/// and the context of th emessage.</param>
		///
		public void Group(string userId, string groupId, Options options)
		{
			Group (userId, groupId, null, options);
		}

		/// <summary>
		/// The `group` method lets you associate a user with a group. Be it a company, 
		/// organization, account, project, team or whatever other crazy name you came up 
		/// with for the same concept! It also lets you record custom traits about the 
		/// group, like industry or number of employees.
		/// </summary>
		///
		/// <param name="userId">The visitor's database identifier after they log in, or you know
		/// who they are. By explicitly grouping a user, you tie all of their actions to their group.</param>
		///
		/// <param name="groupId">The group's database identifier after they log in, or you know
		/// who they are.</param>
		///
		/// <param name="traits">A dictionary with group keys like "name", “subscriptionPlan”. 
		/// You can segment your users by any trait you record. Pass in values in key-value format. 
		/// String key, then its value { String, Integer, Boolean, Double, or Date are acceptable types for a value. } </param>
		///
		public void Group(string userId, string groupId, Traits traits)
		{
			Group (userId, groupId, traits, null);
		}

		/// <summary>
		/// The `group` method lets you associate a user with a group. Be it a company, 
		/// organization, account, project, team or whatever other crazy name you came up 
		/// with for the same concept! It also lets you record custom traits about the 
		/// group, like industry or number of employees.
		/// </summary>
		///
		/// <param name="userId">The visitor's database identifier after they log in, or you know
		/// who they are. By explicitly grouping a user, you tie all of their actions to their group.</param>
		///
		/// <param name="groupId">The group's database identifier after they log in, or you know
		/// who they are.</param>
		///
		/// <param name="traits">A dictionary with group keys like "name", “subscriptionPlan”. 
		/// You can segment your users by any trait you record. Pass in values in key-value format. 
		/// String key, then its value { String, Integer, Boolean, Double, or Date are acceptable types for a value. } </param>
		///
		/// <param name="options">Options allowing you to set timestamp, anonymousId, target integrations,
		/// and the context of the message.</param>
		///
		public void Group(string userId, string groupId, Traits traits, Options options)
		{
			if (String.IsNullOrEmpty(userId) && !HasAnonymousId(options))
				throw new InvalidOperationException("Please supply a valid userId or anonymousId to call #Group.");

			if (String.IsNullOrEmpty(groupId))
				throw new InvalidOperationException("Please supply a valid groupId to call #Group.");

			Enqueue(new Group(userId, groupId, traits, options));
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
			Track(userId, eventName, null, null);
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
			Track(userId, eventName, properties, null);
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
		/// <param name="options">Options allowing you to set timestamp, anonymousId, target integrations,
		/// and the context of th emessage.</param>
		/// 
		///
		public void Track(string userId, string eventName, Options options)
		{
			Track(userId, eventName, null, options);
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
		/// <param name="options">Options allowing you to set timestamp, anonymousId, target integrations,
		/// and the context of th emessage.</param>
		/// 
		///
		public void Track(string userId, string eventName, Properties properties, Options options)
		{
			if (String.IsNullOrEmpty(userId) && !HasAnonymousId(options))
				throw new InvalidOperationException("Please supply a valid userId or anonymousId to call #Track.");

			if (String.IsNullOrEmpty(eventName))
				throw new InvalidOperationException("Please supply a valid event to call #Track.");

			Enqueue(new Track(userId, eventName, properties, options));
		}

		#endregion

		#region Alias

		/// <summary>
		/// Aliases an anonymous user into an identified user.
		/// </summary>
		/// 
		/// <param name="previousId">The anonymous user's id before they are logged in.</param>
		/// 
		/// <param name="userId">the identified user's id after they're logged in.</param>
		/// 
		public void Alias(string previousId, string userId)
		{
			Alias(previousId, userId, null);
		}

		/// <summary>
		/// Aliases an anonymous user into an identified user.
		/// </summary>
		/// 
		/// <param name="previousId">The anonymous user's id before they are logged in.</param>
		/// 
		/// <param name="userId">the identified user's id after they're logged in.</param>
		///
		/// <param name="options">Options allowing you to set timestamp, anonymousId, target integrations,
		/// and the context of th emessage.</param>
		/// 
		public void Alias(string previousId, string userId, Options options)
		{
			if (String.IsNullOrEmpty(previousId))
				throw new InvalidOperationException("Please supply a valid 'previousId' to Alias.");

			if (String.IsNullOrEmpty(userId))
				throw new InvalidOperationException("Please supply a valid 'userId' to Alias.");

			Enqueue(new Alias(previousId, userId, options));
		}

		#endregion

		#region Page

		/// <summary>
		/// The `page` method let your record whenever a user sees a webpage on 
		/// your website, and attach a `name`, `category` or `properties` to the webpage load. 
		/// </summary>
		///
		/// <param name="userId">The visitor's identifier after they log in, or you know
		/// who they are. By explicitly identifying a user, you tie all of their actions to their identity.
		/// This makes it possible for you to run things like segment-based email campaigns.</param>
		///
		/// <param name="name">The name of the webpage, like "Signup", "Login"</param>
		///
		public void Page(string userId, string name)
		{
			Page (userId, name, null, null, null);
		}

		/// <summary>
		/// The `page` method let your record whenever a user sees a webpage on 
		/// your website, and attach a `name`, `category` or `properties` to the webpage load. 
		/// </summary>
		///
		/// <param name="userId">The visitor's identifier after they log in, or you know
		/// who they are. By explicitly identifying a user, you tie all of their actions to their identity.
		/// This makes it possible for you to run things like segment-based email campaigns.</param>
		///
		/// <param name="name">The name of the webpage, like "Signup", "Login"</param>
		///
		/// <param name="options">Options allowing you to set timestamp, anonymousId, target integrations,
		/// and the context of th emessage.</param>
		///
		public void Page(string userId, string name, Options options)
		{
			Page (userId, name, null, null, options);
		}

		/// <summary>
		/// The `page` method let your record whenever a user sees a webpage on 
		/// your website, and attach a `name`, `category` or `properties` to the webpage load. 
		/// </summary>
		///
		/// <param name="userId">The visitor's identifier after they log in, or you know
		/// who they are. By explicitly identifying a user, you tie all of their actions to their identity.
		/// This makes it possible for you to run things like segment-based email campaigns.</param>
		///
		/// <param name="name">The name of the webpage, like "Signup", "Login"</param>
		/// 
		/// <param name="category">The (optional) category of the webpage, like "Authentication", "Sports"</param>
		///
		public void Page(string userId, string name, string category)
		{
			Page (userId, name, category, null, null);
		}

		/// <summary>
		/// The `page` method let your record whenever a user sees a webpage on 
		/// your website, and attach a `name`, `category` or `properties` to the webpage load. 
		/// </summary>
		///
		/// <param name="userId">The visitor's identifier after they log in, or you know
		/// who they are. By explicitly identifying a user, you tie all of their actions to their identity.
		/// This makes it possible for you to run things like segment-based email campaigns.</param>
		///
		/// <param name="name">The name of the webpage, like "Signup", "Login"</param>
		///
		/// <param name="properties"> A dictionary with items that describe the page
		/// in more detail. This argument is optional, but highly recommended —
		/// you’ll find these properties extremely useful later.</param>
		///
		public void Page(string userId, string name, Properties properties)
		{
			Page (userId, name, null, properties, null);
		}

		/// <summary>
		/// The `page` method let your record whenever a user sees a webpage on 
		/// your website, and attach a `name`, `category` or `properties` to the webpage load. 
		/// </summary>
		///
		/// <param name="userId">The visitor's identifier after they log in, or you know
		/// who they are. By explicitly identifying a user, you tie all of their actions to their identity.
		/// This makes it possible for you to run things like segment-based email campaigns.</param>
		///
		/// <param name="name">The name of the webpage, like "Signup", "Login"</param>
		///
		/// <param name="properties"> A dictionary with items that describe the page
		/// in more detail. This argument is optional, but highly recommended —
		/// you’ll find these properties extremely useful later.</param>
		///
		/// <param name="options">Options allowing you to set timestamp, anonymousId, target integrations,
		/// and the context of th emessage.</param>
		///
		public void Page(string userId, string name, Properties properties, Options options)
		{
			Page (userId, name, null, properties, options);
		}

		/// <summary>
		/// The `page` method let your record whenever a user sees a webpage on 
		/// your website, and attach a `name`, `category` or `properties` to the webpage load. 
		/// </summary>
		///
		/// <param name="userId">The visitor's identifier after they log in, or you know
		/// who they are. By explicitly identifying a user, you tie all of their actions to their identity.
		/// This makes it possible for you to run things like segment-based email campaigns.</param>
		///
		/// <param name="name">The name of the webpage, like "Signup", "Login"</param>
		/// 
		/// <param name="category">The (optional) category of the mobile screen, like "Authentication", "Sports"</param>
		///
		/// <param name="properties"> A dictionary with items that describe the page
		/// in more detail. This argument is optional, but highly recommended —
		/// you’ll find these properties extremely useful later.</param>
		///
		/// <param name="options">Options allowing you to set timestamp, anonymousId, target integrations,
		/// and the context of th emessage.</param>
		///
		public void Page(string userId, string name, string category, Properties properties, Options options)
		{
			if (String.IsNullOrEmpty(userId) && !HasAnonymousId(options))
				throw new InvalidOperationException("Please supply a valid userId or anonymousId to call #Page.");

			if (String.IsNullOrEmpty(name))
				throw new InvalidOperationException("Please supply a valid name to call #Page.");

			Enqueue(new Page(userId, name, category, properties, options));
		}

		#endregion

		#region Screen

		/// <summary>
		/// The `screen` method let your record whenever a user sees a mobile screen on 
		/// your mobile app, and attach a `name`, `category` or `properties` to the screen. 
		/// </summary>
		///
		/// <param name="userId">The visitor's identifier after they log in, or you know
		/// who they are. By
		/// explicitly identifying a user, you tie all of their actions to their identity.
		/// This makes it possible for you to run things like segment-based email campaigns.</param>
		///
		/// <param name="name">The name of the mobile screen, like "Signup", "Login"</param>
		///
		public void Screen(string userId, string name)
		{
			Screen (userId, name, null, null, null);
		}

		/// <summary>
		/// The `screen` method let your record whenever a user sees a mobile screen on 
		/// your mobile app, and attach a `name`, `category` or `properties` to the screen. 
		/// </summary>
		///
		/// <param name="userId">The visitor's identifier after they log in, or you know
		/// who they are. By
		/// explicitly identifying a user, you tie all of their actions to their identity.
		/// This makes it possible for you to run things like segment-based email campaigns.</param>
		///
		/// <param name="name">The name of the mobile screen, like "Signup", "Login"</param>
		///
		/// <param name="options">Options allowing you to set timestamp, anonymousId, target integrations,
		/// and the context of th emessage.</param>
		///
		public void Screen(string userId, string name, Options options)
		{
			Screen (userId, name, null, null, options);
		}

		/// <summary>
		/// The `screen` method let your record whenever a user sees a mobile screen on 
		/// your mobile app, and attach a `name`, `category` or `properties` to the screen. 
		/// </summary>
		///
		/// <param name="userId">The visitor's identifier after they log in, or you know
		/// who they are. By
		/// explicitly identifying a user, you tie all of their actions to their identity.
		/// This makes it possible for you to run things like segment-based email campaigns.</param>
		///
		/// <param name="name">The name of the mobile screen, like "Signup", "Login"</param>
		/// 
		/// <param name="category">The (optional) category of the mobile screen, like "Authentication", "Sports"</param>
		///
		public void Screen(string userId, string name, string category)
		{
			Screen (userId, name, category, null, null);
		}

		/// <summary>
		/// The `screen` method let your record whenever a user sees a mobile screen on 
		/// your mobile app, and attach a `name`, `category` or `properties` to the screen. 
		/// </summary>
		///
		/// <param name="userId">The visitor's identifier after they log in, or you know
		/// who they are. By
		/// explicitly identifying a user, you tie all of their actions to their identity.
		/// This makes it possible for you to run things like segment-based email campaigns.</param>
		///
		/// <param name="name">The name of the mobile screen, like "Signup", "Login"</param>
		///
		/// <param name="properties"> A dictionary with items that describe the screen
		/// in more detail. This argument is optional, but highly recommended —
		/// you’ll find these properties extremely useful later.</param>
		///
		public void Screen(string userId, string name, Properties properties)
		{
			Screen (userId, name, null, properties, null);
		}

		/// <summary>
		/// The `screen` method let your record whenever a user sees a mobile screen on 
		/// your mobile app, and attach a `name`, `category` or `properties` to the screen. 
		/// </summary>
		///
		/// <param name="userId">The visitor's identifier after they log in, or you know
		/// who they are. By
		/// explicitly identifying a user, you tie all of their actions to their identity.
		/// This makes it possible for you to run things like segment-based email campaigns.</param>
		///
		/// <param name="name">The name of the mobile screen, like "Signup", "Login"</param>
		///
		/// <param name="properties"> A dictionary with items that describe the screen
		/// in more detail. This argument is optional, but highly recommended —
		/// you’ll find these properties extremely useful later.</param>
		///
		/// <param name="options">Options allowing you to set timestamp, anonymousId, target integrations,
		/// and the context of th emessage.</param>
		///
		public void Screen(string userId, string name, Properties properties, Options options)
		{
			Screen (userId, name, null, properties, options);
		}

		/// <summary>
		/// The `screen` method let your record whenever a user sees a mobile screen on 
		/// your mobile app, and attach a `name`, `category` or `properties` to the screen. 
		/// </summary>
		///
		/// <param name="userId">The visitor's identifier after they log in, or you know
		/// who they are. By
		/// explicitly identifying a user, you tie all of their actions to their identity.
		/// This makes it possible for you to run things like segment-based email campaigns.</param>
		///
		/// <param name="name">The name of the mobile screen, like "Signup", "Login"</param>
		/// 
		/// <param name="category">The (optional) category of the mobile screen, like "Authentication", "Sports"</param>
		///
		/// <param name="properties"> A dictionary with items that describe the screen
		/// in more detail. This argument is optional, but highly recommended —
		/// you’ll find these properties extremely useful later.</param>
		///
		/// <param name="options">Options allowing you to set timestamp, anonymousId, target integrations,
		/// and the context of th emessage.</param>
		///
		public void Screen(string userId, string name, string category, Properties properties, Options options)
		{
			if (String.IsNullOrEmpty(userId) && !HasAnonymousId(options))
				throw new InvalidOperationException("Please supply a valid userId or anonymousId to call #Screen.");

			if (String.IsNullOrEmpty(name))
				throw new InvalidOperationException("Please supply a valid name to call #Screen.");

			Enqueue(new Screen(userId, name, category, properties, options));
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
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="Segment.Client"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="Segment.Client"/> in an unusable state. After calling
		/// <see cref="Dispose"/>, you must release all references to the <see cref="Segment.Client"/> so the garbage
		/// collector can reclaim the memory that the <see cref="Segment.Client"/> was occupying.</remarks>
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

            unchecked
            {
                this.Statistics.Submitted += 1;
            }
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
		internal static bool HasAnonymousId(Options options)
		{
			return options != null &&  !String.IsNullOrEmpty(options.AnonymousId);
		}

        #endregion
    }
}
