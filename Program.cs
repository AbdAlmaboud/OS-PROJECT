using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static AKVLNADKLNV.FAT;
using static AKVLNADKLNV.Virtual_Disk;
using static AKVLNADKLNV.Directory;
using static AKVLNADKLNV.Directory_entry;
using static AKVLNADKLNV.File_Entry;

namespace AKVLNADKLNV
{
    class Program
    {
      
        static public Directory current_directory;
        static public string current_path;

        static string[]  input(string str)
        {
            bool f = false;
            string y = "";
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] != ' ')
                { 
                    y += str[i];
                    f = false; 
                }
                else
                { 
                    if (f == false)
                    { 
                        y += str[i];
                        f = true;
                    } 
                }
            }
            string[] s = y.Split(' ');
            return s;
        }
        static string get_current_direcrory()
        {
            return current_path;
        }
        static void cls()
        {
            Console.Clear();
        }
        static void help(string par = "")
        {
            if (par == "")
            {
                Console.WriteLine("CLS\t\tCommand used to clear the screen.");
                Console.WriteLine("QUIT\t\tCommand used to quit the shell.");
                Console.WriteLine("HELP\t\tCommand used to provide help information for commands.");
                Console.WriteLine("MD\t\tCommand used to create a new directory.");
                Console.WriteLine("RD\t\tCommand used to remove a directory.");
                Console.WriteLine("CD\t\tCommand used to change the current default directory.");
                Console.WriteLine("DIR\t\tCommand used to list the contents of directory.");
                Console.WriteLine("RENAME\t\tCommand used to rename a file.");
                Console.WriteLine("IMPORT\t\tCommand used to import text file(s) from your computer.");
                Console.WriteLine("EXPORT\t\tCommand used to export text file(s) to your computer.");
                Console.WriteLine("DEL\t\tCommand used to delete one or more files.");
                Console.WriteLine("COPY\t\tCommand used to copy one or more files to another location.");
                Console.WriteLine("TYPE\t\tCommand used to display the contents of a text file.");

            }
            else
            {
                if (par == "cls")
                {
                    Console.WriteLine("CLS\t\tCommand used to clear the screen.");
                }
                else if (par == "quit")
                {
                    Console.WriteLine("QUIT\t\tCommand used to quit the shell.");
                }
                else if (par == "help")
                {
                    Console.WriteLine("HELP\t\tCommand used to provide help information for commands.");
                }
                else if (par == "md")
                {
                    Console.WriteLine("MD\t\tCommand used to create a new directory.");
                }
                else if (par == "rd")
                {
                    Console.WriteLine("RD\t\tCommand used to remove a directory.");
                }
                else if (par == "cd")
                {
                    Console.WriteLine("CD\t\tCommand used to change the current default directory.");
                }
                else if (par == "dir")
                {
                    Console.WriteLine("DIR\t\tCommand used to list the contents of directory.");
                }
                else if (par == "import")
                {
                    Console.WriteLine("IMPORT\t\tCommand used to import text file(s) from your computer.");
                }
                else if (par == "export")
                {
                    Console.WriteLine("EXPORT\t\tCommand used to export text file(s) to your computer.");
                }
                else if (par == "del")
                {
                    Console.WriteLine("DEL\t\tCommand used to delete one or more files.");
                }
                else if (par == "copy")
                {
                    Console.WriteLine("COPY\t\tCommand used to copy one or more files to another location.");
                }
                else if (par == "rename")
                {
                    Console.WriteLine("RENAME\t\tCommand used to rename a file.");
                }
                else if (par == "type")
                {
                    Console.WriteLine("TYPE\t\tCommand used to display the contents of a text file.");
                }
                else
                {
                    Console.WriteLine("This command is not correct.");
                }
            }
        }
        static void exit()
        {
            Environment.Exit(0);
        }
        
        static void md(string argu)
        {
            if (Program.current_directory.Search_Directory(argu) == -1)
            {
                Directory_entry d = new Directory_entry(argu,1,0,0);
                Program.current_directory.dir.Add(d);
                Program.current_directory.write_Directory();
                if(Program.current_directory.perant !=null)
                {
                    Program.current_directory.perant.Update_content(Program.current_directory.get_directory_entry());
                    Program.current_directory.perant.write_Directory();
                }
            }
            else
            {
                Console.WriteLine("This directory already exists.");
            }
            
        }
        
        
        
        static void rd(string argu)
        {
            int index=Program.current_directory.Search_Directory(argu);
            if (index != -1)
            {
                int first_cluster = Program.current_directory.dir[index].F_cluster;
                Directory d = new Directory(argu,1,first_cluster,0,current_directory);
                d.delete_directory();
            }
        }
        
        static void cd(string argu)
        {
            int index = Program.current_directory.Search_Directory(argu);
            if (index != -1)
            {
                int first_cluster = Program.current_directory.dir[index].F_cluster;
                Directory d = new Directory(argu,1,first_cluster,0,current_directory);
                current_directory = d;
                current_path +="\\"+current_directory.name;
            }
            else
            {
                Console.WriteLine("This folder doesn't exist.");
            }
        }
        
        static void dir(string argu="")
        {
            if (argu=="")
            {
                int num_of_files=0;
                int num_of_folders=0;
                int size_of_file=0;
                Console.WriteLine("Directory of " + current_path);
                Console.WriteLine();
                for (int i=0;i<Program.current_directory.dir.Count;i++)
                {
                    if (Program.current_directory.dir[i].attr==0)
                    {
                        int size = Program.current_directory.dir[i].size;
                        string name_of_file = Program.current_directory.dir[i].name;
                        Console.WriteLine("\t"+size + "\t"+name_of_file);
                        num_of_files++;
                        size_of_file+=size;
                    }
                    else
                    {
                        string name_of_folder = Program.current_directory.dir[i].name;
                        Console.WriteLine("\t<DIR>   "+name_of_folder);
                        num_of_folders++;
                    }
                }
                Console.WriteLine();
                Console.WriteLine("\t"+num_of_files + " File(s)\t" + size_of_file + " bytes");
                Console.WriteLine("\t" + num_of_folders + " Dir(s)\t" + get_free_space().ToString() + " bytes free");
            }
            
        }
        static public void import(string argu)
        {


            int idx = argu.LastIndexOf('\\');
            string name = argu.Substring(idx + 1);
            string content = File.ReadAllText(argu);
            int size = content.Length;
            int index = Program.current_directory.Search_Directory(name);
            if (index == -1)
            {
                int fc;
                if (size > 0)
                {
                    fc = get_Avaliable_Block_ID();
                }
                else
                {
                    fc = 0;
                }
                File_Entry f = new File_Entry(name, 0, fc, size, Program.current_directory, content);
                f.writeFileContent();
                Directory_entry d = new Directory_entry(name, 0, fc, size);
                Program.current_directory.dir.Add(d);
                Program.current_directory.write_Directory();
            }
            else
            {
                Console.WriteLine("This file already exists.");
            }

        
            
        }
        
        static public void type(string argu)
        {
            int index = Program.current_directory.Search_Directory(argu);
            if (index != -1)
            {
                int fc = Program.current_directory.dir[index].F_cluster;
                int size = Program.current_directory.dir[index].size;
                string content = String.Empty;
                File_Entry f = new File_Entry(argu, 0, fc, size,Program.current_directory,content);
                f.readFileContent();   
                Console.WriteLine(f.content);
            }
            else
            {
                Console.WriteLine("The System can't find the file");
            }
        }
        static public void export (string src,string des)
        {
            int index = Program.current_directory.Search_Directory(src);
            if (index != -1)
            {
                if (System.IO.Directory.Exists(des))
                {
                    int fc = Program.current_directory.dir[index].F_cluster;
                    int size = Program.current_directory.dir[index].size;
                    string temp=String.Empty;
                    File_Entry f = new File_Entry(src, 0, fc, size, Program.current_directory, temp);
                    f.readFileContent();
                    StreamWriter s_w = new StreamWriter(des + "\\" + src);
                    s_w.Write(f.content);

                }
                else
                {
                    Console.WriteLine("The system cannot find the path specified in computer disk"); 
                }
                
            }
            else
            {
                Console.WriteLine("This file doesn't exist in Virtual Disk.");
            }
        }
        static public void rename(string old_name,string new_name)
        {
            int index1 = Program.current_directory.Search_Directory(old_name);
            if (index1 !=-1)
            {
                int index2 = Program.current_directory.Search_Directory(new_name);
                if (index2==-1)
                {
                    Directory_entry d = Program.current_directory.dir[index1];
                    d.name = new_name;
                    Program.current_directory.write_Directory();
                }
                else
                {
                    Console.WriteLine("Duplicate file name exist.");
                }
            }
            else
            {
                Console.WriteLine("The system cannot find the file specified.");   
            }
        }
        
        static public void del(string f_name)
        {
            int index = Program.current_directory.Search_Directory(f_name);
            if (index !=-1)
            {
                if (Program.current_directory.dir[index].attr==0)
                {
                    int fc=Program.current_directory.dir[index].F_cluster;
                    int size = Program.current_directory.dir[index].size;
                    File_Entry f = new File_Entry(f_name , 0,fc,size,Program.current_directory,null);
                    f.deleteFile();
                }
                else
                {
                    Console.WriteLine("The system cannot find the file specified.");
                }
            }
            else
            {
                Console.WriteLine("The system cannot find the file specified.");
            }

        }
        
                  
       /* 
        static public void copy(string src,string des)
        {
            int index = Program.current_directory.Search_Directory(src);
            if (index != -1)
            {
                if (System.IO.Directory.Exists(des))
                {
                    if (Program.current_directory.ToString() != des)
                    {
                        cd_copy(des);
                        import_copy("E:\\cv\\mina.txt");

                        char ask;
                        Console.WriteLine("Do you want to overide (y/n)");
                        ask = Convert.ToChar(Console.ReadLine());
                        if (ask == 'y')
                        {
                            int first = Program.current_directory.dir[index].F_cluster;
                            int size = Program.current_directory.dir[index].size;

                            Directory_entry d1 = new Directory_entry(src, 0, first, size);
                            dirctory d = new dirctory();
                            //المفروض هنا  اوصل لل و اضيف file جوا directory  directoy المعمول 
                            Program.current_directory.dir.Add(d);
                        }
                    }
                    else
                    {
                        Console.WriteLine("The system cannot find the file specified.");
                    }
                }

                else
                {
                    Console.WriteLine("The system cannot find the file specified.");
                }

            }
       
            /*      
            int index1 = Program.current_directory.Search_Directory(src);
            if (index1 !=-1)
            {
                int idx = des.LastIndexOf('\\');
                string name = des.Substring(idx + 1);
                int index2 = Program.current_directory.Search_Directory(des);
                if (index2 !=-1)
                {
                    Console.WriteLine("Do you you want to overwrite ?\t Enter y for Yes and n for No");
                    string s=Console.ReadLine();
                    if (s=="y")
                    {

                    }
                    
                }
                else
                {

                }
            }
            else
            {
                Console.WriteLine("The system cannot find the path specified.");
            }
            */

        
    
