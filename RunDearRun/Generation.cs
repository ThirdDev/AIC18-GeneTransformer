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
        public double MaxScore { get; private set; }
        public string RootPath { get; private set; }

        public Generation(DirectoryInfo d)
        {
            RootPath = d.FullName;
            Name = d.Name;
            Genes = d.GetDirectories().Select(x => new Gene(x)).OrderByDescending(x => x.Score).ToList();
            AverageScore = Genes.Select(x => x.Score).Where(x => x != double.MinValue).DefaultIfEmpty().Average();
            MaxScore = Genes.Select(x => x.Score).Where(x => x != double.MinValue).DefaultIfEmpty().Max();
        }

        public override string ToString()
        {
            return $"{Name} - Avg: {AverageScore.ToString("F3")}, Max: {MaxScore.ToString("F3")}";
        }
    }
}