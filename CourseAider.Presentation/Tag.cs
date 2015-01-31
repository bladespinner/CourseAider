using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CourseAider.Presentation
{
    public class Tag : IDisposable
    {
        private readonly ViewContext _viewContext;
        private bool _disposed;
        private string _tagName;
        private bool _started;
        private TagBuilder _tagBulder;

        public Tag(ViewContext viewContext, string tagName)
        {
            if (viewContext == null)
            {
                throw new ArgumentNullException("viewContext");
            }
            if (tagName == null)
            {
                throw new ArgumentNullException("tagName");
            }
            this._tagName = tagName;
            this._viewContext = viewContext;
            this._started = false;
        }
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                _started = false;
                this._disposed = true;
                if (_tagBulder != null)
                {
                    this._viewContext.Writer.Write(_tagBulder.ToString(TagRenderMode.EndTag));
                }
            }
        }
        public void OpenTag(IDictionary<string,object> htmlAttributes)
        {
            if (_disposed)
            {
                throw new InvalidOperationException("Tag object already disposed");
            }
            if (_started)
            {
                throw new InvalidOperationException("Cannot open tag, tag already opened");
            }
            _started = true;

            _tagBulder = new TagBuilder(_tagName);
            _tagBulder.MergeAttributes<string, object>(htmlAttributes);
            this._viewContext.Writer.Write(_tagBulder.ToString(TagRenderMode.StartTag));
        }
        public void EndTag()
        {
            this.Dispose(true);
        }
    }
}