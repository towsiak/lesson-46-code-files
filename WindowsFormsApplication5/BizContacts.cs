﻿using System;
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
        SqlCommandBuilder commandBuilder;//declare a new sql command builder object
        SqlConnection conn;//declares a variable to hold the sql connection
        string selectionStatement = "Select * from BizContacts";//used throughout so it's placed here

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
            GetData(selectionStatement);
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
                dataGridView1.Columns[0].ReadOnly = true;//this helps prevent the id field from being changed
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);//show a useful message to the user of the program
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            SqlCommand command;//declares a new sql command object
            //field names in the table
            string insert = @"insert into BizContacts(Date_Added, Company, Website, Title, First_Name, Last_Name,Address,
                                                      City,State,Postal_Code,Mobile,Notes) 

                              values(@Date_Added, @Company, @Website, @Title, @First_Name, @Last_Name,@Address,
                                                      @City,@State,@Postal_Code,@Mobile,@Notes)"; //parameter names

            using (conn = new SqlConnection(connString)) //using allows disposing of low level resources
            {
                try
                {
                    conn.Open();//open the connection
                    command = new SqlCommand(insert, conn);//create the the new sql command object
                    command.Parameters.AddWithValue(@"Date_Added", dateTimePicker1.Value.Date);//read value from form and save to table
                    command.Parameters.AddWithValue(@"Company", txtCompany.Text);//read value from form and save to table
                    command.Parameters.AddWithValue(@"Website", txtWebsite.Text);//read value from form and save to table
                    command.Parameters.AddWithValue(@"Title", txtTitle.Text);//read value from form and save to table
                    command.Parameters.AddWithValue(@"First_Name", txtFName.Text);//read value from form and save to table
                    command.Parameters.AddWithValue(@"Last_Name", txtLName.Text);//read value from form and save to table
                    command.Parameters.AddWithValue(@"Address", txtAddress.Text);//read value from form and save to table
                    command.Parameters.AddWithValue(@"City", txtCity.Text);//read value from form and save to table
                    command.Parameters.AddWithValue(@"State", txtState.Text);//read value from form and save to table
                    command.Parameters.AddWithValue(@"Postal_Code", txtPostalCode.Text);//read value from form and save to table
                    command.Parameters.AddWithValue(@"Mobile", txtMobile.Text);
                    command.Parameters.AddWithValue(@"Notes", txtNotes.Text);
                    command.ExecuteNonQuery();//push stuff into the table
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);//if there is something wrong, show the user a message
                }
            }
            GetData(selectionStatement);
            dataGridView1.Update();//redraws the data grid view so the new record is visible on the bottom

        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            commandBuilder = new SqlCommandBuilder(dataAdapter);
            dataAdapter.UpdateCommand = commandBuilder.GetUpdateCommand();//get the update command
            try
            {
                bindingSource1.EndEdit();//updates the table that is in memory in our program
                dataAdapter.Update(table);//actually updates the data base
                MessageBox.Show("Update Successful!");//confirms to user update is saved to actual table in sql server
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);//show message to the user
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            
            DataGridViewRow row = dataGridView1.CurrentCell.OwningRow;//grab a reference to the current row
            string value = row.Cells["ID"].Value.ToString();//grab the value from the id field of the selected record
            string fname = row.Cells["First_Name"].Value.ToString();//grab the value from the field name field of the selected record
            string lname = row.Cells["Last_Name"].Value.ToString();//grab the value from the last name field of the selected record
            DialogResult result = MessageBox.Show("Do you really want to delete " + fname + " " + lname + ", record " + value, "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            string deleteState=@"Delete from BizContacts where id = '"+value+"'";//this is the sql to delete the records from the sql table

            if(result==DialogResult.Yes) //check whether user really wants to delete records
            {
                using (conn = new SqlConnection(connString))
                {
                    try
                    {
                        conn.Open();//try to open connection
                        SqlCommand comm = new SqlCommand(deleteState, conn);
                        comm.ExecuteNonQuery();//this line actually causes the deletion to run
                        GetData(selectionStatement);//get the data again
                        dataGridView1.Update();//redraw the data grid  with updated information
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);//runs when something is wrong with the connection
                    }
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            GetData(selectionStatement);//refill the data and the datagridview
            IEnumerable<DataRow> query;//this variable holds the rows that contains the matched records
            switch(cboSearch.SelectedItem.ToString())
            {
                case "First Name":
                    query = SearchFor("First_Name");//call SearchFor against the first name field
                    if (query.Count() > 0)//ensures that the query actually has some records
                        bindingSource1.DataSource = table = query.CopyToDataTable();
                    break;
                case "Last Name":
                    query = SearchFor("Last_Name");//call SearchFor against the first name field
                    if (query.Count() > 0)//ensures that the query actually has some records
                        bindingSource1.DataSource = table = query.CopyToDataTable();
                    break;

            }
        }
        private IEnumerable<DataRow> SearchFor(string fieldName)
        {
            //code below allows us to examine our table essentially as a collection of rows
            //grab each row, look at the field specified by fieldName inside that
            //if it containcs the value we're searching for, it qualifies and we can displays this as a matched record
            return from contacts in table.AsEnumerable()
                   where contacts.Field<string>(fieldName).ToLower().Contains(txtSearch.Text.ToLower())
                   select contacts;
        }
    }
}


//centralize the string that is the call to get data
//centralize the connection declaration so it can be reused
//find matching records, make a modification, make sure the modification is saved to the underlying sql table

            //switch (cboSearch.SelectedItem.ToString())
            //    {
            //        case "First Name":
            //                GetData("select * from BizContacts where First_Name like '%" + txtSearch.Text + "%'");
            //              break;
            //        case "Last Name":
            //                GetData("select * from BizContacts where Last_Name like '%" + txtSearch.Text + "%'");
            //            break;
            //        case "Company":
            //             GetData("select * from BizContacts where Company like '%" + txtSearch.Text + "%'");
            //         break;

            //}