using Microsoft.VisualStudio.TestTools.UnitTesting;
using Percheron.Core.Irc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Percheron.Test
{
    [TestClass]
    public class IrcMessageParseTests
    {
        [TestMethod]
        public void CanParseTrailingWithNoMiddle()
        {
            var inputString = "PING :server.com";
            var parsed = new IrcMessage(inputString);
            Assert.AreEqual(inputString, parsed.ToString());
            Assert.AreEqual(0, parsed.Middles.Count());
            Assert.IsNotNull(parsed.Trailing);
        }

        [TestMethod]
        public void CanParseMiddleWithoutTrailing()
        {
            var inputString = ":username!username@username.server.com JOIN #channel";
            var parsed = new IrcMessage(inputString);
            Assert.AreEqual(inputString, parsed.ToString());
            Assert.AreEqual(1, parsed.Middles.Count());
            Assert.IsNull(parsed.Trailing);
        }

        [TestMethod]
        public void CanParseMultipleMiddles()
        {
            var inputString = ":server.com CAP * ACK :server.com/cap server.com/ack";
            var parsed = new IrcMessage(inputString);
            Assert.AreEqual(inputString, parsed.ToString());
            Assert.AreEqual(2, parsed.Middles.Count());
        }

        [TestMethod]
        public void CanParseIrc3Tags()
        {
            var inputString = "@badges=premium/1;color=#AAAAAA;display-name=UserName;emote-sets=0,42,19194,375457;mod=0;subscriber=0;user-type= :server.com USERSTATE #channel";
            var parsed = new IrcMessage(inputString);
            Assert.AreEqual(inputString, parsed.ToString());
            Assert.AreEqual(7, parsed.Tags.Count());
        }
    }
}
