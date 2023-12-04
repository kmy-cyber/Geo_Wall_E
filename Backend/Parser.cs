using System.Xml;
using System.Text.RegularExpressions;
using System.Dynamic;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection.Metadata.Ecma335;
using G_Wall_E;

namespace INTERPRETE_C__to_HULK
{
    public class Parser
    {
        /// <summary>
        /// Lista de tokens
        /// </summary>
        List<Token> TS;
        ///<summary>
        /// Posicion actual en la lista de tokens
        /// </summary>
        int position;
        ///<summary>
        /// Diccionario de las variables
        /// </summary>
        public Dictionary<string,object> Variables;

        /// <summary>
        /// Constructor de la clase Parser
        /// </summary>
        public Parser(List<Token>Tokens_Sequency)
        {
            position = 0; //inicializa la posicion a 0
            TS = Tokens_Sequency; // Almacena la secuencia de Tokens
            Variables = new Dictionary<string, object>(); // Inicializa el diccionario de variables
        }

        /// <summary>
        /// Método Parse que genera el árbol de análisis sintáctico
        /// </summary>
        /// <returns>
        /// Arbol de sintaxis AST
        /// </returns>
        public Node Parse()
        {
            List<Node> Children = new List<Node>();

            while (TS[position].Type != TokenType.EOF)
            {
                Children.Add(Global_Layer());
                Expect(TokenType.D_COMMA, ";");
            }

            return new Node { Type = "Root_of_the_tree", Children = Children };
        }
        
        /// <summary>
        /// Método Global_Layer que decide qué acción tomar en función del token actual
        /// </summary>
        public Node Global_Layer()
        {
            if( position < TS.Count && Convert.ToString(TS[position].Value) == "draw" )
            {
                return Drawable();
            }
            
            if( position < TS.Count && Convert.ToString(TS[position].Value) == "measure" )
            {
                return Measure();
            }

            if( position < TS.Count && Convert.ToString(TS[position].Value) == "let" )
            {
                return Assigment();
            }

            if( position < TS.Count && Convert.ToString(TS[position].Value) == "if" )
            {
                return Conditional();
            }
            if( position < TS.Count && Convert.ToString(TS[position].Value) == "import" )
            {
                return Conditional();
            }

            return Layer_6(); 
        }

        /// <summary>
        /// Este método se encarga de procesar la importacion de codigo
        /// </summary>
        public Node Import_Code()
        {
            position ++;
            if(TS[position].Type == TokenType.STRING)
            {
                string dir = TS[position].Value.ToString();
                Node dir_code = new Node { Type = "direction", Value = dir};
                return new Node { Type = "import" , Children = new List<Node>{ dir_code}};
            }
            Input_Error("Se espera un string con la direccion del archivo a importar");
            return null;
        }

    	/// <summary>
        /// Este método se encarga de procesar las asignaciones de variables del lenguaje (LET-IN)
        /// </summary>
        public Node Assigment()
        {
            position++;
            Node assigments = new Node{ Type = "assigment_list"};
            bool d_comma = false;

            do{
                if (d_comma)
                {
                    position++;
                }
                d_comma = true;

                Expect(TokenType.VARIABLE,"nombre_de_variable");
                Node name = new Node { Type = "name" , Value = TS[position-1].Value};
                Expect(TokenType.EQUAL,"=");
                Node value = Layer_6();
                Exceptions_Missing(value,"let-in");

                Node var = new Node { Type = "assigment", Children = new List<Node>{name,value}}; 
                assigments.Children.Add(var);

            }while(TS[position].Type == TokenType.D_COMMA);

            Expect(TokenType.IN,"in");
            Node operations = Global_Layer();
            Exceptions_Missing(operations,"let-in");

            Node variable = new Node { Type = "Let", Children = new List<Node>{assigments,operations} }; 
            return variable;
        }

        /// <summary>
        /// Este método se encarga de procesar los objetos a dibujar (DRAW)
        /// </summary>
        public Node Drawable()
        {
            position++;            
            Node expression = Global_Layer();
            Node str = new Node {Type = "empty" };

            if (TS[position].Type == TokenType.STRING)
            {
                str = Factor();
            }
            
            return new Node {Type = "draw", Children = new List<Node>{expression, str}};
        }
        
