using System.IO;

namespace TCS.Web.OpsPortal
{
    public class Settings
    {
        public string Env { get; set; }
        public LegacyBatchSubmitSettings LegacyBatchSubmit { get; set; }
        public string JobsFolder { get; set; }
        public string GlobalSchedulerRestServiceBaseAddress { get; set; }
        public int MaxRecordsPerCsvBatch { get; set; }
        public int DeleteCsvSchedulesAfterXDays { get; set; }
        public int DeleteArchivedCsvsAfterXDays { get; set; }
        public int AutoRefreshTimeInMilliseconds { get; set; }

        public string BatchFolder => Path.Combine(JobsFolder, "Batch");
        public string CsvFolder => Path.Combine(JobsFolder, "Csv");
        public string CsvArchivedFolder => Path.Combine(CsvFolder, "Archived");
        public string CsvSchedulesFolder => Path.Combine(JobsFolder, "CsvSchedules");
        public string CsvSchedulesArchivedFolder => Path.Combine(CsvSchedulesFolder, "Archived");

        public ConnectionStringsSettings ConnectionStrings { get; set; }
        public VersionEndpointSetting[] VersionEndpoints { get; set; }

        public class ConnectionStringsSettings
        {
            public string MonolithicDB { get; set; }
            public string HangfireComposition { get; set; }
            public string HangfireDelivery { get; set; }
        }

        public class LegacyBatchSubmitSettings
        {
            public string RunDataFolder { get; set; }
            public string InfoFileName { get; set; }
            public string DataFileName { get; set; }
            public string ErrorFileName { get; set; }
        }

        public class VersionEndpointSetting
        {
            public string Name { get; set; }
            public string Url { get; set; }
        }
    }
}
