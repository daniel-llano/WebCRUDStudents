# Web CRUD and Console CRUD for simple table of Students

Example of ASP.NET Web and Console applications for do all CRUD operations on the Student table, which has the Student Type (Kinder, Elementary, High, and University), Student Name, Gender (Male, Female), and Timestamp of last update in the system.

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. See deployment for notes on how to deploy the project on a live system.
You can clone of download code, using the green button above. Once you have the code, you can open it using Visual Studio and compile/run from there, by default is using debug profile, so you can test directly. If you want to publish/install you can change to release. You need access to Internet in order to restore nuget packages used on the project, if you compile and get errors, please verify that nuget packages are being restore. Your firewall might be blocking the connection in some cases.  

### Prerequisites

The actual solution was created, tested and developed using Visual Studio 2015, with .NET Framework 4.5.2 as a target, as a database was used SQL Server Express 2012, but anything greater than SQL Server 2008 will work commercial or express. You can use same version or superior for the tools mentioned, in some cases you will need to run the migration wizard, which will help you open the solution in your local machine.

### Installing

The console program can be installed on a client machine, you need to install the corresponding .NET Framework used as a target, and you can create a simple installer for the program using InnoSetup for example or just copy and paste the CmdClient.exe, DAL.dll, and the CmdClient.exe.config, in the last file you need to specify a valid connection string. 
The web application need to be deployed to IIS 7 or superior, where you need to create a new ASP.NET application and host all the necessary files. This can be done in a Windows OS that supports the necessary IIS version, at the end the Web.config file needs to be updated, changing the connection string, which will point to database server and the specific SQL Server database that will be used. 
Verify that your SQL Server is well configured to accept remote connections from the client machines, also run the * DBScript.sql * file which contains the SQL sentences to create the database and all the necessary elements to be used by the applications. 

### Structure of the Solution 

1.  DAL (Data Access Layer *Class Library*)
	* DBConnection.cs, this class Create a specific type of connection, any connection that would be used must implement the interface IDBConnection.
	* DBParameter.cs, this class a generic parameter for queries or procedures and is independent of database type. It holds the name, the value, the parameter type (IN, OUT), the type of value hold on the parameter (LONG, INTEGER, DECIMAL, BOOLEAN, CHAR, STRING, TEXT, DATE, DATETIME)
	* IDBConnection.cs, base interface that allows any type of database class to be complaint with the factory creator method used in DBConnection class, for any derived class it must have a field to hold the Connection String passed through DBConnection class. Any derived class should have implementation for the following methods: ExecCommand, ExecQuery, ExecStoredProcQuery, ExecScalar, ExecStoredProc, ExecStoredProcAdd, see comments on the interface for more details. 
	* SQLServerConnection, Implementation of the IDBConnection interface for the SQL Server database type. You can refer to its code in order to implement this same class for other RDBMSs, which will need some adjustments for the reading of output parameters and the way to pass parameters. This class has a specific method GetSQLDataType that translate generic DBParameterType to the corresponding SQLDbType; this kind of method is different for each DB type. 
	* Student.cs, Is the class that represents a Student object, holding all necessary fields to represent a student under the logic defined for the project, and also the enumeration StudentType (Kinder, Elementary, High, University) is declared on this class. 
	* StudentMapper.cs, Class which holds all the necessary methods to persist and pull data from database, takes DB tuples and map them to Student objects. The following methods, which have descriptive names, are implement there: Insert, InsertFull, Update, Hide, Delete, GetById, GetAllPaginatedWhere, GetAllWhere, and Exists. 
	* Logger.cs, helper class which uses singleton pattern and allows us to save log messages and exception to a file with for future reference and debugging of the application. 
	* DBScript.sql, SQL Server version of the objects that are necessary to run the projects, it creates the database *Students*, the table *student*, also inserts some test students, it creates the following procedures, which have descriptive names, which refer to the actions performed for each one: addStudent, addFullStudent, delStudent, hideStudent, updStudent, selStudentById, selPaginatedStudentsWhere

1. TestDAL (Test Unit project created on first version in order to test all base logic in the DAL library previous to have any user interface)
	* TestStudents.cs, Test Class, which allows us to test the CRUD operations, performed by the StudentMapper class and at the same time the rest of the logic on the DAL classes. 

1. CmdClient (Console Application project that holds the console client) 
	* Program.cs, this is the class, which holds the Main method the entry point to the program. This implement the logic to read arguments passed at execution time and allow the user to import data from a CSV file and define first filter to apply to the data saved on the database. This class implement all the necessary logic to do all CRUD operations, import and export data from/to CSV. It has an interactive logic that allows the user to do most of the operation in an easy way, and does most of the validations required. You can see comments on this class for further details. 
	* App.config, configuration file were you can find the connection string in order to change it for connect to your local SQL Server instance. 
	* Test.csv, base test file, which can be used to test the importation of data and review the format, expect to import data. 
	* CSV_Database_of_Students.csv, large test case with more than 80000 valid students (also has some non-valid students just for testing), which can be used to test more in deep the applications and see the response time. 

