using System;
using System.Collections.Generic;
using System.Text;

namespace MeltySynth
{
    public interface IFileReader : IDisposable
    {
        ISamplesBuffer ReadSamples(long length);

        string ReadFixedLengthString(int length);

        sbyte ReadSByte();

        byte ReadByte();


        ushort ReadUInt16();

        short ReadInt16();

        int ReadInt32();

        uint ReadUInt32();

        string ReadFourCC();

        long Position { get; set; } 
    }
}
