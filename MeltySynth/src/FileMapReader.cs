using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Text;

namespace MeltySynth
{
    internal class FileMapReader : IFileReader
    {
        MemoryMappedFile _file;
        MemoryMappedViewAccessor _view;
        bool _isDisposed;
        long _curPos;

        public FileMapReader(MemoryMappedFile file)
        {
            _file = file;
            _view = _file.CreateViewAccessor();
            _curPos = 0;
        }

        public long Position 
        {
            get => _curPos;
            set => _curPos = value;
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;
            _isDisposed = true;
            _view.Dispose();
        }

        public byte ReadByte()
        {
            var result = _view.ReadByte(_curPos);
            _curPos++;
            return result;
        }

        public string ReadFixedLengthString(int length)
        {
            var buffer = new byte[length];
            _view.ReadArray(_curPos, buffer, 0, buffer.Length);
            _curPos += length;  
            return Encoding.ASCII.GetString(buffer);

        }

        public string ReadFourCC()
        {
            var data = new byte[4];

            _view.ReadArray(_curPos, data, 0, 4);

            _curPos += 4;

            for (var i = 0; i < data.Length; i++)
            {
                var value = data[i];
                if (!(32 <= value && value <= 126))
                {
                    data[i] = (byte)'?';
                }
            }

            return Encoding.ASCII.GetString(data);
        }

        public short ReadInt16()
        {
            var result = _view.ReadInt16(_curPos);
            _curPos += 2;
            return result;
        }

        public int ReadInt32()
        {
            var result = _view.ReadInt32(_curPos);
            _curPos += 4;
            return result;
        }

        public ISamplesBuffer ReadSamples(long length)
        {
            var result = new FileMapSamplesBuffer(_file, _curPos, length);
            _curPos += length * 2;
            return result;
        }

        public sbyte ReadSByte()
        {
            var result = _view.ReadSByte(_curPos);
            _curPos++;
            return result;
        }

        public ushort ReadUInt16()
        {
            var result = _view.ReadUInt16(_curPos);
            _curPos += 2;
            return result;
        }

        public uint ReadUInt32()
        {
            var result = _view.ReadUInt32(_curPos);
            _curPos += 4;
            return result;
        }
    }
}
