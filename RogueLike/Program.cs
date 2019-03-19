using System;
using ConsoleGameEngine;

namespace RogueLike {
    class Program {

        const int c_WinWidth = 96;
        const int c_WinHeight = 48;
        const int c_PixelWidth = 8;
        const int c_PixelHeight = 16;

        static void Main(string[] args) {
            var Engine = new ConsoleEngine(c_WinWidth, c_WinHeight, c_PixelWidth, c_PixelHeight);
            Engine.SetBackground(0);
            //Engine.WriteText(new Point(1, 1), "DOUBLE SHIT", 5);
            //Engine.DisplayBuffer();

            var pntOldSpot = new Point(0, 0);
            var pntNewSpot = new Point(0, 0);
            int intPixelColor = 1;
            int intBlankSpot = 0;
            Random rndRandomPoint = new Random();

            while (true) {
                //Engine.ClearBuffer();
                Engine.SetPixel(pntNewSpot, intPixelColor, ConsoleCharacter.Full);
                Engine.DisplayBuffer();
                RandomizePixel(ref pntNewSpot, ref intPixelColor, rndRandomPoint);
                Engine.SetPixel(pntOldSpot, intBlankSpot, ConsoleCharacter.Full);
                pntOldSpot = pntNewSpot;
                System.Threading.Thread.Sleep(100);
            }
        }

        static void RandomizePixel (ref Point point, ref int color, Random rndNum) {
            var newPoint = new Point(0,0);
            do {
                newPoint.X = rndNum.Next(0, c_WinWidth - 1);
                newPoint.Y = rndNum.Next(0, c_WinHeight - 1);
            } while (newPoint.X == point.X && newPoint.Y == point.Y);
            point = newPoint;
            color = rndNum.Next(1, 100);
        }
    }
}
