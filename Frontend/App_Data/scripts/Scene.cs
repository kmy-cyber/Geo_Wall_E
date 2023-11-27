using Godot;
using System;
using System.IO;

public class Scene : Control
{
    private AudioStreamPlayer export_audio;
    private AudioStreamPlayer confirm_audio;
    private AudioStreamPlayer error_not_confirmed_code;
    private AudioStreamPlayer error_there_int_export;
    private TextEdit console;
    private RichTextLabel terminal;
    private int count;
    public bool Is_Confirmed;
    public string code;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {   
        //mandando las senales de boton presionado a la orden
        GetNode<Button>("/root/Scene/Fondo/Interact_Area/Confirm_Button").Connect("pressed", this, nameof(Confirm_Button_Pressed));
        GetNode<Button>("/root/Scene/Fondo/Interact_Area/Export_Code_Button").Connect("pressed", this, nameof(Export_Button_Pressed));

        //extraigo:
        //la cantidad de txt que existen en la carpeta de bibliotecas para asi nombrarlas con el contador y no sobreescribirlas
        count = System.IO.Directory.GetFiles("Saved_Code").Length;

        //el audio de la carpeta music
        export_audio = GetNode<AudioStreamPlayer>("Export_Audio");
        confirm_audio = GetNode<AudioStreamPlayer>("Confirm_Audio");
        error_not_confirmed_code = GetNode<AudioStreamPlayer>("Error_Not_Confirmed_Code");
        error_there_int_export = GetNode<AudioStreamPlayer>("Error_There_INT_Export");

        //el nodo de la consola donde el usuario mete codigo
        console = GetNode<TextEdit>("/root/Scene/Fondo/Interact_Area/Console");

        //el nodo de la terminal 
        terminal = GetNode<RichTextLabel>("/root/Scene/Fondo/Interact_Area/Terminal");

        //el boton confirm aun no ha sido presionado
        Is_Confirmed = false;
    }

    //lo que sucede si el boton draw es presionado
    public void Confirm_Button_Pressed()
    {
        //reproduzco la musica
        error_not_confirmed_code.Stop();
        error_there_int_export.Stop();
        export_audio.Stop();
        confirm_audio.Play();

        Is_Confirmed = true;

        //limpio la terminal
        terminal.Clear();

        //guardo en un string el codigo
        code = console.Text;
        //empieza a analizarse el interprete
        //...
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
            terminal.BbcodeText = mensaje;
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
                terminal.BbcodeText = mensaje;
            }
        }
        //por el momento guardare el codigo directamente ya que no tengo todavia acceso al analizador
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (code != console.Text) Is_Confirmed = false;
    }
}
