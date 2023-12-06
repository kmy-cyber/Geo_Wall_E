namespace INTERPRETE_C__to_HULK
{
    public enum TokenType
    {
        //?TIPO DE DATOS
        NUMBER,
        STRING,
        BOOLEAN,

        //?OPERADORES
        OPERATOR,
        EQUAL,
        DO,

        //PUNTUADORES
        L_PHARENTESYS,
        R_PHARENTESYS,
        L_KEY, //LLAVE QUE DEFINE EL INICIO DE UNA SECUENCIA IMPLEMENTAR!
        R_KEY,  //LLAVE QUE DEFINA EL FINAL DE UNA SECUENCIA IMPLEMENTAR!
        PRINT,
        COMMA,
        D_COMMA,
        COMMILLAS,

        //?Own Words
        LET,
        IN,
        IF,
        THEN,
        ELSE,
        TRUE,
        FALSE,
        FUNCTION,

        //? Reserved Word
        COS,
        SEN,
        LOG,

        //? Geometric symbols
        POINT,
        LINE,
        SEGMENT,
        RAY,
        CIRCLE,
        ARC,

        // funciones G#
        MEASURE,
        IMPORT,
        RESTORE, 
        DRAW, 
        COLOR, 
        COUNT, //IMPLEMENTAR!
        
        //funciones que devuelve una secuencia G#
        SEQUENCE, //en codigo: point sequence etc IMPLEMENTAR!
        INTERSECT, //implementar!
        POINTS, //IMPLEMENTAR!
        SAMPLES, //IMPLEMENTAR!
        RANDOMS, //IMPLEMENTAR!

        //variables y operadores G#
        UNDERSCORE, //_ IMPLEMENTAR!
        UNDEFINED, //IMPLEMENTAR!
        INFINITE, //token que se usa en declaracion de secuencias infinitas como {a...} o {a...b} IMPLEMENTAR!
        //REVISAR EL OPERADOR + YA QUE EN G# SE USA PARA CONCATENAR SECUENCIAS Y PROBABLEMENTE STRINGS Y OTROS
        COLOR_RANGE, //colores blue red etc 

        //?
        VARIABLE,
        EOF
    }

    // Definiendo objeto token
    public class Token { 
        public TokenType Type { get; } //tipo de token
        public object Value { get; } //valor del token

        public Token(TokenType type, object value) {
            Type = type;
            Value = value;
        }

        //public override string ToString() {
        //    return $"Token({Type}, {Value})";
        //}
    }

}