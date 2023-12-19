using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MeltySynth
{
    public class ArraySamplesBuffer : ISamplesBuffer
    {
        short[] _buffer;

        public ArraySamplesBuffer(short[] buffer)
        {
            _buffer = buffer;
        }

        public short this[long index] => _buffer[index];

        public long Length => _buffer.Length;

        public void Dispose()
        {
        }

        public byte[] GetBytes(long pos, int length)
        {
            var bytes = MemoryMarshal.Cast<short, byte>(new Span<short>(_buffer, (int)pos, length));
            return bytes.ToArray();
        }
    }
}
