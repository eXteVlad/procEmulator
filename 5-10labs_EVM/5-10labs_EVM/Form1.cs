using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace _5_10labs_EVM
{
    public partial class Form1 : Form
    {
        public String[] reg = new String[9];
        public String[] mem = new String[18];

        public int CF = 0;
        public int ZF = 0;
        public int SF = 0;
        public int OF = 0;
        public int DF = 0;
        public int TF = 0;
        public int IF = 0;

        public int IP = 0;

        public int step = 0;

        public bool sign_flag = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string line;
            int count = 0;
            System.IO.StreamReader input_regs = new System.IO.StreamReader(@"input_regs.txt");
            System.IO.StreamReader input_mems = new System.IO.StreamReader(@"input_mems.txt");

            while ((line = input_regs.ReadLine()) != null)
            {
                reg[count] = line;
                count++;
            }

            count = 0;

            while ((line = input_mems.ReadLine()) != null)
            {
                mem[count] = line;
                count++;
            }

            updateInfo();
        }

        private void updateInfo()
        {
            textBox3.Clear();
            textBox4.Clear();

            for (int i = 0; i < 8; i++) textBox3.Text += reg[i] + Environment.NewLine;
            for (int i = 0; i < 16; i++) textBox4.Text += mem[i] + Environment.NewLine;

            label1.Text = IF.ToString();
            label2.Text = TF.ToString();
            label7.Text = CF.ToString();
            label8.Text = ZF.ToString();
            label9.Text = SF.ToString();
            label10.Text = OF.ToString();
            label6.Text = DF.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private string toBin(int op)
        {
            int count = 0;
            string decOp = "";
            int oop = op;

            if (oop < 0) oop = Math.Abs(oop);

            while (count != 8)
            {
                if (oop % 2 == 1) decOp = "1" + decOp;
                else if (oop % 2 == 0) decOp = "0" + decOp;
                oop /= 2;
                count++;
            }
            
            if (op < 0)
            {
                char[] tempRes = new char[8];
                if (decOp.Length > 8) tempRes = new char[16];
                for (int i = 0; i < tempRes.Length - decOp.Length; i++) tempRes[i] = '0';
                for (int i = tempRes.Length - decOp.Length; i < tempRes.Length; i++) tempRes[i] = decOp[i - tempRes.Length + decOp.Length];
                int temp = 0;
                for (int i = 0; i < 8; i++)
                {
                    int k = int.Parse(tempRes[7 - i].ToString()) + temp + 1;
                    tempRes[7 - i] = Convert.ToChar(k);
                    if (k == 2) { tempRes[7 - i] = '0'; temp = 1; }
                    if (k == 3) { tempRes[7 - i] = '1'; temp = 1; }
                    if (tempRes[7 - i] == '0') tempRes[7 - i] = '1';
                    else tempRes[7 - i] = '0';
                }
                decOp = "";
                for (int i = 0; i < tempRes.Length; i++) decOp += tempRes[i];
            }

            return decOp;
        }

        private string toDec(string op)
        {
            double binOp = 0;
            int st = op.Length - 1;

            for (int i = 0; i < op.Length; i++)
            {
                if (op[i] == '1') binOp += 1 * Math.Pow(2, st);
                st --;
            }

            if(binOp > 127 && sign_flag == true)
            {
                double dec = 0;
                char[] ch = op.ToCharArray();
                int temp = 0;
                for (int i = 0; i < op.Length; i++)
                {
                    if (ch[i] == '0') ch[i] = '1';
                    else ch[i] = '0';
                }
                if (ch[op.Length - 1] == '1')
                {
                    temp = 1;
                    ch[op.Length - 1] = '0';
                }
                else
                {
                    ch[op.Length - 1] = '1';
                }
                for (int i = op.Length - 2; i > 0; i--)
                {
                    if (ch[i] == '0' && temp == 1)
                    {
                        ch[i] = '1';
                        temp = 0;
                    }
                    if (ch[i] == '1' && temp == 1) ch[i] = '0';
                }
                for (int i = 0; i < op.Length; i++)
                {
                    if (ch[op.Length - i - 1] == '1')
                    {
                        dec += Math.Pow(2, i);
                    }
                }
                dec = -dec;
                return dec.ToString();
            }

            return binOp.ToString();
        }

        private string OpAdd(string op1, string op2)
        {
            int c1 = 0, c2 = 0;

            StringBuilder res = new StringBuilder(op1);
            sign_flag = true;
            if (int.Parse(toDec(op1)) > 127 || int.Parse(toDec(op1)) < -127) OF = 1;
            else OF = 0;

            if (int.Parse(toDec(op2)) > 127 || int.Parse(toDec(op2)) < -127) OF = 1;
            else OF = 0;

            for (int i = op1.Length - 1; i >= 0; i--)
            {
                if (op1[i] == '0') c1 = 0;
                else c1 = 1;
                if (op2[i] == '0') c2 = 0;
                else c2 = 1;

                if (c1 + c2 + CF == 0) res[i] = '0';
                else if (c1 + c2 + CF == 1)
                {
                    CF = 0;
                    res[i] = '1';
                }
                else if (c1 + c2 + CF == 2)
                {
                    CF = 1;
                    res[i] = '0';
                }
                else if (c1 + c2 + CF == 3)
                {
                    CF = 1;
                    res[i] = '1';
                }
            }
            op1 = res.ToString();
            if (op1 == "00000000") ZF = 1;
            else ZF = 0;
            if (int.Parse(toDec(op1)) < 0) SF = 1;
            else SF = 0;
            sign_flag = false;
            updateInfo();
            return op1;
        }

        private string OpSub(string op1, string op2)
        {
            StringBuilder res = new StringBuilder(op1);
            string result = "";
            int c1 = 0, c2 = 0;
            bool otr = false;
            sign_flag = true;

            if (int.Parse(toDec(op1)) < int.Parse(toDec(op2)))
            {
                res = new StringBuilder(op2);
                op2 = op1;
                otr = true;
            }

            for (int i = res.Length - 1; i >= 0; i--)
            {
                if (res[i] == '0') c1 = 0;
                else c1 = 1;
                if (op2[i] == '0') c2 = 0;
                else c2 = 1;

                if (c1 == 0 && c2 == 0) res[i] = '0';
                else if (c1 == 1 && c2 == 0) res[i] = '1';
                else if (c1 == 1 && c2 == 1) res[i] = '0';
                else if (c1 == 0 && c2 == 1)
                {
                    CF = 1;
                    res[i] = '1';
                    for (int j = i - 1; j >= 0; j--)
                    {
                        if (res[j] == '1' && CF == 1)
                        {
                            res[j] = '0';
                            for (int k = j + 1; k < i; k++) res[k] = '1';
                            break;
                        }
                    }
                }
            }
            if (otr == true)
            {
                int decTemp = 0;

                decTemp = int.Parse(toDec(res.ToString()));
                decTemp = decTemp * (-1);
                result = toBin(decTemp);
            }
            else result = res.ToString();
            if (result == "00000000") ZF = 1;
            else ZF = 0;
            sign_flag = false;
            updateInfo();
            return result;
        }

        private string OpMul(string op1, string op2)
        {
            sign_flag = true;
            string oop1 = toBin(int.Parse(op1));
            int ress = 0;

            if (Int32.TryParse(op2, out ress) == false)
            {
                switch (op2)
                {
                    case "ah": op2 = reg[0]; break;
                    case "al": op2 = reg[1]; break;
                    case "bh": op2 = reg[2]; break;
                    case "bl": op2 = reg[3]; break;
                    case "dh": op2 = reg[4]; break;
                    case "dl": op2 = reg[5]; break;
                    case "ch": op2 = reg[6]; break;
                    case "cl": op2 = reg[7]; break;
                }
            }
            else op2 = toBin(int.Parse(op2));

            if (int.Parse(op1) < 0 && int.Parse(op2) < 0)
            {
                oop1 = toBin(Math.Abs(int.Parse(op1))).ToString();
                SF = 0;
            }

            if (int.Parse(op1) == 0 || int.Parse(op2) == 0)
            {
                ZF = 1;
                return "00000000";
            }
            else ZF = 0;

            string res = oop1;

            for (int i = 0; i < Math.Abs(int.Parse(op2)) - 1; i++)
            {
                res = OpAdd(oop1, res);
            }

            if ((int.Parse(op1) < 0 && int.Parse(op2) > 0) || (int.Parse(op1) > 0 && int.Parse(op2) < 0))
            {
                int temp = 0;
                SF = 1;
                res = toDec(res);
                temp = int.Parse(res);
                temp = temp * (-1);
                res = toBin(temp);
            }
            else SF = 0;
            sign_flag = false;
            updateInfo();
            return res.ToString();
        }

        private string OpDiv(string op1, string op2)
        {
            int ress = 0;
            sign_flag = true;
            if (Int32.TryParse(op1, out ress) == false)
            {
                switch (op1)
                {
                    case "ah": op1 = reg[0]; break;
                    case "al": op1 = reg[1]; break;
                    case "bh": op1 = reg[2]; break;
                    case "bl": op1 = reg[3]; break;
                    case "dh": op1 = reg[4]; break;
                    case "dl": op1 = reg[5]; break;
                    case "ch": op1 = reg[6]; break;
                    case "cl": op1 = reg[7]; break;
                }
            }
            else op1 = toBin(int.Parse(op1));

            if (Int32.TryParse(op2, out ress) == false)
            {
                switch (op2)
                {
                    case "ah": op2 = reg[0]; break;
                    case "al": op2 = reg[1]; break;
                    case "bh": op2 = reg[2]; break;
                    case "bl": op2 = reg[3]; break;
                    case "dh": op2 = reg[4]; break;
                    case "dl": op2 = reg[5]; break;
                    case "ch": op2 = reg[6]; break;
                    case "cl": op2 = reg[7]; break;
                }
            }
            else op2 = toBin(int.Parse(op2));

            int oop1 = int.Parse(toDec(op1));
            int oop2 = int.Parse(toDec(op2));

            if (oop2 == 0)
            {
                oop2 = 1;
                textBox5.Text = "На ноль делить нельзя! Обход ошибки." + Environment.NewLine;
            }

            int count = 0;
            if (oop1 < 0 && oop2 < 0)
            {
                oop1 = Math.Abs(oop1);
                oop2 = Math.Abs(oop2);
                SF = 0;
            }
            else if (oop1 < 0 || oop2 < 0)
            {
                oop1 = Math.Abs(oop1);
                oop2 = Math.Abs(oop2);
                SF = 1;
            }
            else SF = 0;

            while (oop1 > oop2 || oop1 != 0)
            {
                if (oop1 < oop2) break;
                oop1 = oop1 - oop2;
                count++;
            }

            if (count == 0 && oop1 == 0) ZF = 1;
            else ZF = 0;

            reg[0] = toBin(count).ToString();
            sign_flag = false;
            updateInfo();
            return toBin(oop1).ToString();
        }

        private string OpShr(string op1, string op2)
        {
            StringBuilder res = new StringBuilder(op1);
            int n = int.Parse(op2);

            if (res[res.Length - 1] == '1') CF = 1;
            else CF = 0;

            for (int i = 0; i < n; i++)
            {
                for (int j = op1.Length - 1; j > 0; j--)
                {
                    res[j] = res[j - 1];
                }
                res[0] = '0';
            }

            if (res.ToString() == "00000000") ZF = 1;
            else ZF = 0;

            updateInfo();
            return res.ToString();
        }

        private string OpShl(string op1, string op2)
        {
            StringBuilder res = new StringBuilder(op1);
            int n = int.Parse(op2);

            if (op1 == "00000000") ZF = 1;
            else ZF = 0;

            if (res[0] == '1') CF = 1;
            else CF = 0;

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < op1.Length - 1; j++)
                {
                    res[j] = res[j + 1];
                }
                res[op1.Length - 1] = '0';
            }

            if (res.ToString() == "00000000") ZF = 1;
            else ZF = 0;

            updateInfo();
            return res.ToString();
        }

        private string OpRor(string op1, string op2)
        {
            StringBuilder res = new StringBuilder(op1);
            int n = int.Parse(op2);
            char temp = '0';

            if (op1 == "00000000") ZF = 1;
            else ZF = 0;

            for (int i = 0; i < n; i++)
            {
                temp = res[op1.Length - 1];
                for (int j = op1.Length - 1; j > 0; j--)
                {
                    res[j] = res[j - 1];
                }
                res[0] = temp;
            }
            updateInfo();
            return res.ToString();
        }

        private string OpRol(string op1, string op2)
        {
            StringBuilder res = new StringBuilder(op1);
            int n = int.Parse(op2);
            char temp = '0';

            if (op1 == "00000000") ZF = 1;
            else ZF = 0;

            for (int i = 0; i < n; i++)
            {
                temp = res[0];
                for (int j = 0; j < op1.Length - 1; j++)
                {
                    res[j] = res[j + 1];
                }
                res[op1.Length - 1] = temp;
            }
            updateInfo();
            return res.ToString();
        }

        private string OpRrc(string op1, string op2)
        {
            StringBuilder res = new StringBuilder(op1);
            int n = int.Parse(op2);

            if (op1 == "00000000") ZF = 1;
            else ZF = 0;

            for (int i = 0; i < n; i++)
            {
                if (res[res.Length - 1] == '1') CF = 1;
                else CF = 0;
                
                for (int j = op1.Length - 1; j > 0; j--)
                {
                    res[j] = res[j - 1];
                }

                if (CF == 1) res[0] = '1';
                else res[0] = '0';
            }
            
            updateInfo();
            return res.ToString();
        }

        private string OpRlc(string op1, string op2)
        {
            StringBuilder res = new StringBuilder(op1);
            int n = int.Parse(op2);

            if (op1 == "00000000") ZF = 1;
            else ZF = 0;

            for (int i = 0; i < n; i++)
            {
                if (res[0] == '1') CF = 1;
                else CF = 0;

                for (int j = 0; j < op1.Length - 1; j++)
                {
                    res[j] = res[j + 1];
                }

                if (CF == 1) res[op1.Length - 1] = '1';
                else res[op1.Length - 1] = '0';
            }

            updateInfo();
            return res.ToString();
        }

        private string OpAnd(string op1, string op2)
        {
            string res = "";
            int ress = 0;

            if (Int32.TryParse(op2, out ress) == false)
            {
                switch (op2)
                {
                    case "ah": op2 = reg[0]; break;
                    case "al": op2 = reg[1]; break;
                    case "bh": op2 = reg[2]; break;
                    case "bl": op2 = reg[3]; break;
                    case "dh": op2 = reg[4]; break;
                    case "dl": op2 = reg[5]; break;
                    case "ch": op2 = reg[6]; break;
                    case "cl": op2 = reg[7]; break;
                }
            }
            else op2 = toBin(int.Parse(op2));

            for (int i = 0; i < op1.Length; i++)
            {
                if (op1[i] == '1' && op2[i] == '1') res += '1';
                else res += '0';
            }

            if (res == "00000000") ZF = 1;
            else ZF = 0;

            if (int.Parse(toDec(res)) < 0) SF = 1;
            else SF = 0;

            return res;
        }

        private string OpOr(string op1, string op2)
        {
            string res = "";
            int ress = 0;

            if (Int32.TryParse(op2, out ress) == false)
            {
                switch (op2)
                {
                    case "ah": op2 = reg[0]; break;
                    case "al": op2 = reg[1]; break;
                    case "bh": op2 = reg[2]; break;
                    case "bl": op2 = reg[3]; break;
                    case "dh": op2 = reg[4]; break;
                    case "dl": op2 = reg[5]; break;
                    case "ch": op2 = reg[6]; break;
                    case "cl": op2 = reg[7]; break;
                }
            }

            for (int i = 0; i < op1.Length; i++)
            {
                if (op1[i] == '0' && op2[i] == '0') res += '0';
                else res += '1';
            }

            if (res == "00000000") ZF = 1;
            else ZF = 0;

            return res;
        }

        private string OpXor(string op1, string op2)
        {
            string res = "";
            int ress = 0;

            if (Int32.TryParse(op2, out ress) == false)
            {
                switch (op2)
                {
                    case "ah": op2 = reg[0]; break;
                    case "al": op2 = reg[1]; break;
                    case "bh": op2 = reg[2]; break;
                    case "bl": op2 = reg[3]; break;
                    case "dh": op2 = reg[4]; break;
                    case "dl": op2 = reg[5]; break;
                    case "ch": op2 = reg[6]; break;
                    case "cl": op2 = reg[7]; break;
                }
            }

            for (int i = 0; i < op1.Length; i++)
            {
                if (op1[i] != op2[i]) res += '1';
                else res += '0';
            }

            if (res == "00000000") ZF = 1;
            else ZF = 0;

            return res;
        }

        private string OpNot(string op1)
        {
            StringBuilder res = new StringBuilder(op1);
            string ress = "";

            for (int i = 0; i < op1.Length; i++)
            {
                if (op1[i] == '1') res[i] = '0';
                else res[i] = '1';
            }

            ress = res.ToString();

            if (ress == "00000000") ZF = 1;
            else ZF = 0;

            return ress;
        }

        private void OpMov(string op1, string op2)
        {
            int res = 0;
            string oop2 = "";

            if (Int32.TryParse(op2, out res) == true)
            {
                oop2 = toBin(int.Parse(op2));
            }
            else
            {
                switch (op2)
                {
                    case "ah": oop2 = reg[0]; break;
                    case "al": oop2 = reg[1]; break;
                    case "bh": oop2 = reg[2]; break;
                    case "bl": oop2 = reg[3]; break;
                    case "dh": oop2 = reg[4]; break;
                    case "dl": oop2 = reg[5]; break;
                    case "ch": oop2 = reg[6]; break;
                    case "cl": oop2 = reg[7]; break;

                    case "#1": oop2 = mem[0]; break;
                    case "#2": oop2 = mem[1]; break;
                    case "#3": oop2 = mem[2]; break;
                    case "#4": oop2 = mem[3]; break;
                    case "#5": oop2 = mem[4]; break;
                    case "#6": oop2 = mem[5]; break;
                    case "#7": oop2 = mem[6]; break;
                    case "#8": oop2 = mem[7]; break;
                    case "#9": oop2 = mem[8]; break;
                    case "#10": oop2 = mem[9]; break;
                    case "#11": oop2 = mem[10]; break;
                    case "#12": oop2 = mem[11]; break;
                    case "#13": oop2 = mem[12]; break;
                    case "#14": oop2 = mem[13]; break;
                    case "#15": oop2 = mem[14]; break;
                    case "#16": oop2 = mem[15]; break;
                }
            }

            switch (op1)
            {
                case "ah": reg[0] = oop2; break;
                case "al": reg[1] = oop2; break;
                case "bh": reg[2] = oop2; break;
                case "bl": reg[3] = oop2; break;
                case "dh": reg[4] = oop2; break;
                case "dl": reg[5] = oop2; break;
                case "ch": reg[6] = oop2; break;
                case "cl": reg[7] = oop2; break;

                case "#1": mem[0] = oop2; break;
                case "#2": mem[1] = oop2; break;
                case "#3": mem[2] = oop2; break;
                case "#4": mem[3] = oop2; break;
                case "#5": mem[4] = oop2; break;
                case "#6": mem[5] = oop2; break;
                case "#7": mem[6] = oop2; break;
                case "#8": mem[7] = oop2; break;
                case "#9": mem[8] = oop2; break;
                case "#10": mem[9] = oop2; break;
                case "#11": mem[10] = oop2; break;
                case "#12": mem[11] = oop2; break;
                case "#13": mem[12] = oop2; break;
                case "#14": mem[13] = oop2; break;
                case "#15": mem[14] = oop2; break;
                case "#16": mem[15] = oop2; break;
            }
           
            updateInfo();
        }

        private string OpCmp(string op1, string op2)
        {
            string res = "";

            if (op2 == op1) ZF = 1;
            else ZF = 0;

            if (int.Parse(toDec(op1)) < 0) SF = 1;
            else SF = 0;

            if (int.Parse(toDec(op1)) < int.Parse(toDec(op2))) CF = 1;
            else CF = 0;

            res = OpSub(op1, op2);

            updateInfo();
            return res;
        }

        private void OpMovs(string op1, string op2)
        {
            string oop1, oop2;
            int pos1 = 0, pos2 = 0;
            char flag = ' ';
            int n = int.Parse(toDec(reg[6].ToString()));
  
            switch (op1)
            {
                case "#1": oop1 = mem[0]; pos1 = 0; break;
                case "#2": oop1 = mem[1]; pos1 = 1; break;
                case "#3": oop1 = mem[2]; pos1 = 2; break;
                case "#4": oop1 = mem[3]; pos1 = 3; break;
                case "#5": oop1 = mem[4]; pos1 = 4; break;
                case "#6": oop1 = mem[5]; pos1 = 5; break;
                case "#7": oop1 = mem[6]; pos1 = 6; break;
                case "#8": oop1 = mem[7]; pos1 = 7; break;
                case "#9": oop1 = mem[8]; pos1 = 8; break;
                case "#10": oop1 = mem[9]; pos1 = 9; break;
                case "#11": oop1 = mem[10]; pos1 = 10; break;
                case "#12": oop1 = mem[11]; pos1 = 11; break;
                case "#13": oop1 = mem[12]; pos1 = 12; break;
                case "#14": oop1 = mem[13]; pos1 = 13; break;
                case "#15": oop1 = mem[14]; pos1 = 14; break;
                case "#16": oop1 = mem[15]; pos1 = 15; break;
            }
            switch (op2)
            {
                case "ah": oop2 = reg[0]; pos2 = 0; flag = 'R'; break;
                case "al": oop2 = reg[1]; pos2 = 1; flag = 'R'; break;
                case "bh": oop2 = reg[2]; pos2 = 2; flag = 'R'; break;
                case "bl": oop2 = reg[3]; pos2 = 3; flag = 'R'; break;
                case "dh": oop2 = reg[4]; pos2 = 4; flag = 'R'; break;
                case "dl": oop2 = reg[5]; pos2 = 5; flag = 'R'; break;
                case "ch": oop2 = reg[6]; pos2 = 6; flag = 'R'; break;
                case "cl": oop2 = reg[7]; pos2 = 7; flag = 'R'; break;

                case "#1": oop2 = mem[0]; pos2 = 0; flag = 'M'; break;
                case "#2": oop2 = mem[1]; pos2 = 1; flag = 'M';; break;
                case "#3": oop2 = mem[2]; pos2 = 2; flag = 'M'; break;
                case "#4": oop2 = mem[3]; pos2 = 3; flag = 'M'; break;
                case "#5": oop2 = mem[4]; pos2 = 4; flag = 'M'; break;
                case "#6": oop2 = mem[5]; pos2 = 5; flag = 'M'; break;
                case "#7": oop2 = mem[6]; pos2 = 6; flag = 'M'; break;
                case "#8": oop2 = mem[7]; pos2 = 7; flag = 'M'; break;
                case "#9": oop2 = mem[8]; pos2 = 8; flag = 'M'; break;
                case "#10": oop2 = mem[9]; pos2 = 9; flag = 'M'; break;
                case "#11": oop2 = mem[10]; pos2 = 10; flag = 'M'; break;
                case "#12": oop2 = mem[11]; pos2 = 11; flag = 'M'; break;
                case "#13": oop2 = mem[12]; pos2 = 12; flag = 'M'; break;
                case "#14": oop2 = mem[13]; pos2 = 13; flag = 'M'; break;
                case "#15": oop2 = mem[14]; pos2 = 14; flag = 'M'; break;
                case "#16": oop2 = mem[15]; pos2 = 15; flag = 'M'; break;

                default: oop2 = toBin(int.Parse(op2)); flag = 'N'; break;
            }

            while (reg[6] != "00000000")
            {
                if (flag == 'R')
                {
                    mem[pos1] = oop2;
                    reg[6] = OpSub(reg[6], "00000001");
                    if (DF == 1) pos1++;
                    else pos1--;
                }
                else if (flag == 'M')
                {
                    mem[pos1] = mem[pos2];
                    reg[6] = OpSub(reg[6], "00000001");
                    if (DF == 1) pos1++;
                    else pos1--;
                }
                else if (flag == 'N')
                {
                    mem[pos1] = oop2;
                    reg[6] = OpSub(reg[6], "00000001");
                    if (DF == 1) pos1++;
                    else pos1--;
                }
            }
            reg[6] = "00000000";
            updateInfo();
        }

        private void OpTest(string op1, string op2)
        {
            string res = "";

            for (int i = 0; i < op1.Length; i++)
            {
                if (op1[i] == '1' && op2[i] == '1') res += '1';
                else res += '0';
            }

            if (res == "00000000") ZF = 1;
            else ZF = 0;

            if (int.Parse(toDec(res)) < 0) SF = 1;
            else SF = 0;

            if (res == op1) textBox5.Text += "Результат выполнения команды SWAP:" + Environment.NewLine +
                                             "Операнд: " + op1 +  "(" + toDec(op1) + ")" + Environment.NewLine +
                                             "Сравниваемое число: " + toDec(op2) + Environment.NewLine +
                                             "Операнд и сравниваемое число равны.";
            else textBox5.Text += "Результат выполнения команды SWAP:" + Environment.NewLine +
                                             "Операнд: " + op1 + "(" + toDec(op1) + ")" + Environment.NewLine +
                                             "Сравниваемое число: " + toDec(op2) + Environment.NewLine +
                                             "Операнд и сравниваемое число не равны.";
        }

        private string OpNeg(string op1)
        {
            string res = "";

            res = toBin(int.Parse(toDec(op1)) * (-1)).ToString();

            if (int.Parse(toDec(res)) < 0) SF = 1;
            else SF = 0;

            if (res == "00000000") ZF = 1;
            else ZF = 0;

            //res = OpSub("00000000", op1);
            //res = OpAdd(res, "00000001");

            return res;
        }

        private string OpSwap(string op1)
        {
            StringBuilder res = new StringBuilder(op1);
            StringBuilder part1 = new StringBuilder("0000");
            StringBuilder part2 = new StringBuilder("0000");

            string result = "";

            for (int i = 0; i < 4; i++) part1[i] = res[i];
            for (int i = 4; i < 8; i++) part2[i-4] = res[i];

            result = part2.ToString() + part1.ToString();

            if (result == "00000000") ZF = 1;
            else ZF = 0;

            if (int.Parse(toDec(result)) < 0) SF = 1;
            else SF = 0;

            if (int.Parse(toDec(result)) > 127 || int.Parse(toDec(result)) < -127) OF = 1;
            else OF = 0;

            return result;
        }

        private void OpLoop(string op1)
        {
            String[] str = textBox6.Text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            int c = 0;

            if (reg[6] == "00000000") ZF = 1;
            else ZF = 0;

            for (int i = 0; i < str.Length; i++)
            {
                String[] words = str[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                String[] words2 = { "" };
                c = i;
                if (words[0] == op1 + ":")
                {
                    while (int.Parse(toDec(reg[6])) > 0)
                    {
                        if (str[c] == "loop " + op1) c = i;
                        Comands(c);
                        reg[6] = OpSub(reg[6], "00000001");
                        c++;
                    }
                }
            }
        }

        private void OpJmp(string op1)
        {
            String[] str = textBox6.Text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < str.Length; i++)
            {
                String[] words = str[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                String[] words2 = { "" };

                if (words[0] == op1 + ":") IP = i;
            }
        }

        private void OpJe(string op1)
        {
            if (ZF == 1) OpJmp(op1);
        }

        private void OpJne(string op1)
        {
            if (ZF == 0) OpJmp(op1);
        }

        private void OpJg(string op1)
        {
            if (CF == 0) OpJmp(op1);
        }

        private void OpJl(string op1)
        {
            if (CF == 1) OpJmp(op1);
        }

        private void Comands(int count)
        {
            String[] str = textBox6.Text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            String[] words = str[count].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            String[] words2;

            textBox5.Text += "Выполненная команда: " + Environment.NewLine +
                            str[count] + Environment.NewLine;

            string op0 = words[0];
            string op1, op2, op3;

            int res = 0;

            switch (op0)
            {
                case "mov":
                    words2 = words[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    op2 = words2[0];
                    op3 = words2[1];
                    OpMov(op2, op3);
                    break;
                case "inc":
                    op1 = words[1];
                    switch (op1)
                    {
                        case "ah": reg[0] = OpAdd(reg[0], "00000001"); break;
                        case "al": reg[1] = OpAdd(reg[1], "00000001"); break;
                        case "bh": reg[2] = OpAdd(reg[2], "00000001"); break;
                        case "bl": reg[3] = OpAdd(reg[3], "00000001"); break;
                        case "dh": reg[4] = OpAdd(reg[4], "00000001"); break;
                        case "dl": reg[5] = OpAdd(reg[5], "00000001"); break;
                        case "ch": reg[6] = OpAdd(reg[6], "00000001"); break;
                        case "cl": reg[7] = OpAdd(reg[7], "00000001"); break;
                    }
                    break;
                case "dec":
                    op1 = words[1];
                    switch (op1)
                    {
                        case "ah": reg[0] = OpSub(reg[0], "00000001"); break;
                        case "al": reg[1] = OpSub(reg[1], "00000001"); break;
                        case "bh": reg[2] = OpSub(reg[2], "00000001"); break;
                        case "bl": reg[3] = OpSub(reg[3], "00000001"); break;
                        case "dh": reg[4] = OpSub(reg[4], "00000001"); break;
                        case "dl": reg[5] = OpSub(reg[5], "00000001"); break;
                        case "ch": reg[6] = OpSub(reg[6], "00000001"); break;
                        case "cl": reg[7] = OpSub(reg[7], "00000001"); break;
                    }
                    break;
                case "add":
                    words2 = words[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    op2 = words2[0];
                    op3 = words2[1];

                    if (Int32.TryParse(op2, out res) == false)
                    {
                        switch (op2)
                        {
                            case "ah": reg[0] = OpAdd(reg[0], toBin(int.Parse(op3))); break;
                            case "al": reg[1] = OpAdd(reg[1], toBin(int.Parse(op3))); break;
                            case "bh": reg[2] = OpAdd(reg[2], toBin(int.Parse(op3))); break;
                            case "bl": reg[3] = OpAdd(reg[3], toBin(int.Parse(op3))); break;
                            case "dh": reg[4] = OpAdd(reg[4], toBin(int.Parse(op3))); break;
                            case "dl": reg[5] = OpAdd(reg[5], toBin(int.Parse(op3))); break;
                            case "ch": reg[6] = OpAdd(reg[6], toBin(int.Parse(op3))); break;
                            case "cl": reg[7] = OpAdd(reg[7], toBin(int.Parse(op3))); break;
                        }
                    }
                    break;
                case "adc":
                    words2 = words[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    op2 = words2[0];
                    op3 = words2[1];
                    if (Int32.TryParse(op2, out res) == false)
                    {
                        switch (op2)
                        {
                            case "ah": reg[0] = OpAdd(reg[0], toBin(CF)); reg[0] = OpAdd(reg[0], toBin(int.Parse(op3))); break;
                            case "al": reg[1] = OpAdd(reg[1], toBin(CF)); reg[1] = OpAdd(reg[1], toBin(int.Parse(op3))); break;
                            case "bh": reg[2] = OpAdd(reg[2], toBin(CF)); reg[2] = OpAdd(reg[2], toBin(int.Parse(op3))); break;
                            case "bl": reg[3] = OpAdd(reg[3], toBin(CF)); reg[3] = OpAdd(reg[3], toBin(int.Parse(op3))); break;
                            case "dh": reg[4] = OpAdd(reg[4], toBin(CF)); reg[4] = OpAdd(reg[4], toBin(int.Parse(op3))); break;
                            case "dl": reg[5] = OpAdd(reg[5], toBin(CF)); reg[5] = OpAdd(reg[5], toBin(int.Parse(op3))); break;
                            case "ch": reg[6] = OpAdd(reg[6], toBin(CF)); reg[6] = OpAdd(reg[6], toBin(int.Parse(op3))); break;
                            case "cl": reg[7] = OpAdd(reg[7], toBin(CF)); reg[7] = OpAdd(reg[7], toBin(int.Parse(op3))); break;
                        }
                    }
                    break;
                case "sub":
                    words2 = words[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    op2 = words2[0];
                    op3 = words2[1];
                    if (Int32.TryParse(op2, out res) == false)
                    {
                        switch (op2)
                        {
                            case "ah": reg[0] = OpSub(reg[0], toBin(int.Parse(op3))); break;
                            case "al": reg[1] = OpSub(reg[1], toBin(int.Parse(op3))); break;
                            case "bh": reg[2] = OpSub(reg[2], toBin(int.Parse(op3))); break;
                            case "bl": reg[3] = OpSub(reg[3], toBin(int.Parse(op3))); break;
                            case "dh": reg[4] = OpSub(reg[4], toBin(int.Parse(op3))); break;
                            case "dl": reg[5] = OpSub(reg[5], toBin(int.Parse(op3))); break;
                            case "ch": reg[6] = OpSub(reg[6], toBin(int.Parse(op3))); break;
                            case "cl": reg[7] = OpSub(reg[7], toBin(int.Parse(op3))); break;
                        }
                    }
                    break;
                case "subb":
                    words2 = words[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    op2 = words2[0];
                    op3 = words2[1];
                    if (Int32.TryParse(op2, out res) == false)
                    {
                        switch (op2)
                        {
                            case "ah": reg[0] = OpSub(reg[0], toBin(CF)); reg[0] = OpSub(reg[0], toBin(int.Parse(op3))); break;
                            case "al": reg[1] = OpSub(reg[1], toBin(CF)); reg[1] = OpSub(reg[1], toBin(int.Parse(op3))); break;
                            case "bh": reg[2] = OpSub(reg[2], toBin(CF)); reg[2] = OpSub(reg[2], toBin(int.Parse(op3))); break;
                            case "bl": reg[3] = OpSub(reg[3], toBin(CF)); reg[3] = OpSub(reg[3], toBin(int.Parse(op3))); break;
                            case "dh": reg[4] = OpSub(reg[4], toBin(CF)); reg[4] = OpSub(reg[4], toBin(int.Parse(op3))); break;
                            case "dl": reg[5] = OpSub(reg[5], toBin(CF)); reg[5] = OpSub(reg[5], toBin(int.Parse(op3))); break;
                            case "ch": reg[6] = OpSub(reg[6], toBin(CF)); reg[6] = OpSub(reg[6], toBin(int.Parse(op3))); break;
                            case "cl": reg[7] = OpSub(reg[7], toBin(CF)); reg[7] = OpSub(reg[7], toBin(int.Parse(op3))); break;
                        }
                    }
                    break;
                case "std": DF = 1; break;
                case "sto": OF = 1; break;
                case "sts": SF = 1; break;
                case "stz": ZF = 1; break;
                case "stc": CF = 1; break;
                case "sti": IF = 1; break;
                case "stt": TF = 1; break;
                case "cld": DF = 0; break;
                case "clo": OF = 0; break;
                case "cls": SF = 0; break;
                case "clz": ZF = 0; break;
                case "clc": CF = 0; break;
                case "cli": IF = 0; break;
                case "clt": TF = 0; break;
                case "mul":
                    words2 = words[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    op2 = words2[0];
                    op3 = words2[1];

                    switch (op2)
                    {
                        case "ah": reg[0] = OpMul(reg[0], op3); break;
                        case "al": reg[1] = OpMul(reg[1], op3); break;
                        case "bh": reg[2] = OpMul(reg[2], op3); break;
                        case "bl": reg[3] = OpMul(reg[3], op3); break;
                        case "dh": reg[4] = OpMul(reg[4], op3); break;
                        case "dl": reg[5] = OpMul(reg[5], op3); break;
                        case "ch": reg[6] = OpMul(reg[6], op3); break;
                        case "cl": reg[7] = OpMul(reg[7], op3); break;
                    }
                    break;
                case "div":
                    words2 = words[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    op2 = words2[0];
                    op3 = words2[1];
                    switch (op2)
                    {
                        case "ah": reg[0] = OpDiv(reg[0], op3); break;
                        case "al": reg[1] = OpDiv(reg[1], op3); break;
                        case "bh": reg[2] = OpDiv(reg[2], op3); break;
                        case "bl": reg[3] = OpDiv(reg[3], op3); break;
                        case "dh": reg[4] = OpDiv(reg[4], op3); break;
                        case "dl": reg[5] = OpDiv(reg[5], op3); break;
                        case "ch": reg[6] = OpDiv(reg[6], op3); break;
                        case "cl": reg[7] = OpDiv(reg[7], op3); break;
                    }
                    break;
                case "shr":
                    words2 = words[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    op2 = words2[0];
                    op3 = words2[1];
                    switch (op2)
                    {
                        case "ah": reg[0] = OpShr(reg[0], op3); break;
                        case "al": reg[1] = OpShr(reg[1], op3); break;
                        case "bh": reg[2] = OpShr(reg[2], op3); break;
                        case "bl": reg[3] = OpShr(reg[3], op3); break;
                        case "dh": reg[4] = OpShr(reg[4], op3); break;
                        case "dl": reg[5] = OpShr(reg[5], op3); break;
                        case "ch": reg[6] = OpShr(reg[6], op3); break;
                        case "cl": reg[7] = OpShr(reg[7], op3); break;
                    }
                    break;
                case "shl":
                    words2 = words[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    op2 = words2[0];
                    op3 = words2[1];
                    switch (op2)
                    {
                        case "ah": reg[0] = OpShl(reg[0], op3); break;
                        case "al": reg[1] = OpShl(reg[1], op3); break;
                        case "bh": reg[2] = OpShl(reg[2], op3); break;
                        case "bl": reg[3] = OpShl(reg[3], op3); break;
                        case "dh": reg[4] = OpShl(reg[4], op3); break;
                        case "dl": reg[5] = OpShl(reg[5], op3); break;
                        case "ch": reg[6] = OpShl(reg[6], op3); break;
                        case "cl": reg[7] = OpShl(reg[7], op3); break;
                    }
                    break;
                case "ror":
                    words2 = words[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    op2 = words2[0];
                    op3 = words2[1];
                    switch (op2)
                    {
                        case "ah": reg[0] = OpRor(reg[0], op3); break;
                        case "al": reg[1] = OpRor(reg[1], op3); break;
                        case "bh": reg[2] = OpRor(reg[2], op3); break;
                        case "bl": reg[3] = OpRor(reg[3], op3); break;
                        case "dh": reg[4] = OpRor(reg[4], op3); break;
                        case "dl": reg[5] = OpRor(reg[5], op3); break;
                        case "ch": reg[6] = OpRor(reg[6], op3); break;
                        case "cl": reg[7] = OpRor(reg[7], op3); break;
                    }
                    break;
                case "rol":
                    words2 = words[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    op2 = words2[0];
                    op3 = words2[1];
                    switch (op2)
                    {
                        case "ah": reg[0] = OpRol(reg[0], op3); break;
                        case "al": reg[1] = OpRol(reg[1], op3); break;
                        case "bh": reg[2] = OpRol(reg[2], op3); break;
                        case "bl": reg[3] = OpRol(reg[3], op3); break;
                        case "dh": reg[4] = OpRol(reg[4], op3); break;
                        case "dl": reg[5] = OpRol(reg[5], op3); break;
                        case "ch": reg[6] = OpRol(reg[6], op3); break;
                        case "cl": reg[7] = OpRol(reg[7], op3); break;
                    }
                    break;
                case "rrc":
                    words2 = words[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    op2 = words2[0];
                    op3 = words2[1];
                    switch (op2)
                    {
                        case "ah": reg[0] = OpRrc(reg[0], op3); break;
                        case "al": reg[1] = OpRrc(reg[1], op3); break;
                        case "bh": reg[2] = OpRrc(reg[2], op3); break;
                        case "bl": reg[3] = OpRrc(reg[3], op3); break;
                        case "dh": reg[4] = OpRrc(reg[4], op3); break;
                        case "dl": reg[5] = OpRrc(reg[5], op3); break;
                        case "ch": reg[6] = OpRrc(reg[6], op3); break;
                        case "cl": reg[7] = OpRrc(reg[7], op3); break;
                    }
                    break;
                case "rlc":
                    words2 = words[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    op2 = words2[0];
                    op3 = words2[1];
                    switch (op2)
                    {
                        case "ah": reg[0] = OpRlc(reg[0], op3); break;
                        case "al": reg[1] = OpRlc(reg[1], op3); break;
                        case "bh": reg[2] = OpRlc(reg[2], op3); break;
                        case "bl": reg[3] = OpRlc(reg[3], op3); break;
                        case "dh": reg[4] = OpRlc(reg[4], op3); break;
                        case "dl": reg[5] = OpRlc(reg[5], op3); break;
                        case "ch": reg[6] = OpRlc(reg[6], op3); break;
                        case "cl": reg[7] = OpRlc(reg[7], op3); break;
                    }
                    break;
                case "and":
                    words2 = words[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    op2 = words2[0];
                    op3 = words2[1];
                    switch (op2)
                    {
                        case "ah": reg[0] = OpAnd(reg[0], op3); break;
                        case "al": reg[1] = OpAnd(reg[1], op3); break;
                        case "bh": reg[2] = OpAnd(reg[2], op3); break;
                        case "bl": reg[3] = OpAnd(reg[3], op3); break;
                        case "dh": reg[4] = OpAnd(reg[4], op3); break;
                        case "dl": reg[5] = OpAnd(reg[5], op3); break;
                        case "ch": reg[6] = OpAnd(reg[6], op3); break;
                        case "cl": reg[7] = OpAnd(reg[7], op3); break;
                    }
                    break;
                case "or":
                    words2 = words[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    op2 = words2[0];
                    op3 = words2[1];
                    switch (op2)
                    {
                        case "ah": reg[0] = OpOr(reg[0], op3); break;
                        case "al": reg[1] = OpOr(reg[1], op3); break;
                        case "bh": reg[2] = OpOr(reg[2], op3); break;
                        case "bl": reg[3] = OpOr(reg[3], op3); break;
                        case "dh": reg[4] = OpOr(reg[4], op3); break;
                        case "dl": reg[5] = OpOr(reg[5], op3); break;
                        case "ch": reg[6] = OpOr(reg[6], op3); break;
                        case "cl": reg[7] = OpOr(reg[7], op3); break;
                    }
                    break;
                case "xor":
                    words2 = words[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    op2 = words2[0];
                    op3 = words2[1];
                    switch (op2)
                    {
                        case "ah": reg[0] = OpXor(reg[0], op3); break;
                        case "al": reg[1] = OpXor(reg[1], op3); break;
                        case "bh": reg[2] = OpXor(reg[2], op3); break;
                        case "bl": reg[3] = OpXor(reg[3], op3); break;
                        case "dh": reg[4] = OpXor(reg[4], op3); break;
                        case "dl": reg[5] = OpXor(reg[5], op3); break;
                        case "ch": reg[6] = OpXor(reg[6], op3); break;
                        case "cl": reg[7] = OpXor(reg[7], op3); break;
                    }
                    break;
                case "not":
                    words2 = words[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    op2 = words2[0];
                    switch (op2)
                    {
                        case "ah": reg[0] = OpNot(reg[0]); break;
                        case "al": reg[1] = OpNot(reg[1]); break;
                        case "bh": reg[2] = OpNot(reg[2]); break;
                        case "bl": reg[3] = OpNot(reg[3]); break;
                        case "dh": reg[4] = OpNot(reg[4]); break;
                        case "dl": reg[5] = OpNot(reg[5]); break;
                        case "ch": reg[6] = OpNot(reg[6]); break;
                        case "cl": reg[7] = OpNot(reg[7]); break;
                    }
                    break;
                case "movs":
                    words2 = words[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    op2 = words2[0];
                    op3 = words2[1];
                    OpMovs(op2, op3);
                    break;
                case "swap":
                    op1 = words[1];
                    switch (op1)
                    {
                        case "ah": reg[0] = OpSwap(reg[0]); break;
                        case "al": reg[1] = OpSwap(reg[1]); break;
                        case "bh": reg[2] = OpSwap(reg[2]); break;
                        case "bl": reg[3] = OpSwap(reg[3]); break;
                        case "dh": reg[4] = OpSwap(reg[4]); break;
                        case "dl": reg[5] = OpSwap(reg[5]); break;
                        case "ch": reg[6] = OpSwap(reg[6]); break;
                        case "cl": reg[7] = OpSwap(reg[7]); break;
                    }
                    break;
                case "test":
                    words2 = words[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    op2 = words2[0];
                    op3 = words2[1];
                    switch (op2)
                    {
                        case "ah": OpTest(reg[0], toBin(int.Parse(op3))); break;
                        case "al": OpTest(reg[1], toBin(int.Parse(op3))); break;
                        case "bh": OpTest(reg[2], toBin(int.Parse(op3))); break;
                        case "bl": OpTest(reg[3], toBin(int.Parse(op3))); break;
                        case "dh": OpTest(reg[4], toBin(int.Parse(op3))); break;
                        case "dl": OpTest(reg[5], toBin(int.Parse(op3))); break;
                        case "ch": OpTest(reg[6], toBin(int.Parse(op3))); break;
                        case "cl": OpTest(reg[7], toBin(int.Parse(op3))); break;
                    }
                    break;
                case "neg":
                    op1 = words[1];
                    switch (op1)
                    {
                        case "ah": reg[0] = OpNeg(reg[0]); break;
                        case "al": reg[1] = OpNeg(reg[1]); break;
                        case "bh": reg[2] = OpNeg(reg[2]); break;
                        case "bl": reg[3] = OpNeg(reg[3]); break;
                        case "dh": reg[4] = OpNeg(reg[4]); break;
                        case "dl": reg[5] = OpNeg(reg[5]); break;
                        case "ch": reg[6] = OpNeg(reg[6]); break;
                        case "cl": reg[7] = OpNeg(reg[7]); break;
                    }
                    break;
                case "cmp":
                    words2 = words[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    op2 = words2[0];
                    op3 = words2[1];
                    switch (op2)
                    {
                        case "ah": OpCmp(reg[0], toBin(int.Parse(op3))); break;
                        case "al": OpCmp(reg[1], toBin(int.Parse(op3))); break;
                        case "bh": OpCmp(reg[2], toBin(int.Parse(op3))); break;
                        case "bl": OpCmp(reg[3], toBin(int.Parse(op3))); break;
                        case "dh": OpCmp(reg[4], toBin(int.Parse(op3))); break;
                        case "dl": OpCmp(reg[5], toBin(int.Parse(op3))); break;
                        case "ch": OpCmp(reg[6], toBin(int.Parse(op3))); break;
                        case "cl": OpCmp(reg[7], toBin(int.Parse(op3))); break;
                    }
                    break;
                case "loop":
                    op1 = words[1];
                    OpLoop(op1);
                    break;
                case "jmp":
                    op1 = words[1];
                    OpJmp(op1);
                    break;
                case "je":
                    op1 = words[1];
                    OpJe(op1);
                    break;
                case "jne":
                    op1 = words[1];
                    OpJne(op1);
                    break;
                case "jg":
                    op1 = words[1];
                    OpJg(op1);
                    break;
                case "jl":
                    op1 = words[1];
                    OpJl(op1);
                    break;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (CF == 0) CF = 1;
            else CF = 0;
            updateInfo();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (ZF == 0) ZF = 1;
            else ZF = 0;
            updateInfo();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (SF == 0) SF = 1;
            else SF = 0;
            updateInfo();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (OF == 0) OF = 1;
            else OF = 0;
            updateInfo();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            CF = 0;
            ZF = 0;
            SF = 0;
            OF = 0;
            DF = 0;
            IF = 0;
            TF = 0;
            for (int i = 0; i < 8; i++) reg[i] = "00000000";
            for (int i = 0; i < 16; i++) mem[i] = "00000000";
            textBox5.Clear();
            
            updateInfo();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (DF == 0) DF = 1;
            else DF = 0;
            updateInfo();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.InitialDirectory = @"C:\Users\extev\Desktop\Study\Организация ЭВМ\лабы\labs 5-10\5-10labs_EVM\5-10labs_EVM\bin\Debug";
            if (fd.ShowDialog() == DialogResult.OK)
            {
                textBox6.Text = Encoding.UTF8.GetString(File.ReadAllBytes(fd.FileName));
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            String[] str = textBox6.Text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            while (IP < str.Length)
            {
                Comands(IP);
                IP++;
            }
            updateInfo();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Comands(IP);
            IP++;
            updateInfo();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (IF == 0) IF = 1;
            else IF = 0;
            updateInfo();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (TF == 0) TF = 1;
            else TF = 0;
            updateInfo();
        }
    }
}
