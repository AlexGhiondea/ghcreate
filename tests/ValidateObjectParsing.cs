using Creator.Models.Objects;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Creator.Tests
{
    public class Tests
    {
        [Test]
        [Category("File parsing")]
        public void MilestonesWithDueDate()
        {
            // create a memorystream-backed streamreader.

            using MemoryStream ms = new MemoryStream();
            using StreamWriter sw = new StreamWriter(ms);

            sw.WriteLine("Milestone,Test1,Description1,1/2/2020");
            sw.WriteLine("Milestone,Test2,Description2,1/3/2020");
            sw.WriteLine("Milestone,Test3,Description3,1/4/2020");
            sw.Flush();

            ms.Seek(0, SeekOrigin.Begin);

            using StreamReader sr = new StreamReader(ms);

            GitHubObject[] objects = Creator.Models.Objects.GitHubObject.Parse(sr).ToArray();

            Assert.AreEqual(3, objects.Length);

            for (int i = 0; i < objects.Length; i++)
            {
                CheckMilestone($"Test{i + 1}", $"Description{i + 1}", $"1/{i + 2}/2020", objects[i]);
            }
        }

        [Test]
        [Category("File parsing")]
        public void MilestonesWithoutDueDate()
        {
            // create a memorystream-backed streamreader.

            using MemoryStream ms = new MemoryStream();
            using StreamWriter sw = new StreamWriter(ms);

            sw.WriteLine("Milestone,Test1,Description1");
            sw.WriteLine("Milestone,Test2,Description2,");
            sw.WriteLine("Milestone,Test3,Description3,");
            sw.Flush();

            ms.Seek(0, SeekOrigin.Begin);

            using StreamReader sr = new StreamReader(ms);

            GitHubObject[] objects = Creator.Models.Objects.GitHubObject.Parse(sr).ToArray();

            Assert.AreEqual(3, objects.Length);

            for (int i = 0; i < objects.Length; i++)
            {
                CheckMilestone($"Test{i + 1}", $"Description{i + 1}", null, objects[i]);
            }
        }

        [Test]
        [Category("File parsing")]
        public void LabelsWithColor()
        {
            // create a memorystream-backed streamreader.

            using MemoryStream ms = new MemoryStream();
            using StreamWriter sw = new StreamWriter(ms);

            sw.WriteLine("Label,Service1,Description1,ffeeff");
            sw.WriteLine("Label,Service2,Description2,ffeeff");
            sw.WriteLine("Label,Service3,Description3,ffeeff");
            sw.Flush();

            ms.Seek(0, SeekOrigin.Begin);

            using StreamReader sr = new StreamReader(ms);

            GitHubObject[] objects = Creator.Models.Objects.GitHubObject.Parse(sr).ToArray();

            Assert.AreEqual(3, objects.Length);

            for (int i = 0; i < objects.Length; i++)
            {
                CheckLabel($"Service{i + 1}", $"Description{i + 1}", "ffeeff", objects[i]);
            }
        }

        [Test]
        [Category("File parsing")]
        public void LabelsWithoutColor()
        {
            // create a memorystream-backed streamreader.

            using MemoryStream ms = new MemoryStream();
            using StreamWriter sw = new StreamWriter(ms);

            sw.WriteLine("Label,Service1,Description1");
            sw.WriteLine("Label,Service2,Description2,");
            sw.WriteLine("Label,Service3,Description3,");
            sw.Flush();

            ms.Seek(0, SeekOrigin.Begin);

            using StreamReader sr = new StreamReader(ms);

            GitHubObject[] objects = Creator.Models.Objects.GitHubObject.Parse(sr).ToArray();

            Assert.AreEqual(3, objects.Length);

            for (int i = 0; i < objects.Length; i++)
            {
                CheckLabel($"Service{i + 1}", $"Description{i + 1}", null, objects[i]);
            }
        }

        [Test]
        [Category("File parsing")]
        public void MixedEntries()
        {
            // create a memorystream-backed streamreader.

            using MemoryStream ms = new MemoryStream();
            using StreamWriter sw = new StreamWriter(ms);

            sw.WriteLine("Label,Service1,Description1");
            sw.WriteLine("Label,Service2,Description2,");
            sw.WriteLine("Label,Service3,Description3,ffeeff");
            sw.WriteLine("Milestone,Test1,Description1");
            sw.WriteLine("Milestone,Test2,Description2,");
            sw.WriteLine("Milestone,Test3,Description3,1/2/2020");
            sw.Flush();

            ms.Seek(0, SeekOrigin.Begin);

            using StreamReader sr = new StreamReader(ms);

            GitHubObject[] objects = Creator.Models.Objects.GitHubObject.Parse(sr).ToArray();

            Assert.AreEqual(6, objects.Length);

            // spell out the checks by hand.
            Assert.IsInstanceOf(typeof(Label), objects[0]);
            Assert.IsInstanceOf(typeof(Label), objects[1]);
            Assert.IsInstanceOf(typeof(Label), objects[2]);
            Assert.IsInstanceOf(typeof(Milestone), objects[3]);
            Assert.IsInstanceOf(typeof(Milestone), objects[4]);
            Assert.IsInstanceOf(typeof(Milestone), objects[5]);

            CheckLabel("Service1", "Description1", null, objects[0]);
            CheckLabel("Service2", "Description2", null, objects[1]);
            CheckLabel("Service3", "Description3", "ffeeff", objects[2]);
            CheckMilestone("Test1", "Description1", null, objects[3]);
            CheckMilestone("Test2", "Description2", null, objects[4]);
            CheckMilestone("Test3", "Description3", "1/2/2020", objects[5]);
        }


        private static void CheckLabel(string title, string description, string color, GitHubObject parsedObject)
        {
            Assert.NotNull(parsedObject);
            Assert.IsInstanceOf(typeof(Label), parsedObject);
            Assert.AreEqual(title, parsedObject.Title);
            Assert.AreEqual(description, parsedObject.Description);
            Assert.AreEqual(color, (parsedObject as Label).Color);
        }
        private static void CheckMilestone(string title, string description, string dueon, GitHubObject parsedObject)
        {
            Assert.NotNull(parsedObject);
            Assert.IsInstanceOf(typeof(Milestone), parsedObject);
            Assert.AreEqual(title, parsedObject.Title);
            Assert.AreEqual(description, parsedObject.Description);

            if (dueon == null)
                Assert.Null((parsedObject as Milestone).DueOn);
            else
                Assert.AreEqual(DateTimeOffset.Parse(dueon), (parsedObject as Milestone).DueOn);
        }
    }
}