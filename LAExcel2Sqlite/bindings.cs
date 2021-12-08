using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace LAExcel2Sqlite
{
    /// <summary>
    /// Excel column Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExcelColumnAttribute : Attribute
    {
        private string name;
        private string storage;
        private PropertyInfo propInfo;
        public ExcelColumnAttribute()
        {
            name = string.Empty;
            storage = string.Empty;
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string Storage
        {
            get { return storage; }
            set { storage = value; }
        }
        internal PropertyInfo GetProperty()
        {
            return propInfo;
        }
        internal void SetProperty(PropertyInfo propInfo)
        {
            this.propInfo = propInfo;
        }
        internal string GetSelectColumn()
        {
            if (Name == string.Empty)
            {
                return propInfo.Name;
            }
            return Name;
        }
        internal string GetStorageName()
        {
            if (Storage == string.Empty)
            {
                return propInfo.Name;
            }
            return storage;
        }
        internal bool IsFieldStorage()
        {
            return string.IsNullOrEmpty(storage) == false;
        }
    }

    /// <summary>
    /// Excel sheet Attribute
    /// </summary>
    public class ExcelSheetAttribute : Attribute
    {
        private string name;
        private string id;
        public ExcelSheetAttribute()
        {
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string ID
        {
            get { return id; }
            set { id = value; }
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExcelCellAttribute : Attribute
    {
        private string cell;                // primary cell
        private string scell;               // secondary cell
        private string name;                // name of column
        private string storage;
        private PropertyInfo propInfo;

        public ExcelCellAttribute()
        {
            cell = string.Empty;
            scell = string.Empty;
            name = string.Empty;
            storage = string.Empty;
        }

        public string Cell
        {
            get { return cell; }
            set { cell = value; }
        }

        public string Second_Cell
        {
            get { return scell; }
            set { scell = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Storage
        {
            get { return storage; }
            set { storage = value; }
        }

        internal PropertyInfo GetProperty()
        {
            return propInfo;
        }

        internal void SetProperty(PropertyInfo propInfo)
        {
            this.propInfo = propInfo;
        }

        internal string GetSelectCell()
        {
            if (Cell == string.Empty)
            {
                return propInfo.Name;
            }
            return Cell;
        }

        internal string GetSelectColumn()
        {
            if (Name == string.Empty)
            {
                return propInfo.Name;
            }
            return Name;
        }

        internal string GetStorageName()
        {
            if (Storage == string.Empty)
            {
                return propInfo.Name;
            }
            return storage;
        }

        internal bool IsFieldStorage()
        {
            return string.IsNullOrEmpty(storage) == false;
        }
    }

}
