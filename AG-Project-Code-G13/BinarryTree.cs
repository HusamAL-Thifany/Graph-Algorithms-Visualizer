using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
namespace AG_Project_Code_G13
{
  // كلاس انشاء العقد
    public class TreeNode
    {   
        public string Value;
        public TreeNode Left, Right;
        public  int X, Y;
        public TreeNode parent;
        public TreeNode(string value)
        {
            Value = value;
            Left = Right = null;
            parent = null;
        }
        public TreeNode()
        {
           
        }
    }
}


