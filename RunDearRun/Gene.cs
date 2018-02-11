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

        string attackerAgentArgs;
        string defenderAgentArgs;
        string javaPath = "C:\\ProgramData\\Oracle\\Java\\javapath\\java.EXE";

        int serverPort = -1;


        public Gene(DirectoryInfo x)
        {
            Name = x.Name;
            LoadResults(Path.Combine(x.FullName, @"defend-client\clientConfig.cfg.out"));

            attackerAgentArgs = LoadJsonAttribute(Path.Combine(x.FullName, @"attack-client\process-info\command.info"), "Args");
            defenderAgentArgs = LoadJsonAttribute(Path.Combine(x.FullName, @"defend-client\process-info\command.info"), "Args");

            int.TryParse(LoadJsonAttribute(Path.Combine(x.FullName, @"server\server.cfg"), "ClientsPort"), out serverPort);
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
            attackerAgentArgs = attackerAgentArgs.Replace($"127.0.0.1 {serverPort} ", $"127.0.0.1 {port} ");
            defenderAgentArgs = defenderAgentArgs.Replace($"127.0.0.1 {serverPort} ", $"127.0.0.1 {port} ");

            Execute(javaPath, attackerAgentArgs);
            Execute(javaPath, defenderAgentArgs);
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