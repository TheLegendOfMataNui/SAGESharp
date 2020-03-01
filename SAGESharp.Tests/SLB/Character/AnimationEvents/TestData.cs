/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using SAGESharp.SLB;
using SAGESharp.SLB.Character.AnimationEvents;
using System.Collections.Generic;

namespace SAGESharp.Tests.SLB.Character.AnimationEvents
{
    class TestData
    {
        public static AnimationEventsTable EmptyAnimationEventsTable() => new AnimationEventsTable
        {
            Entries = new List<AnimationEvent>()
        };

        public static AnimationEventsTable SimpleAnimationEventsTable() => new AnimationEventsTable
        {
            Entries = new List<AnimationEvent>
            {
                new AnimationEvent
                {
                    Id = Identifier.From("Id01"),
                    EventArg2 = Identifier.From("Id02"),
                    EventArg3 = Identifier.From("Id03"),
                    EventArg4 = Identifier.From("Id04"),
                    EventArg5 = 5.5,
                    EventArg6 = 0x30363036,
                    EventArg7 = 0x30373037,
                    EventArg8 = 0x30383038,
                    EventArg9 = 0x30393039,
                    EventArg10 = 0x31303130,
                    Unknown = 0x554E4B4E
                }
            }
        };

        public static AnimationEventsTable ComplexAnimationEventsTable() => new AnimationEventsTable
        {
            Entries = new List<AnimationEvent>
            {
                new AnimationEvent
                {
                    Id = Identifier.From("TOA1"),
                    EventArg2 = Identifier.From("TOA2"),
                    EventArg3 = Identifier.From("TOA3"),
                    EventArg4 = Identifier.From("TOA4"),
                    EventArg5 = 5.1,
                    EventArg6 = 0x010000AA,
                    EventArg7 = 0x010000BB,
                    EventArg8 = 0x010000CC,
                    EventArg9 = 0x010000DD,
                    EventArg10 = 0x010000EE,
                    Unknown = 0x0A1A2A3A
                },
                new AnimationEvent
                {
                    Id = Identifier.From("TUR1"),
                    EventArg2 = Identifier.From("TUR2"),
                    EventArg3 = Identifier.From("TUR3"),
                    EventArg4 = Identifier.From("TUR4"),
                    EventArg5 = 5.2,
                    EventArg6 = 0x020000AA,
                    EventArg7 = 0x020000BB,
                    EventArg8 = 0x020000CC,
                    EventArg9 = 0x020000DD,
                    EventArg10 = 0x020000EE,
                    Unknown = 0x0B1B2B3B
                },
                new AnimationEvent
                {
                    Id = Identifier.From("VLG1"),
                    EventArg2 = Identifier.From("VLG2"),
                    EventArg3 = Identifier.From("VLG3"),
                    EventArg4 = Identifier.From("VLG4"),
                    EventArg5 = 5.3,
                    EventArg6 = 0x030000AA,
                    EventArg7 = 0x030000BB,
                    EventArg8 = 0x030000CC,
                    EventArg9 = 0x030000DD,
                    EventArg10 = 0x030000EE,
                    Unknown = 0x0C1C2C3C
                }
            }
        };
    }
}
