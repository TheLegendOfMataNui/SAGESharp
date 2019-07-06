/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using FluentAssertions;
using NUnit.Framework;
using System;

namespace SAGESharp
{
    class BKDTests
    {
        [Test]
        public void Test_Setting_A_Null_Entry_List_To_A_BKD_Object() => new BKD()
            .Invoking(o => o.Entries = null)
            .Should()
            .Throw<ArgumentNullException>()
            .Where(e => e.Message.Contains("value"));

        [Test]
        public void Test_Setting_A_Null_TCBQuaternion_List_To_A_BKDEntry_Object() => new BKDEntry()
            .Invoking(e => e.TCBQuaternionData = null)
            .Should()
            .Throw<ArgumentNullException>()
            .Where(e => e.Message.Contains("value"));

        [Test]
        public void Test_Setting_A_Null_TCBInpterpolationData1_List_To_A_BKDEntry_Object() => new BKDEntry()
            .Invoking(e => e.TCBInterpolatorData1 = null)
            .Should()
            .Throw<ArgumentNullException>()
            .Where(e => e.Message.Contains("value"));

        [Test]
        public void Test_Setting_A_Null_TCBInpterpolationData2_List_To_A_BKDEntry_Object() => new BKDEntry()
            .Invoking(e => e.TCBInterpolatorData2 = null)
            .Should()
            .Throw<ArgumentNullException>()
            .Where(e => e.Message.Contains("value"));
    }
}
