using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunDearRun
{
    public class GeneticProcess
    {
        public string RootPath { get; private set; }
        public string Name { get; private set; }
        public List<Generation> Generations { get; set; }

        public GeneticProcess(DirectoryInfo d)
        {
            RootPath = d.FullName;
            Name = d.Name;
            Generations = d.GetDirectories().Select(x => new Generation(x)).OrderBy(x => ToInt(x.Name)).ToList();
        }

        private object ToInt(string name)
        {
            if (int.TryParse(name, out int result))
                return result;

            return int.MaxValue;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
