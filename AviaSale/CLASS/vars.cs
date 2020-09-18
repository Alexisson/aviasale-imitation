

using MySql.Data.MySqlClient;

namespace AviaSale
{
    class vars
    {
        public static string connectString = "server=localhost;user=root;database=ASALE;password='';";
        public MySqlConnection conn = new MySqlConnection(connectString);
    }
}
