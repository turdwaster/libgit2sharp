using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using LibGit2Sharp.Tests.TestHelpers;
using Xunit;
using System.Threading.Tasks;

namespace LibGit2Sharp.Tests
{
    public class MergeDriverFixture
    {
        private const string MergeDriverName = "the-merge-driver";

        [Fact]
        public void CanRegisterAndUnregisterTheSameMergeDriver()
        {
            var mergeDriver = new EmptyMergeDriver(MergeDriverName);

            var registration = GlobalSettings.RegisterMergeDriver(mergeDriver);
            GlobalSettings.DeregisterMergeDriver(registration);

            var secondRegistration = GlobalSettings.RegisterMergeDriver(mergeDriver);
            GlobalSettings.DeregisterMergeDriver(secondRegistration);
        }

        [Fact]
        public void CanRegisterAndDeregisterAfterGarbageCollection()
        {
            var registration = GlobalSettings.RegisterMergeDriver(new EmptyMergeDriver(MergeDriverName));

            GC.Collect();

            GlobalSettings.DeregisterMergeDriver(registration);
        }

        [Fact]
        public void SameMergeDriverIsEqual()
        {
            var mergeDriver = new EmptyMergeDriver(MergeDriverName);
            Assert.Equal(mergeDriver, mergeDriver);
        }

        [Fact]
        public void InitCallbackNotMadeWhenMergeDriverNeverUsed()
        {
            bool called = false;
            Action initializeCallback = () =>
            {
                called = true;
            };

            var driver = new FakeMergeDriver(MergeDriverName, initializeCallback);
            var registration = GlobalSettings.RegisterMergeDriver(driver);

            try
            {
                Assert.False(called);
            }
            finally
            {
                GlobalSettings.DeregisterMergeDriver(registration);
            }
        }

        //[Fact]
        //public void InitCallbackMadeWhenUsingTheFilter()
        //{
        //    bool called = false;
        //    Action initializeCallback = () =>
        //    {
        //        called = true;
        //    };

        //    var filter = new FakeFilter(FilterName,
        //                                attributes,
        //                                successCallback,
        //                                successCallback,
        //                                initializeCallback);
        //    var registration = GlobalSettings.RegisterFilter(filter);

        //    try
        //    {
        //        Assert.False(called);

        //        string repoPath = InitNewRepository();
        //        using (var repo = CreateTestRepository(repoPath))
        //        {
        //            StageNewFile(repo);
        //            Assert.True(called);
        //        }
        //    }
        //    finally
        //    {
        //        GlobalSettings.DeregisterFilter(registration);
        //    }
        //}

        //[Fact]
        //public void WhenStagingFileApplyIsCalledWithCleanForCorrectPath()
        //{
        //    string repoPath = InitNewRepository();
        //    bool called = false;

        //    Action<Stream, Stream> clean = (reader, writer) =>
        //    {
        //        called = true;
        //        reader.CopyTo(writer);
        //    };

        //    var filter = new FakeFilter(FilterName, attributes, clean);
        //    var registration = GlobalSettings.RegisterFilter(filter);

        //    try
        //    {
        //        using (var repo = CreateTestRepository(repoPath))
        //        {
        //            StageNewFile(repo);
        //            Assert.True(called);
        //        }
        //    }
        //    finally
        //    {
        //        GlobalSettings.DeregisterFilter(registration);
        //    }
        //}

        //[Fact]
        //public void CleanFilterWritesOutputToObjectTree()
        //{
        //    const string decodedInput = "This is a substitution cipher";
        //    const string encodedInput = "Guvf vf n fhofgvghgvba pvcure";

        //    string repoPath = InitNewRepository();

        //    Action<Stream, Stream> cleanCallback = SubstitutionCipherFilter.RotateByThirteenPlaces;

        //    var filter = new FakeFilter(FilterName, attributes, cleanCallback);
        //    var registration = GlobalSettings.RegisterFilter(filter);

        //    try
        //    {
        //        using (var repo = CreateTestRepository(repoPath))
        //        {
        //            FileInfo expectedFile = StageNewFile(repo, decodedInput);
        //            var commit = repo.Commit("Clean that file", Constants.Signature, Constants.Signature);
        //            var blob = (Blob)commit.Tree[expectedFile.Name].Target;

        //            var textDetected = blob.GetContentText();
        //            Assert.Equal(encodedInput, textDetected);
        //        }
        //    }
        //    finally
        //    {
        //        GlobalSettings.DeregisterFilter(registration);
        //    }
        //}

        //[Fact]
        //public void CanHandleMultipleSmudgesConcurrently()
        //{
        //    const string decodedInput = "This is a substitution cipher";
        //    const string encodedInput = "Guvf vf n fhofgvghgvba pvcure";

