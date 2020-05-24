var target = Argument ("target", "Default");

var AAR_VERSION = "0.3.4";

var NUGET_VERSION = AAR_VERSION;

var AAR_URL = $"https://jitpack.io/com/github/kizitonwose/CalendarView/{AAR_VERSION}/CalendarView-{AAR_VERSION}.aar";

Task ("Externals")
	.WithCriteria (!FileExists ("./externals/CalendarView.aar"))
	.Does (() =>
{
	EnsureDirectoryExists ("./externals");

	DownloadFile (AAR_URL, "./externals/CalendarView.aar");

	XmlPoke("./CalendarView/CalendarView.csproj", "/Project/PropertyGroup/PackageVersion", NUGET_VERSION);
});

Task("Libs")
	.IsDependentOn("Externals")
	.Does(() =>
{
	MSBuild("./CalendarView.sln", c => {
		c.Configuration = "Release";
		c.Restore = true;
		c.MaxCpuCount = 0;
		c.Properties.Add("DesignTimeBuild", new [] { "false" });
	});
});

Task("Nuget")
	.IsDependentOn("Libs")
	.Does(() =>
{
	MSBuild ("./CalendarView.sln", c => {
		c.Configuration = "Release";
		c.MaxCpuCount = 0;
		c.Targets.Clear();
		c.Targets.Add("Pack");
		c.Properties.Add("PackageOutputPath", new [] { MakeAbsolute(new FilePath("./output")).FullPath });
		c.Properties.Add("PackageRequireLicenseAcceptance", new [] { "true" });
		c.Properties.Add("DesignTimeBuild", new [] { "false" });
	});
});

Task ("Clean")
	.Does (() =>
{
	if (DirectoryExists ("./externals/"))
		DeleteDirectory ("./externals", new DeleteDirectorySettings {
			Recursive = true,
			Force = true
		});
});

Task ("Default")
    .IsDependentOn("Clean")
    .IsDependentOn("Nuget")
    .Does(() => {});

RunTarget (target);