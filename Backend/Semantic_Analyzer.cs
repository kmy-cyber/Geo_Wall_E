using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Xml;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using INTERPRETE_C_to_HULK;
using G_Wall_E;

namespace INTERPRETE_C__to_HULK
{
    public class Semantic_Analyzer<T> where T : IComparable<T>
    {
        Node AST; // Árbol de Análisis Sintáctico Abstracto (AST)

        string FigureColor; // Color actual para las figuras

        Dictionary<string,object> variables_globales; // Diccionario para almacenar las variables globales
        
        public List<Function_B> functions_declared = new List<Function_B>(); // Lista para almacenar las funciones declaradas
        
        public List<Dictionary<string,dynamic>> Scopes; // Lista de diccionarios para almacenar los ámbitos (scopes)
        
        /// <summary>
        /// Constructor de la clase Semantic_Analyzer
        /// </summary>
        public Semantic_Analyzer()
        {
            // Inicializa el diccionario de variables globales con algunas variables predefinidas
            variables_globales = new Dictionary<string,object>
            {
                {"PI",Math.PI},
                {"TAU",Math.Pow(Math.PI,2)},
                {"true",true},
                {"false",false},
            };
            
            FigureColor = "black";
        }

        /// <summary>
        /// Método para leer el AST generado por el Parser
        /// </summary>
        public void Read_Parser(Node n)
        {
            AST = n;
            Scopes = new List<Dictionary<string,object>>{variables_globales};
        }

        /// <summary>
        ///  Método para decidir qué acción tomar en función del tipo de nodo
        /// </summary>
        public void Choice(Node node)
        {
            switch (node.Type)
            {
                // Si el nodo es una función declarada, evalúa el nodo y muestra el resultado
                case "print":
                    Console.WriteLine(Evaluate(node.Children[0]));
                    break; 
                // Para cualquier otro tipo de nodo, simplemente evalúa el nodo 
                default:
                    Evaluate(node);
                    break;                  
            }
        }