        //    const string branchName = "branch";

        //    Action<Stream, Stream> smudgeCallback = SubstitutionCipherFilter.RotateByThirteenPlaces;

        //    var filter = new FakeFilter(FilterName, attributes, null, smudgeCallback);
        //    var registration = GlobalSettings.RegisterFilter(filter);

        //    try
        //    {
        //        int count = 30;
        //        var tasks = new Task<FileInfo>[count];

        //        for (int i = 0; i < count; i++)
        //        {
        //            tasks[i] = Task.Factory.StartNew(() =>
        //            {
        //                string repoPath = InitNewRepository();
        //                return CheckoutFileForSmudge(repoPath, branchName, encodedInput);
        //            });
        //        }

        //        Task.WaitAll(tasks);

        //        foreach(var task in tasks)
        //        {
        //            FileInfo expectedFile = task.Result;

        //            string readAllText = File.ReadAllText(expectedFile.FullName);
        //            Assert.Equal(decodedInput, readAllText);
        //        }
        //    }
        //    finally
        //    {
        //        GlobalSettings.DeregisterFilter(registration);
        //    }
        //}

        //[Fact]
        //public void WhenCheckingOutAFileFileSmudgeWritesCorrectFileToWorkingDirectory()
        //{
        //    const string decodedInput = "This is a substitution cipher";
        //    const string encodedInput = "Guvf vf n fhofgvghgvba pvcure";

        //    const string branchName = "branch";
        //    string repoPath = InitNewRepository();

        //    Action<Stream, Stream> smudgeCallback = SubstitutionCipherFilter.RotateByThirteenPlaces;

        //    var filter = new FakeFilter(FilterName, attributes, null, smudgeCallback);
        //    var registration = GlobalSettings.RegisterFilter(filter);

        //    try
        //    {
        //        FileInfo expectedFile = CheckoutFileForSmudge(repoPath, branchName, encodedInput);

        //        string combine = Path.Combine(repoPath, "..", expectedFile.Name);
        //        string readAllText = File.ReadAllText(combine);
        //        Assert.Equal(decodedInput, readAllText);
        //    }
        //    finally
        //    {
        //        GlobalSettings.DeregisterFilter(registration);
        //    }
        //}

        //[Fact]
        //public void CanFilterLargeFiles()
        //{
        //    const int ContentLength = 128 * 1024 * 1024 - 13;
        //    const char ContentValue = 'x';

        //    char[] content = (new string(ContentValue, 1024)).ToCharArray();

        //    string repoPath = InitNewRepository();

        //    var filter = new FileExportFilter(FilterName, attributes);
        //    var registration = GlobalSettings.RegisterFilter(filter);

        //    try
        //    {
        //        string filePath = Path.Combine(Directory.GetParent(repoPath).Parent.FullName, Guid.NewGuid().ToString() + ".blob");
        //        FileInfo contentFile = new FileInfo(filePath);
        //        using (var writer = new StreamWriter(contentFile.OpenWrite()) { AutoFlush = true })
        //        {
        //            int remain = ContentLength;

        //            while (remain > 0)
        //            {
        //                int chunkSize = remain > content.Length ? content.Length : remain;
        //                writer.Write(content, 0, chunkSize);
        //                remain -= chunkSize;
        //            }
        //        }

        //        string attributesPath = Path.Combine(Directory.GetParent(repoPath).Parent.FullName, ".gitattributes");
        //        FileInfo attributesFile = new FileInfo(attributesPath);

        //        using (Repository repo = new Repository(repoPath))
        //        {
        //            CreateConfigurationWithDummyUser(repo, Constants.Identity);
        //            File.WriteAllText(attributesPath, "*.blob filter=test");
        //            Commands.Stage(repo, attributesFile.Name);
        //            Commands.Stage(repo, contentFile.Name);
        //            repo.Commit("test", Constants.Signature, Constants.Signature);
        //            contentFile.Delete();
        //            Commands.Checkout(repo, "HEAD", new CheckoutOptions() { CheckoutModifiers = CheckoutModifiers.Force });
        //        }

        //        contentFile = new FileInfo(filePath);
        //        Assert.True(contentFile.Exists, "Contents not restored correctly by forced checkout.");
        //        using (StreamReader reader = contentFile.OpenText())
        //        {
        //            int totalRead = 0;
        //            char[] block = new char[1024];
        //            int read;
        //            while ((read = reader.Read(block, 0, block.Length)) > 0)
        //            {
        //                Assert.True(CharArrayAreEqual(block, content, read));
        //                totalRead += read;
        //            }

        //            Assert.Equal(ContentLength, totalRead);
        //        }

