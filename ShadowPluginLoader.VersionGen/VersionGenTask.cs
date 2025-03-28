using Microsoft.Build.Framework;

namespace ShadowPluginLoader.VersionGen;

/// <summary>
/// 
/// </summary>
public class VersionGenTask : Microsoft.Build.Utilities.Task
{
    /// <summary>
    /// 
    /// </summary>
    public string BuildNumberFile { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public string BuildDateFile { get; set; } = null!;
    /// <summary>
    /// 
    /// </summary>
    public string Configuration { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [Output]
    public string NewBuildVersion { get; set; } = null!;

    public override bool Execute()
    {
        try
        {
            var buildNumber = 0;
            if (Path.GetDirectoryName(BuildNumberFile) is { } s)Directory.CreateDirectory(s);
            if (Path.GetDirectoryName(BuildDateFile) is { } ds)Directory.CreateDirectory(ds);

            if (File.Exists(BuildNumberFile))
            {
                var content = File.ReadAllText(BuildNumberFile).Trim();
                buildNumber = int.TryParse(content, out buildNumber) ? buildNumber : 0;
            }


            var buildDate = DateTime.Now;
            if (File.Exists(BuildDateFile))
            {
                var content = File.ReadAllText(BuildDateFile).Trim();
                buildDate = DateTime.TryParse(content, out buildDate) ? buildDate : DateTime.Now;
            }

            if (Configuration != "Debug")
            {
                NewBuildVersion = $"{buildDate:yyyy.MM.dd}.{buildNumber}";
                return true;
            }
            if (!buildDate.Equals(DateTime.Now.Date))
            {
                buildDate = DateTime.Now;
                buildNumber = 0;
            }
            var date = buildDate.ToString("yyyy.MM.dd");
            buildNumber++;
            File.WriteAllText(BuildNumberFile, buildNumber.ToString());
            File.WriteAllText(BuildDateFile, date);
            NewBuildVersion = $"{date}.{buildNumber}";
            return true;
        }
        catch (Exception ex)
        {
            Log.LogError($"Error incrementing build number: {ex.Message}");
            return false;
        }
    }
}