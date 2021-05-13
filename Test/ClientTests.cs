using NUnit.Framework;

using System.Collections.Generic;

using RudderStack;
using RudderStack.Model;

namespace RudderStack.Test
{
    [TestFixture()]
    public class ClientTests
    {
        RudderClient client;

        [SetUp]
        public void Init()
        {
            client = new RudderClient("foo");
        }

        [Test()]
        public void TrackTest()
        {
            // verify it doesn't fail for a null options
            client.Screen("bar", "qaz", null, null);
        }
    }
}

