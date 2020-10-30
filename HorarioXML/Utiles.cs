using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace HorarioXML
{
    class Utiles
    {
        //Metodo que muestra el ErroProvider si no esta seleccionado el ComboBox indicado
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

        //Metodo que muestra el ErroProvider si no esta seleccionado el ListBox indicado
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

        //Metodo que comprueba que este seleccionado todos los campos de los ListBox
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

        //Metodo que comprueba que este seleccionado todos los campos de los ComboBox
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

        //Metodo que pone las horas en la tabla
        public void datosIniciales(DataSet dsDatos)
        {
            string[] horas = new string[] { "8:30 - 9:25", "9:25 - 10:20", "10:20 - 11:15",
                "11:45 - 12:40", "12:40 - 13:35", "13:35 - 14:30"};

            for (int i = 0; i < horas.Length; i++) { dsDatos.Tables[0].Rows.Add(horas[i]); }
        }

        //Metodo que crea la tabla inicial
        public void crearTabla(DataSet dsDatos, DataGridView dgvHorario)
        {
            dsDatos.Clear();
            dsDatos.Reset();
            dsDatos.Tables.Add(new DataTable("horario"));
            dsDatos.Tables[0].Columns.Add("HORA");
            dsDatos.Tables[0].Columns.Add("LUNES");
            dsDatos.Tables[0].Columns.Add("MARTES");
            dsDatos.Tables[0].Columns.Add("MIERCOLES");
            dsDatos.Tables[0].Columns.Add("JUEVES");
            dsDatos.Tables[0].Columns.Add("VIERNES");
            datosIniciales(dsDatos);
            dgvHorario.DataSource = dsDatos.Tables[0];
            tablaFija(dgvHorario);

        }

        //metodo que añade los tooltip a cada celda del DataGridView dependiendo del curso y ciclo seleccionado
        public void addToolTip(DataGridView dgvHorario, ComboBox cmbCurso, ListBox lsbCiclo, ListBox lsbModulo)
        {
            string[] tooltipSegundoDam = { "Joaquín - Aula 209", "José Alberto - Aula 209", "Inmaculada - Aula 209",
                "María - Aula 209", "Fernando - Aula 209", "José Luis - Aula 209" };
            string[] tooltipSegundoDaw = { "Inmaculada - Aula 206", "Fernando - Aula 206", "María - Aula 209",
                "José Luis - Aula 209",  "Diego - Aula 209" };
            string[] tooltipPrimeroDaw = { "Fernando - Aula 212","Alicia - Aula 212",  "Fernando - Aula 212",
                "Jaime - Aula 212",  "Alicia - Aula 212", "Maria - Aula 212", "Teresa - Aula 212" };
            string[] tooltipPrimeroDam = { "Marciano - Aula 213","Alicia - Aula 213",  "Marciano - Aula 213",
                "Jaime - Aula 213",  "Alicia - Aula 213", "Maria - Aula 213", "Teresa - Aula 213" };

            int op = comprobarCursoCiclo(lsbModulo, lsbCiclo, cmbCurso);

            switch (op)
            {
                case 1: //PrimeroDAM

                    dgvHorario.CurrentCell.ToolTipText = tooltipPrimeroDam[lsbModulo.SelectedIndex];
                    break;

                case 2: //SegundoDAM

                    dgvHorario.CurrentCell.ToolTipText = tooltipSegundoDam[lsbModulo.SelectedIndex];
                    break;

                case 3: //PrimeroDAW

                    dgvHorario.CurrentCell.ToolTipText = tooltipPrimeroDaw[lsbModulo.SelectedIndex];
                    break;

                case 4: //SegundoDAW

                    dgvHorario.CurrentCell.ToolTipText = tooltipSegundoDaw[lsbModulo.SelectedIndex];
                    break;

                default:
                    break;
            }
            
        }

        //MEtodo que elimina el tooltip del DataGridView en la celda actual
        public void deleteToolTip(DataGridView dgvHorario, ComboBox cmbHora, ListBox lsbModulo, ComboBox cmbDia)
        {
            dgvHorario.CurrentCell.ToolTipText = null;
        }

        //Metodo que agrega la asignatura y el tooltip a la celda actual del DataGridView
        public void agregarDato(DataGridView dgvHorario, ComboBox cmbCurso, ListBox lsbModulo, ListBox lsbCiclo)
        {
            string modulo = lsbModulo.Text;

            dgvHorario.CurrentCell.Value = modulo;
            addToolTip(dgvHorario, cmbCurso, lsbCiclo, lsbModulo);
        }

        //Metodo que comprueba el curso y el ciclo que se ha seleccionado
        private int comprobarCursoCiclo(ListBox lsbModulo, ListBox lsbCiclo, ComboBox cmbCurso)
        {
            int curso = 0;

            if (lsbCiclo.SelectedItem != null && cmbCurso.SelectedItem != null)
            {
                if (lsbCiclo.SelectedItem.ToString().Equals("DAM"))
                {
                    if (cmbCurso.SelectedItem.ToString().Equals("Primero")){ curso = 1; }
                    else { curso = 2; }

                }
                else if (lsbCiclo.SelectedItem.ToString().Equals("DAW"))
                {
                    if (cmbCurso.SelectedItem.ToString().Equals("Primero")) { curso = 3; }
                    else { curso = 4; }
                }
            }

            return curso;
        }

        //Metodo que cambias las asignaturas del ListBox de modulo dependiendo del ciclo y el curso seleccionado
        public void cambiarAsig(ListBox lsbModulo, ListBox lsbCiclo, ComboBox cmbCurso)
        {
            string[] segundoDam = {"ACCESO A DATOS", "DESARROLLO DE INTERFACES", "SISTEMAS DE GESTIÓN EMPRESARIAL",
                "EMPRESA E INICIATIVA EMPRENDEDORA", "PROG. DE SERVICIOS Y PROCESOS", "PROG. MULTIMEDIA Y DISPOSITIVOS MÓVILES"};
            string[] segundoDaw = { "DESARROLLO WEB CLIENTE", "DESARROLLO WEB SERVIDOR", "EMPRESA E INICIATIVA EMPRENDEDORA",
                "DESPLIEGUE DE APLICACIONES WEB", "DISEÑO INTERFACES WEB"};
            string[] primero = {"PROGRAMACIÓN", "BASE DE DATOS", "ENTORNOS DE DESARROLLO", "SISTEMAS INFORMATICOS",
                "LENGUAJE DE MARCAS", "FOL", "INGLÉS"};

            int op = comprobarCursoCiclo(lsbModulo, lsbCiclo, cmbCurso);
            switch (op)
            {

                case 1: //PrimeroDAM
                    lsbModulo.Items.Clear();
                    for (int i = 0; i < primero.Length; i++)
                    {
                        lsbModulo.Items.Add(primero[i]);
                    }
                    break;

                case 2: //SegundoDAM

                    lsbModulo.Items.Clear();
                    for (int i = 0; i < segundoDam.Length; i++)
                    {
                        lsbModulo.Items.Add(segundoDam[i]);
                    }
                    break;

                case 3: //PrimeroDAW

                    lsbModulo.Items.Clear();
                    for (int i = 0; i < primero.Length; i++)
                    {
                        lsbModulo.Items.Add(primero[i]);
                    }
                    break;

                case 4: //SegundoDAW

                    lsbModulo.Items.Clear();
                    for (int i = 0; i < segundoDaw.Length; i++)
                    {
                        lsbModulo.Items.Add(segundoDaw[i]);
                    }
                    break;

                default:
                    break;
            }
        }

        //Metodo que generar el XML que guardaremos
        public void generarXML(SaveFileDialog sfdGuardar, DataGridView dgvHorario)
        {
            if (sfdGuardar.ShowDialog() == DialogResult.OK)
            {
                //se crea el elemento raíz y se asocia al documento
                XmlDocument xDoc = new XmlDocument();
                XmlElement elementoRaiz = xDoc.CreateElement(string.Empty, "horario", string.Empty);
                xDoc.AppendChild(elementoRaiz);

                for (int i = 0; i < dgvHorario.Rows.Count; i++)
                {
                    XmlElement xHora = xDoc.CreateElement(string.Empty, "hora", string.Empty);
                    //El atributo id con la hora se puede sacar de la primera columna del DataGridView
                    xHora.SetAttribute("id", dgvHorario.Rows[i].Cells[0].Value.ToString());

                    for (int j = 1; j < dgvHorario.Columns.Count; j++)
                    {
                        XmlElement xDia = xDoc.CreateElement(string.Empty, "dia", string.Empty);
                        XmlElement xAsignatura = xDoc.CreateElement(string.Empty, "asignatura", string.Empty);
                        XmlElement xAyuda = xDoc.CreateElement(string.Empty, "ayuda", string.Empty);

                        XmlText xTxAsignatura = xDoc.CreateTextNode(dgvHorario.Rows[i].Cells[j].Value.ToString());
                        xAsignatura.AppendChild(xTxAsignatura);
                        XmlText xTxAyuda = xDoc.CreateTextNode(dgvHorario.Rows[i].Cells[j].ToolTipText);
                        xAyuda.AppendChild(xTxAyuda);

                        xDia.AppendChild(xAsignatura);
                        xDia.AppendChild(xAyuda);
                        xHora.AppendChild(xDia);
                    }

                    elementoRaiz.AppendChild(xHora);

                }

                //Ahora vamos a guardar el documento con formato correcto (suponiendo que la ruta la
                //devuelve el SaveFileDialog sfdGuardar
                XmlTextWriter xtw = new XmlTextWriter(sfdGuardar.FileName, Encoding.UTF8);
                xtw.Formatting = Formatting.Indented;
                xDoc.Save(xtw);
                xtw.Close();
                MessageBox.Show("Se ha guardado con exito el horario",
                    "Guardado", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        //Metodo que comprueba que todas las celdas contengan un valor y activa el boton guardar
        public void tablaCompleta(DataGridView dgvHorario, Button btnGuardar)
        {
            bool vacio = false;

            foreach (DataGridViewRow dgvr in dgvHorario.Rows)
            {
                foreach (DataGridViewCell dgvc in dgvr.Cells)
                {
                    if (string.IsNullOrEmpty(dgvc.Value.ToString()))
                    {
                        btnGuardar.Enabled = false;
                        vacio = true;
                        break;

                    }
                }
                if (vacio) { break; }
            }
            if (!vacio) { btnGuardar.Enabled = true; }
        }
    }
}
