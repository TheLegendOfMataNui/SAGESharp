/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using SAGESharp.SLB;
using SAGESharp.SLB.Character.MoveList;
using System.Collections.Generic;

namespace SAGESharp.Tests.SLB.Character.MoveList
{
    static class TestData
    {
        public static MoveListTable EmptyMoveListTable() => new MoveListTable
        {
            Id = Identifier.From("ID01"),
            Animations = new List<Animation>(),
            AnimationsWithExtra = new List<AnimationWithExtra>()
        };

        public static MoveListTable SimpleMoveListTable() => new MoveListTable
        {
            Id = Identifier.From("ID02"),
            Animations = new List<Animation>
            {
                new Animation
                {
                    Id1 = Identifier.From("ANI1"),
                    Id2 = Identifier.From("ANI2"),
                    Flags1 = 0x1AAA,
                    Index = 0x1BBB,
                    Int1 = 0x1CCCCCCC,
                    Float1 = 1.5f,
                    Float2 = 2.5f,
                    Flags2 = 0x1DDDDDDD,
                    ReservedCounter = 0x1EEE,
                    Triggers = new List<SplitTrigger>
                    {
                        new SplitTrigger
                        {
                            Input = 0x2AAAAAAA,
                            Id = Identifier.From("TRI1"),
                            Float1 = 3.5f,
                            Float2 = 4.5f,
                            Float3 = 5.5f,
                            Flags = 0x2C
                        }
                    }
                }
            },
            AnimationsWithExtra = new List<AnimationWithExtra>
            {
                new AnimationWithExtra
                {
                    Animation = new Animation
                    {
                        Id1 = Identifier.From("ANI3"),
                        Id2 = Identifier.From("ANI4"),
                        Flags1 = 0x2AAA,
                        Index = 0x2BBB,
                        Int1 = 0x2CCCCCCC,
                        Float1 = 6.5f,
                        Float2 = 7.5f,
                        Flags2 = 0x2DDDDDDD,
                        ReservedCounter = 0x2EEE,
                        Triggers = new List<SplitTrigger>
                        {
                            new SplitTrigger
                            {
                                Input = 0x3AAAAAAA,
                                Id = Identifier.From("TRI2"),
                                Float1 = 8.5f,
                                Float2 = 9.5f,
                                Float3 = 10.5f,
                                Flags = 0x3C
                            }
                        }
                    },
                    Extra = 0x3FFFFFFF
                }
            }
        };

        public static MoveListTable ComplexMoveListTable() => new MoveListTable
        {
            Id = Identifier.From("ID03"),
            Animations = new List<Animation>
            {
                new Animation
                {
                    Id1 = Identifier.From("ANI5"),
                    Id2 = Identifier.From("ANI6"),
                    Flags1 = 0x3AAA,
                    Index = 0x3BBB,
                    Int1 = 0x3CCCCCCC,
                    Float1 = 11.5f,
                    Float2 = 12.5f,
                    Flags2 = 0x3DDDDDDD,
                    ReservedCounter = 0x3EEE,
                    Triggers = new List<SplitTrigger>
                    {
                        new SplitTrigger
                        {
                            Input = 0x4AAAAAAA,
                            Id = Identifier.From("TRI3"),
                            Float1 = 13.5f,
                            Float2 = 14.5f,
                            Float3 = 15.5f,
                            Flags = 0x4C
                        },
                        new SplitTrigger
                        {
                            Input = 0x5AAAAAAA,
                            Id = Identifier.From("TRI4"),
                            Float1 = 16.5f,
                            Float2 = 17.5f,
                            Float3 = 18.5f,
                            Flags = 0x5C
                        },
                        new SplitTrigger
                        {
                            Input = 0x6AAAAAAA,
                            Id = Identifier.From("TRI5"),
                            Float1 = 19.5f,
                            Float2 = 20.5f,
                            Float3 = 21.5f,
                            Flags = 0x6C
                        }
                    }
                }
            },
            AnimationsWithExtra = new List<AnimationWithExtra>
            {
                new AnimationWithExtra
                {
                    Animation = new Animation
                    {
                        Id1 = Identifier.From("ANI7"),
                        Id2 = Identifier.From("ANI8"),
                        Flags1 = 0x7AAA,
                        Index = 0x7BBB,
                        Int1 = 0x7CCCCCCC,
                        Float1 = 22.5f,
                        Float2 = 23.5f,
                        Flags2 = 0x7DDDDDDD,
                        ReservedCounter = 0x7EEE,
                        Triggers = new List<SplitTrigger>()
                    },
                    Extra = 0x7FFFFFFF
                },
                new AnimationWithExtra
                {
                    Animation = new Animation
                    {
                        Id1 = Identifier.From("ANI9"),
                        Id2 = Identifier.From("ANI0"),
                        Flags1 = 0x7000,
                        Index = 0x7001,
                        Int1 = 0x70000002,
                        Float1 = 24.5f,
                        Float2 = 25.5f,
                        Flags2 = 0x70000003,
                        ReservedCounter = 0x7004,
                        Triggers = new List<SplitTrigger>
                        {
                            new SplitTrigger
                            {
                                Input = 0x6AAAAAAA,
                                Id = Identifier.From("TRI6"),
                                Float1 = 26.5f,
                                Float2 = 27.5f,
                                Float3 = 28.5f,
                                Flags = 0x6C
                            },
                            new SplitTrigger
                            {
                                Input = 0x7AAAAAAA,
                                Id = Identifier.From("TRI7"),
                                Float1 = 29.5f,
                                Float2 = 30.5f,
                                Float3 = 31.5f,
                                Flags = 0x7C
                            }
                        }
                    },
                    Extra = 0x7EEEEEEE
                }
            }
        };
    }
}
