using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows;
using Label = System.Windows.Controls.Label;
using TextBox = System.Windows.Controls.TextBox;
using Newtonsoft.Json;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Orientation = System.Windows.Controls.Orientation;
using MessageBox = System.Windows.Forms.MessageBox;

namespace StudentGrage
{
    public partial class MainWindow : Window
    {
        List<Students> arryStudent = new List<Students>();
        List<int> percentGrade = new List<int>();
        List<string> taskName = new List<string>();
        double courseAverage;
        Course currCourse = new Course();


        Dictionary<string, string> comboBoxPaths = new Dictionary<string, string>(); // Dictionary to store paths for ComboBox items
        string currentSelectedPath; // Path corresponding to the currently selected ComboBox item

        public MainWindow()
        {
            InitializeComponent();
            LoadPreviousFiles();
        }

        // Load button click event
        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog();
            bool? success = fileDialog.ShowDialog();
            if (success == true)
            {
                try
                {
                    string path = fileDialog.FileName;
                    string nameCourse = Path.GetFileNameWithoutExtension(path);
                    tbInfo.Text = path;
                    winTit.Title = nameCourse; //Changing the title of the window according to the file name

                    // Add ComboBox item and path to the dictionary
                    cb1.Items.Add(new ComboBoxItem { Content = nameCourse });
                    comboBoxPaths[nameCourse] = path;

                    // Select the newly added item
                    cb1.SelectedIndex = cb1.Items.Count - 1;

                    currCourse.Name = nameCourse;
                    currCourse.Path = path;

                    usingFunc(path);
                    SaveFileToJson(path); // Convert Excel file to JSON and save
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        // Function to load data from file
        private void usingFunc(string path)
        {
            // Clear existing data
            lbStu.Items.Clear();
            arryStudent.Clear();
            percentGrade.Clear();
            taskName.Clear();

            using (StreamReader sr = new StreamReader(path))
            {
                // Read percentage line
                string[] upLine = sr.ReadLine().Split(",");

                // Populate taskName and percentGrade lists
                for (int i = 4; i < upLine.Length; i++)
                {
                    string[] str = upLine[i].Split("-");
                    taskName.Add(str[0]);
                    Regex regex = new Regex(@"\d+");
                    Match match = regex.Match(upLine[i]);

                    if (match.Success)
                    {
                        int number = int.Parse(match.Value);
                        percentGrade.Add(number);
                    }
                }

                // Read student data
                while (!sr.EndOfStream)
                {
                    string[] studentData = sr.ReadLine().Split(",");
                    Students StudentInfo = new Students()
                    {
                        FirstName = studentData[0],
                        LastName = studentData[1],
                        Id = studentData[2],
                        Year = studentData[3]
                    };
                    for (int i = 4; i < studentData.Length; i++)
                    {
                        if (studentData[i] == "")
                        {
                            studentData[i] = "0";
                        }
                        int num = int.Parse(studentData[i]);
                        if (num >= 0 && num <= 100)
                        {
                            StudentInfo.Grade.Add(num);
                        }
                    }

                    // Calculate the average of a student

                    StudentInfo.Average = StudentInfo.calcStudentAvg(percentGrade);
                
                    arryStudent.Add(StudentInfo);
                }
            }

            // Sort students and then add to the ListBox
            arryStudent.Sort();
            foreach (var student in arryStudent)
            {
                lbStu.Items.Add(student.printLbstu());
            }
            currCourse.PercentList = percentGrade;
            currCourse.StudentsList = arryStudent;
            currCourse.TasksList = taskName;
            currCourse.Path = path;

            courseAverage = CalcAvg(arryStudent);
            tb_nc.Text = currCourse.Name + ", " + "Average:" + courseAverage.ToString("n2");


        }

        // Function to create JSON file path
        public string CreateJson(string fileName)
        {
            string baseProjectDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string projectRoot = Path.GetFullPath(Path.Combine(baseProjectDirectory, @"..\..\.."));
            string targetDirectory = Path.Combine(projectRoot, "JSONFiles");

            DateTime localDate = DateTime.Now;
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }
           
            return Path.Combine(targetDirectory, $"{fileName}_{localDate.Date:dd-MM-yyyy}.json");
        }


        // Function to convert Excel file to JSON and save
        private void SaveFileToJson(string filePath)
        {
            string jsonPath = CreateJson(Path.GetFileNameWithoutExtension(filePath));
            courseToJson(jsonPath, currCourse);
            comboBoxPaths[Path.GetFileNameWithoutExtension(filePath)] = filePath; // Update dictionary with new file path
            SaveComboBoxPaths(); // Save updated ComboBox paths
        }


        // write course class to JSON file
        private static void courseToJson(string filePath, Course course)
        {
            try
            {
                var data = new { Course = course };
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("error:" + ex.Message);
            }
        }

        // load previous ComboBox paths
        private void LoadPreviousFiles()
        {
            // Load ComboBox paths from file, if exists
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ComboBoxPaths.txt");
            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] parts = line.Split('|');
                        cb1.Items.Add(new ComboBoxItem { Content = parts[0] });
                        comboBoxPaths[parts[0]] = parts[1];
                    }
                }
            }
        }


        private void SaveComboBoxPaths()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ComboBoxPaths.txt");
            using (StreamWriter sw = new StreamWriter(path))
            {
                foreach (var kvp in comboBoxPaths)
                {
                    sw.WriteLine($"{kvp.Key}|{kvp.Value}");
                }
            }
        }


        // ListBox selection changed event
        private void lbStu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int curItem = lbStu.SelectedIndex;
            if (curItem >= 0 && curItem < arryStudent.Count)
            {
                tb_stuDet.Text = arryStudent[curItem].printDet();
                PrintTasks(arryStudent[curItem]);
            }
        }


        // Function to calculate Course average
        private double CalcAvg(List<Students> students)
        {
            double total = 0;
            foreach (var student in students)
            {
                total += student.Average;
            }
            return total / students.Count;
        }

        // Function to display student tasks and grades
        private void PrintTasks(Students student1)
        {
            sp_grades.Children.Clear();
            sp_grades.Orientation = Orientation.Vertical;

            for (int i = 0; i < taskName.Count; i++)
            {
                StackPanel taskPanel = new StackPanel();
                taskPanel.Orientation = Orientation.Horizontal;
                taskPanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

                // Label for task name
                Label lblTask = new Label();
                lblTask.Content = taskName[i];
                lblTask.FontWeight = FontWeights.Bold;
                lblTask.Width = 60;
                lblTask.FontSize = 12;
                taskPanel.Children.Add(lblTask);

                // TextBlock for grade
                TextBox tbGrade = new TextBox();
                tbGrade.Width = 80;
                tbGrade.Height = 20;
                tbGrade.Text = student1.Grade[i].ToString();
                tbGrade.TextAlignment = TextAlignment.Center;

                taskPanel.Children.Add(tbGrade);

                // Label for percentage
                Label lblPrecent = new Label();
                lblPrecent.Content = percentGrade[i].ToString() + "%";
                taskPanel.Children.Add(lblPrecent);

                sp_grades.Children.Add(taskPanel);

            }

            Label lblAverage = new Label();
            lblAverage.Content = "Final Grade: " + student1.Average.ToString("n2");
            lblAverage.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            lblAverage.FontWeight = FontWeights.Bold;
            sp_grades.Children.Add(lblAverage);
            
        }

        // Save button click event
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Update the grades in the student object when the button is clicked
            for (int i = 0; i < taskName.Count; i++)
            {
                if (sp_grades.Children[i] is StackPanel taskPanel)
                {
                    if (taskPanel.Children[1] is TextBox tbGrade)
                    {
                        if (int.TryParse(tbGrade.Text, out int newGrade))
                        {
                            if (newGrade > 100)
                            {
                                newGrade = 100;
                                tbGrade.Text = newGrade.ToString();
                            }
                            arryStudent[lbStu.SelectedIndex].Grade[i] = newGrade;
                        }
                        else
                        {
                            MessageBox.Show("enter numeric grade between 0-100");
                            return;
                        }
                    }
                }
            }

            // Recalculate average
            arryStudent[lbStu.SelectedIndex].Average = arryStudent[lbStu.SelectedIndex].calcStudentAvg(percentGrade);
            PrintTasks(arryStudent[lbStu.SelectedIndex]);

            // Update the Course average
            string[] curCourse = tb_nc.Text.Split(',');
            courseAverage = CalcAvg(arryStudent);
            tb_nc.Text = curCourse[0] + ", " + "Average: " + courseAverage.ToString("n2");
            SaveFileToJson(currCourse.Path);
        }

        // Factor button click event
        private void btnFactor_Click(object sender, RoutedEventArgs e)
        {
            FactorWindow factor = new FactorWindow(taskName);
            bool? result = factor.ShowDialog();
            if (result == true)
            {
                int index = taskName.FindIndex(a => a.Contains(factor.Task));
                foreach (var item in arryStudent)
                {
                    if (item.Grade[index] + int.Parse(factor.Points) > 100)
                    {
                        item.Grade[index] = 100;
                    }
                    else
                    {
                        item.Grade[index] += int.Parse(factor.Points);
                    }
                    item.Average = item.calcStudentAvg(percentGrade);
                }
                 string[] curCourse = tb_nc.Text.Split(',');
                 courseAverage += int.Parse(factor.Points);
               //  tb_nc.Text = curCourse[0] + ", " + "Average: " + courseAverage.ToString("n2");
                 SaveFileToJson(currCourse.Path);

                ComboBoxItem selectedItem = (ComboBoxItem)cb1.SelectedItem;
                if (selectedItem != null)
                {
                    string selectedCourse = selectedItem.Content.ToString();
                    if (comboBoxPaths.ContainsKey(selectedCourse))
                    {
                        currentSelectedPath = comboBoxPaths[selectedCourse];
                        LoadCourseFromJson(CreateJson(Path.GetFileNameWithoutExtension(currentSelectedPath)));
                    }
                }

            }
        }

        private void LoadCourseFromJson(string filePath)
        {
            try
            {
                string json = File.ReadAllText(filePath);

                // Deserialize JSON to anonymous object
                var data = JsonConvert.DeserializeAnonymousType(json, new { Course = new Course() });

                // Set current course to the one loaded from JSON
                currCourse = data.Course;

                // Clear existing data
                lbStu.Items.Clear();
                arryStudent.Clear();
                percentGrade.Clear();
                taskName.Clear();

                // Populate lists with data from loaded course
                percentGrade.AddRange(currCourse.PercentList);
                taskName.AddRange(currCourse.TasksList);
                arryStudent.AddRange(currCourse.StudentsList);

                // Display course name and average
                tb_nc.Text = currCourse.Name + ", Average: " + CalcAvg(arryStudent).ToString("n2");

                // Display students in ListBox
                foreach (var student in arryStudent)
                {
                    lbStu.Items.Add(student.printLbstu());
                }

                // Display tasks and grades for selected student
                if (lbStu.SelectedIndex >= 0 && lbStu.SelectedIndex < arryStudent.Count)
                {
                    tb_stuDet.Text = arryStudent[lbStu.SelectedIndex].printDet();
                    PrintTasks(arryStudent[lbStu.SelectedIndex]);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("error: " + e.Message);
            }
        }

        // ComboBox selection changed event
        private void cb1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get the selected item
            ComboBoxItem selectedItem = (ComboBoxItem)cb1.SelectedItem;
            if (selectedItem != null)
            {
                string selectedCourse = selectedItem.Content.ToString();
                if (comboBoxPaths.ContainsKey(selectedCourse))
                {
                    currentSelectedPath = comboBoxPaths[selectedCourse];
                    LoadCourseFromJson(CreateJson(Path.GetFileNameWithoutExtension(currentSelectedPath)));
                }
            }
        }


    }
}
