using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Text;

namespace MeltySynth
{
    public unsafe class FileMapSamplesBuffer : ISamplesBuffer
    {
        long _length;
        short* _dataPointer;
        MemoryMappedViewAccessor _view;
        bool _isDisposed;

        public FileMapSamplesBuffer(MemoryMappedFile file, long position, long length)
        {
            _length = length;
            _view = file.CreateViewAccessor();

            byte* ptr = null;
            _view.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);
            _dataPointer = (short*)(ptr + position);
        }

        public short* Lock(long index)
        {
            return _dataPointer + index;
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;
            if (_dataPointer != null)
                _view.SafeMemoryMappedViewHandle.ReleasePointer();
            _view.Dispose();
            _isDisposed = true;
        }

        public unsafe byte[] GetBytes(long pos, int length)
        {
            var res = new byte[length];

            fixed (byte* pRes = res)
                Buffer.MemoryCopy(_dataPointer, pRes, length, length);

            return res;
        }

        public long Length => _length;
    }
}
