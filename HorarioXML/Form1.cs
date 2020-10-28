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

            for (int i = 0; i < horas.Length; i++)
            {
                dgvHorario.Rows.Add(horas[i]);
            }


        }

        private void btnAdd_Click(object sender, EventArgs e)
        {

        }

        private Boolean seleccionado()
        {

            bool todo = false;

            if (true)
            {

            }

            return todo;

        }
    }
}
