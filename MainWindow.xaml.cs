using System;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Collections;

using System.IO;// for Directory.GetCurrentDirectory()
using System.Data.SqlClient;// for SQL
//using Microsoft.SqlServer;

// have to reference, but not work
//using Microsoft.SqlServer.Management.Common;
//using Microsoft.SqlServer.Management.Smo;

namespace Simple_SQL
{
    public class Cus
    {
        public string aa; public string bb;
        public Cus(string a, string b) { aa = a; bb = b; }
    }

    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        //
        // sql info
        //
        private static SqlConnection connectionString = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=smalldb;Integrated Security=True;Pooling=False");

        //
        // funtion
        //
        private DataTable Select_From_Table(SqlCommand command, string slc)
        {
            command.CommandText = slc;
            command.Connection = connectionString;
            SqlDataAdapter da = new SqlDataAdapter(command);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public MainWindow()
        {
            InitializeComponent();

            Initial_Function();
        }

        //
        // Initial
        //
        private void Initial_Function()
        {
            connectionString.Open();
            using (SqlCommand command = new SqlCommand())
            {
                try
                {
                    FileInfo file = new FileInfo(Directory.GetCurrentDirectory().Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\iniDB.sql");
                    command.CommandText = file.OpenText().ReadToEnd();
                    command.Connection = connectionString;
                    command.ExecuteNonQuery();

                    // default: SELECT * From Customer ORDER BY c_id (ASC or DESC)
                    // [former ... later]
                    // first order by later ... then order by former
                    //https://www.fooish.com/sql/order-by.html
                    CustomerGrid.ItemsSource = Select_From_Table(command, "SELECT * From Customer").DefaultView;
                    //CustomerGrid.Items.Refresh();

                    AccountGrid.ItemsSource = Select_From_Table(command, "SELECT * From Account").DefaultView;
                    //AccountGrid.Items.Refresh();

                    // by inner join (the same) 
                    CrossGrid.ItemsSource = Select_From_Table(command, "SELECT Customer.name, Account.balance FROM Customer INNER JOIN Account ON Customer.account = Account.account").DefaultView;
                    // by cross join
                    CrossGrid.ItemsSource = Select_From_Table(command, "SELECT Customer.name, Account.balance FROM Customer CROSS JOIN Account WHERE Customer.account = Account.account").DefaultView;
                    //CrossGrid.Items.Refresh();
                }
                catch (SqlException es)
                {
                    // Message
                    MessageBox.Show(es.ToString());//Console.WriteLine(e.ToString());
                }
            }
            connectionString.Close();

        }

        //
        // Button
        //
        private void Click_Service(object sender, RoutedEventArgs e)
        {
            string line;
            string nm = (sender as Button).Name;
            if (nm == "Left_Join")
            {
                line = "SELECT Customer.name, Customer.account, Account.balance FROM Customer LEFT JOIN Account ON Customer.account = Account.account";
            }
            else if (nm == "Right_Join")
            {
                // difference of Customer.name, Account.account
                //           and Customer.name, Customer.account
                line = "SELECT Customer.name, Account.account, Account.balance FROM Customer RIGHT JOIN Account ON Customer.account = Account.account";
            }
            else if (nm == "Full_Join")
            {
                line = "SELECT Customer.name, Customer.account, Account.balance FROM Customer FULL JOIN Account ON Customer.account = Account.account";
            }
            else if (nm == "Above_Average")
            {
                string table = "SELECT Customer.name, Customer.account, Account.balance ";
                table += "FROM Customer INNER JOIN Account ";
                table += "ON Customer.account = Account.account ";

                line = "DECLARE @LOCAL_TABLE TABLE(name varchar(30), account int, balance int);";
                line += "DECLARE @average int;";

                line += "INSERT INTO @LOCAL_TABLE ";
                line += table;

                line += "SET @average = (SELECT AVG(balance) FROM @LOCAL_TABLE);";

                line += table;
                line += "AND Account.balance > @average";
            }
            else
            {
                return;
            }

            connectionString.Open();
            using (SqlCommand command = new SqlCommand())
            {
                try
                {
                    JoinGrid.ItemsSource = Select_From_Table(command, line).DefaultView;
                    JoinGrid.Items.Refresh();
                }
                catch (SqlException es)
                {
                    // Message
                    MessageBox.Show(es.ToString());//Console.WriteLine(e.ToString());
                }
            }
            connectionString.Close();
        }

        //
        // Closed
        //
        private void Form_Closed(object sender, EventArgs e)
        {

            connectionString.Open();
            using (SqlCommand command = new SqlCommand())
            {
                try
                {
                    command.CommandText = "IF OBJECT_ID(N'Customer') IS NOT NULL BEGIN DROP TABLE Customer END;";
                    command.CommandText += "IF OBJECT_ID(N'Account') IS NOT NULL BEGIN DROP TABLE Account END";
                    command.Connection = connectionString;
                    command.ExecuteNonQuery();
                }
                catch (SqlException es)
                {
                    // Message
                    MessageBox.Show(es.ToString());//Console.WriteLine(e.ToString());
                }
            }
            connectionString.Close();
        }
        
    }
}
