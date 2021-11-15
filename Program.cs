using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;
using System;

namespace SticksAndPoints
{

    public class Point
    {
        public Vector2 position, prevPosition;
        public bool locked;
        public bool selected;
        public bool shouldExist = true;
    }

    public class Stick
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
            //Raylib.SetTargetFPS(targetFps);
            Point point1 = new Point();
            Point point2 = new Point();
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
                            if(stick.pointA == null || stick.pointB == null){
                                continue;
                            }
                            Vector2 stickCenter = (stick.pointA.position + stick.pointB.position) / 2;
                            Vector2 stickDir = Vector2.Normalize(stick.pointA.position - stick.pointB.position);

                            if (!stick.pointA.locked)
                                stick.pointA.position = stick.pointA.position = stickCenter + stickDir * stick.length / 2;
                            if (!stick.pointB.locked)
                                stick.pointB.position = stick.pointB.position = stickCenter - stickDir * stick.length / 2;

                            sticks[i] = stick;

                            if(!stick.pointA.shouldExist || !stick.pointB.shouldExist){
                                sticks.Remove(stick);
                            }
                        }
                    }
                }
                deltaTime = Raylib.GetFrameTime();
                if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE))
                    running = !running;
                if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
                {
                    if(!Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL)){
                        Point p = new Point();
                        p.position = Raylib.GetMousePosition();
                        p.prevPosition = p.position;
                        p.locked = Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT);
                        points.Add(p);
                    }else if(Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL)){
                        Point closestPoint = null;
                        int index = 0;
                        float dist = 10000;
                        foreach(Point p in points){
                            Vector2 mousePoint = Raylib.GetMousePosition();
                            for(int i = 0; i < points.Count; i++){
                                Point curPoint = points[i];
                                if (curPoint == null)
                                    continue;
                                float curDist = Raymath.Vector2Distance(mousePoint, curPoint.position);
                                if (dist > curDist){
                                    index = i;
                                    closestPoint = curPoint;
                                    dist = curDist;
                                }
                            }
                        }
                        if (closestPoint != null){
                            closestPoint.shouldExist = false;
                            points[index] = closestPoint;
                            points.Remove(closestPoint);
                        }
                    }
                }


                if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_RIGHT_BUTTON))
                {
                    if (point1 == null|| point2 == null){
                        if(points.Count >= 2){
                            Point closestPoint = new Point();
                            float dist = 10000;
                            Vector2 mousePoint = Raylib.GetMousePosition();
                            for(int i = 0; i < points.Count; i++){
                                Point curPoint = points[i];
                                float curDist = Raymath.Vector2Distance(mousePoint, curPoint.position);
                                if (dist > curDist){
                                    closestPoint = curPoint;
                                    dist = curDist;
                                }
                            }
                            if(closestPoint != null){
                                int i = points.FindIndex(0, points.Count, x => x.Equals(closestPoint));
                                if (point1 == null){
                                    point1 = points[i];
                                    point1.selected = true;
                                }else{
                                    point2 = points[i];
                                    point2.selected = true;
                                }
                            }
                        }
                        
                    }
                    
                    try{
                        if (point1 == point2){
                        point1.selected = false;
                        point1 = null;
                        point2 = null;
                    }
                    }catch(Exception e){
                        System.Console.WriteLine(e);
                        //point1.selected = false;
                        point1 = null;
                        point2 = null;
                    }
                    

                    if(point1 != null && point2 != null){
                        Stick stick = new Stick();
                        stick.pointA = point1;
                        stick.pointB = point2;
                        stick.length = Raymath.Vector2Distance(point1.position, point2.position);
                        sticks.Add(stick);
                        point1.selected = false;
                        point2.selected = false;
                        point1 = null;
                        point2 = null;
                    }
                }   

                if(Raylib.IsKeyPressed(KeyboardKey.KEY_R)){
                    sticks = new List<Stick>();
                    points = new List<Point>();
                }

                for (int i = 0; i < points.Count; i++)
                {
                    if (points[i] != null){
                        if (!points[i].locked){
                            Raylib.DrawCircleV(points[i].position, 16, renderColor);
                        }else{
                            Raylib.DrawCircleV(points[i].position, 16, new Color(235, 50, 60, 255));
                        }

                        if(points[i].selected){
                           Raylib.DrawCircleV(points[i].position, 16, new Color(35, 255, 50, 128)); 
                        }
                    }
                }
                for (int i = 0; i < sticks.Count; i++)
                {
                    Stick stick = sticks[i];
                    if (stick != null){
                        if (stick.pointA != null && stick.pointB != null){
                            Raylib.DrawLineEx(stick.pointA.position, stick.pointB.position, 8, renderColor);
                        }
                    }

                }

                Raylib.BeginDrawing();
                Raylib.ClearBackground(new Color(35, 40, 45, 255));
                Raylib.EndDrawing();
            }
        }
    }
}
