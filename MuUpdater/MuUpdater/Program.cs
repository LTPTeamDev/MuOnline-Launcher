﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MuUpdater
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MuUdaterForm());
        }
    }
}
