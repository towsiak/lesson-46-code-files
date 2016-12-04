using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.Sql;

namespace WindowsFormsApplication5
{
    public partial class BizContacts : Form
    {
        string connString = @"Data Source=DESKTOP-4L6NSGO\SQLEXPRESS;Initial Catalog=AddressBook;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        SqlDataAdapter dataAdapter;//this object here allows us to build the connection between the program and the database
        DataTable table;//table to hold the information so we can fill the datagrid view
        public BizContacts()
        {
            InitializeComponent();
        }

        private void BizContacts_Load(object sender, EventArgs e)
        {
            cboSearch.SelectedIndex = 0;//first item in combobox is selected when the form loads
            dataGridView1.DataSource = bindingSource1;//sets the source of the data to be displayed in the grid view
                                                      //also binding source encapsulates the data for the form

            //line below calls a method called GetData
            //The argument is a string that represents an sql query
            //select * from BizContacts means select all the data from the biz contacts table
            GetData("Select * from BizContacts");
        }

        private void GetData(string selectCommand)
        {
            try
            {
                dataAdapter = new SqlDataAdapter(selectCommand, connString);//pass in the select command and the connection string
                table = new DataTable();//make a new data table object
                table.Locale = System.Globalization.CultureInfo.InvariantCulture;
                dataAdapter.Fill(table);//fill the data table
                bindingSource1.DataSource = table;//set the data source on the binding source to the table
            }
            catch(SqlException ex)
            {
                MessageBox.Show(ex.Message);//show a useful message to the user of the program
            }
        }
    }
}
