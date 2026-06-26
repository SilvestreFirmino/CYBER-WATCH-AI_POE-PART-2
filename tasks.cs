using System;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace CYBER_WATCH_AI_POE_PART_2
{
    public class tasks
    {
        //this connection points at the MASTER database
        //it's only ever used for one job: checking if pro_tasks exists yet, and creating it if not
        //(you can't create a database while "inside" a database that doesn't exist yet,
        //so we have to connect to master first to do that part)
        private readonly string master_connection = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True";

        //this is the REAL connection every other method in this class uses
        //note it points at pro_tasks, not master - that was the root cause of the column error
        private readonly string connection = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=pro_tasks;Integrated Security=True";


        //call this once when the app starts
        //step 1: make sure the pro_tasks database itself exists
        //step 2: make sure the demo_tasks table exists inside pro_tasks
        public void CreateTableIfNotExists()
        {//start

            //STEP 1 - create the pro_tasks database if it isn't there yet
            //IMPORTANT: we tell SQL Server exactly where to put the database file,
            //inside the current Windows user's own AppData folder - every account
            //(including restricted lab/student accounts) always has write access there.
            //Without this, CREATE DATABASE falls back to LocalDB's own default location,
            //which can silently fail to create on locked-down lab machines.
            using (SqlConnection connect = new SqlConnection(master_connection))
            {
                try
                {
                    connect.Open();

                    string db_folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CYBER_WATCH_AI_POE_PART_2");
                    Directory.CreateDirectory(db_folder); //does nothing if the folder already exists

                    string mdf_path = Path.Combine(db_folder, "pro_tasks.mdf");

                    string createDbQuery = $@"
                        IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'pro_tasks')
                        BEGIN
                            CREATE DATABASE pro_tasks
                            ON PRIMARY (NAME = pro_tasks, FILENAME = '{mdf_path}')
                        END";

                    using (SqlCommand createDbCommand = new SqlCommand(createDbQuery, connect))
                    {
                        createDbCommand.ExecuteNonQuery();
                    }
                }
                catch (Exception error)
                {
                    MessageBox.Show("Error creating database: " + error.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; //no point trying step 2 if the database itself couldn't be created
                }
            }

            //STEP 2 - now that pro_tasks definitely exists, create demo_tasks inside it if it isn't there yet
            using (SqlConnection connect = new SqlConnection(connection))
            {
                try
                {
                    connect.Open();

                    string createTableQuery = @"
                        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='demo_tasks' AND xtype='U')
                        BEGIN
                            CREATE TABLE demo_tasks (
                                task_id INT IDENTITY(1,1) PRIMARY KEY,
                                task_name NVARCHAR(100) NOT NULL,
                                task_description NVARCHAR(255),
                                task_due_date NVARCHAR(50),
                                task_status NVARCHAR(20)
                            )
                        END";

                    using (SqlCommand createCommand = new SqlCommand(createTableQuery, connect))
                    {
                        createCommand.ExecuteNonQuery();
                    }
                }
                catch (Exception error)
                {
                    MessageBox.Show("Error creating table: " + error.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }//end


        //test the database availability
        public void test_connection()
        {
            using (SqlConnection connect = new SqlConnection(connection))
            {
                try
                {
                    connect.Open();
                    MessageBox.Show("Database Connection Successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception error)
                {
                    MessageBox.Show("Connection Failed: " + error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        //safe insert - uses parameterized SQL so user input can never break out of the query
        public void insert_task(string name, string description, string duedate, string status)
        {
            using (SqlConnection connects = new SqlConnection(connection))
            {
                try
                {
                    connects.Open();
                    string query = "INSERT INTO demo_tasks (task_name, task_description, task_due_date, task_status) VALUES (@name, @desc, @due, @status);";

                    using (SqlCommand run_query = new SqlCommand(query, connects))
                    {
                        run_query.Parameters.AddWithValue("@name", name);
                        run_query.Parameters.AddWithValue("@desc", description);
                        run_query.Parameters.AddWithValue("@due", duedate);
                        run_query.Parameters.AddWithValue("@status", status);

                        run_query.ExecuteNonQuery();
                    }
                }
                catch (Exception error)
                {
                    MessageBox.Show("Insert failed: " + error.Message);
                }
            }
        }


        //load tasks directly into a WPF ListView
        public void load_tasks(ListView view_task)
        {
            view_task.Items.Clear();

            using (SqlConnection connects = new SqlConnection(connection))
            {
                try
                {
                    connects.Open();
                    string query = "SELECT * FROM demo_tasks;";

                    using (SqlCommand run_query = new SqlCommand(query, connects))
                    using (SqlDataReader data_collect = run_query.ExecuteReader())
                    {
                        bool data_found = false;

                        while (data_collect.Read())
                        {
                            data_found = true;
                            string task_id = data_collect["task_id"].ToString();
                            string task_name = data_collect["task_name"].ToString();
                            string task_description = data_collect["task_description"].ToString();
                            string task_duedate = data_collect["task_due_date"].ToString();
                            string task_status = data_collect["task_status"].ToString();

                            view_task.Items.Add($"{task_id} | {task_name} - {task_description} (Due: {task_duedate}) [{task_status}]");
                        }

                        if (!data_found)
                        {
                            view_task.Items.Add("No active tasks found.");
                        }
                    }
                }
                catch (Exception error)
                {
                    view_task.Items.Add("Error loading tasks: " + error.Message);
                }
            }
        }


        //mark a task as done
        public void update_taskStatus(int id)
        {
            using (SqlConnection connects = new SqlConnection(connection))
            {
                connects.Open();
                string query = "UPDATE demo_tasks SET task_status = 'done' WHERE task_id = @id";

                using (SqlCommand run_query = new SqlCommand(query, connects))
                {
                    run_query.Parameters.AddWithValue("@id", id);
                    run_query.ExecuteNonQuery();
                }
            }
        }


        //delete a task
        public void delete_task(int id)
        {
            using (SqlConnection connects = new SqlConnection(connection))
            {
                connects.Open();
                string query = "DELETE FROM demo_tasks WHERE task_id = @id";

                using (SqlCommand run_query = new SqlCommand(query, connects))
                {
                    run_query.Parameters.AddWithValue("@id", id);
                    run_query.ExecuteNonQuery();
                }
            }
        }
    }
}