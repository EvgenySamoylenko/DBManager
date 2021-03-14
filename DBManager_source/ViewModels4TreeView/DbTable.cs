using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows;

// Step# 2 DbTable class
// Inherit ViewBase class and implement Property changed notification while setting value of each property

namespace DBManager
{
    public class DbTable : ViewModelBase
    {
        public DatabaseInfo dbInfo = null;
        private string _tableName = string.Empty;
        public string TableName
        {
            get
            {
                return _tableName;
            }
            set
            {
                _tableName = value;
                OnPropertyChanged("TableName");
            }
        }

        private List<TableColumn> columns;
        public List<TableColumn> Columns
        {
            get
            {
                return columns;
            }
            set
            {
                columns = value;
                OnPropertyChanged("Columns");
            }
        }
        public DbTable(string tableName)
        {
            TableName = tableName;
            columns = new List<TableColumn>();
        }

        internal void ClearAllInfo()
        {
            TableName = string.Empty;
            columns.Clear();
        }

        internal void AddColumn(TableColumn colInfo)
        {
            columns.Add(colInfo);
            colInfo.tblInfo = this;
        }

        internal void RemoveTableColumn(TableColumn tblColumn)//-------for columns
        {
            int idx = -1;
            for (int i = 0; i < columns.Count; i++)
            {
                if (columns[i].ColumnName == tblColumn.ColumnName)
                {
                    columns.RemoveAt(i);
                    idx = i;
                    break;
                }
            }
            if (idx == -1)
                return;
            var tblColumnList1 = new List<TableColumn>();
            tblColumnList1.AddRange(columns);
            Columns = tblColumnList1;
        }
    }

    public class TableColumn : ViewModelBase
    {
        public DbTable tblInfo = null;//---------- for cols

        private string _columnName = string.Empty;
        public string ColumnName
        {
            get
            {
                return _columnName;
            }
            set
            {
                _columnName = value;
                OnPropertyChanged("ColumnName");
            }
        }

        public string ColumnDetails => string.Format("{0} ({1}, {2})", ColumnName, ColumnType, ColumnLen);

        private string _columnType = string.Empty;
        public string ColumnType
        {
            get
            {
                return _columnType;
            }
            set
            {
                _columnType = value;
                OnPropertyChanged("ColumnType");
            }
        }

        private int _columnLen = 0;
        public int ColumnLen
        {
            get
            {
                return _columnLen;
            }
            set
            {
                _columnLen = value;
                OnPropertyChanged("ColumnLen");
            }
        }

        public TableColumn(string colName, string colType, int colLength)
        {
            ColumnName = colName;
            ColumnType = colType;
            ColumnLen = colLength;
        }
    }
}
