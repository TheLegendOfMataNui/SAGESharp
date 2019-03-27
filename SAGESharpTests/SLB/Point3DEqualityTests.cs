using NUnit.Framework;
using SAGESharp.Testing;
using System;

namespace SAGESharp.SLB
{
    class Point3DEqualityTests : AbstractEqualityByRefTests<Point3D>
    {
        protected override Point3D GetDefault() => new Point3D { X = 1, Y = 2, Z = 3 };

        protected override bool EqualsOperator(Point3D left, Point3D right) => left == right;

        protected override bool NotEqualsOperator(Point3D left, Point3D right) => left != right;

        [TestCaseSource(nameof(Modifiers))]
        public void Test_Compare_Default_Object_With_Modified_Object(Action<Point3D> modifier) =>
            TestCompareDefaultObjectWithModifiedObject(modifier);

        static object[] Modifiers() => new ParameterGroup<Action<Point3D>>()
            .Parameters(point3D => point3D.X = 0)
            .Parameters(point3D => point3D.Y = 0)
            .Parameters(point3D => point3D.Z = 0)
            .Build();
    }
}
