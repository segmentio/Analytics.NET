using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Segmentio.Exception;
using Segmentio.Model;
using Segmentio.Request;
using Segmentio.Trigger;
using Segmentio.Stats;

namespace Segmentio
{
    /// <summary>
    /// A Segment.io REST client
    /// </summary>
    public class Client
    {
        private IRequestHandler requestHandler;
        private string secret;
        private Options options;

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
            this.secret = secret;
            this.options = options;

            requestHandler = new BatchingRequestHandler(new IFlushTrigger[] {
                new FlushAtTrigger(options.FlushAt),
                new FlushAfterTrigger(options.FlushAfter)
            });

            requestHandler.Initialize(this, secret);
        }

        #endregion

        #region Properties

        public string Secret
        {
            get
            {
                return secret;
            }
        }


        public Options Options
        {
            get
            {
                return options;
            }
        }

        #endregion

        #region Utils


        #endregion

        #region Public Methods

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

            requestHandler.Process(identify);
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

            requestHandler.Process(track);
        }

        #endregion

        #region Flush

        /// <summary>
        /// Triggers a queue flush
        /// </summary>
        public void Flush()
        {
            requestHandler.Flush();
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
