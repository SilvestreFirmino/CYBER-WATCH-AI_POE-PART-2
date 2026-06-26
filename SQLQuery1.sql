--creating dare pro_tasks---
-- Database 'pro_tasks' already exists, so we'll use it
--use the pro_tasks database --
use [pro_tasks];
GO

--creating a table [entity]...
--colummns are task_id, task_name, task_description, task_status, task_priority, task_due_date, task_created_at, task_updated_at
-- list of the columns
--task_name data type is varchar() and not null
--task_id datatype int , and auto increment
-- task_name datatype varchar(put the number of letters)
--task_description datatype varchar()
--task_dueDate datatype varchar()
--task_status datatype varchar()
--create table as tasks when doing the POE
if not exists (select * from sys.tables where name = 'demo_tasks')
begin
	create table demo_tasks(
	task_id int primary key identity(1,1),
	task_name varchar(100) not null,
	task_description varchar(255),
	task_due_date varchar(100),
	task_status varchar(50)
	);
end
GO

-- select all from the table demo_tasks
select * from demo_tasks;

