using System;
using System.Windows;
using System.Windows.Controls;
//using System.Windows.Forms;
using System.Windows.Input;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Data;

namespace DBManager
{
    class SavetToCsvFile
    {  
        public static void ExtractDataToCSV(DataTable dtTab)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV files (*.csv)|*.csv";
            if (sfd.ShowDialog() == true)
            {

                StreamWriter sw = new StreamWriter(sfd.FileName, false);
                //headers    
                for (int i = 0; i < dtTab.Columns.Count; i++)
                {
                    sw.Write(dtTab.Columns[i]);
                    if (i < dtTab.Columns.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);
                foreach (DataRow dr in dtTab.Rows)
                {
                    for (int i = 0; i < dtTab.Columns.Count; i++)
                    {
                        if (!Convert.IsDBNull(dr[i]))
                        {
                            string value = dr[i].ToString();
                            if (value.Contains(","))
                            {
                                value = String.Format("\"{0}\"", value);
                                sw.Write(value);
                            }
                            else
                            {
                                sw.Write(dr[i].ToString());
                            }
                        }
                        if (i < dtTab.Columns.Count - 1)
                        {
                            sw.Write(",");
                        }
                    }
                    sw.Write(sw.NewLine);
                }
                sw.Close();
            }
            /*/----------------------------------------work solution №1------------------------------------
             String result = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);
             char[] delims = { '\n', '\r' };
             var res1 = result.Split(delims, StringSplitOptions.RemoveEmptyEntries);
             var res2 = res1.Where(x => Regex.Match(x, "[^,]").Success).ToArray();
             result = string.Join("\n", res2);

            dtTab.SelectAllCells();
            dtTab.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
            ApplicationCommands.Copy.Execute(null, dtTab);
            dtTab.UnselectAllCells();

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV files (*.csv)|*.csv";

            String result = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);
            if (sfd.ShowDialog() == true)
            {
                try
                {
                    StreamWriter sw = new StreamWriter(sfd.FileName);
                    sw.WriteLine(result);
                    sw.Close();
                    Process.Start(sfd.FileName);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
            //------------------------------------------------------------------------------------------*/

            //-------------------------------------------solution №2-------------------------------------/
            /*/ Don't save if no data is returned
            if (dtTab.Rows.Count == 0)
            {
                return;
            }
            StringBuilder sb = new StringBuilder();
            // Column headers
            string columnsHeader = "";
            for (int i = 0; i < dtTab.Columns.Count; i++)
            {
                columnsHeader += dtTab.Columns[i].Name + ",";
            }
            sb.Append(columnsHeader + Environment.NewLine);
            // Go through each cell in the datagridview
            foreach (DataGridView dtTabRow in dtTab.Rows)
            {
                // Make sure it's not an empty row.
                if (!dtTabRow.CurrentRow.IsNewRow)
                {
                    for (int c = 0; c < dtTabRow.Rows.Count; c++)
                    {
                        // Append the cells data followed by a comma to delimit.

                        sb.Append(dtTabRow.Rows[c] + ",");
                    }
                    // Add a new line in the text file.
                    sb.Append(Environment.NewLine);
                }
            }
            // Load up the save file dialog with the default option as saving as a .csv file.
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV files (*.csv)|*.csv"; 
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                // If they've selected a save location...
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(sfd.FileName, false))
                {
                    // Write the stringbuilder text to the the file.
                    sw.WriteLine(sb.ToString());
                }
            }
            // Confirm to the user it has been completed.
            MessageBox.Show("CSV file saved.");
            //--------------------------------------------------------------------------------------

            /*----------------------------------------solution №3-----------------------------------
            string separator = ",";
            object[,] data = dtTab.PrepareData();
            StringBuilder builder = new StringBuilder(Convert.ToString((char)65279));

            for (int k = 0; k < data.GetLength(0); k++)
            {
                List<string> tempList = new List<string>();
                for (int l = 0; l < data.GetLength(1); l++)
                    tempList.Add(data[k, l].ToString());
                builder.Append(string.Join(separator, tempList)).Append(Environment.NewLine);
            }
            string dataToWrite = builder.ToString();
            try
            {
                string destination = DateTime.Now.ToString("dd_MM_yyyy_HH_mm");
                foreach (char c in System.IO.Path.GetInvalidFileNameChars())
                {
                    destination = destination.Replace(c, '_');
                }
                destination = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + destination + ".xlsx";
                FileStream fs = new FileStream(destination, FileMode.Create, FileAccess.Write);
                StreamWriter objWrite = new StreamWriter(fs);
                objWrite.Write(dataToWrite);
                objWrite.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); 
            }
            //--------------------------------------------------------------------------------------*/

        }

    }
}
