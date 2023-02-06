using System;

namespace RudderStack.Model
{
    public class RudderOptions
    {
        public string AnonymousId { get; private set; }
        public Dict Integrations { get; private set; }
        public DateTime? Timestamp { get; private set; }
        public RudderContext Context { get; private set; }

        /// <summary>
        /// Options object that allows the specification of a timestamp,
        /// an anonymousId, a context, or target integrations.
        /// </summary>
        public RudderOptions()
        {
            this.Integrations = new Dict();
            this.Context = new RudderContext();
        }

        /// <summary>
        /// Sets the anonymousId of the user. This is typically a cookie session id that identifies
        /// a visitor before they have logged in.
        /// </summary>
        /// <returns>This Options object for chaining.</returns>
        /// <param name="anonymousId">The visitor's anonymousId.</param>
        public RudderOptions SetAnonymousId(string anonymousId)
        {
            this.AnonymousId = anonymousId;
            return this;
        }

        /// <summary>
        /// Sets the timestamp of when an analytics call occurred. The timestamp is primarily used for
        /// historical imports or if this event happened in the past. The timestamp is not required,
        /// and if it's not provided, our servers will timestamp the call as if it just happened.
        /// </summary>
        /// <returns>This Options object for chaining.</returns>
        /// <param name="timestamp">The call's timestamp.</param>
        public RudderOptions SetTimestamp(DateTime? timestamp)
        {
            this.Timestamp = timestamp;
            return this;
        }

        /// <summary>
        /// Sets the context of this analytics call. Context contains information about the environment
        /// such as the app, the user agent, ip, etc ..
        /// </summary>
        /// <returns>This Options object for chaining.</returns>
        /// <param name="context">The visitor's context.</param>
        public RudderOptions SetContext(RudderContext context)
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
        public RudderOptions SetIntegration(string integration, bool enabled)
        {
            this.Integrations.Add(integration, enabled);
            return this;
        }

        /// <summary>
        /// Enable destination specific options for integration.
        ///   new Options()
        ///     .Integration("Vero", new Model.Dict() {
        ///         "tags", new Model.Dict() {
        ///             { "id", "235FAG" },
        ///             { "action", "add" },
        ///             { "values", new string[] {"warriors", "giants", "niners"} }
        ///         }
        ///     });
        /// </summary>
        /// <param name="integration">The integration name.</param>
        /// <param name="value">Dict value</param>
        public RudderOptions SetIntegration(string integration, Dict value)
        {
            this.Integrations.Add(integration, value);
            return this;
        }

    }
}
