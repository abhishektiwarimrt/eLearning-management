namespace lms.buildingblocks.RequestResponse
{
    public class ApiResponse<T>
    {
        public string Status { get; set; } = null!;
        public required T Data { get; set; }
        public required Metadata Metadata { get; set; }
        public string Error { get; set; } = null!;
    }

    public class Metadata
    {
        public DateTime Timestamp { get; set; }
        public required string Version { get; set; }
    }

}
