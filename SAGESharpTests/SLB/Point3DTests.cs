/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUnit.Framework;
using SAGESharp.Testing;

namespace SAGESharp.SLB
{
    class Point3DTests
    {
        [TestCaseSource(nameof(EqualObjectsTestCases))]
        public void Test_Comparing_Equal_Objects(IComparisionTestCase<Point3D> testCase) => testCase.Execute();

        public static IComparisionTestCase<Point3D>[] EqualObjectsTestCases() => new IComparisionTestCase<Point3D>[]
        {
            ComparisionTestCase.CompareObjectAgainstItself(SamplePoint3D()),
            ComparisionTestCase.CompareTwoEqualObjects(SamplePoint3D),
            ComparisionTestCase.CompareNullWithOperators<Point3D>()
        };

        [TestCaseSource(nameof(NotEqualObjectsTestCases))]
        public void Test_Comparing_NotEqual_Objects(IComparisionTestCase<Point3D> testCase) => testCase.Execute();

        public static IComparisionTestCase<Point3D>[] NotEqualObjectsTestCases() => new IComparisionTestCase<Point3D>[]
        {
            ComparisionTestCase.CompareTwoNotEqualObjects(supplier: SamplePoint3D, updater: point3D => point3D.X = 0),
            ComparisionTestCase.CompareTwoNotEqualObjects(supplier: SamplePoint3D, updater: point3D => point3D.Y = 0),
            ComparisionTestCase.CompareTwoNotEqualObjects(supplier: SamplePoint3D, updater: point3D => point3D.Z = 0),
            ComparisionTestCase.CompareNotNullObjectAgainstNull(SamplePoint3D())
        };

        public static Point3D SamplePoint3D() => new Point3D {
            X = 1,
            Y = 2,
            Z = 3
        };
    }
}
