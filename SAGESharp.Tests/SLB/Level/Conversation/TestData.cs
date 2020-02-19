/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using SAGESharp.SLB;
using SAGESharp.SLB.Level.Conversation;
using System.Collections.Generic;

namespace SAGESharp.Tests.SLB.Level.Conversation
{
    static class TestData
    {
        public static CharacterTable EmptyCharacterTable() => new CharacterTable
        {
            Entries = new List<Character>()
        };

        public static CharacterTable SimpleCharacterTable() => new CharacterTable
        {
            Entries = new List<Character>()
            {
                new Character()
                {
                    ToaName = Identifier.From("TOA1"),
                    CharName = Identifier.From("NAME"),
                    CharCont = Identifier.From("CONT"),
                    Entries = new List<Info>()
                    {
                        new Info()
                        {
                            LineSide = LineSide.Right,
                            ConditionStart = 0x81818181,
                            ConditionEnd = 0x7E7E7E7E,
                            StringLabel = Identifier.From("STRX"),
                            StringIndex = 1122334455,
                            Frames = new List<Frame>()
                            {
                                new Frame()
                                {
                                    ToaAnimation = 1,
                                    CharAnimation = 2,
                                    CameraPositionTarget = 0x55555555,
                                    CameraDistance = 4,
                                    StringIndex = 5,
                                    ConversationSounds = "SOUND"
                                }
                            }
                        }
                    }
                }
            }
        };

        public static CharacterTable ComplexCharacterTable() => new CharacterTable
        {
            Entries = new List<Character>()
            {
                new Character()
                {
                    ToaName = Identifier.From("TOA1"),
                    CharName = Identifier.From("NAM1"),
                    CharCont = Identifier.From("CON1"),
                    Entries = new List<Info>()
                    {
                        new Info()
                        {
                            LineSide = LineSide.Right,
                            ConditionStart = 0x01010101,
                            ConditionEnd = 0x02020202,
                            StringLabel = Identifier.From("LAB1"),
                            StringIndex = 0x14131211,
                            Frames = new List<Frame>()
                            {
                                new Frame()
                                {
                                    ToaAnimation = 0x0101,
                                    CharAnimation = 0x0102,
                                    CameraPositionTarget = 0x0103,
                                    CameraDistance = 0x0104,
                                    StringIndex = 0x0105,
                                    ConversationSounds = "SOUNDS1"
                                }
                            }
                        },
                        new Info()
                        {
                            LineSide = LineSide.Left,
                            ConditionStart = 0x04040404,
                            ConditionEnd = 0x08080808,
                            StringLabel = Identifier.From("LAB2"),
                            StringIndex = 0x24232221,
                            Frames = new List<Frame>()
                            {
                                new Frame()
                                {
                                    ToaAnimation = 0x0201,
                                    CharAnimation = 0x0202,
                                    CameraPositionTarget = 0x0203,
                                    CameraDistance = 0x0204,
                                    StringIndex = 0x0205,
                                    ConversationSounds = "SOUNDS2"
                                },
                                new Frame()
                                {
                                    ToaAnimation = 0x0301,
                                    CharAnimation = 0x0302,
                                    CameraPositionTarget = 0x0303,
                                    CameraDistance = 0x0304,
                                    StringIndex = 0x0305,
                                    ConversationSounds = "SOUNDS3"
                                }
                            }
                        },
                        new Info()
                        {
                            LineSide = LineSide.Right,
                            ConditionStart = 0x10101010,
                            ConditionEnd = 0x20202020,
                            StringLabel = Identifier.From("LAB3"),
                            StringIndex = 0x34333231,
                            Frames = new List<Frame>()
                            {
                                new Frame()
                                {
                                    ToaAnimation = 0x0401,
                                    CharAnimation = 0x0402,
                                    CameraPositionTarget = 0x0403,
                                    CameraDistance = 0x0404,
                                    StringIndex = 0x0405,
                                    ConversationSounds = "SOUNDS4"
                                },
                                new Frame()
                                {
                                    ToaAnimation = 0x0501,
                                    CharAnimation = 0x0502,
                                    CameraPositionTarget = 0x0503,
                                    CameraDistance = 0x0504,
                                    StringIndex = 0x0505,
                                    ConversationSounds = "SOUNDS5"
                                },
                                new Frame()
                                {
                                    ToaAnimation = 0x0601,
                                    CharAnimation = 0x0602,
                                    CameraPositionTarget = 0x0603,
                                    CameraDistance = 0x0604,
                                    StringIndex = 0x0605,
                                    ConversationSounds = "SOUNDS6"
                                }
                            }
                        }
                    }
                },
                new Character()
                {
                    ToaName = Identifier.From("TOA2"),
                    CharName = Identifier.From("NAM2"),
                    CharCont = Identifier.From("CON2"),
                    Entries = new List<Info>()
                    {
                        new Info()
                        {
                            LineSide = LineSide.Left,
                            ConditionStart = 0x40404040,
                            ConditionEnd = 0x80808080,
                            StringLabel = Identifier.From("LAB4"),
                            StringIndex = 0x44434241,
                            Frames = new List<Frame>()
                            {
                                new Frame()
                                {
                                    ToaAnimation = 0x0701,
                                    CharAnimation = 0x0702,
                                    CameraPositionTarget = 0x0703,
                                    CameraDistance = 0x0704,
                                    StringIndex = 0x0705,
                                    ConversationSounds = "SOUNDS7"
                                }
                            }
                        },
                        new Info()
                        {
                            LineSide = LineSide.Right,
                            ConditionStart = 0x11111111,
                            ConditionEnd = 0x22222222,
                            StringLabel = Identifier.From("LAB5"),
                            StringIndex = 0x54535251,
                            Frames = new List<Frame>()
                            {
                                new Frame()
                                {
                                    ToaAnimation = 0x0801,
                                    CharAnimation = 0x0802,
                                    CameraPositionTarget = 0x0803,
                                    CameraDistance = 0x0804,
                                    StringIndex = 0x0805,
                                    ConversationSounds = "SOUNDS8"
                                }
                            }
                        }
                    }
                },
                new Character()
                {
                    ToaName = Identifier.From("TOA3"),
                    CharName = Identifier.From("NAM3"),
                    CharCont = Identifier.From("CON3"),
                    Entries = new List<Info>()
                    {
                        new Info()
                        {
                            LineSide = LineSide.Left,
                            ConditionStart = 0x44444444,
                            ConditionEnd = 0x88888888,
                            StringLabel = Identifier.From("LAB6"),
                            StringIndex = 0x64636261,
                            Frames = new List<Frame>()
                            {
                                new Frame()
                                {
                                    ToaAnimation = 0x0901,
                                    CharAnimation = 0x0902,
                                    CameraPositionTarget = 0x0903,
                                    CameraDistance = 0x0904,
                                    StringIndex = 0x0905,
                                    ConversationSounds = "SOUNDS9"
                                },
                                new Frame()
                                {
                                    ToaAnimation = 0x0A01,
                                    CharAnimation = 0x0A02,
                                    CameraPositionTarget = 0x0A03,
                                    CameraDistance = 0x0A04,
                                    StringIndex = 0x0A05,
                                    ConversationSounds = "SOUNDSA"
                                },
                                new Frame()
                                {
                                    ToaAnimation = 0x0B01,
                                    CharAnimation = 0x0B02,
                                    CameraPositionTarget = 0x0B03,
                                    CameraDistance = 0x0B04,
                                    StringIndex = 0x0B05,
                                    ConversationSounds = "SOUNDSB"
                                }
                            }
                        }
                    }
                }
            }
        };
    }
}
