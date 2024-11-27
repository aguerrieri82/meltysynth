﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using MeltySynth;

namespace MeltySynthTest
{
    public class PresetTest_Polyphone
    {
        [TestCaseSource(typeof(TestSettings), nameof(TestSettings.SoundFonts))]
        public void ParameterCheck(string soundFontName, SoundFont soundFont)
        {
            var referenceDataDirectory = Path.Combine("ReferenceData", "Polyphone", soundFontName, "Presets");

            if (Directory.Exists(referenceDataDirectory))
            {
                Run(soundFont, referenceDataDirectory);
            }
            else
            {
                Assert.Ignore("The reference data is missing.");
            }
        }

        private void Run(SoundFont soundFont, string referenceDataDirectory)
        {
            var executed = 0;
            var ignored = 0;

            foreach (var preset in soundFont.Presets)
            {
                var name = preset.BankNumber.ToString("000") + " " + preset.PatchNumber.ToString("000") + " " + preset.Name.Replace('/', ' ').Replace(':', ' ');
                var referenceTsvPath = Path.Combine(referenceDataDirectory, name + ".tsv");

                if (File.Exists(referenceTsvPath))
                {
                    RunSinglePreset(referenceTsvPath, preset);
                    executed++;
                }
                else
                {
                    ignored++;
                }
            }

            Console.WriteLine("Executed: " + executed);
            Console.WriteLine("Ignored: " + ignored);

            if (ignored > 0)
            {
                Assert.Ignore();
            }
        }

        private void RunSinglePreset(string referenceTsvPath, Preset preset)
        {
            var polyphoneRegions = PolyphoneRegion.Read(referenceTsvPath);
            var meltyRegions = preset.Regions.ToArray();

            Assert.That(polyphoneRegions.Length, Is.EqualTo(meltyRegions.Length));

            foreach (var polyphoneRegion in polyphoneRegions)
            {
                var meltyRegion = meltyRegions.MinBy(x => GetError(polyphoneRegion, x));
                AreEqual(polyphoneRegion, meltyRegion);
            }
        }

