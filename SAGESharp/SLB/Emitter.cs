/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using SAGESharp.IO;
using System.Collections.Generic;

namespace SAGESharp.SLB
{
    public sealed class EmitterVector
    {
        [DuplicateEntryCount]
        public IList<Emitter> Entries { get; set; }
    }

    public sealed class Emitter
    {
        [SerializableProperty(1)]
        public int Id { get; set; }

        [SerializableProperty(2)]
        public int Id2 { get; set; }

        [SerializableProperty(3)]
        public int EmitterDataSize { get; set; }

        [SerializableProperty(4)]
        public int ParticleSize { get; set; }

        [SerializableProperty(5)]
        public int MaxParticles { get; set; }

        [SerializableProperty(6)]
        [DuplicateEntryCount]
        public IList<EmitterFunction> Group1 { get; set; }

        [SerializableProperty(7)]
        [DuplicateEntryCount]
        public IList<EmitterFunction> Group2 { get; set; }

        [SerializableProperty(8)]
        [DuplicateEntryCount]
        public IList<EmitterFunction> Group3 { get; set; }

        [SerializableProperty(9)]
        [DuplicateEntryCount]
        public IList<EmitterFunction> Group4 { get; set; }

        [SerializableProperty(10)]
        [DuplicateEntryCount]
        public IList<EmitterFunction> Group5 { get; set; }

        [SerializableProperty(11)]
        [DuplicateEntryCount]
        public IList<EmitterFunction> Group6 { get; set; }
    }

    public sealed class EmitterFunction
    {
        [SerializableProperty(1)]
        public int EmitterFunctionIndex { get; set; }

        [SerializableProperty(2)]
        [DuplicateEntryCount]
        public IList<EmitterFunctionParameter> Parameters { get; set; }
    }

    public sealed class EmitterFunctionParameter
    {
        [SerializableProperty(1)]
        public EmitterFunctionParameterType Type { get; set; }

        [SerializableProperty(2)]
        public float Data { get; set; }
    }

    public enum EmitterFunctionParameterType : int
    {
        EmitterData = 0,
        ParticleData = 1,
        Float = 2,
        Int = 3
    }
}
