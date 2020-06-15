using System;
using System.Collections.Generic;
using System.Text;

namespace AutomataTransicion.Procesos
{
    static class GeneradorEquivalencia
    {
        public static Automata GenerarAFD(Automata AFND)
        {
            if(AFND.Tipo != TipoAutomata.AFND)
            {
                throw new Exception("Tratando de convertir AFD a AFD");
            }

            Automata AFD = new Automata(TipoAutomata.AFD);

            

            return AFD;
        }
    }
}
