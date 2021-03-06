﻿using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Forms;

namespace SourceCombiner
{
    static class ExtensionMethods
    {
        /// <summary>
        /// Gets the Items collection as a string array
        /// </summary>
        /// <param name="view"></param>
        public static string[] GetStrings(this ListBox view)
        {
            return view.Items.Cast<string>().ToArray();
        }

        /// <summary>
        /// Shows the message of an Exception using a Message Dialog
        /// </summary>
        public static void ShowError(this Exception ex)
        {
            MessageBox.Show(ex.Message, Application.ProductName,
            MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void AddStrings(this ListBox view, StringCollection SC)
        {
            view.BeginUpdate();

            foreach (string str in SC)
                view.Items.Add(str);

            view.EndUpdate();
        }
    }
}