static void command(string[] str)
        {
            if (str[0] == "quit")
            {
                exit();
            }
            else if (str[0] == "help")
            {
                if (str.Length > 1)
                    help(str[1]);
                else
                    help();
            }
            else if (str[0] == "cls")
            {
                cls();
            }
            else if (str[0]=="cd")
            {
                if (str.Length > 1)
                    cd(str[1]);
                else
                    Console.WriteLine("Please enter the directory");
            }
            else if (str[0] == "rd")
            {
                if (str.Length > 1)
                    rd(str[1]);
                else
                    Console.WriteLine("Please enter the directory");
            }
            else if (str[0] == "md")
            {
                
                if (str.Length > 1)
                    md(str[1]);
                else
                    Console.WriteLine("Please enter the directory");
                
            }
            else if (str[0] == "dir")
            {

                if (str.Length > 1)
                    dir(str[1]);
                else
                    dir();

            }
            else if (str[0] == "import")
            {

                if (str.Length > 1)
                    import(str[1]);
                else
                    Console.WriteLine("Please enter the path of file");

            }
            else if (str[0] == "type")
            {

                if (str.Length > 1)
                    type(str[1]);
                else
                    Console.WriteLine("Please enter the file name");

            }
            else if (str[0] == "export")
            {

                if (str.Length > 1)
                    export(str[1],str[2]);
                else
                    Console.WriteLine("Please enter the file names");

            }
            else if (str[0] == "rename")
            {

                if (str.Length > 1)
                    rename(str[1],str[2]);
                else
                    Console.WriteLine("Please enter the file names");

            }
            else if (str[0] == "del")
            {

                if (str.Length > 1)
                    del(str[1]);
                else
                    Console.WriteLine("Please enter the file name");

            }
            /*
            else if (str[0] == "copy")
            {

                if (str.Length > 1)
                    copy(str[1],str[2]);
                else
                    Console.WriteLine("Please enter the file name");

            }
            */
            else
            {
                Console.WriteLine("The command is not found");

            }
        }
        static void Main(string[] args)
		{
			initializeVdisk();
			initialize_fat_table();
            while(true)
            {
                Console.Write(get_current_direcrory() + "\\>");
                string Input = Console.ReadLine();
                string n = Input.ToLower().TrimStart().TrimEnd();
                string[] str = input(n);
                command(str);
            }
            




        }
    }
}
