using INTERPRETE_C__to_HULK;
using G_Wall_E;

namespace G_Wall_E
{
    //aqui se guardan los metodos que devuelven una secuencia de puntos aleatorios de una figura
    public static class Points_Methods
    {
        //punto
        public static PointSequence Points(Point p)
        {
            return new PointSequence(new List<Point>() { p });
        }

        //linea
        public static PointSequence Points(Line p)
        {
            PointSequence ret = new PointSequence(new List<Point>() { });
            var random = new Random();
            List<Point> list = new List<Point>();
            for (int i = 0; i < 5; i++)
            {
                double x = p.P1.X + random.NextDouble() * (p.P2.X - p.P1.X);
                double y = (p.P2.Y - p.P1.Y) / (p.P2.X - p.P1.X) * (x - p.P1.X) + p.P1.Y;
                list.Add(new Point("random", "black", x, y));
            }
            ret.values.Add(list);
            return ret;
        }

        //segmento
        public static PointSequence Points(Segment p)
        {
            PointSequence ret = new PointSequence(new List<Point>() { });
            var random = new Random();
            List<Point> list = new List<Point>();
            for (int i = 0; i < 5; i++)
            {
                double x = p.P1.X + (p.P2.X - p.P1.X) * random.NextDouble();
                double y = p.P1.Y + (p.P2.Y - p.P1.Y) * random.NextDouble();
                list.Add(new Point("random", "black", x, y));
            }
            ret.values.Add(list);
            return ret;
        }

        //rayo
        public static PointSequence Points(Ray p)
        {
            PointSequence ret = new PointSequence(new List<Point>() { });
            var random = new Random();
            List<Point> list = new List<Point>();
            for (int i = 0; i < 5; i++)
            {
                double t = (double)random.NextDouble();
                double x = p.P1.X + t * (p.P2.X - p.P1.X);
                double y = p.P1.Y + t * (p.P2.Y - p.P1.Y);
                list.Add(new Point("random", "black", x, y));
            }
            ret.values.Add(list);
            return ret;
        }

        //arco
        public static PointSequence Points(Arc p)
        {
            bool EstaDentroDelArco(Point p, Arc a)
            {
                // Calcular el ángulo de la línea que conecta el centro del arco con el punto
                double dx = p.X - a.P1.X;
                double dy = p.Y - a.P1.Y;
                double angle = Math.Atan2(dy, dx);

                // Calcular los ángulos de inicio y fin del arco
                double angle1 = Math.Atan2(a.P2.Y - a.P1.Y, a.P2.X - a.P1.X);
                double angle2 = Math.Atan2(a.P3.Y - a.P1.Y, a.P3.X - a.P1.X);

                // Verificar si el punto está dentro del arco
                if (angle >= Math.Min(angle1, angle2) && angle <= Math.Max(angle1, angle2))
                {
                    // Calcular la distancia del punto al centro del arco
                    double distance = Math.Sqrt(dx * dx + dy * dy);

                    // Si la distancia es igual al radio del arco, hay una intersección
                    if (distance <= a.Distance.Value)
                    {
                        return true;
                    }
                }
                // Si llegamos aquí, no hay intersección
                return false;
            }
            PointSequence ret = new PointSequence(new List<Point>() { });
            var random = new Random();
            List<Point> list = new List<Point>();
            for (int i = 0; i < 5; i++)
            {
                double angulo = random.NextDouble() * 2 * Math.PI; // Genera un ángulo aleatorio entre 0 y 2*PI
                double x = p.P1.X + p.Distance.Value * Math.Cos(angulo); // Calcula la coordenada x del punto
                double y = p.P1.Y + p.Distance.Value * Math.Sin(angulo); // Calcula la coordenada y del punto

                // Asegúrate de que el punto generado está dentro del arco
                if (EstaDentroDelArco(new Point("random", "black", x, y), p))
                {
                    list.Add(new Point("random", "black", x, y));
                }
                else
                {
                    i--; // Si el punto no está dentro del arco, lo generamos de nuevo
                }
            }
            ret.values.Add(list);
            return ret;
        }

        //circunferencia
        public static PointSequence Points(Circle p)
        {
            PointSequence ret = new PointSequence(new List<Point>() { });
            var random = new Random();
            List<Point> list = new List<Point>();
            for (int i = 0; i < 5; i++)
            {
                double angle = 2.0 * Math.PI * random.NextDouble();
                double x = p.P1.X + p.Radius.Value * Math.Cos(angle);
                double y = p.P1.Y + p.Radius.Value * Math.Sin(angle);
                list.Add(new Point("random", "black", x, y));
            }
            ret.values.Add(list);
            return ret;
        }
    }
}