using System.Collections.Generic;
using System.Threading;
using System.Numerics;
using Raylib_cs;
using System;

namespace SticksAndPoints
{

    public struct Point
    {
        public Vector2 position, prevPosition;
        public bool locked;
    }

    public struct Stick
    {
        public Point pointA, pointB;
        public float length;
    }
    class Program
    {
        public static int targetFps = 60;
        public static int itterationLoops = 100;
        public static float deltaTime = 1 / targetFps;
        public static float gravity = 10.0f;
        public static List<Stick> sticks = new List<Stick>();
        public static List<Point> points = new List<Point>();
        public static Color renderColor = new Color(255, 255, 255, 255);

        static void Main(string[] args)
        {
            bool running = false;
            Raylib.InitWindow(1600, 900, "Sticks and Points Simulation");
            Raylib.SetTargetFPS(targetFps);
            while (!Raylib.WindowShouldClose())
            {
                if (running)
                {
                    Point point;
                    Stick stick;
                    for (int i = 0; i < points.Count; i++)
                    {
                        point = points[i];
                        if (!point.locked)
                        {
                            Vector2 prevPos = point.position;
                            point.position += point.position - point.prevPosition;
                            point.position += Vector2.UnitY * gravity * deltaTime;
                            point.prevPosition = prevPos;
                        }
                        points[i] = point;
                    }
                    for (int k = 0; k < itterationLoops; k++)
                    {
                        for (int i = 0; i < sticks.Count; i++)
                        {
                            stick = sticks[i];
                            Vector2 stickCenter = (stick.pointA.position + stick.pointB.position) / 2;
                            Vector2 stickDir = Vector2.Normalize(stick.pointA.position - stick.pointB.position);

                            if (!stick.pointA.locked)
                                stick.pointA.position = stick.pointA.position = stickCenter + stickDir * stick.length / 2;
                            if (!stick.pointB.locked)
                                stick.pointB.position = stick.pointB.position = stickCenter - stickDir * stick.length / 2;

                            sticks[i] = stick;
                        }
                    }
                }
                deltaTime = Raylib.GetFrameTime();
                if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE))
                    running = !running;
                if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
                {
                    Point p = new Point();
                    p.position = Raylib.GetMousePosition();
                    p.prevPosition = p.position;
                    points.Add(p);
                }
                    

                for (int i = 0; i < points.Count; i++)
                {
                    Raylib.DrawCircleV(points[i].position, 16, renderColor);
                }
                for (int i = 0; i < sticks.Count; i++)
                {
                    Stick stick = sticks[i];
                    Raylib.DrawLineEx(stick.pointA.position, stick.pointB.position, 8, renderColor);

                }

                Raylib.BeginDrawing();
                Raylib.ClearBackground(new Color(35, 40, 45, 255));
                Raylib.EndDrawing();
            }
        }
    }
}
