﻿using ERHMS.Utility;
using NUnit.Framework;
using System;

namespace ERHMS.Test.Utility
{
    public class ConvertExtensionsTest
    {
        [Test]
        public void ToNullableGuidTest()
        {
            Assert.AreEqual(Guid.Empty, ConvertExtensions.ToNullableGuid("00000000-0000-0000-0000-000000000000"));
            Assert.IsNull(ConvertExtensions.ToNullableGuid("xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"));
            Assert.IsNull(ConvertExtensions.ToNullableGuid(""));
            Assert.IsNull(ConvertExtensions.ToNullableGuid(null));
        }
    }
}