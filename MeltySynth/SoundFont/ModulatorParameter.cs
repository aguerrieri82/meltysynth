﻿using System;
using System.Collections.Generic;
using System.IO;

namespace MeltySynth.SoundFont
{
    public sealed class ModulatorParameter
    {
        private ModulatorType sourceModulationData;
        private GeneratorType destinationGenerator;
        private short amount;
        private ModulatorType sourceModulationAmount;
        private TransformType sourceTransform;

        private ModulatorParameter(BinaryReader reader)
        {
            sourceModulationData = new ModulatorType(reader);
            destinationGenerator = (GeneratorType)reader.ReadUInt16();
            amount = reader.ReadInt16();
            sourceModulationAmount = new ModulatorType(reader);
            sourceTransform = (TransformType)reader.ReadUInt16();
        }

        internal static IReadOnlyList<ModulatorParameter> ReadFromChunk(BinaryReader reader, int size)
        {
            if (size % 10 != 0)
            {
                throw new InvalidDataException("The modulator list is invalid.");
            }

            var modulators = new ModulatorParameter[size / 10 - 1];

            for (var i = 0; i < modulators.Length; i++)
            {
                modulators[i] = new ModulatorParameter(reader);
            }

            // The last one is the terminator.
            new ModulatorParameter(reader);

            return modulators;
        }

        public override string ToString()
        {
            return "(" + sourceModulationData.ControllerSource + ", " + destinationGenerator + ", " + amount + ")";
        }

        public ModulatorType SourceModulationData => sourceModulationData;
        public GeneratorType DestinationGenerator => destinationGenerator;
        public short Amount => amount;
        public ModulatorType SourceModulationAmount => sourceModulationAmount;
        public TransformType SourceTransform => sourceTransform;
    }
}
