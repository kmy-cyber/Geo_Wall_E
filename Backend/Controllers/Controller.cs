using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

using INTERPRETE_C__to_HULK;
using G_Wall_E;

namespace Geo_Wall_E.Controllers
{
    [Route("api/walle")]
    [ApiController]
    public class TextController : ControllerBase
    {
        [HttpPost]
        public ActionResult<IEnumerable<DrawableProperties>> Post([FromBody] string value)
        {
            Semantic_Analyzer sa = new Semantic_Analyzer();

             // Lexer recibe el input (s) y crea la lista de tokens
            Lexer T =  new Lexer(value);

            // Se obtiene la lista de tokens que hace el Lexer
            List<Token> TS = T.Tokens_sequency;
            
            // Parser recibe la lista de tokens (TS) y crea el arbol de sintaxis
            Parser P = new Parser(TS);
            
            // Se obtiene el arbol (N)
            Node N = P.Parse();
            
            // El metodo Read_Parser del Analizador Semantico y recibe el arbol 
            sa.Read_Parser(N);
            
            // Recibe el arbol, lo analiza y devuelve el resultado
            List<IDrawable> drawables = sa.Choice(N);
            List<DrawableProperties> respuesta = new List<DrawableProperties>();

            foreach (var item in drawables)
            {
                respuesta.Add(item.Export());
            }

            return Ok(respuesta);
        }
    }
}
