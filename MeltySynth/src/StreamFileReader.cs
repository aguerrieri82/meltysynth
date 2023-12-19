using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace MeltySynth.src
{
    public class StreamFileReader : IFileReader
    {
        Stream _stream;
        BinaryReader _binaryReader;
        private bool _isDisposed;

        public StreamFileReader(Stream stream)
        {
            _stream = stream;
            _binaryReader = new BinaryReader(_stream);
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;
            _stream.Dispose();
            _binaryReader.Dispose();
            _isDisposed = true;
        }

        public byte ReadByte()
        {
            return _binaryReader.ReadByte();
        }

        public string ReadFixedLengthString(int length)
        {
            return _binaryReader.ReadFixedLengthString(length);
        }

        public string ReadFourCC()
        {
            return _binaryReader.ReadFourCC();
        }

        public short ReadInt16()
        {
            return _binaryReader.ReadInt16();
        }

        public int ReadInt32()
        {
            return _binaryReader.ReadInt32();
        }

        public ISamplesBuffer ReadSamples(long length)
        {
            var buffer = new short[length];
            _binaryReader.Read(MemoryMarshal.Cast<short, byte>(buffer));
            return new ArraySamplesBuffer(buffer);
        }

        public sbyte ReadSByte()
        {
            return _binaryReader.ReadSByte();
        }

        public ushort ReadUInt16()
        {
            return _binaryReader.ReadUInt16();
        }

        public uint ReadUInt32()
        {
            return _binaryReader.ReadUInt32();
        }

        public long Position
        {
            get => _binaryReader.BaseStream.Position;
            set => _binaryReader.BaseStream.Position = value;
        }
    }
}