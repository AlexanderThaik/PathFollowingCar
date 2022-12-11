using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Xml.XPath;

namespace CarSimulator
{
    public class Track
    {
        public List<Vector2> racetrack = new List<Vector2>();
        public float pastDistance;
        public float distance;
        public Track()
        {
            pastDistance = 0;
            distance = 0;
            //These for loops make add to a list of waypoints on the track.
            for (double i = 0; i < 14.63; i += 0.1)
            {
                racetrack.Add(new Vector2(0, (float)i));
            }
            for (double i = 0; i <= 8.363; i += 0.1)
            {
                racetrack.Add(new Vector2((float)i, GetArcPoint(69.95f, 8.363f, 14.636f, (float)i, true)));
            }
            for (double i = 8.36; i <= 209.09; i += 0.1)
            {
                racetrack.Add(new Vector2((float)i, 23f));
            }
            for (double i = 209.09; i <= 221.636; i += 0.1)
            {
                racetrack.Add(new Vector2((float)i, GetArcPoint(317.14f, 208.04f, 5.227f, (float)i, true)));
            }
            for (double i = 221.636; i <= 246.73; i += 0.1)
            {
                racetrack.Add(new Vector2((float)i, GetArcPoint(245.88f, 234.18f, 26.14f, (float)i, false)));
            }
            for (double i = 246.72; i <= 259.27; i += 0.1)
            {
                racetrack.Add(new Vector2((float)i, GetArcPoint(317.14f, 260.318f, 5.227f, (float)i, true)));
            }
            for (double i = 259.27; i <= 359.64; i += 0.1)
            {
                racetrack.Add(new Vector2((float)i, 23f));
            }
            for (double i = 359.64; i <= 368; i += 0.1)
            {
                racetrack.Add(new Vector2((float)i, GetArcPoint(69.95f, 359.64f, 14.64f, (float)i, true)));
            }
            for (double i = 14.63f; i >= -4.6; i -= 0.1)
            {
                racetrack.Add(new Vector2(368, (float)i));
            }
        }
        public float GetArcPoint(float radius, float xOffset, float yOffset, float num, bool greater)
        {
            //This is a helper function which gives points on arcs
            if (greater)
                return (float)Math.Sqrt(radius - Math.Pow((num - xOffset), 2)) + yOffset;
            else 
                return (float)Math.Sqrt(radius - Math.Pow((num - xOffset), 2)) * -1 + yOffset;
        }

        public float LeastDistance(Vector2 currentPosition)
        {
            //This function returns the distance between the track and car by comparing the car's position to the list of waypoints
            Vector2 closest = new Vector2(100, 100);
            pastDistance = distance * 1f;
            foreach (Vector2 i in racetrack)
            {
                if (Vector2.Distance(currentPosition, i) < Vector2.Distance(currentPosition, closest))
                {
                    closest = i;
                    distance = Vector2.Distance(currentPosition, closest);
                }
            }
            //Distances are multiplied by -1 depending on if the car is to the left or right of the track
            if (closest.X >= 2.75 && closest.X <= 365.7 && currentPosition.Y > closest.Y)
                distance *= -1;
            else if (closest.X < 2.75 && currentPosition.X < closest.X)
                distance *= -1;
            else if (closest.X > 365.7 && currentPosition.X > closest.X)
                distance *= -1;
            return distance;
        }

        public float Derivative(float pDistance, float cDistance)
        {
            return (cDistance - pDistance) / 0.05f;
        }
    }
}
