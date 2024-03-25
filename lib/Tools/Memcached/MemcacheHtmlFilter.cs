// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemcacheHtmlFilter.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   Defines the MemcacheFunctions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.Tools.Memcached
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web;

    /// <summary>
    /// The memory cache html filter.
    /// </summary>
    public class MemcacheHtmlFilter : System.IO.Stream
    {
        /// <summary>
        /// The m check no cache blocks.
        /// </summary>
        private const bool MCheckNoCacheBlocks = false;

        /// <summary>
        /// The m hash table data.
        /// </summary>
        private readonly System.Collections.Hashtable hashTableData;

        /// <summary>
        /// The m response html.
        /// </summary>
        private readonly StringBuilder responseHtml;

        /// <summary>
        /// The base.
        /// </summary>
        private readonly System.IO.Stream Base;

        /// <summary>
        /// The m cache id.
        /// </summary>
        private readonly string cacheId = string.Empty;

        /// <summary>
        /// The m sub key.
        /// </summary>
        private readonly string subKey = string.Empty;

        /// <summary>
        /// The group names.
        /// </summary>
        private readonly string groupNames = string.Empty;

        /// <summary>
        /// The stored session names.
        /// </summary>
        private readonly string storedSessionNames = string.Empty;

        /// <summary>
        /// The expiry.
        /// </summary>
        private readonly DateTime expiry;

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MemcacheHtmlFilter"/> class.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="cacheId">
        /// The cache id.
        /// </param>
        public MemcacheHtmlFilter(System.Web.UI.Page page, string cacheId)
            : this(page, cacheId, string.Empty, MemcacheGroup.GroupAll, string.Empty, DateTime.Now.AddHours(24))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemcacheHtmlFilter"/> class.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="cacheId">
        /// The cache id.
        /// </param>
        /// <param name="groupNames">
        /// The group names.
        /// </param>
        public MemcacheHtmlFilter(System.Web.UI.Page page, string cacheId, string groupNames)
            : this(page, cacheId, string.Empty, groupNames, string.Empty, DateTime.Now.AddHours(24))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemcacheHtmlFilter"/> class.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="cacheId">
        /// The cache id.
        /// </param>
        /// <param name="subKey">
        /// The sub key.
        /// </param>
        /// <param name="groupNames">
        /// The group names.
        /// </param>
        /// <param name="sessionNames">
        /// The session names.
        /// </param>
        public MemcacheHtmlFilter(System.Web.UI.Page page, string cacheId, string subKey, string groupNames, string sessionNames)
            : this(page, cacheId, subKey, groupNames, sessionNames, DateTime.Now.AddHours(24))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemcacheHtmlFilter"/> class.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="cacheId">
        /// The cache id.
        /// </param>
        /// <param name="subKey">
        /// The sub key.
        /// </param>
        /// <param name="groupNames">
        /// The group names.
        /// </param>
        /// <param name="expiry">
        /// The expiry.
        /// </param>
        public MemcacheHtmlFilter(System.Web.UI.Page page, string cacheId, string subKey, string groupNames, DateTime expiry)
            : this(page, cacheId, subKey, groupNames, string.Empty, expiry)
        {
        }

        #endregion
        
        /// <summary>
        /// Gets a value indicating whether is cached.
        /// </summary>
        public bool IsCached
        {
            get
            {
                if (null != this.hashTableData && 0 != this.hashTableData.Count
                    && null != this.hashTableData["Buffer"])
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemcacheHtmlFilter"/> class.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="cacheId">
        /// The cache id.
        /// </param>
        /// <param name="subKey">
        /// The sub key.
        /// </param>
        /// <param name="groupNames">
        /// The group names.
        /// </param>
        /// <param name="sessionNames">
        /// The session names.
        /// </param>
        /// <param name="expiry">
        /// The expiry.
        /// </param>
        public MemcacheHtmlFilter(System.Web.UI.Page page, string cacheId, string subKey, string groupNames, string sessionNames, DateTime expiry)
        {
            if (null == (new HttpContextWrapper(HttpContext.Current)).Response.Filter)
            {
                throw new ArgumentNullException("System.Web.new HttpContextWrapper(HttpContext.Current).Response.Filter");
            }

            this.Base = new HttpContextWrapper(HttpContext.Current).Response.Filter;
            this.cacheId = cacheId;
            this.subKey = subKey;
            this.groupNames = groupNames;
            this.expiry = expiry;
            this.storedSessionNames = sessionNames;
            this.responseHtml = new StringBuilder(30000);

            this.hashTableData = (System.Collections.Hashtable)MemcacheFunctions.GetCachedItem(this.cacheId, this.subKey, this.groupNames);
            if (null == this.hashTableData)
            {
                this.hashTableData = new System.Collections.Hashtable(20);
            }
            if (this.IsCached)
            {
                var sessionNamesWorker = sessionNames.Split(",".ToCharArray());
                foreach (var t in sessionNamesWorker)
                {
                    var session = new HttpContextWrapper(HttpContext.Current).Session;
                    if (session != null)
                    {
                        session[t] = this.hashTableData[t];
                    }
                }
            }
            else 
            {
                page.Unload += this.PagePreRenderComplete;
            }

            (new HttpContextWrapper(HttpContext.Current)).Response.Filter = this;
        }

        /// <summary>
        /// The write.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="offset">
        /// The offset.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (this.IsCached)
            {
                buffer = (byte[])this.hashTableData["Buffer"];
                this.Base.Write(buffer, 0, buffer.Length);
            }
            else
            {
                var strBuffer = Encoding.UTF8.GetString(buffer, offset, count);

                // ---------------------------------
                // Wait for the closing </html> tag or <!--EOF-->
                // ---------------------------------
                var eof = new System.Text.RegularExpressions.Regex("</html>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                var eof1 = new System.Text.RegularExpressions.Regex("<!--EOF-->", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                if (!eof.IsMatch(strBuffer) && !eof1.IsMatch(strBuffer))
                {
                    this.responseHtml.Append(strBuffer);
                }
                else
                {
                    this.responseHtml.Append(strBuffer);
                    string finalHtml = this.responseHtml.ToString();

                    // Transform the response and write it back out
                    var writeData = Encoding.UTF8.GetBytes(finalHtml);
                    var cacheData = writeData;
                    var html = finalHtml;
                    if (MCheckNoCacheBlocks)
                    {
                        var cleared = true;
                        while (cleared)
                        {
                            cleared = false;
                            var startBlk = html.IndexOf("<!--START NO CACHE -->", StringComparison.Ordinal);
                            var endBlk = html.IndexOf("<!--END NO CACHE -->", StringComparison.Ordinal);
                            if (-1 == startBlk || -1 == endBlk || startBlk >= endBlk)
                            {
                                continue;
                            }

                            html = html.Substring(0, startBlk) + html.Substring(endBlk + 20);
                            cleared = true;
                        }
                        finalHtml = html;
                        cacheData = Encoding.UTF8.GetBytes(finalHtml);
                    }
                    this.hashTableData.Add("Buffer", cacheData);
                    MemcacheFunctions.SetCachedItem(this.cacheId, this.subKey, this.hashTableData, this.groupNames, this.expiry);

                    this.Base.Write(writeData, 0, writeData.Length);
                }
            }
        }

        /// <summary>
        /// The page pre render complete.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void PagePreRenderComplete(object sender, System.EventArgs e)
        {
            this.SetSession();
        }


        /// <summary>
        /// The set session.
        /// </summary>
        private void SetSession()
        {
            if (!this.IsCached)
            {
                var sessionNamesWorker = this.storedSessionNames.Split(",".ToCharArray());
                var session = (new HttpContextWrapper(HttpContext.Current)).Session;
                foreach (var t in sessionNamesWorker.Where(t => session != null && (null != session[t] && !this.hashTableData.Contains(t))))
                {
                    this.hashTableData.Add(t, session[t]);
                }
            }
        }

        #region override

        /// <summary>
        /// Gets the can read.
        /// </summary>
        public override bool CanRead
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether can seek.
        /// </summary>
        public override bool CanSeek
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether can write.
        /// </summary>
        public override bool CanWrite
        {
            get { return true; }
        }

        /// <summary>
        /// The flush.
        /// </summary>
        public override void Flush()
        {
        }

        /// <summary>
        /// Gets the length.
        /// </summary>
        public override long Length
        {
            get { return 0; }
        }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        public override long Position
        {
            get { return 0; }
            set { }
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="offset">
        /// The offset.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return this.Base.Read(buffer, offset, count);
        }

        /// <summary>
        /// The seek.
        /// </summary>
        /// <param name="offset">
        /// The offset.
        /// </param>
        /// <param name="origin">
        /// The origin.
        /// </param>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return 0;
        }

        /// <summary>
        /// The set length.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public override void SetLength(long value)
        {
        }
        #endregion
    }
}