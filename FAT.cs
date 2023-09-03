using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
namespace AKVLNADKLNV
{
	class FAT
	{
		static private int[] fat_table = new int[1024];
		FAT()
		{
			
		}
		static private string path = @"C:\Users\Yassa\source\repos\OS-PROJECT\bin\Debug\net6.0\V_disk.txt";
		static public void initialize_fat_table()
		{

			fat_table[0] = -1;
			fat_table[1] = 2;
			fat_table[2] = 3;
			fat_table[3] = 4;
			fat_table[4] = -1;
			for (int i = 5; i < 1024; i++)
				fat_table[i] = 0;
			//Console.WriteLine(fat_table[255]);
			//print_fat_table();
		}
		static public void print_fat_table()
		{
			//Console.WriteLine(fat_table[3]);
			//Console.WriteLine(fat_table[255]);
			int[] fat = read_fat_table();
			//Console.WriteLine(fat_table[255]);
			//Console.WriteLine(fat_table[3]);
			for (int i = 0; i < fat.Length; i++)
			{
				Console.WriteLine(i.ToString() + ' ' + fat[i].ToString());
			}
		}
		static public void write_fat_table_in_file()
		{
			using (FileStream write = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
			{
				write.Seek(1024, SeekOrigin.Begin);
				byte[] data = new byte[4096];
				Buffer.BlockCopy(fat_table, 0, data, 0, data.Length);
				write.Write(data, 0, data.Length);
				write.Close();
			}
		}
		static public int[] read_fat_table()
		{

			FileStream read = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
			read.Seek(1024, SeekOrigin.Begin);
			byte[] data = new byte[4096];
			read.Read(data, 0, data.Length);
			Buffer.BlockCopy(data, 0, fat_table, 0,data.Length);
			read.Close();
			return fat_table;
			
		}
		
		static public int get_Avaliable_Block_ID()
		{
			int[] f = read_fat_table();
			//Console.WriteLine(f[4]);
			int index=0;
			for (int i = 0; i < 1024; i++)
			{
				if (f[i] == 0)
                {
					index = i;
					break;
				}					
			}
			return index;
		}
		static public int getValue(int index)
		{
			int[] f = read_fat_table();
			return f[index];
		}
		static public void set_next(int idx, int value)
		{
			fat_table[idx]=value;
		}
		static public int get_Avaliable_Blocks()
		{
			int[] f = read_fat_table();
			int cont = 0;
			for (int i = 5; i < 1024; i++)
			{
				if (f[i] == 0)
				{
					cont++;
				}
			}
			return cont;
		}
		static public int get_free_space()
        {
			return get_Avaliable_Blocks()*1024;
        }

	}
}