using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
namespace AG_Project_Code_G13
{
    // فورم التنفيذ
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            //
            nodes = new List<TreeNode>(); // عدد العقد تخزن فيها العقد كامل 
            nodes2 = new List<TreeNode>();// تخزن العقد عند التنفيذ خطوة بخطوة
            // مصفوفة يتم تخزين الشجرة الممتدة من الخوارزمية الناتج للخوارزمية يخزن فيها
            traversalResult = new List<string>();
            //
            Panel_Steps.Paint += stepDrawingPanel_Paint;
        }
        public int newCount; //عدد العقد
        //تعريف المصفوفات
        public TextBox[,] weightMatrix;       // مصفوفة الأوزان
        private TextBox[,] adjacencyMatrix;    // مصفوفة الجوار
        public int nodeCount;//   عدد العقد المحدد من المستخدم
        private List<TreeNode> nodes;//قائمة لتخزين العقد
        Pen p = new Pen(Color.Black, 2);//اعداتات القلم
        TreeNode root = new TreeNode();// جذر الشجرة
        private List<TreeNode> nodes2;//مصفوفة تخزين عنصر تنفيذ خطوه بخطوه
        private List<string> traversalResult;//مصفوفة تخزين ناتج تنفيذ الخوارزميات
        int i = 1;//متغير نستخده لدالة تسمية العقد
        int j = 0;//يستخدم في مصفوفة التسميات العقد الاندكس
        string[] NodeName = new string[30];//مصفوف تعمل على عدم تكرار العقد تسميات العقد 
        string[] name = new string[20];//اسماء العقد
        string[] y2 = new string[20];//مصفوفة تستخدم عند التنفيذ خطوه بخطوة تحدد العقد التي يجب نلوينها فقط
        ///////// الاحداث //////////////////////////
        /// //////////////////////////////////////////////////////////////////
        /// حدث منع الادخال لمصفوفة الجوار 
        void isnumric_adjacencyMatrix(object sender, KeyPressEventArgs e)
        {
            TextBox t = new TextBox();
            t = ((TextBox)sender);
            if ((e.KeyChar != 49) || t.TextLength > 0)
                e.Handled = true;
            if (e.KeyChar == 8)//حتي تستطيع الحذف
                e.Handled = false;
        }
        void isnumric_weightMatrix(object sender, KeyPressEventArgs e)
        {

            if (e.KeyChar < 48 || e.KeyChar > 57)
                e.Handled = true;
            if (e.KeyChar == 8)//حتي تستطيع الحذف
                e.Handled = false;
            if (comboBox2.Text == "خوارزمية فلويد" && e.KeyChar == '-')
                e.Handled = false;

        }
        void keypress_char(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= 'a' && e.KeyChar <= 'z') || (e.KeyChar >= 'A' && e.KeyChar <= 'Z'))
                e.Handled = false;
            else
                e.Handled = true;
        }
        //الرجوع للشاشة الرئيسية
        private void button7_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            Nodes_Count.KeyPress += isnumric_weightMatrix;
            nodenametextbox.KeyPress += keypress_char;
            nodenametextbox.Enabled = Nodes_Count.Enabled = false;
            textend.Visible = false;
            textstart.Visible = false;
            label11.Visible = false;
            label12.Visible = false;
            textend.TextChanged += textstart_TextChanged;// ربط حدث التظعيف لادخال نقطتين لخوارزميات ايجاد المسافه بين نقطنين
        }
        //مربع لادخال عدد العقد
        private void Nodes_Count_TextChanged(object sender, EventArgs e)
        {
            if (Nodes_Count.Text.Trim() == "0" || Nodes_Count.TextLength > 1)
            { // في حال ادخل المستخدم صفر 
                Nodes_Count.Text = "";
                return;
            }
          //  تهيئة مصفوفة الاسماء عند ادخال لعدد العقد في التنفيذ الواحد
            i = 1;
            NodeName = new string[10];
            nodenametextbox.Enabled = true; // فتح مربع ادخال اسماء العقد
            if (Nodes_Count.Text.Trim() != "")
            {
                newCount = Convert.ToInt32(Nodes_Count.Text);
                nodeCount = newCount + 1;// عدد اسماء العقد مع الزياده بواحد لاظافة صف وعمود للمصفوفات الجوار والاوزان
                b = newCount;
                if (RadioGraphe.Checked)
                {
                    drawingPanel.Paint += DrawingPanel_Paint; //ربط البنل بحدث الرسم للرسم العقد بدون تسميات 
                    BtnDrawTree();// دالة رسم العقدد 
                    // تحديث عدد العق
                    InitializeMatrices(weightPanel, adjacencyPanel, 30, nodeCount);// إعادة إنشاء المصفوفات                                                             //  DrawNodes(); // استدعاء دالة الرسم لإظهار العقد
                }
                else if (RadioTree.Checked)
                {
                    drawingPanel.Paint += DrawingPanel_Paint;
                    BtnDrawTree();
                    InitializeMatrices(weightPanel, adjacencyPanel, 30, nodeCount);// إعادة إنشاء المصفوفات                                                             //  DrawNodes(); // استدعاء دالة الرسم لإظهار العقد
                    addTextBox_TextChanged(); // ربط حدث الادخال للمصفوفة الجوار للرسم اثناء ادخال قيم للمصفوفة الجوار
                }
            }
            else
            {
                drawingPanel.Invalidate();
                nodes.Clear();// حذف العقد
                  //  تنظيف المصفوفات الجوار مع الاوزان
                matrix_clear(weightMatrix, weightPanel); 
                matrix_clear(adjacencyMatrix, adjacencyPanel);
            }
        }
        //انشاء العقد
        int c = 0;//منغير يقوم بازيادة عند التنفيذ خطوة بخكوة بكل نقرة يقوم بازبادة بواحد
        private void BtnDrawTree()
        {
            if (c == 0)
            {
                // إعداد الشجرة
                nodes.Clear();
                for (int i = 0; i < nodeCount - 1; i++)
                {
                    nodes.Add(new TreeNode("")); // إضافة العقد بدون أسماء
                }
                drawingPanel.Invalidate(); // إعادة رسم Panel
            }

        }
        bool flag=false ; //يتم استخدامه لنحدد اذا كان نريد نرسم العقد في مربع تنفيذ خطوة بخطوة
        //حدث الرسم 
        private void DrawingPanel_Paint(object sender, PaintEventArgs e)
        {
            if (RadioTree.Checked)
            {
                // رسم الشجرة عند كل رسم
                if (nodes.Count > 0)
                {
                  
                    DrawTree(e.Graphics, root, 0, drawingPanel.Width / 2, 40, 70); // بدء رسم الشجرة
                }
            }
            else if (RadioGraphe.Checked)
            {
                if (nodes.Count > 0)
                {
                    Dreawtrr(e.Graphics, 0, drawingPanel.Width / 2, 35);
                }
            }
            
        }
     //   حدث رسم الجراف او الاشجار خطوة بخطوة
        private void stepDrawingPanel_Paint(object sender, PaintEventArgs e)
        {
            // رسم الشجرة عند كل رسم
            if (nodes2.Count > 0 && RadioTree.Checked)
            {
                DrawT(e.Graphics, root, 0, Panel_Steps.Width / 2, 40, 70); // بدء رسم الشجرة
            }
            if (nodes2.Count > 0 && RadioGraphe.Checked)
                Dreawt(e.Graphics, 0, Panel_Steps.Width / 2, 35);//e.Graphics, c-1, Panel_Steps.Width / 2, 35

        }
        //خوارزميات الاجتياز الاجتياز السابق
        private void PreOrderTraversal( bool[] vis, int index)
        {
            if (vis[index])
                return;
            vis[index] = true;
          
            traversalResult.Add(nodes[index].Value);
    
            for (int i = 0; i < adjacency.GetLength(0); i++)
            {

                if (adjacency[index, i] == 1 && !vis[i])
                {
                            PreOrderTraversal( vis, i);
                }
            }

        }
    //   خوارزمية الاجتياز الداخل
        private void InOrderTraversal(int index)
        {
            for (int i = 0; i < adjacency.GetLength(0); i++)
            {
                if (adjacency[index, i] == 1)
                {
                    InOrderTraversal(i);//
                    break;//اجتياز اول عقده يسري فقط
                }
            }
            traversalResult.Add(nodes[index].Value);
          // زيارة العقدة اليمني
            bool f = false;
            for (int i = 0; i < adjacency.GetLength(0); i++)
            {
                if (adjacency[index, i] == 1)
                {
                    if(f)//اجتياز العقد اليمني بعد اليسري
                    InOrderTraversal( i);
                    f = true;
                }
            }
        }
        //الاجتياز الاحق
        private void PostOrderTraversal( bool[] vis, int index)
        {
            if (vis[index])
                return;
            vis[index] = true;
        
            for (int i = 0; i < adjacency.GetLength(0); i++)
            {

                if (adjacency[index, i] == 1 && !vis[i])
                {
                            PostOrderTraversal( vis, i); 
                }
            }

            traversalResult.Add(nodes[index].Value);
        }
       // دالة ايجاد طول الشجرة
        private void GetHeight(TreeNode g)
        {
            if (g == null) return;
            GetHeight(g.Left);
            traversalResult.Add(g.Value);
        }
        //دالة رسم العقدة
        private void DrawNode(Graphics g, string node, int x, int y)
        {
            g.FillEllipse(Brushes.White, x - 15, y - 15, 30, 30); // رسم الدائرة
            g.DrawEllipse(p, x - 15, y - 15, 30, 30); // رسم إطار الدائر
            g.DrawString(node.ToLower(), this.Font, Brushes.Black, x - 10, y - 10); // رسم اسم العقدة
        }
        //دالة تقوم برسم الاشجار خطوة خطوة
        private void DrawT(Graphics g, TreeNode root, int index, int x, int y, int offset)
        {
            if (index > nodes2.Count)//بكل نقرة يتم اضافة عنصر للمصفوفة nodes2 ويتم رسم العقد حسب العناصر الموجودة بالمصفوفة
                return; // إذا كان المؤشر خارج نطاق العقد
            y2[index] = traversalResult[index];
            Drawing_form_3.Invalidate();
            Drawing_form_3.Paint += DrawingPanel_Paint;// تحديث الرسم في الفورم الثالث اشارة الي طريقة تنفيذ الخوارزمية وتلوين الروابط الخوارزمية العقد
            // حساب إحداثيات العقد اليسرى واليمنى
            int leftIndex = 2 * index + 1; // العقدة اليسرى
            int rightIndex = 2 * index + 2; // العقدة اليمنى
            // رسم الحافة إلى العقدة اليسرى
            if (leftIndex < nodes2.Count)
            {
                int leftX = x - offset;
                int leftY = y + 60; // ارتفاع العقدة
                //g.DrawLine(p, x, y + 15, leftX, leftY - 15); // رسم الرابط اليسار
                DrawT(g, root.Left, leftIndex, leftX, leftY, offset / 2); // رسم العقدة اليسرى
            }
            // رسم الحافة إلى العقدة اليمنى
            if (rightIndex < nodes2.Count)
            {
                int rightX = x + offset;
                int rightY = y + 60; // ارتفاع العقدة
                //g.DrawLine(p, x, y + 15, rightX, rightY - 15); // رسم الرابط اليمين
                DrawT(g, root.Right, rightIndex, rightX, rightY, offset / 2); // رسم العقدة اليمنى
            }
        }
        int l = 0,b;
        //دالة تقوم برسم الجرف
        void Dreawtrr(Graphics g, int index, int x, int y)
        {
            if (index >= nodes.Count)
                return; // إذا كان المؤشر خارج نطاق العقد
            var node = nodes[index];
            root.Value = nodes[index].Value;
            node.X = x; // تحديث إحداثيات العقدة
            node.Y = y;
            if (index % 2 == 1)
            { x -= 90; l = 1; }
            else if (index % 2 == 0)
            { x += 90; l = 0; }
            if (l == 1)
            {
                y = y + 70;
            }
            if (index == nodeCount - 2 && (nodeCount - 1) % 2 == 0)
            {
                x = drawingPanel.Width / 2; y = y - 20;
            }
            if (index == 0)
            {
                x = drawingPanel.Width / 2;
                y = 50;
                DrawNode(g, node, x, y);
                nodes[index].X = x;
                nodes[index].Y = y;
            }
            else
            {
                DrawNode(g, node, x, y);
                nodes[index].X = x;
                nodes[index].Y = y;
            }
            x = drawingPanel.Width / 2;
            index++;
            Dreawtrr(g, index, x, y);
        }
        bool t = false; //متغير نستخدمه لنحدد اذا كانت يوجد للخوارزمية مصفوفة اوزان او لا
        //دالة تقوم برسم الجرف خطوة خطوة
        void Dreawt(Graphics g, int index, int x, int y)//c-1, Panel_Steps.Width / 2, 35
        {
            if (index >= nodes2.Count)
                return; // إذا كان المؤشر خارج نطاق العقد
            var node = nodes2[index];
            node.Value = traversalResult[index];
            node.X = x; // تحديث إحداثيات العقدة
            node.Y = y;
            bool flag = comboBox3.Text != "بحث العمق اولا(DFS)" && comboBox3.Text != "البحث العرض اولا(BFS)";
            nodes2[index].Value = traversalResult[index];
            y2[index] = traversalResult[index];
            if (index % 2 == 1)
            { x -= 70; l = 1; }
            else if (index % 2 == 0)
            { x += 70; l = 0; }
            if (l == 1) y = y + 50;
            if (index == nodeCount - 2 && (nodeCount - 1) % 2 == 0)
            {
                x = Panel_Steps.Width / 2; y = y - 10;
            }
            if (index == 0)
            {
                x = Panel_Steps.Width / 2;
                y = 35;
                if(flag)
                DrawNode(g, traversalResult[index], x, y);
                nodes2[index].X =x;
                nodes2[index].Y =y;
            }
            else
            {
                if(flag)
                DrawNode(g, traversalResult[index], x, y);
                nodes2[index].X = x;
                nodes2[index].Y = y;
            }
            if(flag)
            if (index > 0)
                drawing_panel_stap(index, index - 1);
            x = Panel_Steps.Width / 2;
            index++;
            Dreawt(g, index, x, y);
        }
        //رسم الشجره
        private void DrawTree(Graphics g, TreeNode root, int index, int x, int y, int offset)
        {
                if (index > nodes.Count)
                    return;
            var node = nodes[index];
            root.Value = nodes[index].Value;
            node.X = x; // تحديث إحداثيات العقدة
            node.Y = y;
            DrawNode(g, node, x, y); // رسم العقدة الحالية بدون تسميات
            update_Drawing(Drawing_form_3);
            // حساب إحداثيات العقد اليسرى واليمنى
            int leftIndex = 2 * index + 1; // العقدة اليسرى
            int rightIndex = 2 * index + 2; // العقدة اليمنى
            // رسم الحافة إلى العقدة اليسرى
            if (leftIndex < nodes.Count)
            {
                root.Left = new TreeNode(null);
                int leftX = x - offset;
                int leftY = y + 60; // ارتفاع العقدة
                DrawTree(g, root.Left, leftIndex, leftX, leftY, offset / 2); // رسم العقدة اليسرى
            }
            // رسم الحافة إلى العقدة اليمنى
            if (rightIndex < nodes.Count)
            {
                root.Right = new TreeNode(null);
                int rightX = x + offset;
                int rightY = y + 60; // ارتفاع العقدة
                DrawTree(g, root.Right, rightIndex, rightX, rightY, offset / 2); // رسم العقدة اليمنى
            }
        }
        //رسم العقدة
        private void DrawNode(Graphics g, TreeNode node, int x, int y)
        {
            if (y2 != null) //في حال كانت المصفوفة فاضية يتم رسم العقد بدون بشكل طبيعي
                if (y2.Contains(node.Value))// اذا كانت العقد موجودة بالمصفوفة يتم تلوينها باللون الاحمر
                {
                    g.FillEllipse(Brushes.Red, x - 15, y - 15, 30, 30); // رسم الدائرة
                }
                else
                    g.FillEllipse(Brushes.White, x - 15, y - 15, 30, 30); // رسم الدائر
            g.DrawEllipse(p, x - 15, y - 15, 30, 30); // رسم إطار الدائرة
            if (!string.IsNullOrEmpty(node.Value))// اذا كان يوجد اسم للعقدة يتم رسمها كع الاسم
            {
                g.DrawString(node.Value.ToUpper(), this.Font, Brushes.Black, x - 10, y - 10); // رسم اسم العقدة
            }
        }
        private void InitializeMatrices(Panel weightPanel, Panel adjacencyPanel, int textBoxSize, int Count)
        {
            matrix_clear(weightMatrix, weightPanel);
            matrix_clear(adjacencyMatrix, adjacencyPanel);
            if (RadioGraphe.Checked)
            {
                if (balence.Checked)
                {
                    weightMatrix = new TextBox[nodeCount, nodeCount]; 
                    CreateMatrix(weightMatrix, weightPanel, true);
                    adjacencyMatrix = new TextBox[nodeCount, nodeCount];
                    CreateMatrix(adjacencyMatrix, adjacencyPanel, false);
                }
                else if (Unbalance.Checked)
                {

                    adjacencyMatrix = new TextBox[nodeCount, nodeCount];
                    CreateMatrix(adjacencyMatrix, adjacencyPanel, false);
                }
            }
            else if (RadioTree.Checked)
            {
                adjacencyMatrix = new TextBox[nodeCount, nodeCount];
                CreateMatrix(adjacencyMatrix, adjacencyPanel, false);
            }
        }
        //دالة للرسم المصفوفات الجوار والاوزان
        private void CreateMatrix(TextBox[,] matrix, Panel panel, bool f)
        {
            for (int i = 0; i < nodeCount; i++)
            {
                for (int j = 0; j < nodeCount; j++)
                {
                    //لايتم رسم انشاء المربع في النقاط الاولي
                    if (i == 0 && j == 0)
                        continue;
                    matrix[i, j] = new TextBox
                    {
                        Multiline = true,
                        Width = 30,
                        Height = 25,
                        Left = j * (30) + 5,
                        Top = i * (25) + 30,
                        TextAlign = HorizontalAlignment.Center,
                        BorderStyle = BorderStyle.FixedSingle,
                    };
                    //ربط الاحداث حسب نوع المصفوفة
                    if (balence.Checked)
                    {
                        //اذا كانت المصفوف موزون 
                        if (f)
                            matrix[i, j].KeyPress += isnumric_weightMatrix;
                        else
                            matrix[i, j].KeyPress += isnumric_adjacencyMatrix;//اذا كانت المصفوف الجوار
                    }
                    else
                        matrix[i, j].KeyPress += isnumric_adjacencyMatrix;//اذا كانت المصفوفة غير موزونه فيتم ربط حدث مصفوفة الجوار فقط
                    //تلوين مربعات الخاصة بالاسماء
                    if (i == 0 || j == 0)
                    {
                        matrix[i, j].BackColor = Color.WhiteSmoke;
                    }
                    else
                        matrix[i, j].BackColor = Color.White;
                    panel.Controls.Add(matrix[i, j]);
                }
            }
        }
        //دالة تقوم بحذف المصفوفات في حال تم حذف عدد العقد من المربع
        void matrix_clear(TextBox[,] a, Panel p)
        {
            if (a != null)
            {
                foreach (TextBox c in a)
                {
                    p.Controls.Remove(c);
                }
            }
            a = null;
            newCount = 0;
        }
        //دالة ادخال النسميات
        private void nodenametextbox_TextChanged_1(object sender, EventArgs e)
        {
            if (nodenametextbox.Text != "")
            {
                if (RadioTree.Checked)
                {
                    if (i != 0)// متغير يتم اسنخدامه لادخال التسميات حسب عدد العقد المدخلة من المستخدم فقط ويتم تجميد المربع
                    {
                            if (!NodeName.Contains(nodenametextbox.Text))// في حال كان الاسم موجود لا يتم تكرارة
                            {
                                NodeName[i - 1] = nodenametextbox.Text;
                                nodes[i - 1].Value = nodenametextbox.Text;
                                drawingPanel.Invalidate();
                                adjacencyMatrix[0, i].Text = nodenametextbox.Text;
                                adjacencyMatrix[0, i].Enabled = false;
                                adjacencyMatrix[i, 0].Text = nodenametextbox.Text;
                                adjacencyMatrix[i, 0].Enabled = false;
                                i++;
                                if (i == nodeCount)
                                {
                                    i = 0;
                                    NodeName = new string[10];
                                    nodenametextbox.Enabled = false;
                                }
                               }
                            else
                                MessageBox.Show("الاسم موجود سابقا");
                        }
                }
                else if (RadioGraphe.Checked)
                {
                    if (i != 0)
                    {
                        if (!NodeName.Contains(nodenametextbox.Text))
                        {
                            NodeName[i - 1] = nodenametextbox.Text;
                            nodes[i - 1].Value = nodenametextbox.Text;
                            drawingPanel.Invalidate();
                            if (Unbalance.Checked == false && balence.Checked == true)
                            {
                                weightMatrix[0, i].Text = nodenametextbox.Text;
                                weightMatrix[0, i].Enabled = false;
                                weightMatrix[i, 0].Text = nodenametextbox.Text;
                                weightMatrix[i, 0].Enabled = false;
                            }
                            adjacencyMatrix[0, i].Text = nodenametextbox.Text;
                            adjacencyMatrix[0, i].Enabled = false;
                            adjacencyMatrix[i, 0].Text = nodenametextbox.Text;
                            adjacencyMatrix[i, 0].Enabled = false;
                            i++;
                            if (i == nodeCount)
                            {
                                adjacencyMatrix[1,1].Focus();
                                i = 0;
                                NodeName = new string[10];
                                nodenametextbox.Enabled = false;
                                Undirction_CheckedChanged(null, null);
                                dirction_CheckedChanged(null, null);
                            }
                        }
                        else
                            MessageBox.Show("الاسم موجود سابقا");
                    }
                }
                nodenametextbox.Text = "";
            }
        }
        //دوال التحكم و الدخال الى المصفوفات الجور والاوزان
        /// <summary>
        /// ////////////////////////////////////////////////////////////////
        private void Unbalance_CheckedChanged(object sender, EventArgs e)
        {
            if (Unbalance.Checked)
                matrix_clear(weightMatrix, weightPanel);
        }
        //عند حفظ المصفوفات يتم عمل تغير اللوت للمربعات  الفعالة
        void color_weightMatrix_adjacencyMatrix()
        {
            for (int i = 1; i < nodeCount; i++)
            {
                for (int j = 1; j < nodeCount; j++)
                {
                    if (adjacencyMatrix[i, j].Text == "")
                    {
                        adjacencyMatrix[i, j].BackColor = Color.LightGray;
                        adjacencyMatrix[i, j].Enabled = false;
                        weightMatrix[i, j].BackColor = Color.LightGray;
                        weightMatrix[i, j].Enabled = false;
                    }
                }
            }
        }
       // في حال كانت الخوارزمية تستخدم مصفوفة جوار فقط يتم تلوينها فقط
        void color_adjacencyMatrix()
        {
            for (int i = 1; i < nodeCount; i++)
            {
                for (int j = 1; j < nodeCount; j++)
                {
                    if (adjacencyMatrix[i, j].Text == "")
                    {
                        adjacencyMatrix[i, j].BackColor = Color.LightGray;
                        adjacencyMatrix[i, j].Enabled = false;
                    }
                }
            }
        }
        void color()
        {
            if (RadioTree.Checked)
                //دالة تلوين مصفوفة الجوار في الاشجار
                color_adjacencyMatrix();
            if (balence.Checked)
                //دالة تلوين مصفوفة الجوار والاوزان 
                color_weightMatrix_adjacencyMatrix();
            else if (Unbalance.Checked)
                //دالة تلوين مصفوفة الجوار فقط
                color_adjacencyMatrix();

        }
        //دالة تضيف العنصر للراس العقدة المقابل
        void add_adjacency(TextBox temp)
        {
            for (int i = 1; i < nodeCount; i++)
            {
                for (int j = 1; j < nodeCount; j++)
                {
                    if (adjacencyMatrix[i, j] == temp)
                    {
                        adjacencyMatrix[j, i].Text = temp.Text;
                        break;
                    }
                }
            }
        }
        void add_weight(TextBox temp)
        {
            for (int i = 1; i < nodeCount; i++)
            {
                for (int j = 1; j < nodeCount; j++)
                {
                    if (weightMatrix[i, j] == temp)
                    {
                        weightMatrix[j, i].Text = temp.Text;
                        break;
                    }
                }
            }
        }
        void adjacency_TextBox_TextChanged(object send, EventArgs e)
        {
            if (((TextBox)send).Text != "")
            {
                update_Drawing(drawingPanel);
                bool t = true;

                if (Undirction.Checked)
                {
                    add_adjacency((TextBox)send);
                    // دالة تقوم بالربط بين العقد حسب مصفوفة الجوار او الاوازان 
                    Undirction_dirction_adjacencyMatrix_DrawConnections((TextBox)send, drawingPanel, t);
                }
                if (dirction.Checked)
                {
                    Undirction_dirction_adjacencyMatrix_DrawConnections((TextBox)send, drawingPanel, t);
                }
                if (RadioTree.Checked)
                {
                    Undirction_dirction_adjacencyMatrix_DrawConnections((TextBox)send, drawingPanel, t);
                }
            }
            else
            {
                TextBox t = new TextBox();
                t = (TextBox)send;
                t.Text = "";
                add_adjacency(t);
                drawingPanel.Invalidate();
                update_Drawing(drawingPanel);
             //   زر تحديث الرسم في حال حذف عنصر من مصفوفة الجوار الجوار الاوازان
                button3_Click(send, null);
            }
        }
        // دالة لرسم الروابط بين العقد حسب ادخال المصفوفات الجوار او الاوزان
        void weight_TextBox_TextChanged(object send, EventArgs e)
        {
            if (((TextBox)send).Text != "")
            {
                bool t = false;
                if (Undirction.Checked)
                {
                    add_weight((TextBox)send);
                    Undirction_dirction_adjacencyMatrix_DrawConnections((TextBox)send, drawingPanel, t);
                }
                if (dirction.Checked)
                {
                    Undirction_dirction_adjacencyMatrix_DrawConnections((TextBox)send, drawingPanel, t);

                }
            }
            else
            {
                add_weight((TextBox)send);
                drawingPanel.Invalidate();
                update_Drawing(drawingPanel);
            }
        }
     //   دالة تقوم باضافة حدث الي المصفوفات الجوار والاوزان 
        void addTextBox_TextChanged()
        {
            for (int i = 1; i < nodeCount; i++)
            {
                for (int j = 1; j < nodeCount; j++)
                {
                    adjacencyMatrix[i, j].TextChanged += adjacency_TextBox_TextChanged;
                    if (Unbalance.Checked == false && balence.Checked)
                        weightMatrix[i, j].TextChanged += weight_TextBox_TextChanged;
                }
            }
        }
        /// ////////////////////////////////////////////////////////////////
        bool D = true;// متغير نستخدمه لنحدد البحث من البداية تو من النهاية
        int f = 0; // يرجع فهرس العقدة المتصله باخر عقد تم الظغط عليها اثناء التنفيذ خطوة بخطوة
        int count = 0; //عداد يستخدم اذا لنحدد بداية البحث في المصفوفة الناتجم من الخوارزمية
        //دالة تقوم برجع فهرس العقد
        void get_index_node()
        {
       //  c يمثل عدد النقرات علا الزر تنفيذ خطوة بخطوة بكل نقره يتم استدعاء هذه الدالة
            //الدالة تقوم بالبحث عن العنصر حسب مصفوفة الجوار تبحث عت العقد المتصلة باخر اخر تقوم النقر عليها 
            for (int i = 0; i < c; i++)
            {
                for (int j = 1; j < nodeCount; j++)
                {
                    if (adjacencyMatrix[j, 0].Text == traversalResult[count])
                    {
                        for (int k = 1; k < nodeCount; k++)
                            if (adjacencyMatrix[j, k].Text == "1")
                            {
                                if (adjacencyMatrix[0, k].Text == traversalResult[c - 1])
                                {
                                    f = j;
                                    return;
                                }
                            }
                            else
                                continue;
                    }
                }
                if (D)// اذا كان البحث من النهاية يتم تنقيص العداد خلاف ذلك يتم الزياده
                    count--;
                else
                    count++;
            }
        }
        //  دالة تقوم بالربط بين العقد خطوه خطوه 
        void drawing_panel_stap(int v, int b)
        {
                using (Graphics g = Panel_Steps.CreateGraphics())
                {
                    // Panel_Steps.Size = Drawing_form_3.Size;
                    float dx = nodes2[v].X - nodes2[b].X;
                    float dy = nodes2[v].Y - nodes2[b].Y;
                    float distance = (float)Math.Sqrt(dx * dx + dy * dy);
                    //  Pen e = new Pen(Color.Black, 2);
                    // إيجاد نقاط التقاطع مع حواف الدوائر
                    float x1Edge = nodes2[b].X + dx * 15 / distance;
                    float y1Edge = nodes2[b].Y + dy * 15 / distance;
                    float x2Edge = nodes2[v].X - dx * 15 / distance;
                    float y2Edge = nodes2[v].Y - dy * 15 / distance;
                    if (dirction.Checked)
                    {
                        AdjustableArrowCap ff = new AdjustableArrowCap(5, 5);
                        p.CustomStartCap = ff;
                        g.DrawLine(p, x2Edge, y2Edge, x1Edge, y1Edge);
                    }
                    g.DrawLine(p, x2Edge, y2Edge, x1Edge, y1Edge);
                    // رسم الوزن
                    
                    for (int j = 1; j < nodeCount; j++)
                    {
                        if (nodes2[b].Value == weightMatrix[j, 0].Text)
                            for (int k = j; k < nodeCount; k++)
                            {
                                if (weightMatrix != null && weightMatrix[j, k].Text != "")
                                {
                                    if (nodes2[v].Value == weightMatrix[0, k].Text)
                                    {
                                        Point weightPosition = new Point((nodes2[b].X + nodes2[v].X) / 2, ((nodes2[b].Y + nodes2[v].Y) / 2));
                                        g.DrawString(weightMatrix[j, k].Text,
                                            new Font("Arial", 10),
                                            Brushes.Black,
                                            weightPosition);
                                        break;
                                    }
                                }
                            }
                    }
                }
        }
        //دالة تلوين الحواف
        bool colors = false;
        // دالة تربط بين عقدتين بمساعد الدالة get_index_node
        void connect_node()
        {
            if (f != 0)
            {
                int i = 0;
                for (i = 0; i < nodes.Count; i++)
                {
                    if (traversalResult[c - 1] == nodes[i].Value)
                    {
                        break;
                    }
                }
                    using (Graphics g = Drawing_form_3.CreateGraphics())
                    {
                        Pen e;
                        float dx = nodes[i].X - nodes[f - 1].X;
                        float dy = nodes[i].Y - nodes[f - 1].Y;
                        float distance = (float)Math.Sqrt(dx * dx + dy * dy);
                        if (colors)
                        {
                            e = new Pen(Color.Red, 2);
                        }
                        else
                            e = new Pen(Color.Black, 2);
                        // إيجاد نقاط التقاطع مع حواف الدوائر
                      float     x1Edge = nodes[f - 1].X + dx * 15 / distance;
                      float   y1Edge = nodes[f - 1].Y + dy * 15 / distance;
                     float       x2Edge = nodes[i].X - dx * 15 / distance;
                     float      y2Edge = nodes[i].Y - dy * 15 / distance;
                     
                        if (dirction.Checked)// في حال كان الرب موجة يتم تحويل القلم 
                        {
                            AdjustableArrowCap ff = new AdjustableArrowCap(5, 5);
                            e.CustomStartCap = ff;
                            g.DrawLine(e, x2Edge, y2Edge, x1Edge, y1Edge);
                        }
                        else
                            g.DrawLine(e, x1Edge, y1Edge, x2Edge, y2Edge);
                    
                        if (t) // في حتل كنا نريد نلون العقد يتم تحويل المتغير t الي true باي دالة تريدها
                        {
                            g.FillEllipse(Brushes.Red, nodes[f - 1].X - 15, nodes[f - 1].Y - 15, 30, 30); // رسم الدائرة
                            g.DrawString(nodes[f - 1].Value, this.Font, Brushes.Black, nodes[f - 1].X - 10, nodes[f - 1].Y - 10); // رسم اسم العقدة
                            g.DrawEllipse(p, nodes[f - 1].X - 15, nodes[f - 1].Y - 15, 30, 30); // رسم إطار الدائرة}
                            g.FillEllipse(Brushes.Red, nodes[i].X - 15, nodes[i].Y - 15, 30, 30); // رسم الدائرة
                            g.DrawString(nodes[i].Value, this.Font, Brushes.Black, nodes[i].X - 10, nodes[i].Y - 10); // رسم اسم العقدة
                            g.DrawEllipse(p, nodes[i].X - 15, nodes[i].Y - 15, 30, 30); // رسم إطار الدائرة}
                        }
                    }
                }
            }
        // //  دالة تقوم بالربط بين العقد   
        private void Undirction_dirction_adjacencyMatrix_DrawConnections(TextBox temp, Panel pa, bool t)
        {
            using (Graphics g = pa.CreateGraphics())
            {
                if (dirction.Checked)
                {
                    AdjustableArrowCap f = new AdjustableArrowCap(5, 5);
                    p.CustomStartCap = f;
                }

                for (int i = 1; i < nodeCount; i++)
                {
                    for (int j = 1; j < nodeCount; j++)
                    {
                        if (adjacencyMatrix[i, j].Text == temp.Text || (weightMatrix != null && weightMatrix[i, j].Text == temp.Text)) // إذا كان هناك ارتباط
                        {
                            int b = 0;
                            int v = 0;
                            for (int k = 0; k < nodeCount - 1; k++)
                            {
                                if (nodes[k].Value == adjacencyMatrix[0, j].Text)
                                {
                                    b = k;
                                }
                                if (nodes[k].Value == adjacencyMatrix[i, 0].Text)
                                {

                                    v = k;
                                }
                            }
                            if (b >= 0 && v >= 0)
                            {
                                int counter = 0;
                                if (RadioTree.Checked)
                                {
                                    //تعمل علا عدم تكرار ادخال  اكثر من مربعين في مصفوفة الجوار اثنا الاجتيازات للاشجار
                                    for (int h = 1; h < adjacencyMatrix.GetLength(0); h++)
                                    {
                                        if (adjacencyMatrix[i, h].Text == "1")
                                        {
                                            counter++;
                                        }
                                        if (counter > 2)
                                            adjacencyMatrix[i, h].Text = "";
                                    }
                                }
                                int x;
                                float dx = nodes[v].X - nodes[b].X;
                                float dy = nodes[v].Y - nodes[b].Y;
                                float distance = (float)Math.Sqrt(dx * dx + dy * dy);
                                Pen e = new Pen(Color.Red, 2);
                                // إيجاد نقاط التقاطع مع حواف الدوائر
                                float x1Edge = nodes[b].X + dx * 15 / distance;
                                float y1Edge = nodes[b].Y + dy * 15 / distance;
                                float x2Edge = nodes[v].X - dx * 15 / distance;
                                float y2Edge = nodes[v].Y - dy * 15 / distance;
                                if (t && counter < 3)
                                {
                                    g.DrawLine(p, x1Edge, y1Edge, x2Edge, y2Edge);
                                }
                                    //رسم الوزن
                                else if (!t)
                                {
                                    int midX = ((nodes[b].X + nodes[v].X) / 2);
                                    int midY= ((nodes[b].Y + nodes[v].Y) / 2);
                                    double angle = Math.Atan2(nodes[b].Y - nodes[v].Y, nodes[b].X - nodes[v].X);
                                    int offset = 15;
                                    int wightX = midX;
                                    int wightY = midY;
                                    if (Math.Abs(angle) < 0.01)
                                    {
                                        wightY -= offset+5;
                                        wightX += 5;
                                    }
                                    else if(Math.Abs(angle-Math.PI/2)<0.01|| Math.Abs(angle + Math.PI / 2) < 0.01)
                                    {
                                       
                                        wightX -= offset;
                                        wightY += offset;
                                    }
                                    //else
                                    //{
                                    //    wightX = (int)(midX - (offset-8) * Math.Sin(angle));
                                    //     wightY = (int)(midY + (offset-8) * Math.Cos(angle));
                                    //}
                                    Point  weightPosition = new Point(((nodes[b].X + nodes[v].X) / 2), ((nodes[b].Y + nodes[v].Y) / 2));
                                  //   رسم الوزن

                                    g.DrawString(weightMatrix[i, j].Text,
                                                 new Font("Arial", 13),
                                                 Brushes.Black,
                                                 weightPosition);
                                }
                            }
                        }
                    }
                }
            }
        }
        private void Undirction_CheckedChanged(object sender, EventArgs e)
        {
            if (Undirction.Checked)
            {
                addTextBox_TextChanged();
            }
        }
        private void dirction_CheckedChanged(object sender, EventArgs e)
        {
            if (dirction.Checked)
                addTextBox_TextChanged();
        }
        // دوال الحفظ و الحذف و التعديل المصفوفات الجوار و الاوزان
        int[,] adjacency; // مصفوفة يتم حفظ مصفوفة الجوار فيها اثناء الحفظ
        private void Savee_adjacencyMatrix_Click_1(object sender, EventArgs e)
        {
            color();
            adjacency = new int[nodeCount, nodeCount];

            for (int i = 1; i < nodeCount; i++)
            {
                for (int j = 1; j < nodeCount; j++)
                {
                    if (adjacencyMatrix[i, j].Enabled == true)
                    {
                        adjacencyMatrix[i, j].Enabled = false;
                        adjacency[i - 1, j - 1] = int.Parse(adjacencyMatrix[i, j].Text);
                    }
                    else
                        adjacency[i - 1, j - 1] = 0;
                }
            }
            MessageBox.Show("تم حفظ البيانات بنجاح!");
        }
        private void update_adjacencyMatrix_Click_1(object sender, EventArgs e)
        {
            foreach (TextBox temp in adjacencyPanel.Controls.OfType<TextBox>())
            {
                if (temp.BackColor == Color.WhiteSmoke)
                    continue;
                temp.Enabled = true;
                temp.BackColor = Color.White;
            }
            if (Unbalance.Checked == false)
            {
                foreach (TextBox temp in weightPanel.Controls.OfType<TextBox>())
                {
                    if (temp.Enabled == false && temp.Text == "")
                    {
                        temp.Enabled = true;
                        temp.BackColor = Color.White;
                    }
                }
            }
        }
        private void del_adjacencyMatrix_Click_1(object sender, EventArgs e)
        {
            drawingPanel.Invalidate();
            foreach (TextBox temp in adjacencyPanel.Controls.OfType<TextBox>())
            {
                if (temp.BackColor == Color.WhiteSmoke)
                    temp.Clear();
                else
                {
                    temp.Clear();
                    temp.Enabled = true;
                    temp.BackColor = Color.White;
                }
            }
            if (Unbalance.Checked == false)
            {
                foreach (TextBox temp in weightPanel.Controls.OfType<TextBox>())
                {
                    if (temp.BackColor == Color.WhiteSmoke)
                        temp.Clear();
                    else
                    {
                        temp.Clear();
                        temp.Enabled = true;
                        temp.BackColor = Color.White;
                    }

                }
            }
            foreach (Label temp in drawingPanel.Controls.OfType<Label>())
            {
                temp.Text = null;
            }

        }
        int[,] weights;// لحفظ مصفوفة الاوزان
        private void Savee_weightMatrix_Click_1(object sender, EventArgs e)
        {
            weights = new int[nodeCount, nodeCount];

            for (int i = 1; i < nodeCount; i++)
            {
                for (int j = 1; j < nodeCount; j++)
                {
                    if (weightMatrix[i, j].Enabled == true && weightMatrix[i, j].Text != "")
                    {
                        weightMatrix[i, j].Enabled = false;
                        weights[i - 1, j - 1] = Convert.ToInt32(weightMatrix[i, j].Text);
                    }
                }
            }
            color_weightMatrix_adjacencyMatrix();
            MessageBox.Show("تم حفظ البيانات بنجاح!");
        }
        private void del_weightMatrix_Click(object sender, EventArgs e)
        {
            Graphics t = adjacencyPanel.CreateGraphics();
            if (Unbalance.Checked == false)
            {
                foreach (TextBox temp in weightPanel.Controls.OfType<TextBox>())
                {
                    if (temp.BackColor == Color.White && temp.Text != "")
                    {
                        temp.Clear();
                    }
                }
            }
        }
        //دوال قائمة الختيارت
        private void RadioGraphe_CheckedChanged(object sender, EventArgs e)
        {
            if (RadioGraphe.Checked)
            {
                i = 1;
                nodes.Clear();
                panel2.Enabled = true;
                panel3.Enabled = true;
                comboBox1.Items.Clear();
                comboBox3.Items.Clear();
                comboBox2.Items.Clear();
                comboBox1.Items.Add("اجتياز الرسم البياني");
                comboBox1.Items.Add("المسارالأقصر");
            }
            Nodes_Count.Enabled = true;
            nodenametextbox.Enabled = true;
        }
        private void RadioTree_CheckedChanged(object sender, EventArgs e)
        {
            if (RadioTree.Checked)
            {
                j = 0;
                nodes.Clear();
                panel2.Enabled = false;
                panel3.Enabled = false;
                comboBox1.Items.Clear();
                comboBox3.Items.Clear();
                comboBox2.Items.Clear();
                comboBox1.Items.Add("اجتياز الشجرة");
                comboBox1.Items.Add("ارتفاع الشجرة");

            }
            Nodes_Count.Enabled = true;
            nodenametextbox.Enabled = true;
        }

        private void bbb_Click(object sender, EventArgs e)
        {
            if (Unbalance.Checked == false)
            {
                foreach (TextBox temp in weightPanel.Controls.OfType<TextBox>())
                {
                    if (temp.Enabled == false && temp.BackColor == Color.White)
                    {
                        temp.Enabled = true;
                    }
                }
            }
        }
        private void button9_Click(object sender, EventArgs e)
        {
            weightPanel.Controls.Clear();
        }
        //زر النقل الى الفورم 3
        private void button8_Click(object sender, EventArgs e)
        {

            panel5.Visible = true;
            button1.Visible = true;
            button2.Visible = true;
            button10.Visible = false;
            button9.Visible = false;
            button3.Visible = false;

            //تجميد الازارا عند النقل الي الفورم الثالث
            panel6.Enabled = false;
       
            if (RadioTree.Checked)
            {
                label13.Visible = true;
                Drawing_form_3.Width = 415;
                Drawing_form_3.Paint += DrawingPanel_Paint;
                update_Drawing(Drawing_form_3);
                c = 0;
            }
            else if (RadioGraphe.Checked)
            {
                Drawing_form_3.Width = 315;
                Drawing_form_3.Paint += DrawingPanel_Paint;
                update_Drawing(Drawing_form_3);
            }
        }
        //دالة تحديث الروبط بين العقد الى الفورم 3
        void update_Drawing(Panel panel)
        {
            bool t = true;
            for (int i = 1; i < nodeCount; i++)
            {
                for (int j = 1; j < nodeCount; j++)
                {
                    t = true;
                    if (adjacencyMatrix[i, j].Text == "1")
                    {
                        Undirction_dirction_adjacencyMatrix_DrawConnections(adjacencyMatrix[i, j], panel, t);
                        if (Unbalance.Checked == false && balence.Checked)
                        {
                            t = false;
                            Undirction_dirction_adjacencyMatrix_DrawConnections(weightMatrix[i, j], panel, t);
                        }
                    }
                }
            }
        }
        //زر الرجوع الى الفرم 2
        private void button1_Click(object sender, EventArgs e)
        {
            c = 0;
            label13.Text = "-->";
        //   اخفاء panel الفورم الثالث وتفعيل panel الفورم الثاني
            panel6.Enabled = true;
            panel5.Visible = false;
            button1.Visible = false;
            button2.Visible = false;
            button10.Visible = true;
            button9.Visible = true;
            button3.Visible = true;
            label13.Visible = false;
                update_Drawing(drawingPanel);
            Panel_Steps.Invalidate();
            //تصفير العقد في حال اتنقل بين الفورمات
            nodes2.Clear();
            //تصفير المصفوفة في حال رجعت من الفورم الثالث الي الثاني
            traversalResult.Clear();
            //تصفير المتغير الذي نحصل من خلاله علي موقع الاندكس في مصفوفة الرئيسية
            f = 0;
            //
            count = 0;
            D = true;
            t = false;
            y2 = new string[200];
        }
        //زر تنفيذ خطوه بخطوه
        private void button2_Click(object sender, EventArgs e)
        {

            if (RadioTree.Checked)
                BinarryTreee();
            else if (RadioGraphe.Checked)
            {
                Graphace();  
            }
        }
        //دوال تنفيذ الخوارزميات
        public void BinarryTreee()
        {
            if (comboBox1.Text == "اجتياز الشجرة" || comboBox1.Text == "ارتفاع الشجرة")
            {
                if (comboBox3.Text == "اجتياز الشجرة الثنائية")
                {
                    if (c == 0)
                    {
                     
                        bool[] vis = new bool[nodeCount];
                        switch (comboBox2.Text)
                        {

                            case "اجتياز بالترتيب السابق":
                                PreOrderTraversal( vis, 0); // بدء اجتياز ما قبل الترتيب
                                break;
                            case "اجتياز بالترتيب الداخل":

                                InOrderTraversal(0); // بدء اجتياز ما قبل الترتيب
                              
                                break;
                            case "اجتياز بالترتيب اللاحق":
                                PostOrderTraversal( vis, 0); // بدء اجتياز ما قبل الترتيب
                                break;
                        }
                        label13.Text += comboBox2.Text + " ";
                    }
                    if (c < traversalResult.Count)
                    {
                        label13.Text +=  traversalResult[c].ToUpper() + " ";
                        c++;
                        nodes2.Add(new TreeNode("1")); // إضافة عقدة جديدة
                       Panel_Steps.Invalidate(); // إعادة رسم Panel
                    }
                }
                else if (comboBox3.Text == "ايجاد الرتفاع الشجرة" && comboBox2.Text == "خوارزمية ايجاد الرتفاع الشجرة")
                {
                    count = 0;
                    if (c == 0)
                    
                        GetHeight(root);
                    
                    if (c < traversalResult.Count)
                    {
                        label13.Text +=comboBox2.Text+ " "+ traversalResult[c].ToUpper() + " ";
                        c++;
                        nodes2.Add(new TreeNode("1")); // إضافة عقدة جديدة
                        Panel_Steps.Invalidate(); // إعادة رسم Panel
                    }
                    else
                    {
                        button2.Enabled = false;
                        MessageBox.Show("ارتفاع الشجره هو : " + traversalResult.Count);
                    }
                }
            }
            else
                MessageBox.Show("حدد خيار ");
        }
        public void Graphace()
        {
            if (comboBox1.Text == "اجتياز الرسم البياني")
            {
                label13.Visible = true;
                if (comboBox3.Text == "البحث العرض اولا(BFS)")
                {
                    D = false;
                    count = 0;
                    if (comboBox2.Text == " الخوارزمية الأولى (BFS)")
                    {
                        colors = true;

                        if (c == 0)
                        {
                            BFS(nodes[0].Value);
                            label13.Text += comboBox2.Text + " " + traversalResult[c].ToUpper() + " ";
                            c++;
                            get_index_node();
                            connect_node();
                            nodes2.Add(new TreeNode("1"));
                            //   إضافة عقدة جديدة
                            Panel_Steps.Invalidate();

                            // إعادة رسم Panel
                        }

                        else if (c < traversalResult.Count)
                        {

                            label13.Text += traversalResult[c].ToUpper() + " ";
                            c++;
                            get_index_node();
                            connect_node();
                            nodes2.Add(new TreeNode("1"));


                            // إضافة عقدة جديدة
                            Panel_Steps.Invalidate();    // إعادة رسم Panel
                        }
                    }
                    else if (comboBox2.Text == "خوارزمية التلوين (BFS)")
                    {
                        colors = false;
                        t = true;
                        if (c == 0)
                        {
                            BFS(nodes[0].Value);
                            label13.Text += comboBox2.Text + " " + traversalResult[c].ToUpper() + " ";
                            c++;
                            get_index_node();
                            connect_node();
                            nodes2.Add(new TreeNode("1"));
                            Panel_Steps.Invalidate();
                        }
                        else if (c < traversalResult.Count)
                        {

                            label13.Text += traversalResult[c].ToUpper() + " ";
                            c++;

                            get_index_node();
                            connect_node();
                            nodes2.Add(new TreeNode("1"));
                            // إضافة عقدة جديدة
                            Panel_Steps.Invalidate();    // إعادة رسم Panel
                        }
                    }
                }
                else if (comboBox3.Text == "بحث العمق اولا(DFS)")
                {
                    D = true;
                    if (comboBox2.Text == "الخوارزمية الأولى (DFS)")
                    {
                        colors = true;
                        t = false;
                        if (c == 0)
                        {
                            DFS();
                            label13.Text += comboBox2.Text + " ";
                        }

                        if (c < traversalResult.Count)
                        {
                            label13.Text += traversalResult[c].ToUpper() + " ";
                            c++;
                            count = c - 1;

                            get_index_node();
                            connect_node();
                            nodes2.Add(new TreeNode("1"));
                            // إضافة عقدة جديدة
                            Panel_Steps.Invalidate();    // إعادة رسم Panel
                        }
                    }
                    else if (comboBox2.Text == "خوارزمية التلوين (DFS)")
                    {

                        colors = false;
                        t = true;
                        count = 0;
                        if (c == 0)
                        {
                            DFS();
                            label13.Text += comboBox2.Text + " ";
                        }


                        if (c < traversalResult.Count)
                        {

                            label13.Text += traversalResult[c].ToUpper() + " ";
                            c++;
                            count = c - 1;
                            get_index_node();
                            connect_node();
                            nodes2.Add(new TreeNode("1"));
                            // إضافة عقدة جديدة
                            Panel_Steps.Invalidate();    // إعادة رسم Panel
                        }
                    }
                }
                else if (comboBox3.Text == "الاجتياز الأقل كلفة للشجرة")
                {
                    if (comboBox2.Text == "خوارزمية بريم الاساسية")
                    {
                        D = true;
                        colors = true;
                        if (c == 0)
                        {
                            RunPrim();
                            label13.Text += " " + traversalResult[c].ToUpper() + "->" + path[c] + "->";
                            c++;
                            get_index_node();
                            connect_node();
                            nodes2.Add(new TreeNode("1"));
                            // إضافة عقدة جديدة
                            //  Panel_Steps.Invalidate();
                            // إعادة رسم Panel
                        }
                        count = c - 1;
                        if (c < traversalResult.Count)
                        {
                            label13.Text += traversalResult[c].ToUpper() + "->" + path[c] + "->";
                            c++;
                            get_index_node();
                            connect_node();
                            nodes2.Add(new TreeNode("1"));
                            // إضافة عقدة جديدة
                            //  Panel_Steps.Invalidate();
                            // إعادة رسم Panel
                        }
                    }
                    else if (comboBox2.Text == "خوارزمية بريم المتقدمة")
                    {
                        D = false;
                        colors = true;
                        if (c == 0)
                        {
                            RunPrimAdvanced();
                            label13.Text +="path-->"+ traversalResult[c]+" ";
                                c++;
                               get_index_node();
                              connect_node();
                              nodes2.Add(new TreeNode("1"));
                            // إضافة عقدة جديدة
                            // Panel_Steps.Invalidate();
                            // إعادة رسم Panel
                        }
                        count = c - 1;
                        if (c < traversalResult.Count)
                        {
                            label13.Text += traversalResult[c] + " ";
                            c++;
                              get_index_node();
                               connect_node();
                              nodes2.Add(new TreeNode("1"));
                            // إضافة عقدة جديدة
                            //   Panel_Steps.Invalidate();
                            // إعادة رسم Panel
                        }
                    }
                }
                else if (comboBox2.Text == "خوارزمية كروسكال الاساسية")
                {

                    var mst = KruskalAlgorithm();
                    DrawGraph(mst);


                }
            }
            if (comboBox1.Text == "المسارالأقصر")
            {
                if (comboBox3.Text == "إيجاد المسار الاقصر")
                {
                    if (comboBox2.Text == "خوارزمية بليمان")
                    {
                        D = false;

                        colors = true;

                        if (c == 0)
                        {

                            if (textstart.Text != "" && textend.Text != "")
                                RunLehman(textstart.Text, textend.Text);

                            label13.Text += comboBox2.Text + " " + traversalResult[c].ToUpper() + " ";
                            // get_index_node();
                            // connect_node();
                            c++;
                            get_index_node();
                            connect_node();
                            nodes2.Add(new TreeNode("1"));
                            // إضافة عقدة جديدة
                            Panel_Steps.Invalidate();
                            // إعادة رسم Panel

                        }
                        count = c - 1;

                        if (c < traversalResult.Count)
                        {

                            label13.Text += traversalResult[c].ToUpper() + " ";
                            c++;
                            get_index_node();
                            connect_node();


                            nodes2.Add(new TreeNode("1"));
                            // إضافة عقدة جديدة
                            Panel_Steps.Invalidate();
                            // إعادة رسم Panel
                        }

                    }
                    else if (comboBox2.Text == "خوارزمية ديجسترا")
                    {
                        D = false;
                        colors = true;
                        if (c == 0)
                        {
                            if (textstart.Text != "" && textend.Text != "")
                                RunDijkstra(textstart.Text, textend.Text);
                            label13.Text += comboBox2.Text + " " + traversalResult[c].ToUpper() + " ";
                            c++;
                            get_index_node();
                            connect_node();
                            nodes2.Add(new TreeNode("1"));
                            // إضافة عقدة جديدة
                            Panel_Steps.Invalidate();
                            // إعادة رسم Panel

                        }
                        count = c - 1;
                        if (c < traversalResult.Count)
                        {
                            label13.Text += traversalResult[c].ToUpper() + " ";
                            c++;
                            get_index_node();
                            connect_node();
                            nodes2.Add(new TreeNode("1"));
                            // إضافة عقدة جديدة
                            Panel_Steps.Invalidate();
                            // إعادة رسم Panel
                        }
                    }
                    else if (comboBox2.Text == "خوارزمية القوة الغاشمة")
                    {
                        D = false;
                        colors = true;
                        if (c == 0)
                        {

                            if (textstart.Text != "" && textend.Text != "")
                            {
                                int start = 0, end = 0;
                                for (int i = 0; i < nodes.Count; i++)
                                {
                                    if (nodes[i].Value == textend.Text)
                                    { end = i; }
                                    if (nodes[i].Value == textstart.Text)
                                        start = i;
                                }

                                FindShortestPath(start, end, 0);
                            }

                            label13.Text += comboBox2.Text + " " + traversalResult[c].ToUpper() + " ";
                            c++;
                            get_index_node();
                            connect_node();
                            nodes2.Add(new TreeNode("1"));
                            // إضافة عقدة جديدة
                            Panel_Steps.Invalidate();
                            // إعادة رسم Panel

                        }
                        count = c - 1;
                        if (c < traversalResult.Count)
                        {
                            label13.Text += traversalResult[c].ToUpper() + " ";
                            c++;
                            get_index_node();
                            connect_node();
                            nodes2.Add(new TreeNode("1"));
                            // إضافة عقدة جديدة
                            Panel_Steps.Invalidate();
                            // إعادة رسم Panel
                        }
                    }
                    else if (comboBox2.Text == "خوارزمية فلويد")
                    {

                        D = false;
                        colors = true;
                     
                        if (c == 0)
                        {
                       if (textstart.Text != "" && textend.Text != "")
                       {
                           int start = 0, end = 0;
                           for (int i = 0; i < nodes.Count; i++)
                           {
                               if (nodes[i].Value == textend.Text)
                               { end = i; }
                               if (nodes[i].Value == textstart.Text)
                                   start = i;
                           }
                           RunFloydWarshall(start,end);
                       }
                           label13.Text +=" " + traversalResult[c].ToUpper() + " -> ";
                            c++;
                            get_index_node();
                            connect_node();
                            nodes2.Add(new TreeNode("1"));
                            // إضافة عقدة جديدة
                          //  Panel_Steps.Invalidate();
                            // إعادة رسم Panel

                        }
                        count = c - 1;
                        if (c < traversalResult.Count)
                        {
                            label13.Text += "  " + traversalResult[c].ToUpper() + " -> ";
                            c++;
                            get_index_node();
                            connect_node();
                            nodes2.Add(new TreeNode("1"));
                            // إضافة عقدة جديدة
                          //  Panel_Steps.Invalidate();
                            // إعادة رسم Panel
                        }
                        if (c == traversalResult.Count)
                            label13.Text +=shortestPath+"الوزن = " ;
                    }
                }
            }
        }
        // كود خوارزمية اجتياز الرسم البياني بحث العمق اولا
        HashSet<int> visited = new HashSet<int>();
        public void DFS()
        {
            bool[] v = new bool[nodeCount];
            DepthFirstSearch(adjacency, 0, v);

        }
        private void DepthFirstSearch(int[,] adjacencyMatrix, int startNode, bool[] v)
        {
            // تعليم العقدة الحالية كأنها مزارة
            v[startNode] = true;
            traversalResult.Add(nodes[startNode].Value);

            // المرور على كل الجيران
            int i = 0;
            for ( i = 0; i < adjacencyMatrix.GetLength(0); i++)
            {
                // إذا كانت العقدة جارًا ولم يتم زيارتها
                if (adjacencyMatrix[startNode, i] == 1 && !v[i])
                {
                    DepthFirstSearch(adjacencyMatrix, i, v);
                }
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }
        //////////////////////////////////////////////////////////////////////////
        public void BFS(string startNode)
        {
            HashSet<string> visited = new HashSet<string>();
            // bool[] visited = new bool[newCount];// العقد التي تتم زيارتها
            Queue<string> queue = new Queue<string>();//طابور لتخزين العقد التي سيتم زيارتها
            //  List<int> sList = new List<int>();
            string temp;
            int j = 1;
            visited.Add(startNode);
            queue.Enqueue(startNode);

            while (queue.Count != 0)
            {
                temp = queue.Dequeue();//ازالة العقد من الطابور
                traversalResult.Add(temp);
                for (int i = 0; i < adjacencyMatrix.GetLength(0) - 1; i++)//زيارة جميع العقد الجيران للعقد
                {
                    string value = adjacencyMatrix[j, i + 1].Text.Trim();
                    string n = nodes[i].Value;
                    if (value == "1" && !visited.Contains(n))//اذا لم يتم زيارة الجار من قبل
                    {
                        visited.Add(n);//اضافه الي مجموعات الزيارات
                        queue.Enqueue(n);//اضافة الي الطابور ليتم زيارته لاحقا
                    }
                }
                j++;
            }
        }
        // زر تحديث الرسم
        private void button3_Click(object sender, EventArgs e)
        {
            drawingPanel.Paint += DrawingPanel_Paint;
            update_Drawing(drawingPanel);
        }
        void RunLehman(string startVertex, string endVertex)
        {
            int n = adjacencyMatrix.GetLength(0) - 1; // عدد الرؤوس
            string[] vertices = new string[n];

            // استخلاص أسماء الرؤوس
            for (int i = 0; i < n; i++)
            {
                vertices[i] = adjacencyMatrix[0, i + 1].Text; // أسماء الرؤوس في الصف الأول
            }

            // إيجاد إندكسات الرؤوس المطلوبة
            int startIndex = Array.IndexOf(vertices, startVertex);
            int endIndex = Array.IndexOf(vertices, endVertex);

            if (startIndex == -1 || endIndex == -1)
            {
                throw new ArgumentException("الرأس الابتدائي أو النهائي غير موجود في المصفوفة.");
            }

            // المتغيرات اللازمة
            int[] distances = new int[n]; // لتخزين المسافات
            bool[] visited = new bool[n]; // لتتبع الرؤوس المُزارَة
            int[] previous = new int[n];  // لتتبع المسار

            // تعيين القيم الافتراضية
            for (int i = 0; i < distances.Length; i++)
            {
                distances[i] = int.MaxValue;
            }
            for (int i = 0; i < previous.Length; i++)
            {
                previous[i] = -1;
            }

            distances[startIndex] = 0; // المسافة إلى البداية هي 0

            // تطبيق خوارزمية ليمان
            for (int i = 0; i < n - 1; i++) // تكرار العملية لجميع الرؤوس (باستثناء الرأس النهائي)
            {
                int u = -1;
                for (int j = 0; j < n; j++)
                {
                    if (!visited[j] && (u == -1 || distances[j] < distances[u]))
                    {
                        u = j;
                    }
                }

                if (distances[u] == int.MaxValue)
                {
                    break; // لا يمكن الوصول إلى باقي الرؤوس
                }

                visited[u] = true; // قم بزيارة الرأس

                // تحديث المسافات إلى الجيران
                for (int v = 0; v < n; v++)
                {
                    if (!visited[v] && adjacencyMatrix[u + 1, v + 1].Text == "1") // إذا كان هناك اتصال
                    {
                        int weight = int.Parse(weightMatrix[u + 1, v + 1].Text); // الوزن من مصفوفة الأوزان
                        if (distances[u] + weight < distances[v])
                        {
                            distances[v] = distances[u] + weight; // تحديث المسافة
                            previous[v] = u; // تحديث الرأس السابق
                        }
                    }
                }
            }

            // بناء المسار النهائي من البداية إلى النهاية

            int current = endIndex;
            while (current != -1)
            {
                traversalResult.Add(vertices[current]);
                //  MessageBox.Show(vertices[current]);
                current = previous[current];
            }

            // تحقق مما إذا كان هناك مسار
            if (distances[endIndex] == int.MaxValue)
            {
                throw new Exception("لا يوجد مسار بين الرأسين المحددين.");
            }

            traversalResult.Reverse(); // عكس المسار بعد بناءه ليظهر من البداية إلى النهاية

        }
        string[] path = new string[10];
        private void textstart_TextChanged(object sender, EventArgs e)
        {
            int b = 0;
            for (int i = 1; i < nodeCount; i++)
            {
                if (adjacencyMatrix[0, i].Text == ((TextBox)sender).Text)
                {
                    b = 0;
                    break;
                }
                else
                    b = 1;
            }
            if (b == 1)
                ((TextBox)sender).Text = "";
        }
        private void RunPrim()
        {
            // عدد الرؤوس
            int n = adjacencyMatrix.GetLength(0) - 1;
            string[] vertices = new string[n];

            //// استخلاص أسماء الرؤوس (من المصفوفة الأولى في الأعمدة)
            for (int i = 0; i < n; i++)
            {
                vertices[i] = adjacencyMatrix[0, i + 1].Text; // الاسم موجود في الصف الأول
            }

            bool[] visited = new bool[n]; // مصفوفة لتتبع الرؤوس التي تمت زيارتها
            List<string> mst = new List<string>(); // قائمة لتخزين الشجرة الممتدة
            visited[0] = true; // ابدأ من الرأس الأول

            traversalResult.Add(vertices[0]);

            // تنفيذ خوارزمية Prim
            for (int k = 1; k < n; k++)
            {
                int minWeight = int.MaxValue;
                int from = -1, to = -1;

                for (int i = 0; i < n; i++)
                {
                    if (visited[i])
                    {
                        for (int j = 0; j < n; j++)
                        {
                            // تحقق من الجوار (الخلية تحتوي على "1" تعني وجود ارتباط)
                            if (!visited[j] && adjacencyMatrix[i + 1, j + 1].Text == "1")
                            {
                                // الحصول على الوزن (من مربع النص)
                                int weight = int.Parse(weightMatrix[i + 1, j + 1].Text);
                                if (weight < minWeight)
                                {
                                    minWeight = weight;
                                    from = i;
                                    to = j;
                                }
                            }
                        }
                    }
                }
                if (to != -1)
                {
                    visited[to] = true;
                    path[k-1] = minWeight.ToString();
                    MessageBox.Show(minWeight.ToString());
                    traversalResult.Add(vertices[to]);

                }
            }
        }
        // دالة لتنفيذ خوارزمية بريم المتقدمة
        public void RunPrimAdvanced()
        {
            int n = adjacencyMatrix.GetLength(0) - 1; // عدد الرؤوس
            string[] vertices = new string[n];

            // استخلاص أسماء الرؤوس
            for (int i = 0; i < n; i++)
            {
                vertices[i] = adjacencyMatrix[0, i + 1].Text; // أسماء الرؤوس في الصف الأول
            }

            // قائمة لتمثيل MST والنتيجة النهائية
            List<string> mst = new List<string>();
            traversalResult.Add(vertices[0]); // ابدأ من الرأس الأول

            // متغير لتتبع الرؤوس التي تمت زيارتها
            bool[] visited = new bool[n];
            visited[0] = true; // تم زيارة الرأس الأول

            // قائمة للحواف
            List<GraphEdge> edgeList = new List<GraphEdge>();

            // أضف جميع الحواف التي تربط الرأس الأول بالرؤوس الأخرى
            for (int j = 1; j < n; j++)
            {
                if (adjacencyMatrix[1, j + 1].Text == "1") // تحقق من وجود اتصال
                {
                    int weight = int.Parse(weightMatrix[1, j + 1].Text);
                    edgeList.Add(new GraphEdge(weight, 0, j));
                }
            }
            // بناء الشجرة
            while (mst.Count < n && edgeList.Count > 0)
            {
                // ترتيب الحواف يدويًا حسب الوزن من الأقل إلى الأعلى
                edgeList.Sort((a, b) => a.Weight.CompareTo(b.Weight));

                // استخراج الحافة ذات الوزن الأقل
                var edge = edgeList[0];
                edgeList.RemoveAt(0); // إزالة الحافة بعد استخدامها

                int from = edge.From;
                int to = edge.To;

                if (visited[to]) continue; // تخطي إذا كان الرأس الآخر قد زار مسبقًا

                // أضف الرأس الجديد إلى MST
                visited[to] = true;
                traversalResult.Add(vertices[to]);
                // أضف الحواف الجديدة للرأس الجديد
                for (int j = 0; j < n; j++)
                {
                    if (!visited[j] && adjacencyMatrix[to + 1, j + 1].Text == "1")
                    {
                        int weight = int.Parse(weightMatrix[to + 1, j + 1].Text);
                        edgeList.Add(new GraphEdge(weight, to, j));
                    }
                }
            }
        }
        // كلاس GraphEdge يمثل الحافة بين الرأسين
        public class GraphEdge
        {
            public int Weight { get; set; }  // الوزن
            public int From { get; set; }    // من الرأس
            public int To { get; set; }      // إلى الرأس

            public GraphEdge(int weight, int fromVertex, int toVertex)
            {
                Weight = weight;
                From = fromVertex;
                To = toVertex;
            }
        }
        int MinKey(int[] key, bool[] mstSet)
        {
            int min = int.MaxValue;
            int minIndex = -1;
            for (int v = 0; v < nodeCount; v++)
            {
                if (mstSet[v] == false && key[v] < min)
                {
                    min = key[v];
                    minIndex = v;
                }
            }
            return minIndex;
        }
         bool[] visitedd=new bool[10];
         int shortestPath = int.MaxValue;
        int [,] paths=new int[10,10];
        void FindShortestPath(int currentNode, int targetNode, int currentCost)
        {
            if (currentNode == targetNode)
            {
                shortestPath = Math.Min(shortestPath, currentCost);
                return;
            }

            visitedd[currentNode] = true;
            traversalResult.Add(nodes[currentNode].Value);
            for (int i = 1; i < nodeCount; i++)
            {
                if (weights[currentNode, i] > 0 && !visitedd[i])
                {
                    FindShortestPath(i, targetNode, currentCost++);
                }
            }
            visitedd[currentNode] = false;
        }
        private void RunFloydWarshall(int start, int end)
        {
            // عدد الرؤوس
            int n = adjacencyMatrix.GetLength(0) - 1;
            // استخلاص أسماء الرؤوس
            string[] vertices = new string[n];
            for (int i = 0; i < n; i++)
            {
                vertices[i] = adjacencyMatrix[0, i + 1].Text;
            }
            // مصفوفة المسافات
            int[,] dist = new int[n, n];
            // تهيئة مصفوفة المسافات
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i == j)
                    {
                        dist[i, j] = 0; paths[i, j] = -1;
                    }
                    else if (adjacencyMatrix[i + 1, j + 1].Text == "1")
                    {
                        dist[i, j] = int.Parse(weightMatrix[i + 1, j + 1].Text); paths[i, j] = i;
                    }
                    else
                    {
                        dist[i, j] = int.MaxValue; paths[i, j] = -1;
                    }
                }
            }
            // تطبيق خوارزمية فلويد-Warshall
            for (int k = 0; k < n; k++)
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (dist[i, j] > dist[i, k] + dist[k, j] && dist[i, k] != int.MaxValue && dist[k, j] != int.MaxValue)
                        {
                            dist[i, j] = dist[i, k] + dist[k, j];
                            paths[i, j] = paths[k, j];
                        }
                    }
                }
            }
            // بناء النتيجة النهائية بشكل مفصول
            List<string> result = new List<string>();
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (dist[i, j] != int.MaxValue && i != j)
                    {
                        //هنا المسارات التي تنتجها الخوارزميها
                        // traversalResult.Add(vertices[i]+"-->"+vertices[j]+" weight--> " +dist[i,j]);
                        // traversalResult.Add(vertices[j]);
                    }
                }
            }

            GetShortestPath(paths, dist, start, end);
            traversalResult.Add(vertices[start]);
            for (int i = 1; i < fullpath.Count; i++)
            {
                traversalResult.Add(nodes[fullpath[i]].Value);
            }

            shortestPath = dist[start, end];
  
        }
        List<int>fullpath;
        void GetShortestPath(int[,] path2, int[,] dist, int start, int end)
        {
            if (dist[start, end] == int.MaxValue)
                return;// "لا يوجد مسار بين النقطتين";
            fullpath=new List<int>();
            buldpath(path2,start,end,fullpath);
        }
        //دالة مساعدة لخوارزمية فلويد
        void buldpath(int[,] p, int start, int end, List<int> fullpath)
        {
            if (start == end)
            {
                fullpath.Add(start + 1);
                return;
            }
            if (p[start, end] == -1){
                fullpath.Add(end);
            return;
        }
            buldpath(p, start, p[start, end], fullpath);
            fullpath.Add(end);
        }
        // تعريف فئة Edge لتمثيل الأضلاع في الرسم البياني
        private class Edge : IComparable<Edge>
        {
            // خصائص الضلع: العقدة البداية والنهاية والوزن
            public int From { get; set; }
            public int To { get; set; }
            public int Weight { get; set; }
            // مقارنة الأضلاع بناءً على الوزن (للفرز)
            public int CompareTo(Edge other)
            {
                return Weight.CompareTo(other.Weight);
            }
        }
        // خوارزمية كروسكال لتحديد الشجرة الممتدة الصغرى (MST)
        private List<Edge> KruskalAlgorithm()
        {
          var   edges = new List<Edge>();

            // تحويل مصفوفات الجوار والأوزان إلى قائمة من الأضلاع
            for (int i = 0; i < nodeCount; i++)
            {
                for (int j = i + 1; j < nodeCount; j++) // التأكد من عدم تكرار الأضلاع
                {
                    if (adjacency[i, j] == 1) // التحقق من وجود ضلع بين العقدتين
                    {
                        edges.Add(new Edge { From = i, To = j, Weight = weights[i, j] });
                    }
                }
            }

            // فرز الأضلاع حسب الوزن (تصاعديًا)
            edges.Sort();

            // قائمة لحفظ أضلاع الشجرة الممتدة الصغرى
            var mst = new List<Edge>();

            // مصفوفة لتتبع المجموعات (Union-Find)
            int[] parent = Enumerable.Range(0, nodeCount).ToArray();

            // المرور على جميع الأضلاع
            foreach (var edge in edges)
            {
                // التحقق من أن العقدتين ليستا في نفس المجموعة
                if (Find(parent, edge.From) != Find(parent, edge.To))
                {
                   // traversalResult.Add(edge.ToString());
                  //  MessageBox.Show(edge.ToString());
                     mst.Add(edge); // إضافة الضلع إلى MST
                    Union(parent, edge.From, edge.To); // دمج المجموعتين
                }
            }
            // إرجاع قائمة الأضلاع التي تشكل MST
             return mst;
        }
        // دالة Find لتحديد المجموعة التي تنتمي إليها العقدة (مع ضغط المسار)
        private int Find(int[] parent, int node)
        {
            if (parent[node] == node)
                return node;
            return parent[node] = Find(parent, parent[node]);
        }
        // دالة Union لدمج مجموعتين
        private void Union(int[] parent, int u, int v)
        {
            parent[Find(parent, u)] = Find(parent, v);
        }
        // كورسكال دالة لرسم العقد والأضلاع في لوحة الرسم
        private void DrawGraph(List<Edge> mst)
            {
                // مسح محتويات اللوحة
               // drawingPanel.Controls.Clear();
                // إعداد الرسومات
                Graphics g = Panel_Steps.CreateGraphics();
                Pen pen = new Pen(Color.Green, 2);

                // تحديد مواقع العقد عشوائيًا داخل اللوحة
                Random random = new Random();
                Point[] points = new Point[nodeCount];
                for (int i = 0; i < nodeCount; i++)
                {

                points[i] = new Point(random.Next(50, Panel_Steps.Width - 50), random.Next(50, Panel_Steps.Height - 50));

                    // رسم العقدة كدائرة صغيرة
                    g.FillEllipse(Brushes.Red, points[i].X - 10, points[i].Y - 10, 20, 20);

                    // تسمية العقدة
                    g.DrawString("Node {i + 1}", new Font("Arial", 10), Brushes.Black, points[i].X - 15, points[i].Y - 25);
                }

                // رسم الأضلاع بناءً على MST
                foreach (var edge in mst)
                {
                    // رسم الخط الذي يمثل الضلع
                    g.DrawLine(pen, points[edge.From], points[edge.To]);

                    // كتابة الوزن على منتصف الضلع
                    g.DrawString(edge.Weight.ToString(), new Font("Arial", 10), Brushes.Blue,
                        (points[edge.From].X + points[edge.To].X) / 2,
                        (points[edge.From].Y + points[edge.To].Y) / 2);

                //}
            }
        }
        //خوارزمية ديجسترا
        private int RunDijkstra(string startVertex, string endVertex)
        {
            // عدد الرؤوس
            int n = adjacencyMatrix.GetLength(0) - 1;

            // استخلاص أسماء الرؤوس
            string[] vertices = new string[n];
            for (int i = 0; i < n; i++)
            {
                vertices[i] = adjacencyMatrix[0, i + 1].Text;
            }

            // إيجاد إندكسات الرؤوس المطلوبة
            int startIndex = Array.IndexOf(vertices, startVertex);
            int endIndex = Array.IndexOf(vertices, endVertex);

            if (startIndex == -1 || endIndex == -1)
            {
                throw new ArgumentException("الرأس الابتدائي أو النهائي غير موجود في المصفوفة.");
            }

            // المتغيرات اللازمة
            int[] distances = new int[n]; // لتخزين المسافات
            bool[] visited = new bool[n]; // لتتبع الرؤوس المزارة
            int[] previous = new int[n]; // لتتبع المسار

            // تعيين القيم الافتراضية
            for (int i = 0; i < distances.Length; i++)
            {
                distances[i] = int.MaxValue;
                previous[i] = -1;
            }

            // المسافة إلى البداية
            distances[startIndex] = 0;

            // الخوارزمية
            for (int i = 0; i < n; i++)
            {
                // إيجاد الرأس غير المزور ذو المسافة الأقل
                int u = -1;
                for (int j = 0; j < n; j++)
                {
                    if (!visited[j] && (u == -1 || distances[j] < distances[u]))
                    {
                        u = j;
                    }
                }

                // إذا كانت المسافة لا نهائية، لا يمكن الوصول إلى باقي الرؤوس
                if (distances[u] == int.MaxValue)
                {
                    break;
                }

                // قم بزيارة الرأس
                visited[u] = true;

                // تحديث المسافات إلى الجيران
                for (int v = 0; v < n; v++)
                {
                    if (!visited[v] && adjacencyMatrix[u + 1, v + 1].Text == "1")
                    {
                        int weight = int.Parse(weightMatrix[u + 1, v + 1].Text);
                        if (distances[u] + weight < distances[v])
                        {
                            // تحديث المسافة
                            distances[v] = distances[u] + weight;

                            // تحديث الرأس السابق
                            previous[v] = u;
                        }
                    }
                }
            }
            for (int at = endIndex; at != -1; at = previous[at])
            {
                traversalResult.Add(vertices[at]);
                MessageBox.Show(vertices[at]);
            }

            // عكس المسار لأنه تم تتبعه بالعكس
           
            traversalResult.Reverse();

            // تحقق مما إذا كان هناك مسار
            if (distances[endIndex] == int.MaxValue)
            {
                throw new Exception("لا يوجد مسار بين الرأسين المحددين.");
            }
            return 0;
        }
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.Text == "بحث العمق اولا(DFS)")
            {
                // comboBox3.Items.Clear();
                comboBox2.Items.Clear();
                comboBox2.Items.Add("الخوارزمية الأولى (DFS)");
                comboBox2.Items.Add("خوارزمية التلوين (DFS)");

            }
            if (comboBox3.Text == "البحث العرض اولا(BFS)")
            {

                comboBox2.Items.Clear();

                comboBox2.Items.Add(" الخوارزمية الأولى (BFS)");
                comboBox2.Items.Add("خوارزمية التلوين (BFS)");

            }
            if (comboBox3.Text == "الاجتياز الأقل كلفة للشجرة")
            {

                comboBox2.Items.Clear();
                comboBox2.Items.Add("خوارزمية بريم الاساسية");
                comboBox2.Items.Add("خوارزمية بريم المتقدمة");
                comboBox2.Items.Add("خوارزمية كروسكال الاساسية");
                comboBox2.Items.Add("خوارزمية كورسكال المتقدمة");

            }
        }
        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (comboBox1.Text == "اجتياز الشجرة")
            {
                textend.Visible = false;
                textstart.Visible = false;
                label11.Visible = false;
                label12.Visible = false;
                comboBox3.Items.Clear();
                comboBox2.Items.Clear();
                comboBox3.Items.Add("اجتياز الشجرة الثنائية");
                comboBox2.Items.Add("اجتياز بالترتيب السابق");
                comboBox2.Items.Add("اجتياز بالترتيب الداخل");
                comboBox2.Items.Add("اجتياز بالترتيب اللاحق");
            }
            if (comboBox1.Text == "ارتفاع الشجرة")
            {
                textend.Visible = false;
                textstart.Visible = false;
                label11.Visible = false;
                label12.Visible = false;
                comboBox3.Items.Clear();
                comboBox2.Items.Clear();
                comboBox3.Items.Add("ايجاد الرتفاع الشجرة");
                comboBox2.Items.Add("خوارزمية ايجاد الرتفاع الشجرة");
            }
            if (comboBox1.Text == "اجتياز الرسم البياني")
            {
                textend.Visible = false;
                textstart.Visible = false;
                label11.Visible = false;
                label12.Visible = false;
                comboBox3.Items.Clear();
                comboBox2.Items.Clear();
                comboBox3.Items.Add("الاجتياز الأقل كلفة للشجرة");
                comboBox3.Items.Add("البحث العرض اولا(BFS)");
                comboBox3.Items.Add("بحث العمق اولا(DFS)");


            }
            if (comboBox1.Text == "المسارالأقصر")
            {
                comboBox3.Items.Clear();
                comboBox2.Items.Clear();
                textend.Visible = true;
                textstart.Visible = true;
                label11.Visible = true;
                label12.Visible = true;
                comboBox3.Items.Add("إيجاد المسار الاقصر");
                comboBox2.Items.Add("خوارزمية القوة الغاشمة");
                comboBox2.Items.Add("خوارزمية فلويد");
                comboBox2.Items.Add("خوارزمية ديجسترا");
                comboBox2.Items.Add("خوارزمية بليمان");

            }
        }
    }
 }