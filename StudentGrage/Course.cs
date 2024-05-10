using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StudentGrage
{
    public class Course
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public List<Students> StudentsList { get; set; }
        public List<string> TasksList { get; set; }
        public List<int> PercentList { get; set; }

        public Course(string name, string path)
        {
            Name = name;
            Path = path;
            StudentsList = new List<Students>();
            TasksList = new List<string>();
            PercentList = new List<int>();
        }

        public Course() {
            Name = "";
            Path = "";
            StudentsList = new List<Students>();
            TasksList = new List<string>();
            PercentList = new List<int>();


        }
    }
}
