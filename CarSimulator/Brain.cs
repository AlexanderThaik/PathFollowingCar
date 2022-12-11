using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Xml.XPath;

namespace CarSimulator
{
    public class Brain
    {
        public int[] layers;
        public float[][] neurons;
        public float[][] biases;
        public float[][][] weights;

        public Brain(int[] alayers)
        {
            layers = new int[alayers.Length];
            neurons = new float[alayers.Length][];
            biases = new float[alayers.Length][];
            weights = new float[alayers.Length - 1][][];

            for (int i = 0; i < alayers.Length; i++)
            {
                layers[i] = alayers[i];
                neurons[i] = new float[alayers[i]];
                biases[i] = new float[alayers[i]];
            }
            for (int i = 1; i < layers.Length; i++)
            {
                for (int j = 0; j < biases[i].Length; j++)
                {
                    Random rand = new Random();
                    biases[i][j] = (float)(rand.NextDouble() * 2.0 - 1.0) / 1;
                }
            }
            for (int i = 0; i < alayers.Length - 1; i++)
            {
                weights[i] = new float[neurons[i + 1].Length][];
                for (var j = 0; j < weights[i].Length; j++)
                {
                    weights[i][j] = new float[neurons[i].Length];

                    for (var k = 0; k < weights[i][j].Length; k++)
                    {
                        Random rand = new Random();
                        weights[i][j][k] = (float)(rand.NextDouble() * 2.0 - 1.0) / 1;
                    }
                }
            }
        }

        public float[] FeedForward(float[] inputs)
        {
            for (int i = 0; i < inputs.Length; i++)
            {
                neurons[0][i] = inputs[i];
            }
            for (int i = 1; i < neurons.Length; i++)
            {
                for (int j = 0; j < neurons[i].Length; j++)
                {
                    float value = 0f;
                    for (int k = 0; k < neurons[i - 1].Length; k++)
                    {
                        value += weights[i - 1][j][k] * neurons[i - 1][k];
                    }
                    neurons[i][j] = ReLU(value + biases[i][j]);
                }
            }

            return neurons[neurons.Length - 1];
        }
        public Brain Mutate(Brain smartest, float amount)
        {
            int[] layers = new int[] { 2, 4, 3, 2 };
            Brain newBrain = new Brain(layers);
            for (var i = neurons.Length - 1; i >= 1; i--)
            {
                for (var j = 0; j < neurons[i].Length; j++)
                {
                    Random rand = new Random();
                    newBrain.biases[i][j] = Lerp(smartest.biases[i][j], (float)(rand.NextDouble() * 2.0 - 1.0), amount);
                    for (var k = 0; k < neurons[i - 1].Length; k++)
                    {
                        Random rand2 = new Random();
                        newBrain.weights[i-1][j][k] = Lerp(smartest.weights[i-1][j][k], (float)(rand2.NextDouble() * 2.0 - 1.0), amount);
                    }
                }
            }
            return newBrain;
        }

        public void StoreWeights()
        {
            string weightOutput = "";
            string biasOutput = "";
            for (int i = 0; i < weights.Length; i++)
            {
                Console.WriteLine(i + "row of weights");
                for (int j = 0; j < weights[i].Length; j++)
                {
                    for (int k = 0; k < weights[i][j].Length; k++)
                    {
                        weightOutput += weights[i][j][k];
                        weightOutput += ",";
                    }
                }
                weightOutput.Remove(weightOutput.Length - 1, 1);
                Console.WriteLine(weightOutput);
                weightOutput = "";
                Console.WriteLine("");
            }
            for (int i = 0; i < biases.Length; i++)
            {
                Console.WriteLine(i + "row of biases");
                for (int j = 0; j < biases[i].Length; j++)
                {
                    biasOutput += biases[i][j];
                    biasOutput += ",";
                }
                biasOutput.Remove(biasOutput.Length - 1, 1);
                Console.WriteLine(biasOutput);
                biasOutput = "";
                Console.WriteLine("");
            }
            Console.WriteLine(weightOutput);
            Console.WriteLine("");
            Console.WriteLine(biasOutput);
            List<string> weightOutputList = new List<string>();
        }
        public float ReLU(float x)
        {
            return (x > 0) ? x : 0;
        }

        public float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }
    }
}