        /// <summary>
        /// Metodo para Evaluar los Nodos
        /// </summary>
        public object? Evaluate(Node node)
        {
            // Dependiendo del tipo de nodo, realiza diferentes operaciones
            switch (node.Type)
            {
                // Si el nodo es un número, retorna su valor
                case "number":
                    return node.Value;
                // Si el nodo es un string, retorna su valor
                case "string":
                    return node.Value; 
                // Si el nodo es "true", retorna true
                case "true":
                    return true;
                // Si el nodo es "false", retorna false
                case "false":
                    return false;
                // Si el nodo es una variable, retorna su valor del ámbito actual
                case "variable":
                    return Scopes[Scopes.Count - 1][node.Value.ToString()];
                // Si el nodo es el nombre de una función declarada, retorna su valor
                case "d_function_name":
                    return node.Value;

                //? Operaciones arirmeticas 
                // Si el nodo es una operación de suma, resta, multiplicación, división, 
                //exponente o módulo, evalúa los nodos hijos y realiza la operación correspondiente

                case "+" : case "-": case "*": case "/": case "^": case "%":
                    object ?left_a = Evaluate(node.Children[0]);
                    object ?right_a = Evaluate(node.Children[1]);
                    Type_Expected(right_a,left_a,"number","+");
                    switch(node.Type)
                    {
                        case "+": return (double)left_a + (double)right_a;
                        case "-": return (double)left_a - (double)right_a;
                        case "*": return (double)left_a * (double)right_a;
                        case "/": return (double)left_a / (double)right_a;
                        case "^": return Math.Pow((double)left_a, (double)right_a);;
                        default: return (double)left_a + (double)right_a;
                    }
                    

                //? Boolean operations
                // Si el nodo es una operación booleana (>, <, >=, <=, ==, !=, !), 
                //evalúa los nodos hijos y realiza la operación correspondiente
                case "!":
                    object ?not = Evaluate(node.Children[0]);
                    Expected(not,"boolean","! not"); 
                    return !(bool)not;
                case ">" : case "<": case ">=": case "<=": case "==": case "!=":
                    object ?left_b = Evaluate(node.Children[0]);
                    object ?right_b = Evaluate(node.Children[1]);
                    switch(node.Type)
                    {
                        case ">": return (double)left_b > (double)right_b;
                        case "<": return (double)left_b < (double)right_b;
                        case ">=": return (double)left_b >= (double)right_b;
                        case "<=": return (double)left_b <= (double)right_b;
                        case "==":
                            if(left_b is double && right_b is double)
                            {
                                return (double)left_b == (double)right_b;
                            }
                            else if(left_b is string && right_b is string)
                            {
                                return (string)left_b == (string)right_b;
                            }
                            else if(left_b is bool && right_b is bool )
                            {
                                return (bool)left_b == (bool)right_b;
                            }
                            else
                            {
                                Input_Error($"Operator '==' can not be used between values of diferent types");
                            }
                            break;
                        default:
                            if(left_b is double && right_b is double)
                            {
                                return (double)left_b != (double)right_b;
                            }
                            else if(left_b is string && right_b is string)
                            {
                                return (string)left_b != (string)right_b;
                            }
                            else if(left_b is bool && right_b is bool )
                            {
                                return (bool)left_b != (bool)right_b;
                            }
                            else
                            {
                                Input_Error($"Operator '==' can not be used between values of diferent types");
                            }
                            break;
                    }
                    break;
                case "@":
                    object ?left_st = Evaluate(node.Children[0]);
                    object ?right_st = Evaluate(node.Children[1]);
                    string ?type_right = Identify(right_st);
                    string ?type_left = Identify(left_st);
                    if(type_right == "string" || type_left =="string")
                    {
                        return left_st.ToString() + right_st.ToString();
                    }
                    break;
                case "&":
                    object ?left_and = Evaluate(node.Children[0]);
                    object ?right_and = Evaluate(node.Children[1]);
                    Type_Expected(right_and,left_and,"boolean","&");
                    return (bool)left_and &&  (bool)right_and;
                case "|":
                    dynamic ?left_or = Evaluate(node.Children[0]);
                    dynamic ?right_or = Evaluate(node.Children[1]);
                    Type_Expected(right_or,left_or,"boolean","|");
                    return (bool)left_or ||  (bool)right_or;

                //? Expressions
                // Si el nodo es el nombre de una función o un parámetro, retorna su valor
                case "f_name":
                    return node.Value;
                case "p_name":
                    return node.Value;
                case "name":
                    return node.Value;
                case "print":
                // Si el nodo es una impresión (print), evalúa el nodo hijo y muestra el resultado
                    object? value_print = Evaluate(node.Children[0]);
                    Console.WriteLine(value_print);
                    return value_print;
                // Si el nodo es una función coseno o seno, evalúa el nodo hijo y retorna el coseno o seno del resultado
                case "cos":
                    object? value_cos = Evaluate(node.Children[0]);
                    return Math.Cos((double)value_cos * (Math.PI/180));//convirtiendo 
                case "sin":
                    object? value_sin = Evaluate(node.Children[0]);
                    return Math.Sin((double)value_sin* (Math.PI/180));//convirtiendo
                // Si el nodo es una función logaritmo, evalúa los nodos hijos y retorna el logaritmo del segundo resultado 
                //en base al primer resultado  
                case "log":
                    object? value_agrument = Evaluate(node.Children[0]);
                    object? value_base = Evaluate(node.Children[1]);
                    return Math.Log((double)value_base,(double)value_agrument);
                // Si el nodo es un condicional, evalúa la condición y retorna la evaluación del primer o segundo nodo hijo 
                //dependiendo de si la condición es verdadera o falsa
                case "Conditional":
                    object? condition =Evaluate( node.Children[0]);
                    Expected(condition,"bool","if");
                    if((bool)condition)
                    {
                        return Evaluate(node.Children[1]);
                    }
                    return Evaluate(node.Children[2]);
                // Si el nodo es una función, crea una nueva función y la añade a la lista de funciones declaradas
                case "Function":
                    Dictionary<string,object> Var = Get_Var_Param(node.Children[1]);
                    Function_B function = new Function_B(node.Children[0].Value.ToString(),node.Children[2],Var);
                    if(Function_Exist(node.Children[0].Value.ToString()))
                    {
                        throw new Exception("The function "+ "\' " + node.Children[0].Value + " \'" + "already exist in the current context");
                    }
                    functions_declared.Add(function);
                    return functions_declared;
                // Si el nodo es una función declarada, llama a la función y retorna su valor
                case "declared_function":
                    string ?name = node.Children[0].Value.ToString();
                    Node param_f = node.Children[1];
                    if(Function_Exist(name))
                    {
                        Dictionary<string,object> Scope_actual = new Dictionary<string,object>();
                        Scopes.Add(Scope_actual);
                        int f_position = Call_Function(functions_declared,name,param_f);
                        object? value = Evaluate(functions_declared[f_position].Operation_Node);
                        Scopes.Remove(Scopes[Scopes.Count-1]);
                        return value;
                    }
                    else
                    {
                        Input_Error ("The function "+ name +" does not exist in the current context");
                    }
                    break;
                // Si el nodo es una lista de asignaciones, guarda las variables en el ámbito actual
                case "assigment_list":
                    Save_Var(node);   
                    break;
                // Si el nodo es un bloque Let, evalúa las asignaciones y las operaciones y retorna el resultado de las operaciones
                case "Let":
                    Evaluate(node.Children[0]);
                    dynamic ?result = Evaluate(node.Children[1]);
                    Scopes.Remove(Scopes[Scopes.Count-1]);
                    return result;
                
                //* GEO_WALL_E

                case "point":
                    string name_p = node.Children[0].Value.ToString();
                    Point<T> point = new Point<T>(name_p, FigureColor);
                    Scopes[Scopes.Count - 1].Add(name_p, point);
                    return point;
                
                case "line_d":
                    string name_ld = node.Children[0].Value.ToString();
                    Line<T> line = new Line<T>(name_ld, FigureColor);
                    Scopes[Scopes.Count - 1].Add(name_ld, line);
                    return line;
                
                case "segment_d":
                    string name_sd = node.Children[0].Value.ToString();
                    Segment<T> segment = new Segment<T>(name_sd, FigureColor);
                    Scopes[Scopes.Count - 1].Add(name_sd, segment);
                    return segment;
                
                case "ray_d":
                    string name_rd = node.Children[0].Value.ToString();
                    Ray<T> ray = new Ray<T>(name_rd, FigureColor);
                    Scopes[Scopes.Count - 1].Add(name_rd, ray);
                    return ray;
                
                case "arc_d":
                    string name_ad = node.Children[0].Value.ToString();
                    Arc<T> arc = new Arc<T>(name_ad, FigureColor);
                    Scopes[Scopes.Count - 1].Add(name_ad, arc);
                    return arc;
                
                case "circle_d":
                    string name_cd = node.Children[0].Value.ToString();
                    Circle<T> circle = new Circle<T>(name_cd, FigureColor);
                    Scopes[Scopes.Count - 1].Add(name_cd, circle);
                    return circle;
                
                case "line":
                    string name_l = node.Children[0].Value.ToString();
                    object? p1_l = Evaluate(node.Children[1]);
                    object? p2_l = Evaluate(node.Children[2]);
                    Line<T> line1 = new Line<T>(name_l, FigureColor, (Point<T>)p1_l, (Point<T>)p2_l);
                    Scopes[Scopes.Count - 1].Add(name_l, line1);
                    return line1;
                
                case "segment":
                    string name_s = node.Children[0].Value.ToString();
                    object? p1_s = Evaluate(node.Children[1]);
                    object? p2_s = Evaluate(node.Children[2]);
                    Segment<T> segment1 = new Segment<T>(name_s, FigureColor, (Point<T>)p1_s, (Point<T>)p2_s);
                    Scopes[Scopes.Count - 1].Add(name_s, segment1);
                    return segment1;
                
                case "ray":
                    string name_r = node.Children[0].Value.ToString();
                    object? p1_r = Evaluate(node.Children[1]);
                    object? p2_r = Evaluate(node.Children[2]);
                    Ray<T> ray1 = new Ray<T>(name_r, FigureColor, (Point<T>)p1_r, (Point<T>)p2_r);
                    Scopes[Scopes.Count - 1].Add(name_r, ray1);
                    return ray1;
                
                case "arc":
                    string name_a = node.Children[0].Value.ToString();
                    object? p1_a = Evaluate(node.Children[1]);
                    object? p2_a = Evaluate(node.Children[2]);
                    object? p3_a = Evaluate(node.Children[3]);
                    object? m_a  = Evaluate(node.Children[4]);
                    Arc<T> arc1 = new Arc<T>(name_a, FigureColor, (Point<T>)p1_a, (Point<T>)p2_a, (Point<T>)p3_a, (Measure<T>)m_a);
                    Scopes[Scopes.Count - 1].Add(name_a, arc1);
                    return arc1;
                
                case "circle":
                    string name_c = node.Children[0].Value.ToString();
                    object? p_c = Evaluate(node.Children[1]);
                    object? m_c = Evaluate(node.Children[2]);
                    Circle<T> circle1 = new Circle<T>(name_c, FigureColor, (Point<T>)p_c, (Measure<T>)m_c);
                    Scopes[Scopes.Count - 1].Add(name_c, circle1);
                    return circle1;
                
                // Si el nodo no coincide con ninguno de los anteriores lanza un error
                default:
                    throw new Exception("SEMANTIC ERROR: Unknown operator: " + node.Type);
            }
            return 0;
        }

