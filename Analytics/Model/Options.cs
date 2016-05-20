//-----------------------------------------------------------------------
// <copyright file="Options.cs" company="Segment">
//     Copyright (c) Segment. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Segment.Model
{
    using System;

    public class Options
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Options" /> class.
        /// Options object that allows the specification of a time stamp, 
        /// an anonymousId, a context, or target integrations.
        /// </summary>
        public Options()
        {
            this.Integrations = new Dict();
            this.Context = new Context();
        }

        public string AnonymousId { get; private set; }

        public Dict Integrations { get; private set; }

        public DateTime? Timestamp { get; private set; }

        public Context Context { get; private set; }

        /// <summary>
        /// Sets the anonymousId of the user. This is typically a cookie session id that identifies
        /// a visitor before they have logged in.
        /// </summary>
        /// <returns>This Options object for chaining.</returns>
        /// <param name="anonymousId">The visitor's anonymousId.</param>
        public Options SetAnonymousId(string anonymousId)
        {
            this.AnonymousId = anonymousId;
            return this;
        }

        /// <summary>
        /// Sets the timestamp of when an analytics call occurred. The timestamp is primarily used for 
        /// historical imports or if this event happened in the past. The timestamp is not required, 
        /// and if its not provided, our servers will timestamp the call as if it just happened.
        /// </summary>
        /// <param name="timestamp">The call's timestamp.</param>
        /// <returns>This Options object for chaining.</returns>
        public Options SetTimestamp(DateTime? timestamp)
        {
            this.Timestamp = timestamp;
            return this;
        }

        /// <summary>
        /// Sets the context of this analytics call. Context contains information about the environment
        /// such as the app, the user agent, IP, etc ..
        /// </summary>
        /// <returns>This Options object for chaining.</returns>
        /// <param name="context">The visitor's context.</param>
        public Options SetContext(Context context)
        {
            this.Context = context;
            return this;
        }

        /// <summary>
        /// Determines which integrations this messages goes to.
        ///   new Options()
        ///     .Integration("All", false)
        ///     .Integration("Mixpanel", true)
        /// will send a message to only Mixpanel.
        /// </summary>
        /// <param name="integration">The integration name.</param>
        /// <param name="enabled">If set to <c>true</c>, then the integration is enabled.</param>
        /// <returns>This Options object.</returns>
        public Options SetIntegration(string integration, bool enabled)
        {
            this.Integrations.Add(integration, enabled);
            return this;
        }
    }
}
