using Godot;
using System;
using INTERPRETE_C__to_HULK;
using G_Wall_E;
using System.Collections.Generic;

public partial class Drawing_Area : Panel
{
	//en este nodo es donde se va a efectuar el dibujado de las primitivas
	//DATO: el plano XY esta comprendido por x E (0,525) y E (0,600) donde XY es el plano en donde se efectua el dibujado
	//NOTA: revisar el caso que pidan dibujar con nombre al lado

	//numero que me define cuantas primitivas voy a dibujar de cada tipo
	//NOTA: habra que cambiarlo despues pero la idea es esa
	//private int cant = 0;

	//tipo de primitiva que debo dibujar
	//NOTA:tambien debera ser cambiado
	//public PrimitiveType primitiveType = PrimitiveType.None;

	public List<DrawableProperties> figures;

	public bool draw = false;

	//enum que me define los tipos de primitivas que puedo dibujar 
	/*public enum PrimitiveType
	{
		Point,
		Circle,
		Line,
		Segment,
		Ray,
		Arc,
		None,
	}*/


	//metodo draw definido que itera por cada vez que debe dibujar una figura
	//NOTA: habra que modificar, la idea es esa
	public override void _Draw()
	{
		//voy a dibujar en el plano por orden de dependencia, ya que en godot con cuelqier orden se ve feo, o no se ve XD
		var font = GetThemeFont("font");
		string text;

		//primero las circunferencias
		foreach (DrawableProperties f in figures)
		{
			if (f.Type == "circle")
			{
				DrawCircle(new Vector2((float)f.P1.X, (float)f.P1.Y), (float)f.Radius, Paint(f.Color));
				text = f.Msg;
				if (text is not null) DrawString(font, new Vector2((float)f.P1.X, (float)f.P1.Y), text, HorizontalAlignment.Left, 200, 200, Colors.White);
			}
		}

		float m1;
		float m2;
		float angle_1;
		float angle_2;
		//arcos
		foreach (DrawableProperties f in figures)
		{
			if (f.Type == "arc")
			{
				//hallar pendiendtes de las recas
				m1 = ((float)f.P2.Y - (float)f.P1.Y) / ((float)f.P1.X - (float)f.P1.X);
				m2 = ((float)f.P3.Y - (float)f.P1.Y) / ((float)f.P3.X - (float)f.P1.X);
				//hallar angulos con resepecto al eje x
				angle_1 = (float)Math.Pow(Math.Tan(m1), -1);
				angle_2 = (float)Math.Pow(Math.Tan(m2), -1);

				if (m1 < 0) angle_1 += (float)Math.PI;
				if (m2 < 0) angle_2 += (float)Math.PI;

				DrawArc(new Vector2((float)f.P1.X, (float)f.P1.Y), (float)f.Radius, angle_1, angle_2, 200, Paint(f.Color));
				text = f.Msg;
				if (text is not null) DrawString(font, new Vector2((float)f.P1.X, (float)f.P1.Y), text, HorizontalAlignment.Left, 200, 200, Colors.White);
			}
		}

		double x3;
		double y3;
		double x4;
		double y4;

		//lineas 
		foreach (DrawableProperties f in figures)
		{
			if (f.Type == "line")
			{
				x4 = (float)f.P2.X;
				y4 = (float)f.P2.Y;
				if (f.P1.Y == f.P2.Y)
				{
					x3 = 0;
					y3 = (double)f.P1.Y;
					x4 = 525;
					y4 = (double)f.P2.Y;
				}
				if (f.P1.X == f.P2.X)
				{
					x3 = (double)f.P1.X;
					y3 = 0;
					x4 = (double)f.P2.X;
					y4 = 600;
				}
				else
				{
					double m = (double)(f.P2.Y - f.P1.Y) / (double)(f.P2.X - f.P1.X);
					double n = (double)f.P1.Y - m * (double)f.P1.X;
					x3 = 0;
					y3 = n;
					int extreme0 = Limit_X((int)m, (int)n, 525);
					int extremef = Limit_Y((int)m, (int)n, 600);
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
				DrawLine(new Vector2((float)x3, (float)y3), new Vector2((float)x4, (float)y4), Paint(f.Color));
				text = f.Msg;
				if (text is not null) DrawString(font, new Vector2((float)f.P1.X, (float)f.P1.Y), text, HorizontalAlignment.Left, 200, 200, Colors.White);
			}
		}

		//segmentos 
		foreach (DrawableProperties f in figures)
		{
			if (f.Type == "segment")
			{
				DrawLine(new Vector2((float)f.P1.X, (float)f.P1.Y), new Vector2((float)f.P2.X, (float)f.P2.Y), Paint(f.Color));
				text = f.Msg;
				if (text is not null) DrawString(font, new Vector2((float)f.P1.X, (float)f.P1.Y), text, HorizontalAlignment.Left, 200, 200, Colors.White);
			}
		}

		//rayos
		foreach (DrawableProperties f in figures)
		{
			if (f.Type == "ray")
			{
				x3 = (double)f.P1.X;
				y3 = (double)f.P1.Y;

				if (f.P1.Y == f.P2.Y)
				{
					x3 = 525;
					y3 = (double)f.P2.Y;
				}
				if (f.P1.X == f.P2.X)
				{
					x3 = (double)f.P2.X;
					y3 = 600;
				}
				else
				{
					double m = (double)(f.P2.Y - f.P1.Y) / (double)(f.P2.X - f.P1.X);
					double n = (double)f.P1.Y - m * (double)f.P1.X;
					if (f.P2.X > f.P1.X)
					{
						double extreme0 = Limit_X((int)m, (int)n, 525);
						double extremef = Limit_Y((int)m, (int)n, 600);
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
					if (f.P2.X < f.P1.X)
					{
						x3 = 0;
						y3 = n;
					}
				}
				DrawLine(new Vector2((float)f.P1.X, (float)f.P1.Y), new Vector2((float)x3, (float)y3), Colors.Green);
				text = f.Msg;
				if (text is not null) DrawString(font, new Vector2((float)f.P1.X, (float)f.P1.Y), text, HorizontalAlignment.Left, 200, 200, Colors.White);
			}
		}

		//puntos
		foreach (DrawableProperties f in figures)
		{
			if (f.Type == "point")
			{
				DrawCircle(new Vector2((float)f.X, (float)f.Y), 5, Paint(f.Color));
				text = f.Msg;
				if (text is not null) DrawString(font, new Vector2((float)f.P1.X, (float)f.P1.Y), text, HorizontalAlignment.Left, 200, 200, Colors.White);
			}
		}

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
	public void Changed(List<DrawableProperties> drawables)
	{
		figures = drawables;
		draw = true;
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
		//GetNode<Button>("/root/Scene/Fondo/Interact_Area/Confirm_Button").Connect("pressed", Callable.From(Changed));
		_Draw();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (draw)
		{
			QueueRedraw();
			draw = false;
		}
	}
}
