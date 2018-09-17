using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;

using Segment;
using Segment.Model;

namespace Segment.Test
{
    [TestClass]
    public class ClientTests
    {
        Client client;

        [TestInitialize]
        public void Init()
        {
            client = new Client("foo");
        }

        [TestMethod]
        public void TrackTestNetPortable()
        {
            // verify it doesn't fail for a null options
            client.Screen("bar", "qaz", null, null);
        }
    }
}
