// CA1852 Type 'Program' can be sealed because it has no subtypes in its containing assembly and is not externally visible
// TODO: Remove this when the following issue is addressed as this is a workaround: https://github.com/dotnet/roslyn-analyzers/issues/6141
#pragma warning disable CA1852

using CliFx;

return await new CliApplicationBuilder()
    .SetTitle("Style")
    .SetExecutableName("dotnet style")
    .SetDescription("A customizable dotnet tool that helps maintain a consistent C# coding style.")
    .AddCommandsFromThisAssembly()
    .Build()
    .RunAsync();
