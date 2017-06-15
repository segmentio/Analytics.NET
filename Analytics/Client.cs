using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Segment.Flush;
using Segment.Model;
using Segment.Request;
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
        public async Task Identify(string userId, IDictionary<string, object> traits)
        {
            await Identify(userId, traits, null).ConfigureAwait(false);
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
		public async Task Identify(string userId, IDictionary<string, object> traits, Options options)
        {
			if (String.IsNullOrEmpty(userId) && !HasAnonymousId(options))
				throw new InvalidOperationException("Please supply a valid userId to Identify.");

			await Enqueue(new Identify(userId, traits, options)).ConfigureAwait(false);
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
		public async Task Group(string userId, string groupId, Options options)
		{
			await Group (userId, groupId, null, options).ConfigureAwait(false);
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
		public async Task Group(string userId, string groupId, IDictionary<string, object> traits)
		{
			await Group (userId, groupId, traits, null).ConfigureAwait(false);
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
		public async Task Group(string userId, string groupId, IDictionary<string, object> traits, Options options)
		{
			if (String.IsNullOrEmpty(userId) && !HasAnonymousId(options))
				throw new InvalidOperationException("Please supply a valid userId or anonymousId to call #Group.");

			if (String.IsNullOrEmpty(groupId))
				throw new InvalidOperationException("Please supply a valid groupId to call #Group.");

			await Enqueue(new Group(userId, groupId, traits, options)).ConfigureAwait(false);
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
		public async Task Track(string userId, string eventName)
        {
			await Track(userId, eventName, null, null).ConfigureAwait(false);
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
		public async Task Track(string userId, string eventName, IDictionary<string, object> properties)
        {
			await Track(userId, eventName, properties, null).ConfigureAwait(false);
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
		public async Task Track(string userId, string eventName, Options options)
		{
			await Track(userId, eventName, null, options).ConfigureAwait(false);
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
		public async Task Track(string userId, string eventName, IDictionary<string, object> properties, Options options)
		{
			if (String.IsNullOrEmpty(userId) && !HasAnonymousId(options))
				throw new InvalidOperationException("Please supply a valid userId or anonymousId to call #Track.");

			if (String.IsNullOrEmpty(eventName))
				throw new InvalidOperationException("Please supply a valid event to call #Track.");

			await Enqueue(new Track(userId, eventName, properties, options)).ConfigureAwait(false);
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
		public async Task Alias(string previousId, string userId)
		{
			await Alias(previousId, userId, null).ConfigureAwait(false);
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
		public async Task Alias(string previousId, string userId, Options options)
		{
			if (String.IsNullOrEmpty(previousId))
				throw new InvalidOperationException("Please supply a valid 'previousId' to Alias.");

			if (String.IsNullOrEmpty(userId))
				throw new InvalidOperationException("Please supply a valid 'userId' to Alias.");

			await Enqueue(new Alias(previousId, userId, options)).ConfigureAwait(false);
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
		public async Task Page(string userId, string name)
		{
			await Page (userId, name, null, null, null).ConfigureAwait(false);
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
		public async Task Page(string userId, string name, Options options)
		{
			await Page (userId, name, null, null, options).ConfigureAwait(false);
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
		public async Task Page(string userId, string name, string category)
		{
			await Page (userId, name, category, null, null).ConfigureAwait(false);
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
		public async Task Page(string userId, string name, IDictionary<string, object> properties)
		{
			await Page (userId, name, null, properties, null).ConfigureAwait(false);
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
		public async Task Page(string userId, string name, IDictionary<string, object> properties, Options options)
		{
			await Page (userId, name, null, properties, options).ConfigureAwait(false);
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
		public async Task Page(string userId, string name, string category, IDictionary<string, object> properties, Options options)
		{
			if (String.IsNullOrEmpty(userId) && !HasAnonymousId(options))
				throw new InvalidOperationException("Please supply a valid userId or anonymousId to call #Page.");

			if (String.IsNullOrEmpty(name))
				throw new InvalidOperationException("Please supply a valid name to call #Page.");

			await Enqueue(new Page(userId, name, category, properties, options)).ConfigureAwait(false);
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
		public async Task Screen(string userId, string name)
		{
			await Screen (userId, name, null, null, null).ConfigureAwait(false);
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
		public async Task Screen(string userId, string name, Options options)
		{
			await Screen (userId, name, null, null, options).ConfigureAwait(false);
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
		public async Task Screen(string userId, string name, string category)
		{
			await Screen (userId, name, category, null, null).ConfigureAwait(false);
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
		public async Task Screen(string userId, string name, IDictionary<string, object> properties)
		{
			await Screen (userId, name, null, properties, null).ConfigureAwait(false);
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
		public async Task Screen(string userId, string name, IDictionary<string, object> properties, Options options)
		{
			await Screen (userId, name, null, properties, options).ConfigureAwait(false);
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
		public async Task Screen(string userId, string name, string category, IDictionary<string, object> properties, Options options)
		{
			if (String.IsNullOrEmpty(userId) && !HasAnonymousId(options))
				throw new InvalidOperationException("Please supply a valid userId or anonymousId to call #Screen.");

			if (String.IsNullOrEmpty(name))
				throw new InvalidOperationException("Please supply a valid name to call #Screen.");

			await Enqueue(new Screen(userId, name, category, properties, options)).ConfigureAwait(false);
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

        private async Task Enqueue(BaseAction action)
        {
            await _flushHandler.Process(action).ConfigureAwait(false);

            this.Statistics.Submitted = Statistics.Increment(this.Statistics.Submitted);
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
