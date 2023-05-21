using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Parcial_Tercer_Corte
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // conexion a la base de datos
        SqlConnection conexion = new SqlConnection("server=DESKTOP-A38J5RV ; database=db-parcial ; integrated security = true");

        // limpiar el formulario
        public void formularioBlanco()
        {
            txtNombre.Clear();
            txtApellido.Clear();
            cb_tipo.SelectedIndex = -1;
            txtDocumento.Clear();
            dateNacimiento.ResetText();
        }

        //boton de agregar a la base de datos
        private void btn_agregar_Click(object sender, EventArgs e)
        {
            conexion.Open();

            try
            {

                if (string.IsNullOrEmpty(txtNombre.Text) || string.IsNullOrEmpty(txtApellido.Text) ||
                    string.IsNullOrEmpty(cb_tipo.Text) || string.IsNullOrEmpty(txtDocumento.Text) ||
                    string.IsNullOrEmpty(dateNacimiento.Text))
                {
                    throw new Exception("Hay campos vacios, por favor completar");
                }
                else
                {
                    try
                    {
                        string cadena = "insert into tb_datos (nombre, apellido, tipo_documento, numero_documento, fecha_nacimiento)" +
                        " values ('" + txtNombre.Text + "', '" + txtApellido.Text + "', '" + cb_tipo.SelectedItem.ToString() + "', '" + txtDocumento.Text + "', '" + dateNacimiento.Text + "')";


                        SqlCommand comando = new SqlCommand(cadena, conexion);

                        comando.ExecuteNonQuery();
                        MessageBox.Show("La informacion ha sido guardada exitosamente");

                        formularioBlanco();

                        conexion.Close();
                    }
                    catch (System.Data.SqlClient.SqlException SqlEx)
                    {
                        MessageBox.Show("El numero de identificacion ya existe, verifica por favor");
                    }

                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hay campos vacios, por favor completar el formulario");
            }
  
        }

        // Boton de mostrar tabla en el formulario
        private void btn_mostrar_Click(object sender, EventArgs e)
        {
            conexion.Open();

            string mostrar = "select * from tb_datos";

            SqlDataAdapter adaptador = new SqlDataAdapter(mostrar, conexion);

            DataTable tabla = new DataTable();
            adaptador.Fill(tabla);
            dataGridView1.DataSource = tabla;
            SqlCommand comando = new SqlCommand();

            

            conexion.Close();
        }

        //Boton para buscar un elemento
        private void btn_buscar_Click(object sender, EventArgs e)
        {
            
            conexion.Open();

            txtDocumento.Enabled = false;

            string texto = txtBuscar.Text;
            string cadena = "select * from tb_datos where numero_documento='" + texto + "'";

            SqlCommand comando = new SqlCommand(cadena, conexion);
            SqlDataReader reader = comando.ExecuteReader();

            if (reader.Read())
            {
                txtNombre.Text = reader["nombre"].ToString();
                txtApellido.Text = reader["apellido"].ToString();
                cb_tipo.Text = reader["tipo_documento"].ToString();
                txtDocumento.Text = reader["numero_documento"].ToString();
                dateNacimiento.Text = reader["fecha_nacimiento"].ToString();


                reader.Close(); 

                string mostrar = "select * from tb_datos where numero_documento='" + txtBuscar.Text + "'";
                SqlDataAdapter adaptador = new SqlDataAdapter(mostrar, conexion);

                DataTable tabla = new DataTable();

                adaptador.Fill(tabla);
                dataGridView1.DataSource = tabla;

                btn_eliminar.Enabled = true;
                btn_modificar.Enabled = true;

                
            }
            else
            {
                MessageBox.Show("Numero de documento no encontrado, por favor verificar");
            }

            conexion.Close();
        }

        //Boton para modificar o actualizar un elemento
        private void btn_modificar_Click_1(object sender, EventArgs e)
        {
            conexion.Open();
            
            string num = txtBuscar.Text;

            string cadena = "UPDATE tb_datos SET nombre = @nombre, apellido = @apellido, tipo_documento = @tipo, fecha_nacimiento = @fecha WHERE numero_documento = @numero_documento";

            SqlCommand comando = new SqlCommand(cadena, conexion);
            comando.Parameters.AddWithValue("@nombre", txtNombre.Text);
            comando.Parameters.AddWithValue("@apellido", txtApellido.Text);
            comando.Parameters.AddWithValue("@tipo", cb_tipo.SelectedItem.ToString());
            comando.Parameters.AddWithValue("@fecha", dateNacimiento.Text);
            comando.Parameters.AddWithValue("@numero_documento", num);

            int cant;
            cant = comando.ExecuteNonQuery();

            if (cant == 1)
            {
                MessageBox.Show("Se ha actualizado exitosamente");
                txtDocumento.Enabled = true;
                formularioBlanco();
                btn_modificar.Enabled=false;
                btn_eliminar.Enabled = false;

            }
            else
            {
                MessageBox.Show("No se pudo actualizar");
            }

            conexion.Close();

        }

        //Bonton para eliminar un elemento
        private void btn_eliminar_Click(object sender, EventArgs e)
        {

            conexion.Open();

            string texto = txtBuscar.Text;

            string cadena = "delete from tb_datos where numero_documento='" + texto + "'";

            SqlCommand comando = new SqlCommand( cadena, conexion);

            int cant;

            cant = comando.ExecuteNonQuery();
            if (cant == 1)
            {
                MessageBox.Show("Se ha elimina exitosamente");
                txtDocumento.Enabled = true;
                formularioBlanco();
                btn_modificar.Enabled=false;
                btn_eliminar.Enabled=false;

            }
            else
            {
                MessageBox.Show("No se ha podido eliminar ");
            }
            conexion.Close();
        }
    }
}
