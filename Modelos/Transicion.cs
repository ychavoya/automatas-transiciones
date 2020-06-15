using System;

public class Transicion
{

	public Estado Origen { get; set; }
	public string Entrada { get; set; }
	public Estado Destino { get; set; }

	public Transicion()
	{
	}
}
