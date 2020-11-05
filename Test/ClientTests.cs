using NUnit.Framework;

using System.Collections.Generic;

using RudderStack;
using RudderStack.Model;

namespace RudderStack.Test
{
    [TestFixture()]
    public class ClientTests
    {
        Client client;

        [SetUp]
        public void Init()
        {
            client = new Client("foo");
        }

        [Test()]
        public void TrackTest()
        {
            // verify it doesn't fail for a null options
            client.Screen("bar", "qaz", null, null);
        }
    }
}

