using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentGrage
{
    public class Students:IComparable
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Id { get; set; }
        public string Year { get; set; }
        public List<int> Grade { get; set; }
        public double Average {  get; set; }
       // public List<int> Precents { get; set; }
        //public List<string> Tasks { get; set; }


        public Students(string firstName, string lastName, string id, string year) 
        {
            FirstName = firstName;
            LastName = lastName;
            Id = id;
            Year = year;
            Grade = new List<int>();
            Average = 0;
           // Precents = new List<int>();
            //Tasks = new List<string>();
        }

        public Students()
        {
            Grade = new List<int>();
        }

        public string printDet()
        {
            return "Name: " + FirstName + "\nLast Name: " + LastName + "\nID: " + Id + "\nYear: " + Year;
        }

        public string printLbstu() 
        {
            return FirstName + " " + LastName + " " + Id;
        }
    

        public double calcStudentAvg(List<int> percent) {
            double sum = 0;
            for (int i = 0; i < Grade.Count; i++)
            {
              
                sum +=(percent[i]/100.0) * Grade[i];
            }
            return sum > 100 ? 100 : sum;
        }

        public int CompareTo(object? obj)
        {
            return FirstName.CompareTo(((Students)obj).FirstName);
        }
    }
}
