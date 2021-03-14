using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows;
using System.Data;

namespace DBManager
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string connectionString = null;
        DBConnects dBConnects = new DBConnects();

        public void setConnStr(string dataSource)
        {
            connectionString = dataSource;
        }
        
        private List<DatabaseInfo> dataBases;
        public MainWindowViewModel()
        {
            setConnStr(DBManager.Properties.Settings.Default.severName);
        }

        public List<DatabaseInfo> DataBases
        {
            get
            {
                return dataBases;
            }
            set
            {
                dataBases = value;
                OnPropertyChanged("DataBases");
            }
        }

        public bool ReloadData()
        {
            if(DataBases != null)
            {
                foreach (var db in DataBases)
                    db.ClearAllInfo();
                DataBases.Clear();
            }
            //-----------------------------------------------------------------------

            var dataBasesList = new List<DatabaseInfo>();
            try
            {
                //specify connec to server DB
                dBConnects.serverConnect = new SqlConnection(connectionString);
                dBConnects.serverConnect.Open();

                dBConnects.command = new SqlCommand(dBConnects.serverConnect.ConnectionString, dBConnects.serverConnect);
                dBConnects.command.CommandText = "SELECT * FROM master.dbo.sysdatabases";

                dBConnects.SqlDataRead = dBConnects.command.ExecuteReader();

                while (dBConnects.SqlDataRead.Read())
                {
                    string db = dBConnects.SqlDataRead.GetString(0);
                    DatabaseInfo dbinfo = new DatabaseInfo(db);
                    dataBasesList.Add(dbinfo);
                }
                dBConnects.serverConnect.Close();

                // !AN: added the loop by every db
                foreach (var db in dataBasesList)
                    GetAllTablesOfDb(db);
            }
            catch (Exception except) 
            { 
                MessageBox.Show(except.ToString());
                return false;
            }

            DataBases = dataBasesList;

            return true;
            //-----------------------------------------------------------------------
        }

        private void GetAllTablesOfDb(DatabaseInfo db)
        {
            string connectionStr4database = $"{connectionString} Initial Catalog={db.DBName};";
            string sqlColumnsOfTables = "select TABLE_NAME, COLUMN_NAME, IS_NULLABLE, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH  "+
                    "from Information_schema.COLUMNS ORDER BY TABLE_NAME";
            SqlConnection serverConnectLc = new SqlConnection(connectionStr4database);
            serverConnectLc.Open();
            SqlCommand cmdLc = new SqlCommand(sqlColumnsOfTables, serverConnectLc);

            SqlDataReader sqlDataReaderLc = cmdLc.ExecuteReader();

            string currentTableName = string.Empty;
            DbTable curTbl = null;
            TableColumn curColumnInfo = null;  

            while (sqlDataReaderLc.Read())
            {
                string tblName = sqlDataReaderLc.GetString(0);
                string collName = sqlDataReaderLc.GetString(1);
                string isNull = sqlDataReaderLc.GetString(2);
                string dbType = sqlDataReaderLc.GetString(3);
                int charMaxLen = (sqlDataReaderLc.IsDBNull(4)) ? 0 : sqlDataReaderLc.GetInt32(4);
                curColumnInfo = new TableColumn(collName, dbType, charMaxLen);

                // If table is changed in the loop of reading - > create the new DbTable object
                if (currentTableName != tblName)
                {
                    curTbl = new DbTable(tblName);
                    db.AddTable(curTbl);
                    currentTableName = tblName;
                }
                curTbl.AddColumn(curColumnInfo);
            }
            sqlDataReaderLc.Close();
            serverConnectLc.Close();
        }

        public void Clear()
        {
            DataBases.Clear();
            DataBases = new List<DatabaseInfo>();
        }

        public DataTable Querying(string queryString, string connectString)
        {
            SqlConnection ConnString = new SqlConnection(connectString);
            ConnString.Open();

            DataTable answerAsTable = new DataTable();
            SqlDataAdapter DataAdapter = new SqlDataAdapter(queryString, ConnString);
            
            DataAdapter.Fill(answerAsTable);

            return answerAsTable;
        }

        internal void RemoveTable(DbTable tbl, string connOptions)
        {
            var dbInfo = tbl.dbInfo;
            dbInfo.RemoveTable(tbl);
            
            string sqlCrtTable = $"DROP TABLE {tbl.TableName};";
            SQLDBConn(connOptions, sqlCrtTable);
        }

        internal void RemoveTableColumn(TableColumn tblColumn, string connOptions)
        { 
            tblColumn.tblInfo.RemoveTableColumn(tblColumn);
            string sqlCrtTable = $"ALTER TABLE {tblColumn.tblInfo.TableName} DROP COLUMN {tblColumn.ColumnName};";
            SQLDBConn(connOptions, sqlCrtTable);
        }

        static void SQLDBConn(string connOptions, string toDelete)
        {
            var Conn = new SqlConnection(connOptions);
            Conn.Open();
            SqlCommand cmdToDb = new SqlCommand(toDelete, Conn);
            cmdToDb.ExecuteReader();
            Conn.Close();
        }

    }
}
