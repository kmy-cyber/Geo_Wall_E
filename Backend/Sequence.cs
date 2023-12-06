using INTERPRETE_C__to_HULK;
using G_Wall_E;

namespace G_Wall_E
{
    //pensar despues que hacer cuando me encuentre una secuencia de esta forma : {} ya que no es de un tipo ni de otro
    public interface ISequence<T> //interfaz sequence general para los tipos de secuencia
    {
        List<T> values { get; set; }
        public string name { get; set; }
        bool is_finite { get; set; }
    }

    public class PointSequence : ISequence<Point> //secuencia de puntos
    {
        public List<Point> values { get; set; }
        public string name { get; set; }
        public bool is_finite { get; set; }
        public PointSequence(List<Point> points, string name)
        {
            this.name = name;
            values = points;
        }
        public PointSequence(string name, string color)
        {
            this.name = name;
            values = new List<Point>();
            if (is_finite)
            {
                int count = 0;
                while (count != 10)
                {
                    values.Add(new Point("V" + count.ToString(), color));
                    count++;
                }
            }
        }
    }

    public class LineSequence : ISequence<Line> //secuencia de lineas
    {
        public List<Line> values { get; set; }
        public string name { get; set; }
        public bool is_finite { get; set; }
        public LineSequence(List<Line> values, string name)
        {
            this.name = name;
            this.values = values;
        }
        public LineSequence(string name, string color)
        {
            this.name = name;
            values = new List<Line>();
            if (is_finite)
            {
                int count = 0;
                while (count != 10)
                {
                    values.Add(new Line("V" + count.ToString(), color));
                    count++;
                }
            }
        }
    }

    public class SegmentSequence : ISequence<Segment> //secuencia de segmentos
    {
        public List<Segment> values { get; set; }
        public string name { get; set; }
        public bool is_finite { get; set; }
        public SegmentSequence(List<Segment> values, string name)
        {
            this.name = name;
            this.values = values;
        }
        public SegmentSequence(string name, string color)
        {
            this.name = name;
            values = new List<Segment>();
            if (is_finite)
            {
                int count = 0;
                while (count != 10)
                {
                    values.Add(new Segment("V" + count.ToString(), color));
                    count++;
                }
            }
        }
    }

    public class RaySequence : ISequence<Ray> //secuencia de rayos
    {
        public List<Ray> values { get; set; }
        public string name { get; set; }
        public bool is_finite { get; set; }
        public RaySequence(List<Ray> values, string name)
        {
            this.name = name;
            this.values = values;
        }
        public RaySequence(string name, string color)
        {
            this.name = name;
            values = new List<Ray>();
            if (is_finite)
            {
                int count = 0;
                while (count != 10)
                {
                    values.Add(new Ray("V" + count.ToString(), color));
                    count++;
                }
            }
        }
    }

    public class CircleSequence : ISequence<Circle> //secuencia de circunferencias
    {
        public List<Circle> values { get; set; }
        public string name { get; set; }
        public bool is_finite { get; set; }
        public CircleSequence(List<Circle> values, string name)
        {
            this.name = name;
            this.values = values;
        }
        public CircleSequence(string name, string color)
        {
            this.name = name;
            values = new List<Circle>();
            if (is_finite)
            {
                int count = 0;
                while (count != 10)
                {
                    values.Add(new Circle("V" + count.ToString(), color));
                    count++;
                }
            }
        }
    }

    public class ArcSequence : ISequence<Arc> //secuencia de arcos
    {
        public List<Arc> values { get; set; }
        public string name { get; set; }
        public bool is_finite { get; set; }
        public ArcSequence(List<Arc> values, string name)
        {
            this.name = name;
            this.values = values;
        }
        public ArcSequence(string name, string color)
        {
            this.name = name;
            values = new List<Arc>();
            if (is_finite)
            {
                int count = 0;
                while (count != 10)
                {
                    values.Add(new Arc("V" + count.ToString(), color));
                    count++;
                }
            }
        }
    }

    public class StringSequence : ISequence<string> //secuencia de string
    {
        public List<string> values { get; set; }
        public string name { get; set; }
        public bool is_finite { get; set; }
        public StringSequence(List<string> values, string name)
        {
            this.name = name;
            this.values = values;
        }
    }

    public class IntSequence : ISequence<int> //secuencia de enteros
    {
        public List<int> values { get; set; }
        public string name { get; set; }
        public bool is_finite { get; set; }
        public IntSequence(List<int> values, string name)
        {
            this.name = name;
            this.values = values;
        }
        public IntSequence(string name, int inf, int sup)
        {
            this.name = name;
            values = new List<int>();
            while (inf <= sup)
            {
                values.Add(inf);
                inf++;
            }
        }
        public IntSequence(string name, int inf)
        {
            this.name = name;
            values = new List<int>() { inf }; //ir anadiendo a medida que se vayan pidiendo valores ya que es infinito
        }
    }

    public class DoubleSequence : ISequence<double> //secuencia de doubles
    {
        public List<double> values { get; set; }
        public string name { get; set; }
        public bool is_finite { get; set; }
        public DoubleSequence(List<double> values, string name)
        {
            this.name = name;
            this.values = values;
        }
        public DoubleSequence(string name)
        {
            this.name = name;
            values = new List<double>();
            int count = 0;
            while (count < 10)
            {
                values.Add(new Random().Next() % 1);
                count++;
            }
        }
    }
}