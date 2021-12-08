using System;
using System.Collections.Generic;
using System.Linq;
//using System.Text.RegularExpressions;
using Path = System.IO.Path;

using SQLite;


namespace LAExcel2Sqlite
{
    using LAExcel2Sqlite.Core;

    class Program
    {

        static void Main(string[] args)
        {
       
            new Program().Run();
        }

        Database _db;
        PlacesDatabase _pdb;

        void Initialize()
        {
            //var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "quizDb");
            //_db = new Database(dbPath);

            var udbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ubiDatabase");
            _pdb = new PlacesDatabase(udbPath);

        }

        void DisplayBanner()
        {
            Console.WriteLine("Questions converter and states, lga & countries");
            Console.WriteLine("Using " + _pdb.ToString());
            Console.WriteLine();
        }

        void DisplayHelp(string cmd)
        {
            Action<string, string> display = (c, h) => { Console.WriteLine("{0} {1}", c, h); };
            var cmds = new SortedDictionary<string, string> {
				{
					"1",
					"\t Run excel to new excel"
				},
				{
					"x",
					"\t Exit program"
				},
				{
					"2",
					"\t Run new excel to sqlite"
				},
                {
                    "3",
                    "\t Run excel to sqlite to countries, states and lgas"
                },
				{
					"help",
					"\t Displays help"
				},
			};
            if (cmds.ContainsKey(cmd))
            {
                display(cmd, cmds[cmd]);
            }
            else
            {
                foreach (var ch in cmds)
                {
                    display(ch.Key, ch.Value);
                }
            }
        }

        void Run()
        {

            var WS = new char[] {
				'\t',
				'\r',
				'\n'
			};

            Initialize();

            DisplayBanner();
            DisplayHelp("");

            DisplaySection();

            for (; ; )
            {
                Console.Write("$ ");
                var cmdline = Console.ReadLine();

                var args = cmdline.Split(WS, StringSplitOptions.RemoveEmptyEntries);
                if (args.Length < 1)
                    continue;

                var cmd = args[0].ToLowerInvariant();

                string file2read = string.Empty;
                if (args.Length > 1)
                    file2read = args[1].ToLowerInvariant();

                string file2write = string.Empty;
                if (args.Length > 2)
                    file2write = args[2].ToLowerInvariant();

                if (cmd == "?" || cmd == "help")
                {
                    DisplayHelp("");
                }
                else if (cmd == "x")
                {
                    break;
                }
                else if (cmd == "1")
                {
                    RunExcelShaper(file2read, file2write);
                }
                else if (cmd == "2")
                {
                    RunImageInserter();
                }
                else if (cmd == "3")
                {
                    RunExcel2States(file2read);
                }
            }

        }

        public void RunImageInserter()
        {
            //string _test = "helo hi <img src=\"62q1\" /> i was just there.";

            _db.TestInsertImage();

        }

        public void RunExcelShaper(string infile, string outfile)
        {
            List<question> questions = new List<question>();
            List<option> options = new List<option>();

            ExcelProvider xprovider = ExcelProvider.Create(infile);
            foreach (question question in (from q in xprovider.GetSheet<question>() select q))
                questions.Add(question);

            foreach (option option in (from o in xprovider.GetSheet<option>() select o))
                options.Add(option);

            var nform = (from q in questions
                        select new Zestion
                        {
                            id = (int)q.ID,
                            ques = q.Question,
                            options = (from o in options
                                       where q.ID == o.QuestionID
                                       select new Optionz { option = o.Option, isAnswer = (o.Answer == null) ? false : true }).ToList()
                        }).ToList();

            _db.InsertQuestions(nform);
        }

        public void RunExcel2States(string infile)
        {
            List<state> states = new List<state>();
            List<lgo> lgas = new List<lgo>();
            List<country> countries = new List<country>();


            ExcelProvider xprovider = ExcelProvider.Create(infile);
            foreach (state state in (from s in xprovider.GetSheet<state>() select s))
                states.Add(state);

            foreach (lgo lga in (from l in xprovider.GetSheet<lgo>() select l))
                lgas.Add(lga);

            var sal = (from s in states
                         select new Ctate
                         {
                             id    = (int)s.ID,
                             state = s.State,
                             Lgas  = (from l in lgas
                                      where s.ID == l.StateID
                                      select new Kga { lga = l.LGA }).ToList()
                             
                         }).ToList();

            _pdb.InsertStates(sal);


            foreach (country country in (from c in xprovider.GetSheet<country>() select c))
                countries.Add(country);

            var ctry = (from c in countries
                        select new Sountry
                        {
                            id = (int)c.ID,
                            country = c.Country
                        }).ToList();

            _pdb.InsertCountries(ctry);
        }

        /// <summary>
        /// 
        /// </summary>
        public void DisplaySection()
        {
            //var section = _pdb.QuerySection(2);

            //if (section == null)
            //{
            //    Console.WriteLine("I don't know about anything");
            //}
            //else
            //{
            //    Console.WriteLine(section.section);
            //}
        }
    }
}