    #region Auxiliar

        /// <summary>
        /// Metodo para obtener un diccionario con los parametros que se declaran en una funcion
        /// </summary>
        private Dictionary<string,dynamic> Get_Var_Param(Node parameters)
        {
            // Crea un nuevo diccionario para almacenar los parámetros
            Dictionary<string, dynamic> Param = new Dictionary<string, dynamic>();

            // Para cada parámetro, añade su nombre al diccionario con un valor inicial de null
            for(int i=0; i<parameters.Children.Count; i++)
            {
                string ?name = parameters.Children[i].Value.ToString();
                Param.Add(name, null);
            } 
            return Param;
        }


        /// <summary>
        /// Metodo para almacenar y asignar las variables que se declaran en el LET
        /// </summary>
        private void Save_Var(Node Children_assigment_list)
        {
            // Crea un nuevo diccionario para almacenar las variables del bloque Let
            Dictionary<string,dynamic> Var_let_in = new Dictionary<string,dynamic>();
            // Añade todas las variables del ámbito actual al nuevo diccionario
            foreach(string key in Scopes[Scopes.Count - 1].Keys)
            {
                Var_let_in.Add(key, Scopes[Scopes.Count - 1][key]);
            }
            // Para cada asignación en la lista de asignaciones, evalúa el valor y añade la variable al nuevo diccionario
            foreach (Node Child in Children_assigment_list.Children)
            {
                string ?name = Child.Children[0].Value.ToString();
                dynamic ?value = Evaluate(Child.Children[1]);

                // Si el nombre de la variable coincide con el nombre de una función existente, lanza una excepción
                if(Function_Exist(name))
                {
                    Input_Error ("The variable "+ name +" already has a definition as a function in the current context");
                }

                // Si la variable ya existe en el diccionario, actualiza su valor
                if(Var_let_in.ContainsKey(name))
                {
                    Var_let_in[name] = value;
                }

                else
                {
                    Var_let_in.Add(name, value);
                }
            }
            // Añade el nuevo diccionario de variables al ámbito actual
            Scopes.Add(Var_let_in);
        }

