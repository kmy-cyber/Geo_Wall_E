using System.Collections.Generic;
using INTERPRETE_C_to_HULK;

namespace INTERPRETE_C__to_HULK
{
    public class Program
    {

        public static void Main() 
        {


            // Inicializa el diccionario de variables_globales
            Semantic_Analyzer<double> sa = new Semantic_Analyzer<double>();
//
            //List<string> input = new List<string>{
            ////    "function fib(n) => if (n > 1) fib(n-1) + fib(n-2) else 1;",
            ////    "print(fib(5));",
            //   "function myfib(n) => if (n == 0 | n == 1) 1 else myfib(n-1) + myfib(n-2);",
            //   "print(myfib(5));"
            //};
//
            //foreach(string s in input)
            //{
//
            //   Console.Write("> ");
            //   //try- catch en caso de que lance una excepcion, que lo imprima y siga funcionando
            //   try
            //   {
            //       // Lexer recibe el input (s) y crea la lista de tokens
            //       Lexer T =  new Lexer(s);
            //       // Se obtiene la lista de tokens que hace el Lexer
            //       List<Token> TS = T.Tokens_sequency;
            //       // Parser recibe la lista de tokens (TS) y crea el arbol de sintaxis
            //       Parser P = new Parser(TS);
            //       // Se obtiene el arbol (N)
            //       Node N = P.Parse();
            //       // El metodo Read_Parser del Analizador Semantico y recibe el arbol 
            //       sa.Read_Parser(N);
            //       
            //       Console.ForegroundColor = ConsoleColor.DarkBlue;
            //       // Recibe el arbol, lo analiza y devuelve el resultado
            //       sa.Choice (N);   
            //   }
            //   catch (System.Exception ex)
            //   {
            //       Console.ForegroundColor = ConsoleColor.Red;
            //       //Console.WriteLine($"Error: {ex.Message}");
            //       Console.WriteLine(ex.Message);
            //   }
////
            //   Console.ForegroundColor = ConsoleColor.White;
            //}
            
            //Mientras se reciba una entrada el interprete sigue ejecutandose
             while(true)
             {
                 Console.Write("> ");
                 // Input (linea) a analizar
                 string? s = Console.ReadLine();
                 //string? s = "let x=4 in \n print(x);";
                 if(s == "")
                 {
                     break;
                 }
                 //try- catch en caso de que lance una excepcion, que lo imprima y siga funcionando
                 try
                 {
                     // Lexer recibe el input (s) y crea la lista de tokens
                     Lexer T =  new Lexer(s);
                     // Se obtiene la lista de tokens que hace el Lexer
                     List<Token> TS = T.Tokens_sequency;
                     // Parser recibe la lista de tokens (TS) y crea el arbol de sintaxis
                     Parser P = new Parser(TS);
                     // Se obtiene el arbol (N)
                     Node N = P.Parse();
                     // El metodo Read_Parser del Analizador Semantico y recibe el arbol 
                     sa.Read_Parser(N);
                     Console.ForegroundColor = ConsoleColor.DarkBlue;
                     // Recibe el arbol, lo analiza y devuelve el resultado
                     sa.Choice (N);   
                 }
                 catch (System.Exception ex)
                 {
                     Console.ForegroundColor = ConsoleColor.Red;
                     //Console.WriteLine($"Error: {ex.Message}");
                     Console.WriteLine(ex.Message);
                 }
                 Console.ForegroundColor = ConsoleColor.White;
             }
        }
        
    }
}
