using CommandLine.Attributes;
using CommandLine.Attributes.Advanced;
using Creator.Models.Objects;
using System.Collections.Generic;
using System.Linq;

namespace Creator
{
    internal class CmdLineArgs
    {
        [ActionArgument]
        public CommandAction Action { get; set; }

        [CommonArgument]
        [OptionalArgument(null, "token", "The GitHub authentication token.")]
        public string Token { get; set; }

        [RequiredArgument(0,"type", "The type of object we want to list")]
        [ArgumentGroup(nameof(CommandAction.List))]
        public GitHubObjectType ObjectType { get; set; }

        [ArgumentGroup(nameof(CommandAction.Check))]
        [ArgumentGroup(nameof(CommandAction.Create))]
        [ArgumentGroup(nameof(CommandAction.CreateOrUpdate))]
        [RequiredArgument(0, "objectsFile", "The file containing the list of objects to create or check. The structure is: <type>,<name>,<description>,[any other type specific information]")]
        public string ObjectsFile { get; set; }

        [ArgumentGroup(nameof(CommandAction.Check))]
        [ArgumentGroup(nameof(CommandAction.List))]
        [ArgumentGroup(nameof(CommandAction.Create))]
        [ArgumentGroup(nameof(CommandAction.CreateOrUpdate))]
        [RequiredArgument(1, "repos", "The list of repositories where to add the milestones to. The format is: owner\\repoName;owner\\repoName")]
        public string Repositories { get; set; }
        public List<string> RepositoriesList => Repositories.Split(';').ToList();
    }
}

