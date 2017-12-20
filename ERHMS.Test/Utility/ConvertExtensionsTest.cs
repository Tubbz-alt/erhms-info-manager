﻿using ERHMS.Utility;
using NUnit.Framework;
using System;

namespace ERHMS.Test.Utility
{
    public class ConvertExtensionsTest
    {
        [Serializable]
        private class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public bool Male { get; set; }
        }

        [Test]
        public void ToDegreesTest()
        {
            Assert.AreEqual(0.0, ConvertExtensions.ToDegrees(0.0));
            Assert.AreEqual(180.0 / Math.PI, ConvertExtensions.ToDegrees(1.0));
            Assert.AreEqual(360.0, ConvertExtensions.ToDegrees(2.0 * Math.PI));
        }

        [Test]
        public void ToRadiansTest()
        {
            Assert.AreEqual(0.0, ConvertExtensions.ToRadians(0.0));
            Assert.AreEqual(Math.PI / 180.0, ConvertExtensions.ToRadians(1.0));
            Assert.AreEqual(2.0 * Math.PI, ConvertExtensions.ToRadians(360.0));
        }

        [Test]
        public void ToAndFromBase64StringTest()
        {
            Assert.AreEqual("", ConvertExtensions.ToBase64String(null));
            Assert.IsNull(ConvertExtensions.FromBase64String(""));
            Person original = new Person
            {
                Name = "John Doe",
                Age = 20,
                Male = true
            };
            Person converted = (Person)ConvertExtensions.FromBase64String(ConvertExtensions.ToBase64String(original));
            Assert.AreEqual(original.Name, converted.Name);
            Assert.AreEqual(original.Age, converted.Age);
            Assert.AreEqual(original.Male, converted.Male);
        }

        [Test]
        public void ToNullableGuidTest()
        {
            Assert.IsNull(ConvertExtensions.ToNullableGuid(null));
            Assert.IsNull(ConvertExtensions.ToNullableGuid(""));
            Assert.AreEqual(Guid.Empty, ConvertExtensions.ToNullableGuid("00000000-0000-0000-0000-000000000000"));
            Assert.Catch(() =>
            {
                ConvertExtensions.ToNullableGuid("xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx");
            });
        }
    }
}
