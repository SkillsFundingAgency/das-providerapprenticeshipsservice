open Fake

Target "Dotnet Restore" (fun _ ->
    DotNetCli.Restore(fun p ->
        { p with
                Project = ".\\SFA.DAS.ProviderUrlHelper" })
    DotNetCli.Restore(fun p ->
        { p with
                Project = ".\\SFA.DAS.ProviderUrlHelperTests" })
    DotNetCli.Restore(fun p ->
        { p with
                Project = ".\\SFA.DAS.PAS.Account.Api.ClientV2" })
    DotNetCli.Restore(fun p ->
        { p with
                Project = ".\\SFA.DAS.PAS.Account.Api.Client.UnitTests" })
)