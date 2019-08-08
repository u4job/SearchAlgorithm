using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SearchAlgorithm
{
    class Program
    {
        static void Main(string[] args)
        {
            //get current directory / project directory
            string startupPath = Directory.GetCurrentDirectory();

            //ask the user for the name of the file
            Console.WriteLine("Put the file you what to load in this folder");
            Console.WriteLine(startupPath);
            Console.WriteLine("After that enter the file name including extension");

            //get the file name from the user
            string fileName = "";
            fileName = Console.ReadLine();

            while (!File.Exists(startupPath + "\\" + fileName))
            {
                Console.WriteLine("File not exists in the folder or you misspelled the name");
                Console.WriteLine("You need to enter the file name with is extension(*.txt)");
                fileName = Console.ReadLine();
            }

            /*------------------------------------------------------------------------*/
            //clear the console and start processing the file
            Console.Clear();

            //check if folder for the file is exists - in that folder we will build the keys for the file
            string dictionaryPath = startupPath + "\\DictionaryFor" + fileName;
            bool needToProcessFile = true;

            if (!Directory.Exists(dictionaryPath))
            {
                Directory.CreateDirectory(dictionaryPath);
            }
            else
            {
                Console.WriteLine("There is dictionary for that file");
                do
                {
                    Console.WriteLine("Press Y to delete it and process again or N to use the existing dictionary");
                    var readKey = Console.ReadKey(true);
                    if (readKey.KeyChar == 'Y' || readKey.KeyChar == 'y')
                    {
                        Directory.Delete(dictionaryPath, true);
                        Directory.CreateDirectory(dictionaryPath);
                        break;
                    }
                    else if (readKey.KeyChar == 'N' || readKey.KeyChar == 'n')
                    {
                        needToProcessFile = false;
                        break;
                    }
                } while (true);
            }

            if (needToProcessFile)
            {
                Console.WriteLine("We process the file, it may take a while");

                //start to read the file line by line
                StreamReader fileToRead = new StreamReader(startupPath + "\\" + fileName);
                string oldLine;

                while ((oldLine = fileToRead.ReadLine()) != null)
                {
                    //re order the file so it will be alphabetically order
                    string newLine = String.Concat(oldLine.OrderBy(c => c)).ToUpper();

                    //check if there is folder with the first letter of the string
                    //if there is no - create one
                    string letterPath = dictionaryPath + "\\" + newLine[0].ToString();
                    if (!Directory.Exists(letterPath))
                    {
                        Directory.CreateDirectory(letterPath);
                    }

                    //check if there is file with the second letter of the string
                    //if there is no - create one
                    string filePath = letterPath + "\\" + newLine[1].ToString() + ".txt";
                    if (!File.Exists(filePath))
                    {
                        File.Create(filePath).Close();
                    }
                    else
                    {
                        newLine = "\r\n" + newLine;
                    }

                    File.AppendAllText(filePath, newLine + ":" + oldLine);
                }

                //close the file
                fileToRead.Close();

                //move over all the new files and sort them alphabetically
                try
                {
                    foreach (string directory in Directory.GetDirectories(dictionaryPath))
                    {
                        foreach (string file in Directory.GetFiles(directory))
                        {
                            var contents = File.ReadAllLines(file);
                            Array.Sort(contents);
                            File.WriteAllLines(file, contents);
                        }
                    }
                }
                catch
                { }

                Console.WriteLine("Processing complete.");
            }

            //get the string the search from the user
            string lineToSearch = "";

            do
            {
                Console.Write("Enter 5 char long string: ");
                lineToSearch = Console.ReadLine();
            } while (lineToSearch.Length < 4 || !Regex.IsMatch(lineToSearch, @"^[a-zA-Z0-9]+$"));

            //order the stirng to search alphabetically
            string newLineToSearch = String.Concat(lineToSearch.OrderBy(c => c)).ToUpper();
            string SearchLetterPath = dictionaryPath + "\\" + newLineToSearch[0].ToString();

            //check if there is index folder with the first letter
            if (!Directory.Exists(SearchLetterPath))
            {
                Console.WriteLine("The line is not in the file.");
                return;
            }

            string SearchFilePath = SearchLetterPath + "\\" + newLineToSearch[1].ToString() + ".txt";
            //check if there is index file with the second letter
            if (!File.Exists(SearchFilePath))
            {
                Console.WriteLine("The line is not in the file.");
                return;
            }
            else
            {
                //move over the index file line by line
                StreamReader IndexFile = new StreamReader(SearchFilePath);
                string readLine;
                while ((readLine = IndexFile.ReadLine()) != null)
                {
                    //if find the string in the line - show it to the user
                    if (readLine.Contains(newLineToSearch))
                    {
                        Console.WriteLine("You enter {0} and we find the string {1} in the file!", lineToSearch, readLine.Split(":")[1]);
                        break;
                    }
                    else
                    {
                        //this is order list so we can stop search of the next letter in the file is bigger then the search string
                        if ((int)readLine[2] > (int)newLineToSearch[2])
                        {
                            Console.WriteLine("The line is not in the file.");
                            break;
                        }
                        else if ((int)readLine[2] == (int)newLineToSearch[2])
                        {
                            if ((int)readLine[3] > (int)newLineToSearch[3])
                            {
                                Console.WriteLine("The line is not in the file.");
                                break;
                            }
                            else if ((int)readLine[3] == (int)newLineToSearch[3])
                            {
                                if ((int)readLine[4] > (int)newLineToSearch[4])
                                {
                                    Console.WriteLine("The line is not in the file.");
                                    break;
                                }
                            }
                        }
                    }
                }

                IndexFile.Close();
            }
        }
    }
}
