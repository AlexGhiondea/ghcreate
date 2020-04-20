using Octokit;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Creator.Models.Objects
{
    public class Label : GitHubObject, IConvertToOctokit<Octokit.NewLabel>
    {
        public string Color { get; set; }

        public Label(Octokit.Label label) : base(label.Name, label.Description)
        {
            Color = label.Color;
        }

        public Label(params string[] entries) : base(entries)
        {
            if (entries.Length > 2 && !string.IsNullOrEmpty(entries[2]))
            {
                Color = entries[2];
            }
        }

        public Octokit.NewLabel ConvertTo()
        {
            return new Octokit.NewLabel(Title, Color)
            {
                Description = Description
            };
        }

        public override bool Equals(object obj)
        {
            Label other = obj as Label;

            if (other == null)
                return false;

            return StringComparer.Ordinal.Equals(Title, other.Title) &&
                StringComparer.Ordinal.Equals(Description, other.Description) &&
                StringComparer.OrdinalIgnoreCase.Equals(Color, other.Color);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Title, Description, Color);
        }

        public override string ToString()
        {
            return $"Label: {Title} ({Description}). Color: {Color}";
        }
    }
}
