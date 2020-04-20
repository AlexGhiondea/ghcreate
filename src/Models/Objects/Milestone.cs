using Octokit;
using OutputColorizer;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Creator.Models.Objects
{
    public class Milestone : GitHubObject, IConvertToOctokit<NewMilestone>
    {
        public Milestone(Octokit.Milestone milestone) : base(milestone.Title, milestone.Description)
        {
            DueOn = milestone.DueOn;
        }

        public Milestone(params string[] entries) : base(entries)
        {
            // Allow milestone with no DueOn date.
            if (entries.Length > 2 && !string.IsNullOrEmpty(entries[2]))
            {
                DueOn = DateTimeOffset.Parse(entries[2]);
            }
        }

        public DateTimeOffset? DueOn { get; set; }
        public object OpenIssues { get; internal set; }
        public object ClosedIssues { get; internal set; }

        public NewMilestone ConvertTo()
        {
            var milestone = new NewMilestone(Title)
            {
                Description = Description,
                DueOn = DueOn
            };

            if (string.IsNullOrEmpty(DueOn?.ToString()))
                milestone.DueOn = null;
            return milestone;
        }

        public override bool Equals(object obj)
        {
            Milestone other = obj as Milestone;

            if (other == null)
                return false;

            return StringComparer.Ordinal.Equals(Title, other.Title) &&
                StringComparer.Ordinal.Equals(Description, other.Description) &&
                DueOn == other.DueOn;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Title, Description, DueOn, OpenIssues, ClosedIssues);
        }

        public override string ToString()
        {
            return $"Milestone: {Title} ({Description}). DueOn: {DueOn}";
        }
    }
}
