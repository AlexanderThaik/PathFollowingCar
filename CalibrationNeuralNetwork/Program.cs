using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace CalibrationNeuralNetwork
{
    public class Program
    {
        public static void Main()
        {
            string filepath = @"C:\Users\athai\Downloads\Calibration Data - Sheet8 (1).csv";
            int[] layers = new int[] {8, 8, 4, 2, 1};
            Network myNetwork = new Network(layers);
            float guess;
            int size = 81;
            float[][] newInputs = new float[size][];
            float[] newOutputs = new float[size];
            float[] expected = new float[size];
            List<string> sensors = new List<string>();
            sensors = File.ReadAllLines(filepath).ToList();
            int count = 0;
            foreach (string i in sensors)
            {
                if (count >= size)
                    break;
                string[] items = i.Split(',');
                List<float> itemList = new List<float>(Array.ConvertAll(items, s => float.Parse(s)));
                newInputs[count] = new float[8];
                float min = 2600;
                for (int j = 0; j < itemList.Count - 1; j++)
                {
                    newInputs[count][j] = itemList[j];
                    if (newInputs[count][j] < min)
                        min = newInputs[count][j];
                }
                float max = 0;
                for (int j = 0; j < itemList.Count - 1; j++)
                {
                    newInputs[count][j] -= min;
                    if (newInputs[count][j] > max)
                        max = newInputs[count][j];
                }
                for (int j = 0; j < itemList.Count - 1; j++)
                {
                    newInputs[count][j] /= max;
                }
                expected[count] = itemList[itemList.Count - 1] / 80 + 0.5f;
                count++;
            }
            for (int a = 0; a < 2000; a++)
            {
                float totalError = 0;
                for (int i = 0; i < newInputs.Length; i++)
                {
                    guess = myNetwork.BackProp(newInputs[i], expected[i]);
                    totalError += Math.Abs((guess - 0.5f) * 80f - (expected[i] - 0.5f) * 80f);
                }
                if (totalError / size < 2)
                {
                    Console.WriteLine(a + " iterations needed");
                    Console.WriteLine("Average Error is " + Math.Round(totalError / size) + " mm");
                    for (int i = 0; i < newInputs.Length; i++)
                    {
                        guess = myNetwork.BackProp(newInputs[i], expected[i]);
                        Console.WriteLine("Guess is " + Math.Round((guess - 0.5) * 80));
                        //Console.WriteLine("Guess is " + guess);
                        Console.WriteLine("Expected is " + Math.Round((expected[i] - 0.5) * 80));
                        Console.WriteLine("");
                    }
                    myNetwork.StoreWeights();
                    break;
                }
                myNetwork.Mutate();
            }
        }
    }
}