using Creator.Helpers;
using CsvHelper;
using CsvHelper.Configuration;
using OutputColorizer;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            using CsvReader csv = new CsvReader(stream, new CsvConfiguration(CultureInfo.InvariantCulture));
            while (csv.Read())
            {
                // the first field is the type of the object
                GitHubObjectType type = Enum.Parse<GitHubObjectType>(csv.GetField(0));

                // the remaining field are properties of that object
                yield return CreateObject(type, csv.Context.Parser.Record.Skip(1).ToArray());
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
                case GitHubObjectType.Issue:
                    throw new InvalidOperationException("Creating issues is not yet supported");
                default:
                    throw new InvalidOperationException($"Type {type} is not supported.");
            }
        }

        public override string ToString()
        {
            return $"{StringHelpers.EncodeString(Title)},{StringHelpers.EncodeString(Description)}";
        }
    }
}
