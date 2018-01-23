﻿using ERHMS.Utility;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Test.Utility
{
    public class StringExtensionsTest
    {
        private static class NewLines
        {
            public const string Windows = "\r\n";
            public const string Unix = "\n";
            public const string Mac = "\r";
        }

        [Test]
        public void ContainsTest()
        {
            Assert.IsTrue("Hello, world!".Contains("world", StringComparison.Ordinal));
            Assert.IsFalse("Hello, world!".Contains("WORLD", StringComparison.Ordinal));
            Assert.IsTrue("Hello, world!".Contains("WORLD", StringComparison.OrdinalIgnoreCase));
        }

        private string GetLines(string newLine1, string newLine2, string newLine3)
        {
            return string.Format("A{0}B{0}{1}C{0}{1}{2}D", newLine1, newLine2, newLine3);
        }

        private string GetLines(string newLine)
        {
            return GetLines(newLine, newLine, newLine);
        }

        [Test]
        public void SplitLinesTest()
        {
            ICollection<string> expected = new string[] { "A", "B", "", "C", "", "", "D" };
            SplitLinesTest(expected, GetLines(NewLines.Windows));
            SplitLinesTest(expected, GetLines(NewLines.Unix));
            SplitLinesTest(expected, GetLines(NewLines.Mac));
            SplitLinesTest(expected, GetLines(NewLines.Windows, NewLines.Unix, NewLines.Mac));
        }

        private void SplitLinesTest(IEnumerable<string> expected, string lines)
        {
            CollectionAssert.AreEqual(expected, lines.SplitLines());
        }

        [Test]
        public void MakeUniqueTest()
        {
            ICollection<string> values = new string[] { "test", "test (2)", "TEST (3)" };
            string format = "{0} ({1})";
            Assert.AreEqual("test", "test".MakeUnique(format, value => false));
            Assert.AreEqual("test (3)", "test".MakeUnique(format, value => values.Contains(value)));
            Assert.AreEqual("test (4)", "test".MakeUnique(format, value => values.Contains(value, StringComparer.OrdinalIgnoreCase)));
        }
    }
}
