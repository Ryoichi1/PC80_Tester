using NAudio.Wave;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.IntegralTransforms;
using System.Numerics;

//下記サイトを参考にした
//http://wildpie.hatenablog.com/entry/2014/09/24/000900

namespace PC80_Tester
{
    public class FFT
    {
        public double Freq { get; private set; }
        public double Vol { get; private set; }

        public List<DataPoint> FreqPoints { get; set; }
        public List<DataPoint> VolPoints { get; set; }

        List<float> _recorded = new List<float>(); // 音声データ

        WaveIn waveIn;

        public bool Init()
        {
            try
            {
                waveIn = new WaveIn()
                {
                    DeviceNumber = 0, // Default
                };
                waveIn.DataAvailable += WaveIn_DataAvailable;
                waveIn.WaveFormat = new WaveFormat(sampleRate: 16000, channels: 1);
                waveIn.StartRecording();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Close()
        {
            waveIn.Dispose();
        }


        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            // 32bitで最大値1.0fにする
            for (int index = 0; index < e.BytesRecorded; index += 2)
            {
                short sample = (short)((e.Buffer[index + 1] << 8) | e.Buffer[index + 0]);

                float sample32 = sample / 32768f;
                ProcessSample(sample32);
            }
        }



        private void ProcessSample(float sample)
        {
            _recorded.Add(sample);

            if (_recorded.Count == 1024)
            {
                VolPoints = _recorded.Select((v, index) =>
                        new DataPoint((double)index, v)
                    ).ToList();


                Vol = VolPoints.OrderByDescending(p => p.Y).First().Y;

 
                var windowsize = 1024;
                var window = MathNet.Numerics.Window.Hamming(windowsize);
                _recorded = _recorded.Select((v, i) => v * (float)window[i]).ToList();

                Complex[] complexData = _recorded.Select(v => new Complex(v, 0.0)).ToArray();

                Fourier.Forward(complexData, FourierOptions.Matlab); // arbitrary length
                //Fourier.Radix2Forward(complexData.ToArray(), FourierOptions.Default);

                var s = windowsize * (1.0 / 16000.0);
                FreqPoints = complexData.Take(complexData.Count() / 2).Select((v, index) =>
                      new DataPoint(((double)index / s),
                          Math.Sqrt(v.Real * v.Real + v.Imaginary * v.Imaginary))
                ).ToList();

                //マイク 7
                //マイクブースト　+20.0dB
                //マイクの設定を上記レベルにすると、周りの音を拾っても0.1以上にはならなかった

                if (Vol < 0.1)
                {
                    Freq = 0;
                }
                else
                {
                    Freq = FreqPoints.OrderByDescending(p => p.Y).First().X;
                }
                
                _recorded.Clear();
            }
        }


    }
}
