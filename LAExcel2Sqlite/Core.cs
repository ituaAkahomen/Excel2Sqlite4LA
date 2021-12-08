using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LAExcel2Sqlite.Core
{
    #region Quiz

    /// <summary>
    /// 
    /// </summary>
    public class Zestion
    {
        public int id { get; set; }
        public string ques { get; set; }
        public List<Optionz> options { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Optionz
    {
        public long id { get; set; }
        public string option { get; set; }
        public bool isAnswer { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [ExcelSheet(Name = "Sheet1")]
    public class question
    {
        private double id;
        private string quest;

        public question()
        {
            this.id = 0;
        }

        [ExcelColumn(Name = "ID", Storage = "id")]
        public double ID
        {
            get { return this.id; }
        }

        [ExcelColumn(Name = "Question", Storage = "quest")]
        public string Question
        {
            get { return this.quest; }
            set { this.quest = value; }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [ExcelSheet(Name = "Sheet2")]
    public class option
    {
        private double id;
        private double questionid;
        private string opt;
        private string answer;

        public option()
        {
            this.id = 0;
        }

        [ExcelColumn(Name = "ID", Storage = "id")]
        public double ID
        {
            get { return this.id; }
        }

        [ExcelColumn(Name = "qid", Storage = "questionid")]
        public double QuestionID
        {
            get { return this.questionid; }
            set { this.questionid = value; }
        }

        [ExcelColumn(Name = "Options", Storage = "opt")]
        public string Option
        {
            get { return this.opt; }
            set { this.opt = value; }
        }

        [ExcelColumn(Name = "ans", Storage = "answer")]
        public string Answer
        {
            get { return this.answer; }
            set { this.answer = value; }
        }
    }

#endregion

    #region Place

    public class Ctate
    {
        public int id { get; set; }
        public string state { get; set; }
        public List<Kga> Lgas { get; set; }
    }

    public class Kga
    {
        public int id { get; set; }
        public string lga { get; set; }
    }

    public class Sountry
    {
        public int id { get; set; }
        public string country { get; set; }
    }

    [ExcelSheet(Name = "Sheet1")]
    public class state
    {
        private double id;
        private string st;

        public state()
        {
            this.id = 0;
        }

        [ExcelColumn(Name = "ID", Storage = "id")]
        public double ID
        {
            get { return this.id; }
        }

        [ExcelColumn(Name = "State", Storage = "st")]
        public string State
        {
            get { return this.st; }
            set { this.st = value; }
        }
    }

    [ExcelSheet(Name = "Sheet2")]
    public class lgo
    {
        private double id;
        private double stateid;
        private string _lgo;

        public lgo()
        {
            this.id = 0;
        }

        [ExcelColumn(Name = "ID", Storage = "id")]
        public double ID
        {
            get { return this.id; }
        }

        [ExcelColumn(Name = "stateid", Storage = "stateid")]
        public double StateID
        {
            get { return this.stateid; }
            set { this.stateid = value; }
        }

        [ExcelColumn(Name = "lga", Storage = "_lgo")]
        public string LGA
        {
            get { return this._lgo; }
            set { this._lgo = value; }
        }
    }

    [ExcelSheet(Name = "Sheet3")]
    public class country
    {
        private double id;
        private string ctry;

        public country()
        {
            this.id = 0;
        }

        [ExcelColumn(Name = "ID", Storage = "id")]
        public double ID
        {
            get { return this.id; }
        }

        [ExcelColumn(Name = "Country", Storage = "ctry")]
        public string Country
        {
            get { return this.ctry; }
            set { this.ctry = value; }
        }    }

    #endregion

}
