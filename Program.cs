﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CGLab2
{
    class Program
    {
        public static void Main()
        {
            Application.Run(new MyForm() {ClientSize = new Size(450, 500)});
        }
    }
}
