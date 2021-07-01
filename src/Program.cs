using Creator.Helpers;
using Creator.Models;
using Creator.Models.Objects;
using Octokit;
using OutputColorizer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace Creator
{
    class Programon
    {
        private static GitHubClient s_gitHub;
        private static CmdLineArgs s_cmdLine;

        static void Main(string[] args)
        {
            using (ColorizerFileLog s_writer = new ColorizerFileLog("fileLog.txt"))
            {
                Colorizer.SetupWriter(s_writer);
                InternalMainAsync(args).GetAwaiter().GetResult();
            }
        }

        static async Task InternalMainAsync(string[] args)
        {
            if (!CommandLine.Parser.TryParse(args, out s_cmdLine))
            {
                return;
            }

            // if we have specified a token, use that.
            if (s_cmdLine.Token == null)
            {
                Colorizer.WriteLine("[Red!Error] Please specify a GitHub access token!");
            }

            s_gitHub = GitHubHelpers.GetGitHubClientWithToken(s_cmdLine.Token);

            if (s_cmdLine.Action == CommandAction.Create)
            {
                await CreateObjectsInGitHubAsync();
            }
            else if (s_cmdLine.Action == CommandAction.CreateOrUpdate)
            {
                await CreateOrUpdateObjectsInGitHubAsync();
            }
            else if (s_cmdLine.Action == CommandAction.List)
            {
                await ListObjectsAsync();
            }
            else if (s_cmdLine.Action == CommandAction.Check)
            {
                await CheckObjectsAsync();
            }
        }

        private static async Task CheckObjectsAsync()
        {
            // This checks that the objects in the list, they exist on those repos
            List<GitHubObject> objectsToCheck = GitHubObject.Parse(s_cmdLine.ObjectsFile);
            IEnumerable<RepositoryInfo> repoToCreateObjectIn = RepositoryInfo.Parse(s_cmdLine.Repositories);

            foreach (RepositoryInfo repo in repoToCreateObjectIn)
            {
                Colorizer.WriteLine("Processing [Magenta!{0}\\{1}] repo.", repo.Owner, repo.Name);

                // we can check labels and milestones.
                HashSet<Creator.Models.Objects.Label> labelsInRepo = new HashSet<Creator.Models.Objects.Label>(await s_gitHub.ListLabelsAsync(repo));
                HashSet<Creator.Models.Objects.Milestone> milestonesInRepo = new HashSet<Creator.Models.Objects.Milestone>(await s_gitHub.ListMilestonesAsync(repo));

                List<GitHubObject> foundObjects = new List<GitHubObject>();

                foreach (GitHubObject item in objectsToCheck)
                {
                    switch (item)
                    {
                        case Creator.Models.Objects.Milestone milestone:
                            if (milestonesInRepo.Contains(milestone))
                            {
                                foundObjects.Add(item);
                            }
                            break;
                        case Creator.Models.Objects.Label label:
                            if (labelsInRepo.Contains(label))
                            {
                                foundObjects.Add(item);
                            }
                            break;
                        default:
                            throw new InvalidOperationException($"Unknown type {item.GetType()}.");
                    }
                }

                // remove all the found objets from the initial list.
                foreach (GitHubObject item in foundObjects)
                {
                    objectsToCheck.Remove(item);
                }

                if (objectsToCheck.Count == 0)
                {
                    Colorizer.WriteLine("[Green!Done]: All requested objects are present");
                }
                else
                {
                    Colorizer.WriteLine("[Red!Done]: The following were not found:");
                    foreach (var item in objectsToCheck)
                    {
                        Console.WriteLine(item.ToString());
                    }
                }
            }
        }

        private static async Task ListObjectsAsync()
        {
            List<RepositoryInfo> reposToList = RepositoryInfo.Parse(s_cmdLine.Repositories).ToList();

            foreach (var repo in reposToList)
            {
                Colorizer.WriteLine("Processing [Magenta!{0}\\{1}] repo.", repo.Owner, repo.Name);

                List<GitHubObject> objects = new List<GitHubObject>();
                if (s_cmdLine.ObjectType.HasFlag(GitHubObjectType.Milestone))
                {
                    objects.AddRange(await s_gitHub.ListMilestonesAsync(repo));
                }
                if (s_cmdLine.ObjectType.HasFlag(GitHubObjectType.Label))
                {
                    objects.AddRange(await s_gitHub.ListLabelsAsync(repo));
                }
                if (s_cmdLine.ObjectType.HasFlag(GitHubObjectType.Issue))
                {
                    SearchIssuesRequest searchIssue = new SearchIssuesRequest()
                    {
                        State = ItemState.Open //Only list open issues
                    };

                    if (!string.IsNullOrEmpty(s_cmdLine.Label))
                    {
                        searchIssue.Labels = new List<string>() { s_cmdLine.Label };
                    }
                    objects.AddRange(await s_gitHub.ListIssuesAsync(repo, searchIssue));
                }

                foreach (var obj in objects)
                {
                    // This is written this way to work around issue #22 in the OutputColorizer
                    Colorizer.WriteLine("{0}", obj.ToString());
                }
            }
        }

        private static async Task CreateObjectsAsync(Func<RepositoryInfo, Models.Objects.Milestone, Task> milestoneDelegate,
            Func<RepositoryInfo, Models.Objects.Label, Task> labelsDelegate)
        {
            Colorizer.WriteLine("[Yellow!Warning]: About to create milestones on GitHub. Proceed? \\[[Red!Yes]/[Green!No]\\]");
            if (Console.ReadKey().Key != ConsoleKey.Y)
            {
                Colorizer.WriteLine("\b[Green!No changes made].");
                return;
            }

            Colorizer.WriteLine("\b[Red!Creating items on GitHub].");

            // parse the data
            IEnumerable<GitHubObject> objectsToCreate = GitHubObject.Parse(s_cmdLine.ObjectsFile);
            IEnumerable<RepositoryInfo> repoToCreateObjectIn = RepositoryInfo.Parse(s_cmdLine.Repositories);

            foreach (RepositoryInfo repo in repoToCreateObjectIn)
            {
                Colorizer.WriteLine("Processing [Yellow!{0}\\{1}] repo.", repo.Owner, repo.Name);
                foreach (GitHubObject item in objectsToCreate)
                {
                    try
                    {
                        switch (item)
                        {
                            case Creator.Models.Objects.Milestone milestone:
                                await milestoneDelegate(repo, milestone);
                                break;
                            case Creator.Models.Objects.Label label:
                                await labelsDelegate(repo, label);
                                break;
                            default:
                                throw new InvalidOperationException($"Unknown type {item.GetType()}.");
                        }
                    }
                    catch (Exception e) when (e is AggregateException)
                    {
                        Colorizer.WriteLine("[Red!Error occured] Creating object failed.");
                        var ag = e as AggregateException;

                        foreach (var exs in ag.InnerExceptions)
                        {
                            Colorizer.WriteLine(exs.Message);
                        }
                    }
                    catch (Exception e)
                    {
                        Colorizer.WriteLine("[Red!Error occured] Creating milestone failed.");
                        Colorizer.WriteLine(e.Message);
                    }
                }
            }
        }

        private static async Task CreateObjectsInGitHubAsync()
        {
            await CreateObjectsAsync(async (repo, milestone) => await s_gitHub.CreateMilestoneAsync(repo, milestone),
                async (repo, label) => await s_gitHub.CreateLabelAsync(repo, label));
        }

        private static async Task CreateOrUpdateObjectsInGitHubAsync()
        {
            await CreateObjectsAsync(async (repo, milestone) => await s_gitHub.CreateOrUpdateMilestoneAsync(repo, milestone),
                async (repo, label) => await s_gitHub.CreateOrUpdateLabelAsync(repo, label));
        }
    }
}

