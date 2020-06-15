using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

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
            AFD.Entradas = AFND.Entradas;

            AFD.EstadoInicial = AFND.EstadoInicial;
            AFD.Estados.AddRange(AFND.Estados);

            var transiciones = new List<Transicion>();


            // Generar estados compuestos
            GenerarEstadosCompuestos(ref AFD, AFND.Transiciones);

            // Agregar estado de error para estados sin una transición
            Estado error = new Estado(false) { Nombre = "∅" };
            AFD.Estados.Add(error);
            foreach (var entrada in AFD.Entradas)
            {
                AFD.Transiciones.Add(new Transicion { Origen = error, Destino = error, Entrada = entrada });
            }
            foreach(var estado in AFD.Estados)
            {
                var t = AFD.Transiciones.Where(x => x.Origen == estado);
                foreach(var entrada in AFD.Entradas)
                {
                    if (t.Where(x => x.Entrada == entrada).Count() == 0)
                    {
                        AFD.Transiciones.Add(new Transicion { Origen = estado, Destino = error, Entrada = entrada });
                    }
                }
            }

            // Eliminar Estados inaccesibles
            // Evaluar autómata con todas las posibilidades
            List<Estado> accedidos = new List<Estado>(), eliminados = new List<Estado>();
            EvaluarAccedidos(AFD.EstadoInicial, AFD, ref accedidos);
            foreach (var estado in AFD.Estados)
            {
                if(accedidos.Find(x => x == estado) == null)
                {
                    eliminados.Add(estado);
                    AFD.Transiciones.RemoveAll(x => x.Origen == estado);

                }
            }
            foreach (var estado in eliminados)
            {
                AFD.Estados.Remove(estado);
            }

            // Eliminar transiciones repetidas
            var repetidas = AFD.Transiciones.GroupBy(x => new { x.Origen, x.Destino, x.Entrada },
                (x, g) => new
                {
                    x.Origen,
                    x.Destino,
                    x.Entrada,
                    Transiciones = g,
                }).Where(x => x.Transiciones.Count() > 1);
            foreach(var r in repetidas)
            {
                AFD.Transiciones.Remove(
                    AFD.Transiciones.First(x => x.Origen == r.Origen && x.Destino == r.Destino && x.Entrada == r.Entrada)
                );
            }


            return AFD;
        }

        private static void GenerarEstadosCompuestos(ref Automata AFD, List<Transicion> TransicionesAFND)
        {
            var tCompuestas = TransicionesAFND.GroupBy(t => new { t.Origen, t.Entrada },
                (key, group) => new
                {
                    key.Origen,
                    key.Entrada,
                    Destinos = group.Select(x => x.Destino)
                }); //.Where(x => x.Destinos.Count() > 1);
            foreach(var tc in tCompuestas)
            {
                if(tc.Destinos.Count() > 1)
                {
                    var strNombre = string.Empty;
                    bool isFinal = false;
                    var destinos = new List<Estado>();
                    foreach (var d in tc.Destinos.OrderBy(x => x.Nombre)) {
                        if (destinos.Contains(d)) continue;
                        destinos.Add(d);
                        strNombre += d.Nombre;
                        if (d.Aceptacion) isFinal = true;
                    }
                    Estado estado = AFD.Estados.Find(x => x.Nombre == strNombre);
                    if(estado == null)
                    {
                        estado = new EstadoCompuesto(isFinal) {
                            Nombre = strNombre,
                            Estados = tc.Destinos.ToList()
                        };
                        AFD.Estados.Add(estado);
                        var nt = new Transicion
                        {
                            Origen = tc.Origen,
                            Entrada = tc.Entrada,
                            Destino = estado,
                        };
                        if(AFD.Transiciones.Find(x =>
                            x.Origen == nt.Origen &&
                            x.Destino == nt.Destino &&
                            x.Entrada == nt.Entrada
                        ) == null)
                        {
                            AFD.Transiciones.Add(nt);
                        }

                        foreach (var e in ((EstadoCompuesto)estado).Estados)
                        {
                            var transiciones = new List<Transicion>();
                            foreach (var t in TransicionesAFND.Where(x => x.Origen == e))
                            {
                                var nti  = new Transicion
                                {
                                    Origen = estado,
                                    Entrada = t.Entrada,
                                    Destino = t.Destino
                                };
                                if(transiciones.Find(x =>
                                    x.Origen == nti.Origen &&
                                    x.Destino == nti.Destino &&
                                    x.Entrada == nti.Entrada
                                ) == null)
                                {
                                    transiciones.Add(nti);
                                }
                            }
                            TransicionesAFND.AddRange(transiciones);
                        }


                        GenerarEstadosCompuestos(ref AFD, TransicionesAFND);

                        
                    } else
                    {
                        if(estado.GetType() == typeof(EstadoCompuesto))
                        {
                            var nt = new Transicion
                            {
                                Origen = tc.Origen,
                                Entrada = tc.Entrada,
                                Destino = estado
                            };
                            if (AFD.Transiciones.Find(x =>
                                 x.Origen == nt.Origen &&
                                 x.Destino == nt.Destino &&
                                 x.Entrada == nt.Entrada
                            ) == null)
                            {
                                AFD.Transiciones.Add(nt);
                            }
                        }
                    }
                } 
                else
                {
                    if(tc.Destinos.Count() == 1)
                    {
                        var destino = tc.Destinos.First();
                        if(AFD.Estados.Find(x => x == destino) == null)
                        {
                            AFD.Estados.Add(destino);
                        }
                        var nt = new Transicion {
                            Origen = tc.Origen,
                            Destino = destino,
                            Entrada = tc.Entrada
                        };
                        if (AFD.Transiciones.Find(x =>
                             x.Origen == nt.Origen &&
                             x.Destino == nt.Destino &&
                             x.Entrada == nt.Entrada
                        ) == null)
                        {
                            AFD.Transiciones.Add(nt);
                        }
                    }
                }

            }

        }

        private static void EvaluarAccedidos(Estado e, Automata AFD, ref List<Estado> accedidos)
        {
            if(accedidos.Find(x => x == e) != null)
            {
                return;
            }
            
            accedidos.Add(e);
            
            foreach(var t in AFD.Transiciones.FindAll(x => x.Origen == e))
            {
                EvaluarAccedidos(t.Destino, AFD, ref accedidos);
            }
        } 
    }
}
