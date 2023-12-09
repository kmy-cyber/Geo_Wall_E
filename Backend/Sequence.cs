using INTERPRETE_C__to_HULK;
using G_Wall_E;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System;

namespace G_Wall_E
{
    //pensar despues que hacer cuando me encuentre una secuencia de esta forma : {} ya que no es de un tipo ni de otro
    public interface ISequence<T> //interfaz sequence general para los tipos de secuencia
    {
        public List<List<T>> values { get; set; } //lsita de secuencias que conforman a la secuencia concatenada
        public string name { get; set; } //nombre de la secuencia
        //las unicas secuecias que pueden ser infinitas son las de enteros
        public bool is_infinite { get; set; } //define si una secuencia es infinita o no, si es verdadero, la ultima de la lista de secuencias concatenadas es la que es infinita
        public bool is_undefined { get; set; } //define si la secuencia esta indefinida o no
        public int Count { get; set; } //define la cantidad de elementos que tiene la secuencia 
        public List<int> is_undefined_concat { get; set; } //define en que indices de la lista de secuencias, las secuencias estan concatenadas con undefined
    }

    /*public class UnknownSequence<T> : ISequence<T>
    {
        List<T> values { get; set; }
        public string? name { get; set; }
        bool is_finite { get; set; }
        List<T> ISequence<T>.values { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        bool ISequence<T>.is_finite { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public UnknownSequence(UnknownSequence<T> seq)
        {
            values = seq.values;
            name = seq.name;
            is_finite = seq.is_finite;
        }
    }*/

    /*public class UndefinedSequence<T>: ISequence<T>
    {
        public List<List<T>> values { get; set; } 
        public string name { get; set; }
        public bool is_infinite { get; set; }
        public int Count { get; set; }
        public bool is_undefined { get; set; }
        public List<int> is_undefined_concat { get; set; }

        public UndefinedSequence()
        {
            is_undefined = true;
        }
    }*/
    public class PointSequence : ISequence<Point> //secuencia de puntos
    {
        public List<List<Point>> values { get; set; }
        public string? name { get; set; }
        public bool is_infinite { get; set; } //las secuencias de puntos no son infinitas, si lo son, son undefined
        public bool is_undefined { get; set; }
        public int Count { get; set; }
        public List<int> is_undefined_concat { get; set; }

        public PointSequence(bool is_undefined)
        {
            this.is_undefined = is_undefined;
            values = new List<List<Point>>();
            is_undefined_concat = new List<int>();
        }
        public PointSequence(List<Point> points) //constructor para una secuencia
        {
            values = new List<List<Point>>() { points };
            Count = points.Count;
            is_undefined_concat = new List<int>();
        }
        public PointSequence(List<List<Point>> values) //constructor para la lista de secuencias, usado en concatenacion
        {
            this.values = values;
            Count = 0;
            foreach (var x in values)
            {
                foreach (var y in x) Count++;
            }
            is_undefined_concat = new List<int>();
        }
        public PointSequence(List<Point> points, string name)
        {
            this.name = name;
            values = new List<List<Point>>() { points };
            Count = points.Count;
            is_undefined_concat = new List<int>();
        }
        public PointSequence(string name, string color, bool is_finite) //cobstructor para una secuencia aleatoria
        {
            this.name = name;
            values = new List<List<Point>>();
            if (is_finite)
            {
                var l = new List<Point>();
                int count = 0;
                while (count != 10)
                {
                    l.Add(new Point("V" + count.ToString(), color));
                    count++;
                }
                values.Add(l);
                Count = l.Count;
            }
            else is_infinite = true;
            is_undefined_concat = new List<int>();
        }

        public PointSequence Concat(PointSequence s) //metodo que concatena la secuencia actual con la recibida y devuelve el resultado
        {
            List<List<Point>> concat_values = new List<List<Point>>();
            PointSequence concat_sequence = new PointSequence(concat_values);

            if (is_undefined) //si la primera secuencia a concatenar es indefinida, indefinir la concatenacion y devolver 
            {
                concat_sequence.is_undefined = true;
                return concat_sequence;
            }
            for (int i = 0; i < values.Count; i++) //por cada secuencia concatenada en la primera secuencia a concatenar
            {
                var l = new List<Point>();
                //itera en la secuenca e incerta los valores
                foreach (var x in values[i]) l.Add(x);
                //anade la secuencia a la familia de secuencias concatenadas
                concat_values.Add(l);
                //ir actualizando el largo de la secuencia
                concat_sequence.Count += l.Count;
                //si la secuencia actual esta concatenada con undefined, anadir a la lista 
                if (is_undefined_concat.Contains(i)) concat_sequence.is_undefined_concat.Add(i);
            }

            if (s.is_undefined) //si la segunda secuencia es indefinida, la secuencia resultado sera concatenada con undefined, devolver
            {
                concat_sequence.is_undefined_concat.Add(concat_values.Count - 1); //la secuencia general esta concatenada(la ultima secuencia concatenada esta concatenada con undefined)
                return concat_sequence;
            }
            for (int i = 0; i < s.values.Count; i++) //por cada secuencia concatenada en la segunda secuencia a concatenar
            {
                var l = new List<Point>();
                //itera en la secuencia e incerta los valores
                foreach (var x in s.values[i]) l.Add(x);
                //anade la secuencia a la familia de secuencias concatenadas
                concat_values.Add(l);
                //ir actualizando el largo d la secuencia
                concat_sequence.Count += l.Count;
                //si la secuencia actual esta concatenada con undefined, anadir a la lista 
                //si una secuencia esta sumada n veces con undefined, contara como que esta concatenada una sola vez
                if (s.is_undefined_concat.Contains(i)) concat_sequence.is_undefined_concat.Add(concat_values.Count - 1);
            }

            return concat_sequence;
        }

        public Node Return_Global_Var(Node node) //metodo que guarda en variables sus valores
        {
            //creo el nodo familia de variables
            Node var_fam = new Node { };
            int i = 0;
            int j = 0;
            //itero por las variables
            foreach (Node child in node.Children)
            {
                if (child.Value.ToString() == "_")
                {
                    if (j < values[i].Count || is_undefined_concat.Contains(i)) j++;
                    continue;
                }
                //su nombre
                string name = child.Value.ToString();
                //su valor en la sequencia
                dynamic var_value;

                if (is_undefined || j == values[i].Count) var_value = "undefined";

                //si no alcanzo secuencia para el valor de la variable asignarle una secuencia vacua
                else if (node.Children.Last() == child && i > values.Count() - 1)
                {
                    var_value = new PointSequence(false);
                    continue;
                }

                //si el ultimo valor termina siendo una sequencia
                else if (node.Children.Last() == child && (j != values[i].Count - 1 || i != values.Count() - 1))
                {
                    //creo la sequencia
                    var golbal_sequence = new List<List<Point>>();
                    PointSequence s = new PointSequence(golbal_sequence);
                    int index = 0;

                    //ir introduciendolos valores
                    while (i < values.Count())
                    {
                        var sequence = new List<Point>();
                        for (int k = j; k < values[i].Count(); k++)
                        {
                            sequence.Add(values[i][k]);
                        }
                        golbal_sequence.Add(sequence);
                        //si la secuencia actual esta concatenada con undefined, concatenar esta tambien
                        if (is_undefined_concat.Contains(i))
                        {
                            s.is_undefined_concat.Add(index);
                        }
                        i++;
                        index++;
                    }

                    //darle el valor de la sequencia
                    var_value = s;
                }

                else var_value = values[i][j];
                //lo anado al nodo de familia de variables
                var_fam.Children.Add(new Node { Type = name, Value = var_value });

                if (j == values[i].Count)
                {
                    j = 0;
                    i++;
                }
                else if (j < values[i].Count || is_undefined_concat.Contains(i)) j++;
                else
                {
                    j = 0;
                    i++;
                }
            }
            return var_fam;
        }
    }

