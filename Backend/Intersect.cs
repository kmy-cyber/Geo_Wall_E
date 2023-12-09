using INTERPRETE_C__to_HULK;
using G_Wall_E;
using System;
using System.Collections.Generic;

namespace G_Wall_E
{
    //aqui se guardan todos los metodos necesarios para hallar el intercepto entre dos figuras
    public static class Intersect_Methods
    {
        //PUNTOS
        //con punto
        public static PointSequence Intersect(Point p1, Point p2)
        {
            //si los puntos son iguales retorna cualquiera de los dos puntos
            if (p1.X == p2.X && p1.Y == p2.Y) return new PointSequence(new List<Point>() { p1 });

            //si no lo son retornar secuencia vacia
            else return new PointSequence(new List<Point>());
        }

        //con linea
        public static PointSequence Intersect(Point p, Line l)
        {
            if (Point_Line(p, l)) return new PointSequence(new List<Point>() { p });
            else return new PointSequence(new List<Point>());
        }

        //con segmento
        public static PointSequence Intersect(Point p, Segment s)
        {
            if (Point_Segment(p, s)) return new PointSequence(new List<Point>() { p });
            else return new PointSequence(new List<Point>());
        }

        //con rayo
        public static PointSequence Intersect(Point p, Ray r)
        {
            //definiendo direccion del rayo
            string x_dir = "undefined";
            string y_dir = "undefined";
            //verificando si el punto que define el rayo es mínimo en el eje x
            if (r.P1.X < r.P2.X) x_dir = "min";
            //verificando si el punto que define el rayo en el eje x es igual al punto que define la dirección
            else if (r.P1.X == r.P2.X) x_dir = "equals";
            //verificando si el punto que define el rayo es máximo en el eje x
            else x_dir = "max";
            //verificando si el punto que define el rayo es mínimo en el eje y
            if (r.P1.Y < r.P2.Y) y_dir = "min";
            //verificando si el punto que define el rayo en el eje y es igual al punto que define la dirección
            else if (r.P1.Y == r.P2.Y) y_dir = "equals";
            //verificando si el punto que define el rayo es máximo en el eje y
            else y_dir = "max";

            //ya tengo las direcciones del rayo, ahora evaluo que el punto p se encuentre dentro de los límites del rayo
            if ((x_dir == "min" && p.X < r.P1.X) || (x_dir == "max" && p.X > r.P1.X) || (x_dir == "equals" && p.X != r.P1.X) || (y_dir == "min" && p.Y < r.P1.Y) || (y_dir == "max" && p.Y > r.P1.Y) || (y_dir == "equals" && p.Y != r.P1.Y))
            {
                return new PointSequence(new List<Point>());
            }
            else
            {
                //hallando ecuación de la recta
                float m = (float)(r.P2.Y - r.P1.Y) / (float)(r.P2.X - r.P1.X);
                float n = (float)(r.P1.Y - m * r.P1.X);
                //si la igualdad coincide, devuelve p, sino retorna vacío
                if (p.Y == m * p.X + n) return new PointSequence(new List<Point>() { p });
                else return new PointSequence(new List<Point>());
            }
        }

