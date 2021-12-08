using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Data.OleDb;
using System.Reflection;

using Excel = Microsoft.Office.Interop.Excel;

namespace LAExcel2Sqlite
{
    /// <summary>
    /// linq wrapper class for Excel
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ExcelSheet<T> : IEnumerable<T>
    {
        private string[] cols = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA" };
        private static Excel.Application objApp;
        private static readonly object singletonLock = new object();
        Excel._Workbook objSpread;

        private ExcelProvider provider;
        private List<T> rows;

        private static Excel.Application GetExcelInstance()
        {
            //critical section, which ensures the singleton
            //is thread safe
            lock (singletonLock)
            {
                if (objApp == null)
                {
                    objApp = new Excel.Application();
                }
                return objApp;
            }
        }

        internal ExcelSheet(ExcelProvider provider)
        {
            this.provider = provider;
            rows = new List<T>();
        }

        /// <summary>
        /// Get the ID of the excel sheet
        /// </summary>
        /// <returns></returns>
        private string GetSheetID()
        {
            object[] attr = typeof(T).GetCustomAttributes(typeof(ExcelSheetAttribute), true);
            if (attr.Length == 0)
            {
                throw new InvalidOperationException("ExcelSheetAttribute not found on type " + typeof(T).FullName);
            }
            ExcelSheetAttribute sheet = (ExcelSheetAttribute)attr[0];
            if (sheet.ID == string.Empty)
                return typeof(T).Name;
            return sheet.ID;
        }

        /// <summary>
        /// Get name of excel sheet
        /// </summary>
        /// <returns>string</returns>
        private string GetSheetName()
        {
            object[] attr = typeof(T).GetCustomAttributes(typeof(ExcelSheetAttribute), true);
            if (attr.Length == 0)
            {
                throw new InvalidOperationException("ExcelSheetAttribute not found on type " + typeof(T).FullName);
            }
            ExcelSheetAttribute sheet = (ExcelSheetAttribute)attr[0];
            if (sheet.Name == string.Empty)
                return typeof(T).Name;
            return sheet.Name;
        }

        /// <summary>
        /// Get a list of Columns as mapped in generic class T
        /// </summary>
        /// <returns>List of ExcelColumnAttribute</returns>
        private List<ExcelColumnAttribute> GetColumnList()
        {
            List<ExcelColumnAttribute> lst = new List<ExcelColumnAttribute>();
            foreach (PropertyInfo propInfo in typeof(T).GetProperties())
            {
                object[] attr = propInfo.GetCustomAttributes(typeof(ExcelColumnAttribute), true);
                if (attr.Length > 0)
                {
                    ExcelColumnAttribute col = (ExcelColumnAttribute)attr[0];
                    col.SetProperty(propInfo);
                    lst.Add(col);
                }

            }
            return lst;
        }

        /// <summary>
        /// Get a list of cells as mapped in generic class T
        /// </summary>
        /// <returns>List of ExcelCellAttribute</returns>
        private List<ExcelCellAttribute> GetCellList()
        {
            List<ExcelCellAttribute> lst = new List<ExcelCellAttribute>();
            foreach (PropertyInfo propInfo in typeof(T).GetProperties())
            {
                object[] attr = propInfo.GetCustomAttributes(typeof(ExcelCellAttribute), true);
                if (attr.Length > 0)
                {
                    ExcelCellAttribute cell = (ExcelCellAttribute)attr[0];
                    cell.SetProperty(propInfo);
                    lst.Add(cell);
                }
            }
            return lst;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string BuildSelect()
        {
            string sheet = GetSheetName();
            StringBuilder builder = new StringBuilder();
            foreach (ExcelColumnAttribute col in GetColumnList())
            {
                if (builder.Length > 0)
                {
                    builder.Append(", ");
                }
                builder.Append(col.GetSelectColumn());
            }
            builder.Append(" FROM [");
            builder.Append(sheet);
            builder.Append("$]");
            return "SELECT " + builder.ToString();
        }
        private T CreateInstance()
        {
            return Activator.CreateInstance<T>();
        }
        private void Load()
        {
            if (!provider.OleDB)
            {
            }
            else
            {
                string connectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=    ""Excel 8.0;HDR=YES;""";
                connectionString = string.Format(connectionString, provider.Filepath);
                List<ExcelColumnAttribute> columns = GetColumnList();
                rows.Clear();
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    using (OleDbCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = BuildSelect();
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                T item = CreateInstance();
                                foreach (ExcelColumnAttribute col in columns)
                                {
                                    object val = reader[col.GetSelectColumn()];
                                    string tmp = val.ToString();

                                    if (col.IsFieldStorage())
                                    {
                                        FieldInfo fi = typeof(T).GetField(col.GetStorageName(), BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetField);
                                        
                                        if (!DBNull.Value.Equals(val))
                                            fi.SetValue(item, val);
                                    }
                                    else
                                    {
                                        typeof(T).GetProperty(col.GetStorageName()).SetValue(item, val, null);
                                        //typeof(T).GetProperty(col.GetStorageName()).SetValue();
                                    }
                                }
                                rows.Add(item);
                            }
                        }
                    }
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            Load();
            return rows.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            Load();
            return rows.GetEnumerator();
        }

        private int GetRowCol(string s, ref int col)
        {
            string prefix = null;
            int rtn = -1;
            string tmp = s;
            int j = s.ToCharArray().Length;
            char[] c = s.ToCharArray();
            string qwert = null;

            string w = null;

            while (j > 0)
            {
                tmp = tmp.TrimStart(c[s.ToCharArray().Length - j]);
                w += c[s.ToCharArray().Length - j];
                if (int.TryParse(tmp, out rtn))
                {
                    for (int i = 0; i < tmp.Length; i++)
                        qwert += '0';

                    prefix = w.ToString();
                    //return rtn;
                    break;
                }
                j--;
            }

            j = 1;
            foreach (string q in cols)
            {
                if (q == prefix)
                    break;
                j++;
            }

            col = j;
            return rtn;
        }

    }

    /// <summary>
    /// Excel provider implementation.
    /// </summary>
    public class ExcelProvider
    {
        private bool oledb;
        private string filePath;

        public ExcelProvider()
        {
            oledb = true;
        }

        internal string Filepath
        {
            get { return filePath; }
        }

        internal bool OleDB
        {
            get { return oledb; }
        }

        /// <summary>
        /// Instanciate the excel provider (using oleDB technology).
        /// </summary>
        /// <param name="filePath">path to excel file</param>
        /// <returns>Excel provider</returns>
        public static ExcelProvider Create(string filePath)
        {
            ExcelProvider provider = new ExcelProvider();
            provider.filePath = filePath;
            return provider;
        }

        /// <summary>
        /// Instanciate the excel provider.
        /// </summary>
        /// <param name="filePath">path to excel file</param>
        /// <param name="oleDB">true if OleDb tech is used, else false</param>
        /// <returns>Excel provider</returns>
        public static ExcelProvider Create(string filePath, bool oleDB)
        {
            ExcelProvider provider = new ExcelProvider();
            provider.oledb = oleDB;
            provider.filePath = filePath;
            return provider;
        }

        public ExcelSheet<T> GetSheet<T>()
        {
            return new ExcelSheet<T>(this);
        }
    }
}