    public class LineSequence : ISequence<Line> //secuencia de lineas
    {
        public List<List<Line>> values { get; set; }
        public string? name { get; set; }
        public bool is_infinite { get; set; }
        public bool is_undefined { get; set; }
        public int Count { get; set; }
        public List<int> is_undefined_concat { get; set; }

        public LineSequence(bool is_undefined)
        {
            this.is_undefined = is_undefined;
            values = new List<List<Line>>();
            is_undefined_concat = new List<int>();
        }
        public LineSequence(List<Line> values) //
        {
            this.values = new List<List<Line>>() { values };
            Count = values.Count;
            is_undefined_concat = new List<int>();
        }
        public LineSequence(List<List<Line>> values) //constructor para la lista de secuencias, usado en concatenacion
        {
            this.values = values;
            Count = 0;
            foreach (var x in values)
            {
                foreach (var y in x) Count++;
            }
            is_undefined_concat = new List<int>();
        }
        public LineSequence(List<Line> values, string name)
        {
            this.name = name;
            this.values = new List<List<Line>>() { values };
        }
        public LineSequence(string name, string color, bool is_finite) //
        {
            this.name = name;
            values = new List<List<Line>>();
            if (is_finite)
            {
                var l = new List<Line>();
                int count = 0;
                while (count != 10)
                {
                    l.Add(new Line("V" + count.ToString(), color));
                    count++;
                }
                values.Add(l);
                Count = l.Count;
            }
            else is_infinite = true;
            is_undefined_concat = new List<int>();
        }

        public LineSequence Concat(LineSequence s) //metodo que concatena la secuencia actual con la recibida y devuelve el resultado
        {
            List<List<Line>> concat_values = new List<List<Line>>();
            LineSequence concat_sequence = new LineSequence(concat_values);

            if (is_undefined) //si la primera secuencia a concatenar es indefinida, indefinir la concatenacion y devolver 
            {
                concat_sequence.is_undefined = true;
                return concat_sequence;
            }
            for (int i = 0; i < values.Count; i++) //por cada secuencia concatenada en la primera secuencia a concatenar
            {
                var l = new List<Line>();
                //itera en la secuenca e incerta los valores
                foreach (var x in values[i]) l.Add(x);
                //anade la secuencia a la familia de secuencias concatenadas
                concat_values.Add(l);
                //ir actualizando el largo de la secuencia
                concat_sequence.Count += l.Count;
                //si la secuencia actual esta concatenada con undefined, anadir a la lista 
                if (is_undefined_concat.Contains(i)) concat_sequence.is_undefined_concat.Add(i);
            }

            if (s.is_undefined) //si la segunda secuencia es indefinida, la secuencia resultado sera concatenada con undefined, devolver
            {
                concat_sequence.is_undefined_concat.Add(concat_values.Count - 1); //la secuencia general esta concatenada(la ultima secuencia concatenada esta concatenada con undefined)
                return concat_sequence;
            }
            for (int i = 0; i < s.values.Count; i++) //por cada secuencia concatenada en la segunda secuencia a concatenar
            {
                var l = new List<Line>();
                //itera en la secuencia e incerta los valores
                foreach (var x in s.values[i]) l.Add(x);
                //anade la secuencia a la familia de secuencias concatenadas
                concat_values.Add(l);
                //ir actualizando el largo d la secuencia
                concat_sequence.Count += l.Count;
                //si la secuencia actual esta concatenada con undefined, anadir a la lista 
                //si una secuencia esta sumada n veces con undefined, contara como que esta concatenada una sola vez
                if (s.is_undefined_concat.Contains(i)) concat_sequence.is_undefined_concat.Add(concat_values.Count - 1);
            }

            return concat_sequence;
        }

        public Node Return_Global_Var(Node node) //metodo que guarda en variables sus valores
        {
            //creo el nodo familia de variables
            Node var_fam = new Node { };
            int i = 0;
            int j = 0;
            //itero por las variables
            foreach (Node child in node.Children)
            {
                if (child.Value.ToString() == "_")
                {
                    if (j < values[i].Count || is_undefined_concat.Contains(i)) j++;
                    continue;
                }
                //su nombre
                string name = child.Value.ToString();
                //su valor en la sequencia
                dynamic var_value;

                if (is_undefined || j == values[i].Count) var_value = "undefined";

                //si no alcanzo secuencia para el valor de la variable asignarle una secuencia vacua
                else if (node.Children.Last() == child && i > values.Count() - 1)
                {
                    var_value = new LineSequence(false);
                    continue;
                }

                //si el ultimo valor termina siendo una sequencia
                else if (node.Children.Last() == child && (j != values[i].Count - 1 || i != values.Count() - 1))
                {
                    //creo la sequencia
                    var golbal_sequence = new List<List<Line>>();
                    LineSequence s = new LineSequence(golbal_sequence);
                    int index = 0;

                    //ir introduciendolos valores
                    while (i < values.Count())
                    {
                        var sequence = new List<Line>();
                        for (int k = j; k < values[i].Count(); k++)
                        {
                            sequence.Add(values[i][k]);
                        }
                        golbal_sequence.Add(sequence);
                        //si la secuencia actual esta concatenada con undefined, concatenar esta tambien
                        if (is_undefined_concat.Contains(i))
                        {
                            s.is_undefined_concat.Add(index);
                        }
                        i++;
                        index++;
                    }

                    //darle el valor de la sequencia
                    var_value = s;
                }

                else var_value = values[i][j];
                //lo anado al nodo de familia de variables
                var_fam.Children.Add(new Node { Type = name, Value = var_value });

                if (j == values[i].Count)
                {
                    j = 0;
                    i++;
                }
                else if (j < values[i].Count || is_undefined_concat.Contains(i)) j++;
                else
                {
                    j = 0;
                    i++;
                }
            }
            return var_fam;
        }
    }

