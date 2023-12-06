using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace INTERPRETE_C__to_HULK
{
    public class Lexer
    {
        string Text; //texto de entrada
        int position; // Posición actual en el texto de entrada
        char currentChar; // Carácter actual en el texto de entrada
        public List<Token> Tokens_sequency { get; } // Secuencia de tokens generada por el lexer

        public Lexer(string text)
        {
            Text = text; // Inicializa el texto de entrada
            position = 0; // Inicializa la posición al principio del texto de entrada
            currentChar = Text[position]; // Inicializa el carácter actual al primer carácter del texto de entrada
            Tokens_sequency = Get_Sequency(Text, position); // Genera la secuencia de tokens
        }

    #region  TOKENS

        /// <summary>
        /// Metodo que clasifica los elementos de la entrada en su token correspondiente
        /// </summary>
        public Token Get_token()
        {
            // Mientras no se llegue al final del texto de entrada
            while (currentChar != '\0')
            {
                // Si el carácter actual es un espacio en blanco, lo omite y continúa con el siguiente carácter
                if (char.IsWhiteSpace(currentChar))
                {
                    Skip_Space();
                    continue;
                }

                // Si el carácter actual es un dígito, retorna un token de tipo NUMBER con el valor del número
                if (char.IsDigit(currentChar))
                {
                    return new Token(TokenType.NUMBER, Int_Analyzer());
                }

                // Si el carácter actual es una letra o un guion bajo, puede retornar un tipo variable con su valor, o puede retornar un tipo underscore
                if (char.IsLetter(currentChar) || currentChar == '_')
                {
                    string word = String_Analyzer();
                    return Own_Words(word);

                }

                //si el caracter actual es un punto, rectificar que se cumplan los tres puntos consecutivos para que sea un token valido
                if (currentChar == '.')
                {
                    string op = Infinite_Analyzer();
                    if (op == "...") return Own_Words(op);
                }

                // Si el carácter actual es un operador, retorna un token de tipo OPERATOR con el valor del operador
                if (currentChar == '+')
                {
                    Move_on();
                    return new Token(TokenType.OPERATOR, '+');
                }

                if (currentChar == '-')
                {
                    Move_on();
                    return new Token(TokenType.OPERATOR, '-');
                }

                if (currentChar == '*')
                {
                    Move_on();
                    return new Token(TokenType.OPERATOR, '*');
                }

                if (currentChar == '/')
                {
                    Move_on();
                    return new Token(TokenType.OPERATOR, '/');
                }

                if (currentChar == '^')
                {
                    Move_on();
                    return new Token(TokenType.OPERATOR, '^');
                }

                if (currentChar == '%')
                {
                    Move_on();
                    return new Token(TokenType.OPERATOR, '%');
                }

                // Si el carácter actual es '=', verifica si el siguiente carácter es '=' o '>' , y retorna un token correspondiente
                if (currentChar == '=')
                {
                    Move_on();
                    if (currentChar == '=')
                    {
                        Move_on();
                        return new Token(TokenType.OPERATOR, "==");
                    }
                    else if (currentChar == '>')
                    {
                        Move_on();
                        return new Token(TokenType.DO, "=>");
                    }
                    else
                    {
                        return new Token(TokenType.EQUAL, "=");
                    }
                }

                // Si el carácter actual es '!', verifica si el siguiente carácter es '=' o no, y retorna un token correspondiente
                if (currentChar == '!')
                {
                    Move_on();
                    if (currentChar == '=')
                    {
                        Move_on();
                        return new Token(TokenType.OPERATOR, "!=");
                    }
                    else
                    {
                        return new Token(TokenType.OPERATOR, "!");
                    }
                }

                // Si el carácter actual es '<', verifica si el siguiente carácter es '=' o no, y retorna un token correspondiente
                if (currentChar == '<')
                {
                    Move_on();
                    if (currentChar == '=')
                    {
                        Move_on();
                        return new Token(TokenType.OPERATOR, "<=");
                    }
                    else
                    {
                        return new Token(TokenType.OPERATOR, "<");
                    }
                }

                // Si el carácter actual es '>', verifica si el siguiente carácter es '=' o no, y retorna un token correspondiente
                if (currentChar == '>')
                {
                    Move_on();
                    if (currentChar == '=')
                    {
                        Move_on();
                        return new Token(TokenType.OPERATOR, ">=");
                    }
                    else
                    {
                        return new Token(TokenType.OPERATOR, ">");
                    }
                }

                // Si el carácter actual es '&', retorna un token de tipo OPERATOR con el valor '&'
                if (currentChar == '&')
                {
                    Move_on();
                    return new Token(TokenType.OPERATOR, '&');
                }

                // Si el carácter actual es '|', retorna un token de tipo OPERATOR con el valor '|'
                if (currentChar == '|')
                {
                    Move_on();
                    return new Token(TokenType.OPERATOR, '|');
                }

                // Si el carácter actual es '(', retorna un token de tipo L_PHARENTESYS con el valor '('
                if (currentChar == '(')
                {
                    Move_on();
                    return new Token(TokenType.L_PHARENTESYS, '(');
                }

                // Si el carácter actual es ')', retorna un token de tipo R_PHARENTESYS con el valor ')'
                if (currentChar == ')')
                {
                    Move_on();
                    return new Token(TokenType.R_PHARENTESYS, ')');
                }

                //Si el caracter actual es '{', retorna un token de tipo L_KEY con el valor '{'
                if(currentChar == '{')
                {
                    Move_on();
                    return new Token(TokenType.L_KEY, '{');
                }

                //Si el caracter actual es '}', retorna un token de tipo R_KEY con el valor '}'
                if(currentChar == '}')
                {
                    Move_on();
                    return new Token(TokenType.R_KEY, '}');
                }

                // Si el carácter actual es ';', retorna un token de tipo D_COMMA con el valor ';'
                if (currentChar == ';')
                {
                    Move_on();
                    return new Token(TokenType.D_COMMA, ';');
                }

                // Si el carácter actual es ',', retorna un token de tipo COMMA con el valor ','
                if (currentChar == ',')
                {
                    Move_on();
                    return new Token(TokenType.COMMA, ',');
                }

                // Si el carácter actual es '\"', lee una cadena y retorna un token de tipo STRING con el valor de la cadena
                if (currentChar == '\"')
                {
                    Move_on();
                    (bool, string, int) s = Read(position, Text);
                    if (!s.Item1)
                    {
                        Input_Error(" no ha cerrado las comillas, string invalido");
                    }
                    position = s.Item3;
                    Move_on();
                    return new Token(TokenType.STRING, s.Item2);
                }

                // Si el carácter actual es '@', retorna un token de tipo OPERATOR con el valor '@'
                if (currentChar == '@')
                {
                    Move_on();
                    return new Token(TokenType.OPERATOR, '@');
                }

                // Si ninguno de los casos anteriores se cumple, lanza una excepción
                Input_Error("Unknown character " + "\'" + currentChar + "\'");
            }
            // Al final del texto de entrada, retorna un token de tipo EOF
            return new Token(TokenType.EOF, null);
        }
        #endregion

        /// <summary>
        /// Metodo que dependiendo de la palabra, retorna un token con el tipo y valor correspondiente
        /// </summary>
        public Token Own_Words(string word)
        {

            switch (word)
            {
                case "print":
                    return new Token(TokenType.PRINT, "print");
                case "let":
                    return new Token(TokenType.LET, "let");
                case "in":
                    return new Token(TokenType.IN, "in");
                case "if":
                    return new Token(TokenType.IF, "if");
                case "then":
                    return new Token(TokenType.THEN, "then");
                case "else":
                    return new Token(TokenType.ELSE, "else");
                case "function":
                    return new Token(TokenType.FUNCTION, "function");
                case "PI":
                    return new Token(TokenType.NUMBER, Math.PI);
                case "TAU":
                    return new Token(TokenType.NUMBER, Math.Pow(Math.PI, 2));
                case "true":
                    return new Token(TokenType.TRUE, "true");
                case "false":
                    return new Token(TokenType.FALSE, "false");
                case "cos":
                    return new Token(TokenType.COS, "cos");
                case "sin":
                    return new Token(TokenType.SEN, "sin");
                case "log":
                    return new Token(TokenType.LOG, "log");


                //* New add Geo_Wall_E

                //Figuras geometricas
                case "point":
                    return new Token(TokenType.POINT, "point");
                case "line":
                    return new Token(TokenType.LINE, "line");
                case "segment":
                    return new Token(TokenType.SEGMENT, "segment");
                case "ray":
                    return new Token(TokenType.RAY, "ray");
                case "circle":
                    return new Token(TokenType.CIRCLE, "circle");
                case "arc":
                    return new Token(TokenType.ARC, "arc");

                //funciones
                case "draw":
                    return new Token(TokenType.DRAW, "draw");
                case "color":
                    return new Token(TokenType.COLOR, "color");
                case "restore":
                    return new Token(TokenType.RESTORE, "restore");
                case "import":
                    return new Token(TokenType.IMPORT, "import");
                case "measure":
                    return new Token(TokenType.MEASURE, "measure");
                case "count":
                    return new Token(TokenType.COUNT, "count");

                //funciones que devuelven una secuencia
                case "intersect":
                    return new Token(TokenType.INTERSECT, word);
                case "points":
                    return new Token(TokenType.POINT, word);
                case "samples":
                    return new Token(TokenType.SAMPLES, word);
                case "randoms":
                    return new Token(TokenType.RANDOMS, word);
                case "sequence":
                    return new Token(TokenType.SEQUENCE, word); //revisar este caso, ya que en codigo sería de la forma: point sequence ps;
                                                                //por tanto, se estaria hablando de un tipo de token secuencia, donde los 
                                                                //elementos de la sequencia son tipo de token point(esto supongo que seria en 
                                                                //el sintactico y semantico)

                //parametros
                case "_":
                    return new Token(TokenType.UNDERSCORE, "_");
                case "...":
                    return new Token(TokenType.INFINITE, "...");
                case "undefined":
                    return new Token(TokenType.UNDEFINED, "undefined");

                //ganma de colores
                case "blue":
                case "red":
                case "yellow":
                case "green":
                case "cyan":
                case "magenta":
                case "white":
                case "gray":
                case "black":
                    return new Token(TokenType.COLOR_RANGE, word);
                //*

                // Si la palabra no es ninguna de las anteriores, retorna un token de tipo VARIABLE con el valor de la palabra
                default:
                    return new Token(TokenType.VARIABLE, word);
            }
        }

        #region Auxiliares

        /// <summary>
        /// Método que lanza una excepción con un mensaje de error lexico
        /// </summary>
        private void Input_Error(string error)
        {
            throw new Exception("LEXICAL ERROR: " + error);
        }

        /// <summary>
        /// Metodo que avanza a la siguiente posición en el texto de entrada
        /// </summary>
        public void Move_on()
        {
            position++;
            if (position < Text.Length)
            {
                currentChar = Text[position];
            }
            else
            {
                currentChar = '\0';
            }
        }

        /// <summary>
        /// Metodo que salta espacios en blanco
        /// </summary>
        public void Skip_Space()
        {
            // Mientras no se llegue al final del texto de entrada y el carácter actual 
            //sea un espacio en blanco, avanza al siguiente carácter
            while (currentChar != '\0' && char.IsWhiteSpace(currentChar))
            {
                Move_on();
            }
        }

        /// <summary>
        /// Metodo que capta el numero entero
        /// </summary>
        public double Int_Analyzer()
        {
            string number = "";
            int dot_counter = 0;
            while (currentChar != '\0' && (char.IsDigit(currentChar) || currentChar == '.'))
            {
                if (currentChar == '.') dot_counter++;
                number += currentChar;
                Move_on();
            }
            if (char.IsLetter(currentChar))
            {
                number += currentChar;
                Input_Error("Invalid token " + "\'" + number + "\'");
            }
            if (currentChar == '.') number += '0';
            if (dot_counter > 1) Input_Error("Invalid number");
            return Convert.ToDouble(number);
        }

        /// <summary>
        /// Metodo que capta la palabra completa
        /// </summary>
        /// 
        public string String_Analyzer()
        {
            string value = "";
            while (position < Text.Length && (char.IsLetterOrDigit(currentChar) || currentChar == '_'))
            {
                value += currentChar;
                Move_on();
            }
            return value;
        }

        //metodo que rectifica que el token es tipo "..."
        public string Infinite_Analyzer()
        {
            string value = "";
            while (position < Text.Length && currentChar == '.')
            {
                value += currentChar;
                Move_on();
            }
            return value;
        }

        /// <summary>
        /// Metodo que capta el string entre comillas
        /// </summary>
        /// <returns>
        /// La cadena y si no se cierran las comillas retorna false
        /// </returns>
        static (bool, string, int) Read(int position, string text)
        {
            string s = "";
            while (position < text.Length && text[position] != '\"')
            {
                s += text[position];
                position++;
            }
            if (position <= text.Length && text[position] == '\"')
            {

                return (true, s, position);
            }

            return (false, "", 0);
        }

        /// <summary>
        /// Metodo que recibe el texto de entrada y la posicion inicial
        /// </summary>
        /// <returns>
        /// Lista de tokens
        /// </returns>
        public List<Token> Get_Sequency(string text, int position)
        {
            List<Token> TS = new List<Token>();
            while (text[position] != '\0')
            {
                Token currentToken = Get_token();
                TS.Add(currentToken);
                if (currentToken.Type == TokenType.EOF)
                {
                    break;
                }
            }
            return TS;
        }
        #endregion


    }
}