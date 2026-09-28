using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace CreamInstaller;

public class ProgramRelease
{
    private Asset asset;

    private string[] changes;

    private Version version;

    [JsonProperty("tag_name", NullValueHandling = NullValueHandling.Ignore)]
    public string TagName { get; set; }

    [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
    public string Name { get; set; }

    [JsonProperty("draft", NullValueHandling = NullValueHandling.Ignore)]
    public bool Draft { get; set; }

    [JsonProperty("prerelease", NullValueHandling = NullValueHandling.Ignore)]
    public bool Prerelease { get; set; }

    [JsonProperty("assets", NullValueHandling = NullValueHandling.Ignore)]
    public List<Asset> Assets { get; } = new();

    [JsonProperty("body", NullValueHandling = NullValueHandling.Ignore)]
    public string Body { get; set; }

    public Asset Asset => asset ??= Assets.FirstOrDefault(a => a.Name == Program.RepositoryPackage);

    public Version Version => version ??= ParseVersion(TagName);

    public string[] Changes => changes ??= (Body ?? "").Replace("- ", "")
        .Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);

    // Accepts tags like "v5", "v5.0" or "v5.0.2.3"; returns null for tags that aren't versions.
    private static Version ParseVersion(string tag)
    {
        string text = tag?.TrimStart('v', 'V') ?? "";
        if (!text.Contains('.'))
            text += ".0";
        return Version.TryParse(text, out Version parsed) ? parsed : null;
    }
}

public class Asset
{
    [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
    public string Name { get; set; }

    [JsonProperty("size", NullValueHandling = NullValueHandling.Ignore)]
    public int Size { get; set; }

    [JsonProperty("browser_download_url", NullValueHandling = NullValueHandling.Ignore)]
    public string BrowserDownloadUrl { get; set; }
}