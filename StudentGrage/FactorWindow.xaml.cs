using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MessageBox = System.Windows.Forms.MessageBox;

namespace StudentGrage
{
    /// <summary>
    /// Interaction logic for FactorWindow.xaml
    /// </summary>
    public partial class FactorWindow : Window
    {
        private List<string> myList;
        public string Task { get; set; }
        public string Points { get; set; }
        public FactorWindow()
        {
            InitializeComponent();
        }

      

        public FactorWindow(List<string> list)
        {
            InitializeComponent();
            myList = list;
            foreach (var item in myList)
            {
                taskList.Items.Add(item);
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
           if (taskList.SelectedItem == null || tb_factor.Text == "")
            {
                MessageBox.Show("Please select a key and enter number of points to factor");
                return;
            }
            DialogResult = true;
            Task = taskList.SelectedItem.ToString();
            //validate input is a number between 0-100
            if (int.TryParse(tb_factor.Text, out int result))
            {
                if (result >= 0 && result <= 100)
                {
                    Points = tb_factor.Text;
                }
                else
                {
                    MessageBox.Show("enter points between 0-100");
                    return;
                }
            }
            else
            {
                MessageBox.Show("enter points between 0-100");
                return;
            }
            Points = tb_factor.Text;
        }
    }
}
