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

namespace CalibrationNeuralNetwork
{
    public class Network
    {
        public int[] layers;
        public float[][] neurons;
        public float[][] biases;
        public float[][][] weights;
        public float[][] dneurons;
        public float[][] dbiases;
        public float[][][] dweights;
        private const float learningRate = 0.005f;

        public Network(int[] alayers)
        {
            layers = new int[alayers.Length];
            neurons = new float[alayers.Length][];
            biases = new float[alayers.Length][];
            weights = new float[alayers.Length - 1][][];
            dneurons = new float[alayers.Length][];
            dbiases = new float[alayers.Length][];
            dweights = new float[alayers.Length - 1][][];

            for (int i = 0; i < alayers.Length; i++)
            {
                layers[i] = alayers[i];
                neurons[i] = new float[alayers[i]];
                biases[i] = new float[alayers[i]];
                dneurons[i] = new float[alayers[i]];
                dbiases[i] = new float[alayers[i]];
            }
            for (int i = 0; i < alayers.Length - 1; i++)
            {
                weights[i] = new float[neurons[i + 1].Length][];
                dweights[i] = new float[neurons[i + 1].Length][];
                for (var j = 0; j < weights[i].Length; j++)
                {
                    weights[i][j] = new float[neurons[i].Length];
                    dweights[i][j] = new float[neurons[i].Length];

                    for (var k = 0; k < weights[i][j].Length; k++)
                    {
                        Random rand = new Random();
                        weights[i][j][k] = (float)((float)(rand.NextDouble() - 0.5) * Math.Sqrt(2f / weights[i][j].Length));
                    }
                }
            }
        }

        public float FeedForward(float[] inputs)
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
                    dneurons[i][j] = neurons[i][j];
                }
            }

            return neurons[neurons.Length - 1][0];
        }

        public float BackProp(float[] inputs, float expected)
        {
            float output = FeedForward(inputs);

            dneurons[dneurons.Length - 1][0] = expected;
            for (int i = neurons.Length - 1; i >= 1; i--)
            {
                for (int j = 0; j < neurons[i].Length; j++)
                {
                    var biasSmudge = dReLU(neurons[i][j]) * -1f * (neurons[i][j] - dneurons[i][j]);
                    //var biasSmudge = dReLU(neurons[i][j]) * dneurons[i][j];
                    dbiases[i][j] += biasSmudge;

                    for (var k = 0; k < neurons[i - 1].Length; k++)
                    {
                        var weightSmudge = neurons[i - 1][k] * biasSmudge;
                        dweights[i - 1][j][k] += weightSmudge;

                        var valueSmudge = weights[i - 1][j][k] * biasSmudge;
                        dneurons[i - 1][k] += valueSmudge;
                    }
                }
            }
            return output;
        }

        public void Mutate()
        {
            for (var i = neurons.Length - 1; i >= 1; i--)
            {
                for (var j = 0; j < neurons[i].Length; j++)
                {
                    biases[i][j] += dbiases[i][j] * learningRate;
                    dbiases[i][j] = 0;

                    for (var k = 0; k < neurons[i - 1].Length; k++)
                    {
                        weights[i - 1][j][k] += dweights[i - 1][j][k] * learningRate;
                        dweights[i - 1][j][k] = 0;
                    }
                    dneurons[i][j] = 0;
                }
            }
        }
        public void StoreWeights()
        {
            string filepath = @"C:\Users\athai\Downloads\carweights.txt";
            File.Delete(filepath);
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
            File.AppendAllLines(filepath, weightOutputList);
        }

        public float ReLU(float x)
        {
            return (x > 0) ? x : 0;
        }

        public float dReLU(float x)
        {
            return (x > 0) ? 1 : 0;
        }
        public float Sigmoid(float x)
        {
            return 1f / (1f + (float)Math.Exp(-x));
        }
        public float dSigmoid(float x)
        {
            return (float)(x * (1 - x));
        }
        public float[] Softmax(float[] x)
        {
            float total = 0;
            for (int i = 0; i < 10; i++)
            {
                total += (float)Math.Exp(x[i]);
            }
            for (int i = 0; i < 10; i++)
            {
                x[i] = (float)Math.Exp(x[i]) / total;
            }
            return x;
        }
        public float[] OneHot(int x)
        {
            float[] OneHots = new float[10];
            OneHots[x] = 1f;
            return OneHots;
        }

    }
}

