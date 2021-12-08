using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;

using SQLite;

namespace LAExcel2Sqlite
{
    using LAExcel2Sqlite.Core;

    public class sections
    {
        [PrimaryKey, AutoIncrement]
        public int _id { get; set; }
        public string section { get; set; }

        public override string ToString()
        {
            return section;
        }
    }

    public class correctAnswers
    {
        [PrimaryKey, AutoIncrement]
        public int _id { get; set; }
        public int qid { get; set; }
        public int aid { get; set; }

        public override string ToString()
        {
            return this.ToString();
        }
    }

    public class questionOptions
    {
        [PrimaryKey, AutoIncrement]
        public int _id { get; set; }
        public int qid { get; set; }
        public string option { get; set; }

        public override string ToString()
        {
            return this.ToString();
        }
    }

    public class questions
    {
        [PrimaryKey, AutoIncrement]
        public int _id { get; set; }
        public int sectionid { get; set; }
        public int difficulty { get; set; }
        public string question { get; set; }
        public byte[] qimage { get; set; }

        public override string ToString()
        {
            return this.ToString();
        }
    }

    public class commonQuestions
    {
        [PrimaryKey, AutoIncrement]
        public int _id { get; set; }
        public string cquestion { get; set; }
        public byte[] qimage { get; set; }

        public override string ToString()
        {
            return this.ToString();
        }
    }

    public class common
    {
        [PrimaryKey, AutoIncrement]
        public int _id { get; set; }
        public int cid { get; set; }
        public int qid { get; set; }

        public override string ToString()
        {
            return this.ToString();
        }
    }

    public class imagesTable
    {
        [PrimaryKey, AutoIncrement]
        public int _id { get; set; }
        public string qname { get; set; }
        public byte[] qimage { get; set; }

        public override string ToString()
        {
            return this.ToString();
        }
    }

    public class Database : SQLiteConnection
    {
        private string _path;
        private string _test = "Hello hi <img src=\"62q1\" /> bye fuck.";
        private Regex _regex = new Regex(@"<img\W+src=\W+(?<source>\w*)\W+/>");

        public override string ToString()
        {
            return this._path;
        }
        public Database(string path)
            : base(path)
        {
            _path = path;
        }

        public imagesTable QueryImage(int id)
        {
            return (from i in Table<imagesTable>()
                    where i._id == id
                    select i).FirstOrDefault();
        }

        public IEnumerable<imagesTable> QueryAllImages()
        {
            return from i in Table<imagesTable>()
                   orderby i._id
                   select i;
        }

        public sections QuerySection(int id)
        {
            return (from s in Table<sections>()
                    where s._id == id
                    select s).FirstOrDefault();
        }

        public IEnumerable<sections> QueryAllSections()
        {
            return from s in Table<sections>()
                   orderby s._id
                   select s;
        }

        public correctAnswers QueryCorrectAnswer(int id)
        {
            return (from c in Table<correctAnswers>()
                    where c._id == id
                    select c).FirstOrDefault();
        }

        public IEnumerable<correctAnswers> QueryAllCorrectAnswers()
        {
            return from c in Table<correctAnswers>()
                   orderby c._id
                   select c;
        }

        public commonQuestions QueryQuestionOption(int id)
        {
            return (from s in Table<commonQuestions>()
                    where s._id == id
                    select s).FirstOrDefault();
        }

        public IEnumerable<commonQuestions> QueryAllQuestionOptions()
        {
            return from s in Table<commonQuestions>()
                   orderby s._id
                   select s;
        }

        public questions QueryQuestion(int id)
        {
            return (from q in Table<questions>()
                    where q._id == id
                    select q).FirstOrDefault();
        }

        public IEnumerable<questions> QueryAllQuestions()
        {
            return from q in Table<questions>()
                   orderby q._id
                   select q;
        }

        public commonQuestions QueryCommonQuestion(int id)
        {
            return (from cq in Table<commonQuestions>()
                    where cq._id == id
                    select cq).FirstOrDefault();
        }

        public IEnumerable<commonQuestions> QueryAllCommonQuestions()
        {
            return from cq in Table<commonQuestions>()
                   orderby cq._id
                   select cq;
        }

        public common QueryCommon(int id)
        {
            return (from c in Table<common>()
                    where c._id == id
                    select c).FirstOrDefault();
        }

        public IEnumerable<common> QueryAllCommons()
        {
            return from c in Table<common>()
                   orderby c._id
                   select c;
        }

