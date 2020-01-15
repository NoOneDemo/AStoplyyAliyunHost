namespace AliyunOpenApiJob.AliyunResponseModel
{
    public class DescribeDomainRecordsResponse
    {
        public string RequestId { get; set; }

        public long? TotalCount { get; set; }

        public long? PageNumber { get; set; }

        public long? PageSize { get; set; }

        public Record[] DomainRecords { get; set; }
    }
    public class Record
    {

        public string DomainName { get; set; }

        public string RecordId { get; set; }

        public string RR { get; set; }

        public string Type { get; set; }

        public string Value { get; set; }

        public long? TTL { get; set; }

        public long? Priority { get; set; }

        public string Line { get; set; }

        public string Status { get; set; }

        public bool? Locked { get; set; }

        public int? Weight { get; set; }
    }
}