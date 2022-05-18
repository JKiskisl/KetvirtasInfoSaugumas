using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ketvpraktNoforms

{
    class Program
    {

        static void Main(string[] args)
        {
            

            String KEY = "SecretKey1234567";
            String TestPath = "C:\\Users\\smics\\Desktop\\InfosaugumasNoFORMS\\ketvpraktNoforms\\Test.txt";
            AESCrypto aes = new AESCrypto(KEY);

            AESCrypto.Decrypt(TestPath, KEY, "labas");
            #region "Stuff"
            Console.WriteLine("Choose an option: ");
            Console.WriteLine("1. Create a new password");
            Console.WriteLine("2. Search for password");
            Console.WriteLine("3. Renew password");
            Console.WriteLine("4. Delete password");


            Console.WriteLine("0. to exit");
            int option = Convert.ToInt32(Console.ReadLine());
            while (option != 0)
            {
                switch (option)
                {
                    case 1:
                        Console.WriteLine("Create new password");
                        Console.WriteLine("---------------");
                        Console.WriteLine("Password name:");
                        String passwordName = Console.ReadLine();
                        Console.WriteLine("Password:");
                        String password = Console.ReadLine();
                        Console.WriteLine("URL:");
                        String URL = Console.ReadLine();
                        Console.WriteLine("Comment");
                        String Comment = Console.ReadLine();
                        String EncryptedPassword = aes.EncryptText(password);

                        using (StreamWriter writer = new StreamWriter(Path.Combine(TestPath), true))
                        {
                            writer.WriteLine("Password name:" + passwordName);
                            writer.WriteLine("Password: " + EncryptedPassword);
                            writer.WriteLine("URL:" + URL);
                            writer.WriteLine("Comment: " + Comment);
                        }


                        Console.WriteLine("Password saved!");
                        break;
                    case 2:
                        Console.WriteLine("Search password name");
                        String search = Console.ReadLine();

                        string[] words = File.ReadAllText(TestPath).Split(':');
                        bool condition = false;
                        for (int i = 0; i < words.Length; i++)
                        {
                            if (words[i].Contains(search) == true)
                            {
                                condition = true;
                                break;
                            }
                            else
                            {
                                condition = false;
                            }
                        }
                        if (condition == true)
                        {
                            Console.WriteLine("{0} found in file", search);

                            string[] lines = File.ReadAllLines(TestPath);
                            for (int i = 0; i < lines.Length; i++)
                            {
                                var line = lines[i];
                                if (line.Contains(search))
                                {
                                    Console.WriteLine(lines[i++]);
                                    Console.WriteLine(lines[i++]);
                                    Console.WriteLine(lines[i++]);
                                    Console.WriteLine(lines[i++]);
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("not found");
                        }
                        break;
                    case 3:
                        Console.WriteLine("Renew password");
                        String choice = Console.ReadLine();

                        string[] wordss = File.ReadAllText(TestPath).Split(' ');
                        bool conditionn = false;
                        for (int i = 0; i < wordss.Length; i++)
                        {
                            if (wordss[i].Contains(choice) == true)
                            {
                                conditionn = true;
                                break;
                            }
                            else
                            {
                                conditionn = false;
                            }
                        }
                        if (conditionn == true)
                        {
                            Console.WriteLine("{0} found in file", choice);
                            string[] lines = File.ReadAllLines(TestPath);
                            for (int i = 0; i < lines.Length; i++)
                            {
                                var line = lines[i];
                                if (line.Contains(choice))
                                {
                                    Console.WriteLine("Password name: ");
                                    String tempName = Console.ReadLine();
                                    lines[i++] = "Password name: " + tempName;
                                    File.WriteAllLines(TestPath, lines);

                                    Console.WriteLine("Password: ");
                                    String tempPw = Console.ReadLine();
                                    tempPw = aes.EncryptText(tempPw);
                                    lines[i++] = "Password: " + tempPw;
                                    File.WriteAllLines(TestPath, lines);

                                    Console.WriteLine("URL: ");
                                    String tempURL = Console.ReadLine();
                                    lines[i++] = "URL: " + tempURL;
                                    File.WriteAllLines(TestPath, lines);

                                    Console.WriteLine("Comment: ");
                                    String tempComm = Console.ReadLine();
                                    lines[i++] = "Comment: " + tempComm;
                                    File.WriteAllLines(TestPath, lines);
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Not found");
                        }

                        break;
                    case 4:
                        Console.WriteLine("Password deletion");
                        String tempsearch = Console.ReadLine();

                        string[] tempwords = File.ReadAllText(TestPath).Split(' ');
                        bool tempcondition = false;
                        for (int i = 0; i < tempwords.Length; i++)
                        {
                            if (tempwords[i].Contains(tempsearch) == true)
                            {
                                tempcondition = true;
                                break;
                            }
                            else
                            {
                                tempcondition = false;
                            }
                        }
                        if (tempcondition == true)
                        {
                            Console.WriteLine("{0} found in file", tempsearch);

                            Console.WriteLine("Are you sure about deleting this password?");
                            Console.WriteLine("Y/N");
                            String answer = Console.ReadLine();
                            if (answer == "Y")
                            {
                                string[] lines = File.ReadAllLines(TestPath);
                                for (int i = 0; i < lines.Length; i++)
                                {
                                    var line = lines[i];
                                    if (line.Contains(tempsearch))
                                    {
                                        //delete lines
                                        lines[i++] = "";
                                        File.WriteAllLines(TestPath, lines);
                                        lines[i++] = "";
                                        File.WriteAllLines(TestPath, lines);
                                        lines[i++] = "";
                                        File.WriteAllLines(TestPath, lines);
                                        lines[i++] = "";
                                        File.WriteAllLines(TestPath, lines);
                                    }
                                }
                                var removeWhite = File.ReadAllLines(TestPath).Where(arg => !string.IsNullOrWhiteSpace(arg));
                                File.WriteAllLines(TestPath, removeWhite);
                                Console.WriteLine("Succesfully deleted!");
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            Console.WriteLine("not found");
                        }
                        break;
                    case 9:

                        Console.WriteLine("File Encrypted.");
                        break;
                    default:
                        break;
                }
                Console.WriteLine("Choose an option: ");
                option = Convert.ToInt32(Console.ReadLine());
            }
            #endregion

            AESCrypto.Encrypt(TestPath, KEY, "labas");
        }
    }


}
