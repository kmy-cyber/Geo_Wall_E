using Godot;
using System;
using INTERPRETE_C__to_HULK;
using G_Wall_E;
using System.Collections.Generic;

public partial class scene : Control
{
	private AudioStreamPlayer export_audio;
	private AudioStreamPlayer confirm_audio;
	private AudioStreamPlayer error_not_confirmed_code;
	private AudioStreamPlayer error_there_int_export;
	private CodeEdit console;
	private RichTextLabel terminal;
	private Drawing_Area drawing_area;
	private int count;
	public bool Is_Confirmed;
	public string last_text_console = "";
	public string code;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//mandando las senales de boton presionado a la orden
		GetNode<Button>("/root/Scene/Fondo/Interact_Area/Confirm_Button").Connect("pressed", Callable.From(Confirm_Button_Pressed));
		GetNode<Button>("/root/Scene/Fondo/Interact_Area/Export_Code_Button").Connect("pressed", Callable.From(Export_Button_Pressed));

		//extraigo:
		//la cantidad de txt que existen en la carpeta de bibliotecas para asi nombrarlas con el contador y no sobreescribirlas
		count = System.IO.Directory.GetFiles("Saved_Code").Length;

		//el audio de la carpeta music
		export_audio = GetNode<AudioStreamPlayer>("Export_Audio");
		confirm_audio = GetNode<AudioStreamPlayer>("Confirm_Audio");
		error_not_confirmed_code = GetNode<AudioStreamPlayer>("Error_Not_Confirmed_Code");
		error_there_int_export = GetNode<AudioStreamPlayer>("Error_There_INT_Export");

		//el nodo de la consola donde el usuario mete codigo
		console = GetNode<CodeEdit>("/root/Scene/Fondo/Interact_Area/Console");

		//el nodo de la terminal 
		terminal = GetNode<RichTextLabel>("/root/Scene/Fondo/Interact_Area/Terminal");

		drawing_area = GetNode<Drawing_Area>("/root/Scene/Fondo/Drawing_Area");

		//el boton confirm aun no ha sido presionado
		Is_Confirmed = false;

	}

	public void Confirm_Button_Pressed()
	{
		//reproduzco la musica
		error_not_confirmed_code.Stop();
		error_there_int_export.Stop();
		export_audio.Stop();
		confirm_audio.Play();

		Is_Confirmed = true;
		last_text_console = console.Text;

		//limpio la terminal
		terminal.Clear();
		//guardo en un string el codigo
		code = console.Text;

		//drawing_area.Changed();
		//empieza a analizarse el interprete
		if (!string.IsNullOrEmpty(code))
		{
			try
			{
				Semantic_Analyzer sa = new Semantic_Analyzer();

				// Lexer recibe el input (s) y crea la lista de tokens
				Lexer T = new Lexer(code);

				// Se obtiene la lista de tokens que hace el Lexer
				List<Token> TS = T.Tokens_sequency;

				// Parser recibe la lista de tokens (TS) y crea el arbol de sintaxis
				Parser P = new Parser(TS);

				// Se obtiene el arbol (N)
				INTERPRETE_C__to_HULK.Node N = P.Parse();

				// El metodo Read_Parser del Analizador Semantico y recibe el arbol 
				sa.Read_Parser(N);

				// Recibe el arbol, lo analiza y devuelve el resultado
				List<IDrawable> drawables = sa.Choice(N);
				var drawables_2 = new List<DrawableProperties>();

				foreach (var x in drawables)
				{
					drawables_2.Add(x.Export());
				}

				drawing_area.Changed(drawables_2);
			}
			catch (Exception ex)
			{
				terminal.Clear();
				terminal.AddText(ex.Message);
			}
		}
		//drawing_area.primitiveType = Drawing_Area.PrimitiveType.Point;
		//drawing_area._Draw();
	}

	//lo que sucede si el boton export es presionado
	public void Export_Button_Pressed()
	{
		//en caso de que el usuario no haya analizado el codigo primero, lanzar un error en la terminal
		if (!Is_Confirmed)
		{
			confirm_audio.Stop();
			export_audio.Stop();
			error_there_int_export.Stop();
			error_not_confirmed_code.Play();
			string mensaje = "You can't export your code before confirm it";
			terminal.Clear();
			terminal.AddText(mensaje);
		}
		//si no, guardar el codigo en un txt de la carpeta saved code
		else
		{
			if (!string.IsNullOrEmpty(code))
			{
				//reproduzco la musica
				confirm_audio.Stop();
				error_not_confirmed_code.Stop();
				error_there_int_export.Stop();
				export_audio.Play();
				string ruta = "Saved_Code/saved_code_" + count.ToString() + ".txt";
				System.IO.File.WriteAllText(ruta, code);
				count++;
			}
			else
			{
				//reproduzco
				confirm_audio.Stop();
				export_audio.Stop();
				error_not_confirmed_code.Stop();
				error_there_int_export.Play();
				string mensaje = "There is nothing to export";
				terminal.Clear();
				terminal.AddText(mensaje);
			}
		}
		//por el momento guardare el codigo directamente ya que no tengo todavia acceso al analizador
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (console.Text != last_text_console)
		{
			Is_Confirmed = false;
		}
	}
}
