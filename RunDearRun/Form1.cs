using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;


namespace RunDearRun
{
    public partial class Form1 : Form
    {
        string folderName;
        List<GeneticProcess> processes = new List<GeneticProcess>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Show();

            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
            };

            if (dialog.ShowDialog() != CommonFileDialogResult.Ok)
            {
                Application.Exit();
            }

            folderName = dialog.FileName;

            Init();
        }

        private void Init()
        {
            LoadProcesses();
            ShowData();
        }

        private void ShowData()
        {
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            listBox3.Items.Clear();
            button1.Enabled = false;

            foreach (var item in processes)
            {
                listBox1.Items.Add(item);
            }
        }

        private void LoadProcesses()
        {
            var directories = new DirectoryInfo(folderName).GetDirectories();
            progressBar1.Minimum = 0;
            progressBar1.Value = 0;
            progressBar1.Maximum = directories.Length;
            progressBar1.Visible = true;

            foreach (var item in directories)
            {
                processes.Add(new GeneticProcess(item));

                progressBar1.Value++;
                this.Refresh();

            }

            progressBar1.Visible = false;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadProcessItems(listBox1.SelectedItem as GeneticProcess);
        }

        private void LoadProcessItems(GeneticProcess geneticProcess)
        {
            if (geneticProcess == null)
                return;

            listBox2.Items.Clear();
            listBox3.Items.Clear();
            button1.Enabled = false;

            foreach (var item in geneticProcess.Generations)
            {
                listBox2.Items.Add(item);
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadGeneration(listBox2.SelectedItem as Generation);
        }

        private void LoadGeneration(Generation generation)
        {
            if (generation == null)
                return;

            listBox3.Items.Clear();
            button1.Enabled = false;

            foreach (var item in generation.Genes)
            {
                listBox3.Items.Add(item);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Init();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            (listBox3.SelectedItem as Gene).Execute(int.Parse(textBox1.Text));
        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            button1.Enabled = (listBox3.SelectedIndex >= 0);
        }
    }
}
