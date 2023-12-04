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
        RESTORE, //! IMPLEMENTAR!
        DRAW, //! IMPLEMENTAR!
        COLOR, //! IMPLEMENTAR!
        COUNT, //IMPLEMENTAR!
        
        //funciones que devuelve una secuencia G#
        SEQUENCE, //en codigo: point sequence etc IMPLEMENTAR!

        //variables y operadores G#
        UNDERSCORE, //_ IMPLEMENTAR!
        UNDEFINED, //IMPLEMENTAR!
        INFINITE, //token que se usa en declaracion de secuencias infinitas como {a...} o {a...b} IMPLEMENTAR!
        //REVISAR EL OPERADOR + YA QUE EN G# SE USA PARA CONCATENAR SECUENCIAS Y PROBABLEMENTE STRINGS Y OTROS
        COLOR_RANGE, //colores blue red etc IMPLEMENTAR!

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