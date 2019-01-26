using SAGESharp.SLB.Level.Conversation;

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
    }
}
