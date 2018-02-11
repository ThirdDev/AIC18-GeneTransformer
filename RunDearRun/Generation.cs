using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RunDearRun
{
    public class Generation
    {
        public string Name { get; private set; }
        public List<Gene> Genes { get; private set; }
        public double AverageScore { get; private set; }

        public Generation(DirectoryInfo d)
        {
            Name = d.Name;
            Genes = d.GetDirectories().Select(x => new Gene(x)).OrderByDescending(x => x.Score).ToList();
            AverageScore = Genes.Select(x => x.Score).Where(x => x != double.MinValue).DefaultIfEmpty().Average();
        }

        public override string ToString()
        {
            return $"{Name}: {AverageScore.ToString("F3")}";
        }
    }
}