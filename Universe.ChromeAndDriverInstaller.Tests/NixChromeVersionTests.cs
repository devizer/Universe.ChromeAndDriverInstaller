using System;
using NUnit.Framework;
using Universe.NUnitTests;

namespace Universe.ChromeAndDriverInstaller.Tests
{

    public class NixChromeVersionTests : NUnitTestsBase
    {
        [Test]
        public void TestValidWord()
        {
            var actual = CurrentChromeVersionClient.TryParseVersion("1.2.3.4");
            Assert.AreEqual(new Version(1, 2, 3, 4), actual);
        }

        [Test]
        public void TestInvalidWord()
        {
            var actual = CurrentChromeVersionClient.TryParseVersion("hi");
            Assert.IsNull(actual);
        }

        [Test]
        public void TestValidOutput()
        {
            var actual = CurrentChromeVersionClient.ParseVersionByChromeOutput("Google Chrome 7.8.9.1 Wow!");
            Assert.AreEqual("7.8.9.1", actual);
        }

        [Test]
        public void TestValidOutput2()
        {
            var actual = CurrentChromeVersionClient.ParseVersionByChromeOutput("Google Chrome 7.8.9.0 Wow!");
            Assert.AreEqual("7.8.9.0", actual);
        }

        [Test]
        public void TestInvalidOutput()
        {
            var actual = CurrentChromeVersionClient.ParseVersionByChromeOutput("Google Chrome 106");
            Assert.IsNull(actual);
        }

    }
}