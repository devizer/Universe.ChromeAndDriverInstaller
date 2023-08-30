using System;
using NUnit.Framework;
using Universe.NUnitTests;

namespace Universe.ChromeAndDriverInstaller.Tests
{
    public class VersionsPerChannelTests : NUnitTestsBase
    {
        [Test]
        [TestCase("Stable", true)]
        [TestCase("Beta", true)]
        [TestCase("Dev", false)]
        [TestCase("Canary", false)]
        public void TestKnownChannels(string channel, bool isRequired)
        {
            LastKnownGoodVersionsClient client = new LastKnownGoodVersionsClient();
            var versions = client.ReadVersions();
            if (true != versions?.Dictionary.TryGetValue(channel, out var rawVersion)) rawVersion = null;
            Console.WriteLine($"Channel '{channel}': [{rawVersion}]");
            if (rawVersion == null && isRequired)
                Assert.Fail($"Missing required channel '{channel}'");
        }

        [Test]
        public void StableVersionExists()
        {
            LastKnownGoodVersionsClient client = new LastKnownGoodVersionsClient();
            var versions = client.ReadVersions();
            Console.WriteLine($"Stable version: [{versions.TryGetStableVersion()}]");
            Assert.IsNotNull(versions.TryGetStableVersion());
        }

        [Test]
        public void BetaVersionExists()
        {
            LastKnownGoodVersionsClient client = new LastKnownGoodVersionsClient();
            var versions = client.ReadVersions();
            Console.WriteLine($"Beta version: [{versions.TryGetBetaVersion()}]");
            Assert.IsNotNull(versions.TryGetBetaVersion());
        }

    }
}