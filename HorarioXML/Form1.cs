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

        Comprobar c = new Comprobar();
        ListBox[] listaLB;
        Label[] listaLblLb, listaLblCb;
        ComboBox[] listaCB;
        string ruta;
        bool cargado;

        public frmMain()
        {
            InitializeComponent();
           
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            datosIniciales();
        }

        private void datosIniciales()
        {
            string[] horas = new string[] { "8:30 - 9:25", "9:25 - 10:20", "10:20 - 11:15",
                "11:45 - 12:40", "12:40 - 13:35", "13:35 - 14:30"};

            for (int i = 0; i < horas.Length; i++) { dgvHorario.Rows.Add(horas[i]); }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            listaCB = new ComboBox[] { this.cmbCurso, this.cmbDia, this.cmbHora };
            listaLB = new ListBox[] { this.lsbCiclo, this.lsbModulo };
            listaLblCb = new Label[] { this.lblCurso, this.lblDia, this.lblHora };
            listaLblLb = new Label[] { this.lblCiclo, this.lblModulo };

            bool combo = c.seleccionadoCB(listaCB, erpErrorCB, listaLblCb);
            bool lista = c.seleccionadoLB(listaLB, erpErrorLB, listaLblLb);

            if (!combo || !lista) {

                MessageBox.Show("Tiene que insertar un valor en los campos obligatorios",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);               

            } else {

                agregarDato();
                btnGuardar.Enabled = true;
            }
        }

        private void agregarDato()
        {
            string ciclo = this.lsbCiclo.SelectedItem.ToString();
            string modulo = this.lsbModulo.SelectedItem.ToString();

            this.dgvHorario.Rows[this.cmbHora.SelectedIndex].Cells[this.cmbDia.SelectedIndex + 1].Value = modulo;

        }

        private void dgvHorario_Click(object sender, EventArgs e)
        {
            comprobarSeleccion();
        }

        private void cmbDia_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.dgvHorario.ClearSelection();
            this.dgvHorario.Rows[this.cmbHora.SelectedIndex].Cells[this.cmbDia.SelectedIndex + 1].Selected = true;
        }

        private void cmbHora_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.dgvHorario.ClearSelection();
            this.dgvHorario.Rows[this.cmbHora.SelectedIndex].Cells[this.cmbDia.SelectedIndex + 1].Selected = true;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            listaCB = new ComboBox[] { this.cmbDia, this.cmbHora };
            listaLblCb = new Label[] { this.lblDia, this.lblHora };
            bool combo = c.seleccionadoCB(listaCB, erpErrorCB, listaLblCb);
            var valor = this.dgvHorario.Rows[this.cmbHora.SelectedIndex].Cells[this.cmbDia.SelectedIndex + 1].Value;

            if (!combo)
            {
                MessageBox.Show("Tiene que insertar un valor en los campos obligatorios",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }else if (valor == null)
            {
                MessageBox.Show("Nada que borrar",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                this.dgvHorario.Rows[this.cmbHora.SelectedIndex].Cells[this.cmbDia.SelectedIndex + 1].Value = "";
            }
        }

        private void btnCargar_Click(object sender, EventArgs e)
        {
            XmlDocument xDoc;

            if (ofdAbrir.ShowDialog() == DialogResult.OK)
            {
                ruta = ofdAbrir.FileName;
                //limpiarDatos();

                //Se lee el XML con un XMLDocument
                xDoc = new XmlDocument();
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
                int cont_hora = 0;
                foreach (XmlElement hora in horas)
                {
                    //Construimos dos arrays de strings, uno para los textos a mostrar y otros con las ayudas
                    string[] fila_pant = new string[6];
                    string[] fila_ayu = new string[6];

                    //La primera columna de cada fila será la hora: primera, segunda, etc.
                    fila_pant[0] = hora.GetAttribute("id").ToString();
                    fila_ayu[0] = "";
                    int col = 0;
                    XmlNodeList dias = hora.GetElementsByTagName("dia");
                    foreach (XmlElement dia in dias)
                    {
                        XmlNodeList entrada_pant = dia.GetElementsByTagName("asignatura");
                        fila_pant[col] = ((XmlElement)entrada_pant[0]).InnerText.ToString();
                        //XmlNodeList entrada_ayu = dia.GetElementsByTagName("ayuda");
                        ///fila_ayu[col] = ((XmlElement)entrada_ayu[0]).InnerText.ToString();
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
                cargado = true;
            }
            else
            {
                cargado = false;
            }
        }

        private void limpiarDatos()
        {
            dsDatos = new DataSet(); //Generamos un nuevo Data Set
            dgvHorario.DataSource = null; //Ponemos el contenido a null
            dgvHorario.Rows.Clear(); //Limpiamos filas
            dgvHorario.Refresh(); //Lo recargamos

        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            generarXML();
        }

        private void generarXML()
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

                    for (int j = 0; j < dgvHorario.Columns.Count; j++)
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

        private void comprobarSeleccion()
        {
            this.cmbHora.SelectedIndex = this.dgvHorario.CurrentRow.Index;
            this.cmbDia.SelectedIndex = this.dgvHorario.CurrentCell.ColumnIndex - 1;
        }
    }
}
