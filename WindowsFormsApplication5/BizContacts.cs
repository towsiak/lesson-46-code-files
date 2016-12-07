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
                dataGridView1.Columns[0].ReadOnly = true;//this helps prevent the id field from being changed
                
            }
            catch(SqlException ex)
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

            using (SqlConnection conn = new SqlConnection(connString)) //using allows disposing of low level resources
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
            GetData("Select * from BizContacts");
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
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);//show message to the user
            }
        }
    }
}