    public class SegmentSequence : ISequence<Segment> //secuencia de segmentos
    {
        public List<List<Segment>> values { get; set; }
        public string? name { get; set; }
        public bool is_infinite { get; set; }
        public bool is_undefined { get; set; }
        public int Count { get; set; }
        public List<int> is_undefined_concat { get; set; }

        public SegmentSequence(bool is_undefined)
        {
            this.is_undefined = is_undefined;
            values = new List<List<Segment>>();
            is_undefined_concat = new List<int>();
        }
        public SegmentSequence(List<Segment> values) //
        {
            this.values = new List<List<Segment>>() { values };
            Count = values.Count;
            is_undefined_concat = new List<int>();
        }
        public SegmentSequence(List<List<Segment>> values) //constructor para la lista de secuencias, usado en concatenacion
        {
            this.values = values;
            Count = 0;
            foreach (var x in values)
            {
                foreach (var y in x) Count++;
            }
            is_undefined_concat = new List<int>();
        }
        public SegmentSequence(List<Segment> values, string name)
        {
            this.name = name;
            this.values = new List<List<Segment>>() { values };
            is_undefined_concat = new List<int>();
        }
        public SegmentSequence(string name, string color, bool is_finite) //
        {
            this.name = name;
            values = new List<List<Segment>>();
            if (is_finite)
            {
                var l = new List<Segment>();
                int count = 0;
                while (count != 10)
                {
                    l.Add(new Segment("V" + count.ToString(), color));
                    count++;
                }
                values.Add(l);
                Count = l.Count;
            }
            else is_infinite = true;
            is_undefined_concat = new List<int>();
        }

        public SegmentSequence Concat(SegmentSequence s) //metodo que concatena la secuencia actual con la recibida y devuelve el resultado
        {
            List<List<Segment>> concat_values = new List<List<Segment>>();
            SegmentSequence concat_sequence = new SegmentSequence(concat_values);

            if (is_undefined) //si la primera secuencia a concatenar es indefinida, indefinir la concatenacion y devolver 
            {
                concat_sequence.is_undefined = true;
                return concat_sequence;
            }
            for (int i = 0; i < values.Count; i++) //por cada secuencia concatenada en la primera secuencia a concatenar
            {
                var l = new List<Segment>();
                //itera en la secuenca e incerta los valores
                foreach (var x in values[i]) l.Add(x);
                //anade la secuencia a la familia de secuencias concatenadas
                concat_values.Add(l);
                //ir actualizando el largo de la secuencia
                concat_sequence.Count += l.Count;
                //si la secuencia actual esta concatenada con undefined, anadir a la lista 
                if (is_undefined_concat.Contains(i)) concat_sequence.is_undefined_concat.Add(i);
            }

            if (s.is_undefined) //si la segunda secuencia es indefinida, la secuencia resultado sera concatenada con undefined, devolver
            {
                concat_sequence.is_undefined_concat.Add(concat_values.Count - 1); //la secuencia general esta concatenada(la ultima secuencia concatenada esta concatenada con undefined)
                return concat_sequence;
            }
            for (int i = 0; i < s.values.Count; i++) //por cada secuencia concatenada en la segunda secuencia a concatenar
            {
                var l = new List<Segment>();
                //itera en la secuencia e incerta los valores
                foreach (var x in s.values[i]) l.Add(x);
                //anade la secuencia a la familia de secuencias concatenadas
                concat_values.Add(l);
                //ir actualizando el largo d la secuencia
                concat_sequence.Count += l.Count;
                //si la secuencia actual esta concatenada con undefined, anadir a la lista 
                //si una secuencia esta sumada n veces con undefined, contara como que esta concatenada una sola vez
                if (s.is_undefined_concat.Contains(i)) concat_sequence.is_undefined_concat.Add(concat_values.Count - 1);
            }

            return concat_sequence;
        }

        public Node Return_Global_Var(Node node) //metodo que guarda en variables sus valores
        {
            //creo el nodo familia de variables
            Node var_fam = new Node { };
            int i = 0;
            int j = 0;
            //itero por las variables
            foreach (Node child in node.Children)
            {
                if (child.Value.ToString() == "_")
                {
                    if (j < values[i].Count || is_undefined_concat.Contains(i)) j++;
                    continue;
                }
                //su nombre
                string name = child.Value.ToString();
                //su valor en la sequencia
                dynamic var_value;
                if (is_undefined || j == values[i].Count) var_value = "undefined";

                else if (node.Children.Last() == child && i > values.Count() - 1)
                {
                    var_value = new IntSequence(false);
                    continue;
                }

                //si el ultimo valor termina siendo una sequencia
                else if (node.Children.Last() == child && (j != values[i].Count - 1 || i != values.Count() - 1))
                {
                    //creo la sequencia
                    var golbal_sequence = new List<List<Segment>>();
                    SegmentSequence s = new SegmentSequence(golbal_sequence);
                    int index = 0;

                    //ir introduciendolos valores
                    while (i < values.Count())
                    {
                        var sequence = new List<Segment>();
                        for (int k = j; k < values[i].Count(); k++)
                        {
                            sequence.Add(values[i][k]);
                        }
                        golbal_sequence.Add(sequence);
                        //si la secuencia actual esta concatenada con undefined, concatenar esta tambien
                        if (is_undefined_concat.Contains(i))
                        {
                            s.is_undefined_concat.Add(index);
                        }
                        i++;
                        index++;
                    }

                    //darle el valor de la sequencia
                    var_value = s;
                }

                else var_value = values[i][j];
                //lo anado al nodo de familia de variables
                var_fam.Children.Add(new Node { Type = name, Value = var_value });
                if (j == values[i].Count)
                {
                    j = 0;
                    i++;
                }
                if (j < values[i].Count || is_undefined_concat.Contains(i)) j++;
                else
                {
                    j = 0;
                    i++;
                }
            }
            return var_fam;
        }
    }

    public class RaySequence : ISequence<Ray> //secuencia de rayos
    {
        public List<List<Ray>> values { get; set; }
        public string? name { get; set; }
        public bool is_infinite { get; set; }
        public bool is_undefined { get; set; }
        public int Count { get; set; }
        public List<int> is_undefined_concat { get; set; }

        public RaySequence(bool is_undefined)
        {
            this.is_undefined = is_undefined;
            values = new List<List<Ray>>();
            is_undefined_concat = new List<int>();
        }
        public RaySequence(List<Ray> values) //
        {
            this.values = new List<List<Ray>>() { values };
            Count = values.Count;
            is_undefined_concat = new List<int>();
        }
        public RaySequence(List<List<Ray>> values) //constructor para la lista de secuencias, usado en concatenacion
        {
            this.values = values;
            Count = 0;
            foreach (var x in values)
            {
                foreach (var y in x) Count++;
            }
            is_undefined_concat = new List<int>();
        }
        public RaySequence(List<Ray> values, string name)
        {
            this.name = name;
            this.values = new List<List<Ray>>() { values };
            is_undefined_concat = new List<int>();
        }
        public RaySequence(string name, string color, bool is_finite) //
        {
            this.name = name;
            values = new List<List<Ray>>();
            if (is_finite)
            {
                var l = new List<Ray>();
                int count = 0;
                while (count != 10)
                {
                    l.Add(new Ray("V" + count.ToString(), color));
                    count++;
                }
                values.Add(l);
                Count = l.Count;
            }
            else is_infinite = true;
            is_undefined_concat = new List<int>();
        }

