using System;
using System.Collections.Generic;

public class Estado
{
    public string Nombre { get; set; }
    public bool Aceptacion { get; protected set; }

    public Estado(bool aceptacion)
    {
        Aceptacion = aceptacion;
    }

    public override string ToString()
    {
        return (Aceptacion? "* ": string.Empty) + Nombre;
    }
}