/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUnit.Framework;
using SAGESharp.SLB;
using SAGESharp.SLB.Level.Conversation;
using System.Collections.Generic;

namespace SAGESharp.Tests.SLB.Level.Conversation
{
    class CharacterTests
    {
        [TestCaseSource(nameof(EqualObjectsTestCases))]
        public void Test_Comparing_Equal_Objects(IComparisionTestCase<Character> testCase) => testCase.Execute();

        public static IComparisionTestCase<Character>[] EqualObjectsTestCases() => new IComparisionTestCase<Character>[]
        {
            ComparisionTestCase.CompareObjectAgainstItself(SampleCharacter()),
            ComparisionTestCase.CompareTwoEqualObjects(SampleCharacter),
            ComparisionTestCase.CompareNullWithOperators<Character>()
        };

        [TestCaseSource(nameof(NotEqualObjectsTestCases))]
        public void Test_Comparing_NotEqual_Objects(IComparisionTestCase<Character> testCase) => testCase.Execute();

        public static IComparisionTestCase<Character>[] NotEqualObjectsTestCases() => new IComparisionTestCase<Character>[]
        {
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleCharacter,
                updater: character => character.ToaName = Identifier.From("TOAX")
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleCharacter,
                updater: character => character.CharName = Identifier.From("CHAX")
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleCharacter,
                updater: character => character.CharCont = Identifier.From("CONX")
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleCharacter,
                updater: character => character.Entries = null
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleCharacter,
                updater: character => character.Entries.Clear()
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleCharacter,
                updater: character => character.Entries[0].ConditionStart += 20
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleCharacter,
                updater: character => character.Entries.Add(null)
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleCharacter,
                updater: character => character.Entries.Add(InfoTests.SampleInfo())
            ),
            ComparisionTestCase.CompareNotNullObjectAgainstNull(SampleCharacter())
        };

        public static Character SampleCharacter() => new Character
        {
            ToaName = Identifier.From("TOA1"),
            CharName = Identifier.From("CHA1"),
            CharCont = Identifier.From("CON1"),
            Entries = new List<Info>() { InfoTests.SampleInfo() }
        };
    }
}
