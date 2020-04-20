using OutputColorizer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;

namespace Creator.Models.Objects
{
    public abstract class GitHubObject
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public GitHubObject(params string[] entries)
        {
            Title = entries[0];
            Description = entries[1];
        }

        public static IEnumerable<GitHubObject> Parse(StreamReader stream)
        {
            string line; int lineCount = 0;

            while ((line = stream.ReadLine()) != null)
            {
                lineCount++;
                // Parse the type, title and description
                string[] entries = line.Split(',', StringSplitOptions.None); // we want empty entries

                if (entries.Length < 3)
                {
                    Colorizer.WriteLine("[Yellow!Warning] Line {0} too short!", lineCount);
                }

                // the type of the object
                GitHubObjectType type = Enum.Parse<GitHubObjectType>(entries[0]);

                yield return CreateObject(type, entries.Skip(1).ToArray());
            }
        }
        public static List<GitHubObject> Parse(string filePath)
        {
            using StreamReader sr = new StreamReader(filePath);
            return Parse(sr).ToList();
        }

        private static GitHubObject CreateObject(GitHubObjectType type, string[] entries)
        {
            switch (type)
            {
                case GitHubObjectType.Milestone:
                    return new Milestone(entries);
                case GitHubObjectType.Label:
                    return new Label(entries);
                default:
                    throw new InvalidOperationException($"Type {type} is not supported.");
            }
        }
    }
}