        private static double GetError(PolyphoneRegion polyphoneRegion, PresetRegion meltyRegion)
        {
            if (polyphoneRegion.KeyRange != (meltyRegion.KeyRangeStart + "-" + meltyRegion.KeyRangeEnd))
            {
                return 1000000;
            }

            if (polyphoneRegion.VelocityRange != (meltyRegion.VelocityRangeStart + "-" + meltyRegion.VelocityRangeEnd))
            {
                return 1000000;
            }

            var error = 0.0;
            error += Math.Abs(polyphoneRegion.Attenuation - 0.4 * meltyRegion.InitialAttenuation);
            error += Math.Abs(polyphoneRegion.Pan - meltyRegion.Pan);
            error += Math.Abs(polyphoneRegion.TuningSemiTones - meltyRegion.CoarseTune);
            error += Math.Abs(polyphoneRegion.TuningCents - meltyRegion.FineTune);
            error += Math.Abs(polyphoneRegion.ScaleTuning - meltyRegion.ScaleTuning);
            error += Math.Abs(polyphoneRegion.FilterCutoffX - meltyRegion.InitialFilterCutoffFrequency);
            error += Math.Abs(polyphoneRegion.FilterResonanceX - meltyRegion.InitialFilterQ);
            error += Math.Abs(polyphoneRegion.VolEnvDelayX - meltyRegion.DelayVolumeEnvelope);
            error += Math.Abs(polyphoneRegion.VolEnvAttackX - meltyRegion.AttackVolumeEnvelope);
            error += Math.Abs(polyphoneRegion.VolEnvHoldX - meltyRegion.HoldVolumeEnvelope);
            error += Math.Abs(polyphoneRegion.VolEnvDecayX - meltyRegion.DecayVolumeEnvelope);
            error += Math.Abs(polyphoneRegion.VolEnvSustainDb - meltyRegion.SustainVolumeEnvelope);
            error += Math.Abs(polyphoneRegion.VolEnvReleaseX - meltyRegion.ReleaseVolumeEnvelope);
            error += Math.Abs(polyphoneRegion.KeyToVolEnvHoldC - meltyRegion.KeyNumberToVolumeEnvelopeHold);
            error += Math.Abs(polyphoneRegion.KeyToVolEnvDecayC - meltyRegion.KeyNumberToVolumeEnvelopeDecay);
            error += Math.Abs(polyphoneRegion.ModEnvDelayX - meltyRegion.DelayModulationEnvelope);
            error += Math.Abs(polyphoneRegion.ModEnvAttackX - meltyRegion.AttackModulationEnvelope);
            error += Math.Abs(polyphoneRegion.ModEnvHoldX - meltyRegion.HoldModulationEnvelope);
            error += Math.Abs(polyphoneRegion.ModEnvDecayX - meltyRegion.DecayModulationEnvelope);
            error += Math.Abs(polyphoneRegion.ModEnvSustainP - meltyRegion.SustainModulationEnvelope);
            error += Math.Abs(polyphoneRegion.ModEnvReleaseX - meltyRegion.ReleaseModulationEnvelope);
            error += Math.Abs(polyphoneRegion.ModEnvToPitchC - meltyRegion.ModulationEnvelopeToPitch);
            error += Math.Abs(polyphoneRegion.ModEnvToFilterC - meltyRegion.ModulationEnvelopeToFilterCutoffFrequency);
            error += Math.Abs(polyphoneRegion.KeyToModEnvHoldC - meltyRegion.KeyNumberToModulationEnvelopeHold);
            error += Math.Abs(polyphoneRegion.KeyToModEnvDecayC - meltyRegion.KeyNumberToModulationEnvelopeDecay);
            error += Math.Abs(polyphoneRegion.ModLfoDelayX - meltyRegion.DelayModulationLfo);
            error += Math.Abs(polyphoneRegion.ModLfoFreqX - meltyRegion.FrequencyModulationLfo);
            error += Math.Abs(polyphoneRegion.ModLfoToPitchC - meltyRegion.ModulationLfoToPitch);
            error += Math.Abs(polyphoneRegion.ModLftToFilterC - meltyRegion.ModulationLfoToFilterCutoffFrequency);
            error += Math.Abs(polyphoneRegion.ModLftToVolumeDb - meltyRegion.ModulationLfoToVolume);
            error += Math.Abs(polyphoneRegion.VibLfoDelayX - meltyRegion.DelayVibratoLfo);
            error += Math.Abs(polyphoneRegion.VibLfoFreqX - meltyRegion.FrequencyVibratoLfo);
            error += Math.Abs(polyphoneRegion.VibLfoPitchC - meltyRegion.VibratoLfoToPitch);
            error += Math.Abs(polyphoneRegion.ChorusP - meltyRegion.ChorusEffectsSend);
            error += Math.Abs(polyphoneRegion.ReverbP - meltyRegion.ReverbEffectsSend);

            return error;
        }