        public RaySequence Concat(RaySequence s) //metodo que concatena la secuencia actual con la recibida y devuelve el resultado
        {
            List<List<Ray>> concat_values = new List<List<Ray>>();
            RaySequence concat_sequence = new RaySequence(concat_values);

            if (is_undefined) //si la primera secuencia a concatenar es indefinida, indefinir la concatenacion y devolver 
            {
                concat_sequence.is_undefined = true;
                return concat_sequence;
            }
            for (int i = 0; i < values.Count; i++) //por cada secuencia concatenada en la primera secuencia a concatenar
            {
                var l = new List<Ray>();
                //itera en la secuenca e incerta los valores
                foreach (var x in values[i]) l.Add(x);
                //anade la secuencia a la familia de secuencias concatenadas
                concat_values.Add(l);
                //ir actualizando el largo de la secuencia
                concat_sequence.Count += l.Count;
                //si la secuencia actual esta concatenada con undefined, anadir a la lista 
                if (is_undefined_concat.Contains(i)) concat_sequence.is_undefined_concat.Add(i);
            }

            if (s.is_undefined) //si la segunda secuencia es indefinida, la secuencia resultado sera concatenada con undefined, devolver
            {
                concat_sequence.is_undefined_concat.Add(concat_values.Count - 1); //la secuencia general esta concatenada(la ultima secuencia concatenada esta concatenada con undefined)
                return concat_sequence;
            }
            for (int i = 0; i < s.values.Count; i++) //por cada secuencia concatenada en la segunda secuencia a concatenar
            {
                var l = new List<Ray>();
                //itera en la secuencia e incerta los valores
                foreach (var x in s.values[i]) l.Add(x);
                //anade la secuencia a la familia de secuencias concatenadas
                concat_values.Add(l);
                //ir actualizando el largo d la secuencia
                concat_sequence.Count += l.Count;
                //si la secuencia actual esta concatenada con undefined, anadir a la lista 
                //si una secuencia esta sumada n veces con undefined, contara como que esta concatenada una sola vez
                if (s.is_undefined_concat.Contains(i)) concat_sequence.is_undefined_concat.Add(concat_values.Count - 1);
            }

            return concat_sequence;
        }

        public Node Return_Global_Var(Node node) //metodo que guarda en variables sus valores
        {
            //creo el nodo familia de variables
            Node var_fam = new Node { };
            int i = 0;
            int j = 0;
            //itero por las variables
            foreach (Node child in node.Children)
            {
                if (child.Value.ToString() == "_")
                {
                    if (j < values[i].Count || is_undefined_concat.Contains(i)) j++;
                    continue;
                }
                //su nombre
                string name = child.Value.ToString();
                //su valor en la sequencia
                dynamic var_value;
                if (is_undefined || j == values[i].Count) var_value = "undefined";

                else if (node.Children.Last() == child && i > values.Count() - 1)
                {
                    var_value = new IntSequence(false);
                    continue;
                }

                //si el ultimo valor termina siendo una sequencia
                else if (node.Children.Last() == child && (j != values[i].Count - 1 || i != values.Count() - 1))
                {
                    //creo la sequencia
                    var golbal_sequence = new List<List<Ray>>();
                    RaySequence s = new RaySequence(golbal_sequence);
                    int index = 0;

                    //ir introduciendolos valores
                    while (i < values.Count())
                    {
                        var sequence = new List<Ray>();
                        for (int k = j; k < values[i].Count(); k++)
                        {
                            sequence.Add(values[i][k]);
                        }
                        golbal_sequence.Add(sequence);
                        //si la secuencia actual esta concatenada con undefined, concatenar esta tambien
                        if (is_undefined_concat.Contains(i))
                        {
                            s.is_undefined_concat.Add(index);
                        }
                        i++;
                        index++;
                    }

                    //darle el valor de la sequencia
                    var_value = s;
                }

                else var_value = values[i][j];
                //lo anado al nodo de familia de variables
                var_fam.Children.Add(new Node { Type = name, Value = var_value });
                if (j == values[i].Count)
                {
                    j = 0;
                    i++;
                }
                if (j < values[i].Count || is_undefined_concat.Contains(i)) j++;
                else
                {
                    j = 0;
                    i++;
                }
            }
            return var_fam;
        }
    }

    public class CircleSequence : ISequence<Circle> //secuencia de circunferencias
    {
        public List<List<Circle>> values { get; set; }
        public string? name { get; set; }
        public bool is_infinite { get; set; }
        public bool is_undefined { get; set; }
        public int Count { get; set; }
        public List<int> is_undefined_concat { get; set; }

        public CircleSequence(bool is_undefined)
        {
            this.is_undefined = is_undefined;
            values = new List<List<Circle>>();
            is_undefined_concat = new List<int>();
        }
        public CircleSequence(List<Circle> values) //
        {
            this.values = new List<List<Circle>>() { values };
            Count = values.Count;
            is_undefined_concat = new List<int>();
        }
        public CircleSequence(List<List<Circle>> values) //constructor para la lista de secuencias, usado en concatenacion
        {
            this.values = values;
            Count = 0;
            foreach (var x in values)
            {
                foreach (var y in x) Count++;
            }
            is_undefined_concat = new List<int>();
        }
        public CircleSequence(List<Circle> values, string name)
        {
            this.name = name;
            this.values = new List<List<Circle>>() { values };
        }
        public CircleSequence(string name, string color, bool is_finite) //
        {
            this.name = name;
            values = new List<List<Circle>>();
            if (is_finite)
            {
                var l = new List<Circle>();
                int count = 0;
                while (count != 10)
                {
                    l.Add(new Circle("V" + count.ToString(), color));
                    count++;
                }
                values.Add(l);
                Count = l.Count;
            }
            else is_infinite = true;
            is_undefined_concat = new List<int>();
        }

