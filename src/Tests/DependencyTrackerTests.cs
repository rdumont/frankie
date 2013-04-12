using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace RDumont.Frankie.Tests
{
    [TestFixture]
    public class DependencyTrackerTests
    {
        public class FindFilesThatDependOn
        {
            [Test]
            public void Find_files()
            {
                // Arrange
                var tracker = new TestableDependencyTracker();
                tracker.DependentFiles.Add("layout 1", new HashSet<string>
                    {
                        "some post",
                        "another post"
                    });

                // Act
                var files = tracker.FindDependentFiles("layout 1");

                // Assert
                Assert.That(files, Is.EquivalentTo(new List<string>
                    {
                        "some post",
                        "another post"
                    }));
            }

            [Test]
            public void Empty_result()
            {
                // Arrange
                var tracker = new TestableDependencyTracker();

                // Act
                var files = tracker.FindDependentFiles("unused layout");

                // Assert
                Assert.That(files, Is.Empty);
            }
        }

        public class FindDependenciesOf
        {
            [Test]
            public void With_one_dependency()
            {
                // Arrange
                var tracker = new TestableDependencyTracker();
                tracker.FileDependencies.Add("some post", new HashSet<string> {"layout 1"});

                // Act
                var dependency = tracker.FindFileDependencies("some post");

                // Assert
                Assert.That(dependency, Is.EquivalentTo(new HashSet<string> {"layout 1"}));
            }

            [Test]
            public void With_zero_dependencies()
            {
                // Arrange
                var tracker = new TestableDependencyTracker();

                // Act
                var dependency = tracker.FindFileDependencies("some post");

                // Assert
                Assert.That(dependency, Is.Null);
            }
        }

        public class FindAllDependentFiles
        {
            [Test]
            public void Find_recursively()
            {
                // Arrange
                var tracker = new TestableDependencyTracker();
                tracker.DependentFiles.Add("base layout", new HashSet<string> {"layout 1", "layout 2"});
                tracker.DependentFiles.Add("layout 1", new HashSet<string> {"A", "B"});
                tracker.DependentFiles.Add("layout 2", new HashSet<string> {"C"});

                var expectedOrder = new string[]
                    {
                        "layout 1",
                        "layout 2",
                        "A",
                        "B",
                        "C"
                    };

                // Act
                var files = tracker.FindAllDependentFiles("base layout");

                // Assert
                Assert.That(files, Is.EquivalentTo(expectedOrder));
                for (int i = 0; i < expectedOrder.Length; i++)
                    Assert.That(files[i], Is.EqualTo(expectedOrder[i]));
            }
        }

        public class Add
        {
            [Test]
            public void Add_a_file_with_no_dependencies()
            {
                // Arrange
                var tracker = new TestableDependencyTracker();

                // Act
                tracker.Add("some template", null);

                // Assert
                Assert.That(tracker.DependentFiles, Is.Empty);
                Assert.That(tracker.FileDependencies, Is.Empty);
            }

            [Test]
            public void Add_a_file_with_dependencies()
            {
                // Arrange
                var tracker = new TestableDependencyTracker();

                // Act
                tracker.Add("some post", "layout 1");

                // Assert
                Assert.That(tracker.FileDependencies["some post"],
                    Is.EquivalentTo(new HashSet<string> {"layout 1"}));

                Assert.That(tracker.DependentFiles.Keys, Contains.Item("layout 1"));
                Assert.That(tracker.DependentFiles["layout 1"], Contains.Item("some post"));
            }

            [Test]
            public void Add_same_file_and_dependency_twice()
            {
                // Arrange
                var tracker = new TestableDependencyTracker();

                // Act
                tracker.Add("some post", "layout 1");
                tracker.Add("some post", "layout 1");

                // Assert
                Assert.That(tracker.FileDependencies["some post"],
                    Is.EquivalentTo(new HashSet<string> {"layout 1"}));

                Assert.That(tracker.DependentFiles.Keys, Contains.Item("layout 1"));
                Assert.That(tracker.DependentFiles["layout 1"], Contains.Item("some post"));
            }

            [Test]
            public void Add_two_files_with_same_dependency()
            {
                // Arrange
                var tracker = new TestableDependencyTracker();

                // Act
                tracker.Add("some post", "layout 1");
                tracker.Add("another post", "layout 1");

                // Assert
                Assert.That(tracker.FileDependencies["some post"],
                    Is.EquivalentTo(new HashSet<string> {"layout 1"}));
                Assert.That(tracker.FileDependencies["another post"],
                    Is.EquivalentTo(new HashSet<string> {"layout 1"}));

                Assert.That(tracker.DependentFiles.Keys, Contains.Item("layout 1"));
                Assert.That(tracker.DependentFiles["layout 1"], Contains.Item("some post"));
                Assert.That(tracker.DependentFiles["layout 1"], Contains.Item("another post"));
            }

            [Test]
            public void Add_two_dependencies_for_a_file()
            {
                // Arrange
                var tracker = new TestableDependencyTracker();

                // Act
                tracker.Add("some post", "layout 1");
                tracker.Add("some post", "some component");

                // Assert
                Assert.That(tracker.FileDependencies["some post"],
                    Is.EquivalentTo(new HashSet<string> {"layout 1", "some component"}));

                Assert.That(tracker.DependentFiles["layout 1"], Contains.Item("some post"));
                Assert.That(tracker.DependentFiles["some component"], Contains.Item("some post"));
            }
        }

        public class Remove
        {
            [Test]
            public void Remove_a_file_with_an_unique_dependency()
            {
                // Arrange
                var tracker = new TestableDependencyTracker();
                tracker.FileDependencies.Add("some post", new HashSet<string> {"layout 1"});
                tracker.DependentFiles.Add("layout 1", new HashSet<string>{"some post"});

                // Act
                tracker.Remove("some post");

                // Assert
                Assert.That(tracker.FileDependencies, Is.Empty);
                Assert.That(tracker.DependentFiles, Is.Empty);
            }

            [Test]
            public void Remove_a_file_with_a_shared_dependency()
            {
                // Arrange
                var tracker = new TestableDependencyTracker();

                tracker.FileDependencies.Add("some post", new HashSet<string> {"layout 1"});
                tracker.FileDependencies.Add("another post", new HashSet<string> {"layout 1"});
                tracker.DependentFiles.Add("layout 1", new HashSet<string>
                    {
                        "some post",
                        "another post"
                    });

                // Act
                tracker.Remove("some post");

                // Assert
                Assert.That(tracker.FileDependencies.Keys, Has.None.Contains("some post"));
                Assert.That(tracker.DependentFiles["layout 1"], Has.No.Contains("some post"));
            }

            [Test]
            public void Remove_an_untracked_file()
            {
                // Arrange
                var tracker = new TestableDependencyTracker();

                // Act
                tracker.Remove("untracked");

                // Assert
            }
        }

        public static KeyValuePair<string, HashSet<string>> NameValue(string name, string value)
        {
            return new KeyValuePair<string, HashSet<string>>(name, new HashSet<string> {value});
        } 
    }
}