        //        contentFile.Delete();
        //    }
        //    finally
        //    {
        //        GlobalSettings.DeregisterFilter(registration);
        //    }
        //}

        //[Fact]
        //public void DoubleRegistrationFailsButDoubleDeregistrationDoesNot()
        //{
        //    Assert.Equal(0, GlobalSettings.GetRegisteredFilters().Count());

        //    var filter = new EmptyFilter(FilterName, attributes);
        //    var registration = GlobalSettings.RegisterFilter(filter);

        //    Assert.Throws<EntryExistsException>(() => { GlobalSettings.RegisterFilter(filter); });
        //    Assert.Equal(1, GlobalSettings.GetRegisteredFilters().Count());

        //    Assert.True(registration.IsValid, "FilterRegistration.IsValid should be true.");

        //    GlobalSettings.DeregisterFilter(registration);
        //    Assert.Equal(0, GlobalSettings.GetRegisteredFilters().Count());

        //    Assert.False(registration.IsValid, "FilterRegistration.IsValid should be false.");

        //    GlobalSettings.DeregisterFilter(registration);
        //    Assert.Equal(0, GlobalSettings.GetRegisteredFilters().Count());

        //    Assert.False(registration.IsValid, "FilterRegistration.IsValid should be false.");
        //}

        //private unsafe bool CharArrayAreEqual(char[] array1, char[] array2, int count)
        //{
        //    if (ReferenceEquals(array1, array2))
        //    {
        //        return true;
        //    }
        //    if (ReferenceEquals(array1, null) || ReferenceEquals(null, array2))
        //    {
        //        return false;
        //    }
        //    if (array1.Length < count || array2.Length < count)
        //    {
        //        return false;
        //    }

        //    int len = count * sizeof(char);
        //    int cnt = len / sizeof(long);

        //    fixed (char* c1 = array1, c2 = array2)
        //    {
        //        long* p1 = (long*)c1,
        //              p2 = (long*)c2;

        //        for (int i = 0; i < cnt; i++)
        //        {
        //            if (p1[i] != p2[i])
        //            {
        //                return false;
        //            }
        //        }

        //        byte* b1 = (byte*)c1,
        //              b2 = (byte*)c2;

        //        for (int i = len * sizeof(long); i < len; i++)
        //        {
        //            if (b1[i] != b2[i])
        //            {
        //                return false;
        //            }
        //        }
        //    }

        //    return true;
        //}

        //private FileInfo CheckoutFileForSmudge(string repoPath, string branchName, string content)
        //{
        //    FileInfo expectedPath;
        //    using (var repo = CreateTestRepository(repoPath))
        //    {
        //        StageNewFile(repo, content);

        //        repo.Commit("Initial commit", Constants.Signature, Constants.Signature);

        //        expectedPath = CommitFileOnBranch(repo, branchName, content);

        //        Commands.Checkout(repo, "master");

        //        Commands.Checkout(repo, branchName);
        //    }
        //    return expectedPath;
        //}

        //private static FileInfo CommitFileOnBranch(Repository repo, string branchName, String content)
        //{
        //    var branch = repo.CreateBranch(branchName);
        //    Commands.Checkout(repo, branch.FriendlyName);

        //    FileInfo expectedPath = StageNewFile(repo, content);
        //    repo.Commit("Commit", Constants.Signature, Constants.Signature);
        //    return expectedPath;
        //}

        //private static FileInfo StageNewFile(IRepository repo, string contents = "null")
        //{
        //    string newFilePath = Touch(repo.Info.WorkingDirectory, Guid.NewGuid() + ".txt", contents);
        //    var stageNewFile = new FileInfo(newFilePath);
        //    Commands.Stage(repo, newFilePath);
        //    return stageNewFile;
        //}

        //private Repository CreateTestRepository(string path)
        //{
        //    var repository = new Repository(path);
        //    CreateConfigurationWithDummyUser(repository, Constants.Identity);
        //    CreateAttributesFile(repository, "* filter=test");
        //    return repository;
        //}

        class EmptyMergeDriver : MergeDriver
        {
            public EmptyMergeDriver(string name)
                : base(name)
            { }

            protected override int Apply(MergeDriverSource source)
            {
                throw new NotImplementedException();
            }

            protected override void Initialize()
            {
                throw new NotImplementedException();
            }
        }

        class FakeMergeDriver : MergeDriver
        {
            private readonly Action initCallback;

            public FakeMergeDriver(string name, Action initCallback = null)
                : base(name)
            {
                this.initCallback = initCallback;
            }

            protected override void Initialize()
            {
                if (initCallback != null)
                {
                    initCallback();
                }
            }

            protected override int Apply(MergeDriverSource source)
            {
                throw new NotImplementedException();
            }
        }
    }
}
