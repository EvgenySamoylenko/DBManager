using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBManager
{
    public class DbTblDescription
    {
        public string tableName;
        public List<DbTblFldDesc> listOfFieldDescriptions = new List<DbTblFldDesc>();
    }
}
