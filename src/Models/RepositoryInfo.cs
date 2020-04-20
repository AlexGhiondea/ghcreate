using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Creator.Models
{
    public class RepositoryInfo
    {
        public string Owner { get; set; }
        public string Name { get; set; }

        public static IEnumerable<RepositoryInfo> Parse(List<string> repoList)
        {
            foreach (var item in repoList)
            {
                if (string.IsNullOrEmpty(item))
                    continue;

                var parts = item.Split('\\');
                if (parts.Length != 2)
                    throw new FormatException($"Could not parse repo '{item}'");

                yield return new RepositoryInfo() { Owner = parts[0], Name = parts[1] };
            } 
        }
        public override string ToString()
        {
            return $"{Owner}\\{Name}";
        }
    }
}
