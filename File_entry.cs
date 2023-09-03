using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AKVLNADKLNV
{
    public class File_Entry : Directory_entry
    {
        public static List<byte[]> split_bytes(byte[] bytes)
        {
            List<byte[]> ls = new List<byte[]>();
            if (bytes.Length > 0)
            {
                int number_of_arrays = bytes.Length / 1024;
                int rem = bytes.Length % 1024;
                for (int i = 0; i < number_of_arrays; i++)
                {
                    byte[] b = new byte[1024];
                    for (int j = i * 1024, k = 0; k < 1024; j++, k++)
                    {
                        b[k] = bytes[j];
                    }
                    ls.Add(b);
                }
                if (rem > 0)
                {
                    byte[] b1 = new byte[1024];
                    for (int i = number_of_arrays * 1024, k = 0; k < rem; i++, k++)
                    {
                        b1[k] = bytes[i];
                    }
                    ls.Add(b1);
                }
            }
            else
            {
                byte[] b1 = new byte[1024];
                ls.Add(b1);
            }
            return ls;
        }
    
        public static byte[] string_to_bytes(string s)
        {
            byte[] bytes = new byte[s.Length];
            for (int i = 0; i < s.Length; i++)
            {
                bytes[i] = (byte)s[i];
            }
            return bytes;
        }
        public static string bytes_to_string(byte[] bytes)
        {
            string s = String.Empty;
            for (int i = 0; i < bytes.Length; i++)
            {
                if ((char)bytes[i] != '\0')
                    s += (char)bytes[i];
                else
                    break;
            }
            return s;
        }
        public string content;
        public Directory parent;
        public File_Entry(string name, byte atr, int fc, int file_size, Directory p, string c) : base(name, atr, fc, file_size)
        {
            content = string.Empty;
            if (p != null)
                parent = p;
        }
        public Directory_entry GetDirectory_Entry()
        {
            Directory_entry me = new Directory_entry(new string(this.name), this.attr, this.F_cluster,this.size);
            return me;
        }
        public void writeFileContent()
        {
            byte[] contentBYTES = string_to_bytes(content);
            List<byte[]> bytesls = split_bytes(contentBYTES);
            int clusterFATIndex;
            if (this.F_cluster != 0)
            {
                clusterFATIndex = this.F_cluster;
            }
            else
            {
                clusterFATIndex = FAT.get_Avaliable_Block_ID();
                this.F_cluster = clusterFATIndex;
            }
            int lastCluster = -1;
            for (int i = 0; i < bytesls.Count; i++)
            {
                if (clusterFATIndex != -1)
                {
                    Virtual_Disk.write_block(bytesls[i], clusterFATIndex);
                    FAT.set_next(clusterFATIndex, -1);
                    if (lastCluster != -1)
                        FAT.set_next(lastCluster, clusterFATIndex);
                    lastCluster = clusterFATIndex;
                    clusterFATIndex = FAT.get_Avaliable_Block_ID();
                }
            }
            FAT.write_fat_table_in_file();
        }
        public void readFileContent()
        {
            ///Console.WriteLine(F_cluster);
            if (this.F_cluster != 0)
            {
                content = string.Empty;
                int cluster = this.F_cluster;
                int next = FAT.getValue(cluster);
                List<byte> ls = new List<byte>();
                do
                {
                    ls.AddRange(Virtual_Disk.get_block(cluster));
                    cluster = next;
                    if (cluster != -1)
                        next = FAT.getValue(cluster);
                }
                while (next != -1);
                content = bytes_to_string(ls.ToArray());
            }
        }
        public void deleteFile()
        {
            if (this.F_cluster != 0)
            {
                int cluster = this.F_cluster;
                int next = FAT.getValue(cluster);
                do
                {
                    FAT.set_next(cluster, 0);
                    cluster = next;
                    if (cluster != -1)
                        next = FAT.getValue(cluster);
                }
                while (cluster != -1);
            }
            if (this.parent != null)
            {
                this.parent.Read_Directory();
                int index = this.parent.Search_Directory(new string(this.name));
                if (index != -1)
                {

                    
                    this.parent.dir.RemoveAt(index);
                    this.parent.write_Directory();
                    
                }
                FAT.write_fat_table_in_file();
            }
        }
    }
}

