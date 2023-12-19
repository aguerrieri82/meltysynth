using MeltySynth.src;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;

namespace MeltySynth
{
    /// <summary>
    /// Reperesents a SoundFont.
    /// </summary>
    public sealed class SoundFont : IDisposable
    {
        private SoundFontInfo info;
        private int bitsPerSample;
        private ISamplesBuffer waveData;
        private SampleHeader[] sampleHeaders;
        private Preset[] presets;
        private Instrument[] instruments;
        private MemoryMappedFile? _mmFile;

        /// <summary>
        /// Loads a SoundFont from the file.
        /// </summary>
        /// <param name="path">
        /// The SoundFont file name and path.
        /// </param>
        public SoundFont(string path, bool useMemoryMap = false)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (useMemoryMap)
            {
                _mmFile = MemoryMappedFile.CreateFromFile(path);
                using var reader = new FileMapReader(_mmFile);
                Load(reader);
            }
            else
            {
                using var reader = new StreamFileReader(File.OpenRead(path));
                Load(reader);
            }

            // Workaround for nullable warnings in .NET Standard 2.1.
            Debug.Assert(info != null);
            Debug.Assert(waveData != null);
            Debug.Assert(sampleHeaders != null);
            Debug.Assert(presets != null);
            Debug.Assert(instruments != null);
        }


        private void Load(IFileReader reader)
        {
            var chunkId = reader.ReadFourCC();
            if (chunkId != "RIFF")
            {
                throw new InvalidDataException("The RIFF chunk was not found.");
            }

            var size = reader.ReadUInt32();

            var formType = reader.ReadFourCC();
            if (formType != "sfbk")
            {
                throw new InvalidDataException($"The type of the RIFF chunk must be 'sfbk', but was '{formType}'.");
            }

            info = new SoundFontInfo(reader);

            var sampleData = new SoundFontSampleData(reader);
            bitsPerSample = sampleData.BitsPerSample;
            waveData = sampleData.Samples;

            var parameters = new SoundFontParameters(reader);
            sampleHeaders = parameters.SampleHeaders;
            presets = parameters.Presets;
            instruments = parameters.Instruments;

            CheckSamples();
            CheckRegions();
        }

        /// <summary>
        /// Gets the name of the SoundFont.
        /// </summary>
        /// <returns>
        /// The name of the SoundFont.
        /// </returns>
        public override string ToString()
        {
            return info.BankName;
        }

        private void CheckSamples()
        {
            var sampleCount = waveData.Length - 4; // This offset is to ensure that out of range access is safe.

            foreach (var sample in sampleHeaders)
            {
                if (!(0 <= sample.Start && sample.Start < sampleCount))
                {
                    throw new InvalidDataException($"The start position of the sample '{sample.Name}' is out of range.");
                }
                if (!(0 <= sample.StartLoop && sample.StartLoop < sampleCount))
                {
                    throw new InvalidDataException($"The loop start position of the sample '{sample.Name}' is out of range.");
                }
                if (!(0 < sample.End && sample.End <= sampleCount))
                {
                    throw new InvalidDataException($"The end position of the sample '{sample.Name}' is out of range.");
                }
                if (!(0 <= sample.EndLoop && sample.EndLoop <= sampleCount))
                {
                    throw new InvalidDataException($"The loop end position of the sample '{sample.Name}' is out of range.");
                }
            }
        }

        private void CheckRegions()
        {
            var sampleCount = waveData.Length - 4; // This offset is to ensure that out of range access is safe.

            foreach (var instrument in instruments)
            {
                foreach (var region in instrument.RegionArray)
                {
                    if (!(0 <= region.SampleStart && region.SampleStart < sampleCount))
                    {
                        throw new InvalidDataException($"The start position of the sample '{region.Sample.Name}' in the instrument '{instrument.Name}' is out of range.");
                    }
                    if (!(0 <= region.SampleStartLoop && region.SampleStartLoop < sampleCount))
                    {
                        throw new InvalidDataException($"The loop start position of the sample '{region.Sample.Name}' in the instrument '{instrument.Name}' is out of range.");
                    }
                    if (!(0 < region.SampleEnd && region.SampleEnd <= sampleCount))
                    {
                        throw new InvalidDataException($"The end position of the sample '{region.Sample.Name}' in the instrument '{instrument.Name}' is out of range.");
                    }
                    if (!(0 <= region.SampleEndLoop && region.SampleEndLoop <= sampleCount))
                    {
                        throw new InvalidDataException($"The loop end position of the sample '{region.Sample.Name}' in the instrument '{instrument.Name}' is out of range.");
                    }

                    switch (region.SampleModes)
                    {
                        case LoopMode.NoLoop:
                        case LoopMode.Continuous:
                        case LoopMode.LoopUntilNoteOff:
                            break;
                        default:
                            throw new InvalidDataException($"The sample '{region.Sample.Name}' in the instrument '{instrument.Name}' has an invalid loop mode.");
                    }
                }
            }
        }

        public void Dispose()
        {
            waveData.Dispose();
            _mmFile?.Dispose();
            _mmFile = null;
        }

        /// <summary>
        /// The information of the SoundFont.
        /// </summary>
        public SoundFontInfo Info => info;

        /// <summary>
        /// The bits per sample of the sample data.
        /// </summary>
        /// <remarks>
        /// This value is always 16.
        /// </remarks>
        public int BitsPerSample => bitsPerSample;

        /// <summary>
        /// The sample data.
        /// </summary>
        /// <remarks>
        /// This single array contains all the waveform data in the SoundFont.
        /// An instance of <see cref="SampleHeader"/> indicates a range of the array corresponding to a sample.
        /// </remarks>
        public ISamplesBuffer WaveData => waveData;

        /// <summary>
        /// The samples of the SoundFont.
        /// </summary>
        public IReadOnlyList<SampleHeader> SampleHeaders => sampleHeaders;

        /// <summary>
        /// The presets of the SoundFont.
        /// </summary>
        public IReadOnlyList<Preset> Presets => presets;

        /// <summary>
        /// The instruments of the SoundFont.
        /// </summary>
        public IReadOnlyList<Instrument> Instruments => instruments;

        // Internally exposes the raw arrays for fast enumeration.
        internal ISamplesBuffer WaveDataArray => waveData;
        internal SampleHeader[] SampleHeaderArray => sampleHeaders;
        internal Preset[] PresetArray => presets;
        internal Instrument[] InstrumentArray => instruments;
    }
}
