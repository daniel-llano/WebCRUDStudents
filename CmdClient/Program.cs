using DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;

namespace CmdClient
{
    class Program
    {
        #region Declarations 
        /// <summary>
        /// Current student's data filter applied
        /// </summary>
        static string currentFilter = string.Empty;
        /// <summary>
        /// SQL representation of the current student's data filter applied 
        /// </summary>
        static string currentSqlFilter = string.Empty;
        /// <summary>
        /// Default sort field to show student's list
        /// </summary>
        static string currentFieldSort = "updated_on";
        /// <summary>
        /// Default sort direction for the field used to order the student's list
        /// </summary>
        static string currentSortDir = "DESC";
        /// <summary>
        /// Size of the data grid for the student's list on console
        /// </summary>
        static int tableWidth = Console.WindowWidth - 6;
        /// <summary>
        /// The current list of students being show and used for the rest of the operations
        /// </summary>
        static List<Student> students = null;
        /// <summary>
        /// Max number of errors to show to the user at importation time 
        /// </summary>
        const int MaxImportErrorsToShow = 10;
        #endregion
        #region Entry point
        /// <summary>
        /// Entry point of the console application which presents a menu of options to do all the CRUD operations over the students table
        /// </summary>
        /// <param name="args">List of arguments which can be applied to the console client at the time of execution which can be the name of CSV file to import and given filter in the format expected:
        /// Ex: CmdClient.exe Test.csv name=leia
        /// Ex: CmdClient.exe Test.csv type=Kinder
        /// Ex: CmdClient.exe Test.csv type=Elementary gender=female
        /// </param>
        static void Main(string[] args)
        {
            //Setting background and color
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.Clear();

            //Reading the connection string from app.settings
            DBConnection.ConnectionString = ConfigurationManager.ConnectionStrings["SqlServer"].ConnectionString;

            //Verifying if some arguments were passed to the program and processing them
            if (args.Length > 0) {
                ImportCSVFile(args[0]);
                DefineFilter(args);
                if (!string.IsNullOrEmpty(currentFilter))
                    ShowStudents();
            }

            //Showing the menu and doing the main logic of the program 
            char option = '\0';
            while (option != '9') {
                Console.Clear();
                Console.WriteLine("===================== Console Students Administration =====================");
                Console.WriteLine("1. Load students from CSV");
                Console.WriteLine("2. Define a filter to show students");
                Console.WriteLine("3. Clear current filter");
                Console.WriteLine("4. Show students");
                Console.WriteLine("5. Create new student");
                Console.WriteLine("6. Edit student");
                Console.WriteLine("7. Hide student");
                Console.WriteLine("8. Delete student");
                Console.WriteLine("9. Quit");

                Console.WriteLine("Please choose an option number from 1 to 9 and press Enter: ");
                var userInput = Console.ReadLine();
                if (!string.IsNullOrEmpty(userInput))
                {
                    option = userInput[0];
                    switch (option)
                    {
                        case '1':
                            ImportCSVFile();
                            break;
                        case '2':
                            DefineFilter();
                            break;
                        case '3':
                            if (string.IsNullOrEmpty(currentFilter))
                            {
                                ShowContinueMessage("There isn't set any filter right now.");
                            }
                            else
                            {
                                Console.WriteLine(Environment.NewLine + "CURRENT filter is: " + currentFilter);
                                Console.WriteLine("Are you sure that do you want to clear it? (Y/N) [Y]: ");
                                string answer = Console.ReadLine();

                                if (string.IsNullOrEmpty(answer) || answer[0].ToString().ToUpper() == "Y")
                                {
                                    currentFilter = string.Empty;
                                    currentSqlFilter = string.Empty;
                                    currentFieldSort = "updated_on";
                                    currentSortDir = "DESC";
                                }
                            }
                            break;
                        case '4':
                            ShowStudents();
                            break;
                        case '5':
                            CreateStudent();
                            break;
                        case '6':
                            EditStudent();
                            break;
                        case '7':
                            var student = GetStudent();

                            if (student != null)
                            {
                                Console.WriteLine(string.Format("Are you sure that you want to HIDE the student: {0}? (Y/N) [Y]: ", student.ToString()));
                                string value = Console.ReadLine();

                                if (string.IsNullOrEmpty(value) || value[0].ToString().ToUpper() == "Y")
                                {
                                    StudentMapper.Hide(student.Id);
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(value) && !(value[0].ToString().ToUpper() == "Y" || value[0].ToString().ToUpper() == "N"))
                                        ShowContinueMessage("You should choose Y for yes or N for no, please try again.");
                                }
                            }
                            break;
                        case '8':
                            var studentToDelete = GetStudent();

                            if (studentToDelete != null)
                            {
                                Console.WriteLine(string.Format("Are you sure that you want to DELETE the student: {0}? (Y/N) [Y]: ", studentToDelete.ToString()));
                                string value = Console.ReadLine();

                                if (string.IsNullOrEmpty(value) || value[0].ToString().ToUpper() == "Y")
                                {
                                    StudentMapper.Delete(studentToDelete.Id);
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(value) && !(value[0].ToString().ToUpper() == "Y" || value[0].ToString().ToUpper() == "N"))
                                        ShowContinueMessage("You should choose Y for yes or N for no, please try again.");
                                }
                            }
                            break;
                    }
                }
                else {
                    Console.WriteLine("You must type the respective number from 1 to 9 to pick and option from the menu. Press any key to continue ...");
                    Console.ReadKey();
                }
            }