        public CircleSequence Concat(CircleSequence s) //metodo que concatena la secuencia actual con la recibida y devuelve el resultado
        {
            List<List<Circle>> concat_values = new List<List<Circle>>();
            CircleSequence concat_sequence = new CircleSequence(concat_values);

            if (is_undefined) //si la primera secuencia a concatenar es indefinida, indefinir la concatenacion y devolver 
            {
                concat_sequence.is_undefined = true;
                return concat_sequence;
            }
            for (int i = 0; i < values.Count; i++) //por cada secuencia concatenada en la primera secuencia a concatenar
            {
                var l = new List<Circle>();
                //itera en la secuenca e incerta los valores
                foreach (var x in values[i]) l.Add(x);
                //anade la secuencia a la familia de secuencias concatenadas
                concat_values.Add(l);
                //ir actualizando el largo de la secuencia
                concat_sequence.Count += l.Count;
                //si la secuencia actual esta concatenada con undefined, anadir a la lista 
                if (is_undefined_concat.Contains(i)) concat_sequence.is_undefined_concat.Add(i);
            }

            if (s.is_undefined) //si la segunda secuencia es indefinida, la secuencia resultado sera concatenada con undefined, devolver
            {
                concat_sequence.is_undefined_concat.Add(concat_values.Count - 1); //la secuencia general esta concatenada(la ultima secuencia concatenada esta concatenada con undefined)
                return concat_sequence;
            }
            for (int i = 0; i < s.values.Count; i++) //por cada secuencia concatenada en la segunda secuencia a concatenar
            {
                var l = new List<Circle>();
                //itera en la secuencia e incerta los valores
                foreach (var x in s.values[i]) l.Add(x);
                //anade la secuencia a la familia de secuencias concatenadas
                concat_values.Add(l);
                //ir actualizando el largo d la secuencia
                concat_sequence.Count += l.Count;
                //si la secuencia actual esta concatenada con undefined, anadir a la lista 
                //si una secuencia esta sumada n veces con undefined, contara como que esta concatenada una sola vez
                if (s.is_undefined_concat.Contains(i)) concat_sequence.is_undefined_concat.Add(concat_values.Count - 1);
            }

            return concat_sequence;
        }

        public Node Return_Global_Var(Node node) //metodo que guarda en variables sus valores
        {
            //creo el nodo familia de variables
            Node var_fam = new Node { };
            int i = 0;
            int j = 0;
            //itero por las variables
            foreach (Node child in node.Children)
            {
                if (child.Value.ToString() == "_")
                {
                    if (j < values[i].Count || is_undefined_concat.Contains(i)) j++;
                    continue;
                }
                //su nombre
                string name = child.Value.ToString();
                //su valor en la sequencia
                dynamic var_value;
                if (is_undefined || j == values[i].Count) var_value = "undefined";

                else if (node.Children.Last() == child && i > values.Count() - 1)
                {
                    var_value = new IntSequence(false);
                    continue;
                }

                //si el ultimo valor termina siendo una sequencia
                else if (node.Children.Last() == child && (j != values[i].Count - 1 || i != values.Count() - 1))
                {
                    //creo la sequencia
                    var golbal_sequence = new List<List<Circle>>();
                    CircleSequence s = new CircleSequence(golbal_sequence);
                    int index = 0;

                    //ir introduciendolos valores
                    while (i < values.Count())
                    {
                        var sequence = new List<Circle>();
                        for (int k = j; k < values[i].Count(); k++)
                        {
                            sequence.Add(values[i][k]);
                        }
                        golbal_sequence.Add(sequence);
                        //si la secuencia actual esta concatenada con undefined, concatenar esta tambien
                        if (is_undefined_concat.Contains(i))
                        {
                            s.is_undefined_concat.Add(index);
                        }
                        i++;
                        index++;
                    }

                    //darle el valor de la sequencia
                    var_value = s;
                }

                else var_value = values[i][j];
                //lo anado al nodo de familia de variables
                var_fam.Children.Add(new Node { Type = name, Value = var_value });
                if (j == values[i].Count)
                {
                    j = 0;
                    i++;
                }
                if (j < values[i].Count || is_undefined_concat.Contains(i)) j++;
                else
                {
                    j = 0;
                    i++;
                }
            }
            return var_fam;
        }
    }

    public class ArcSequence : ISequence<Arc> //secuencia de arcos
    {
        public List<List<Arc>> values { get; set; }
        public string? name { get; set; }
        public bool is_infinite { get; set; }
        public bool is_undefined { get; set; }
        public int Count { get; set; }
        public List<int> is_undefined_concat { get; set; }

        public ArcSequence(bool is_undefined)
        {
            this.is_undefined = is_undefined;
            values = new List<List<Arc>>();
            is_undefined_concat = new List<int>();
        }
        public ArcSequence(List<Arc> values) //
        {
            this.values = new List<List<Arc>>() { values };
            Count = values.Count;
            is_undefined_concat = new List<int>();
        }
        public ArcSequence(List<List<Arc>> values) //constructor para la lista de secuencias, usado en concatenacion
        {
            this.values = values;
            Count = 0;
            foreach (var x in values)
            {
                foreach (var y in x) Count++;
            }
            is_undefined_concat = new List<int>();
        }
        public ArcSequence(List<Arc> values, string name)
        {
            this.name = name;
            this.values = new List<List<Arc>>() { values };
            is_undefined_concat = new List<int>();
        }
        public ArcSequence(string name, string color, bool is_finite) //
        {
            this.name = name;
            values = new List<List<Arc>>();
            if (is_finite)
            {
                var l = new List<Arc>();
                int count = 0;
                while (count != 10)
                {
                    l.Add(new Arc("V" + count.ToString(), color));
                    count++;
                }
                values.Add(l);
                Count = l.Count;
            }
            else is_infinite = true;
            is_undefined_concat = new List<int>();
        }

        public ArcSequence Concat(ArcSequence s) //metodo que concatena la secuencia actual con la recibida y devuelve el resultado
        {
            List<List<Arc>> concat_values = new List<List<Arc>>();
            ArcSequence concat_sequence = new ArcSequence(concat_values);

            if (is_undefined) //si la primera secuencia a concatenar es indefinida, indefinir la concatenacion y devolver 
            {
                concat_sequence.is_undefined = true;
                return concat_sequence;
            }
            for (int i = 0; i < values.Count; i++) //por cada secuencia concatenada en la primera secuencia a concatenar
            {
                var l = new List<Arc>();
                //itera en la secuenca e incerta los valores
                foreach (var x in values[i]) l.Add(x);
                //anade la secuencia a la familia de secuencias concatenadas
                concat_values.Add(l);
                //ir actualizando el largo de la secuencia
                concat_sequence.Count += l.Count;
                //si la secuencia actual esta concatenada con undefined, anadir a la lista 
                if (is_undefined_concat.Contains(i)) concat_sequence.is_undefined_concat.Add(i);
            }

            if (s.is_undefined) //si la segunda secuencia es indefinida, la secuencia resultado sera concatenada con undefined, devolver
            {
                concat_sequence.is_undefined_concat.Add(concat_values.Count - 1); //la secuencia general esta concatenada(la ultima secuencia concatenada esta concatenada con undefined)
                return concat_sequence;
            }
            for (int i = 0; i < s.values.Count; i++) //por cada secuencia concatenada en la segunda secuencia a concatenar
            {
                var l = new List<Arc>();
                //itera en la secuencia e incerta los valores
                foreach (var x in s.values[i]) l.Add(x);
                //anade la secuencia a la familia de secuencias concatenadas
                concat_values.Add(l);
                //ir actualizando el largo d la secuencia
                concat_sequence.Count += l.Count;
                //si la secuencia actual esta concatenada con undefined, anadir a la lista 
                //si una secuencia esta sumada n veces con undefined, contara como que esta concatenada una sola vez
                if (s.is_undefined_concat.Contains(i)) concat_sequence.is_undefined_concat.Add(concat_values.Count - 1);
            }

            return concat_sequence;
        }

