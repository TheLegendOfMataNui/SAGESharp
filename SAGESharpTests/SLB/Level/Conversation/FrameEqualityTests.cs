using NUnit.Framework;
using SAGESharp.SLB.Level.Conversation;
using SAGESharpTests.Util;
using System;

using static SAGESharpTests.SLB.Level.Conversation.Defaults;

namespace SAGESharpTests.SLB.Level.Conversation
{
    class FrameEqualityTests : AbstractEqualityTests<Frame>
    {
        protected override Frame GetDefault() => DefaultFrame();

        protected override bool EqualsOperator(Frame left, Frame right) => left == right;

        protected override bool NotEqualsOperator(Frame left, Frame right) => left != right;

        [TestCaseSource(nameof(FrameModifiers))]
        public void Test_Compare_Default_Object_With_Modified_Object(Action<Frame> frameModifier) =>
            TestCompareDefaultObjectWithModifiedObject(frameModifier);

        static object[] FrameModifiers() => new ParameterGroup()
            .Parameters((Action<Frame>)(frame => frame.ToaAnimation = 0))
            .Parameters((Action<Frame>)(frame => frame.CharAnimation = 0))
            .Parameters((Action<Frame>)(frame => frame.CameraPositionTarget = 0))
            .Parameters((Action<Frame>)(frame => frame.CameraDistance = 0))
            .Parameters((Action<Frame>)(frame => frame.StringIndex = 0))
            .Parameters((Action<Frame>)(frame => frame.ConversationSounds = null))
            .Build();

        [TestCaseSource(nameof(DualFrameModifiers))]
        public void Test_Compare_Modified_Objects(Action<Frame> frameModifierA, Action<Frame> frameModifierB) =>
            TestCompareModifiedObjects(frameModifierA, frameModifierB);

        static object[] DualFrameModifiers() => new ParameterGroup()
            .Parameters(
                (Action<Frame>)(frame => frame.ConversationSounds = null),
                (Action<Frame>)(frame => frame.ConversationSounds = "")
            )
            .Build();
    }
}
