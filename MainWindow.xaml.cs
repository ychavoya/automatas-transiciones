using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using AutomataTransicion.Procesos;

namespace AutomataTransicion
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Datos para el proceso
        private Automata AFND { get; set; }
        
        public MainWindow()
        {
            InitializeComponent();
            AFND = new Automata();
        }

        #region UI
        private void SetDefaultButton(Button button)
        {
            List<Button> buttons = new List<Button>()
            {
                btnNewEntrada,
                btnNewEstado,
                btnConvertir,
                btnNewTransicion
            };
            foreach(var btn in buttons)
            {
                 btn.IsDefault = btn == button;
            }
        }

        private void EnableTransiciones()
        {
            if(AFND.Estados.Count > 0 && AFND.Entradas.Count > 0)
            {
                cbOrigen.IsEnabled = true;
                cbDestino.IsEnabled = true;
                cbEntrada.IsEnabled = true;
            }
        }
        #endregion

        #region Buttons

        private void Event_NewEstado(object sender, RoutedEventArgs e)
        {
            var nombre = textEstado.Text.Trim();
            if(nombre.Length == 0)
            {
                return;
            }
            if (AFND.Estados.Find(x => x.Nombre == nombre) != null)
            {
                textEstado.Text = string.Empty;
                checkEstado.IsChecked = false;
                textEstado.Focus();

                return;
            }

            var estado = new Estado(checkEstado.IsChecked == true) { Nombre = nombre };
            
            // Agregar tanto al modelo de datos como a la interfaz
            AFND.Estados.Add(estado);
            lbEstados.Items.Add(estado);
            cbInicial.Items.Add(estado);
            cbOrigen.Items.Add(estado);
            cbDestino.Items.Add(estado);
            cbInicial.IsEnabled = true;
            EnableTransiciones();

            textEstado.Text = string.Empty;
            checkEstado.IsChecked = false;
            textEstado.Focus();
        }

        private void Event_NewEntrada(object sender, RoutedEventArgs e)
        {
            var entrada = textEntrada.Text;
            if(entrada.Length == 0)
            {
                return;
            }
            if(AFND.Entradas.Find(x => x == entrada) != null)
            {
                textEntrada.Text = string.Empty;
                textEntrada.Focus();
                btnNewEntrada.IsDefault = true;
                return;
            }

            AFND.Entradas.Add(entrada);
            lbEntradas.Items.Add(entrada);
            cbEntrada.Items.Add(entrada);
            EnableTransiciones();

            textEntrada.Text = string.Empty;
            textEntrada.Focus();
            btnNewEntrada.IsDefault = true;
        }

        private void cbInicial_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbInicial.SelectedIndex == -1)
            {
                return;
            }
            AFND.EstadoInicial = (Estado)cbInicial.SelectedItem;

            lbInicial.Content = $"Inicial --->  {AFND.EstadoInicial.Nombre}";
        }

        private void Event_NewTransicion(object sender, RoutedEventArgs e)
        {
            if (cbOrigen.SelectedIndex == -1 || cbDestino.SelectedIndex == -1 || cbEntrada.SelectedIndex == -1)
            {
                return;
            }

            Estado origen = (Estado)cbOrigen.SelectedItem;
            Estado destino = (Estado)cbDestino.SelectedItem;
            string entrada = (string)cbEntrada.SelectedItem;

            // Verificar que la entrada no esté repetida
            foreach (var t in AFND.Transiciones)
            {
                if (t.Origen == origen && t.Destino == destino && t.Entrada == entrada)
                {
                    return;
                }
            }

            var transicion = new Transicion
            {
                Origen = origen,
                Destino = destino,
                Entrada = entrada
            };

            AFND.Transiciones.Add(transicion);
            lbTransiciones.Items.Add(transicion);


        }

        private void Event_Convertir(object sender, RoutedEventArgs e)
        {
            try
            {
                AFND.Validar();
                dataOriginal.ItemsSource = GeneradorTabla.Generar(AFND).DefaultView;
                var AFD = GeneradorEquivalencia.GenerarAFD(AFND);
                dataNew.ItemsSource = GeneradorTabla.Generar(AFD).DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error generando equivalencia");
            }
        }

        private void Event_Eliminar(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("¿Seguro que desea borrar todo?", "Advertencia", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {

                AFND = new Automata();

                cbOrigen.IsEnabled = false;
                cbDestino.IsEnabled = false;
                cbEntrada.IsEnabled = false;
                cbInicial.IsEnabled = false;

                lbEntradas.Items.Clear();
                lbEstados.Items.Clear();
                lbTransiciones.Items.Clear();

                lbInicial.Content = "";

                cbOrigen.Items.Clear();
                cbDestino.Items.Clear();
                cbEntrada.Items.Clear();
                cbInicial.Items.Clear();

            }
        }

        #endregion

        #region Focus
        private void textEstado_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            SetDefaultButton(btnNewEstado);
        }

        private void textEntrada_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            SetDefaultButton(btnNewEntrada);
        }
        private void Event_TransicionFocus(object sender, RoutedEventArgs e)
        {
            SetDefaultButton(btnNewTransicion);
        }
        #endregion
    }
}
