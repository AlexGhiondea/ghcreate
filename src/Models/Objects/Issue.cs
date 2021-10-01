using Creator.Helpers;
using Octokit;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Creator.Models.Objects
{
    public class Issue : GitHubObject, IConvertToOctokit<Octokit.NewIssue>
    {
        public string AssignedTo { get; set; }
        public string Milestone { get; set; }

        public string RepositoryName { get; set; }
        public string OrganizationName { get; set; }

        public string Labels { get; set; }

        public Issue(Octokit.Issue issue) : base(issue.Title, issue.HtmlUrl)
        {
            AssignedTo = issue.Assignee?.Login;
            Milestone = issue.Milestone?.Title;
            Labels = string.Join(",", issue.Labels.OrderBy(x => x.Name).Select(x => x.Name));
        }

        public Issue(params string[] entries) : base(entries)
        {
            // The way the information is layed out in the input file is:
            // title, HtmlUrl, assignedTo, Milestone, RepoName, OrgName
            if (entries.Length != 6)
            {
                throw new NotSupportedException("Not enough information to create an issue");
            }
            AssignedTo = entries[2];
            Milestone = entries[3];
            RepositoryName = entries[4];
            OrganizationName = entries[5];
        }

        public override bool Equals(object obj)
        {
            Issue other = obj as Issue;

            if (other == null)
                return false;

            return StringComparer.Ordinal.Equals(Title, other.Title) &&
                StringComparer.Ordinal.Equals(Description, other.Description) &&
                StringComparer.OrdinalIgnoreCase.Equals(AssignedTo, other.AssignedTo) &&
                StringComparer.OrdinalIgnoreCase.Equals(Milestone, other.Milestone) &&
                StringComparer.OrdinalIgnoreCase.Equals(RepositoryName, other.RepositoryName) &&
                StringComparer.OrdinalIgnoreCase.Equals(OrganizationName, other.OrganizationName);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Title, Description, AssignedTo, Milestone, RepositoryName, OrganizationName);
        }

        public override string ToString()
        {
            return $"Issue,{base.ToString()},{AssignedTo},{Milestone},{RepositoryName},{OrganizationName},{Labels}";
        }

        public NewIssue ConvertTo()
        {
            throw new NotImplementedException();
        }
    }
}