        /// <summary>
        /// Metodo que llama a una funcion ya declarada
        /// </summary>
        private int Call_Function(List<Function_B> f,string name, Node param)
        {
            bool is_found = false;
            // Recorre la lista de funciones declaradas
            for(int i=0; i<f.Count; i++)
            {   
                // Si encuentra una función con el mismo nombre
                if(f[i].Name_function == name)
                {
                    is_found = true;
                    // Si la función tiene el mismo número de parámetros
                    if(f[i].variable_param.Count == param.Children.Count )
                    {
                        // Añade todas las variables del ámbito anterior al ámbito actual
                        foreach(string key in Scopes[Scopes.Count - 2].Keys)
                        {
                            Scopes[Scopes.Count - 1].Add(key, Scopes[Scopes.Count - 2][key]);
                        }

                        int count = 0;
                        // Para cada parámetro de la función, actualiza su valor en el ámbito actual
                        foreach(string key in f[i].variable_param.Keys)
                        {
                            f[i].variable_param[key] = param.Children[count].Value;
                            if(Scopes[Scopes.Count - 1].ContainsKey(key))
                            {
                                Scopes[Scopes.Count - 1][key] = Evaluate((Node)param.Children[count].Value);
                                count++;
                            }
                            else
                            {
                                Scopes[Scopes.Count - 1].Add(key, Evaluate((Node)param.Children[count].Value));
                                count++;
                            }
                        }

                        return i; 
                    }
                    // Si no coincide el numero de parametros de la funcion con los introducidos a la hora de llamarla
                    //lanza un error
                    else
                    {
                        Input_Error ("Function "+ name + " receives " +f[i].variable_param.Count+" argument(s), but "+ param.Children.Count +" were given.");
                    }
                }
            }
            // Si no se encuentra la funcion, no se ha declarado, lanza un error
            if(!is_found)
            {
                Input_Error ("The function "+ name +" has not been declared");
            }

            return -1;
        }


