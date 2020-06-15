using System;
using System.Collections.Generic;
using System.Text;

public class EstadoCompuesto : Estado
{
    public List<Estado> Estados { get; set; }
    public EstadoCompuesto(bool aceptacion) : base(aceptacion)
    {
        Estados = new List<Estado>();
    }
}