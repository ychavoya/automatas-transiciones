using System;

public class Transicion
{

	public Estado Origen { get; set; }
	public string Entrada { get; set; }
	public Estado Destino { get; set; }

	public Transicion()
	{
	}

    public override string ToString()
    {
		return $"{Origen} --[ {Entrada} ]-> {Destino}";
    }

	public bool IsEqual(Transicion otra)
    {
		return Origen == otra.Origen && Entrada == otra.Entrada && Destino == otra.Destino;
    }
}