        /// <summary>
        /// Metodo que verifica si la funcion existe declarada
        /// </summary>
        private bool Function_Exist(string? name)
        {
            // Recorre la lista de funciones declaradas
            foreach( Function_B b in functions_declared)
            {
                // Si encuentra una función con el mismo nombre, retorna true
                if(b.Name_function == name)
                {
                    return true;
                }
            }
            // Si no encuentra ninguna función con ese nombre, retorna false
            return false;
        }

        /// <summary>
        /// Método que lanza una excepción con un mensaje de error semantico
        /// </summary>
        private void Input_Error(string error )
        {
            throw new Exception("SEMANTIC ERROR: " + error);
        }

        /// <summary>
        /// Metodo que verifica si dos valores son del mismo tipo (del tipo desperado)
        /// </summary>
        private void Type_Expected(object value1, object value2 , string type, string op)
        {
            // Si los valores son del tipo esperado, no hace nada
            if(value1 is double && value2 is double && type == "number")
            {
                return;
            }
            else if(value1 is string && value2 is string && type == "string")
            {
                return;
            }
            else if(value1 is bool && value2 is bool && type == "boolean")
            {
                return;
            }
            // Si los valores no son del tipo esperado, lanza una excepción
            else
            {
                Input_Error("Operator \'"+ op+"\' cannot be used between \'" + Identify(value1) +"\' and \'"+ Identify(value2) +"\'");
            }
        }

        /// <summary>
        /// Metodo que dependiendo del tipo esperado, verifica si el valor es de ese tipo
        /// </summary>
        private void Expected(object value1, string type, string express)
        {
            string v1_type = Identify(value1);

            if(v1_type == type) return;

            //switch(type)
            //{
            //    case "string":
            //        if(value1 is string)
            //            return;
            //        break;
            //    case "number":
            //        if(value1 is double)
            //            return;
            //        break;
            //    case "boolean":
            //        if(value1 is bool)
            //            return;
            //        break;
            //    default:
            //        Input_Error("The \'"+ express +"\' expression must receive type \'" + value1 +"\'");
            //        break;
            //}

            Input_Error("The \'"+ express +"\' expression must receive type \'" + type +"\'");
           
        }

        private string Identify(object value)
        {
            if(value is string) return "string";
            if(value is double) return "number";
            if(value is bool) return "bool";
            return "Unknown";
        }
        
    }
    #endregion
}