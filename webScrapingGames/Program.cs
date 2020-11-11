using System;
using System.Windows.Forms;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using CsQuery.Engine.PseudoClassSelectors;
using com.sun.org.apache.xpath.@internal.operations;

namespace webScrapingGames
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
