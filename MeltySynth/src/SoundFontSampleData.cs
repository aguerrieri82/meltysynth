using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Text;

namespace MeltySynth
{
    internal sealed class SoundFontSampleData
    {
        private readonly int bitsPerSample;
        private readonly ISamplesBuffer samples;

        internal SoundFontSampleData(IFileReader reader)
        {
            var chunkId = reader.ReadFourCC();
            if (chunkId != "LIST")
            {
                throw new InvalidDataException("The LIST chunk was not found.");
            }

            var end = (long)reader.ReadUInt32();
            end += reader.Position;

            var listType = reader.ReadFourCC();
            if (listType != "sdta")
            {
                throw new InvalidDataException($"The type of the LIST chunk must be 'sdta', but was '{listType}'.");
            }

            while (reader.Position < end)
            {
                var id = reader.ReadFourCC();
                var size = reader.ReadUInt32();

                switch (id)
                {
                    case "smpl":
                        bitsPerSample = 16;
                        samples = reader.ReadSamples(size / 2);
                        break;
                    case "sm24":
                        // 24 bit audio is not supported.
                        reader.Position += size;
                        break;
                    default:
                        throw new InvalidDataException($"The INFO list contains an unknown ID '{id}'.");
                }
            }

            if (samples == null)
            {
                throw new InvalidDataException("No valid sample data was found.");
            }

            if (Encoding.ASCII.GetString(samples.GetBytes(0, 4)) == "OggS")
            {
                throw new NotSupportedException("SoundFont3 is not yet supported.");
            }

            if (!BitConverter.IsLittleEndian)
            {
                // TODO: Insert the byte swapping code here.
                throw new NotSupportedException("Big endian architectures are not yet supported.");
            }
        }

        public int BitsPerSample => bitsPerSample;
        public ISamplesBuffer Samples => samples;
    }
}
