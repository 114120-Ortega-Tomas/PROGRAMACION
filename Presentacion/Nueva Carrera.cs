using _1._1_Carreras.Presentacion;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _1._1_Carreras
{
    public partial class FrmNueva_Carrera : Form
    {
        DBHelper oBD = new DBHelper();
        Carrera c = new Carrera();
        List<Carrera> lCarreras = new List<Carrera>();
        public FrmNueva_Carrera()
        {
            InitializeComponent();
            CenterToScreen();
        }

        private void CargarAsignaturas()
        {
            DataTable tabla = oBD.LeerBD("pa_ver_asignaturas");
            cboAsignaturas.DataSource = tabla;
            cboAsignaturas.ValueMember = tabla.Columns[0].ColumnName;
            cboAsignaturas.DisplayMember = tabla.Columns[1].ColumnName;
            cboAsignaturas.SelectedIndex = -1;


        }
       

        private void FrmNueva_Carrera_Load(object sender, EventArgs e)
        {
           CargarLista();
            CargarAsignaturas();
        }
        public void CargarLista()
        {
            lstCarreras.Items.Clear();
            lCarreras.Clear();

            DataTable Tcarrera = oBD.LeerBD("SP_VER_CARRERAS");
            foreach (DataRow fila in Tcarrera.Rows)
            {
                Carrera c = new Carrera();
                c.Nombre = fila["nombre"].ToString();
                c.IdCarrera = Convert.ToInt32(fila["id_carrera"]);

                DataTable Tdetalle = oBD.Detalles(c.IdCarrera);
                foreach (DataRow Detallefila in Tdetalle.Rows)
                {
                    DetalleCarrera d = new DetalleCarrera();
                    d.AnioCursado = Convert.ToInt32(Detallefila["anioCursado"]);
                    d.Cuatrimestre = Convert.ToInt32(Detallefila["cuatrimestre"]);
                    int idAsig = Convert.ToInt32(Detallefila["id_asignatura"]);
                    string name = Detallefila["asignatura"].ToString();
                    Asignaturas a = new Asignaturas(idAsig, name);
                    d.Asignatura = a;
                    c.AgregarDetalle(d);
                }

                lstCarreras.Items.Add(c);
                lCarreras.Add(c);
            }

        }

        public void CargarDatos(int index)
        {
            foreach (Carrera c in lCarreras)
            {
                txtNombre.Text = lCarreras[index].Nombre;

            }
            foreach (DetalleCarrera dc in lCarreras[index].Detalles)
            {
                int anio = dc.AnioCursado;
                int cuatr = dc.Cuatrimestre;
                string asig = dc.Asignatura.Name;
                int id = dc.Asignatura.Id;
                dgvDetalle.Rows.Add(new object[] { id, asig, anio, cuatr });
            }
        }
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (Validar())
            {
                
                DataRowView item = (DataRowView)cboAsignaturas.SelectedItem;
                int id = Convert.ToInt32(item.Row.ItemArray[0]);
                string asig = item.Row.ItemArray[1].ToString();
                int cuatr;
                if (rbtPrimero.Checked) cuatr = 1;
                else cuatr = 2;
                int anio = Convert.ToInt32(txtAnioCursado.Text);

                Asignaturas a = new Asignaturas(id, asig);
                DetalleCarrera dc = new DetalleCarrera(a, cuatr, anio);

                c.AgregarDetalle(dc);
                
                dgvDetalle.Rows.Add(new object[] { id,asig,cuatr,anio });
            }
        }
        private void GuardarCarrera()
        {
            c.Nombre=txtNombre.Text;
            if (c.Confirmar())
            {
                MessageBox.Show("Carrera cargada", "Informe", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Dispose();
            }
            else
            {
                MessageBox.Show("Error, no se pudo registrar correctamente la carrera", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool ValidarCarrera()
        {
            bool valido = true;
            if (txtNombre.Text == "")
            {
                valido = false;
                MessageBox.Show("Debe ingresar un nombre de carrera", "Control", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtNombre.Focus();
            }
            else
            {
                try { txtNombre.Text.ToString(); }
                catch (Exception)
                {
                    valido = false;
                    MessageBox.Show("Debe ingresar un nombre de carrera válido", "Control", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtNombre.Focus();
                }
            }
            if(dgvDetalle.Rows.Count == 0)
            {
                MessageBox.Show("Debe ingresar por lo menos una asignatura", "Control", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                valido = false;
            }
            return valido;
        }
        private bool Validar()
        {
            bool valido = true;
            if (cboAsignaturas.Text.Equals(string.Empty))
            {
                MessageBox.Show("Debe seleccionar una asignatura", "Control",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                valido = false;

            }
            if (rbtPrimero.Checked == false & rbtSegundo.Checked == false)
            {
                MessageBox.Show("Debe ingresar un cuatrimestre", "Control",
                      MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                valido = false;

            }
            if (txtAnioCursado.Text == "")
            {
                MessageBox.Show("Debe ingresar un año de cursado", "Control",
                      MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                valido = false;

            }
            else
                try { Convert.ToInt32(txtAnioCursado.Text); }
                catch (Exception)
                {
                    MessageBox.Show("Debe ingresar un año de cursado válido", "Control",
                      MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    valido = false;
                }
            foreach (DataGridViewRow row in dgvDetalle.Rows)
            {
                if (row.Cells["ClmAsignatura"].Value.ToString().Equals(cboAsignaturas.Text))
                {
                    MessageBox.Show("Asignatura: " + cboAsignaturas.Text + " ya fue añadida",
                        "Control",
                      MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    valido=false; break;

                }
            }
            return valido;
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (ValidarCarrera())
            {
                GuardarCarrera();
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Está seguro que desea cancelar y salir?\n\n Los cambios no se guardarán","Salir",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)==DialogResult.Yes)
            {
                this.Dispose();
            }
            
        }

        private void dgvDetalle_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //Al hacer click:
            if (dgvDetalle.CurrentCell.ColumnIndex == 4)
            {
                c.QuitarDetalle(dgvDetalle.CurrentRow.Index); //eliminar detalle de list<>
                dgvDetalle.Rows.Remove(dgvDetalle.CurrentRow); //eliminar detalle del dgv
            }
        }

    
       
        private void lstCarreras_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstCarreras.SelectedIndex > -1)
            {
                CargarDatos(lstCarreras.SelectedIndex);
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (oBD.BorrarCarrera("SP_Modificar", lCarreras[lstCarreras.SelectedIndex]) > 0)
            {
                lstCarreras.SelectedIndex = -1;
                MessageBox.Show("Carrera modificada", "Modificada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarLista();
                dgvDetalle.ClearSelection();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
           

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
        
            DataTable Ccarrera = oBD.LeerBD("SP_INSERTAR_DETALLE");
        }

        private void cboAsignaturas_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
