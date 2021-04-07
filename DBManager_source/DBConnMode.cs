using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBManager
{
    class DBConnMode
    {
        public static string GetConnectionString(string srvName, string dbName, string userName, string pwd, bool IsWinAuth)
        {
          string initConnStr;

          if(IsWinAuth == true)
          {
            return initConnStr = $"Data Source={srvName};Initial Catalog={dbName};User ID={userName};Password={pwd};Integrated Security={IsWinAuth};";
          }
          else
          {
            return initConnStr = $"Data Source={srvName};Initial Catalog={dbName};Integrated Security=SSPI;";
          }
        }
    }
}
