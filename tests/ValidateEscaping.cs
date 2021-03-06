using Creator.Helpers;
using Creator.Models.Objects;
using NUnit.Framework;
using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Creator.Tests
{
    public class ValidateEscaping
    {
        [Test]
        [Category("Escaping")]
        [TestCase("my,string", "\"my,string\"")]
        [TestCase("string", "string")]
        [TestCase("string with spaces", "string with spaces")]
        [TestCase("string\"", "\"string\"\"\"")]
        [TestCase(null, null)]
        public void ConvertMilestone(string input, string expected)
        {
            Assert.AreEqual(expected, StringHelpers.EncodeString(input));
        }
    }
}