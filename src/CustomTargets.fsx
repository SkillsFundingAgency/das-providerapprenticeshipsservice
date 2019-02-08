open Fake

Target "Dotnet Restore" (fun _ ->
    DotNetCli.Restore(fun p ->
        { p with
                Project = ".\\SFA.DAS.ProviderUrlHelper" })
    DotNetCli.Restore(fun p ->
        { p with
                Project = ".\\SFA.DAS.ProviderUrlHelperTests" })
)