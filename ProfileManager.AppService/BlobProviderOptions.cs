namespace ProfileManager.AppService
{
    // options feel very bound to an implementation, perhaps they should be named stronger? like AzureStorageBlobOptions? I guess that wouldn't work well for injecting options.
    public class BlobProviderOptions
    {
        public string ConnectionString { get; set; }
        public string UploadContainer { get; set; }
        public string ProcessedContainer { get; set; }
    }
}
