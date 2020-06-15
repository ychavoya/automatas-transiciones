using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;

namespace AutomataTransicion
{
    static class GeneradorTabla
    {
        public static DataTable Generar(Automata automata)
        {
            var data = new DataTable(automata.Tipo.ToString());
            DataColumn column;
            DataRow row;

            column = new DataColumn("Estado", typeof(Estado));
            column.Unique = true;
            data.Columns.Add(column);

            foreach (var entrada in automata.Entradas)
            {
                column = new DataColumn(entrada, typeof(string));
                data.Columns.Add(column);
            }

            foreach (var estado in automata.Estados)
            {
                row = data.NewRow();
                row["Estado"] = estado;

                var transiciones = automata.Transiciones.Where(x => x.Origen == estado);
                var transicionesDict = new Dictionary<string, string>();
                foreach (var t in transiciones)
                {
                    if (transicionesDict.TryGetValue(t.Entrada, out string val))
                    {
                        transicionesDict[t.Entrada] = val + "," + t.Destino.Nombre;
                    }
                    else
                    {
                        transicionesDict[t.Entrada] = t.Destino.Nombre;
                    }
                }

                foreach (var t in transicionesDict)
                {
                    row[t.Key] = t.Value;
                }

                data.Rows.Add(row);

            }

            return data;

        }
    }
}