        private static void AreEqual(PolyphoneRegion polyphoneRegion, PresetRegion meltyRegion)
        {
            Assert.That(polyphoneRegion.KeyRange, Is.EqualTo(meltyRegion.KeyRangeStart + "-" + meltyRegion.KeyRangeEnd));
            Assert.That(polyphoneRegion.VelocityRange, Is.EqualTo(meltyRegion.VelocityRangeStart + "-" + meltyRegion.VelocityRangeEnd));
            Assert.That(polyphoneRegion.Attenuation, Is.EqualTo(0.4 * meltyRegion.InitialAttenuation).Within(0.01));
            Assert.That(polyphoneRegion.Pan, Is.EqualTo(meltyRegion.Pan).Within(0.1));
            Assert.That(polyphoneRegion.TuningSemiTones, Is.EqualTo(meltyRegion.CoarseTune));
            Assert.That(polyphoneRegion.TuningCents, Is.EqualTo(meltyRegion.FineTune));
            Assert.That(polyphoneRegion.ScaleTuning, Is.EqualTo(meltyRegion.ScaleTuning));
            Assert.That(polyphoneRegion.FilterCutoffX, Is.EqualTo(meltyRegion.InitialFilterCutoffFrequency).Within(0.001));
            Assert.That(polyphoneRegion.FilterResonanceX, Is.EqualTo(meltyRegion.InitialFilterQ).Within(0.1));
            Assert.That(polyphoneRegion.VolEnvDelayX, Is.EqualTo(meltyRegion.DelayVolumeEnvelope).Within(0.001));
            Assert.That(polyphoneRegion.VolEnvAttackX, Is.EqualTo(meltyRegion.AttackVolumeEnvelope).Within(0.001));
            Assert.That(polyphoneRegion.VolEnvHoldX, Is.EqualTo(meltyRegion.HoldVolumeEnvelope).Within(0.001));
            Assert.That(polyphoneRegion.VolEnvDecayX, Is.EqualTo(meltyRegion.DecayVolumeEnvelope).Within(0.001));
            Assert.That(polyphoneRegion.VolEnvSustainDb, Is.EqualTo(meltyRegion.SustainVolumeEnvelope).Within(0.1));
            Assert.That(polyphoneRegion.VolEnvReleaseX, Is.EqualTo(meltyRegion.ReleaseVolumeEnvelope).Within(0.001));
            Assert.That(polyphoneRegion.KeyToVolEnvHoldC, Is.EqualTo(meltyRegion.KeyNumberToVolumeEnvelopeHold));
            Assert.That(polyphoneRegion.KeyToVolEnvDecayC, Is.EqualTo(meltyRegion.KeyNumberToVolumeEnvelopeDecay));
            Assert.That(polyphoneRegion.ModEnvDelayX, Is.EqualTo(meltyRegion.DelayModulationEnvelope).Within(0.001));
            Assert.That(polyphoneRegion.ModEnvAttackX, Is.EqualTo(meltyRegion.AttackModulationEnvelope).Within(0.001));
            Assert.That(polyphoneRegion.ModEnvHoldX, Is.EqualTo(meltyRegion.HoldModulationEnvelope).Within(0.001));
            Assert.That(polyphoneRegion.ModEnvDecayX, Is.EqualTo(meltyRegion.DecayModulationEnvelope).Within(0.001));
            Assert.That(polyphoneRegion.ModEnvSustainP, Is.EqualTo(meltyRegion.SustainModulationEnvelope).Within(0.1));
            Assert.That(polyphoneRegion.ModEnvReleaseX, Is.EqualTo(meltyRegion.ReleaseModulationEnvelope).Within(0.001));
            Assert.That(polyphoneRegion.ModEnvToPitchC, Is.EqualTo(meltyRegion.ModulationEnvelopeToPitch));
            Assert.That(polyphoneRegion.ModEnvToFilterC, Is.EqualTo(meltyRegion.ModulationEnvelopeToFilterCutoffFrequency));
            Assert.That(polyphoneRegion.KeyToModEnvHoldC, Is.EqualTo(meltyRegion.KeyNumberToModulationEnvelopeHold));
            Assert.That(polyphoneRegion.KeyToModEnvDecayC, Is.EqualTo(meltyRegion.KeyNumberToModulationEnvelopeDecay));
            Assert.That(polyphoneRegion.ModLfoDelayX, Is.EqualTo(meltyRegion.DelayModulationLfo).Within(0.001));
            Assert.That(polyphoneRegion.ModLfoFreqX, Is.EqualTo(meltyRegion.FrequencyModulationLfo).Within(0.001));
            Assert.That(polyphoneRegion.ModLfoToPitchC, Is.EqualTo(meltyRegion.ModulationLfoToPitch));
            Assert.That(polyphoneRegion.ModLftToFilterC, Is.EqualTo(meltyRegion.ModulationLfoToFilterCutoffFrequency));
            Assert.That(polyphoneRegion.ModLftToVolumeDb, Is.EqualTo(meltyRegion.ModulationLfoToVolume).Within(0.1));
            Assert.That(polyphoneRegion.VibLfoDelayX, Is.EqualTo(meltyRegion.DelayVibratoLfo).Within(0.001));
            Assert.That(polyphoneRegion.VibLfoFreqX, Is.EqualTo(meltyRegion.FrequencyVibratoLfo).Within(0.001));
            Assert.That(polyphoneRegion.VibLfoPitchC, Is.EqualTo(meltyRegion.VibratoLfoToPitch));
            Assert.That(polyphoneRegion.ChorusP, Is.EqualTo(meltyRegion.ChorusEffectsSend).Within(0.1));
            Assert.That(polyphoneRegion.ReverbP, Is.EqualTo(meltyRegion.ReverbEffectsSend).Within(0.1));
        }



