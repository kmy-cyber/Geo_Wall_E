using System;
using System.Globalization;
using System.Numerics;
using System.Text.Json.Serialization;
using Microsoft.VisualBasic.CompilerServices;
namespace G_Wall_E
{
    public interface IFigure
    {
    }

    public interface IDrawable
    {
        public string Msg { get; set; }
        public string Color { get; }
        public string Name { get; }

        public DrawableProperties Export();
    }

    public class DrawableProperties
    {
        public string Msg { get; set; }
        public string Color { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }

        public double? X { get; set; }
        public double? Y { get; set; }
        public DrawableProperties P1 { get; set; }
        public DrawableProperties P2 { get; set; }
        public DrawableProperties P3 { get; set; }
        public double? Radius { get; set; }
    }

    public class Point : IDrawable
    {
        public string Color { get; private set; }
        public string Name { get; private set; }
        
        [JsonPropertyName("coordinateX")]
        public double X { get; set; }
        
        [JsonPropertyName("coordinateY")]
        public double Y { get; set; }
        public string Msg { get; set; }

        public Point(string name, string color)
        {
            Name = name;
            Color = color;
            X = new Random().Next() % 525;
            Y = new Random().Next() % 600;
        }
        
        public Point(string name, string color, double x, double y)
        {
            Name = name;
            Color = color;
            X = x;
            Y = y;
        }

        public DrawableProperties Export()
        {
            return new DrawableProperties {
                Name = Name,
                Color = Color,
                Type = "point",
                Msg = Msg,
                X = X,
                Y = Y
            };
        }
    }

    public class Line : IDrawable
    {
        public string Color { get; private set; }
        public string Name { get; private set; }
        public Point P1 { get; set; }
        public Point P2 { get; set; }
        public string Msg { get; set; }

        public Line(string name, string color)
        {
            Name = name;
            Color = color;
            P1 = new Point(name, color);
            P2 = new Point(name, color);
        }

        public Line(string name, string color, Point p1, Point p2)
        {
            Name = name;
            Color = color;
            P1 = p1;
            P2 = p2;
        }

        public DrawableProperties Export()
        {
            return new DrawableProperties {
                Name = Name,
                Color = Color,
                Type = "line",
                Msg = Msg,
                P1 = P1.Export(),
                P2 = P2.Export()
            };
        }
    }

    public class Segment : IDrawable
    {
        public string Color { get; private set; }
        public string Name { get; private set; }
        public Point P1 { get; set; }
        public Point P2 { get; set; }
        public string Msg { get; set; }

        public Segment(string name, string color)
        {
            Name = name;
            Color = color;
            P1 = new Point(name, color);
            P2 = new Point(name, color);
        }

        public Segment(string name, string color, Point p1, Point p2)
        {
            Name = name;
            Color = color;
            P1 = p1;
            P2 = p2;
        }

        public DrawableProperties Export()
        {
            return new DrawableProperties {
                Name = Name,
                Color = Color,
                Type = "segment",
                Msg = Msg,
                P1 = P1.Export(),
                P2 = P2.Export()
            };
        }
    }

    public class Ray : IDrawable
    {
        public string Color { get; private set; }
        public string Name { get; private set; }
        public Point P1 { get; set; }
        public Point P2 { get; set; }
        public string Msg { get; set; }

        public Ray(string name, string color)
        {
            Name = name;
            Color = color;
            P1 = new Point(name, color);
            P2 = new Point(name, color);
        }

        public Ray(string name, string color, Point p1, Point p2)
        {
            Name = name;
            Color = color;
            P1 = p1;
            P2 = p2;
        }

        public DrawableProperties Export()
        {
            return new DrawableProperties {
                Name = Name,
                Color = Color,
                Type = "ray",
                Msg = Msg,
                P1 = P1.Export(),
                P2 = P2.Export()
            };
        }
    }

    public class Circle : IDrawable
    {
        public string Color { get; private set; }
        public string Name { get; private set; }
        public Point P1 { get; set; }
        public Measure Radius { get; set; }
        public string Msg { get; set; }

        public Circle(string name, string color)
        {
            Name = name;
            Color = color;
            P1 = new Point(name, color);
            Point P2 = new Point(name, color);
            Radius = new Measure(color, "meassure", P1, P2);
        }

        public Circle(string name, string color, Point p1, Measure radius)
        {
            Name = name;
            Color = color;
            P1 = p1;
            Radius = radius;
        }

        public DrawableProperties Export()
        {
            return new DrawableProperties {
                Name = Name,
                Color = Color,
                Type = "circle",
                Msg = Msg,
                P1 = P1.Export(),
                Radius = Radius.Execute()
            };
        }
    }

    public class Arc : IDrawable
    {
        public string Color { get; private set; }
        public string Name { get; private set; }
        public Point P1 { get; set; }
        public Point P2 { get; set; }
        public Point P3 { get; set; }
        public Measure Distance { get; set; }
        public string Msg { get; set; }

        public Arc(string color, string name)
        {
            Color = color;
            Name = name;
            P1 = new Point(name, color);
            P2 = new Point(name, color);
            P3 = new Point(name, color);
            Distance = new Measure(color, name, P1, P3);
        } 

        public Arc(string color, string name, Point p1, Point p2, Point p3, Measure distance)
        {
            Color = color;
            Name = name;
            P1 = p1;
            P2 = p2;
            P3 = p3;
            Distance = distance;
        }       

        public DrawableProperties Export()
        {
            return new DrawableProperties {
                Name = Name,
                Color = Color,
                Type = "arc",
                Msg = Msg,
                P1 = P1.Export(),
                P2 = P2.Export(),
                P3 = P3.Export(),
                Radius = Distance.Execute()
            };
        }
    }

    public class Measure : IComparable<Measure>
    {
        public double Value { get; set; }
        public string Color { get; private set; }
        public string Name { get; private set; }
        public Point P1 { get; set; }
        public Point P2 { get; set; }

        public int CompareTo(Measure other)
        {
            if(other == null)
            {
                return -1;
            }

            return Value.CompareTo(other.Value);
        }

        public Measure(string color, string name, Point p1, Point p2)
        {
            //Color = color;
            Name = name;
            P1 = p1;
            P2 = p2;
        }

        public double Execute()
        {
            double xDiff = P1.X - P2.X;
            double yDiff = P1.Y - P2.Y;

            return Math.Sqrt(Convert.ToDouble(xDiff * xDiff + yDiff * yDiff));
        }
    }
}