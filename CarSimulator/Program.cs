using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Xml.XPath;
using static System.Formats.Asn1.AsnWriter;
using static System.Net.Mime.MediaTypeNames;

namespace CarSimulator
{
    public class Program
    {
        Vector2 finishLine = new Vector2(368, 0);
        public static void Main()
        {
            var Simulation = new Program();
            List<Car> myCarList = Simulation.GenerateCars(100);
            float learningRate = 0.005f;
            //i is the number of iterations the AI is trained on
            for (int i = 0; i < 400; i++)
            {
                if (i == 200)
                    learningRate = 0.001f;
                if (i == 400)
                    learningRate = 0.0005f;
                if (i == 600)
                    learningRate = 0.0001f;
                if (i == 800)
                    learningRate = 0.0005f;
                Car bestCar = Simulation.FindBestCar(myCarList);
                //0.5 is the exploration%
                //This for loop generates completely new cars with random neural networks as brains
                for (int j = 0; j < (int)(0.5 * myCarList.Count); j++)
                {
                    if (myCarList[j].carBrain == bestCar.carBrain)
                        myCarList[j] = new Car(bestCar.carBrain);
                    else
                        myCarList[j] = new Car();
                }
                //This for loop slightly modifies the best car from the past group of 100 cars
                for (int j = (int)(0.5 * myCarList.Count); j < myCarList.Count; j++)
                {
                    if (myCarList[j].carBrain == bestCar.carBrain)
                        myCarList[j] = new Car(bestCar.carBrain);
                    else
                        myCarList[j] = new Car(myCarList[j].carBrain.Mutate(bestCar.carBrain, learningRate));
                }
                Console.WriteLine("Iteration " + i + ": Best car scored " + bestCar.score + " at " + bestCar.position + " in " + bestCar.time + " s");
                //After the AI is done training, all the weights are printed out
                if (i == 399)
                    bestCar.carBrain.StoreWeights();
            }
        }
        public List<Car> GenerateCars(int n)
        {
            //This function creates a list of the number of specified cars
            List<Car> carList = new List<Car>();
            for (int i = 0; i < n; i++)
            {
                carList.Add(new Car());
            }
            return carList;
        }
        public float CalculateScore(Car testCar)
        {
            bool finished = false;
            Track myTrack = new Track();
            int trackPoints = myTrack.racetrack.Count;
            List<Vector2> pointsUnpassed = new List<Vector2>(myTrack.racetrack);
            int count = 0;
            while (!finished)
            {
                //If the car has not finished after a certain amount of time, end its run
                if (count >= 2999)
                {
                    finished = true;
                }
                //Takes in distance and velocity as inputs
                float[] inputs = new float[] { myTrack.distance / 8 + 0.5f, myTrack.Derivative(myTrack.pastDistance, myTrack.distance) / 180 + 0.5f};
                //Makes sure inputs are between -1 and 1
                inputs[0] = Math.Max(Math.Min(inputs[0], 1f), -1f);
                inputs[1] = Math.Max(Math.Min(inputs[1], 1f), -1f);
                float[] speeds = testCar.ChangeSpeeds(testCar.carBrain.FeedForward(inputs));
                testCar.UpdatePosition();
                //Rewards the car for going faster
                testCar.score += (testCar.vLeft + testCar.vRight)/2;
                myTrack.LeastDistance(testCar.position);
                //First part of this statement penalizes the car based on how far it is from the track, second part is a flat penalty which is meant to penalize based on time
                testCar.score -= (float)Math.Pow(myTrack.distance, 2) * 0.25f + 5.0f;
                pointsUnpassed = Checkpoints(testCar, pointsUnpassed);
                //If the car goes more than 3 cm from the center of the track, end its run
                if (Math.Abs(myTrack.distance) > 3)
                {
                    finished = true;
                }
                //If the car gets close to the finish line, reward it with a lot of points
                if (Vector2.Distance(testCar.position, finishLine) < 1)
                {
                    testCar.score += 10000;
                    finished = true;
                }
                count++;
            }
            //The car's calculations are meant to run every 50 ms, for each iteration through the while loop, 50 ms elapses
            testCar.time = count * 0.05f;
            //Reward the car based on how many waypoints it passed
            testCar.score += (trackPoints - pointsUnpassed.Count);
            return testCar.score;
        }
        public Car FindBestCar(List<Car> carList)
        {
            //Function goes through all the cars in the list, finds the one with the best score and returns it
            float highScore = -9999;
            Car best = null;
            for (int i = 0; i < carList.Count; i++)
            {
                carList[i].score = CalculateScore(carList[i]);
                if (carList[i].score > highScore)
                {
                    best = carList[i];
                    highScore = carList[i].score;
                }
            }
            return best;
        }

        public List<Vector2> Checkpoints (Car testCar, List <Vector2> pointsUnpassed)
        {
            //Checks if which waypoints the car has passed
            while (pointsUnpassed.Count > 0 && PointDistance(testCar.position, pointsUnpassed[0]))
            {
                pointsUnpassed.RemoveAt(0);
            }
            return pointsUnpassed;
        }
        public bool PointDistance(Vector2 carPosition, Vector2 trackPosition)
        {
            //Checks if the car is close enough to a waypoint for it to be given credit
            return (Vector2.Distance(carPosition, trackPosition) <= 4.6f);
        }
    }
}