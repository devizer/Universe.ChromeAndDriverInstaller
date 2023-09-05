namespace ValidateAndGrabHash.LinksValidator;

public static class Env
{
    public static string ChromeDownloadDir => Environment.GetEnvironmentVariable("CHROMEDRIVER_DOWNLOAD_DIR");
}