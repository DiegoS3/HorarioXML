using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HorarioXML
{
    class Utiles
    {
        private Boolean errorProviderCB(ComboBox cb, ErrorProvider ep, Label lbl)
        {
            bool error = false;

            if (cb.SelectedIndex < 0)
            {
                error = true;
                ep.SetError(lbl, "Debes seleccionar un valor");
            }

            return error;
        }

        private Boolean errorProviderLB(ListBox lb, ErrorProvider ep, Label lbl)
        {
            bool error = false;

            if (lb.SelectedIndex < 0)
            {
                error = true;
                ep.SetError(lbl, "Debes seleccionar un valor");
            }

            return error;
        }

        public Boolean seleccionadoLB(ListBox[] listaLB, ErrorProvider erpError, Label[] lbl)
        {
            bool todo = false;
            int contLB = 0;

            erpError.Clear();

            for (int i = 0; i < listaLB.Length; i++)
            {
                if (!errorProviderLB(listaLB[i], erpError, lbl[i]))
                {
                    contLB++;
                    if (contLB == listaLB.Length) { todo = true; }
                }
            }

            return todo;
        }

        public Boolean seleccionadoCB(ComboBox[] listaCB, ErrorProvider erpError, Label[] lbl)
        {
            bool todo = false;
            int contCB = 0;

            erpError.Clear();

            for (int i = 0; i < listaCB.Length; i++)
            {
                if (!errorProviderCB(listaCB[i], erpError, lbl[i]))
                {
                    contCB++;
                    if (contCB == listaCB.Length) { todo = true; }
                }
            }

            return todo;
        }

        private void tablaFija(DataGridView dgvHorario)
        {
            foreach (DataGridViewColumn column in dgvHorario.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
                column.Resizable = DataGridViewTriState.False;
            }
        }

        public void datosIniciales(DataGridView dgvHorario)
        {
            string[] horas = new string[] { "8:30 - 9:25", "9:25 - 10:20", "10:20 - 11:15",
                "11:45 - 12:40", "12:40 - 13:35", "13:35 - 14:30"};

            for (int i = 0; i < horas.Length; i++) { dgvHorario.Rows.Add(horas[i]); }
        }

        public void crearTabla(DataSet dsDatos, DataGridView dgvHorario)
        {
            dsDatos.Reset();
            dsDatos.Tables.Add(new DataTable("horario"));
            dsDatos.Tables[0].Columns.Add("HORA");
            dsDatos.Tables[0].Columns.Add("LUNES");
            dsDatos.Tables[0].Columns.Add("MARTES");
            dsDatos.Tables[0].Columns.Add("MIERCOLES");
            dsDatos.Tables[0].Columns.Add("JUEVES");
            dsDatos.Tables[0].Columns.Add("VIERNES");
            datosIniciales(dgvHorario);
            dgvHorario.DataSource = dsDatos.Tables[0];
            tablaFija(dgvHorario);

        }

        public void addToolTip(DataGridView dgvHorario, ComboBox cmbHora, ListBox lsbModulo, ComboBox cmbDia)
        {
            string[] tooltipSegundoDam = { "Joaquín - Aula 209", "José Alberto - Aula 209", "Inmaculada - Aula 209",
                "María - Aula 209", "Fernando - Aula 209", "José Luis - Aula 209" };
            string[] tooltipSegundoDaw = { "Inmaculada - Aula 206", "Fernando - Aula 206", "María - Aula 209",
                "José Luis - Aula 209",  "Diego - Aula 209" };
            dgvHorario.Rows[cmbHora.SelectedIndex].Cells[cmbDia.SelectedIndex + 1].ToolTipText = tooltipSegundoDam[lsbModulo.SelectedIndex];
        }

        public void deleteToolTip(DataGridView dgvHorario, ComboBox cmbHora, ListBox lsbModulo, ComboBox cmbDia)
        {
            dgvHorario.Rows[cmbHora.SelectedIndex].Cells[cmbDia.SelectedIndex + 1].ToolTipText = null;
        }


        public void agregarDato(DataGridView dgvHorario, ComboBox cmbHora, ListBox lsbModulo, ComboBox cmbDia, ListBox lsbCiclo)
        {
            string ciclo = lsbCiclo.SelectedItem.ToString();
            string modulo = lsbModulo.SelectedItem.ToString();

            dgvHorario.Rows[cmbHora.SelectedIndex].Cells[cmbDia.SelectedIndex + 1].Value = modulo;
            addToolTip(dgvHorario, cmbHora, lsbModulo, cmbDia);

        }

        public void cambiarAsig(ListBox lsbModulo, ListBox lsbCiclo, ComboBox cmbCurso)
        {
            string[] segundoDam = {"ACCESO A DATOS", "DESARROLLO DE INTERFACES", "SISTEMAS DE GESTIÓN EMPRESARIAL",
                "EMPRESA E INICIATIVA EMPRENDEDORA", "PROG. DE SERVICIOS Y PROCESOS", "PROG. MULTIMEDIA Y DISPOSITIVOS MÓVILES"};
            string[] segundoDaw = { "DESARROLLO WEB CLIENTE", "DESARROLLO WEB SERVIDOR", "EMPRESA E INICIATIVA EMPRENDEDORA",
                "DESPLIEGUE DE APLICACIONES WEB", "DISEÑO INTERFACES WEB"};
            string[] primero = {"PROGRMACIÓN", "BASE DE DATOS", "ENTORNOS DE DESARROLLO", "SISTEMAS INFORMATICOS",
                "LENGUAJE DE MARCAS", "FOL", "INGLÉS"};

            if (lsbCiclo.SelectedItem.ToString().Equals("DAM"))
            {
                if (cmbCurso.SelectedItem != null)
                {
                    if (cmbCurso.SelectedItem.ToString().Equals("Primero"))
                    {
                        lsbModulo.Items.Clear();
                        for (int i = 0; i < primero.Length; i++)
                        {
                            lsbModulo.Items.Add(primero[i]);
                        }
                    }
                    else
                    {
                        lsbModulo.Items.Clear();
                        for (int i = 0; i < segundoDam.Length; i++)
                        {
                            lsbModulo.Items.Add(segundoDam[i]);
                        }
                    }
                }                

            }
            else if (lsbCiclo.SelectedItem.ToString().Equals("DAW"))
            {
                if (cmbCurso.SelectedItem != null)
                {
                    if (cmbCurso.SelectedItem.ToString().Equals("Primero"))
                    {
                        lsbModulo.Items.Clear();
                        for (int i = 0; i < primero.Length; i++)
                        {
                            lsbModulo.Items.Add(primero[i]);
                        }
                    }
                    else
                    {
                        lsbModulo.Items.Clear();
                        for (int i = 0; i < segundoDaw.Length; i++)
                        {
                            lsbModulo.Items.Add(segundoDaw[i]);
                        }
                    }
                }
            }

        }


    }
}