        /// <summary>
        /// Este método se encarga de procesar las medidas entre 2 puntos (MEASURE)
        /// </summary>
        public Node Measure()
        {
            position++;

            Expect(TokenType.L_PHARENTESYS,"(");
            Node p1 = Factor();
            Expect(TokenType.COMMA,",");
            Node p2 = Factor();
            Expect(TokenType.R_PHARENTESYS,")");
            
            return new Node {Type = "measure", Children = new List<Node>{p1, p2}};
        }

        /// <summary>
        /// Este método se encarga de procesar las estructuras condicionales del lenguaje (IF-ELSE)
        /// </summary>
        public Node Conditional()
        {
            position++;
            Node condition = Layer_6();
            Expect(TokenType.THEN,"then");
            Node operations_if = Global_Layer();
            Expect(TokenType.ELSE,"else");
            Node operations_else = Global_Layer();
            Node conditional_if_else = new Node { Type = "Conditional", Children = new List<Node>{condition,operations_if,operations_else} }; 
            return conditional_if_else;
        }

        /// <summary>
        /// Este método se encarga de procesar la declaracion de funciones del lenguaje
        /// </summary>
        public Node Function()
        {
            position++;
        
            Node operation = Global_Layer();
            Exceptions_Missing(operation,"function");
            return operation; 
        }

        #region CAPAS // Estos métodos implementan la precedencia de operadores del lenguaje

            /// <summary>
            /// CAPA 6 (Operador '@' de concatenacion)
            /// </summary>
            public Node Layer_6()
            {
                Node node = Layer_5();
                if(position < TS.Count && Convert.ToString(TS[position].Value) == "@" )
                {
                    string? op = Convert.ToString(TS[position ++].Value);
                    Node right = Layer_5();
                    Exceptions_Missing(right,"");
                    node = new Node { Type = op, Children = new List<Node>{node,right}};
                }
                return node;
            }

            /// <summary>
            /// CAPA 5 Operadores ('&' '|')
            /// </summary>
            public Node Layer_5()
            {
                Node node = Layer_4();
                while(position < TS.Count && (Convert.ToString(TS[position].Value) == "&" || Convert.ToString(TS[position].Value) == "|"))
                {
                    string? op = Convert.ToString(TS[position++].Value);
                    Node right = Layer_4();
                    Exceptions_Missing(right,"");
                    node = new Node { Type = op, Children = new List<Node> { node, right } };
                }
                return node;
            }

            /// <summary>
            /// CAPA 4 (Operadores '>' '<' '==' '!=' '>=' '<=' de comparacion)
            /// </summary>
            public Node Layer_4()
            {
                Node node = Layer_3();
                while (position < TS.Count && (Convert.ToString(TS[position].Value) == "==" || Convert.ToString(TS[position].Value) == "!=" || Convert.ToString(TS[position].Value) == "<=" || Convert.ToString(TS[position].Value) == ">=" || Convert.ToString(TS[position].Value) == "<" || Convert.ToString(TS[position].Value) == ">"  ))
                {
                    string? op =  Convert.ToString(TS[position++].Value);
                    Node right = Layer_3();
                    Exceptions_Missing(right,"");
                    node = new Node { Type = op, Children = new List<Node> { node, right } };
                }
                return node;
            }

            /// <summary>
            /// CAPA 3 (Operadores '+' suma y  '-' resta)
            /// </summary>
            public Node Layer_3()
            {
                Node node = Layer_2();
                while (position < TS.Count && (Convert.ToString(TS[position].Value) == "+" || Convert.ToString(TS[position].Value) == "-"))
                {
                    string? op =  Convert.ToString(TS[position++].Value);
                    Node right = Layer_2();
                    Exceptions_Missing(right,"");
                    node = new Node { Type = op, Children = new List<Node> { node, right } };
                }
                return node;

            }

