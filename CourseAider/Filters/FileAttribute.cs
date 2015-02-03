using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CourseAider.Filters
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class FileAttribute : ValidationAttribute
    {
        private int _fileSize = -1;
        public string[] FileTypes { get; set; }
        public int FileSize
        {
            get
            {
                return _fileSize;
            }
            set
            {
                _fileSize = value;
            }
        }
        public FileAttribute(): this("{0} must be a file")
        {

        }
        public FileAttribute(string errorMessage) : base(errorMessage) { }

        public override string FormatErrorMessage(string name)
        {
            string fileTypes = FileTypes == null ? "" : FileTypes.Aggregate((a, b) => a + "," + b);
            return String.Format(ErrorMessageString, name, _fileSize, fileTypes);
        }

        public override bool IsValid(object value)
        {
            var file = value as HttpPostedFileWrapper;
            if (file == null) return false;

            if(_fileSize > 0)
            {
                if (file.ContentLength > _fileSize) return false;
            }
            var extention = file.FileName.Split('.').Last().ToLower();
            if (FileTypes != null && FileTypes.Length > 0)
            {
                if (!FileTypes.Contains(extention))
                {
                    return false;
                }
            }

            return true;
        }
    }
}