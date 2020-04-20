using creator.Models.Objects;
using NUnit.Framework;
using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Creator.Tests
{
    public class ConvertTests
    {
        [Test]
        [Category("Object conversion")]
        public void ConvertMilestone()
        {
            creator.Models.Objects.Milestone m = new creator.Models.Objects.Milestone("Title", "Description", "1/2/2020");
            NewMilestone octokitMilestone = m.ConvertTo();
            Assert.AreEqual(m.Title, octokitMilestone.Title);
            Assert.AreEqual(m.Description, octokitMilestone.Description);
            Assert.AreEqual(m.DueOn, octokitMilestone.DueOn);
        }


        [Test]
        [Category("Object conversion")]
        public void ConvertLabel()
        {
            creator.Models.Objects.Label l = new creator.Models.Objects.Label("Title", "Description", "ffeeff");
            NewLabel octokitLabel = l.ConvertTo();
            Assert.AreEqual(l.Title, octokitLabel.Name);
            Assert.AreEqual(l.Description, octokitLabel.Description);
            Assert.AreEqual(l.Color, octokitLabel.Color);
        }
    }
}