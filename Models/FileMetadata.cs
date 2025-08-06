namespace GraphFileTransferSdk.Models
{
    // Represents metadata for a file involved in transfer operations
    public class FileMetadata
    {
        public string Name { get; set; }          // Original name of the file
        public string Id { get; set; }            // Unique identifier in Graph
        public long Size { get; set; }            // Size in bytes
        public string DownloadedAs { get; set; }  // Local filename after download
    }
}
