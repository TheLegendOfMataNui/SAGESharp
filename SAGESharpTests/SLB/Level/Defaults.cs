/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using System.Collections.Generic;

namespace SAGESharp.SLB.Level
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

        public static ConversationCharacter DefaultConversationCharacter() => new ConversationCharacter()
        {
            ToaName = Identifier.From("TOA1"),
            CharName = Identifier.From("CHA1"),
            CharCont = Identifier.From("CON1"),
            Entries = new List<Info>() { DefaultInfo() }
        };
    }
}
