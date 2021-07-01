using Creator.Models;
using Octokit;
using OutputColorizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateLimiter;
using ComposableAsync;

namespace Creator.Helpers
{
    internal static class GitHubHelpers
    {
        public static GitHubClient GetGitHubClientWithToken(string token)
        {
            GitHubClient ghClient = new GitHubClient(new ProductHeaderValue("GitHubSync"));
            ghClient.Credentials = new Credentials(token);
            return ghClient;
        }

        public static async Task<IEnumerable<Models.Objects.Milestone>> ListMilestonesAsync(this GitHubClient s_gitHub, RepositoryInfo repo)
        {
            MilestonesClient ms = new MilestonesClient(new ApiConnection(s_gitHub.Connection));

            var milestonesFromOctokit = await ms.GetAllForRepository(repo.Owner, repo.Name);

            return milestonesFromOctokit.Select(m => new Models.Objects.Milestone(m));
        }

        public static async Task<IEnumerable<Models.Objects.Label>> ListLabelsAsync(this GitHubClient s_gitHub, RepositoryInfo repo)
        {
            IssuesLabelsClient ilc = new IssuesLabelsClient(new ApiConnection(s_gitHub.Connection));

            var labelsFromOctokit = await ilc.GetAllForRepository(repo.Owner, repo.Name);

            return labelsFromOctokit.Select(l => new Models.Objects.Label(l));
        }

        public static async Task<IEnumerable<Models.Objects.Issue>> ListIssuesAsync(this GitHubClient s_gitHub, RepositoryInfo repo, SearchIssuesRequest issueQuery)
        {
            TimeLimiter rateLimiter = TimeLimiter.GetFromMaxCountByInterval(1, TimeSpan.FromSeconds(1));

            List<Models.Objects.Issue> issuesFound = new List<Models.Objects.Issue>();

            int totalPages = -1, currentPage = 0;

            issueQuery.Repos.Add(repo.Owner, repo.Name);

            do
            {
                currentPage++;
                issueQuery.Page = currentPage;

                SearchIssuesResult searchresults = null;

                // make sure the rate limit is met
                await rateLimiter;
                searchresults = await s_gitHub.Search.SearchIssues(issueQuery);

                foreach (Issue item in searchresults.Items)
                {
                    Models.Objects.Issue issueFound = new Models.Objects.Issue(item)
                    {
                        // set the repo on the item
                        RepositoryName = repo.Name,
                        OrganizationName = repo.Owner
                    };
                    issuesFound.Add(issueFound);
                }

                // if this is the first call, setup the totalpages stuff
                if (totalPages == -1)
                {
                    totalPages = (searchresults.TotalCount / 100) + 1;
                }
            } while (totalPages > currentPage);

            return issuesFound;
        }

        public static async Task CreateMilestoneAsync(this GitHubClient client, RepositoryInfo repository, Models.Objects.Milestone milestone)
        {
            MilestonesClient ms = new MilestonesClient(new ApiConnection(client.Connection));

            Colorizer.WriteLine("Creating milestone [Cyan!{0}] in repo [Yellow!{1}]", $"Title: {milestone.Title}, Description: {milestone.Description}, DueOn: {milestone.DueOn}", repository);
            await ms.Create(repository.Owner, repository.Name, milestone.ConvertTo());
            Colorizer.WriteLine("[Green!Success]");
        }

        public static async Task CreateLabelAsync(this GitHubClient client, RepositoryInfo repository, Models.Objects.Label label)
        {
            IssuesLabelsClient ilc = new IssuesLabelsClient(new ApiConnection(client.Connection));

            Colorizer.WriteLine("Creating label [Cyan!{0}] in repo [Yellow!{1}]", $"Title: {label.Title}, Description: {label.Description}, Color: {label.Color}", repository);
            await ilc.Create(repository.Owner, repository.Name, label.ConvertTo());
            Colorizer.WriteLine("[Green!Success]");
        }

        public static async Task CreateOrUpdateLabelAsync(this GitHubClient client, RepositoryInfo repository, Models.Objects.Label label)
        {
            IssuesLabelsClient ilc = new IssuesLabelsClient(new ApiConnection(client.Connection));
            Colorizer.WriteLine("Creating or updating label [Cyan!{0}] in repo [Yellow!{1}]", $"Title: {label.Title}, Description: {label.Description}, Color: {label.Color}", repository);

            try
            {
                Label existingLabel = await ilc.Get(repository.Owner, repository.Name, label.Title);

                // try to update the label
                Colorizer.WriteLine("Label found, updating");
                LabelUpdate updateOperation = new LabelUpdate(label.Title, label.Color);
                updateOperation.Description = label.Description;
                await ilc.Update(repository.Owner, repository.Name, label.Title, updateOperation);
                Colorizer.WriteLine("[Green!Success]");
            }
            catch (NotFoundException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Colorizer.WriteLine("Label not found, creating");
                    // try to create the label
                    await CreateLabelAsync(client, repository, label);
                    Colorizer.WriteLine("[Green!Success]");
                }
            }
        }

        public static async Task CreateOrUpdateMilestoneAsync(this GitHubClient client, RepositoryInfo repository, Models.Objects.Milestone milestone)
        {
            Colorizer.WriteLine("[Yellow!Warning] Updating milestones not supported. Will attempt to create.");

            await CreateMilestoneAsync(client, repository, milestone);
        }


        //private static IEnumerable<Issue> SearchForGitHubIssues(this GitHubClient s_gitHub, SearchIssuesRequest issueQuery)
        //{
        //    List<Issue> totalIssues = new List<Issue>();
        //    int totalPages = -1, currentPage = 0;

        //    do
        //    {
        //        currentPage++;
        //        issueQuery.Page = currentPage;

        //        SearchIssuesResult searchresults = null;

        //        WaitForGitHubCapacity();
        //        searchresults = s_gitHub.Search.SearchIssues(issueQuery).Result;

        //        foreach (Issue item in searchresults.Items)
        //        {
        //            totalIssues.Add(item);
        //        }

        //        // if this is the first call, setup the totalpages stuff
        //        if (totalPages == -1)
        //        {
        //            totalPages = (searchresults.TotalCount / 100) + 1;
        //        }
        //    } while (totalPages > currentPage);

        //    return totalIssues;
        //}

        //private static TimeLimiter rateLimiter = TimeLimiter.GetFromMaxCountByInterval(1, TimeSpan.FromSeconds(1));

        //private static void WaitForGitHubCapacity()
        //{
        //    rateLimiter.GetAwaiter().GetResult();
        //}
    }
}
