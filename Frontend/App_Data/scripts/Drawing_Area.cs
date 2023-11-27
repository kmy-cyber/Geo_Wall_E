using Godot;
using System;
using System.Dynamic;
using System.Security.Cryptography.X509Certificates;

public class Drawing_Area : Panel
{

    //en este nodo es donde se va a efectuar el dibujado de las primitivas
    //DATO: el plano XY esta comprendido por x E (0,525) y E (0,600) donde XY es el plano en donde se efectua el dibujado
    //NOTA: revisar el caso que pidan dibujar con nombre al lado

    //numero que me define cuantas primitivas voy a dibujar de cada tipo
    //NOTA: habra que cambiarlo despues pero la idea es esa
    private int cant = 0;

    //tipo de primitiva que debo dibujar
    //NOTA:tambien debera ser cambiado
    private PrimitiveType primitiveType = PrimitiveType.None;

    //enum que me define los tipos de primitivas que puedo dibujar 
    public enum PrimitiveType
    {
        Point,
        Circle,
        Line,
        Segment,
        Ray,
        Arc,
        None,
    }


    //metodo draw definido que itera por cada vez que debe dibujar una figura
    //NOTA: habra que modificar, la idea es esa
    public override void _Draw()
    {
        //var text = "circunferencia c1";
        //var font = GetFont("font");
        for (int i = 0; i < cant; i++)
        {
            if (primitiveType == PrimitiveType.Point) Draw_Point(i);
            if (primitiveType == PrimitiveType.Circle) Draw_Circle(i);
            if (primitiveType == PrimitiveType.Line) Draw_Line(i);
            if (primitiveType == PrimitiveType.Segment) Draw_Segment(i);
            if(primitiveType == PrimitiveType.Ray) Draw_Ray(i);
            if (primitiveType == PrimitiveType.Arc) Draw_Arc(i);
            //DrawString(font, new Vector2(50 * i, 50 * i), text, Colors.White, 500);
        }
    }

    //lista de metodos que dibujam cada figura
    //NOTA: habra que modificar en dependencia de como recibo los datos de dibujado
    private void Draw_Point(int index)
    {
        DrawCircle(new Vector2(100 * index, 100 * index), 5, Colors.Red);

    }

    private void Draw_Circle(int index)
    {
        DrawCircle(new Vector2(100 * index, 100 * index), 100, Colors.Blue);
    }

    private void Draw_Segment(int index)
    {
        DrawLine(new Vector2(100 * index, 100), new Vector2(200, 200 * index), Colors.Green);
    }

    public void Draw_Line(int index)
    {
        //preguntar a camila si analiza el error de que -en una linea segmento y rayo reciban dos puntos iguales u en ese caso retornar eeror ya que no debe ser asi
        int x1 = 100;
        int y1 = 100;
        int x2 = 200;
        int y2 = 200;
        int x3 = x1;
        int y3 = y1;
        int x4 = x2;
        int y4 = y2;
        if (y1 == y2)
        {
            x3 = 0;
            y3 = y1;
            x4 = 525;
            y4 = y2;
        }
        if (x1 == x2)
        {
            x3 = x1;
            y3 = 0;
            x4 = x2;
            y4 = 600;
        }
        else
        {
            int m = (y2 - y1) / (x2 - x1);
            int n = y1 - m * x1;
            x3 = 0;
            y3 = n;
            int extreme0 = Limit_X(m, n, 525);
            int extremef = Limit_Y(m, n, 600);
            if (extreme0 < 600)
            {
                x4 = 525;
                y4 = extreme0;
            }
            else if (extremef < 525)
            {
                x4 = extremef;
                y4 = 600;
            }
        }
        DrawLine(new Vector2(x3, y3), new Vector2(x4, y4), Colors.Green);
    }

    private void Draw_Ray(int index)
    {
        int x1 = 100;
        int y1 = 100;
        int x2 = 200;
        int y2 = 200;
        int x3 = x1;
        int y3 = y1;
        if (y1 == y2)
        {
            x3 = 525;
            y3 = y2;
        }
        if (x1 == x2)
        {
            x3 = x2;
            y3 = 600;
        }
        else
        {
            int m = (y2 - y1) / (x2 - x1);
            int n = y1 - m * x1;
            if (x2 > x1)
            {
                int extreme0 = Limit_X(m, n, 525);
                int extremef = Limit_Y(m, n, 600);
                if (extreme0 < 600)
                {
                    x3 = 525;
                    y3 = extreme0;
                }
                else if (extremef < 525)
                {
                    x3 = extremef;
                    y3 = 600;
                }
            }
            if (x2 < x1)
            {
                x3 = 0;
                y3 = n;
            }
        }
        DrawLine(new Vector2(x1, y1), new Vector2(x3, y3), Colors.Green);
    }

    private void Draw_Arc(int index)
    {
        DrawArc(new Vector2(100 * index, 100), 300, (float)0.78, (float)1.57, 200 * index, Colors.White);
    }

    //metodos para hallar limites
    int Limit_X(int m, int n, int x)
    {
        return m * x + n;
    }
    int Limit_Y(int m, int n, int y)
    {
        return (y - n) / m;
    }

    //metodo que recibe la orden de dibujar
    //NOTA: cambiar tambien(no mucho como los otros)
    public void Changed()
    {
        cant = 9;
        primitiveType = PrimitiveType.Segment;
        Update();
    }

    //metodo para asignar colores cuando vengan del input del interprete
    public Color Paint(string color)
    {
        switch (color)
        {
            case "blue":
                return Colors.Blue;

            case "red":
                return Colors.Red;

            case "yellow":
                return Colors.Yellow;

            case "green":
                return Colors.Green;

            case "cyan":
                return Colors.Cyan;

            case "magenta":
                return Colors.Magenta;

            case "gray":
                return Colors.Gray;

            case "black":
                return Colors.Black;
        }
        return Colors.White;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //conecto la senal de boton presionado a la orden de dibujar
        //NOTA: esto obviamente hay que cambiarlo a cuando reciba los datos del interprete y dibujar(cambiar la senal)
        GetNode<Button>("/root/Scene/Fondo/Interact_Area/Confirm_Button").Connect("pressed", this, nameof(Changed));
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
