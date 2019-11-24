using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;

namespace System.IO
{
    public partial class DelayFileSystemWatcher : Component
    {
        public event EventHandler<DelayFileChangeEventArgs> FileChanged;

        public DelayFileSystemWatcher()
        {
            InitializeComponent();
            InitDefaultValue();
        }
        public DelayFileSystemWatcher(string path)
        {
            InitializeComponent();
            InitDefaultValue();
            this.Path = path;
        }
        public DelayFileSystemWatcher(string path, string filter)
        {
            InitializeComponent();
            InitDefaultValue();
            this.Path = path;
            this.Filter = filter;
        }
        public DelayFileSystemWatcher(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
            InitDefaultValue();
        }
        private void InitDefaultValue()
        {
            this.IncludeSubdirectories = false;
            this.Interval = 500;
            this.Filter = this.IgnoreFilter = "";
            this.MergerKind = FileChangeEventMergerKind.FileName;
            this.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.DirectoryName;
        }

        public string Path
        {
            get { return this.fileSystemWatcher1.Path; }
            set { this.fileSystemWatcher1.Path = value; }
        }
        /// <summary>
        /// 获取或设置过滤器，例如 *.rar ,有多种类型时用字符‘|’分割，例如 *.rar|*.zip
        /// </summary>
        [DefaultValue("")]
        public string Filter
        {
            get;
            set;
        }
        [DefaultValue("")]
        public string IgnoreFilter
        {
            get;
            set;
        }
        /// <summary>
        /// 获取或设置一个值，该值指示是否监视指定路径中的子目录。
        /// </summary>
        [DefaultValue(false)]
        public bool IncludeSubdirectories
        {
            get { return this.fileSystemWatcher1.IncludeSubdirectories; }
            set { this.fileSystemWatcher1.IncludeSubdirectories = value; }
        }
        [DefaultValue(500)]
        public double Interval
        {
            get { return this.timer1.Interval; }
            set { this.timer1.Interval = value; }
        }

        public void Start()
        {
            lock (this)
            {
                this.cacheItems.Clear();
            }
            this.fileSystemWatcher1.EnableRaisingEvents = true;
            this.timer1.Start();
        }

        public void Stop()
        {
            this.fileSystemWatcher1.EnableRaisingEvents = false;
            this.timer1.Stop();
        }
        [DefaultValue(typeof(NotifyFilters), "FileName, DirectoryName, LastWrite")]
        public NotifyFilters NotifyFilter
        {
            get { return this.fileSystemWatcher1.NotifyFilter; }
            set { this.fileSystemWatcher1.NotifyFilter = value; }
        }
        [DefaultValue(typeof(FileChangeEventMergerKind), "FileName")]
        public FileChangeEventMergerKind MergerKind { get; set; }
        private List<DelayFileChangeEventArgs> cacheItems = new List<DelayFileChangeEventArgs>();

        private void fileSystemWatcher1_Changed(object sender, IO.FileSystemEventArgs e)
        {
            if (IsMatch(e.FullPath, this.Filter, this.IgnoreFilter))
            {
                lock (this)
                {
                    cacheItems.Add(new DelayFileChangeEventArgs(DateTime.Now, e.ChangeType, e.FullPath, e.Name));
                }
                this.timer1.Stop();
                this.timer1.Start();
            }
        }
        private bool IsMatch(string file, string filter, string ignoreFilter)
        {
            if (!string.IsNullOrEmpty(ignoreFilter))
            {
                foreach (var s in ignoreFilter.Split('|'))
                {
                    if (s.Length == 0) continue;
                    if (file.IsMatchWildcard(s, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return false;
                    }
                }
            }

            if (string.IsNullOrEmpty(filter))
            {
                return true;
            }
            {
                foreach (var s in filter.Split('|'))
                {
                    if (s.Length == 0) continue;
                    if (file.IsMatchWildcard(s, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        private void fileSystemWatcher1_Renamed(object sender, IO.RenamedEventArgs e)
        {
            if (IsMatch(e.FullPath, this.Filter, this.IgnoreFilter))
            {
                lock (this)
                {
                    cacheItems.Add(new DelayFileChangeEventArgs(DateTime.Now, e.ChangeType, e.FullPath, e.Name, e.OldFullPath, e.OldName));
                }
                this.timer1.Stop();
                this.timer1.Start();
            }
        }

        private void timer1_Elapsed(object sender, ElapsedEventArgs e)
        {

            Dictionary<string, DelayFileChangeEventArgs> ht = new Dictionary<string, DelayFileChangeEventArgs>(StringComparer.InvariantCultureIgnoreCase);
            lock (this)
            {
                foreach (var v in cacheItems)
                {
                    string key = GetKey(v);
                    if (!ht.ContainsKey(key))
                    {
                        ht[key] = v;
                    }
                }
                this.cacheItems.Clear();
            }
            foreach (var v in ht.Values)
            {
                this.OnFileChanged(v);
            }

        }

        private string GetKey(DelayFileChangeEventArgs e)
        {
            switch (this.MergerKind)
            {
                case FileChangeEventMergerKind.None:
                    return Guid.NewGuid().ToString();
                case FileChangeEventMergerKind.FileName:
                    return e.FullPath;
                case FileChangeEventMergerKind.FileNameAndChangeType:
                    return string.Format("{0}_{1}", e.ChangeType, e.FullPath);
                default:
                    return Guid.NewGuid().ToString();
            }
        }

        protected virtual void OnFileChanged(DelayFileChangeEventArgs e)
        {
            if (this.FileChanged != null)
            {
                this.FileChanged(this, e);
            }
        }
    }

    public enum FileChangeEventMergerKind
    {
        None,
        FileName,
        FileNameAndChangeType,
    }

    [Serializable]
    public class DelayFileChangeEventArgs : EventArgs
    {
        internal DelayFileChangeEventArgs(DateTime changeTime, WatcherChangeTypes changeType, string fullPath, string name)
        {
            this.ChangeTime = changeTime;
            this.ChangeType = changeType;
            this.Name = name;
            this.FullPath = fullPath;
        }
        internal DelayFileChangeEventArgs(DateTime changeTime, WatcherChangeTypes changeType, string fullPath, string name, string oldfullPath, string oldName)
        {
            this.ChangeTime = changeTime;
            this.ChangeType = changeType;
            this.Name = name;
            this.FullPath = fullPath;
            this.OldFullPath = oldfullPath;
            this.OldName = oldName;
        }
        public WatcherChangeTypes ChangeType { get; private set; }
        //
        // 摘要:
        //     获取受影响的文件或目录的完全限定的路径。
        //
        // 返回结果:
        //     受影响的文件或目录的路径。
        public string FullPath { get; private set; }
        //
        // 摘要:
        //     获取受影响的文件或目录的名称。
        //
        // 返回结果:
        //     受影响的文件或目录名。
        public string Name { get; private set; }


        public DateTime ChangeTime { get; private set; }

        public string OldFullPath { get; private set; }
        //
        // 摘要:
        //     获取受影响的文件或目录的旧名称。
        //
        // 返回结果:
        //     受影响的文件或目录的前一个名称。
        public string OldName { get; private set; }
    }
}
