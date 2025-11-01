using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient; //Siempre se tiene que incluir esta libreria para conectar con la clase de mysql llamada ConexionMa.cs

//Nombre del proyecto: frmMaquillaje
//Nombre del desarrollador: Andrea Dueñas de Luna.
//Fecha: 31/10/2025
//Descripcion: Este formulario permite el registro, consulta, actualizacion y eliminacion
//de productos de maquillaje conectada a una base de datos en MySQL.
namespace ADL_Controles
{
    public partial class frmMaquillaje : Form
    {
        // Guarda el ID del producto que se registro en el formulario
        private string idSeleccionado = null;

        public frmMaquillaje()
        {
            InitializeComponent();
        }

        private void frmMaquillaje_Load(object sender, EventArgs e)
        {
            
            LimpiarCampos();
        }
        // Limpia todos los campos del formulario y los prepara para un nuevo registro.
        private void LimpiarCampos()
        {
            txtID.Clear();
            txtNom.Clear();
            txtMarca.Clear();
            cbCate.SelectedIndex = -1; // Limpia ComboBox
            txtTono.Clear();
            txtPrecio.Clear();

            // Desmarca todos los RadioButtons
            rbBueno.Checked = false;
            rbMalo.Checked = false;
            rbPM.Checked = false; //PM es Puede Mejorar.

            idSeleccionado = null; // Borra el ID guardado

            txtID.ReadOnly = false;

            cbConsul.SelectedIndex = -1; // Limpia la selección del ComboBox
            txtBusqueda.Clear();

            // Pone el cursor en el campo ID
            txtID.Focus();
        }
        // Obtiene el valor numérico ('2', '1', '0') de la opinión para la Base de Datos.
        private string ObtenerOpinionParaDB()
        {
            if (rbBueno.Checked) return "1";
            if (rbPM.Checked) return "0";
            if (rbMalo.Checked) return "1";

            return "1";
        }
        // Marca el RadioButton correcto basado en el valor ('2', '1', '0').
        private void CargarOpinionDesdeDB(string opinionDB)
        {
            switch (opinionDB)
            {
                case "1":
                    rbBueno.Checked = true;
                    break;
                case "0":
                    rbPM.Checked = true;
                    break;
                case "2":
                    rbMalo.Checked = true;
                    break;
                default:
                    rbBueno.Checked = false;
                    rbMalo.Checked = false;
                    rbPM.Checked = false;
                    break;
            }
        }
        private void btnRe_Click(object sender, EventArgs e)
        {
            //Se valida que el ID, Nombre y Precio no esten vacios
            if (string.IsNullOrWhiteSpace(txtID.Text) || string.IsNullOrWhiteSpace(txtNom.Text) || string.IsNullOrWhiteSpace(txtPrecio.Text))
            {
                MessageBox.Show("Los campos ID, Nombre y Precio son obligatorios.", "Campos Vacíos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // No se puede registrar si hay un producto cargado se debe limpiar primero
            if (idSeleccionado != null)
            {
                MessageBox.Show("Ya hay un producto cargado. Limpie los campos antes de registrar uno nuevo.", "Acción Inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string sql = "INSERT INTO producto (id, nombre, marca, categoria, tono, precio_publico, opinion) " +
                         "VALUES (@id, @nombre, @marca, @categoria, @tono, @precio, @opinion)";

            // Aqui es donde la clase ConexionMa se empieza a utilizar para la conexion
            using (MySqlConnection conn = ConexionMa.conexion())
            {
                if (conn == null) return; 

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    //Aqui estan todos los paramertros del formulario
                    cmd.Parameters.AddWithValue("@id", txtID.Text);
                    cmd.Parameters.AddWithValue("@nombre", txtNom.Text);
                    cmd.Parameters.AddWithValue("@marca", txtMarca.Text);
                    cmd.Parameters.AddWithValue("@categoria", cbCate.Text);
                    cmd.Parameters.AddWithValue("@tono", txtTono.Text);
                    cmd.Parameters.AddWithValue("@precio", Convert.ToDecimal(txtPrecio.Text));
                    cmd.Parameters.AddWithValue("@opinion", ObtenerOpinionParaDB());

                    try
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Producto registrado con éxito.", "Registro Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarCampos();
                    }
                    catch (MySqlException mex)
                    {
                        //Muestra un mensaje cuando el ID esta repetido
                        if (mex.Number == 1062)
                        {
                            MessageBox.Show("Error: El ID '" + txtID.Text + "' ya existe.", "ID Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            MessageBox.Show("Error de base de datos: " + mex.Message, "Error SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al registrar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void btnCon_Click(object sender, EventArgs e)
        {
            if (cbConsul.SelectedItem == null || string.IsNullOrWhiteSpace(txtBusqueda.Text))
            {
                MessageBox.Show("Selecciona un tipo de consulta (ID o Nombre) y escribe un valor.", "Datos Faltantes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string tipoConsulta = cbConsul.SelectedItem.ToString();
            string valorBusqueda = txtBusqueda.Text;
            string sql = "";

            if (tipoConsulta == "ID")
            {
                sql = "SELECT * FROM producto WHERE id = @valor";
            }
            else if (tipoConsulta == "Nombre")
            {
                sql = "SELECT * FROM producto WHERE nombre = @valor LIMIT 1";
            }
            else
            {
                MessageBox.Show("Tipo de consulta no válido.");
                return;
            }

            bool encontrado = false;
            //Se velve a usar la clase de conexion
            using (MySqlConnection conn = ConexionMa.conexion())
            {
                if (conn == null) return;

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@valor", valorBusqueda);

                    try
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtID.Text = reader["id"].ToString();
                                txtNom.Text = reader["nombre"].ToString();
                                txtMarca.Text = reader["marca"].ToString();
                                cbCate.SelectedItem = reader["categoria"].ToString();
                                txtTono.Text = reader["tono"].ToString();
                                txtPrecio.Text = reader["precio_publico"].ToString();
                                CargarOpinionDesdeDB(reader["opinion"].ToString());

                                idSeleccionado = txtID.Text;

                                // Bloqueamos el ID DESPUÉS de consultar
                                txtID.ReadOnly = true;
                                encontrado = true;
                            }
                        }
                    }
                    catch (Exception ex)// Muestro error si no se puede consultar
                    {
                        MessageBox.Show("Error al consultar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            if (!encontrado)//Si no se encontro el producto
            {
                MessageBox.Show("No se encontró ningún producto con ese " + tipoConsulta + ".", "No Encontrado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
            }
        }
        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (idSeleccionado == null)//Si no hay un producto selecionado para su actualizacion
            {
                MessageBox.Show("Primero consulta un producto para poder actualizarlo.", "Sin Selección", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string sql = "UPDATE producto SET id = @idNuevo, nombre = @nombre, marca = @marca, " +
                         "categoria = @categoria, tono = @tono, precio_publico = @precio, opinion = @opinion " +
                         "WHERE id = @idViejo";

            // Usamos TU clase de conexión: ConexionMa.conexion()
            using (MySqlConnection conn = ConexionMa.conexion())
            {
                if (conn == null) return;

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {//Nuevamente van todos los parametros 
                    cmd.Parameters.AddWithValue("@idNuevo", txtID.Text); 
                    cmd.Parameters.AddWithValue("@nombre", txtNom.Text);
                    cmd.Parameters.AddWithValue("@marca", txtMarca.Text);
                    cmd.Parameters.AddWithValue("@categoria", cbCate.Text);
                    cmd.Parameters.AddWithValue("@tono", txtTono.Text);
                    cmd.Parameters.AddWithValue("@precio", txtPrecio.Text);
                    cmd.Parameters.AddWithValue("@opinion", ObtenerOpinionParaDB());
                    cmd.Parameters.AddWithValue("@idViejo", idSeleccionado);//Este es el ID original antes de actualizarlo

                    try
                    {
                        int filasAfectadas = cmd.ExecuteNonQuery();
                        if (filasAfectadas > 0)
                        {//Muestra que se actualizo correctamente 
                            MessageBox.Show("Producto actualizado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {//Aqui que no se encontro el producto original
                            MessageBox.Show("No se encontro el producto original para actualizar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        LimpiarCampos();
                    }
                    catch (MySqlException mex)
                    {
                        if (mex.Number == 1062)
                        {
                            MessageBox.Show("Error: El nuevo ID '" + txtID.Text + "' ya le pertenece a otro producto.", "ID Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            MessageBox.Show("Error en la base de datos: " + mex.Message, "Error SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al actualizar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void btnEli_Click(object sender, EventArgs e)
        {
            if (idSeleccionado == null)
            {//Cundo se selecciona eliminar sin haber consultado previamente 
                MessageBox.Show("Primero consulta un producto para poder eliminarlo.", "Sin Seleccion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //Pegunta la confirmacion si en verdad se quiere eliminar 
            if (MessageBox.Show("¿Estas seguro de que quieres eliminar este producto (" + idSeleccionado + " - " + txtNom.Text + ")?",
                                "Confirmar Eliminacion", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string sql = "DELETE FROM producto WHERE id = @id";

                using (MySqlConnection conn = ConexionMa.conexion())
                {
                    if (conn == null) return;

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idSeleccionado);

                        try
                        {
                            cmd.ExecuteNonQuery();//Realiza la eliminacion 
                            MessageBox.Show("Producto eliminado.", " Con Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LimpiarCampos();
                        }
                        catch (Exception ex)
                        {//Muestra error si no se puede eliminar 
                            MessageBox.Show("Error al eliminar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }
        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();//Se limpia todo el formulario 
        }
        private void btnSalir_Click(object sender, EventArgs e)//Cierra todo el programa 
        {
            this.Close();
        }
     
    }
}