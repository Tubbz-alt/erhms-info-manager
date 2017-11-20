﻿using ERHMS.Test.Infrastructure;
using ERHMS.Utility;
using NUnit.Framework;
using System;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace ERHMS.Test.Utility
{
    public class EnumExtensionsTest
    {
        private enum Number
        {
            NaN,

            [Description("1")]
            One,

            [Description("2")]
            Two,

            [Description("3")]
            Three
        }

        [Test]
        public void ParseTest()
        {
            Assert.AreEqual(Number.NaN, EnumExtensions.Parse<Number>("NaN"));
            Assert.AreEqual(Number.One, EnumExtensions.Parse<Number>("One"));
            Assert.Catch(() =>
            {
                EnumExtensions.Parse<Number>(null);
            });
            Assert.Catch(() =>
            {
                EnumExtensions.Parse<Number>("Four");
            });
        }

        [Test]
        public void GetValuesTest()
        {
            AssertExtensions.AreEqual(EnumExtensions.GetValues<Number>(), Number.NaN, Number.One, Number.Two, Number.Three);
        }

        [Test]
        public void ToDescriptionTest()
        {
            Assert.IsNull(EnumExtensions.ToDescription(Number.NaN));
            Assert.AreEqual("1", EnumExtensions.ToDescription(Number.One));
        }

        [Test]
        public void FromDescriptionTest()
        {
            Assert.AreEqual(Number.NaN, EnumExtensions.FromDescription<Number>(null));
            Assert.AreEqual(Number.One, EnumExtensions.FromDescription<Number>("1"));
            Assert.Catch(() =>
            {
                EnumExtensions.FromDescription<DayOfWeek>("Sunday");
            });
            Assert.Catch(() =>
            {
                EnumExtensions.FromDescription<DayOfWeek>(null);
            });
        }
    }
}
