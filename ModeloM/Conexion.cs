using MySql.Data.MySqlClient; //Libreria para trabajar con los datos de la base de datos MySQL
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace ModeloM
{
    public class Conexion
    {
        public MySqlConnection conexionBD = new MySqlConnection();

        public void abrirBD()
        {
            string servidor = "localhost";
            string bd = "maquillaje";
            string usuario = "root";
            string password = "rooot";
            //se crea la cadena conexion con los datos anteriomente guardados 
            string cadenaConexion = "Database=" + bd + ";Data Source=" + servidor + ";User Id=" + usuario +";Password=" + password + "";


            try //se intenta la conexion 
            {
                conexionBD.ConnectionString = cadenaConexion;
                conexionBD.Open();
                System.Diagnostics.Debug.WriteLine("Se conecta");
            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);

            }
        }//conexion 

        public void cerrarBD()
        {
            try
            {
                if (conexionBD.State == ConnectionState.Open)
                {
                    conexionBD.Close();
                    System.Diagnostics.Debug.WriteLine("se desconecta");
                }
            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);
            }
        }// se cierra 
    }
}
