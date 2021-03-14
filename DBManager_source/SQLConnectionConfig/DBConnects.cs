using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;


namespace DBManager
{
    class DBConnects
    {
       public SqlConnection serverConnect;
       public SqlCommand command;
       public SqlDataReader SqlDataRead;
    }
}
