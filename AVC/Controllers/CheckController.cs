using LibGit2Sharp;
using LibGit2Sharp.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckController : ControllerBase
    {
        [HttpGet]
        public ActionResult checkConnection()
        {
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> TestZipAsync(IFormFile zip)
        {
            if (zip.Length > 0)
            {
                try
                {
                    var branchName = "testBranch";

                    string filePath = Path.Combine("D:/GitPushCheck", zip.FileName);
                    CloneOptions cloneOption = new CloneOptions();
                    cloneOption.CredentialsProvider = new CredentialsHandler(
                            (url, usernameFromUrl, types) =>
                                new UsernamePasswordCredentials()
                                {
                                    Username = "huytmse130336@fpt.edu.vn",
                                    Password = "kayw0n69"
                                });
                    Repository.Clone("https://gitlab.com/huytmse130336/testgitpush", @"D:\GitPushCheck", cloneOption);
                    using (var repo = new Repository(@"D:\GitPushCheck"))
                    using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        Remote remote = repo.Network.Remotes["origin"];
                        //repo.Network.Remotes.Add(branchName, "https://gitlab.com/huytmse130336/testgitpush/-/tree/" + branchName);

                        var localBranch = repo.Branches[branchName];

                        if (localBranch == null)
                        {
                            localBranch = repo.CreateBranch(branchName);
                        }

                        Commands.Checkout(repo, localBranch);

                        repo.Branches.Update(localBranch, b => b.Remote = remote.Name, b => b.UpstreamBranch = localBranch.CanonicalName);

                        PushOptions pushOptions = new PushOptions();
                        pushOptions.CredentialsProvider = new CredentialsHandler(
                             (url, usernameFromUrl, types) =>
                                    new UsernamePasswordCredentials
                                    {
                                        Username = "huytmse130336@fpt.edu.vn",
                                        Password = "kayw0n69"
                                    });

                        repo.Network.Push(localBranch, pushOptions);

                        fileStream.Position = 0;
                        await zip.CopyToAsync(fileStream);
                        //fileStream.EndWrite();
                        fileStream.Flush();
                        fileStream.Dispose();
                        fileStream.Close();

                        Commands.Stage(repo, "*");

                        // Create the committer's signature and commit
                        Signature author = new Signature("Kay", "huytmse130336@fpt.edu.vn", DateTime.UtcNow.AddHours(7));
                        Signature committer = author;

                        // Commit to the repository
                        Commit commit = repo.Commit("Here's a commit i made!", author, committer);



                        repo.Network.Push(localBranch, pushOptions);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    var dir = new DirectoryInfo("D:/GitPushCheck");
                    dir.Delete(true);
                }
            }


            return Ok();
        }
    }
}
