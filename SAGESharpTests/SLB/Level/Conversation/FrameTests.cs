using NUnit.Framework;
using SAGESharp.SLB.Level.Conversation;
using SAGESharpTests.Util;
using System;

using static SAGESharpTests.SLB.Level.Conversation.Defaults;

namespace SAGESharpTests.SLB.Level.Conversation
{
    class FrameTests
    {
        [Test]
        public void Test_Compare_Frame_With_Itself()
        {
            var frame = DefaultFrame();

            AssertFramesHaveSameHashCode(frame, frame);
            AssertFramesAreEqual(frame, frame);
        }

        [Test]
        public void Test_Compare_Equal_Frames()
        {
            var a = DefaultFrame();
            var b = DefaultFrame();

            AssertFramesHaveSameHashCode(a, b);
            AssertFramesAreEqual(a, b);

            AssertFramesHaveSameHashCode(b, a);
            AssertFramesAreEqual(b, a);
        }

        private static void AssertFramesHaveSameHashCode(Frame a, Frame b)
        {
            Assert.That(a.GetHashCode() == b.GetHashCode(), Is.True);
        }

        private static void AssertFramesAreEqual(Frame a, Frame b)
        {
            Assert.That(a.Equals(b), Is.True);
            Assert.That(a.Equals((object)b), Is.True);
            Assert.That(a == b, Is.True);
            Assert.That(a != b, Is.False);
        }

        [TestCaseSource(nameof(FrameModifiers))]
        public void Test_Compare_Different_Frames(Action<Frame> frameModifier)
        {
            var a = DefaultFrame();
            var b = DefaultFrame();

            frameModifier(b);

            AssertFramesHaveDifferentHashCode(a, b);
            AssertFramesAreDifferent(a, b);

            AssertFramesHaveDifferentHashCode(b, a);
            AssertFramesAreDifferent(b, a);
        }

        static object[] FrameModifiers() => new ParameterGroup()
            .Parameters((Action<Frame>)(frame => frame.ToaAnimation = 0))
            .Parameters((Action<Frame>)(frame => frame.CharAnimation = 0))
            .Parameters((Action<Frame>)(frame => frame.CameraPositionTarget = 0))
            .Parameters((Action<Frame>)(frame => frame.CameraDistance = 0))
            .Parameters((Action<Frame>)(frame => frame.StringIndex = 0))
            .Parameters((Action<Frame>)(frame => frame.ConversationSounds = null))
            .Build();

        [TestCaseSource(nameof(DualFrameModifiers))]
        public void Test_Compare_Different_Frames(Action<Frame> frameModifierA, Action<Frame> frameModifierB)
        {
            var a = DefaultFrame();
            var b = DefaultFrame();

            frameModifierA(a);
            frameModifierB(b);

            AssertFramesHaveDifferentHashCode(a, b);
            AssertFramesAreDifferent(a, b);

            AssertFramesHaveDifferentHashCode(b, a);
            AssertFramesAreDifferent(b, a);
        }

        static object[] DualFrameModifiers() => new ParameterGroup()
            .Parameters(
                (Action<Frame>)(frame => frame.ConversationSounds = null),
                (Action<Frame>)(frame => frame.ConversationSounds = "")
            )
            .Build();

        [Test]
        public void Test_Compare_Frame_With_Null()
        {
            AssertFramesAreDifferent(DefaultFrame(), null);
        }

        private static void AssertFramesHaveDifferentHashCode(Frame a, Frame b)
        {
            Assert.That(a.GetHashCode() != b.GetHashCode(), Is.True);
        }

        private static void AssertFramesAreDifferent(Frame a, Frame b)
        {
            Assert.That(a.Equals(b), Is.False);
            Assert.That(a.Equals((object)b), Is.False);
            Assert.That(a == b, Is.False);
            Assert.That(a != b, Is.True);
        }
    }
}
