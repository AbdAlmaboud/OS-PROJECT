using System;
using System.Collections.Generic;

namespace AKVLNADKLNV
{
	public class Directory : Directory_entry
	{
		public List<Directory_entry> dir = new List<Directory_entry>();
		public Directory perant;

		public Directory()
		{

		}
		public Directory(string name, byte art, int F_C,int f_size, Directory temp) : base(name, art, F_C,f_size)
		{
			this.perant = temp;


		}



		public void write_Directory()
		{
			byte[] DTB = new byte[32 * dir.Count];
			byte[] DEB = new byte[32];
			for (int i = 0; i < dir.Count; i++)
			{
				DEB = dir[i].getBytes();
				for (int j = i * 32; j < 32 * (i + 1); j++)
				{
					DTB[j] = DEB[j % 32];
				}

			}
			int num_of_req_block = (int)Math.Ceiling(DTB.Length / 1024.0);
			int num_no_full_size_block = (DTB.Length / 1024);
			int remainder = DTB.Length % 1024;
			List<byte[]> blocks = new List<byte[]>();
			byte[] temp = new byte[1024];
			for (int i = 0; i < num_no_full_size_block; i++)
			{


				for (int j = 0; j < 1024; j++)
				{
					temp[j] = DTB[j + i * 1024];
				}
				blocks.Add(temp);
			}
			int indexR = (num_no_full_size_block * 1024);

			for (int i = 0; i < remainder; i++, indexR++)
			{

				temp[i] = DTB[indexR];


			}
			if (remainder > 0)
			{
				blocks.Add(temp);
			}
			int fc = 0, lc = -1;
			if (F_cluster != 0)
			{
				fc = F_cluster;
			}
			else
			{
				fc = FAT.get_Avaliable_Block_ID();
				F_cluster = fc;
			}

			for (int i = 0; i < num_of_req_block; i++)
			{
				Virtual_Disk.write_block(blocks[i], fc);
				FAT.set_next(fc, -1);
				if (i + 1 < num_of_req_block)
				{
					lc = FAT.get_Avaliable_Block_ID();
					FAT.set_next(fc, lc);

					fc = FAT.get_Avaliable_Block_ID();
				}
				lc = fc;
			}
			FAT.write_fat_table_in_file();
		}
		public void Read_Directory()
		{
			
				List<byte> ls = new List<byte>();
				byte[] d = new byte[32];
				int fc = 0, Nc;
				
				if (F_cluster != 0)
				{
					fc = F_cluster;
				}
				
				Nc = FAT.getValue(fc);
				do
				{
					ls.AddRange(Virtual_Disk.get_block(fc));
					if (fc != -1)
					{
						Nc = FAT.getValue(fc);
					}
					fc = Nc;
					break;
				}
				while (fc != -1);

				bool flage = false;
				for (int i = 0; i < ls.Count / 32; i++)
				{
					for (int j = 0; j < 32; j++)
					{
						if (ls[j + (i * 32)] == (byte)'#')
						{
							flage = true;
							break;
						}
						d[j] = ls[j + (i * 32)];
					}
					if (flage)
						break;
					if (GetDirectory(d).F_cluster != 0)
						dir.Add(GetDirectory(d));
				}

			
		}


		public int Search_Directory(string name)
		{

			for (int i = 0; i < dir.Count; i++)
			{
				string s = "";
				for (int j = 0; j < dir[i].name.Length; j++)
				{
					if (dir[i].name[j] == '\0')
					{
						break;
					}

					s += dir[i].name[j];
				}

				if (s == name)
				{
					return i;
				}

			}
			return -1;
		}


		public void Update_content(Directory_entry d)
		{

			Read_Directory();
			int index = Search_Directory(d.name.ToString());
			if (index != -1)
			{
				dir.RemoveAt(index);
				dir.Insert(index, d);
			}

		}

		public void delete_directory()
		{
			int index, next;
			if (F_cluster != 0)
			{
				index = F_cluster;
				next = FAT.getValue(index);


				do
				{
					FAT.set_next(index, 0);
					index = next;
					if (index != -1)
					{
						next = FAT.getValue(index);
					}

				} while (index != -1);
			}
			if (perant != null)
			{
				perant.dir.Clear();
				//perant.Read_directory();
				string s = "";

				for (int c = 0; c < name.Length && name[c] != '\0'; c++)
				{
					s += name[c];

				}
				index = perant.Search_Directory(s);
				if (index != -1)
				{
					perant.dir.RemoveAt(index);
					perant.write_Directory();
				}

			}
			FAT.write_fat_table_in_file();

		}
		
	}
}
