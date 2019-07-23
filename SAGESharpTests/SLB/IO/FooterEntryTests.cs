/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using FluentAssertions;
using NUnit.Framework;
using SAGESharp.Testing;

namespace SAGESharp.SLB.IO
{
    class FooterEntryTests : AbstractEqualityByValTests<FooterEntry>
    {
        protected override bool EqualsOperator(FooterEntry left, FooterEntry right)
            => left == right;

        protected override bool NotEqualsOperator(FooterEntry left, FooterEntry right)
            => left != right;

        [TestCaseSource(nameof(FooterEntriesWithStringValues))]
        public void Test_Converting_Footer_Entry_To_String(FooterEntry footerEntry, string expected)
            => footerEntry.ToString().Should().Be(expected);

        static object[] FooterEntriesWithStringValues() => new ParameterGroup<FooterEntry, string>()
            .Parameters(new FooterEntry { }, "OffsetPosition=0x00, Offset=0x00")
            .Parameters(new FooterEntry { OffsetPosition = 255, Offset = 12 }, "OffsetPosition=0xFF, Offset=0x0C")
            .Build();

        [TestCaseSource(nameof(ModifiedFooterEntries))]
        public void Test_Compare_Default_Object_With_Modified_Object(FooterEntry footerEntry)
            => TestCompareNonEqualsDefaultValueAndNonDefaultValue(footerEntry);

        static object[] ModifiedFooterEntries() => new ParameterGroup<FooterEntry>()
            .Parameters(new FooterEntry { OffsetPosition = 1 })
            .Parameters(new FooterEntry { Offset = 1 })
            .Build();
    }
}
