using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SourceCombiner
{
    static class ExtensionMethods
    {
        public static string[] GetStrings(this ListBox view)
        {
            var strs = new List<string>();

            foreach (string item in view.Items)
                strs.Add(item);

            return strs.ToArray();
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
