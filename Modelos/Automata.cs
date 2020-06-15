using System;
using System.Collections.Generic;
using System.Text;

namespace AutomataTransicion
{
    class Automata
    {
        public List<Estado> Estados { get; set; }
        public List<string> Entradas { get; set; }
        public Estado EstadoInicial { get; set; }
        public List<Transicion> Transiciones { get; set; }
        public TipoAutomata Tipo { get; protected set; }

        public Automata(TipoAutomata tipo = TipoAutomata.AFND)
        {
            Estados = new List<Estado>();
            Entradas = new List<string>();
            EstadoInicial = null;
            Transiciones = new List<Transicion>();
            Tipo = tipo;
        }

        public void Validar()
        {
            bool hasFinal = false;
            foreach (var estado in Estados)
            {
                if (estado.Aceptacion) hasFinal = true;
            }
            if (!hasFinal)
            { 
                throw new Exception("El autómata debe tener al menos un estado final");
            }

            if (EstadoInicial == null)
            {
                throw new Exception("El autómata debe tener un estado inicial");
            }

            if (Transiciones.Count == 0)
            {
                throw new Exception("El autómata debe tener al menos una transición");
            }
        }
    }

    enum TipoAutomata
    {
        AFD,
        AFND
    }
}