1. WebCRUDStudents (ASP.NET, Web API application client, it has the standard structure of an empty project of this type created with the Visual Studio wizard, in the following comments will be included only the updated/added source files) 
	* App_Start, folder that contains: 
		** BundleConfig.cs, were a new bundle was created to include the libraries used in the view with the students CRUD, which are mustache.js for the templates parsing and rendering on client side, select2.js for the custom combo box for the form, moment.js used by date time picker, bootstrap-datetimepicker.js the date time picker used in the filter by dates part of the search form, and validator.js a custom made validator JavaScript set of methods which support Bootstrap. Their corresponding CSS files for the plugins which need them were include on the Style Bundle. All this files are inside Scripts folder and Content folder. 
		** WebApiConfig.cs, added JSON formatter, since from the Web API all the responses are generated using JSON. 
	* Content, in this folder were added all the styles used by the JavaScript’s widgets used on the UI which are: bootstrap-datetimepicker.css, select2-bootstrap.css and select2.css
	* Scripts, in this folder were added all the JavaScript’s files that necessary for the UI side widgets and logic, which include:
		** Home, this folder contain the studentsLogic.js file which olds all the logic for the students CRUD page, this logic contains the calls to the Web API services and the CRUD operations call too. 
		** bootstrap-datetimepicker.js, widget to show the data time pickers to the user. 
		** select2.js, widget to show the combo boxes to the user, is a replacement for common HTML select boxes, which supports some nice feature like searching, tagging, remote data sets, infinite scrolling, and many other highly used options. 
		** moment.js, library to parse, validate, manipulate, and display dates and times in JavaScript.
		** mustache.js, is an implementation of the mustache template system in JavaScript.
		** validator.js, custom-made library to added easy to use data validator for bootstrap forms. 
	* Controllers, folder that contains the MVC controller and the API controller:
		** HomeController.cs, MVC Controller that was created by default and presents the first page or default page to the users, the students CRUD was created in the Index view served by this controller. 
		** StudentsServiceController.cs, Web API Controller, which has all the necessary methods that can be consumed by the client side AJAX, calls from JavaScript logic. It contains methods with self-descriptive names for perform all CRUD operations. 
	* Models, folder that contains the classes used in the responses to the client side calls, it contains the following classes: 
		** OperationResponse.cs, class that represents a response for the insert, update, hide, delete calls. This response allows client side to detect if any error has occurred and the message of the error. 
		** StudentsResponse.cs, class that hold the fields that are necessary for the search/list of students on client side. This class contains fields to know the total numbers of students for a given call, also the total number of pages, taking by default 10 elements for page, and the list of students to show. 
	* Views, folder that contains all the cshtml files which at the end represents the actual page served to the user. 
		** Home, folder that contains all the view for the HomeController, just the index view was modified, was removed all the default elements, and was replaced by the Add/Update form, the filter form, a couple of mustache templates in order to show the paginated table/grid of students.
		** Shared, folder that contains the layout used to show the views, was updated the file _Layout.cshtml in order to include jQuery before include the student’s logic JavaScript file which uses this library, and to include the custom bundle with the widgets and libraries used.  
	* Global.asax, is an optional file which is used to handling higher level application events such as Application_Start, Application_End, Session_Start, Session_End etc. In this case was used the Application_Start event to set the connection string read from Web.config file and include the JSON formatter. 
	* Web.config, is the main settings and configuration file for web application. Here is where was added the connection string and this needs to be change to point to the right SQL Server instance. 
	
## Authors

* **Daniel Llano** - *Developer* - [daniel-llano](https://github.com/daniel-llano)

## License

This project is licensed under the GNU AFFERO GENERAL PUBLIC LICENSE Version 3, 19 November 2007 - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments
* DAL DB Connection Factory Logic was based on [Factory Method]( http://www.dofactory.com/net/factory-method-design-pattern)
* DAL Student and StudentMapper classes were done based on the [Data Mapper described by Martin Fowler](https://martinfowler.com/eaaCatalog/dataMapper.html)
* Singletons were implemented in base of what is described in the web article [Implementing the Singleton Pattern in C#](http://csharpindepth.com/Articles/General/Singleton.aspx)
* UI was build using the following JavaScript libraries/widgets: 
	** [moment](https://momentjs.com/)
	** [bootstrap-datetimepicker](http://eonasdan.github.io/bootstrap-datetimepicker/)
	** [select2](https://select2.org/)
	** [mustache.js](https://github.com/janl/mustache.js/)

