﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsForm
{
    public partial class CoreForm : Form
    {
        private int _bikeId = 1;
        private List<Bike> _bikes = new List<Bike>();
        public CoreForm()
        {
            InitializeComponent();
        }

        
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddBike ab = new AddBike(_bikeId);
            if (ab.ShowDialog() == DialogResult.OK) /* show dialog prints our window */
            {
                Bike bike = ab.GetCreatedBike();
                _bikes.Add(bike);
                AddItemsToListView(bike);
                _bikeId++;
            }
        }

        private void AddItemsToListView(Bike bike)
        {
            var item = new ListViewItem(bike.Name);
            item.SubItems.Add(bike.WheelDiameter.ToString());
            item.SubItems.Add(_bikeId.ToString());
            lvBikes.Items.Add(item);
        }

        private void coreForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            MessageBox.Show("Goodbye!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void lvBikes_SelectedIndexChanged(object sender, EventArgs e)
        {
            var listView = (ListView)sender;
            if (listView.SelectedItems.Count > 0)
            {
                var listViewItem = listView.SelectedItems[0];

                int id = listViewItem.SubItems[2].Text.GetInt();

                foreach (var bike in _bikes)
                {
                    if (bike.Id == id)
                    {
                        BikeType type = (bike.typeOfBike);
                        stContent.Panel2.Controls.Clear();
                        switch (type)
                        {
                            /* Здесь будет вылетать Доработать tryParce */
                            case (BikeType.Cross):
                                stContent.Panel2.Controls.Add(new UcCross((CrossBike)bike) { Dock = DockStyle.Fill });
                                break;
                            case (BikeType.Hard):
                                stContent.Panel2.Controls.Add(new UcHard((HardBike)bike) { Dock = DockStyle.Fill });
                                break;
                            case (BikeType.HardTeil):
                                stContent.Panel2.Controls.Add(new UcHardTeil((HardTeilBike)bike) { Dock = DockStyle.Fill });
                                break;
                        }
                        break;
                    }
                }
            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_bikes.Count > 0)
            {
                RemoveBike rb = new RemoveBike(_bikes);
                if (rb.ShowDialog() == DialogResult.OK)
                {
                    _bikes.RemoveAt(rb.IndexToRemove);
                    lvBikes.Items.RemoveAt(rb.IndexToRemove);
                    stContent.Panel2.Controls.Clear();
                }
            }
            else
            {
                MessageBox.Show("There are no recordings to delete", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void saveToFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string result = string.Empty;
            foreach (var bike in _bikes)
            {
                result += bike + Environment.NewLine;
            }
            File.WriteAllText(@"D:\ITacademy\C#\WindowsFormLongTerm\WindowsForm\TextFile\base.txt", result);



            /* Method 2 
            try
            {
                using (var st = new FileStream(@"D:\ITacademy\C#\WindowsFormLongTerm\WindowsForm\TextFile\base.txt", FileMode.OpenOrCreate))
                {
                    using (var sr = new StreamWriter(st))
                    {
                        foreach (var bike in _bikes)
                        {
                            sr.Write(bike + Environment.NewLine);
                        }
                        sr.Close();
                    }
                    st.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }*/

            /* Method 3 - Old
             * 
             * 
             * Stream st = null;
            StreamWriter sr = null;
            try
            {
                st = new FileStream(@"D:\ITacademy\C#\WindowsFormLongTerm\WindowsForm\TextFile\base.txt",
                    FileMode.OpenOrCreate);
                sr = new StreamWriter(st);
                foreach (var bike in _bikes)
                {
                    sr.Write(bike.ToString() + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                }
                if (st != null)
                {
                    st.Close();
                }
            }*/

        }

        private void openSavedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Text Files|*.txt";
            openFile.Multiselect = false;
            openFile.InitialDirectory = Directory.GetCurrentDirectory();

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                string filetext = File.ReadAllText(openFile.FileName);
                List<string> stringBikes = filetext.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                _bikes.Clear();

                foreach (var stringBike in stringBikes)
                {
                    var stringBikeType = stringBike.Split(';').ToList()[1];
                    BikeType type;
                    Enum.TryParse(stringBikeType, true, out type);

                    switch (type)
                    {
                        /* Add try parce method to Cross and HardTeil classes. Update the method in Hard bike class */

                            /*case BikeType.Cross:
                            HardBike bike;
                            if (HardBike.TryParce(stringBike, out bike))
                            {
                                _bikes.Add(bike);
                            }
                            break;*/

                            case BikeType.Hard:
                            HardBike bike;
                            if (HardBike.TryParce(stringBike, out bike))
                            {
                                _bikes.Add(bike);
                                AddItemsToListView(bike);
                            }
                            break;

                            /*case BikeType.HardTeil:
                            HardBike bike;
                            if (HardBike.TryParce(stringBike, out bike))
                            {
                                _bikes.Add(bike);
                            }
                            break;*/
                    }
                }
            }
        }
    }
}
