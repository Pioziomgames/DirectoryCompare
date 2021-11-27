using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DirectoryCompare
{
    class Program
    {

        public static string ToHexString(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
                sb.Append(b.ToString("x2").ToLower());

            return sb.ToString();
        }

        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Directory Compare by pioziomgames#5557");
                Console.WriteLine("Compares files in 2 directories\n");
                Console.WriteLine("usage:");
                Console.WriteLine("         DirectoryCompare.exe [oldPath] [newPath] (optional)[outputListFileName] or -s\n");
                Console.WriteLine("         -s do not save additional details to a file");
                Console.WriteLine("Press any key to continue");
                Console.ReadKey();
                return;
            }
            bool saveOutput = true;
            string outputPath = Environment.CurrentDirectory + "\\DirectoryCompare_Output.txt";
            if (args.Length > 2)
            {
                if (args[2].ToLower() == "-x" || args[2].ToLower() == "x")
                {
                    saveOutput = false;
                }
                else
                {
                    outputPath = args[2];
                }
            }

            if (!Directory.Exists(args[0]))
            {
                Console.WriteLine($"Directory {args[0]} does not exist");
                return;
            }
            if (!Directory.Exists(args[1]))
            {
                Console.WriteLine($"Directory {args[1]} does not exist");
                return;
            }

            string[] OldFiles = Directory.GetFiles(args[0], "*", SearchOption.AllDirectories);
            string[] NewFiles = Directory.GetFiles(args[1], "*", SearchOption.AllDirectories);

            List<string> NotFullNewFiles = new List<string>();
            List<string> NotFullOldFiles = new List<string>();

            foreach (string OldFile in OldFiles)
            {
                NotFullOldFiles.Add(OldFile.Replace(args[0], ""));
            }

            foreach (string NewFile in NewFiles)
            {
                NotFullNewFiles.Add(NewFile.Replace(args[1], ""));
            }






            List<string> DeletedFiles = new List<string>();
            List<string> AddedFiles = new List<string>();
            List<string> CurrentFiles = new List<string>();
            List<string> EditedFiles = new List<string>();
            List<string> UnEditedFiles = new List<string>();

            foreach (string OldFile in NotFullOldFiles)
            {
                if (!NotFullNewFiles.Contains(OldFile))
                {
                    DeletedFiles.Add(OldFile);
                }
                else
                {
                    CurrentFiles.Add(OldFile);
                }
            }
            foreach (string NewFile in NotFullNewFiles)
            {
                if (!NotFullOldFiles.Contains(NewFile))
                {
                    AddedFiles.Add(NewFile);
                }
            }

            foreach (string CurrentFile in CurrentFiles)
            {
                string OldContents = File.ReadAllText(args[0] + CurrentFile);
                string NewContents = File.ReadAllText(args[1] + CurrentFile);

                byte[] Oldhash;
                byte[] Newhash;

                using (HashAlgorithm hashAlgorithm = SHA256.Create())
                {
                    Oldhash = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(OldContents));
                }
                using (HashAlgorithm hashAlgorithm = SHA256.Create())
                {
                    Newhash = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(NewContents));
                }
                    if (ToHexString(Oldhash) == ToHexString(Newhash))
                    {
                        UnEditedFiles.Add(CurrentFile);
                    }
                    else
                    {
                        EditedFiles.Add(CurrentFile);
                    }
                

            }

            if (saveOutput)
            {
                string output = $"Result of Comparing:\n{args[0]} to {args[1]}\n\nAdded files: " +
                    $"{AddedFiles.Count()}\nDeleted files: {DeletedFiles.Count()}\nEdited files: " +
                    $"{EditedFiles.Count()}\nUnedited files: {UnEditedFiles.Count()}\n\n";
                if (AddedFiles.Count > 0)
                {
                    output += "\nAdded files:\n";
                    foreach (string File in AddedFiles)
                    {
                        output += File + "\n";
                    }
                }
                if (DeletedFiles.Count > 0)
                {
                    output += "\nDeleted files:\n";
                    foreach (string File in DeletedFiles)
                    {
                        output += File + "\n";
                    }
                }
                if (EditedFiles.Count > 0)
                {
                    output += "\nEdited files:\n";
                    foreach (string File in EditedFiles)
                    {
                        output += File + "\n";
                    }
                }
                if (UnEditedFiles.Count > 0)
                {
                    output += "\nUnedited files:\n";
                    foreach (string File in UnEditedFiles)
                    {
                        output += File + "\n";
                    }
                }
                
                File.WriteAllText(outputPath, output);
            }


            Console.WriteLine("Result of Comparing:");
            Console.WriteLine(args[0]);
            Console.WriteLine("to");
            Console.WriteLine(args[1] + "\n");

            Console.WriteLine($"Added files: {AddedFiles.Count()}");
            Console.WriteLine($"Deleted files: {DeletedFiles.Count()}");
            Console.WriteLine($"Edited files: {EditedFiles.Count()}");
            Console.WriteLine($"Unedited files: {UnEditedFiles.Count()}");

            if (saveOutput)
            {
                Console.WriteLine($"\n\nAdditional Output saved to: {outputPath}");
            }


        }
    }
}
