using SAGESharp.SLB;
using SAGESharp.SLB.Level.Conversation;
using System.Collections.Generic;

namespace SAGESharpTests.SLB.Level.Conversation
{
    static class Defaults
    {
        public static Frame DefaultFrame() => new Frame()
        {
            ToaAnimation = 1,
            CharAnimation = 2,
            CameraPositionTarget = 3,
            CameraDistance = 4,
            StringIndex = 5,
            ConversationSounds = "sounds"
        };

        public static Info DefaultInfo() => new Info()
        {
            LineSide = LineSide.Right,
            ConditionStart = 1,
            ConditionEnd = 2,
            StringLabel = new Identifier(3),
            StringIndex = 4,
            Frames =  new List<Frame>() { DefaultFrame() }
        };

        public static Character DefaultCharacter() => new Character()
        {
            ToaName = new Identifier("TOA1"),
            CharName = new Identifier("CHA1"),
            CharCont = new Identifier("CON1"),
            Entries = new List<Info>() { DefaultInfo() }
        };
    }
}
