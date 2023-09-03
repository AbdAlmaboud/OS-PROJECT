
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AKVLNADKLNV
{
    public class Directory_entry
    {
        public string name = "";
        public byte attr;
        public byte[] empety = new byte[12];
        public int size;
        public int F_cluster;
        public Directory_entry()
        {

        }
        public Directory_entry(string na, byte atr, int fc,int c)
        {
            size = c;
            set_name(na);
            set_attr(atr);
            set_F_claster(fc);

        }


        public void set_name(string data)
        {
            if (data.Length > 11)
            {
                if (attr == 1)
                {
                    for (int i = 0; i < 11; i++)
                    {
                        name += data[i];
                    }
                }
                else
                {
                    string x = "";
                    for (int i = data.Length - 1; i >= 0; i--)
                    {
                        x += data[i].ToString();
                        if (data[i] == '.') break;
                    }
                    x.Reverse();
                    for (int i = 0; i < 11 - x.Length; i++)
                    {
                        name += data[i].ToString();
                    }
                    name += x;
                }
            }
            else
            {
                name = data;
            }
        }

        public void set_size(int x)
        {
            size = x;
        }
        public void set_F_claster(int x)
        {
            F_cluster = x;
        }
        public void set_attr(byte x)
        {
            attr = x;
        }
        public void set_empte(byte[] x)
        {
            for (int i = 0; i < x.Length; i++)
            {
                empety[i] = x[i];
            }
        }

        public string get_name()
        { return name; }
        public int get_F_claster()
        { return F_cluster; }

        public byte[] getBytes()
        {
            byte[] date = new byte[32];
            date[11] = attr;

            byte[] temp = Encoding.ASCII.GetBytes(name);
            for (int i = 0; i < temp.Length; i++)
            {
                date[i] = temp[i];
            }
            for (int i = 0; i < 12; i++)
            {
                date[i + 12] = empety[i];
            }
            temp = BitConverter.GetBytes(F_cluster);
            for (int i = 0; i < temp.Length; i++)
            {
                date[i + 24] = (byte)temp[i];
            }
            temp = BitConverter.GetBytes(size);
            for (int i = 0; i < temp.Length; i++)
            {
                date[i + 28] = (byte)temp[i];
            }
            return date;
        }
        public Directory_entry GetDirectory(byte[] data)
        {
            Directory_entry dir = new Directory_entry();
            dir.set_attr((byte)data[11]);

            byte[] temp = new byte[11];
            for (int i = 0; i < 11; i++)
            {
                temp[i] = data[i];
            }
            dir.set_name(Encoding.ASCII.GetString(temp).ToString());

            temp = new byte[12];
            for (int i = 0; i < 12; i++)
            {
                temp[i] = data[i + 12];
            }
            dir.set_empte(temp);

            temp = new byte[4];
            for (int i = 24; i < 28; i++)
            {
                temp[i - 24] = data[i];
            }
            dir.set_F_claster(BitConverter.ToInt32(temp));

            for (int i = 28; i < 32; i++)
            {
                temp[i - 28] = data[i];
            }
            dir.set_size(BitConverter.ToInt32(temp));

            return dir;
        }
        public Directory_entry get_directory_entry()
        {
            Directory_entry d = new Directory_entry(new string(this.name), this.attr, this.F_cluster,this.size);
            return d;
        }
       
    

    }
}