        private class PolyphoneRegion
        {
            public string KeyRange;
            public string VelocityRange;
            public double Attenuation;
            public double Pan;
            public double TuningSemiTones;
            public double TuningCents;
            public double ScaleTuning;
            public double FilterCutoffX;
            public double FilterResonanceX;
            public double VolEnvDelayX;
            public double VolEnvAttackX;
            public double VolEnvHoldX;
            public double VolEnvDecayX;
            public double VolEnvSustainDb;
            public double VolEnvReleaseX;
            public double KeyToVolEnvHoldC;
            public double KeyToVolEnvDecayC;
            public double ModEnvDelayX;
            public double ModEnvAttackX;
            public double ModEnvHoldX;
            public double ModEnvDecayX;
            public double ModEnvSustainP;
            public double ModEnvReleaseX;
            public double ModEnvToPitchC;
            public double ModEnvToFilterC;
            public double KeyToModEnvHoldC;
            public double KeyToModEnvDecayC;
            public double ModLfoDelayX;
            public double ModLfoFreqX;
            public double ModLfoToPitchC;
            public double ModLftToFilterC;
            public double ModLftToVolumeDb;
            public double VibLfoDelayX;
            public double VibLfoFreqX;
            public double VibLfoPitchC;
            public double ChorusP;
            public double ReverbP;

