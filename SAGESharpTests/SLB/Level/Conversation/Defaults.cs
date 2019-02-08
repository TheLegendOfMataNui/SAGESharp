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
            StringLabel = 3,
            StringIndex = 4,
            Frames =  new List<Frame>() { DefaultFrame() }
        };

        public static Character DefaultCharacter() => new Character()
        {
            ToaName = Identifier.From("TOA1"),
            CharName = Identifier.From("CHA1"),
            CharCont = Identifier.From("CON1"),
            Entries = new List<Info>() { DefaultInfo() }
        };
    }
}
