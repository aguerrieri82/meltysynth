using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MeltySynth
{
    public class ArraySamplesBuffer : ISamplesBuffer
    {
        short[] _buffer;
        GCHandle _handle;   

        public ArraySamplesBuffer(short[] buffer)
        {
            _buffer = buffer;
        }

        public unsafe short* Lock(long index)
        {
           var baseAddr = _handle.AddrOfPinnedObject();
            return ((short*)baseAddr) + index;
        }

        public long Length => _buffer.Length;

        public void Dispose()
        {
            _handle.Free();
        }

        public byte[] GetBytes(long pos, int length)
        {
            var bytes = MemoryMarshal.Cast<short, byte>(new Span<short>(_buffer, (int)pos, length));
            return bytes.ToArray();
        }

    }
}
