using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.NerdbankGitVersioning;
using Nuke.Common.Tools.DotNet;
using Serilog;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using CP.BuildTools;
using Nuke.Common.Tools.MSBuild;
using System;

////[GitHubActions(
////    "BuildOnly",
////    GitHubActionsImage.WindowsLatest,
////    OnPushBranchesIgnore = new[] { "master" },
////    FetchDepth = 0,
////    InvokedTargets = new[] { nameof(Compile) })]
////[GitHubActions(
////    "BuildDeploy",
////    GitHubActionsImage.WindowsLatest,
////    OnPushBranches = new[] { "master" },
////    FetchDepth = 0,
////    ImportSecrets = new[] { nameof(NuGetApiKey) },
////    InvokedTargets = new[] { nameof(Compile), nameof(Deploy) })]
partial class Build : NukeBuild
{    
    public static int Main() => Execute<Build>(x => x.Pack);

    [GitRepository] readonly GitRepository Repository;
    [Solution(GenerateProjects = true)] readonly Solution Solution;
    [NerdbankGitVersioning] readonly NerdbankGitVersioning NerdbankVersioning;
    [Parameter][Secret] readonly string NuGetApiKey;
    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    AbsolutePath PackagesDirectory => RootDirectory / "output";

    Target Print => _ => _
        .Executes(() => Log.Information("NerdbankVersioning = {Value}", NerdbankVersioning.NuGetPackageVersion));

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            if (IsLocalBuild)
            {
                return;
            }

            PackagesDirectory.CreateOrCleanDirectory();
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetWorkloadRestore(s => s.DisableSkipManifestUpdate().SetProject(Solution));
            return DotNetRestore(s => s.SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore, Print)
        .Executes(() => DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetNoRestore(true)));

    Target Pack => _ => _
    .DependsOn(Compile)
    .Produces(PackagesDirectory / "*.nupkg")
    .Executes(() =>
    {
        if (Repository.IsOnMainOrMasterBranch())
        {
            var packableProjects = Solution.GetPackableProjects();
            foreach (var project in packableProjects!)
            {
                Log.Information("Packing {Project}", project.Name);
            }

            DotNetPack(settings => settings
                .SetConfiguration(Configuration)
                .SetNoBuild(true)
                .SetVersion(NerdbankVersioning.NuGetPackageVersion)
                .SetOutputDirectory(PackagesDirectory)
                .CombineWith(packableProjects, (packSettings, project) =>
                    packSettings.SetProject(project)));
        }
    });

    Target Deploy => _ => _
    .Requires(() => NuGetApiKey)
    .Executes(() =>
    {
        if (Repository.IsOnMainOrMasterBranch())
        {
            DotNetNuGetPush(settings => settings
                        .SetSource(this.PublicNuGetSource())
                        .SetSkipDuplicate(true)
                        .SetApiKey(NuGetApiKey)
                        .CombineWith(PackagesDirectory.GlobFiles("*.nupkg"), (s, v) => s.SetTargetPath(v)),
                    degreeOfParallelism: 5, completeOnFailure: true);
        }
    });
}
