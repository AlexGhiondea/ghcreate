using Creator.Models;
using Octokit;
using OutputColorizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