        public Node Return_Global_Var(Node node) //metodo que guarda en variables sus valores
        {
            //creo el nodo familia de variables
            Node var_fam = new Node { };
            int i = 0;
            int j = 0;
            //itero por las variables
            foreach (Node child in node.Children)
            {
                if (child.Value.ToString() == "_")
                {
                    if (j < values[i].Count || is_undefined_concat.Contains(i)) j++;
                    continue;
                }
                //su nombre
                string name = child.Value.ToString();
                //su valor en la sequencia
                dynamic var_value;
                if (is_undefined || j == values[i].Count) var_value = "undefined";

                else if (node.Children.Last() == child && i > values.Count() - 1)
                {
                    var_value = new IntSequence(false);
                    continue;
                }

                //si el ultimo valor termina siendo una sequencia
                else if (node.Children.Last() == child && (j != values[i].Count - 1 || i != values.Count() - 1))
                {
                    //creo la sequencia
                    var golbal_sequence = new List<List<Arc>>();
                    ArcSequence s = new ArcSequence(golbal_sequence);
                    int index = 0;

                    //ir introduciendolos valores
                    while (i < values.Count())
                    {
                        var sequence = new List<Arc>();
                        for (int k = j; k < values[i].Count(); k++)
                        {
                            sequence.Add(values[i][k]);
                        }
                        golbal_sequence.Add(sequence);
                        //si la secuencia actual esta concatenada con undefined, concatenar esta tambien
                        if (is_undefined_concat.Contains(i))
                        {
                            s.is_undefined_concat.Add(index);
                        }
                        i++;
                        index++;
                    }

                    //darle el valor de la sequencia
                    var_value = s;
                }

                else var_value = values[i][j];
                //lo anado al nodo de familia de variables
                var_fam.Children.Add(new Node { Type = name, Value = var_value });
                if (j == values[i].Count)
                {
                    j = 0;
                    i++;
                }
                if (j < values[i].Count || is_undefined_concat.Contains(i)) j++;
                else
                {
                    j = 0;
                    i++;
                }
            }
            return var_fam;
        }
    }

    public class StringSequence : ISequence<string> //secuencia de string
    {
        public List<List<string>> values { get; set; }
        public string? name { get; set; }
        public bool is_infinite { get; set; } //las secuencias de string nunca son infinitas
        public bool is_undefined { get; set; }
        public int Count { get; set; }
        public List<int> is_undefined_concat { get; set; }

        public StringSequence(bool is_undefined)
        {
            this.is_undefined = is_undefined;
            values = new List<List<string>>();
            is_undefined_concat = new List<int>();
        }
        public StringSequence(List<string> values) //
        {
            this.values = new List<List<string>>() { values };
            Count = values.Count;
            is_undefined_concat = new List<int>();
        }
        public StringSequence(List<List<string>> values) //constructor para la lista de secuencias, usado en concatenacion
        {
            this.values = values;
            Count = 0;
            foreach (var x in values)
            {
                foreach (var y in x) Count++;
            }
            is_undefined_concat = new List<int>();
        }
        public StringSequence(List<string> values, string name)
        {
            this.name = name;
            this.values = new List<List<string>>() { values };
        }

        public StringSequence Concat(StringSequence s) //metodo que concatena la secuencia actual con la recibida y devuelve el resultado
        {
            List<List<string>> concat_values = new List<List<string>>();
            StringSequence concat_sequence = new StringSequence(concat_values);

            if (is_undefined) //si la primera secuencia a concatenar es indefinida, indefinir la concatenacion y devolver 
            {
                concat_sequence.is_undefined = true;
                return concat_sequence;
            }
            for (int i = 0; i < values.Count; i++) //por cada secuencia concatenada en la primera secuencia a concatenar
            {
                var l = new List<string>();
                //itera en la secuenca e incerta los valores
                foreach (var x in values[i]) l.Add(x);
                //anade la secuencia a la familia de secuencias concatenadas
                concat_values.Add(l);
                //ir actualizando el largo de la secuencia
                concat_sequence.Count += l.Count;
                //si la secuencia actual esta concatenada con undefined, anadir a la lista 
                if (is_undefined_concat.Contains(i)) concat_sequence.is_undefined_concat.Add(i);
            }

            if (s.is_undefined) //si la segunda secuencia es indefinida, la secuencia resultado sera concatenada con undefined, devolver
            {
                concat_sequence.is_undefined_concat.Add(concat_values.Count - 1); //la secuencia general esta concatenada(la ultima secuencia concatenada esta concatenada con undefined)
                return concat_sequence;
            }
            for (int i = 0; i < s.values.Count; i++) //por cada secuencia concatenada en la segunda secuencia a concatenar
            {
                var l = new List<string>();
                //itera en la secuencia e incerta los valores
                foreach (var x in s.values[i]) l.Add(x);
                //anade la secuencia a la familia de secuencias concatenadas
                concat_values.Add(l);
                //ir actualizando el largo d la secuencia
                concat_sequence.Count += l.Count;
                //si la secuencia actual esta concatenada con undefined, anadir a la lista 
                //si una secuencia esta sumada n veces con undefined, contara como que esta concatenada una sola vez
                if (s.is_undefined_concat.Contains(i)) concat_sequence.is_undefined_concat.Add(concat_values.Count - 1);
            }

            return concat_sequence;
        }

        public Node Return_Global_Var(Node node) //metodo que guarda en variables sus valores
        {
            //creo el nodo familia de variables
            Node var_fam = new Node { };
            int i = 0;
            int j = 0;
            //itero por las variables
            foreach (Node child in node.Children)
            {
                if (child.Value.ToString() == "_")
                {
                    if (j < values[i].Count || is_undefined_concat.Contains(i)) j++;
                    continue;
                }
                //su nombre
                string name = child.Value.ToString();
                //su valor en la sequencia
                dynamic var_value;
                if (is_undefined || j == values[i].Count) var_value = "undefined";

                else if (node.Children.Last() == child && i > values.Count() - 1)
                {
                    var_value = new IntSequence(false);
                    continue;
                }

                //si el ultimo valor termina siendo una sequencia
                else if (node.Children.Last() == child && (j != values[i].Count - 1 || i != values.Count() - 1))
                {
                    //creo la sequencia
                    var golbal_sequence = new List<List<string>>();
                    StringSequence s = new StringSequence(golbal_sequence);
                    int index = 0;

                    //ir introduciendolos valores
                    while (i < values.Count())
                    {
                        var sequence = new List<string>();
                        for (int k = j; k < values[i].Count(); k++)
                        {
                            sequence.Add(values[i][k]);
                        }
                        golbal_sequence.Add(sequence);
                        //si la secuencia actual esta concatenada con undefined, concatenar esta tambien
                        if (is_undefined_concat.Contains(i))
                        {
                            s.is_undefined_concat.Add(index);
                        }
                        i++;
                        index++;
                    }

                    //darle el valor de la sequencia
                    var_value = s;
                }

                else var_value = values[i][j];
                //lo anado al nodo de familia de variables
                var_fam.Children.Add(new Node { Type = name, Value = var_value });
                if (j == values[i].Count)
                {
                    j = 0;
                    i++;
                }
                if (j < values[i].Count || is_undefined_concat.Contains(i)) j++;
                else
                {
                    j = 0;
                    i++;
                }
            }
            return var_fam;
        }
    }

