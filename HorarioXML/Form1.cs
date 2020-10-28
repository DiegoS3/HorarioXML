using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HorarioXML
{
    public partial class frmMain : Form
    {

        Comprobar c = new Comprobar();
        ListBox[] listaLB;
        Label[] listaLblLb, listaLblCb;
        ComboBox[] listaCB;

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

            if (c.seleccionadoCB(listaCB, erpErrorCB, listaLblCb) 
                && c.seleccionadoLB(listaLB, erpErrorLB, listaLblLb))
            {

                MessageBox.Show("BIEN");

            }
            else { MessageBox.Show("Tiene que seleccionar todos los campos obligatorios", 
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
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

        private void comprobarSeleccion()
        {
            this.cmbHora.SelectedIndex = this.dgvHorario.CurrentRow.Index;
            this.cmbDia.SelectedIndex = this.dgvHorario.CurrentCell.ColumnIndex - 1;
        }
    }
}