            /// <summary>
            /// CAPA 2 (Operadores de '*' multiplicacion y '/' division)
            /// </summary>
            public Node Layer_2()
            {
                Node node = Layer_1();
                string? a = Convert.ToString(TS[position].Value);
                while (position < TS.Count && (Convert.ToString(TS[position].Value) == "*" || Convert.ToString(TS[position].Value) == "/" || Convert.ToString(TS[position].Value) == "%"))
                {
                    string? op = Convert.ToString(TS[position++].Value);
                    Node right = Layer_1();
                    Exceptions_Missing(right,"");
                    node = new Node { Type = op, Children = new List<Node> { node, right } };
                }
                return node;
            }

            /// <summary>
            /// CAPA 1 (Operador '^' Potencia)
            /// </summary>
            public Node Layer_1()
            {
                Node node = Factor();
                while(position < TS.Count && Convert.ToString(TS[position].Value) == "^" )
                {
                    string? op = Convert.ToString(TS[position++].Value);
                    Node right = Factor();
                    Exceptions_Missing(right,"");
                    node = new Node { Type = op, Children = new List<Node> { node, right } };
                }
                return node;
            }

            public Node Layer_0()
            {
                if(TS[position].Type == TokenType.LINE) //recibe dos variables
                {
                    Node name = new Node{Type = "g_name", Value = TS[position-2].Value.ToString()};
                    position++;
                    Expect(TokenType.L_PHARENTESYS,"(");
                    Node var1 = Factor();
                    Expect(TokenType.COMMA,",");
                    Node var2 = Factor();
                    Expect(TokenType.R_PHARENTESYS,")");
                    return new Node{ Type = "line", Children = new List<Node>{name, var1,var2} };

                }
                
                else if(TS[position].Type == TokenType.SEGMENT) //recibe dos variables
                {
                    Node name = new Node{Type = "g_name", Value = TS[position-2].Value.ToString()};
                    position++;
                    Expect(TokenType.L_PHARENTESYS,"(");
                    Node var1 = Factor();
                    Expect(TokenType.COMMA,",");
                    Node var2 = Factor();
                    Expect(TokenType.R_PHARENTESYS,")");
                    return new Node{ Type = "segment", Children = new List<Node>{name, var1,var2} };
   
                }
                
                else if(TS[position].Type == TokenType.RAY)//recibe dos variables
                {
                    Node name = new Node{Type = "g_name", Value = TS[position-2].Value.ToString()};
                    position++;
                    Expect(TokenType.L_PHARENTESYS,"(");
                    Node var1 = Factor();
                    Expect(TokenType.COMMA,",");
                    Node var2 = Factor();
                    Expect(TokenType.R_PHARENTESYS,")");
                    return new Node{ Type = "ray", Children = new List<Node>{name, var1,var2} };

                }
                
                else if(TS[position].Type == TokenType.CIRCLE)//recibe dos variables
                {
                    Node name = new Node{Type = "g_name", Value = TS[position-2].Value.ToString()};
                    position++;
                    Expect(TokenType.L_PHARENTESYS,"(");
                    Node point = Factor();
                    Expect(TokenType.COMMA,",");
                    Node measure = Factor();
                    Expect(TokenType.R_PHARENTESYS,")");
                    return new Node{ Type = "circle", Children = new List<Node>{name, point, measure} };

                }

                else if(TS[position].Type == TokenType.ARC)//recibe tres variables
                {
                    Node name = new Node{Type = "g_name", Value = TS[position-2].Value.ToString()};
                    position++;
                    Expect(TokenType.L_PHARENTESYS,"(");
                    Node var1 = Factor();
                    Expect(TokenType.COMMA,",");
                    Node var2 = Factor();
                    Expect(TokenType.COMMA,",");
                    Node var3 = Factor();
                    Expect(TokenType.COMMA,",");
                    Node var4 = Factor();
                    Expect(TokenType.R_PHARENTESYS,")");
                    return new Node{ Type = "arc", Children = new List<Node>{name, var1, var2, var3, var4} };
                }

                else if(TS[position].Type == TokenType.MEASURE)//recibe dos variables
                {
                    Node name = new Node{Type = "g_name", Value = TS[position-2].Value.ToString()};
                    position++;
                    Expect(TokenType.L_PHARENTESYS,"(");
                    Node point_1 = Factor();
                    Expect(TokenType.COMMA,",");
                    Node point_2 = Factor();
                    Expect(TokenType.R_PHARENTESYS,")");
                    return new Node{ Type = "measure", Children = new List<Node>{name, point_1, point_2} };

                }


                

                return null;
            }

