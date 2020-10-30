using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace HorarioXML
{
    public partial class frmMain : Form
    {
        //Variables
        Utiles c = new Utiles();
        ListBox[] listaLB;
        Label[] listaLblLb, listaLblCb;
        ComboBox[] listaCB;
        string ruta;

        public frmMain()
        {
            InitializeComponent();           
        }

        //Evento que se produce al cargar el formulario
        private void frmMain_Load(object sender, EventArgs e)
        {
            //Creamos la tabla con los datos iniciales
            c.crearTabla(this.dsDatos, this.dgvHorario);
        }        

        //Evento que se produce al hacer click en el boton añadir
        private void btnAdd_Click(object sender, EventArgs e)
        {
            //Arrays que contienen los diferentes componentes del formulario
            listaCB = new ComboBox[] { this.cmbCurso, this.cmbDia, this.cmbHora };
            listaLB = new ListBox[] { this.lsbCiclo, this.lsbModulo };
            listaLblCb = new Label[] { this.lblCurso, this.lblDia, this.lblHora };
            listaLblLb = new Label[] { this.lblCiclo, this.lblModulo };

            //Booleans que varian en base a si se han seleccionado todos los campos o no
            bool combo = c.seleccionadoCB(listaCB, erpErrorCB, listaLblCb);
            bool lista = c.seleccionadoLB(listaLB, erpErrorLB, listaLblLb);

            //Si alguno de los campos no esta seleccionado mostramos un dialog
            if (!combo || !lista) {

                MessageBox.Show("Tiene que insertar un valor en los campos obligatorios",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);               

            } else {

                //Agregamos la asignatura a la celda seleccionada
                c.agregarDato(this.dgvHorario, this.cmbCurso, this.lsbModulo, this.lsbCiclo);
                //Desactivamos los campos para que no pueda mezclar asignaturas de diferentes ciclos y cursos
                this.lsbCiclo.Enabled = false;
                this.cmbCurso.Enabled = false;
            }
        }

        //Evento que se activa al hacer click en el DataGridView
        private void dgvHorario_Click(object sender, EventArgs e)
        {
            //Comprobamos la celda seleccionada
            comprobarSeleccion();
        }

        //Evento que se activa cuando cambia el Valor de la ComboBox de dia
        private void cmbDia_SelectedIndexChanged(object sender, EventArgs e)
        {
            //modificamos la celda seleccionada en base a los nuevos valores
            this.dgvHorario.CurrentCell = this.dgvHorario.Rows[this.cmbHora.SelectedIndex].Cells[this.cmbDia.SelectedIndex + 1];
            this.dgvHorario.CurrentCell.Selected = true;
        }

        //Evento que se activa cuando cambia el Valor de la ComboBox de hora
        private void cmbHora_SelectedIndexChanged(object sender, EventArgs e)
        {
            //modificamos la celda seleccionada en base a los nuevos valores
            this.dgvHorario.CurrentCell = this.dgvHorario.Rows[this.cmbHora.SelectedIndex].Cells[this.dgvHorario.CurrentCell.ColumnIndex];
            this.dgvHorario.CurrentCell.Selected = true;
        }

        //Evento que se activa al hacer click en el boton de eliminar
        private void btnDelete_Click(object sender, EventArgs e)
        {
            //Arrays que contienen los diferentes componentes del formulario
            listaCB = new ComboBox[] { this.cmbDia, this.cmbHora };
            listaLblCb = new Label[] { this.lblDia, this.lblHora };
            //Booleans que varian en base a si se han seleccionado todos los campos o no
            bool combo = c.seleccionadoCB(listaCB, erpErrorCB, listaLblCb);

            try
            {
                //valor que tiene la celda seleccionada
                var valor = this.dgvHorario.CurrentCell.Value;

                //Si no ha seleccionado los valores necesarios se lo indicamos
                if (!combo)
                {
                    MessageBox.Show("Tiene que insertar un valor en los campos obligatorios",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
                else if (string.IsNullOrEmpty(valor.ToString())) //Si esta vacio le indicamos que no se puede borrar
                {
                    MessageBox.Show("Nada que borrar",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    //Borrado de la celada actual y de su tooltip
                    this.dgvHorario.Rows[this.cmbHora.SelectedIndex].Cells[this.cmbDia.SelectedIndex + 1].Value = "";
                    this.c.deleteToolTip(this.dgvHorario, this.cmbHora, this.lsbModulo, this.cmbDia);
                    this.btnGuardar.Enabled = false;
                }
            }
            catch (ArgumentOutOfRangeException)
            {

                MessageBox.Show("Seleciona un campo valido para borrar",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }            
        }

        //Evento que se activa al hacer click en el boton de cargar
        private void btnCargar_Click(object sender, EventArgs e)
        {
            XmlDocument xDoc;

            if (ofdAbrir.ShowDialog() == DialogResult.OK)
            {
                //metodo que limpia el DataGridVIew
                limpiarDatos();

                //Se lee el XML con un XMLDocument
                xDoc = new XmlDocument();
                int cont_hora = 0;

                try
                {
                    //Ruta en la que se encuentra el .xml seleccionado
                    ruta = ofdAbrir.FileName;
                    xDoc.Load(ruta);

                    //Añadimos la tabla al horario directamente, con las columnas fijas
                    dsDatos.Tables.Add(new DataTable("horario"));
                    dsDatos.Tables[0].Columns.Add("Hora", typeof(string));
                    dsDatos.Tables[0].Columns.Add("Lunes", typeof(string));
                    dsDatos.Tables[0].Columns.Add("Martes", typeof(string));
                    dsDatos.Tables[0].Columns.Add("Miercoles", typeof(string));
                    dsDatos.Tables[0].Columns.Add("Jueves", typeof(string));
                    dsDatos.Tables[0].Columns.Add("Viernes", typeof(string));

                    //Recorremos el XMLDocument y vamos rellenando el DataSet junto con los tooltiptext
                    XmlNodeList horario = xDoc.GetElementsByTagName("horario");
                    XmlNodeList horas = ((XmlElement)horario[0]).GetElementsByTagName("hora");                 
                   
                    foreach (XmlElement hora in horas)
                    {
                        //Construimos dos arrays de strings, uno para los textos a mostrar y otros con las ayudas
                        string[] fila_pant = new string[6];
                        string[] fila_ayu = new string[6];

                        //La primera columna de cada fila será la hora: primera, segunda, etc.
                        fila_pant[0] = hora.GetAttribute("id").ToString();
                        fila_ayu[0] = "";
                        int col = 1;
                        XmlNodeList dias = hora.GetElementsByTagName("dia");

                        foreach (XmlElement dia in dias)
                        {
                            XmlNodeList entrada_pant = dia.GetElementsByTagName("asignatura");
                            fila_pant[col] = ((XmlElement)entrada_pant[0]).InnerText.ToString();
                            XmlNodeList entrada_ayu = dia.GetElementsByTagName("ayuda");
                            fila_ayu[col] = ((XmlElement)entrada_ayu[0]).InnerText.ToString();
                            col++;
                        }
                        dsDatos.Tables[0].Rows.Add(fila_pant);

                        //Antes de pasar a la siguiente fila, se enlaza el dataGridView y el DataSet
                        //Y así podemos establecer los textos de ayuda
                        dgvHorario.DataSource = dsDatos.Tables[0];

                        for (int c = 1; c < fila_ayu.Length; c++)
                        {
                            dgvHorario.Rows[cont_hora].Cells[c].ToolTipText = fila_ayu[c];
                        }
                        cont_hora++;
                    }
                    
                }
                catch (XmlException xmle)
                {
                    MessageBox.Show("Intentando cargar un archivo no compatible", "ERROR",
                         MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //Evento que se activa al hacer click en el boton de guardar
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            //Metodo que genera el XML con el formato otorgado
            c.generarXML(this.sfdGuardar, this.dgvHorario);
            //Limpiamos la tabla, la volvemos a crear y activamos los campos
            limpiarDatos();
            c.crearTabla(this.dsDatos, this.dgvHorario);
            this.lsbCiclo.Enabled = true;
            this.cmbCurso.Enabled = true;
        }

        //Evento que se activa al cambiar el valor del ListBox ciclo
        private void lsbCiclo_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Metodo que cambia las asignaturas
            c.cambiarAsig(this.lsbModulo, this.lsbCiclo, this.cmbCurso);
        }

        //Evento que se activa al cambiar el valor del ComboBox curso
        private void cmbCurso_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Metodo que cambia las asignaturas
            c.cambiarAsig(this.lsbModulo, this.lsbCiclo, this.cmbCurso);
        }

        //Metodo que comprueaba la celda seleccionada y modifica los ComboBox dia y hora
        private void comprobarSeleccion()
        {
            this.cmbHora.SelectedIndex = this.dgvHorario.CurrentRow.Index;
            this.cmbDia.SelectedIndex = this.dgvHorario.CurrentCell.ColumnIndex - 1;
        }
        //Evento que se activa al cambiar el valor de una celda
        private void dgvHorario_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //Metodo que comprueba que todas las celdas de la tabla esten completas y activa el boton guardar
            c.tablaCompleta(this.dgvHorario, this.btnGuardar);
        }

        //Metodo que limpia la tabla
        private void limpiarDatos()
        {
            dsDatos = new DataSet(); //Generamos un nuevo Data Set
            dgvHorario.DataSource = null; //Ponemos el contenido a null
            dgvHorario.Rows.Clear(); //Limpiamos filas
            dgvHorario.Refresh(); //Lo recargamos

        }
    }
}