        public void TestInsertImage()
        {
            // instead get a list of all the images in the dir, then find source there and load it.
            IEnumerable<FileInfo> imgfiles = GetAllFiles(@"C:\Users\Akahomen Itua\Desktop\Imgs");

            MatchCollection matches = _regex.Matches(_test);
            if (matches.Count > 0)
            {
                foreach (Match m in matches)
                {
                    string source = m.Groups["source"].Value;

                    
                    FileInfo imgfile = (from file in imgfiles
                                        where (file.Extension.ToLower() == ".jpg" | file.Extension.ToLower() == ".png") & file.Name.ToLower().Contains(source.ToLower())
                                        select file).FirstOrDefault();

                    // we've got the file, now convert it into blob and store in database table.
                    if (imgfile != null)
                    {
                        byte[] tmp = imageToByteArray(new Bitmap(imgfile.FullName));
                        //images i = new images();
                        //i.qname = source; 
                        imagesTable i = new imagesTable()
                        {
                            qname = source,
                            qimage = imageToByteArray(new Bitmap(imgfile.FullName))
                        };

                        // add image to the database
                        long imid = InsertX(i);
                    }
                }
            }

        }

        public void InsertQuestions(List<Zestion> questions)
        {
            foreach (Zestion qu in questions)  {
                questions q = new questions
                {
                    difficulty = 1,
                    sectionid = 1,
                    question = qu.ques
                };
                               
                // instead get a list of all the images in the dir, then find source there and load it.
                IEnumerable<FileInfo> imgfiles = GetAllFiles(@"C:\Users\Akahomen Itua\Desktop\Imgs");

                MatchCollection matches = _regex.Matches(qu.ques);
                if (matches.Count > 0)
                {
                    foreach (Match m in matches)
                    {
                        string source = m.Groups["source"].Value;

                        FileInfo imgfile = (from file in imgfiles
                                            where (file.Extension.ToLower() == ".jpg" | file.Extension.ToLower() == ".png") & file.Name.ToLower().Equals(source.ToLower())
                                            select file).FirstOrDefault();

                        // we've got the file, now convert if into blob and store in database table.
                        if (imgfile != null)
                        {
                            imagesTable i = new imagesTable()
                            {
                                qname = imgfile.Name,
                                qimage = imageToByteArray(new Bitmap(imgfile.FullName))
                            };

                            // add image to the database
                            Insert(i);
                        }
                    }
                }


                long _qid = InsertX(q);
                
                foreach (Optionz oz in qu.options) {
	                questionOptions o = new questionOptions
                    {
                        option = oz.option,
                        qid = (int)_qid
                    };

                    long _oid = InsertX(o);
                    oz.id = _oid;
                }

                foreach (Optionz az in qu.options.Where(n => n.isAnswer)) {
                    correctAnswers ca = new correctAnswers
                    {
                        qid = (int)_qid,
                        aid = (int)az.id
                    };

                    InsertX(ca);
                }
            }
        }

        /// <summary>
        /// Method to convert Bitmap to byte array.
        /// </summary>
        /// <param name="imageIn">bitmap to convert to byte array</param>
        /// <returns>byte array of passport image</returns>
        private byte[] imageToByteArray(Bitmap imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

            return ms.ToArray();
        }

        // Get text files
        static IEnumerable<System.IO.FileInfo> GetAllFiles(string path)
        {
            if (!System.IO.Directory.Exists(path))
                throw new System.IO.DirectoryNotFoundException();

            string[] fileNames = null;
            List<System.IO.FileInfo> files = new List<System.IO.FileInfo>();

            fileNames = System.IO.Directory.GetFiles(path, "*.*", System.IO.SearchOption.AllDirectories);
            foreach (string name in fileNames)
            {
                files.Add(new System.IO.FileInfo(name));
            }
            return files;
        }

        //public IEnumerable<Valuation> QueryValuations(Stock stock)
        //{
        //    return Table<Valuation>().Where(x => x.StockId == stock.Id);
        //}
        //public Valuation QueryLatestValuation(Stock stock)
        //{
        //    return Table<Valuation>().Where(x => x.StockId == stock.Id).OrderBy(x => x.Time).Take(1).FirstOrDefault();
        //}

        //public void UpdateStock(string stockSymbol)
        //{
        //    //
        //    // Ensure that there is a valid Stock in the DB
        //    //
        //    var stock = QueryStock(stockSymbol);
        //    if (stock == null)
        //    {
        //        stock = new Stock { Symbol = stockSymbol };
        //        Insert(stock);
        //    }

        //    //
        //    // When was it last valued?
        //    //
        //    var latest = QueryLatestValuation(stock);
        //    var latestDate = latest != null ? latest.Time : new DateTime(1950, 1, 1);

        //    //
        //    // Get the latest valuations
        //    //
        //    try
        //    {
        //        var newVals = new YahooScraper().GetValuations(stock, latestDate + TimeSpan.FromHours(23), DateTime.Now);
        //        foreach (var v in newVals)
        //        {
        //            Insert(v);
        //        }
        //    }
        //    catch (System.Net.WebException ex)
        //    {
        //        Console.WriteLine(ex);
        //    }
        //}
    }

}
