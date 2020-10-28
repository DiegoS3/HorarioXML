using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HorarioXML
{
    class Comprobar
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

    }
}
