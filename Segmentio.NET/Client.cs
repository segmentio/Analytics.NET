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

        private string apiKey;
        private bool _initialized;

        public Statistics Statistics { get; set; }

        #region Events

        public delegate void FailedHandler(BaseAction action, string reason);
        public delegate void SucceededHandler(BaseAction action);

        public event FailedHandler Failed;
        public event SucceededHandler Succeeded;

        #endregion

        #region Initialization

        public Client()
        {
            requestHandler = new BatchingRequestHandler(new IFlushTrigger[] {
                new QueueSizeFlushTrigger(50),
                new TimeSinceLastFlushedTrigger(TimeSpan.FromSeconds(30))
            });

            this.Statistics = new Statistics();
        }

        public void Initialize(string apiKey)
        {
            if (!_initialized)
            {
                if (String.IsNullOrEmpty(apiKey))
                    throw new InvalidOperationException("Please supply a valid apiKey to initialize.");

                this.apiKey = apiKey;
                this._initialized = true;
                
                requestHandler.Initialize(this, apiKey);
            }
        }

        #endregion

        #region Properties

        public string ApiKey
        {
            get
            {
                return apiKey;
            }
        }

        #endregion

        #region Utils

        private Dictionary<string, object> clean(Dictionary<string, object> properties)
        {
            Dictionary<string, object> cleaned = new Dictionary<string,object>();

            if (properties != null)
            {
                foreach (var pair in properties)
                {
                    if (pair.Value is string || pair.Value is bool || IsNumeric(pair.Value))
                    {
                        cleaned.Add(pair.Key, pair.Value);
                    }
                    else if (pair.Value is DateTime)
                    {
                        cleaned.Add(pair.Key, ((DateTime)pair.Value).ToString("o"));
                    }
                    else
                    {
                        // do nothing here, this parameter is invalid and just gets removed
                    }
                }
            }

            return cleaned;
        }

        private static bool IsNumeric(object expression)
        {
            if (expression == null)
                return false;

            double number;
            return Double.TryParse(Convert.ToString(expression, CultureInfo.InvariantCulture), 
                System.Globalization.NumberStyles.Any, NumberFormatInfo.InvariantInfo, out number);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Identifying a visitor ties all of their actions to an ID you 
        /// recognize and records visitor traits you can segment by.
        /// </summary>
        /// 
        /// <param name="sessionId">The visitor's anonymous identifier until they log in, 
        /// or until your system knows who they are. In web systems, 
        /// this is usually the ID of this user in the sessions table</param>
        /// 
        /// <param name="userId">The visitor's identifier after they log in, or you know 
        /// who they are. This is usually an email, but any unique ID will work. By 
        /// explicitly identifying a user, you tie all of their actions to their identity.</param>
        /// 
        public void Identify(string sessionId, string userId)
        {
            Identify(sessionId, userId, null, null, null);
        }

        /// <summary>
        /// Identifying a visitor ties all of their actions to an ID you 
        /// recognize and records visitor traits you can segment by.
        /// </summary>
        /// 
        /// <param name="sessionId">The visitor's anonymous identifier until they log in, 
        /// or until your system knows who they are. In web systems, 
        /// this is usually the ID of this user in the sessions table</param>
        /// 
        /// <param name="userId">The visitor's identifier after they log in, or you know 
        /// who they are. This is usually an email, but any unique ID will work. By 
        /// explicitly identifying a user, you tie all of their actions to their identity.</param>
        /// 
        /// <param name="traits">A dictionary with keys like “Subscription Plan” or 
        /// “Favorite Genre”. You can segment your users by any trait you record. 
        /// Pass in values in key-value format. String key, then its value 
        /// { String, Integer, Boolean, Double, or Date are acceptable types for a value. } 
        /// So, traits array could be: "Subscription Plan", "Premium", 
        /// "Friend Count", 13 , and so forth.  </param>
        public void Identify(string sessionId, string userId, Dictionary<String, object> traits)
        {
            Identify(sessionId, userId, traits, null, null);
        }

        /// <summary>
        /// Identifying a visitor ties all of their actions to an ID you 
        /// recognize and records visitor traits you can segment by.
        /// </summary>
        /// 
        /// <param name="sessionId">The visitor's anonymous identifier until they log in, 
        /// or until your system knows who they are. In web systems, 
        /// this is usually the ID of this user in the sessions table</param>
        /// 
        /// <param name="userId">The visitor's identifier after they log in, or you know 
        /// who they are. This is usually an email, but any unique ID will work. By 
        /// explicitly identifying a user, you tie all of their actions to their identity.</param>
        /// 
        /// <param name="traits">A dictionary with keys like “Subscription Plan” or 
        /// “Favorite Genre”. You can segment your users by any trait you record. 
        /// Pass in values in key-value format. String key, then its value 
        /// { String, Integer, Boolean, Double, or Date are acceptable types for a value. } 
        /// So, traits array could be: "Subscription Plan", "Premium", 
        /// "Friend Count", 13 , and so forth.  </param>
        /// 
        /// <param name="context"> A dictionary with additional information thats related to the visit. 
        /// Examples are userAgent, and IP address of the visitor. 
        /// Feel free to pass in null if you don't have this information.</param>
        /// 
        public void Identify(string sessionId, string userId, Dictionary<String, object> traits,
            Context context)
        {
            Identify(sessionId, userId, traits, context, null);
        }

        /// <summary>
        /// Identifying a visitor ties all of their actions to an ID you 
        /// recognize and records visitor traits you can segment by.
        /// </summary>
        /// 
        /// <param name="sessionId">The visitor's anonymous identifier until they log in, 
        /// or until your system knows who they are. In web systems, 
        /// this is usually the ID of this user in the sessions table</param>
        /// 
        /// <param name="userId">The visitor's identifier after they log in, or you know 
        /// who they are. This is usually an email, but any unique ID will work. By 
        /// explicitly identifying a user, you tie all of their actions to their identity.</param>
        /// 
        /// <param name="traits">A dictionary with keys like “Subscription Plan” or 
        /// “Favorite Genre”. You can segment your users by any trait you record. 
        /// Pass in values in key-value format. String key, then its value 
        /// { String, Integer, Boolean, Double, or Date are acceptable types for a value. } 
        /// So, traits array could be: "Subscription Plan", "Premium", 
        /// "Friend Count", 13 , and so forth.  </param>
        /// 
        /// <param name="context"> A dictionary with additional information thats related to the visit. 
        /// Examples are userAgent, and IP address of the visitor. 
        /// Feel free to pass in null if you don't have this information.</param>
        /// 
        /// <param name="timestamp">  If this event happened in the past, the timestamp 
        /// can be used to designate when the identification happened. Careful with this one,
        /// if it just happened, leave it null.</param>
        /// 
        /// 
        public void Identify(string sessionId, string userId, Dictionary<String, object> traits, 
            Context context, DateTime? timestamp)
        {
            if (!_initialized) throw new NotInitializedException();

            if (String.IsNullOrEmpty(sessionId) && String.IsNullOrEmpty(userId))
                throw new InvalidOperationException("Please supply either a valid sessionId or userId (or both) to Identify.");

            traits = clean(traits);

            Identify identify = new Identify(sessionId, userId, traits, context, timestamp);
            
            requestHandler.Process(identify);
        }

        /// <summary>
        /// Whenever a user triggers an event on your site, you’ll want to track it 
        /// so that you can analyze and segment by those events later.
        /// </summary>
        /// 
        /// <param name="sessionId">The visitor's anonymous identifier until they log in, 
        /// or until your system knows who they are. In web systems, 
        /// this is usually the ID of this user in the sessions table</param>
        /// 
        /// <param name="userId">The visitor's identifier after they log in, or you know 
        /// who they are. This is usually an email, but any unique ID will work. By 
        /// explicitly identifying a user, you tie all of their actions to their identity. 
        /// This makes it possible for you to run things like segment-based email campaigns.</param>
        /// 
        /// <param name="eventName">The event name you are tracking. It is recommended 
        /// that it is in human readable form. For example, "Bought T-Shirt" 
        /// or "Started an exercise"</param>
        /// 
        public void Track(string sessionId, string userId, string eventName)
        {
            Track(sessionId, userId, eventName, null, null);
        }

        /// <summary>
        /// Whenever a user triggers an event on your site, you’ll want to track it 
        /// so that you can analyze and segment by those events later.
        /// </summary>
        /// 
        /// <param name="sessionId">The visitor's anonymous identifier until they log in, 
        /// or until your system knows who they are. In web systems, 
        /// this is usually the ID of this user in the sessions table</param>
        /// 
        /// <param name="userId">The visitor's identifier after they log in, or you know 
        /// who they are. This is usually an email, but any unique ID will work. By 
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
        public void Track(string sessionId, string userId, string eventName,
            Dictionary<string, object> properties)
        {
            Track(sessionId, userId, eventName, properties, null);
        }


        /// <summary>
        /// Whenever a user triggers an event on your site, you’ll want to track it 
        /// so that you can analyze and segment by those events later.
        /// </summary>
        /// 
        /// <param name="sessionId">The visitor's anonymous identifier until they log in, 
        /// or until your system knows who they are. In web systems, 
        /// this is usually the ID of this user in the sessions table</param>
        /// 
        /// <param name="userId">The visitor's identifier after they log in, or you know 
        /// who they are. This is usually an email, but any unique ID will work. By 
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
        public void Track(string sessionId, string userId, string eventName,
            Dictionary<string, object> properties, DateTime? timestamp)
        {
            if (!_initialized) throw new NotInitializedException();

            if (String.IsNullOrEmpty(sessionId) && String.IsNullOrEmpty(userId))
                throw new InvalidOperationException("Please supply either a valid sessionId or userId (or both) to Track.");

            if (String.IsNullOrEmpty(eventName))
                throw new InvalidOperationException("Please supply a valid eventName to Track.");

            properties = clean(properties);

            Track track = new Track(sessionId, userId, eventName, properties, timestamp);

            requestHandler.Process(track);
        }

        #endregion

        #region Event API

        internal void RaiseSuccess(BaseAction action)
        {
            if (Succeeded != null) Succeeded(action);
        }

        internal void RaiseFailure(BaseAction action, string reason)
        {
            if (Failed != null) Failed(action, reason);
        }

        #endregion
    }
}
