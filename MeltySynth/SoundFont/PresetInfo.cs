﻿using System;
using System.Collections.Generic;
using System.IO;

namespace MeltySynth.SoundFont
{
    internal sealed class PresetInfo
    {
        private string name;
        private int patchNumber;
        private int bankNumber;
        private int zoneStartIndex;
        private int zoneEndIndex;
        private int library;
        private int genre;
        private int morphology;

        private PresetInfo()
        {
        }

        internal static IReadOnlyList<PresetInfo> ReadFromChunk(BinaryReader reader, int size)
        {
            if (size % 38 != 0)
            {
                throw new InvalidDataException("The preset list is invalid.");
            }

            var count = size / 38;

            var presets = new List<PresetInfo>(count);

            for (var i = 0; i < count; i++)
            {
                var preset = new PresetInfo();
                preset.name = reader.ReadAsciiString(20);
                preset.patchNumber = reader.ReadUInt16();
                preset.bankNumber = reader.ReadUInt16();
                preset.zoneStartIndex = reader.ReadUInt16();
                preset.library = reader.ReadInt32();
                preset.genre = reader.ReadInt32();
                preset.morphology = reader.ReadInt32();

                presets.Add(preset);
            }

            for (var i = 0; i < count - 1; i++)
            {
                presets[i].zoneEndIndex = presets[i + 1].zoneStartIndex - 1;
            }

            // The last one is the terminator.
            presets.RemoveAt(count - 1);

            return presets;
        }

        public string Name => name;
        public int PatchNumber => patchNumber;
        public int BankNumber => bankNumber;
        public int ZoneStartIndex => zoneStartIndex;
        public int ZoneEndIndex => zoneEndIndex;
        public int Library => library;
        public int Genre => genre;
        public int Morphology => morphology;
    }
}
