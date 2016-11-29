using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System.IO;

namespace BitBucketServerCSharp.IntegrationTests
{
    [TestFixture]
    public abstract class TestBase
    {
        protected string BASE_URL { get { return _config["base-url"]; } }
        protected string USERNAME { get { return _config["username"]; } }
        protected string PASSWORD { get { return _config["password"]; } }
        protected string EXISTING_PROJECT { get { return _config["existing-project"]; } }
        protected string EXISTING_REPOSITORY { get { return _config["existing-repository"]; } }
        protected string EXISTING_FILE { get { return _config["existing-file"]; } }
        protected string EXISTING_FILE_IN_SUBFOLDER { get { return _config["existing-file-in-subfolder"]; } }
        protected string EXISTING_FILE_IN_SUBFOLDER_WITH_SPACES { get { return _config["existing-file-in-subfolder-with-spaces"]; } }
        protected string EXISTING_COMMIT { get { return _config["existing-commit"]; } }
        protected string EXISTING_OLDER_COMMIT { get { return _config["existing-older-commit"]; } }
        protected string EXISTING_BRANCH_REFERENCE { get { return _config["existing-branch-reference"]; } }
        protected string EXISTING_GROUP { get { return _config["existing-group"]; } }
        protected string EXISTING_HOOK { get { return _config["existing-hook"]; } }
        protected int EXISTING_NUMBER_OF_CHANGES { get { return int.Parse(_config["existing-number-of-changes"]); } }

        protected BitBucketApiClient bitBucketApiClient;

        private IConfigurationRoot _config;

        [OneTimeSetUp]
        public void Initialize()
        {
            // work with with a builder using multiple calls
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("integrationTest-settings.json");
            _config = builder.Build();

            bitBucketApiClient = new BitBucketApiClient(BASE_URL, USERNAME, PASSWORD);
        }

    }
}