            Console.WriteLine("Thanks for using the Student's console client. Press any key to continue ...");
            Console.ReadKey();
        }
        #endregion
        #region Private methods 
        /// <summary>
        /// Reads from the user the ID and gets the student to edit/update and allows to save changes if any 
        /// </summary>
        private static void EditStudent()
        {
            var student = GetStudent();

            if (student != null)
            {

                Console.WriteLine("The current student data is: " + student.ToString());

                bool changed = false;
                StudentType type = StudentType.Kinder;
                Console.WriteLine("Specify the student type (K)inder, (E)lementary, (H)igh, (U)niversity; introducing the letter enclosed in parenthesis and hitting enter or EMPTY if you want keep original: ");
                string answer = Console.ReadLine();
                if (!string.IsNullOrEmpty(answer))
                {
                    answer = answer[0].ToString();
                    switch (answer)
                    {
                        case "K":
                            break;
                        case "E":
                            type = StudentType.Elementary;
                            break;
                        case "H":
                            type = StudentType.High;
                            break;
                        case "U":
                            type = StudentType.University;
                            break;
                    }

                    if (type != student.Type)
                    {
                        student.Type = type;
                        changed = true;
                    }
                }

                string name = string.Empty;
                Console.WriteLine("Please introduce the student's name or EMPTY if you want keep original: ");
                name = Console.ReadLine();

                if (!string.IsNullOrEmpty(name) && name != student.Name)
                {
                    student.Name = name;
                    changed = true;
                }

                Console.WriteLine("Specify the student gender (M)ale, (F)emale; introducing the letter enclosed in parenthesis and hitting enter or EMPTY if you want keep original: ");

                string gender = string.Empty;
                gender = Console.ReadLine();
                if (!string.IsNullOrEmpty(gender))
                {
                    gender = gender.Trim();
                    if (gender.Length > 0)
                    {
                        gender = gender[0].ToString().ToUpper();
                    }

                    if ((gender == "M" || gender == "F") && gender != student.Gender[0].ToString())
                    {
                        student.Gender = gender == "M" ? "Male" : "Female";
                        changed = true;
                    }
                }

                if (changed)
                {
                    student.UpdatedOn = DateTime.Now;
                    if (StudentMapper.Update(student))
                    {
                        ShowContinueMessage("The student was updated successfully.");
                    }
                    else
                    {
                        ShowContinueMessage("We found and error updating the student please review logs and try again.");
                    }
                }
                else
                {
                    ShowContinueMessage("You did not change any data on the student, so there is nothing to save.");
                }
            }
        }
        /// <summary>
        /// Ask the user to introduce a valid ID of a student and validates if the student requested exists or not
        /// </summary>
        /// <returns>The student selected by the user or null in case of error or not valid</returns>
        private static Student GetStudent()
        {
            long id = -1;
            Console.WriteLine("Please enter the ID of the student that you want to edit (You can get the ID from the list of students in the option 4. Show students): ");
            string value = Console.ReadLine();
            if (!string.IsNullOrEmpty(value))
            {
                if (!long.TryParse(value, out id))
                {
                    ShowContinueMessage("You must enter a valid ID, please look for it and try again.");
                }
            }
            else
            {
                ShowContinueMessage("You must enter a valid ID, please look for it and try again.");
            }

            var student = StudentMapper.GetById(id);

            if (student == null)
            {
                ShowContinueMessage("We could not get any student with the ID provided, please look for a valid one on the list of students and try again.");
            }

            return student;
        }
        /// <summary>
        /// Reads valid data to create a new student and save it to the database
        /// </summary>
        private static void CreateStudent()
        {
            StudentType type = StudentType.Kinder;
            bool validType = false;
            Console.WriteLine("Specify the student type (K)inder, (E)lementary, (H)igh, (U)niversity; introducing the letter enclosed in parenthesis and hitting enter: ");
            string answer = Console.ReadLine();
            if (!string.IsNullOrEmpty(answer))
            {
                answer = answer[0].ToString().ToUpper();
                switch (answer)
                {
                    case "K":
                        validType = true;
                        break;
                    case "E":
                        validType = true;
                        type = StudentType.Elementary;
                        break;
                    case "H":
                        validType = true;
                        type = StudentType.High;
                        break;
                    case "U":
                        validType = true;
                        type = StudentType.University;
                        break;
                }
            }

            string name = string.Empty;
            Console.WriteLine("Please introduce the student's name, it must no be empty: ");
            name = Console.ReadLine();

            Console.WriteLine("Specify the student gender (M)ale, (F)emale; introducing the letter enclosed in parenthesis and hitting enter: ");

            string gender = string.Empty;
            gender = Console.ReadLine();
            if (!string.IsNullOrEmpty(gender))
            {
                gender = gender.Trim();
                if (gender.Length > 0)
                {
                    gender = gender[0].ToString().ToUpper();
                }
            }

            if (validType && !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(gender) && (gender == "M" || gender == "F"))
            {
                Student student = new Student();
                student.Type = type;
                student.Name = name;
                student.Gender = gender == "M" ? "Male" : "Female";
                student.Enabled = true;
                student.UpdatedOn = DateTime.Now;
                if (!StudentMapper.Exists(student))
                {
                    if (StudentMapper.Insert(student))
                    {
                        ShowContinueMessage("The student was added successfully.");
                    }
                    else
                    {
                        ShowContinueMessage("We found and error adding the student please review logs and try again.");
                    }
                }
                else
                {
                    ShowContinueMessage("There is already a student with the same data added, we don't allow duplicates. You can search it for edit if you want.");
                }
            }
            else
            {
                ShowContinueMessage("There is an error with your data. Please verify that the Type is K for Kinder, E for Elementary, H for High, U for University, The name is not empty and Gender is M for Male or F for Female.");
            }
        }
        /// <summary>
        /// Shows a paginated in memory list of students and provides simple commands to navigate trough the list of students.
        /// This method populates the global student's list applying the current SQL filter set.
        /// </summary>
        private static void ShowStudents()
        {
            students = StudentMapper.GetAllWhere(currentSqlFilter, currentFieldSort, currentSortDir, true);
            if (students != null)
            {
                int page = 1;
                int itemsPage = 10;
                
                string navOption = string.Empty;
                int total = students.Count / itemsPage;
                if (students.Count % itemsPage > 0)
                    total++;

                while (string.IsNullOrEmpty(navOption) || navOption != "Q")
                {
                    Console.Clear();
                    string msg = "==== List of Students ====";
                    Console.WriteLine(Environment.NewLine + "{0," + ((Console.WindowWidth / 2) + msg.Length / 2) + "}" + Environment.NewLine, msg);
                    PrintLine();
                    PrintRow("ID", "Name", "Gender", "Type", "Last Update");
                    PrintLine();

                    for (int i = 10 * (page - 1); i < students.Count && i < 10 * page; i++)
                    {
                        var student = students[i];
                        PrintRow(student.Id.ToString(), student.Name, student.Gender, student.Type.ToString(), student.UpdatedOn.ToString("MM-dd-yyyy HH:mm"));
                        PrintLine();
                    }

                    Console.WriteLine(string.Format("You are seeing page {0} of {1}, select one of the following options: " + Environment.NewLine + "(N)ext, (P)revious, (F)irst, (L)ast, (E)xport to CSV, (Q)uit to main menu; " + Environment.NewLine + "introducing the letter enclosed in parenthesis and hitting enter: ", page, total));

                    navOption = Console.ReadLine();
                    if (!string.IsNullOrEmpty(navOption))
                    {
                        navOption = navOption[0].ToString().ToUpper();
                        switch (navOption)
                        {
                            case "N":
                                page++;
                                if (page > total)
                                    page = 1;
                                break;
                            case "P":
                                page--;
                                if (page < 1)
                                    page = total;
                                break;
                            case "F":
                                page = 1;
                                break;
                            case "L":
                                page = total;
                                break;
                            case "E":
                                if (!ExportToCSV()) {
                                    ShowContinueMessage("We could not export the current list of students please review previous messages and logs, and try again.");
                                }
                                break;
                        }
                    }
                }
            }
            else
                ShowContinueMessage("There are no students to show.");
        }
        /// <summary>
        /// Allows user to save a copy of the current list of students to the disk using a CSV file
        /// </summary>
        /// <returns>True if the file was saved successfully, False in other case</returns>
        private static bool ExportToCSV()
        {
            bool result = false;
            if (students == null || students.Count == 0)
            {
                Console.WriteLine("The current list of students is empty, we can not export it.");
                return result;
            }

            try
            {
                Console.WriteLine("Please introduce a valid and writable DIRECTORY: ");
                string directory = Console.ReadLine();
                if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory))
                {
                    string name = "students_" + DateTime.Now.ToString("MM_dd_yyyy_HH_mm_ss") + ".csv";
                    string path = Path.Combine(directory, name);
                    using (StreamWriter sw = new StreamWriter(path))
                    {
                        foreach (var student in students)
                        {
                            sw.WriteLine(student.ToString());
                        }
                        sw.Flush();
                        sw.Close();
                    }
                    ShowContinueMessage("You data was successfully exported in the following path: " + path);
                    result = true;
                }
                else
                    Console.WriteLine("The directory you specified does not exits, please verify and try again.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("We found an error saving your data to the location chosen.");
                Logger.Create().Exception(ex);
            }

            return result;
        }
        /// <summary>
        /// Parse the arguments provided or reads from console some filter provided from the user and also parse that input.
        /// If the filter is valid, the current filter is saved, also is set the corresponding sort to apply and the SQL filter. 
        /// </summary>
        /// <param name="args">Arguments provided from the call to the program in order to create a filter</param>
        private static void DefineFilter(string[] args = null)
        {
            string[] parts = null;

            if (args == null || args.Length < 2)
            {
                Console.WriteLine("Ways of student's filters supported: ");
                Console.WriteLine("- By name, sorted alphabetically. Example: name=John Dough");
                Console.WriteLine("- By student type (Kinder, Elementary, High, University) sorting by date, most recent to least recent. Example: type=Kinder");
                Console.WriteLine("- By gender (male, female) and type sorting by date, most recent to least recent. Example: gender=male type=High");
                if (!string.IsNullOrEmpty(currentFilter))
                    Console.WriteLine(Environment.NewLine + "CURRENT filter applied is: " + currentFilter);
                else
                    Console.WriteLine(Environment.NewLine + "CURRENT filter is empty.");

                Console.WriteLine("Please introduce a filter following above supported patterns: ");
                string filter = Console.ReadLine();
                parts = filter.Trim().Split(' ');
            }
            else
            {
                if (!string.IsNullOrEmpty(currentFilter))
                    Console.WriteLine(Environment.NewLine + "CURRENT  filter applied is: " + currentFilter);
                else
                    Console.WriteLine(Environment.NewLine + "CURRENT  filter is empty.");

                if (args.Length > 1)
                {
                    parts = new string[args.Length - 1];
                    if (args.Length >= 2)
                        parts[0] = args[1];

                    if (args.Length >= 3)
                        parts[1] = args[2];
                }
            }

            if (parts.Length == 1)
            {
                string[] subParts = parts[0].Split('=');
                if (subParts.Length == 2 && subParts[0] == "name")
                {
                    currentFilter = parts[0] + " sorted alphabetically";
                    currentSqlFilter = string.Format("name like '%{0}%'", subParts[1]);
                    currentFieldSort = "name";
                    currentSortDir = "ASC";
                }
                else
                {
                    if (subParts.Length == 2 && subParts[0] == "type")
                    {
                        StudentType type = StudentType.Elementary;
                        bool validType = Enum.TryParse(subParts[1].ToString(), out type);
                        if (validType)
                        {
                            currentFilter = parts[0] + " sorted by date, most recent to least recent";
                            currentSqlFilter = string.Format("type='{0}'", type.ToString());
                            currentFieldSort = "updated_on";
                            currentSortDir = "DESC";
                        }
                    }
                    else
                    {
                        ShowContinueMessage("You didn't fill a valid filter, please try again following the given format.");
                    }
                }
            }
            else
            {
                if (parts.Length == 2)
                {
                    string[] subPartsOne = parts[0].Split('=');
                    string[] subPartsTwo = parts[1].Split('=');
                    if (subPartsOne.Length == 2 && subPartsTwo.Length == 2 && subPartsOne[0] == "gender" && subPartsTwo[0] == "type")
                    {
                        StudentType type = StudentType.Elementary;
                        bool validType = Enum.TryParse(subPartsTwo[1].ToString(), out type);

                        string gender = string.Empty;
                        if (!string.IsNullOrEmpty(subPartsOne[1]))
                            gender = subPartsOne[1].Trim()[0].ToString().ToUpper();

                        if (validType && !string.IsNullOrEmpty(gender) && (gender == "M" || gender == "F"))
                        {
                            currentFilter = parts[0] + " " + parts[1] + " sorted by date, most recent to least recent";
                            currentSqlFilter = string.Format("gender='{0}' AND type='{1}'", gender, type.ToString());
                            currentFieldSort = "updated_on";
                            currentSortDir = "DESC";
                        }
                        else
                        {
                            ShowContinueMessage("You didn't fill a valid filter, please try again following the given format.");
                        }
                    }
                }
                else
                {
                    ShowContinueMessage("You didn't fill a valid filter, please try again following the given format.");
                }
            }
        }
        /// <summary>
        /// Import a CSV file existing on disk, the path of the file to import could be provided by the console call as an argument or could be read from the program. Once validated that we can access the file all students are validated and verified if they don't already exists on the database, the if any student could be imported is saved if the users wants. 
        /// </summary>
        /// <param name="path">Path of the file to import provided as an argument at execution time</param>
        private static void ImportCSVFile(string path = "")
        {
            if (string.IsNullOrEmpty(path))
            {
                Console.WriteLine("Please introduce the path of the CSV file which contains the students to load: ");
                path = Console.ReadLine();
            }

            if (File.Exists(path) && Path.GetExtension(path) == ".csv")
            {
                var content = File.ReadAllLines(path);
                if (content != null && content.Length > 0)
                {
                    bool hasErrors = false;
                    bool hasDuplicates = false;
                    var currentStudents = StudentMapper.GetAllWhere(string.Empty);
                    currentStudents.Sort();
                    var studentsToImport = new List<Student>();
                    int errorCount = 0;
                    for (int position = 0; position < content.Length; position++)
                    {
                        string[] parts = content[position].Split(',');
                        if (parts.Length == 4)
                        {
                            StudentType type = StudentType.Elementary;
                            bool validType = Enum.TryParse(parts[0].ToString(), out type);
                            DateTime updatedOn = DateTime.Now;
                            bool validDateTime = DateTime.TryParseExact(parts[3], "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out updatedOn);
                            string name = null;
                            if (parts[1] != null)
                                name = parts[1].Trim();
                            string gender = null;
                            if (parts[2] != null)
                                gender = parts[2].Trim().ToUpper();

                            if (validType && validDateTime && !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(gender) && (gender == "M" || gender == "F"))
                            {
                                Student student = new Student();
                                student.Type = type;
                                student.Name = name;
                                student.Gender = gender == "M" ? "Male" : "Female";
                                student.Enabled = true;
                                student.UpdatedOn = updatedOn;
                                if (!currentStudents.Contains(student, new StudentsComparer()))
                                {
                                    studentsToImport.Add(student);
                                }
                                else
                                {
                                    string msg = string.Format("There is a duplicated student in row {0}. Please verify the data if you want before import.", position + 1);
                                    if (errorCount < MaxImportErrorsToShow)
                                    {
                                        Console.WriteLine(msg);
                                        Logger.Create().Message(msg);
                                    }
                                    
                                    hasDuplicates = true;
                                    errorCount++;
                                }
                            }
                            else
                            {
                                string msg = string.Format("There is an error with your data in row {0}. Please verify that the Type is (Kinder, Elementary, High, University), The name is not empty, Gender is M(Male) or F(Female) and you placed a valid time stamp in the following format <year><month><day><hour><minute><second> for example the representation for December 31st, 2013 14:59:34 is 20131231145934. All this FOUR values must be in a valid CSV format.", position + 1);
                                if (errorCount < MaxImportErrorsToShow)
                                {
                                    Console.WriteLine(msg);
                                    Logger.Create().Message(msg);
                                }
                                
                                hasErrors = true;
                                errorCount++;
                            }
                        }
                        else
                        {
                            string msg = string.Format("There is an error with your data in row {0}. Please verify that the Type is (Kinder, Elementary, High, University), The name is not empty, Gender is M(Male) or F(Female) and you placed a valid time stamp in the following format <year><month><day><hour><minute><second> for example the representation for December 31st, 2013 14:59:34 is 20131231145934. All this FOUR values must be in a valid CSV format.", position + 1);
                            if (errorCount < MaxImportErrorsToShow)
                            {
                                Console.WriteLine(msg);
                                Logger.Create().Message(msg);
                            }
                            
                            hasErrors = true;
                            errorCount++;
                        }

                        if (position % 10000 == 0)
                            Console.WriteLine("We are processing your data, please wait ...");
                    } //End of the processing CSV file

                    string message = string.Empty;
                    if (hasErrors)
                        message += "We found some errors processing your file, above we show some of them. ";
                    if (hasDuplicates)
                        message += "We found some duplicated students when processing your file, above we show some of them.";

                    if (studentsToImport.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(message))
                            Console.WriteLine(message + string.Format(" The total of errors/duplicates found was {0}. Even if you have errors and duplicates we found some students which can be imported.", errorCount));

                        Console.WriteLine(string.Format("You have {0} students to import, do you wish to insert them to the database? (Y/N) [Y]: ", studentsToImport.Count));
                        string answer = Console.ReadLine();

                        if (string.IsNullOrEmpty(answer) || answer[0].ToString().ToUpper() == "Y")
                        {
                            int count = 0;
                            foreach (var student in studentsToImport)
                            {
                                if (StudentMapper.InsertFull(student))
                                {
                                    count++;
                                    if (count % 10000 == 0)
                                        Console.WriteLine("We are saving your data, please wait ...");
                                }
                                else
                                    Logger.Create().Message("We couldn't save the student: " + student.ToString());
                            }

                            ShowContinueMessage(string.Format("{0} students were saved of {1}. {2}", count, studentsToImport.Count, (count < studentsToImport.Count ? "Some students were not saved, please review the log file to see more details." : "")));
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(answer) && !(answer[0].ToString().ToUpper() == "Y" || answer[0].ToString().ToUpper() == "N"))
                                ShowContinueMessage("You should choose Y for yes or N for no, please try again.");
                        }
                    }
                    else
                    {
                        ShowContinueMessage(message + " After the processing of your file we couldn't find any students to import, please review your data and try again.");
                    }
                }
                else
                {
                    ShowContinueMessage("We couldn't read anything from your file, please check it and try again");
                }
            }
            else
            {
                ShowContinueMessage("Please verify your path and try again, be sure to provide a valid CSV file!");
            }
        }
        /// <summary>
        /// Helper method to show a message by console and reads a key from the user
        /// </summary>
        /// <param name="message">The message to be show</param>
        private static void ShowContinueMessage(string message)
        {
            Console.WriteLine(message);
            Console.WriteLine("Press any key to come back to the menu ...");
            Console.ReadKey();
        }
        /// <summary>
        /// Helper method used for the console data grid, draws a line of hyphens to divide rows of the table grid 
        /// </summary>
        private static void PrintLine()
        {
            Console.WriteLine(new string('-', tableWidth));
        }
        /// <summary>
        /// Helper method used for the console data grid, prints a data row of values to the console
        /// </summary>
        /// <param name="columns">The values of the data row to show</param>
        private static void PrintRow(params string[] columns)
        {
            if (columns.Length == 0)
                return; 
            int width = (tableWidth - columns.Length) / columns.Length;
            string row = "|";

            foreach (string column in columns)
            {
                row += CenterAlign(column, width) + "|";
            }

            Console.WriteLine(row);
        }
        /// <summary>
        /// Helper method used for the console data grid, prepares a text given a max with to use, centers it and truncates if necessary 
        /// </summary>
        /// <param name="text">Text to prepare</param>
        /// <param name="width">Max with to use</param>
        /// <returns>The prepared to text to be show</returns>
        private static string CenterAlign(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

            if (string.IsNullOrEmpty(text))
            {
                return new string(' ', width);
            }
            else
            {
                return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
            }
        }
        #endregion
    }
}