    public class IntSequence : ISequence<double> //secuencia de enteros
    {
        public List<List<double>> values { get; set; }
        public string? name { get; set; }
        public bool is_infinite { get; set; }
        public bool is_undefined { get; set; }
        public int Count { get; set; }
        public List<int> is_undefined_concat { get; set; }

        public IntSequence(bool is_undefined)
        {
            this.is_undefined = is_undefined;
            values = new List<List<double>>();
            is_undefined_concat = new List<int>();
        }
        public IntSequence(List<double> values) //
        {
            this.values = new List<List<double>>() { values };
            Count = values.Count;
            is_undefined_concat = new List<int>();
        }
        public IntSequence(List<List<double>> values) //constructor para la lista de secuencias, usado en concatenacion
        {
            this.values = values;
            Count = 0;
            foreach (var x in values)
            {
                foreach (var y in x) Count++;
            }
            is_undefined_concat = new List<int>();
        }
        public IntSequence(List<double> values, string name)
        {
            this.name = name;
            this.values = new List<List<double>>() { values };
        }
        public IntSequence(/*string name,*/ double inf, double sup)
        {
            var l = new List<double>();
            while (inf <= sup)
            {
                l.Add(inf);
                inf++;
            }
            values = new List<List<double>>() { l };
            Count = l.Count;
            is_undefined_concat = new List<int>();
        }
        public IntSequence(/*string name,*/ double inf)
        {
            //ir anadiendo a medida que se vayan pidiendo valores ya que es infinito*/
            this.is_infinite = true;
            values = new List<List<double>>() { new List<double>() { inf } };
            Count = 1;
            is_undefined_concat = new List<int>();
        }

        public IntSequence Concat(IntSequence s) //metodo que concatena la secuencia actual con la recibida y devuelve el resultado
        {
            List<List<double>> concat_values = new List<List<double>>();
            IntSequence concat_sequence = new IntSequence(concat_values);
            concat_sequence.Count = 0;

            if (is_undefined) //si la primera secuencia a concatenar es indefinida, indefinir la concatenacion y devolver 
            {
                concat_sequence.is_undefined = true;
                return concat_sequence;
            }
            for (int i = 0; i < values.Count; i++) //por cada secuencia concatenada en la primera secuencia a concatenar
            {
                var l = new List<double>();
                //itera en la secuenca e incerta los valores
                foreach (var x in values[i]) l.Add(x);
                //anade la secuencia a la familia de secuencias concatenadas
                concat_values.Add(l);
                //ir actualizando el largo de la secuencia
                concat_sequence.Count += l.Count;
                //si la secuencia actual esta concatenada con undefined, anadir a la lista 
                if (is_undefined_concat.Contains(i)) concat_sequence.is_undefined_concat.Add(i);
            }
            //si la parte de la secuencia que estamos analizando es infinita, detiene la concatenacion, ya que al ser infinita, nunca llega a anadir a la otra
            if (is_infinite)
            {
                concat_sequence.is_infinite = true;
                return concat_sequence;
            }


            if (s.is_undefined) //si la segunda secuencia es indefinida, la secuencia resultado sera concatenada con undefined, devolver
            {
                concat_sequence.is_undefined_concat.Add(concat_values.Count - 1); //la secuencia general esta concatenada(la ultima secuencia concatenada esta concatenada con undefined)
                return concat_sequence;
            }
            for (int i = 0; i < s.values.Count; i++) //por cada secuencia concatenada en la segunda secuencia a concatenar
            {
                var l = new List<double>();
                //itera en la secuencia e incerta los valores
                foreach (var x in s.values[i]) l.Add(x);
                //anade la secuencia a la familia de secuencias concatenadas
                concat_values.Add(l);
                //ir actualizando el largo d la secuencia
                concat_sequence.Count += l.Count;
                //si la secuencia actual esta concatenada con undefined, anadir a la lista 
                //si una secuencia esta sumada n veces con undefined, contara como que esta concatenada una sola vez
                if (s.is_undefined_concat.Contains(i)) concat_sequence.is_undefined_concat.Add(concat_values.Count - 1);
            }
            //si la parte de la secuencia que estamos analizando es infinita, incluir al resultado como secuencia infinita
            if (s.is_infinite)
            {
                concat_sequence.is_infinite = true;
            }

            return concat_sequence;
        }

        public void GrowSequence() //metodo para anadir uno mas a la secuencia infinita
        {
            int index = values.Count - 1;
            double i = values[index][values[index].Count - 1];
            values[index].Add(i + 1);
        }

        public Node Return_Global_Var(Node node) //metodo que guarda en variables sus valores
        {
            //creo el nodo familia de variables
            Node var_fam = new Node { };
            int i = 0;
            int j = 0;
            //itero por las variables
            foreach (Node child in node.Children)
            {
                if (child.Value.ToString() == "_")
                {
                    if (j < values[i].Count || is_undefined_concat.Contains(i)) j++;
                    continue;
                }
                //su nombre
                string name = child.Value.ToString();
                //su valor en la sequencia
                dynamic var_value = null;

                if (is_undefined || j == values[i].Count) var_value = "undefined";

                else if (node.Children.Last() == child && i > values.Count() - 1)
                {
                    var_value = new IntSequence(false);
                    continue;
                }

                //si el ultimo valor termina siendo una sequencia
                else if (node.Children.Last() == child && (j != values[i].Count - 1 || i != values.Count() - 1))
                {
                    //creo la sequencia
                    var golbal_sequence = new List<List<double>>();
                    IntSequence s = new IntSequence(golbal_sequence);
                    int index = 0;

                    //ir introduciendolos valores
                    while (i < values.Count())
                    {
                        var sequence = new List<double>();

                        //si la scuencia actual es infinita, guarda esa secuencia y para
                        if (is_infinite && i == values.Count() - 1)
                        {
                            index = values.Count() - 1;
                            golbal_sequence.Add(values[index]);
                            s.is_infinite = true;
                            break;
                        }

                        for (int k = j; k < values[i].Count(); k++)
                        {
                            sequence.Add(values[i][k]);
                        }

                        golbal_sequence.Add(sequence);
                        //si la secuencia actual esta concatenada con undefined, concatenar esta tambien
                        if (is_undefined_concat.Contains(i))
                        {
                            s.is_undefined_concat.Add(index);
                        }

                        i++;
                        index++;
                    }

                    //darle el valor de la sequencia
                    var_value = s;
                }

                //si la secuencia es infinita y es el ultimo hijo
                else if (node.Children.Last() == child && is_infinite && var_value is null)
                {
                    int index = values.Count - 1;
                    var s = new IntSequence(values[index][0]); //la secuencia infinita siempre la va a tener en el ultimo lugar, con el valor en el primero
                    s.is_infinite = true;
                    var_value = s;
                }

                else var_value = values[i][j];
                //lo anado al nodo de familia de variables
                var_fam.Children.Add(new Node { Type = name, Value = var_value });

                if (j == values[i].Count)
                {
                    j = 0;
                    i++;
                }

                //si ya llegue al final de la secuencia y la secuencia debe ser infinita, agrandar la secuencia y seguir
                if (i == values.Count - 1 && j == values[i].Count - 1 && is_infinite)
                {
                    GrowSequence();
                    j++;
                }

                else if (j < values[i].Count || is_undefined_concat.Contains(i)) j++;

                else
                {
                    j = 0;
                    i++;
                }
            }
            return var_fam;
        }
    }