            /// <summary>
            /// CAPA 0 o CAPA FACTOR 
            /// </summary>
            public Node Factor ()
            {
                Token current_token= TS[position]; // Obtiene el token actual
                if (position >= TS.Count)
                    throw new Exception("Unexpected end of input");

                // Si el token actual es un paréntesis izquierdo, procesa una expresión entre paréntesis
                if(current_token.Type == TokenType.L_PHARENTESYS) 
                {
                    position++;
                    Node node = Global_Layer();
                    if(position>=TS.Count && TS[position].Type != TokenType.R_PHARENTESYS )
                    {
                        Input_Error(" ')' Expected!");
                    }
                    position++;
                    return node;
                }
                
                // si el token es !
                if(current_token.Value.ToString() == "!")
                {
                    string? op = Convert.ToString(TS[position++].Value);
                    Node n = Global_Layer();
                    return new Node {Type = op, Children = new List<Node>{n}};
                    
                }

                //Si el token actual es un número, retorna un nodo de tipo "number" con el valor del número
                else if( TS[position].Type == TokenType.NUMBER )
                {
                    dynamic value = Convert.ToDouble(TS[position++].Value);
                    return new Node { Type = "number", Value = value };
                }

                //Si el token actual es "true", retorna un nodo de tipo "true" con el valor true
                else if( TS[position].Type == TokenType.TRUE)
                {
                    dynamic value = TS[position++].Value;
                    return new Node { Type = "true", Value = value };
                }

                // Si el token actual es "false", retorna un nodo de tipo "false" con el valor false
                else if( TS[position].Type == TokenType.FALSE )
                {
                    dynamic value = TS[position++].Value;
                    return new Node { Type = "false", Value = value };
                }

                // Si el token actual es una cadena, retorna un nodo de tipo "string" con el valor de la cadena
                else if( TS[position].Type == TokenType.STRING )
                {
                    dynamic? value = Convert.ToString(TS[position++].Value);
                    return new Node { Type = "string", Value = value};
                }

               
                else if( TS[position].Type == TokenType.VARIABLE)
                {
                    if(TS[position+1].Type == TokenType.EQUAL)
                    {
                        position+=2;
                        return Layer_0();
                    }
                    if(TS[position+1].Type == TokenType.L_PHARENTESYS) // si el token siguiente es parentesis procesar como funcion 
                    {
                        dynamic? f_name = Convert.ToString(TS[position++].Value);
                        position++;
                        Node name =  new Node { Type = "d_function_name", Value = f_name};
                        Node param = new Node{ Type = "parameters"};
                        if(TS[position].Type != TokenType.R_PHARENTESYS)
                        {
                            do
                            {
                                Node parammeter_name = new Node { Type = "p_name" , Value = Layer_6()};
                                param.Children.Add(parammeter_name);

                                if(TS[position].Type == TokenType.COMMA)
                                {
                                    position++;
                                }
                            }while (TS[position-1].Type == TokenType.COMMA);
                        }

                        Expect(TokenType.R_PHARENTESYS, ")");
                        if(TS[position].Type == TokenType.EQUAL)
                        {
                            Node action = Function();
                            return new Node { Type = "function", Children = new List<Node> {name,param,action}};
                        }
                        return new Node { Type = "declared_function", Children = new List<Node> {name,param}};

                    }
                    // procesar como variable
                    dynamic? value = Convert.ToString(TS[position++].Value);
                    return new Node { Type = "variable", Value = value};
                }

                // Si el token actual es "cos", procesa una función coseno
                else if(TS[position].Type == TokenType.COS)
                {
                    position++;
                    Expect(TokenType.L_PHARENTESYS,"(");
                    Node valor = Layer_6();
                    Expect(TokenType.R_PHARENTESYS, ")");
                    return new Node {Type = "cos", Children = new List<Node>{valor}};
                }

                // Si el token actual es "sen", procesa una función seno
                else if(TS[position].Type == TokenType.SEN)
                {
                    position++;
                    Expect(TokenType.L_PHARENTESYS,"(");
                    Node valor = Layer_6();
                    Expect(TokenType.R_PHARENTESYS, ")");
                    return new Node {Type = "sin", Children = new List<Node>{valor}};
                }

                // Si el token actual es "log", procesa una función logaritmo
                else if(TS[position].Type == TokenType.LOG)
                {
                    position++;
                    Expect(TokenType.L_PHARENTESYS,"(");
                    Node valor = Layer_6();
                    Expect(TokenType.COMMA,",");
                    Node valor2 = Layer_6();
                    Expect(TokenType.R_PHARENTESYS, ")");
                    return new Node {Type = "log", Children = new List<Node>{valor,valor2}};
                }
                 
                // Si el token actual es "let", procesa una asignacion
                else if( position < TS.Count && Convert.ToString(TS[position].Value) == "let" )
                {
                    return Assigment();
                }

                //* GEO_WALL_E
                else if(TS[position].Type == TokenType.POINT)
                {
                    position++;
                    Node name = Factor();
                    Node x = null;
                    Node y = null;
                    
                    return new Node {Type = "point", Children = new List<Node>{name,x,y}};
                }

                else if(TS[position].Type == TokenType.LINE)
                {
                    position++;
                    Node name = Factor();
                    Expect(TokenType.VARIABLE, TS[position].Value);
                   
                    return new Node {Type = "line_d", Children = new List<Node>{name}};
                }

                else if(TS[position].Type == TokenType.SEGMENT)
                {
                    position++;
                    Node name = Factor();
                    Expect(TokenType.VARIABLE, TS[position].Value);
                    
                    return new Node {Type = "segment_d", Children = new List<Node>{name}};
                }

                else if(TS[position].Type == TokenType.RAY)
                {
                    position++;
                    Node name = Factor();
                    Expect(TokenType.VARIABLE, TS[position].Value);
                    
                    return new Node {Type = "ray_d", Children = new List<Node>{name}};
                }

                else if(TS[position].Type == TokenType.ARC)
                {
                    position++;
                    Node name = Factor();
                    Expect(TokenType.VARIABLE, TS[position].Value);
                    
                    return new Node {Type = "arc_d", Children = new List<Node>{name}};
                }

                else if(TS[position].Type == TokenType.CIRCLE)
                {
                    position++;
                    Node name = Factor();
                    Expect(TokenType.VARIABLE, TS[position].Value);
                    
                    return new Node {Type = "circle_d", Children = new List<Node>{name}};
                }

                // Si el token actual es nulo, retorna un nodo vacío
                else if(TS[position] == null)
                {
                    return new Node{};
                }

                // Si el token actual no coincide con ninguno de los anteriores, retorna un nodo de error
                else
                {
                    return new Node{Type="error",Value=0};
                }
            }
        #endregion

        #region Auxiliar

            /// <summary>
            /// Método que lanza una excepción con un mensaje de error de sintaxis
            /// </summary>
            private void Input_Error(string error)
            {
                throw new Exception("SYNTAX ERROR: " + error);
            }

            /// <summary>
            ///  Método que verifica si un nodo es de tipo "error" y lanza una excepción en ese caso
            /// </summary>
            private void Exceptions_Missing(Node node, string op)
            {
                if(node.Type == "error")
                {
                    if(op == "")
                    {
                       Input_Error($"Missing expression after variable `{TS[position-1].Value}`");
                    }
                    else
                    {
                        string msg = $"Missing expression in `{op}` after variable `{TS[position-1].Value}`";
                        Input_Error(msg);
                    }
                }
            }

            /// <summary>
            /// Método que verifica si el token actual es del tipo esperado y avanza a la siguiente posición en ese caso, o lanza una excepción si no lo es
            /// </summary>
            public void Expect(TokenType tokenType, object? value)
            {
                if(TS[position].Type == tokenType)
                {
                    position++;
                }
                else
                {
                    Input_Error($"[{position}] `{value}` Expected! after `{TS[position-1].Value}`,`{TS[position].Value}` was received");
                }
            }




        #endregion

    }
}