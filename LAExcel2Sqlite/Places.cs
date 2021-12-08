using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SQLite;

namespace LAExcel2Sqlite
{
    using LAExcel2Sqlite.Core;

    public class states
    {
        [PrimaryKey, AutoIncrement]
        public int _id { get; set; }
        public string state { get; set; }

        public override string ToString()
        {
            return state;
        }
    }

    public class Lga
    {
        [PrimaryKey, AutoIncrement]
        public int _id { get; set; }
        public int stateid { get; set; }
        public string lga { get; set; }

        public override string ToString()
        {
            return this.ToString();
        }
    }

    public class countries
    {
        [PrimaryKey, AutoIncrement]
        public int _id { get; set; }
        public string country { get; set; }

        public override string ToString()
        {
            return country;
        }
    }


    public class PlacesDatabase : SQLiteConnection
    {
        private string _path;

        public override string ToString()
        {
            return this._path;
        }
        public PlacesDatabase(string path)
            : base(path)
        {
            _path = path;
        }


        public void InsertStates(List<Ctate> States)
        {
            // states and lga
            foreach (Ctate state in States)
            {
                states s = new states
                {
                    state = state.state
                };

                long sid = InsertX(s);

                foreach (Kga kga in state.Lgas)
                {
                    Lga l = new Lga
                    {
                        stateid = (int)sid,
                        lga = kga.lga
                    };

                    InsertX(l);
                }
            }
        }

        public void InsertCountries(List<Sountry> Countries)
        {
            foreach (Sountry ctr in Countries)
            {
                countries c = new countries
                {
                    country = ctr.country
                };

                long cid = InsertX(c);
            }
        }
    }
}
