namespace CUE4Parse2UEAT
{
    public class ExporterConfig
    {
        public string ExportDirectory { get; protected set; }

        protected ExporterConfig(string exportDirectory)
        {
            ExportDirectory = exportDirectory;
        }

        public class Builder
        {
            private string _exportDirectory = string.Empty;

            public Builder() { }

            public ExporterConfig Build()
            {
                return new ExporterConfig(_exportDirectory);
            }

            public Builder SetExportDirectory(string exportDirectory)
            {
                _exportDirectory = exportDirectory;
                return this;
            }
        }
    }
}
