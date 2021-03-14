using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.IO;


namespace DBManager
{
    class LoadFromCsvFile
    {  
        //Convert DataColumn.DataType to SqlDbType вар1
        /*static SqlDbType GetDBType(string strType)
        {
            System.Type theType = strType.GetType();
                                  Type.GetType(strType);
            System.Data.SqlClient.SqlParameter p1;
            System.ComponentModel.TypeConverter tc;
            p1 = new System.Data.SqlClient.SqlParameter();
            tc = System.ComponentModel.TypeDescriptor.GetConverter(p1.DbType);
            if (tc.CanConvertFrom(theType))
            {
                p1.DbType = (DbType)tc.ConvertFrom(theType.Name);
            }
            else
            {
                //Try brute force
                try
                {
                    p1.DbType = (DbType)tc.ConvertFrom(theType.Name);
                }
                catch (Exception)
                {
                    //Do Nothing; will return NVarChar as default
                }
            }
            return p1.SqlDbType;
        }*/
        
        //вар2
        static Dictionary<string, string> GetSQLTypeConversionMap()
        {
            var result = new Dictionary<string, string>();
            result.Add(typeof(string).ToString(),   "nvarchar(1024)");
            result.Add(typeof(Int16).ToString(),    "smallint");
            result.Add(typeof(Int32).ToString(),    "int");
            result.Add(typeof(DateTime).ToString(), "datetime");
            return result;
        }
        //can be solved over 'if' or 'switch' selector

        public static string LoadToDb(string connStr)
        {
            SqlConnection Conn = null;
            OpenFileDialog ofd = null;
            //-------------upload json file to db, creating empty table------------------------
            //var fldSqlType = GetSQLTypeConversionMap();//для словаря
            string tblName = null;
            try
            {
                ofd = new OpenFileDialog();
                ofd.Filter = "CSV files (*.csv)|*.csv";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string filePathJson = Path.Combine(Path.GetDirectoryName(ofd.FileName), Path.GetFileNameWithoutExtension(ofd.FileName) + ".json");
                    string jsStr = File.ReadAllText(filePathJson);
                    var deserealised = new JavaScriptSerializer().Deserialize<DbTblDescription>(jsStr);
                    tblName = Path.GetFileNameWithoutExtension(deserealised.tableName);
                    CreateSQLTable(connStr, tblName, deserealised.listOfFieldDescriptions);
                    UploadCsvToDbTable(connStr, ofd.FileName, tblName);
                }
            }
            catch (Exception expt)
            {
                MessageBox.Show(expt.Message);
            }
            return tblName;
        }

        private static void UploadCsvToDbTable(string connStr, string fileName, string tableName)
        {
            StreamReader sr = new StreamReader(fileName);
            string line = sr.ReadLine();
            string[] value = line.Split(',');
            DataTable dt = new DataTable();
            DataRow row;
            foreach (string dc in value)
            {
                dt.Columns.Add(new DataColumn(dc));
            }

            while (!sr.EndOfStream)
            {
                value = sr.ReadLine().Split(',');
                if (value.Length == dt.Columns.Count)
                {
                    row = dt.NewRow();
                    row.ItemArray = value;
                    dt.Rows.Add(row);
                }
            }
            SqlBulkCopy blkCopy = new SqlBulkCopy(connStr, SqlBulkCopyOptions.TableLock);
            blkCopy.DestinationTableName = tableName;
            blkCopy.BatchSize = dt.Rows.Count;
            blkCopy.WriteToServer(dt);
            blkCopy.Close();
        }

        private static void CreateSQLTable(string connStr, string tableName, List<DbTblFldDesc> listOfFieldDescriptions)
        {
             string sqlCrtTable = $"CREATE TABLE {Path.GetFileNameWithoutExtension(tableName)}(";

            foreach (DbTblFldDesc param in listOfFieldDescriptions)
            {
                //string fld = fldSqlType[param.FldType];// для словаря //GetSQLTypeConversionMap()[param.FldType]}
                string fldType = GetSQLTypeConversionMap()[param.FldType];
                //if (fldType == "varchar")
                //{
                //    fldType = "nvarchar(1024)";
                //}
                string fldStr = $"{ param.FldName } { fldType} NOT NULL,"; //для словаря
                //string fldStr = $"[{ param.FldName }] { GetDBType(param.FldType) } NOT NULL,";//  для метода
                sqlCrtTable += fldStr;
            }

            sqlCrtTable = sqlCrtTable.Remove(sqlCrtTable.Length - 1) + ")";

            var Conn = new SqlConnection(connStr);
            Conn.Open();
            SqlCommand cmdToDb = new SqlCommand(sqlCrtTable, Conn);
            cmdToDb.ExecuteReader();
            Conn.Close();
        }
    }
}
