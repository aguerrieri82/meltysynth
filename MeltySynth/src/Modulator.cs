using System;
using System.IO;

namespace MeltySynth
{
    internal static class Modulator
    {
        // Since modulators will not be IFileReader, we discard the data.
        internal static void DiscardData(IFileReader reader, int size)
        {
            if (size % 10 != 0)
            {
                throw new InvalidDataException("The modulator list is invalid.");
            }

            reader.Position += size;
        }
    }
}
