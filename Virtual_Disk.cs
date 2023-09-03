using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
namespace AKVLNADKLNV
{
	class Virtual_Disk
	{
		static private string path =
			@"C:\Users\Yassa\source\repos\OS-PROJECT\bin\Debug\net6.0\V_disk.txt";
		static public void initializeVdisk()
		{
			FileInfo V_disk_txt = new FileInfo(path);
			if (!V_disk_txt.Exists)
			{
				FileStream V_disk_open = V_disk_txt.Open(FileMode.OpenOrCreate, FileAccess.Write);
				StreamWriter write = new StreamWriter(V_disk_open);
				char c = '0';//for super
				for (int i = 0; i < 1024 * 1024; i++) //write 0s , *s , #s with storage 1024 KB
				{
					if (i == 1024) c = '*';// for fat
					if (i == 4 * 1024) c = '#';//for disk
					write.Write(c);
				}
				Directory root = new Directory("H:", 1, 5,0, null);
				//root.write_Directory();
				Program.current_directory = root;
				Program.current_path = root.name;
				FAT.initialize_fat_table();
				root.Read_Directory();
				FAT.write_fat_table_in_file();
				write.Close();
				V_disk_open.Close();
			}
            else
            {
				
				Directory root = new Directory("H:", 1, 5,0, null);
				//root.write_Directory();
				Program.current_directory=root;
				Program.current_path = root.name;
				//FAT.read_fat_table();
				FAT.write_fat_table_in_file();
				root.Read_Directory();
				

			}
		}
		public static void write_block(byte[] data, int index)
		{
			
			
			using (FileStream write = new FileStream(path, FileMode.Open,FileAccess.ReadWrite))
			{
				write.Seek(1024 * index, SeekOrigin.Begin);
				for (int idx = 0; idx < data.Length; idx++)
				{
					write.WriteByte(data[idx]);
				}
				write.Close();
			}
			
		}
		public static byte[] get_block(int idx)
		{
			byte[] block = new byte[1024];
			using (FileStream read = new FileStream(path, FileMode.Open,FileAccess.ReadWrite))
			{
				read.Seek(idx * 1024, SeekOrigin.Begin);
				read.Read(block, 0, block.Length);
				read.Close();
			}
			return block;
		}
	}
}
