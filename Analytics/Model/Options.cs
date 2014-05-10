using System;

using Segment.Model;

namespace Segment.Model
{
	public class Options : Dict
	{
		internal string AnonymousId { get; private set; }
		internal Dict Integrations { get; private set; }
		internal Context Context { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Segment.Options"/> class.
		/// </summary>
		public Options ()
		{
			this.Integrations = new Dict ();
			this.Context = new Context ();
		}

		/// <summary>
		/// Sets the anonymousId of the user. This is typically a cookie session id that identifies
		/// a visitor before they have logged in.
		/// </summary>
		/// <returns>This Options object for chaining.</returns>
		/// <param name="anonymousId">The visitor's anonymousId.</param>
		public Options SetAnonymousId (string anonymousId)
		{
			this.AnonymousId = anonymousId;
			return this;
		}

		/// <summary>
		/// Sets the context of this analytics call. Context contains information about the environemtn
		/// such as the app, the user agent, ip, etc ..
		/// </summary>
		/// <returns>This Options object for chaining.</returns>
		/// <param name="anonymousId">The visitor's context.</param>
		public Options SetContext (Context context)
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
		public Options Integration (string integration, bool enabled)
		{
			this.Integrations.Add (integration, enabled);
			return this;
		}

	}
}