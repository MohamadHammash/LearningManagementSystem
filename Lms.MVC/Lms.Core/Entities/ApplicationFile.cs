using System;

namespace Lms.MVC.Core.Entities
{
    public class ApplicationFile
    {
        public int Id { get; set; }

        public byte[] Content { get; set; }

        public string UntrustedName { get; set; }

        public string Description { get; set; }

        public long Size { get; set; }

        public DateTime UploadDT { get; set; }

        public bool Assignment { get; set; }
    }
}