            public static PolyphoneRegion[] Read(string tsvPath)
            {
                var tsv = File.ReadLines(tsvPath).Select(line => line.Split('\t')).ToArray();

                var globalValues = tsv.Select(row => row[0]).ToArray();

                var regionCount = tsv[0].Length - 1;

                var regions = new PolyphoneRegion[regionCount];

                for (var i = 0; i < regions.Length; i++)
                {
                    var col = i + 1;

                    var localValues = tsv.Select(row => row[col]).ToArray();

                    var region = new PolyphoneRegion();

                    region.KeyRange = GetRange(localValues[0], globalValues[0]);
                    region.VelocityRange = GetRange(localValues[1], globalValues[1]);
                    region.Attenuation = GetValue(localValues[2], globalValues[2], PresetRegion.Default.InitialAttenuation);
                    region.Pan = GetValue(localValues[3], globalValues[3], PresetRegion.Default.Pan);
                    region.TuningSemiTones = GetValue(localValues[4], globalValues[4], PresetRegion.Default.CoarseTune);
                    region.TuningCents = GetValue(localValues[5], globalValues[5], PresetRegion.Default.FineTune);
                    region.ScaleTuning = GetValue(localValues[6], globalValues[6], PresetRegion.Default.ScaleTuning);
                    region.FilterCutoffX = GetValue(localValues[7], globalValues[7], PresetRegion.Default.InitialFilterCutoffFrequency);
                    region.FilterResonanceX = GetValue(localValues[8], globalValues[8], PresetRegion.Default.InitialFilterQ);
                    region.VolEnvDelayX = GetValue(localValues[9], globalValues[9], PresetRegion.Default.DelayVolumeEnvelope);
                    region.VolEnvAttackX = GetValue(localValues[10], globalValues[10], PresetRegion.Default.AttackVolumeEnvelope);
                    region.VolEnvHoldX = GetValue(localValues[11], globalValues[11], PresetRegion.Default.HoldVolumeEnvelope);
                    region.VolEnvDecayX = GetValue(localValues[12], globalValues[12], PresetRegion.Default.DecayVolumeEnvelope);
                    region.VolEnvSustainDb = GetValue(localValues[13], globalValues[13], PresetRegion.Default.SustainVolumeEnvelope);
                    region.VolEnvReleaseX = GetValue(localValues[14], globalValues[14], PresetRegion.Default.ReleaseVolumeEnvelope);
                    region.KeyToVolEnvHoldC = GetValue(localValues[15], globalValues[15], PresetRegion.Default.KeyNumberToVolumeEnvelopeHold);
                    region.KeyToVolEnvDecayC = GetValue(localValues[16], globalValues[16], PresetRegion.Default.KeyNumberToVolumeEnvelopeDecay);
                    region.ModEnvDelayX = GetValue(localValues[17], globalValues[17], PresetRegion.Default.DelayModulationEnvelope);
                    region.ModEnvAttackX = GetValue(localValues[18], globalValues[18], PresetRegion.Default.AttackModulationEnvelope);
                    region.ModEnvHoldX = GetValue(localValues[19], globalValues[19], PresetRegion.Default.HoldModulationEnvelope);
                    region.ModEnvDecayX = GetValue(localValues[20], globalValues[20], PresetRegion.Default.DecayModulationEnvelope);
                    region.ModEnvSustainP = GetValue(localValues[21], globalValues[21], PresetRegion.Default.SustainModulationEnvelope);
                    region.ModEnvReleaseX = GetValue(localValues[22], globalValues[22], PresetRegion.Default.ReleaseModulationEnvelope);
                    region.ModEnvToPitchC = GetValue(localValues[23], globalValues[23], PresetRegion.Default.ModulationEnvelopeToPitch);
                    region.ModEnvToFilterC = GetValue(localValues[24], globalValues[24], PresetRegion.Default.ModulationEnvelopeToFilterCutoffFrequency);
                    region.KeyToModEnvHoldC = GetValue(localValues[25], globalValues[25], PresetRegion.Default.KeyNumberToModulationEnvelopeHold);
                    region.KeyToModEnvDecayC = GetValue(localValues[26], globalValues[26], PresetRegion.Default.KeyNumberToModulationEnvelopeDecay);
                    region.ModLfoDelayX = GetValue(localValues[27], globalValues[27], PresetRegion.Default.DelayModulationLfo);
                    region.ModLfoFreqX = GetValue(localValues[28], globalValues[28], PresetRegion.Default.FrequencyModulationLfo);
                    region.ModLfoToPitchC = GetValue(localValues[29], globalValues[29], PresetRegion.Default.ModulationLfoToPitch);
                    region.ModLftToFilterC = GetValue(localValues[30], globalValues[30], PresetRegion.Default.ModulationLfoToFilterCutoffFrequency);
                    region.ModLftToVolumeDb = GetValue(localValues[31], globalValues[31], PresetRegion.Default.ModulationLfoToVolume);
                    region.VibLfoDelayX = GetValue(localValues[32], globalValues[32], PresetRegion.Default.DelayVibratoLfo);
                    region.VibLfoFreqX = GetValue(localValues[33], globalValues[33], PresetRegion.Default.FrequencyVibratoLfo);
                    region.VibLfoPitchC = GetValue(localValues[34], globalValues[34], PresetRegion.Default.VibratoLfoToPitch);
                    region.ChorusP = GetValue(localValues[35], globalValues[35], PresetRegion.Default.ChorusEffectsSend);
                    region.ReverbP = GetValue(localValues[36], globalValues[36], PresetRegion.Default.ReverbEffectsSend);

                    regions[i] = region;
                }

                return regions;
            }

            private static string GetRange(string local, string global)
            {
                if (local == "!" || local == "?")
                {
                    if (global == "!" || global == "?")
                    {
                        return "0-127";
                    }
                    else
                    {
                        return FormatRange(global);
                    }
                }
                else
                {
                    return FormatRange(local);
                }
            }

            private static string FormatRange(string range)
            {
                if (range.Contains('-'))
                {
                    return range;
                }
                else
                {
                    return range + "-" + range;
                }
            }

            private static double GetValue(string local, string global, double defaultValue)
            {
                if (local == "!" || local == "?")
                {
                    if (global == "!" || global == "?")
                    {
                        return defaultValue;
                    }
                    else
                    {
                        return double.Parse(global);
                    }
                }
                else
                {
                    return double.Parse(local);
                }
            }
        }
    }
}
