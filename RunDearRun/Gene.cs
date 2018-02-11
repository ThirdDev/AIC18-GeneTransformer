using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace RunDearRun
{
    public class Gene
    {
        public string Name { get; private set; }
        public double Score { get; private set; }
        public string ResultText { get; set; }

        string fullPath;
        string attackerAgentArgs;
        string defenderAgentArgs;
        string attackerConfig;
        string defenderConfig;
        string javaPath = "C:\\ProgramData\\Oracle\\Java\\javapath\\java.EXE";

        int serverPort = -1;


        public Gene(DirectoryInfo x)
        {
            Name = x.Name;
            fullPath = x.FullName;
            LoadResults(Path.Combine(x.FullName, @"defend-client\clientConfig.cfg.out"));

            attackerAgentArgs = LoadJsonAttribute(Path.Combine(x.FullName, @"attack-client\process-info\command.info"), "Args");
            defenderAgentArgs = LoadJsonAttribute(Path.Combine(x.FullName, @"defend-client\process-info\command.info"), "Args");

            int.TryParse(LoadJsonAttribute(Path.Combine(x.FullName, @"server\server.cfg"), "ClientsPort"), out serverPort);
        }

        private string CopyConfig(string file)
        {
            var path = Path.GetDirectoryName(file);
            var fileName = Path.GetFileNameWithoutExtension(file);
            var extension = Path.GetExtension(file);

            int i = 2;
            string newFile;
            string newFileFullPath;
            do
            {
                newFile = fileName + i.ToString();
                newFileFullPath = Path.Combine(path, newFile + extension);
                i++;
            }
            while (File.Exists(newFileFullPath));

            File.Copy(file, newFileFullPath);

            return newFile + extension;
        }

        private string LoadJsonAttribute(string file, string attr)
        {
            try
            {
                string json = File.ReadAllText(file);
                var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

                return data[attr];
            }
            catch
            {
                return "";
            }
        }

        private void LoadResults(string fileName)
        {
            try
            {
                ResultText = File.ReadAllText(fileName);
                if (double.TryParse(ResultText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)[2], out double score))
                    Score = score;
            }
            catch (Exception ex)
            {
                ResultText += ex.Message;
                Score = double.MinValue;
            }
        }

        public void Execute(int port)
        {
            attackerConfig = CopyConfig(Path.Combine(fullPath, @"attack-client\clientConfig.cfg"));
            defenderConfig = CopyConfig(Path.Combine(fullPath, @"defend-client\clientConfig.cfg"));

            string attArgs = attackerAgentArgs.Replace($"127.0.0.1 {serverPort} ", $"127.0.0.1 {port} ").Replace("clientConfig.cfg", attackerConfig);
            string defArgs = defenderAgentArgs.Replace($"127.0.0.1 {serverPort} ", $"127.0.0.1 {port} ").Replace("clientConfig.cfg", defenderConfig);

            Execute(javaPath, attArgs);
            Execute(javaPath, defArgs);
        }

        private void Execute(string executable, string args)
        {
            var p = new Process();
            p.StartInfo.FileName = executable;
            p.StartInfo.Arguments = args;
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();

            p.Start();
        }

        public override string ToString()
        {
            return $"{Name} ({serverPort}): {Score.ToString("F3")}";
        }
    }
}