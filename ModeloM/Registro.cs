using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient; //Libreria para trabajar con los datos de la base de datos MySQL
using System.Web.UI.WebControls; //Libreria  para usar controles de Web Forms (como GridView)
using System.Data; //Libreria para usar tipos de datos como DataTable

namespace ModeloM
{
    //Contiene toda la lógica de la base de datos o Capa de ModeloM
    public class Registro
    {
        // Variable para manejar la conexión a la base de datos
        Conexion con;
       
        //Metodo privado que obtiene todos los datos de la tabla llamada producto
        private DataTable TablaMaquillaje()
        {
            DataTable producto = new DataTable();   
            con = new Conexion();
            con.abrirBD();
     
            string sql = "SELECT id, nombre, marca, categoria, tono, precio_publico FROM producto";
            MySqlDataAdapter query = new MySqlDataAdapter(sql, con.conexionBD);
            query.Fill(producto);
            
            con.cerrarBD();
            return producto; // Devuelve los datos obtenidos
        }
        
        //Metodo que se llama desde la página ASPX para mostrar los datos en el GridView
        public void gridMaquillaje(GridView grid)
        {
            grid.DataSource = TablaMaquillaje();
            grid.DataBind();
        }

        //Metodo para agregar un producto a la base de datos son ingresados 6 parámetros 5 de string y 1 decimal
        public int agregarProducto(string id, string nombre, string marca, string categoria, string tono, decimal precio)
        {
            int bandera = 0; //Indica el exito (0 = fallo, 1 = exito)
            con = new Conexion();
            con.abrirBD();
            string sql = string.Format("INSERT INTO producto(id, nombre, marca, categoria, tono, precio_publico) VALUES('{0}', '{1}', '{2}', '{3}', '{4}', {5});",id, nombre, marca, categoria, tono, precio);
            
            MySqlCommand comando = new MySqlCommand(sql, con.conexionBD);
            comando.Connection = con.conexionBD; 
            
            //Realiza y guarda el número de filas afectadas (debe ser 1 si es exitoso)
            bandera = comando.ExecuteNonQuery(); 
            con.cerrarBD();
            return bandera;
        }
        
       
        //Metodo para modificar los datos de un producto recibe todos los campos, incluyendo el 'id' para saber que fila actualizar.
        public int actualizarProducto(string id, string nombre, string marca, string categoria, string tono, decimal precio)
        {
            int bandera = 0;
            con = new Conexion();
            con.abrirBD();
            
            //Consulta SQL UPDATE o actualizar tal como indica el titulo: 
            string sql = string.Format(
                "UPDATE producto SET nombre='{1}', marca='{2}', categoria='{3}', tono='{4}', precio_publico={5} WHERE id='{0}';", 
                id, nombre, marca, categoria, tono, precio);
            
            MySqlCommand comando = new MySqlCommand(sql, con.conexionBD);
            comando.Connection = con.conexionBD;
            bandera = comando.ExecuteNonQuery(); //Realiza la actualización
            
            con.cerrarBD();
            return bandera;
        }
        
        //Metodo para eliminar un producto usando su ID
        public int eliminarProducto(string id)
        {
            int bandera = 0;
            con = new Conexion();
            con.abrirBD();
            
            //Consulta SQL DELETE: Usa el ID para eliminar la fila específica
            string sql = string.Format("delete from producto where id={0};", id); 
            
            MySqlCommand comando = new MySqlCommand(sql, con.conexionBD);
            comando.Connection = con.conexionBD;
            bandera = comando.ExecuteNonQuery(); //Realiza la eliminación
            
            con.cerrarBD();
            return bandera;
        }
    }
}