using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ADL_Controles
{
    internal class ConexionMa
    {
        public static MySqlConnection conexion()
        {
            string servidor = "localhost";
            string bd = "maquillaje";
            string usuario = "root";
            string password = "rooot";
            //se crea la cadena conexión con los datos anteriormente guardados
            string cadenaConexion = "Database=" + bd + ";Data Source=" + servidor + ";User Id=" + usuario + ";Password=" + password + "";

            try //se intenta la coneccion
            {
                MySqlConnection conexionBD = new MySqlConnection(cadenaConexion);
                conexionBD.Open();
                return conexionBD;
            }

            catch (MySqlException ex)
            {
                Console.WriteLine("Error: " + ex.Message); //en caso de que no se conecte a mysql da este mensaje
                return null;
            }
        }
    }
}

