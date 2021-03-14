using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows;

namespace DBManager
{
    public class DatabaseInfo : ViewModelBase
    {
        private List<DbTable> tables;

        private string databaseName;

        public DatabaseInfo(string DatabaseName)
        {
            DBName = DatabaseName;
            tables = new List<DbTable>();
        }

        public string DBName
        {
            get
            {
                return databaseName;
            }
            set
            {
                databaseName = value;
                OnPropertyChanged("DBName");
            }
        }

        public List<DbTable> DbTables
        {
            get
            {
                return tables;
            }
            set
            {
                tables = value;
                OnPropertyChanged("DbTables");
            }
        }

        public void ClearAllInfo()
        {
            DbTables.Clear();
        }

        public void AddTable(DbTable tbl)
        {
            tables.Add(tbl);
            tbl.dbInfo = this;
        }

        internal void RemoveTable(DbTable tbl)
        {
            int idx = -1;
            for (int i = 0; i < tables.Count; i++) 
            {
                if (tables[i].TableName == tbl.TableName)
                {
                    tables.RemoveAt(i);
                    idx = i;
                    break;
                }
            }
            if (idx == -1)
                return;
            var tblList1 = new List<DbTable>();
            tblList1.AddRange(tables);
            DbTables = tblList1;
        }
    }
}