        //con arco
        public static PointSequence Intersect(Point p, Arc a)
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
                if (distance == a.Distance.Value)
                {
                    return new PointSequence(new List<Point>() { p });
                }
            }
            // Si llegamos aquí, no hay intersección
            return new PointSequence(new List<Point>());
        }

        //con circunferencia
        public static PointSequence Intersect(Point p, Circle c)
        {
            if (Math.Pow(p.X - c.P1.X, 2) + Math.Pow(p.Y - c.P1.Y, 2) == Math.Pow(c.Radius.Value, 2)) return new PointSequence(new List<Point>() { p });
            else return new PointSequence(new List<Point>());
        }

        //LINEAS
        //con linea
        public static PointSequence Intersect(Line l1, Line l2)
        {
            //hallando ecuación de la recta
            float l1_m = (float)((l1.P2.Y - l1.P1.Y) / (l1.P2.X - l1.P1.X));
            float l1_n = (float)(l1.P1.Y - l1_m * l1.P1.X);
            float l2_m = (float)((l2.P2.Y - l2.P1.Y) / (l2.P2.X - l2.P1.X));
            float l2_n = (float)(l2.P1.Y - l2_m * l2.P1.X); ;
            if (l1_m == l2_m)
            {
                if (l1_n == l2_n) return new PointSequence(true);
                else return new PointSequence(new List<Point>());
            }
            else
            {
                double xr = (l2_n - l1_n) / (l1_m - l2_m);
                double yr = l1_m * xr + l1_n;
                return new PointSequence(new List<Point>() { new Point("_intersect_", "black", xr, yr) });
            }
        }

        //con segmento
        public static PointSequence Intersect(Line l, Segment s)
        {
            float l_m = (float)((l.P2.Y - l.P1.Y) / (l.P2.X - l.P1.X));
            float l_n = (float)(l.P1.Y - l_m * l.P1.X);
            float s_m = (float)((s.P2.Y - s.P1.Y) / (s.P2.X - s.P1.X));
            float s_n = (float)(s.P1.Y - s_m * s.P1.X); ;
            if (l_m == s_m)
            {
                if (l_n == s_n) return new PointSequence(true);
                else return new PointSequence(new List<Point>());
            }
            else
            {
                double xr = (s_n - l_n) / (l_m - s_m);
                double yr = l_m * xr + l_n;
                if (xr < Math.Min(s.P1.X, s.P2.X) || xr > Math.Max(s.P1.X, s.P2.X) || yr < Math.Min(s.P1.Y, s.P2.Y) || yr > Math.Max(s.P1.Y, s.P2.Y)) return new PointSequence(new List<Point>());
                else return new PointSequence(new List<Point>() { new Point("_intersect_", "black", xr, yr) });
            }
        }

        //con rayo
        public static PointSequence Intersect(Line l, Ray r)
        {
            float l_m = (float)((l.P2.Y - l.P1.Y) / (l.P2.X - l.P1.X));
            float l_n = (float)(l.P1.Y - l_m * l.P1.X);
            float r_m = (float)((r.P2.Y - r.P1.Y) / (r.P2.X - r.P1.X));
            float r_n = (float)(r.P1.Y - r_m * r.P1.X); ;
            if (l_m == r_m)
            {
                if (l_n == r_n) return new PointSequence(true);
                else return new PointSequence(new List<Point>());
            }
            else
            {
                double xr = (r_n - l_n) / (l_m - r_m);
                double yr = l_m * xr + l_n;
                string x_dir;
                string y_dir;
                //verificando si el punto que define el rayo es mínimo en el eje x
                if (r.P1.X < r.P2.X) x_dir = "min";
                //verificando si el punto que define el rayo en el eje x es igual al punto que define la dirección
                else if (r.P1.X == r.P2.X) x_dir = "equals";
                //verificando si el punto que define el rayo es máximo en el eje x
                else x_dir = "max";
                //verificando si el punto que define el rayo es mínimo en el eje y
                if (r.P1.Y < r.P2.Y) y_dir = "min";
                //verificando si el punto que define el rayo en el eje y es igual al punto que define la dirección
                else if (r.P1.Y == r.P2.Y) y_dir = "equals";
                //verificando si el punto que define el rayo es máximo en el eje y
                else y_dir = "max";

                //ya tengo las direcciones del rayo, ahora evaluo que el punto p se encuentre dentro de los límites del rayo
                if ((x_dir == "min" && xr < r.P1.X) || (x_dir == "max" && xr > r.P1.X) || (x_dir == "equals" && xr != r.P1.X) || (y_dir == "min" && yr < r.P1.Y) || (y_dir == "max" && yr > r.P1.Y) || (y_dir == "equals" && yr != r.P1.Y))
                {
                    return new PointSequence(new List<Point>());
                }
                else return new PointSequence(new List<Point>() { new Point("_intersect_", "black", xr, yr) });
            }
        }

        //con arco
        public static PointSequence Intersect(Line l, Arc a)
        {
            //hallando vector de direccion de la recta
            double dx = l.P2.X - l.P1.X;
            double dy = l.P2.Y - l.P1.Y;
            //hallando ecuación paramétrica de la recta
            double A = dx * dx + dy * dy;
            double B = 2 * (dx * (l.P1.X - a.P1.X) + dy * (l.P1.Y - a.P1.Y));
            double C = Math.Pow(l.P1.X - a.P1.X, 2) + Math.Pow(l.P1.Y - a.P1.Y, 2) - Math.Pow(a.Distance.Value, 2);
            //calculando determinante
            double det = B * B - 4 * A * C;
            //no intersecta en ningún punto
            if (det < 0) return new PointSequence(new List<Point>());
            //intersecta en un solo punto
            else if (det == 0)
            {
                double t = -B / (2 * A);
                return Intersect(new Point("_intersection_", "black", l.P1.X + t * dx, l.P1.Y + t * dy), a);
            }
            //intersecta en dos puntos
            else
            {
                double t1 = (-B + Math.Sqrt(det)) / (2 * A);
                double t2 = (-B - Math.Sqrt(det)) / (2 * A);
                PointSequence P_1 = Intersect(new Point("_intersection_", "black", l.P1.X + t1 * dx, l.P1.Y + t1 * dy), a);
                PointSequence P_2 = Intersect(new Point("_intersection_", "black", l.P1.X + t2 * dx, l.P1.Y + t2 * dy), a);
                if (P_1.Count == 0 && P_2.Count != 0) return P_2;
                if (P_2.Count == 0 && P_1.Count != 0) return P_1;
                if (P_2.Count == 0 && P_1.Count == 0) return P_1;
                return P_1.Concat(P_2);
            }
        }

        //con circunferencia
        public static PointSequence Intersect(Line l, Circle c)
        {
            //hallando vector de direccion de la recta
            double dx = l.P2.X - l.P1.X;
            double dy = l.P2.Y - l.P1.Y;
            //hallando ecuación paramétrica de la recta
            double A = dx * dx + dy * dy;
            double B = 2 * (dx * (l.P1.X - c.P1.X) + dy * (l.P1.Y - c.P1.Y));
            double C = Math.Pow((l.P1.X - c.P1.X), 2) + Math.Pow((l.P1.Y - c.P1.Y), 2) - Math.Pow(c.Radius.Value, 2);
            //calculando determinante
            double det = B * B - 4 * A * C;
            //no intersecta en ningún punto
            if (det < 0) return new PointSequence(new List<Point>());
            //intersecta en un solo punto
            else if (det == 0)
            {
                double t = -B / (2 * A);
                return new PointSequence(new List<Point>() { new Point("_intersection_", "black", l.P1.X + t * dx, l.P1.Y + t * dy) });
            }
            //intersecta en dos puntos
            else
            {
                double t1 = (-B + Math.Sqrt(det)) / (2 * A);
                double t2 = (-B - Math.Sqrt(det)) / (2 * A);
                return new PointSequence(new List<Point>() { new Point("_intersection_", "black", l.P1.X + t1 * dx, l.P1.Y + t1 * dy), new Point("_intersection_", "black", l.P1.X + t2 * dx, l.P1.Y + t2 * dy) });
            }
        }

        //SEGMENTOS
        //con segmento
        public static PointSequence Intersect(Segment s1, Segment s2)
        {
            float s1_m = (float)((s1.P2.Y - s1.P1.Y) / (s1.P2.X - s1.P1.X));
            float s1_n = (float)(s1.P1.Y - s1_m * s1.P1.X);
            float s2_m = (float)((s2.P2.Y - s2.P1.Y) / (s2.P2.X - s2.P1.X));
            float s2_n = (float)(s2.P1.Y - s2_m * s2.P1.X); ;
            if (s1_m == s2_m)
            {
                if (s1_n == s2_n) return new PointSequence(true);
            }
            double p1x = s1.P1.X;
            double p1y = s1.P1.Y;
            double p2x = s1.P2.X;
            double p2y = s1.P2.Y;
            double p3x = s2.P1.X;
            double p3y = s2.P1.Y;
            double p4x = s2.P2.X;
            double p4y = s2.P2.Y;
            double d = (p2x - p1x) * (p4y - p3y) - (p2y - p1y) * (p4x - p3x);

            if (d == 0)
            {
                return new PointSequence(new List<Point>());
            }

            double t = ((p1x - p3x) * (p4y - p3y) - (p1y - p3y) * (p4x - p3x)) / (double)d;
            double u = -((p1x - p3x) * (p2y - p1y) - (p1y - p3y) * (p2x - p1x)) / (double)d;

            if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
            {

                Point p = new Point("_intersection_", "black", p1x + (int)(t * (p2x - p1x)), p1y + (int)(t * (p2y - p1y)));
                return new PointSequence(new List<Point>() { p });
            }
            return new PointSequence(new List<Point>());
        }

        //con rayo
        public static PointSequence Intersect(Segment s, Ray r)
        {
            float l_m = (float)((r.P2.Y - r.P1.Y) / (r.P2.X - r.P1.X));
            float l_n = (float)(r.P1.Y - l_m * r.P1.X);
            float r_m = (float)((r.P2.Y - r.P1.Y) / (r.P2.X - r.P1.X));
            float r_n = (float)(r.P1.Y - r_m * r.P1.X); ;
            if (l_m == r_m)
            {
                if (l_n == r_n) return new PointSequence(true);
                else return new PointSequence(new List<Point>());
            }
            else
            {
                double xr = (r_n - l_n) / (l_m - r_m);
                double yr = l_m * xr + l_n;
                string x_dir;
                string y_dir;
                //verificando si el punto que define el rayo es mínimo en el eje x
                if (r.P1.X < r.P2.X) x_dir = "min";
                //verificando si el punto que define el rayo en el eje x es igual al punto que define la dirección
                else if (r.P1.X == r.P2.X) x_dir = "equals";
                //verificando si el punto que define el rayo es máximo en el eje x
                else x_dir = "max";
                //verificando si el punto que define el rayo es mínimo en el eje y
                if (r.P1.Y < r.P2.Y) y_dir = "min";
                //verificando si el punto que define el rayo en el eje y es igual al punto que define la dirección
                else if (r.P1.Y == r.P2.Y) y_dir = "equals";
                //verificando si el punto que define el rayo es máximo en el eje y
                else y_dir = "max";

                //ya tengo las direcciones del rayo, ahora evaluo que el punto p se encuentre dentro de los límites del rayo
                if ((!Point_Segment(new Point("", "black", xr, yr), s)) || (x_dir == "min" && xr < r.P1.X) || (x_dir == "max" && xr > r.P1.X) || (x_dir == "equals" && xr != r.P1.X) || (y_dir == "min" && yr < r.P1.Y) || (y_dir == "max" && yr > r.P1.Y) || (y_dir == "equals" && yr != r.P1.Y))
                {
                    return new PointSequence(new List<Point>());
                }
                else return new PointSequence(new List<Point>() { new Point("_intersect_", "black", xr, yr) });
            }
        }

        //con arco
        public static PointSequence Intersect(Segment s, Arc a)
        {
            //hallando vector de direccion de la recta
            double dx = s.P2.X - s.P1.X;
            double dy = s.P2.Y - s.P1.Y;
            //hallando ecuación paramétrica de la recta
            double A = dx * dx + dy * dy;
            double B = 2 * (dx * (s.P1.X - a.P1.X) + dy * (s.P1.Y - a.P1.Y));
            double C = Math.Pow(s.P1.X - a.P1.X, 2) + Math.Pow(s.P1.Y - a.P1.Y, 2) - Math.Pow(a.Distance.Value, 2);
            //calculando determinante
            double det = B * B - 4 * A * C;
            //no intersecta en ningún punto
            if (det < 0) return new PointSequence(new List<Point>());
            //intersecta en un solo punto
            else if (det == 0)
            {
                double t = -B / (2 * A);
                if (Point_Segment(new Point("_intersection_", "black", s.P1.X + t * dx, s.P1.Y + t * dy), s)) return Intersect(new Point("_intersection_", "black", s.P1.X + t * dx, s.P1.Y + t * dy), a);
                else return new PointSequence(new List<Point>());
            }
            //intersecta en dos puntos
            else
            {
                double t1 = (-B + Math.Sqrt(det)) / (2 * A);
                double t2 = (-B - Math.Sqrt(det)) / (2 * A);
                PointSequence P_1 = Intersect(new Point("_intersection_", "black", s.P1.X + t1 * dx, s.P1.Y + t1 * dy), a);
                PointSequence P_2 = Intersect(new Point("_intersection_", "black", s.P1.X + t2 * dx, s.P1.Y + t2 * dy), a);
                if (P_1.Count == 0 && P_2.Count != 0)
                {
                    if (Point_Segment(new Point("_intersection_", "black", s.P1.X + t2 * dx, s.P1.Y + t2 * dy), s)) return P_2;
                }
                if (P_2.Count == 0 && P_1.Count != 0)
                {
                    if (Point_Segment(new Point("_intersection_", "black", s.P1.X + t1 * dx, s.P1.Y + t1 * dy), s)) return P_1;
                }
                if (P_2.Count != 0 && P_1.Count != 0)
                {
                    if (Point_Segment(new Point("_intersection_", "black", s.P1.X + t2 * dx, s.P1.Y + t2 * dy), s) && Point_Segment(new Point("_intersection_", "black", s.P1.X + t1 * dx, s.P1.Y + t1 * dy), s)) return P_1.Concat(P_2);
                    if (!Point_Segment(new Point("_intersection_", "black", s.P1.X + t2 * dx, s.P1.Y + t2 * dy), s) && Point_Segment(new Point("_intersection_", "black", s.P1.X + t1 * dx, s.P1.Y + t1 * dy), s)) return P_1;
                    if (Point_Segment(new Point("_intersection_", "black", s.P1.X + t2 * dx, s.P1.Y + t2 * dy), s) && !Point_Segment(new Point("_intersection_", "black", s.P1.X + t1 * dx, s.P1.Y + t1 * dy), s)) return P_2;
                }
                return new PointSequence(new List<Point>());
            }
        }

        //con circunferencia
        public static PointSequence Intersect(Segment s, Circle c)
        {
            //hallando vector de direccion de la recta
            double dx = s.P2.X - s.P1.X;
            double dy = s.P2.Y - s.P1.Y;
            //hallando ecuación paramétrica de la recta
            double A = dx * dx + dy * dy;
            double B = 2 * (dx * (s.P1.X - c.P1.X) + dy * (s.P1.Y - c.P1.Y));
            double C = Math.Pow(s.P1.X - c.P1.X, 2) + Math.Pow(s.P1.Y - c.P1.Y, 2) - Math.Pow(c.Radius.Value, 2);
            //calculando determinante
            double det = B * B - 4 * A * C;
            //no intersecta en ningún punto
            if (det < 0) return new PointSequence(new List<Point>());
            //intersecta en un solo punto
            else if (det == 0)
            {
                double t = -B / (2 * A);
                if (Point_Segment(new Point("_intersection_", "black", s.P1.X + t * dx, s.P1.Y + t * dy), s))
                {
                    return new PointSequence(new List<Point>() { new Point("_intersection_", "black", s.P1.X + t * dx, s.P1.Y + t * dy) });
                }
                return new PointSequence(new List<Point>());
            }
            //intersecta en dos puntos
            else
            {
                double t1 = (-B + Math.Sqrt(det)) / (2 * A);
                double t2 = (-B - Math.Sqrt(det)) / (2 * A);
                if (Point_Segment(new Point("_intersection_", "black", s.P1.X + t1 * dx, s.P1.Y + t1 * dy), s) && !Point_Segment(new Point("_intersection_", "black", s.P1.X + t2 * dx, s.P1.Y + t2 * dy), s))
                {
                    return new PointSequence(new List<Point>() { new Point("_intersection_", "black", s.P1.X + t1 * dx, s.P1.Y + t1 * dy) });
                }
                if (Point_Segment(new Point("_intersection_", "black", s.P1.X + t2 * dx, s.P1.Y + t2 * dy), s) && !Point_Segment(new Point("_intersection_", "black", s.P1.X + t1 * dx, s.P1.Y + t1 * dy), s))
                {
                    return new PointSequence(new List<Point>() { new Point("_intersection_", "black", s.P1.X + t2 * dx, s.P1.Y + t2 * dy) });
                }
                if (Point_Segment(new Point("_intersection_", "black", s.P1.X + t2 * dx, s.P1.Y + t2 * dy), s) && Point_Segment(new Point("_intersection_", "black", s.P1.X + t1 * dx, s.P1.Y + t1 * dy), s))
                {
                    return new PointSequence(new List<Point>() { new Point("_intersection_", "black", s.P1.X + t1 * dx, s.P1.Y + t1 * dy), new Point("_intersection_", "black", s.P1.X + t2 * dx, s.P1.Y + t2 * dy) });
                }
                return new PointSequence(new List<Point>());
            }
        }

        //RAYOS
        //con rayo
        public static PointSequence Intersect(Ray r1, Ray r2)
        {
            // Calculando las pendientes de las líneas
            double m1 = r1.P2.Y - r1.P1.Y / r1.P2.X - r1.P1.X;
            double m2 = r2.P2.Y - r2.P1.Y / r2.P2.X - r2.P1.X;

            // Calculando los interceptos en Y de las líneas
            double yIntercept1 = r1.P1.Y - m1 * r1.P1.X;
            double yIntercept2 = r2.P1.Y - m2 * r2.P1.X;

            // Calculando la intersección en X
            double xIntersection = (yIntercept2 - yIntercept1) / (m1 - m2);

            // Calculando la intersección en Y
            double yIntersection = m1 * xIntersection + yIntercept1;

            // Verificando que la intercepción pertenezca a ambos rayos
            if ((xIntersection - r1.P1.X) / r1.P2.X - r1.P1.X >= 0 && (xIntersection - r2.P1.X) / r2.P2.X - r2.P1.X >= 0)
            {
                return new PointSequence(new List<Point>() { new Point("_intersect_", "black", xIntersection, yIntersection) });
            }
            if (m1 == m2)
            {
                if (Math.Sign(r1.P2.Y - r1.P1.Y) == Math.Sign(r2.P2.Y - r2.P1.Y) && Math.Sign(r1.P2.X - r1.P1.X) == Math.Sign(r2.P2.X - r2.P1.X))
                {
                    return new PointSequence(true);
                }
            }
            return new PointSequence(new List<Point>());
        }

        //con arco
        public static PointSequence Intersect(Ray p, Arc a)
        {
            //hallando vector de direccion de la recta
            double dx = p.P2.X - p.P1.X;
            double dy = p.P2.Y - p.P1.Y;
            //hallando ecuación paramétrica de la recta
            double A = dx * dx + dy * dy;
            double B = 2 * (dx * (p.P1.X - a.P1.X) + dy * (p.P1.Y - a.P1.Y));
            double C = Math.Pow(p.P1.X - a.P1.X, 2) + Math.Pow(p.P1.Y - a.P1.Y, 2) - Math.Pow(a.Distance.Value, 2);
            //calculando determinante
            double det = B * B - 4 * A * C;
            //no intersecta en ningún punto
            if (det < 0) return new PointSequence(new List<Point>());
            //intersecta en un solo punto
            else if (det == 0)
            {
                double t = -B / (2 * A);
                if (Point_Ray(new Point("_intersection_", "black", p.P1.X + t * dx, p.P1.Y + t * dy), p)) return Intersect(new Point("_intersection_", "black", p.P1.X + t * dx, p.P1.Y + t * dy), a);
                else return new PointSequence(new List<Point>());
            }
            //intersecta en dos puntos
            else
            {
                double t1 = (-B + Math.Sqrt(det)) / (2 * A);
                double t2 = (-B - Math.Sqrt(det)) / (2 * A);
                PointSequence P_1 = Intersect(new Point("_intersection_", "black", p.P1.X + t1 * dx, p.P1.Y + t1 * dy), a);
                PointSequence P_2 = Intersect(new Point("_intersection_", "black", p.P1.X + t2 * dx, p.P1.Y + t2 * dy), a);
                if (P_1.Count == 0 && P_2.Count != 0)
                {
                    if (Point_Ray(new Point("_intersection_", "black", p.P1.X + t2 * dx, p.P1.Y + t2 * dy), p)) return P_2;
                }
                if (P_2.Count == 0 && P_1.Count != 0)
                {
                    if (Point_Ray(new Point("_intersection_", "black", p.P1.X + t1 * dx, p.P1.Y + t1 * dy), p)) return P_1;
                }
                if (P_2.Count != 0 && P_1.Count != 0)
                {
                    if (Point_Ray(new Point("_intersection_", "black", p.P1.X + t2 * dx, p.P1.Y + t2 * dy), p) && Point_Ray(new Point("_intersection_", "black", p.P1.X + t1 * dx, p.P1.Y + t1 * dy), p)) return P_1.Concat(P_2);
                    if (!Point_Ray(new Point("_intersection_", "black", p.P1.X + t2 * dx, p.P1.Y + t2 * dy), p) && Point_Ray(new Point("_intersection_", "black", p.P1.X + t1 * dx, p.P1.Y + t1 * dy), p)) return P_1;
                    if (Point_Ray(new Point("_intersection_", "black", p.P1.X + t2 * dx, p.P1.Y + t2 * dy), p) && !Point_Ray(new Point("_intersection_", "black", p.P1.X + t1 * dx, p.P1.Y + t1 * dy), p)) return P_2;
                }
                return new PointSequence(new List<Point>());
            }
        }

        //con circunferencia
        public static PointSequence Intersect(Ray s, Circle c)
        {
            //hallando vector de direccion de la recta
            double dx = s.P2.X - s.P1.X;
            double dy = s.P2.Y - s.P1.Y;
            //hallando ecuación paramétrica de la recta
            double A = dx * dx + dy * dy;
            double B = 2 * (dx * (s.P1.X - c.P1.X) + dy * (s.P1.Y - c.P1.Y));
            double C = Math.Pow(s.P1.X - c.P1.X, 2) + Math.Pow(s.P1.Y - c.P1.Y, 2) - Math.Pow(c.Radius.Value, 2);
            //calculando determinante
            double det = B * B - 4 * A * C;
            //no intersecta en ningún punto
            if (det < 0) return new PointSequence(new List<Point>());
            //intersecta en un solo punto
            else if (det == 0)
            {
                double t = -B / (2 * A);
                if (Point_Ray(new Point("_intersection_", "black", s.P1.X + t * dx, s.P1.Y + t * dy), s))
                {
                    return new PointSequence(new List<Point>() { new Point("_intersection_", "black", s.P1.X + t * dx, s.P1.Y + t * dy) });
                }
                return new PointSequence(new List<Point>());
            }
            //intersecta en dos puntos
            else
            {
                double t1 = (-B + Math.Sqrt(det)) / (2 * A);
                double t2 = (-B - Math.Sqrt(det)) / (2 * A);
                if (Point_Ray(new Point("_intersection_", "black", s.P1.X + t1 * dx, s.P1.Y + t1 * dy), s) && !Point_Ray(new Point("_intersection_", "black", s.P1.X + t2 * dx, s.P1.Y + t2 * dy), s))
                {
                    return new PointSequence(new List<Point>() { new Point("_intersection_", "black", s.P1.X + t1 * dx, s.P1.Y + t1 * dy) });
                }
                if (Point_Ray(new Point("_intersection_", "black", s.P1.X + t2 * dx, s.P1.Y + t2 * dy), s) && !Point_Ray(new Point("_intersection_", "black", s.P1.X + t1 * dx, s.P1.Y + t1 * dy), s))
                {
                    return new PointSequence(new List<Point>() { new Point("_intersection_", "black", s.P1.X + t2 * dx, s.P1.Y + t2 * dy) });
                }
                if (Point_Ray(new Point("_intersection_", "black", s.P1.X + t2 * dx, s.P1.Y + t2 * dy), s) && Point_Ray(new Point("_intersection_", "black", s.P1.X + t1 * dx, s.P1.Y + t1 * dy), s))
                {
                    return new PointSequence(new List<Point>() { new Point("_intersection_", "black", s.P1.X + t1 * dx, s.P1.Y + t1 * dy), new Point("_intersection_", "black", s.P1.X + t2 * dx, s.P1.Y + t2 * dy) });
                }
                return new PointSequence(new List<Point>());
            }
        }

        //ARCOS
        //con arco
        public static PointSequence Intersect(Arc c1, Arc c2)
        {
            // Verificar si los arcos son el mismo
            if (c1.P1.X == c2.P1.X && c1.P1.Y == c2.P1.Y && c1.P2.X == c2.P2.X && c1.P2.Y == c2.P2.Y && c1.P3.X == c2.P3.X && c1.P3.Y == c2.P3.Y) return new PointSequence(true);
            double d = Math.Sqrt(Math.Pow(c2.P1.X - c1.P1.X, 2) + Math.Pow(c2.P1.Y - c1.P1.Y, 2));
            if (d > c1.Distance.Value + c2.Distance.Value || d < Math.Abs(c1.Distance.Value - c2.Distance.Value)) return new PointSequence(new List<Point>());
            double a = Math.Pow(c1.Distance.Value, 2) - Math.Pow(c2.Distance.Value, 2) + d * d / 2 * d;
            double h = Math.Sqrt(Math.Pow(c1.Distance.Value, 2)) - Math.Pow(a, 2);
            double x = c1.P1.X + a * (c2.P1.X - c1.P1.X) / d;
            double y = c1.P1.Y + a * (c2.P1.Y - c1.P1.Y) / d;
            double x1 = x + h * (c2.P1.Y - c1.P1.Y) / d;
            double y1 = y - h * (c2.P1.X - c1.P1.X) / d;
            double x2 = x - h * (c2.P1.Y - c1.P1.Y) / d;
            double y2 = y + h * (c2.P1.X - c1.P1.X) / d;
            Point P1 = new Point("i", "black", x1, y1);
            Point P2 = new Point("i", "black", x2, y2);
            if (x1 == x2 && y1 == y2 && Point_Arc(P1, c1) && Point_Arc(P1, c2)) return new PointSequence(new List<Point>() { P1 });
            if (Point_Arc(P1, c1) && Point_Arc(P1, c2) && Point_Arc(P2, c1) && Point_Arc(P2, c2)) return new PointSequence(new List<Point>() { P1, P2 });
            if (Point_Arc(P1, c1) && Point_Arc(P1, c2)) return new PointSequence(new List<Point>() { P1 });
            if (Point_Arc(P2, c1) && Point_Arc(P2, c2)) return new PointSequence(new List<Point>() { P2 });
            return new PointSequence(new List<Point>());
        }


        //con circunferencia
        public static PointSequence Intersect(Arc p, Circle c)
        {
            // Verificar si los arcos son el mismo
            if (p.P1.X == c.P1.X && p.P1.Y == p.P1.Y && p.Distance.Value == c.Radius.Value) return new PointSequence(true);
            double d = Math.Sqrt(Math.Pow(c.P1.X - p.P1.X, 2) + Math.Pow(c.P1.Y - p.P1.Y, 2));
            if (d > p.Distance.Value + c.Radius.Value || d < Math.Abs(p.Distance.Value - c.Radius.Value)) return new PointSequence(new List<Point>());
            double a = Math.Pow(p.Distance.Value, 2) - Math.Pow(c.Radius.Value, 2) + d * d / 2 * d;
            double h = Math.Sqrt(Math.Pow(p.Distance.Value, 2)) - Math.Pow(a, 2);
            double x = p.P1.X + a * (c.P1.X - p.P1.X) / d;
            double y = p.P1.Y + a * (c.P1.Y - p.P1.Y) / d;
            double x1 = x + h * (c.P1.Y - p.P1.Y) / d;
            double y1 = y - h * (c.P1.X - p.P1.X) / d;
            double x2 = x - h * (c.P1.Y - p.P1.Y) / d;
            double y2 = y + h * (c.P1.X - p.P1.X) / d;
            Point P1 = new Point("i", "black", x1, y1);
            Point P2 = new Point("i", "black", x2, y2);
            if (x1 == x2 && y1 == y2 && Point_Arc(P1, p)) return new PointSequence(new List<Point>() { P1 });
            if (Point_Arc(P1, p) && Point_Arc(P2, p)) return new PointSequence(new List<Point>() { P1, P2 });
            if (Point_Arc(P1, p)) return new PointSequence(new List<Point>() { P1 });
            if (Point_Arc(P2, p)) return new PointSequence(new List<Point>() { P2 });
            return new PointSequence(new List<Point>());
        }

        //CIRCUNFERENCIAS
        //con circunferencia
        public static PointSequence Intersect(Circle c1, Circle c2)
        {
            if (c1.P1.X == c2.P1.X && c1.P1.Y == c2.P1.Y && c1.Radius.Value == c2.Radius.Value) return new PointSequence(true);
            double d = Math.Sqrt(Math.Pow(c2.P1.X - c1.P1.X, 2) + Math.Pow(c2.P1.Y - c1.P1.Y, 2));
            if (d > c1.Radius.Value + c2.Radius.Value || d < Math.Abs(c1.Radius.Value - c2.Radius.Value)) return new PointSequence(new List<Point>());
            double a = Math.Pow(c1.Radius.Value, 2) - Math.Pow(c2.Radius.Value, 2) + (d * d) / 2 * d;
            double h = Math.Sqrt(Math.Pow(c1.Radius.Value, 2)) - Math.Pow(a, 2);
            double x = c1.P1.X + a * (c2.P1.X - c1.P1.X) / d;
            double y = c1.P1.Y + a * (c2.P1.Y - c1.P1.Y) / d;
            double x1 = x + h * (c2.P1.Y - c1.P1.Y) / d;
            double y1 = y - h * (c2.P1.X - c1.P1.X) / d;
            double x2 = x - h * (c2.P1.Y - c1.P1.Y) / d;
            double y2 = y + h * (c2.P1.X - c1.P1.X) / d;
            Point P1 = new Point("i", "black", x1, y1);
            Point P2 = new Point("i", "black", x2, y2);
            if (x1 == x2 && y1 == y2) return new PointSequence(new List<Point>() { P1 });
            return new PointSequence(new List<Point>() { P1, P2 });
        }
        //métodos auxiliares
        static bool Point_Line(Point p, Line l)
        {
            //hallando ecuación de la recta
            float m = (float)(l.P2.Y - l.P1.Y) / (float)(l.P2.X - l.P1.X);
            float n = (float)(l.P1.Y - m * l.P1.X);
            //si la igualdad coincide, devuelve p, sino retorna vacío
            if (p.Y == m * p.X + n) return true;
            return false;
        }
        static bool Point_Segment(Point p, Segment s)
        {
            double Producto_cruz = (s.P2.Y - s.P1.Y) * (p.X - s.P1.X) - (s.P2.X - s.P1.X) * (p.Y - s.P1.Y);
            if (Math.Abs(Producto_cruz) > 1e-10) return false;

            double Producto_escalar = (p.X - s.P1.X) * (s.P2.X - s.P1.X) + (p.Y - s.P1.Y) * (s.P2.Y - s.P1.Y);
            if (Producto_escalar < 0)
            {
                return false;
            }
            //calculando la longitud al cuadrado
            double lc = (s.P2.X - s.P1.X) * (s.P2.X - s.P1.X) + (s.P2.Y - s.P1.Y) * (s.P2.Y - s.P1.Y);
            if (Producto_escalar > lc)
            {
                return false;
            }

            return true;
        }
        static bool Point_Ray(Point p, Ray r)
        {
            double Vx = p.X - r.P1.X;
            double Vy = p.Y - r.P1.Y;
            double Rx = r.P2.X - r.P1.X;
            double Ry = r.P2.Y - r.P1.Y;
            double dotProduct = Rx * Vx + Ry * Vy;
            double R_Magnitude = Math.Sqrt(Rx * Rx + Ry * Ry);
            double V_Magnitude = Math.Sqrt(Vx * Vx + Vy * Vy);
            double angle = Math.Acos(dotProduct / (R_Magnitude * V_Magnitude));
            return Math.Abs(angle) < 0.0001;
        }
        static bool Point_Arc(Point p, Arc a)
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
                if (distance == a.Distance.Value)
                {
                    return true;
                }
            }
            // Si llegamos aquí, no hay intersección
            return false;
        }
    }

}