    public class FloatSequence : ISequence<float> //secuencia de doubles
    {
        public List<List<float>> values { get; set; }
        public string? name { get; set; }
        public bool is_infinite { get; set; }
        public bool is_undefined { get; set; }
        public int Count { get; set; }
        public List<int> is_undefined_concat { get; set; }

        public FloatSequence(bool is_undefined)
        {
            this.is_undefined = is_undefined;
            values = new List<List<float>>();
            is_undefined_concat = new List<int>();
        }
        public FloatSequence(List<float> values) //
        {
            this.values = new List<List<float>>() { values };
            Count = values.Count;
            is_undefined_concat = new List<int>();
        }
        public FloatSequence(List<List<float>> values) //constructor para la lista de secuencias, usado en concatenacion
        {
            this.values = values;
            Count = 0;
            foreach (var x in values)
            {
                foreach (var y in x) Count++;
            }
            is_undefined_concat = new List<int>();
        }
        public FloatSequence(List<float> values, string name)
        {
            this.name = name;
            this.values = new List<List<float>>() { values };
        }
        public FloatSequence() // 
        {
            //this.name = name;
            var l = new List<float>();
            int count = 0;
            while (count < 10)
            {
                l.Add(new Random().Next() % 1);
                count++;
            }
            values = new List<List<float>>() { l };
            Count = l.Count;
            is_undefined_concat = new List<int>();
        }

        public FloatSequence Concat(FloatSequence s) //metodo que concatena la secuencia actual con la recibida y devuelve el resultado
        {
            List<List<float>> concat_values = new List<List<float>>();
            FloatSequence concat_sequence = new FloatSequence(concat_values);

            if (is_undefined) //si la primera secuencia a concatenar es indefinida, indefinir la concatenacion y devolver 
            {
                concat_sequence.is_undefined = true;
                return concat_sequence;
            }
            for (int i = 0; i < values.Count; i++) //por cada secuencia concatenada en la primera secuencia a concatenar
            {
                var l = new List<float>();
                //itera en la secuenca e incerta los valores
                foreach (var x in values[i]) l.Add(x);
                //anade la secuencia a la familia de secuencias concatenadas
                concat_values.Add(l);
                //ir actualizando el largo de la secuencia
                concat_sequence.Count += l.Count;
                //si la secuencia actual esta concatenada con undefined, anadir a la lista 
                if (is_undefined_concat.Contains(i)) concat_sequence.is_undefined_concat.Add(i);
            }

            if (s.is_undefined) //si la segunda secuencia es indefinida, la secuencia resultado sera concatenada con undefined, devolver
            {
                concat_sequence.is_undefined_concat.Add(concat_values.Count - 1); //la secuencia general esta concatenada(la ultima secuencia concatenada esta concatenada con undefined)
                return concat_sequence;
            }
            for (int i = 0; i < s.values.Count; i++) //por cada secuencia concatenada en la segunda secuencia a concatenar
            {
                var l = new List<float>();
                //itera en la secuencia e incerta los valores
                foreach (var x in s.values[i]) l.Add(x);
                //anade la secuencia a la familia de secuencias concatenadas
                concat_values.Add(l);
                //ir actualizando el largo d la secuencia
                concat_sequence.Count += l.Count;
                //si la secuencia actual esta concatenada con undefined, anadir a la lista 
                //si una secuencia esta sumada n veces con undefined, contara como que esta concatenada una sola vez
                if (s.is_undefined_concat.Contains(i)) concat_sequence.is_undefined_concat.Add(concat_values.Count - 1);
            }

            return concat_sequence;
        }

        public Node Return_Global_Var(Node node) //metodo que guarda en variables sus valores
        {
            //creo el nodo familia de variables
            Node var_fam = new Node { };
            int i = 0;
            int j = 0;
            //itero por las variables
            foreach (Node child in node.Children)
            {
                if (child.Value.ToString() == "_")
                {
                    if (j < values[i].Count || is_undefined_concat.Contains(i)) j++;
                    continue;
                }
                //su nombre
                string name = child.Value.ToString();
                //su valor en la sequencia
                dynamic var_value;
                if (is_undefined || j == values[i].Count) var_value = "undefined";

                else if (node.Children.Last() == child && i > values.Count() - 1)
                {
                    var_value = new IntSequence(false);
                    continue;
                }

                //si el ultimo valor termina siendo una sequencia
                else if (node.Children.Last() == child && (j != values[i].Count - 1 || i != values.Count() - 1))
                {
                    //creo la sequencia
                    var golbal_sequence = new List<List<float>>();
                    FloatSequence s = new FloatSequence(golbal_sequence);
                    int index = 0;

                    //ir introduciendolos valores
                    while (i < values.Count())
                    {
                        var sequence = new List<float>();
                        for (int k = j; k < values[i].Count(); k++)
                        {
                            sequence.Add(values[i][k]);
                        }
                        golbal_sequence.Add(sequence);
                        //si la secuencia actual esta concatenada con undefined, concatenar esta tambien
                        if (is_undefined_concat.Contains(i))
                        {
                            s.is_undefined_concat.Add(index);
                        }
                        i++;
                        index++;
                    }

                    //darle el valor de la sequencia
                    var_value = s;
                }

                else var_value = values[i][j];
                //lo anado al nodo de familia de variables
                var_fam.Children.Add(new Node { Type = name, Value = var_value });
                if (j == values[i].Count)
                {
                    j = 0;
                    i++;
                }
                if (j < values[i].Count || is_undefined_concat.Contains(i)) j++;
                else
                {
                    j = 0;
                    i++;
                }
            }
            return var_fam;
        }
    }
}