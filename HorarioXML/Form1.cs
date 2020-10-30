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
        Utiles c = new Utiles();
        ListBox[] listaLB;
        Label[] listaLblLb, listaLblCb;
        ComboBox[] listaCB;
        string ruta;

        public frmMain()
        {
            InitializeComponent();           
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            c.crearTabla(this.dsDatos, this.dgvHorario);
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

                c.agregarDato(this.dgvHorario, this.cmbCurso, this.lsbModulo, this.lsbCiclo);
                this.lsbCiclo.Enabled = false;
                this.cmbCurso.Enabled = false;
            }
        }

        private void dgvHorario_Click(object sender, EventArgs e)
        {
            comprobarSeleccion();
        }

        private void cmbDia_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.dgvHorario.CurrentCell = this.dgvHorario.Rows[this.cmbHora.SelectedIndex].Cells[this.cmbDia.SelectedIndex + 1];
            this.dgvHorario.CurrentCell.Selected = true;
        }

        private void cmbHora_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.dgvHorario.CurrentCell = this.dgvHorario.Rows[this.cmbHora.SelectedIndex].Cells[this.dgvHorario.CurrentCell.ColumnIndex];
            this.dgvHorario.CurrentCell.Selected = true;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            listaCB = new ComboBox[] { this.cmbDia, this.cmbHora };
            listaLblCb = new Label[] { this.lblDia, this.lblHora };
            bool combo = c.seleccionadoCB(listaCB, erpErrorCB, listaLblCb);
            try
            {
                var valor = this.dgvHorario.CurrentCell.Value;

                if (!combo)
                {
                    MessageBox.Show("Tiene que insertar un valor en los campos obligatorios",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
                else if (string.IsNullOrEmpty(valor.ToString()))
                {
                    MessageBox.Show("Nada que borrar",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
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

        private void btnCargar_Click(object sender, EventArgs e)
        {
            XmlDocument xDoc;

            if (ofdAbrir.ShowDialog() == DialogResult.OK)
            {

                limpiarDatos();

                //Se lee el XML con un XMLDocument
                xDoc = new XmlDocument();
                int cont_hora = 0;

                try
                {
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

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            c.generarXML(this.sfdGuardar, this.dgvHorario);
            this.lsbCiclo.Enabled = true;
            this.cmbCurso.Enabled = true;
        }

        private void lsbCiclo_SelectedIndexChanged(object sender, EventArgs e)
        {
            c.cambiarAsig(this.lsbModulo, this.lsbCiclo, this.cmbCurso);
        }

        private void cmbCurso_SelectedIndexChanged(object sender, EventArgs e)
        {
            c.cambiarAsig(this.lsbModulo, this.lsbCiclo, this.cmbCurso);
        }

        private void comprobarSeleccion()
        {
            this.cmbHora.SelectedIndex = this.dgvHorario.CurrentRow.Index;
            this.cmbDia.SelectedIndex = this.dgvHorario.CurrentCell.ColumnIndex - 1;
        }

        private void dgvHorario_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            c.tablaCompleta(this.dgvHorario, this.btnGuardar);
        }

        private void limpiarDatos()
        {
            dsDatos = new DataSet(); //Generamos un nuevo Data Set
            dgvHorario.DataSource = null; //Ponemos el contenido a null
            dgvHorario.Rows.Clear(); //Limpiamos filas
            dgvHorario.Refresh(); //Lo recargamos

        }
    }
}
