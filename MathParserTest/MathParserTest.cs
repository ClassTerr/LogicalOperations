/*
 * Author: Patrik Lundin, patrik@lundin.info
 * Web: http://www.lundin.info
 * 
 * Source code released under the Microsoft Public License (Ms-PL) 
 * http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
*/
using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using info.lundin.math;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace info.lundin.math
{
    public partial class MathParserTest : Form
    {
        public MathParserTest()
        {
            InitializeComponent();
        }

        List<Node> shortExpr = new List<Node>();
        List<string> varNames = new List<string>();
        int labelCounter = 0;
        int deep = 0;

        int n=0;
        int h;
        int w;

        string[,] table;

        private void LPK(Node n)
        {
            if (n == null)
                return;

            if (n.Type != NodeType.Expression)
            {
                if (n.Type == NodeType.Value)
                    n.Label = n.FullExpression = n.Value.ToString();
                else n.Label = n.FullExpression = n.Variable;
                return;
            }

            if (n.SecondArgument == null)//кусь говнокод конечно, но в 3:20 ночи я не хо придумывать что-то лучше
            {
                deep++;
                LPK(n.FirstArgument);
                deep--;

                n.FullExpression = n.Operator.ToString() + "(" + n.FirstArgument.FullExpression + ")";

                if (deep == 0)
                    n.Label = "F";
                else n.Label = ((char)('A' + labelCounter++)).ToString();

                n.ShortExpression = n.Operator.ToString() + "(" + n.FirstArgument.Label + ")";
                if (n.Label == "E")
                    labelCounter++;
                shortExpr.Add(n);
                return;
            }

            deep++;
            LPK(n.FirstArgument);
            LPK(n.SecondArgument);
            deep--;

            n.FullExpression = "(" + n.FirstArgument.FullExpression + n.Operator.ToString() + n.SecondArgument.FullExpression + ")";

            if (deep == 0)
                n.Label = "F";
            else n.Label = ((char)('A' + labelCounter++)).ToString();

            n.ShortExpression = n.FirstArgument.Label + n.Operator.ToString() + n.SecondArgument.Label;
            if (n.Label == "E")
                labelCounter++;
            shortExpr.Add(n);
        }

        void Increment(List<int> lst)
        {
            for (int i = lst.Count - 1; i >= 0; i--)
            {
                lst[i] = (lst[i] + 1) % 2;
                if (lst[i] == 1)
                    break;
            }
        }

        public bool IsVariable(string s)
        {
            if (s.IndexOf('v') != -1) return false;
            if (s.IndexOf('V') != -1) return false;
            return Regex.IsMatch(s, @"^(_|[a-z]|[A-Z])\w*$");
        }

        public List<string> GetAllVariables(string s)
        {
            DefaultOperators op = new DefaultOperators();
            HashSet<string> set = new HashSet<string>();
            List<string> res = new List<string>();
            foreach (var item in op.Operators)
                s = s.Replace(item.Symbol, " ");

            s = s.Replace('(', ' ').Replace(')', ' ');

            var arr = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var item in arr)
                if (IsVariable(item))
                    set.Add(item);

            foreach (var item in set)
                res.Add(item);

            res.Sort();

            return res;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            labelCounter = 0;
            listView1.Items.Clear();
            listView1.Columns.Clear();
            varNames.Clear();
            shortExpr.Clear();
            textBox2.Clear();
           
            ExpressionParser oParser = new ExpressionParser();

            string sFunction = BOX.Text.Trim();
            double fResult = 0f;

            try
            {
                // Parse expression once
                fResult = oParser.Parse(sFunction);

                // Fetch parsed tree
                Expression expression = oParser.Expressions[sFunction];
                LPK(expression.ExpressionTree);

                List<string> vars = GetAllVariables(sFunction);
                n = vars.Count;
                h = (int)Math.Pow(2, n) + 1;
                w = n + shortExpr.Count + 1;

                table = new string[h, w];

                table[0, 0] = "N";

                foreach (var item in vars)
                    varNames.Add(item);

                varNames.Sort();

                for (int i = 0; i < n; i++)
                    table[0, i + 1] = varNames[i].ToLower();

                for (int i = n + 1; i < w; i++)
                    table[0, i] = shortExpr[i - n - 1].Label + " = " + shortExpr[i - n - 1].ShortExpression;

               for (int i = 0; i < w; i++)
                    listView1.Columns.Add(table[0, i], 70);

               List<int> sets = new List<int>();
                for (int i = 0; i < n; i++)
                    sets.Add(0);

                for (int i = 1; i < h; i++)
                {
                    oParser.Values.Clear();

                    table[i, 0] = (i - 1).ToString();

                    for (int j = 1; j <= sets.Count; j++)
                        table[i, j] = sets[j - 1].ToString();

                    for (int j = 0; j < varNames.Count; j++)
                        oParser.Values.Add(varNames[j], sets[j]);

                    for (int j = 0; j < shortExpr.Count; j++)
                        table[i, j + vars.Count + 1] = oParser.Parse(shortExpr[j].FullExpression).ToString();

                    ListViewItem row = new ListViewItem();
                    row.Text = table[i, 0];
                    for (int j = 1; j < w; j++)
                        row.SubItems.Add(table[i, j]);

                    listView1.Items.Add(row);
                    Increment(sets);
                }

                
                if (textBox2.Text == "")
                    textBox2.Text = "Таблица успешно построена!";
            }
            catch (Exception ex)
            {
                textBox2.Text = ex.Message;
            }
        }

        private void insert_Click(object sender, EventArgs e)
        {
            string s = (sender as Button).Text;
            string s1 = BOX.Text.Substring(0, BOX.SelectionStart);
            string s2 = BOX.Text.Substring(BOX.SelectionStart + BOX.SelectionLength, BOX.Text.Length - BOX.SelectionStart - BOX.SelectionLength);
            BOX.Text = s1 + s + s2;
            BOX.SelectionStart = s1.Length + s.Length;
            BOX.SelectionLength = 0;
            BOX.Focus();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            DNF.Clear();
            CNF.Clear();

            if (n == 0)
                textBox2.Text = "Для начала постройте таблицу истинности!";
            else
            {
                bool tavt = true, otr = true;
                for (int i = 1; i < h; i++)
                    if (table[i, w - 1] == "1")
                    {
                        if (DNF.Text != "") DNF.Text += " ⋁ ";
                        for (int j = 1; j <= varNames.Count; j++)
                            if (table[i, j] == "0")
                                DNF.Text += "!" + table[0, j];
                            else
                                DNF.Text += table[0, j];
                        tavt = false;
                    }
                if (tavt) textBox2.Text = "Данное логическое выражение является отрицанием. Поэтому построить СДНФ невозможно";
                for (int i = 1; i < h; i++)
                    if (table[i, w - 1] == "0")
                    {
                        if (CNF.Text != "") CNF.Text += " ⋀ (";
                        else CNF.Text += "(";
                        bool f = true;
                        for (int j = 1; j <= varNames.Count; j++)
                        {
                            if (!f) CNF.Text += " V ";
                            if (table[i, j] == "1")
                                CNF.Text += "!" + table[0, j];
                            else
                                CNF.Text += table[0, j];
                            f = false;
                        }
                        CNF.Text += ")";
                        otr = false;
                    }
                if (otr) textBox2.Text = "Данное логическое выражение является тавтологией. Поэтому построить СКНФ невозможно";
                if(!otr && !tavt)
                    textBox2.Text = "СДНФ и СКНФ успешно построены!";
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            About a = new About();
            a.ShowDialog(this);
        }
        
    }
}
