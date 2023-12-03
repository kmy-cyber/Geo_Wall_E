using System;
using System.Globalization;
using Microsoft.VisualBasic.CompilerServices;
namespace G_Wall_E
{
    public interface IFigure
    {
    }

    public interface IGeometric_Place
    {
        public string Color { get; }
        public string Name { get; }
    }


    public class Point<T> : IGeometric_Place
    {
        public string Color { get; private set; }
        public string Name { get; private set; }
        public T X { get; set; }
        public T Y { get; set; }

        public Point(string name, string color)
        {
            Name = name;
            Color = color;
            //X = new Random();
            //Y = new Random(); 
        }
        
        public Point(string name, string color, T x, T y)
        {
            Name = name;
            Color = color;
            X = x;
            Y = y;
        }
    }


    public class Line<T> : IGeometric_Place
    {
        public string Color { get; private set; }
        public string Name { get; private set; }
        public Point<T> P1 { get; set; }
        public Point<T> P2 { get; set; }

        public Line(string name, string color)
        {
            Name = name;
            Color = color;
            P1 = new Point<T>(name, color);
            P2 = new Point<T>(name, color);
        }

        public Line(string name, string color, Point<T> p1, Point<T> p2)
        {
            Name = name;
            Color = color;
            P1 = p1;
            P2 = p2;
        }
    }


    public class Segment<T> : IGeometric_Place
    {
        public string Color { get; private set; }
        public string Name { get; private set; }
        public Point<T> P1 { get; set; }
        public Point<T> P2 { get; set; }

        public Segment(string name, string color)
        {
            Name = name;
            Color = color;
            P1 = new Point<T>(name, color);
            P2 = new Point<T>(name, color);
        }

        public Segment(string name, string color, Point<T> p1, Point<T> p2)
        {
            Name = name;
            Color = color;
            P1 = p1;
            P2 = p2;
        }
    }


    public class Ray<T> : IGeometric_Place
    {
        public string Color { get; private set; }
        public string Name { get; private set; }
        public Point<T> P1 { get; set; }
        public Point<T> P2 { get; set; }

        public Ray(string name, string color)
        {
            Name = name;
            Color = color;
            P1 = new Point<T>(name, color);
            P2 = new Point<T>(name, color);
        }

        public Ray(string name, string color, Point<T> p1, Point<T> p2)
        {
            Name = name;
            Color = color;
            P1 = p1;
            P2 = p2;
        }
    }


    public class Circle<T> : IGeometric_Place where T : IComparable<T>
    {
        public string Color { get; private set; }
        public string Name { get; private set; }
        public Point<T> P1 { get; set; }
        public Measure<T> Radius { get; set; }

        public Circle(string name, string color)
        {
            Name = name;
            Color = color;
            P1 = new Point<T>(name, color);
            Radius = new Measure<T>(); //da error de que no le estas dando los parametros de entrada
        }

        public Circle(string name, string color, Point<T> p1, Measure<T> radius)
        {
            Name = name;
            Color = color;
            P1 = p1;
            Radius = radius;
        }
    }


    public class Arc<T> : IGeometric_Place where T : IComparable<T>
    {
        public string Color { get; private set; }
        public string Name { get; private set; }
        public Point<T> P1 { get; set; }
        public Point<T> P2 { get; set; }
        public Point<T> P3 { get; set; }
        public Measure<T> Distance { get; set; }

        public Arc(string color, string name)
        {
            Color = color;
            Name = name;
            P1 = new Point<T>(name, color);
            P2 = new Point<T>(name, color);
            P3 = new Point<T>(name, color);
            Distance = new Measure<T>(); //aqui da error de que no le estas dando los parametros de entrada para crear measure
        } 

        public Arc(string color, string name, Point<T> p1, Point<T> p2, Point<T> p3, Measure<T> distance)
        {
            Color = color;
            Name = name;
            P1 = p1;
            P2 = p2;
            P3 = p3;
            Distance = distance;
        }       
    }


    public class Measure<T> : IComparable<Measure<T>> where T : IComparable<T>
    {
        public T Value { get; set; }
        //public string Color { get; private set; } //measure no lleva color
        public string Name { get; private set; }
        public delegate T Operator(T a, T b);
        public Point<T> P1 { get; set; }
        public Point<T> P2 { get; set; }

        public int CompareTo(Measure<T> other)
        {
            if(other == null)
            {
                return -1;
            }

            return Value.CompareTo(other.Value);
        }

        public Measure(/*string color,*/ string name, Point<T> p1, Point<T> p2)
        {
            //Color = color;
            Name = name;
            P1 = p1;
            P2 = p2;

        }
    }


   // public class Sequence
   // {
   //     public IEnumerable<IGeometric_Place> 
   // }
}