# s23087_Inzynierka_2024

# Project Description
This is web application that allow user to manage his organization warehouse and sales. It split in the 3 parts:

1. .Net Api that allow to communicate with SQL Server and it's databases.
2. Next.js application for user to access the functions.
3. Python scripts run by Windows Task scheduler.

Each of registered user will have their own database created. The app allow to create two types of accounts. One the solo one which target people working in their own organizations without employees and organization one which allow to add users and assigned role to them.

# Prequest to run the appliaction.

1. Setting .env for next.js project

For next.js application to run smoothly you need to create .env file in folder web/handler_b2b containing variables:

<ul>
    <li>SESSION_SECRET - secret key to sign JWT token. Genereted using command 'openssl rand -base64 32'</li>
    <li>API_DEST - Value of .Net Api Endpoint. If you host the api localy on port 5050, then te value would probably be 'http://localhost:5050'</li>
</ul>

2. Create folder named 'database'. In this folder the new databases will be saved.

3. Create folder named 'Logs' in database_comunicator. This is where the errors from Api will be loged.

4. Create nlog.config in database_comunicator. You can create your own way to log errors or you can use template below.

```
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">

	<extensions>
		<add assembly="NLog.Web.AspNetCore"/>
	</extensions>

	<targets>
		<target xsi:type="File" name="mainLogFile" fileName="{**insert_path_to_Logs_file**}\ErrorLog_${shortdate}.log" 
				layout="${longdate}|${level:uppercase=true}|${logger}|${message} ${exception:format=tostring}|url: ${aspnet-request-url}|action: ${aspnet-mvc-action}"/>
		<target name="console" xsi:type="Console" />
	</targets>

	<rules>
		<logger name="*" minlevel="Error" writeTo="mainLogFile" />
		<logger name="*" minlevel="Info" writeTo="console" />
	</rules>
</nlog>
```

5. Set up user secrets for 'database_communicator' and 'database_communicator.Test'. The first one need variables:

<ul>
<li>script:setupDb -> The path to db_setup/db_setup.sql</li>
<li>script:createDb -> The path to db_setup/createDB.sql</li>
<li>ConnectionStrings:flexible -> The connection string where Initial Catalog=db_name</li>
</ul>

The test one needs:

<ul>
<li>testDb -> The connection string to test database</li>
</ul>