using BitBucketServerCSharp.Api;
using BitBucketServerCSharp.Entities;
using BitBucketServerCSharp.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BitBucketServerCSharp.IntegrationTests
{
    [TestFixture]
    public class BitBucketApiClientTester : TestBase
    {
        [Test]
        public async Task Can_GetFileContents()
        {
            var response = await bitBucketApiClient.Repositories.GetFileContents(EXISTING_PROJECT, EXISTING_REPOSITORY, EXISTING_FILE);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.FileContents.Count > 0);
            Assert.IsTrue(response.Size > 0);
        }

        [Test]
        public async Task Can_GetFileContents_In_SubFolder()
        {
            var response = await bitBucketApiClient.Repositories.GetFileContents(EXISTING_PROJECT, EXISTING_REPOSITORY, EXISTING_FILE_IN_SUBFOLDER);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.FileContents.Count > 0);
            Assert.AreEqual(1, response.Size);
        }

        [Test]
        public async Task Can_GetFileContents_In_SubFolder_With_Spaces()
        {
            var response = await bitBucketApiClient.Repositories.GetFileContents(EXISTING_PROJECT, EXISTING_REPOSITORY, EXISTING_FILE_IN_SUBFOLDER_WITH_SPACES);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.FileContents.Count > 0);
            Assert.AreEqual(1, response.Size);
        }

        [Test]
        public async Task Can_GetBranchesForCommit()
        {
            var response = await bitBucketApiClient.Branches.GetByCommitId(EXISTING_PROJECT, EXISTING_REPOSITORY, EXISTING_OLDER_COMMIT);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Values.Any(x => x.Id.Equals(EXISTING_BRANCH_REFERENCE)));
        }

        [Test]
        public async Task Can_GetAllProjects()
        {
            var response = await bitBucketApiClient.Projects.Get();
            var projects = response.Values;

            Assert.IsNotNull(projects);
            Assert.IsInstanceOf<IEnumerable<Project>>(projects);
            Assert.IsTrue(projects.Any());
        }

        [Test]
        public async Task Can_GetAllProjects_WithRequestOptions()
        {
            int requestLimit = 1;
            var response = await bitBucketApiClient.Projects.Get(new RequestOptions { Limit = requestLimit, Start = 0 });
            var projects = response.Values;

            Assert.IsNotNull(projects);
            Assert.IsInstanceOf<IEnumerable<Project>>(projects);
            Assert.AreEqual(requestLimit, projects.Count());
        }

        [Test]
        public async Task Can_GetByIdProject()
        {
            var project = await bitBucketApiClient.Projects.GetById(EXISTING_PROJECT);

            Assert.IsNotNull(project);
            Assert.IsInstanceOf<Project>(project);
            Assert.AreEqual(EXISTING_PROJECT.ToLower(), project.Name.ToLower());
        }

        [Test]
        public async Task Can_GetAllRepositories()
        {
            var response = await bitBucketApiClient.Repositories.Get(EXISTING_PROJECT);
            var repositories = response.Values;

            Assert.IsNotNull(repositories);
            Assert.IsInstanceOf<IEnumerable<Repository>>(repositories);
            Assert.IsTrue(repositories.Any());
        }

        [Test]
        public async Task Can_GetAllRepositories_WithRequestOptions()
        {
            int requestLimit = 1;
            var response = await bitBucketApiClient.Repositories.Get(EXISTING_PROJECT, new RequestOptions { Limit = requestLimit });
            var repositories = response.Values;

            Assert.IsNotNull(repositories);
            Assert.IsInstanceOf<IEnumerable<Repository>>(repositories);
            Assert.AreEqual(requestLimit, repositories.Count());
        }

        [Test]
        public async Task Can_GetByIdRepository()
        {
            var repository = await bitBucketApiClient.Repositories.GetById(EXISTING_PROJECT, EXISTING_REPOSITORY);

            Assert.IsNotNull(repository);
            Assert.IsInstanceOf<Repository>(repository);
            Assert.AreEqual(EXISTING_REPOSITORY.ToLower(), repository.Name.ToLower());
        }

        [Test]
        public async Task Can_GetAllTags()
        {
            var response = await bitBucketApiClient.Repositories.GetTags(EXISTING_PROJECT, EXISTING_REPOSITORY);
            var tags = response.Values;

            Assert.IsNotNull(tags);
            Assert.IsInstanceOf<IEnumerable<Tag>>(tags);
            Assert.IsTrue(tags.Any());
        }

        [Test]
        public async Task Can_GetAllTags_WithRequestOptions()
        {
            int requestLimit = 1;
            var response = await bitBucketApiClient.Repositories.GetTags(EXISTING_PROJECT, EXISTING_REPOSITORY, new RequestOptions { Limit = requestLimit });
            var tags = response.Values;

            Assert.IsNotNull(tags);
            Assert.IsInstanceOf<IEnumerable<Tag>>(tags);
            Assert.AreEqual(requestLimit, tags.Count());
        }

        [Test]
        public async Task Can_Create_And_Delete_Tags()
        {
            var initialResponse = await bitBucketApiClient.Repositories.GetTags(EXISTING_PROJECT, EXISTING_REPOSITORY);
            int initialTagCount = initialResponse.Values.Count();

            // create tag
            Tag createTag = new Tag
            {
                Force = true,
                Name = "integration-test-tag",
                Message = "integration test tag",
                StartPoint = "refs/heads/master",
                Type = TagType.ANNOTATED
            };
            var createResponse = await bitBucketApiClient.Repositories.CreateTag(EXISTING_PROJECT, EXISTING_REPOSITORY, createTag);

            // mid-step get tags again
            var midResponse = await bitBucketApiClient.Repositories.GetTags(EXISTING_PROJECT, EXISTING_REPOSITORY);
            int midTagCount = midResponse.Values.Count();
            Assert.AreEqual(initialTagCount + 1, midTagCount);
            Assert.IsTrue(midResponse.Values.Any(x => x.Id.Contains(createTag.Name)));

            // delete tag
            await bitBucketApiClient.Repositories.DeleteTag(EXISTING_PROJECT, EXISTING_REPOSITORY, createTag.Name);

            // final check to ensure the tag count didn't change
            var finalResponse = await bitBucketApiClient.Repositories.GetTags(EXISTING_PROJECT, EXISTING_REPOSITORY);
            int finalTagCount = initialResponse.Values.Count();

            Assert.AreEqual(initialTagCount, finalTagCount);
        }

        [Test]
        public async Task Can_GetAllFiles()
        {
            var response = await bitBucketApiClient.Repositories.GetFiles(EXISTING_PROJECT, EXISTING_REPOSITORY);
            var files = response.Values;

            Assert.IsNotNull(files);
            Assert.IsInstanceOf<IEnumerable<string>>(files);
            Assert.IsTrue(files.Any());
        }

        [Test]
        public async Task Can_GetAllFiles_WithRequestOptions()
        {
            int requestLimit = 1;
            var response = await bitBucketApiClient.Repositories.GetFiles(EXISTING_PROJECT, EXISTING_REPOSITORY, new RequestOptions { Limit = requestLimit });
            var files = response.Values;

            Assert.IsNotNull(files);
            Assert.IsInstanceOf<IEnumerable<string>>(files);
            Assert.AreEqual(requestLimit, files.Count());
        }

        [Test]
        public async Task Can_GetAllBranches()
        {
            var response = await bitBucketApiClient.Branches.Get(EXISTING_PROJECT, EXISTING_REPOSITORY);
            var branches = response.Values;

            Assert.IsNotNull(branches);
            Assert.IsInstanceOf<IEnumerable<Branch>>(branches);
            Assert.IsTrue(branches.Any());
        }

        [Test]
        public async Task GetPullRequest_RetrieveAllPullRequests_ReturnsSomePullRequests()
        {
            var response = await bitBucketApiClient.PullRequests.Get(EXISTING_PROJECT, EXISTING_REPOSITORY, state: PullRequestState.ALL);
            var pullRequests = response.Values;

            Assert.IsNotNull(pullRequests);
            Assert.IsInstanceOf<IEnumerable<PullRequest>>(pullRequests);
            Assert.IsTrue(pullRequests.Any());
        }

        [Test]
        public async Task GetPullRequest_WithRequestOptions_ReturnsSomePullRequests()
        {
            var response = await bitBucketApiClient.PullRequests.Get(EXISTING_PROJECT, EXISTING_REPOSITORY, new RequestOptions { Limit = 1 }, state: PullRequestState.ALL);
            var pullRequests = response.Values;

            Assert.IsNotNull(pullRequests);
            Assert.IsInstanceOf<IEnumerable<PullRequest>>(pullRequests);
            Assert.IsTrue(pullRequests.Any());
        }

        [Test]
        public async Task Can_GetAllBranches_WithRequestOptions()
        {
            int requestLimit = 1;
            var response = await bitBucketApiClient.Branches.Get(EXISTING_PROJECT, EXISTING_REPOSITORY, new RequestOptions { Limit = requestLimit });
            var branches = response.Values;

            Assert.IsNotNull(branches);
            Assert.IsInstanceOf<IEnumerable<Branch>>(branches);
            Assert.AreEqual(requestLimit, branches.Count());
        }

        [Test]
        public async Task Can_GetAllCommits()
        {
            var response = await bitBucketApiClient.Commits.Get(EXISTING_PROJECT, EXISTING_REPOSITORY);
            var commits = response.Values;

            Assert.IsNotNull(commits);
            Assert.IsInstanceOf<IEnumerable<Commit>>(commits);
            Assert.IsTrue(commits.Any());
        }

        [Test]
        public async Task Can_GetAllCommits_WithRequestOptions()
        {
            int requestLimit = 2;
            var response = await bitBucketApiClient.Commits.Get(EXISTING_PROJECT, EXISTING_REPOSITORY, new RequestOptions { Limit = requestLimit });
            var commits = response.Values;

            Assert.IsNotNull(commits);
            Assert.IsInstanceOf<IEnumerable<Commit>>(commits);
            Assert.AreEqual(requestLimit, commits.Count());
        }

        [Test]
        public async Task Can_GetAllCommits_WithRequestOptionsForCommits()
        {
            int expectedCommitCount = 1;
            var response = await bitBucketApiClient.Commits.Get(EXISTING_PROJECT, EXISTING_REPOSITORY, null, new RequestOptionsForCommits { Until = EXISTING_COMMIT, Since = EXISTING_OLDER_COMMIT });
            var commits = response.Values;

            Assert.IsNotNull(commits);
            Assert.IsInstanceOf<IEnumerable<Commit>>(commits);
            Assert.AreEqual(expectedCommitCount, commits.Count());
        }

        [Test]
        public async Task Can_GetByIdCommit()
        {
            var commit = await bitBucketApiClient.Commits.GetById(EXISTING_PROJECT, EXISTING_REPOSITORY, EXISTING_COMMIT);

            Assert.IsNotNull(commit);
            Assert.IsInstanceOf<Commit>(commit);
            Assert.AreEqual(EXISTING_COMMIT.ToLower(), commit.Id.ToLower());
        }

        [Test]
        public async Task Can_GetChangesUntil()
        {
            var changes = await bitBucketApiClient.Commits.GetChanges(EXISTING_PROJECT, EXISTING_REPOSITORY, EXISTING_COMMIT);

            Assert.IsNotNull(changes);
            Assert.IsInstanceOf<Changes>(changes);
            Assert.AreEqual(EXISTING_COMMIT.ToLower(), changes.ToHash.ToLower());
        }

        [Test]
        public async Task Can_GetChangesUntil_WithRequestOptions()
        {
            int requestLimit = 1;
            var changes = await bitBucketApiClient.Commits.GetChanges(EXISTING_PROJECT, EXISTING_REPOSITORY, EXISTING_COMMIT, null, new RequestOptions { Limit = requestLimit });

            Assert.IsNotNull(changes);
            Assert.IsInstanceOf<Changes>(changes);
            Assert.AreEqual(EXISTING_COMMIT.ToLower(), changes.ToHash.ToLower());
            Assert.AreEqual(requestLimit, changes.ListOfChanges.Count());
        }

        [Test]
        public async Task Can_GetChangesUntil_And_Since()
        {
            var changes = await bitBucketApiClient.Commits.GetChanges(EXISTING_PROJECT, EXISTING_REPOSITORY, EXISTING_COMMIT, EXISTING_OLDER_COMMIT);

            Assert.IsNotNull(changes);
            Assert.IsInstanceOf<Changes>(changes);
            Assert.AreEqual(EXISTING_COMMIT.ToLower(), changes.ToHash.ToLower());
        }

        [Test]
        public async Task Can_GetChangesUntil_And_Since_WithRequestOptions()
        {
            int requestLimit = 1;
            var changes = await bitBucketApiClient.Commits.GetChanges(EXISTING_PROJECT, EXISTING_REPOSITORY, EXISTING_COMMIT, EXISTING_OLDER_COMMIT, new RequestOptions { Limit = requestLimit });

            Assert.IsNotNull(changes);
            Assert.IsInstanceOf<Changes>(changes);
            Assert.AreEqual(EXISTING_COMMIT.ToLower(), changes.ToHash.ToLower());
            Assert.AreEqual(requestLimit, changes.ListOfChanges.Count());
        }

        [Test]
        public async Task Can_GetChangesUntil_And_Since_MoreThanOneResult()
        {
            var changes = await bitBucketApiClient.Commits.GetChanges(EXISTING_PROJECT, EXISTING_REPOSITORY, EXISTING_COMMIT, EXISTING_OLDER_COMMIT);

            Assert.IsNotNull(changes);
            Assert.IsInstanceOf<Changes>(changes);
            Assert.IsTrue(EXISTING_COMMIT.Equals(changes.ToHash, StringComparison.OrdinalIgnoreCase));
            Assert.AreEqual(EXISTING_NUMBER_OF_CHANGES, changes.ListOfChanges.Count());
        }

        [Test]
        public async Task Can_GetCommitsUntil()
        {
            var commits = await bitBucketApiClient.Commits.GetCommits(EXISTING_PROJECT, EXISTING_REPOSITORY, EXISTING_COMMIT);

            Assert.IsNotNull(commits);
            Assert.IsInstanceOf<ResponseWrapper<Commit>>(commits);
            Assert.IsTrue(commits.Values.Count() > 1);
            Assert.IsTrue(commits.Values.Any(x => x.Id.Equals(EXISTING_COMMIT, StringComparison.OrdinalIgnoreCase)));
        }

        [Test]
        public async Task Can_GetCommitsUntil_WithRequestOptions()
        {
            int requestLimit = 1;
            var commits = await bitBucketApiClient.Commits.GetCommits(EXISTING_PROJECT, EXISTING_REPOSITORY, EXISTING_COMMIT, null, new RequestOptions { Limit = requestLimit });

            Assert.IsNotNull(commits);
            Assert.IsInstanceOf<ResponseWrapper<Commit>>(commits);
            Assert.IsTrue(commits.Values.Count() > 0);
            Assert.IsTrue(commits.Values.Any(x => x.Id.Equals(EXISTING_COMMIT, StringComparison.OrdinalIgnoreCase)));
        }

        [Test]
        public async Task Can_GetCommitsUntil_And_Since()
        {
            var commits = await bitBucketApiClient.Commits.GetCommits(EXISTING_PROJECT, EXISTING_REPOSITORY, EXISTING_COMMIT, EXISTING_OLDER_COMMIT);

            Assert.IsNotNull(commits);
            Assert.IsInstanceOf<ResponseWrapper<Commit>>(commits);
            Assert.IsTrue(commits.Values.Count() > 0);
            Assert.IsTrue(commits.Values.Any(x => x.Id.Equals(EXISTING_COMMIT, StringComparison.OrdinalIgnoreCase)));
            // excluside call (excludes 'since' commit)
            Assert.IsFalse(commits.Values.Any(x => x.Id.Equals(EXISTING_OLDER_COMMIT, StringComparison.OrdinalIgnoreCase)));
        }

        [Test]
        public async Task Can_GetCommitsUntil_And_Since_WithRequestOptions()
        {
            int requestLimit = 1;
            var commits = await bitBucketApiClient.Commits.GetCommits(EXISTING_PROJECT, EXISTING_REPOSITORY, EXISTING_COMMIT, EXISTING_OLDER_COMMIT, new RequestOptions { Limit = requestLimit });

            Assert.IsNotNull(commits);
            Assert.IsInstanceOf<ResponseWrapper<Commit>>(commits);
            Assert.IsTrue(commits.Values.Count() > 0);
            Assert.IsTrue(commits.Values.Any(x => x.Id.Equals(EXISTING_COMMIT, StringComparison.OrdinalIgnoreCase)));
            // excluside call (excludes 'since' commit)
            Assert.IsFalse(commits.Values.Any(x => x.Id.Equals(EXISTING_OLDER_COMMIT, StringComparison.OrdinalIgnoreCase)));
        }

        [Test]
        public async Task Can_GetCommitsUntil_And_Since_MoreThanOneResult()
        {
            var commits = await bitBucketApiClient.Commits.GetCommits(EXISTING_PROJECT, EXISTING_REPOSITORY, EXISTING_COMMIT, EXISTING_OLDER_COMMIT);

            Assert.IsNotNull(commits);
            Assert.IsInstanceOf<ResponseWrapper<Commit>>(commits);
            Assert.IsTrue(commits.Values.Count() > 0);
            Assert.IsTrue(commits.Values.Any(x => x.Id.Equals(EXISTING_COMMIT, StringComparison.OrdinalIgnoreCase)));
            // excluside call (excludes 'since' commit)
            Assert.IsFalse(commits.Values.Any(x => x.Id.Equals(EXISTING_OLDER_COMMIT, StringComparison.OrdinalIgnoreCase)));
        }

        [Test]
        public async Task Can_GetRepository_Hooks()
        {
            var response = await bitBucketApiClient.Repositories.GetHooks(EXISTING_PROJECT, EXISTING_REPOSITORY);
            var hooks = response.Values;

            Assert.IsNotNull(hooks);
            Assert.IsInstanceOf<IEnumerable<Hook>>(hooks);
            Assert.IsTrue(hooks.Any());
        }

        [Test]
        public async Task Can_GetRepository_Hooks_WithRequestOptions()
        {
            int requestLimit = 1;
            var response = await bitBucketApiClient.Repositories.GetHooks(EXISTING_PROJECT, EXISTING_REPOSITORY, new RequestOptions { Limit = requestLimit });
            var hooks = response.Values;

            Assert.IsNotNull(hooks);
            Assert.IsInstanceOf<IEnumerable<Hook>>(hooks);
            Assert.AreEqual(requestLimit, hooks.Count());
        }

        [Test]
        public async Task Can_GetRepository_Hook_ById()
        {
            var response = await bitBucketApiClient.Repositories.GetHookById(EXISTING_PROJECT, EXISTING_REPOSITORY, EXISTING_HOOK);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<Hook>(response);
            Assert.AreEqual(EXISTING_HOOK, response.Details.Key);
        }

        #region Feature tests

        [Test]
        public async Task Can_GetBranchPermissions()
        {
            var response = await bitBucketApiClient.Branches.GetPermissions(EXISTING_PROJECT, EXISTING_REPOSITORY);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<ResponseWrapper<BranchPermission>>(response);
        }

        [Test]
        public async Task Can_SetBranchPermissions_Than_DeleteBranchPermissions()
        {
            BranchPermission setBranchPerm = new BranchPermission
            {
                Type = BranchPermissionType.READ_ONLY,
                Matcher = new BranchPermissionMatcher
                {
                    Id = "master",
                    DisplayId = "master",
                    Active = true,
                    Type = new BranchPermissionMatcherType
                    {
                        Id = BranchPermissionMatcherTypeName.BRANCH,
                        Name = "Branch"
                    }
                },
                Users = new List<User>(),
                Groups = new string[] { EXISTING_GROUP }
            };

            var response = await bitBucketApiClient.Branches.SetPermissions(EXISTING_PROJECT, EXISTING_REPOSITORY, setBranchPerm);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<BranchPermission>(response);
            Assert.AreEqual(setBranchPerm.Type, response.Type);
            Assert.AreEqual(setBranchPerm.Matcher.Id, response.Matcher.Id);
            Assert.AreEqual(setBranchPerm.Matcher.Type.Id, response.Matcher.Type.Id);
            Assert.IsTrue(response.Id > 0);

            await bitBucketApiClient.Branches.DeletePermissions(EXISTING_PROJECT, EXISTING_REPOSITORY, response.Id);
        }

        [Test]
        public async Task Can_SetBranchPermissions_Than_DeleteBranchPermissions_Using_Pattern()
        {
            BranchPermission setBranchPerm = new BranchPermission
            {
                Type = BranchPermissionType.READ_ONLY,
                Matcher = new BranchPermissionMatcher
                {
                    Id = "**",
                    DisplayId = "**",
                    Active = true,
                    Type = new BranchPermissionMatcherType
                    {
                        Id = BranchPermissionMatcherTypeName.PATTERN,
                        Name = "Pattern"
                    }
                },
                Users = new List<User>(),
                Groups = new string[] { EXISTING_GROUP }
            };

            var response = await bitBucketApiClient.Branches.SetPermissions(EXISTING_PROJECT, EXISTING_REPOSITORY, setBranchPerm);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<BranchPermission>(response);
            Assert.AreEqual(setBranchPerm.Type, response.Type);
            Assert.AreEqual(setBranchPerm.Matcher.Id, response.Matcher.Id);
            Assert.AreEqual(setBranchPerm.Matcher.Type.Id, response.Matcher.Type.Id);
            Assert.IsTrue(response.Id > 0);

            await bitBucketApiClient.Branches.DeletePermissions(EXISTING_PROJECT, EXISTING_REPOSITORY, response.Id);
        }

        [Test]
        public async Task Can_CreateProject_Than_DeleteProject()
        {
            Project newProject = new Project { Key = "ZTEST", Name = "Project of Integration tests", Description = "Project created by integration tests, please delete!" };
            var createdProject = await bitBucketApiClient.Projects.Create(newProject);

            Assert.IsNotNull(createdProject);
            Assert.IsInstanceOf<Project>(createdProject);
            Assert.AreEqual(newProject.Key.ToLower(), createdProject.Key.ToLower());

            await bitBucketApiClient.Projects.Delete(newProject.Key);
        }

        [Test]
        public async Task Can_CreateRepository_Than_DeleteRepository()
        {
            Repository newRepository = new Repository { Name = "Repository of Integration tests" };
            var createdRepository = await bitBucketApiClient.Repositories.Create(EXISTING_PROJECT, newRepository);

            Assert.IsNotNull(createdRepository);
            Assert.IsInstanceOf<Repository>(createdRepository);
            Assert.AreEqual(newRepository.Name.ToLower(), createdRepository.Name.ToLower());

            await bitBucketApiClient.Repositories.Delete(EXISTING_PROJECT, createdRepository.Slug);
        }

        [Test]
        public async Task Can_CreateBranch_Than_DeleteBranch()
        {
            Branch newBranch = new Branch { Name = "test-repo", StartPoint = EXISTING_BRANCH_REFERENCE };
            var createdBranch = await bitBucketApiClient.Branches.Create(EXISTING_PROJECT, EXISTING_REPOSITORY, newBranch);

            Assert.IsNotNull(createdBranch);
            Assert.IsInstanceOf<Branch>(createdBranch);
            Assert.AreEqual(newBranch.Name.ToLower(), createdBranch.DisplayId.ToLower());


            Branch deleteBranch = new Branch { Name = newBranch.Name, DryRun = false };

            await bitBucketApiClient.Branches.Delete(EXISTING_PROJECT, EXISTING_REPOSITORY, deleteBranch);
        }

        [Test]
        public async Task Can_Get_Enable_And_Disable_Hook()
        {
            var enableHook = await bitBucketApiClient.Repositories.EnableHook(EXISTING_PROJECT, EXISTING_REPOSITORY, EXISTING_HOOK);

            Assert.IsNotNull(enableHook);
            Assert.IsTrue(enableHook.Enabled);
            Assert.IsInstanceOf<Hook>(enableHook);
            Assert.AreEqual(EXISTING_HOOK, enableHook.Details.Key);

            var disableHook = await bitBucketApiClient.Repositories.DisableHook(EXISTING_PROJECT, EXISTING_REPOSITORY, EXISTING_HOOK);

            Assert.IsNotNull(disableHook);
            Assert.IsFalse(disableHook.Enabled);
            Assert.IsInstanceOf<Hook>(disableHook);
            Assert.AreEqual(EXISTING_HOOK, disableHook.Details.Key);
        }

        [Test]
        public async Task Can_Get_Then_Create_Then_Grant_Access_To_Project_And_Delete_User()
        {
            #region Setup/Clean up
            var existingTestUsers = await bitBucketApiClient.Users.Get("tmpTestUser");

            foreach (var existingUser in existingTestUsers.Values)
            {
                await bitBucketApiClient.Users.Delete(existingUser.Name);
            }
            #endregion

            await bitBucketApiClient.Users.Create("tmpTestUser", "Temporary test user", "tmpUser@company.com", "password");

            await bitBucketApiClient.Projects.GrantUser(EXISTING_PROJECT, "tmpTestUser", ProjectPermissions.PROJECT_ADMIN);

            var deletedUser = await bitBucketApiClient.Users.Delete("tmpTestUser");

            Assert.IsNotNull(deletedUser);
            Assert.IsInstanceOf<User>(deletedUser);
            Assert.AreEqual("tmpTestUser", deletedUser.Name);
        }

        #endregion
    }
}
