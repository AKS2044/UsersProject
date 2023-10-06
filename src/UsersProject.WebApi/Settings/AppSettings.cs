namespace UsersProject.WebApi.Settings
{
    /// <summary>
    /// App settings.
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// Secret key for Jwt generation.
        /// </summary>
        public string? Secret { get; set; }
    }
}