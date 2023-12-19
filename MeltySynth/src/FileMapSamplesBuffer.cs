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

        public short this[long index] => _dataPointer[index];

        public long Length => _length;

        public void Dispose()
        {
            if (_isDisposed)
                return;
            if (_dataPointer != null)
                _view.SafeMemoryMappedViewHandle.ReleasePointer();
            _view.Dispose();
            _isDisposed = true;
        }

        public byte[] GetBytes(long pos, int length)
        {
            var res = new byte[length];
            var curPtr = ((byte*)_dataPointer) + pos;
            for (var i = 0; i < length; i++)
            {
                res[i] = *curPtr;
                curPtr++;
            }
            return res;
        }
    }
}
