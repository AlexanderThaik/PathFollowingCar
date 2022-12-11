using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Xml.XPath;

namespace CarSimulator
{
    public class Car
    {
        public float vLeft;
        public float vRight;
        public Vector2 position;
        public Vector2 step;
        public float angle;
        public float time;
        public int[] layers = new int[] {2, 4, 3, 2};
        public float score;
        public Brain carBrain;
        //Cars are objects with left and right wheel speeds, a current position, angle, time, and brain
        public Car()
        {
            vLeft = 0;
            vRight = 0;
            position = new Vector2(0.0f, 0.0f);
            angle = (float)Math.PI / 2;
            time = 0;
            carBrain = new Brain(layers);
        }

        public Car(Brain b)
        {
            vLeft = 0;
            vRight = 0;
            position = new Vector2(0.0f, 0.0f);
            angle = (float)Math.PI / 2;
            time = 0;
            carBrain = b;
        }

        public float[] ChangeSpeeds(float[] speeds)
        {
            speeds[0] = Math.Min(speeds[0], 1);
            speeds[1] = Math.Min(speeds[1], 1);
            //The numbers in the speeds array are only between 0 and 1, so the car has to be multiplied by 92, which is the max speed of the car in cm/s
            //0.0 here is the base speed
            vLeft = speeds[0] * 92 + 0.0f;
            vRight = speeds[1] * 92 + 0.0f;
            return new float[] { vLeft, vRight };
        }
        public float UpdatePosition()
        {
            //Updating position based on the physics equations
            angle += (float)(3.5 / 13.5) * 0.05f * (vRight - vLeft);
            step = new Vector2((float)(3.5/2 * 0.05f * (vRight + vLeft) * Math.Cos(angle)), (float)(3.5/2 * 0.05f * (vRight + vLeft) * Math.Sin(angle)));
            position = Vector2.Add(position, step);
            return (float)(Math.Sqrt(step.X * step.X + step.Y * step.Y));
        }
    }
}