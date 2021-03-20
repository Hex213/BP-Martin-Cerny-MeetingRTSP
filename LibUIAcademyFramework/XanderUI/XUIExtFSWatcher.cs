using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace LibUIAcademyFramework.XanderUI
{
    public class XUIExtFSWatcher : Component
    {
        // Fields
        private FileSystemWatcher watcher = new FileSystemWatcher();
        private DateTime lastRead = DateTime.MinValue;
        private Control outputControl;
        private bool slimOutput = true;

        // Events
        public event FileSystemEventHandler FileCreated;

        public event FileSystemEventHandler FileDeleted;

        public event FileSystemEventHandler FileChanged;

        public event RenamedEventHandler FileRenamed;

        public event EventHandler ServiceStarted;

        public event EventHandler ServiceStopped;

        // Methods
        public XUIExtFSWatcher()
        {
            this.watcher.NotifyFilter = NotifyFilters.Security | NotifyFilters.CreationTime | NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.Attributes | NotifyFilters.DirectoryName | NotifyFilters.FileName;
            this.watcher.Filter = "*.*";
            this.watcher.IncludeSubdirectories = true;
            this.watcher.Created += new FileSystemEventHandler(this.OnCreated);
            this.watcher.Changed += new FileSystemEventHandler(this.OnChanged);
            this.watcher.Deleted += new FileSystemEventHandler(this.OnDeleted);
            this.watcher.Renamed += new RenamedEventHandler(this.OnRenamed);
            this.watcher.EnableRaisingEvents = false;
            this.watcher.InternalBufferSize = 0x5000;
        }

        private void OnCreated(object source, FileSystemEventArgs e)
        {
            DateTime lastWriteTime = File.GetLastWriteTime(e.FullPath);
            if (lastWriteTime != this.lastRead)
            {
                this.lastRead = lastWriteTime;
                if (this.slimOutput)
                {
                    if (this.outputControl.InvokeRequired)
                    {
                        this.outputControl.Invoke(new Action(() => this.outputControl.Text = this.outputControl.Text + "\nCreated: " + e.FullPath.Replace(this.watcher.Path, "")));
                    }
                }
                else if (this.outputControl.InvokeRequired)
                {
                    this.outputControl.Invoke(new Action(() => this.outputControl.Text = this.outputControl.Text + "\nCreated: " + e.FullPath));
                }
                this.OnFileCreated(e);
            }
        }

        private void OnDeleted(object source, FileSystemEventArgs e)
        {
            DateTime lastWriteTime = File.GetLastWriteTime(e.FullPath);
            this.lastRead = lastWriteTime;
            if (this.slimOutput)
            {
                if (this.outputControl.InvokeRequired)
                {
                    this.outputControl.Invoke(new Action(() => this.outputControl.Text = this.outputControl.Text + "\nDeleted: " + e.FullPath.Replace(this.watcher.Path, "")));
                }
            }
            else if (this.outputControl.InvokeRequired)
            {
                this.outputControl.Invoke(new Action(() => this.outputControl.Text = this.outputControl.Text + "\nDeleted: " + e.FullPath));
            }
            this.OnFileDeleted(e);
        }

        protected virtual void OnFileCreated(FileSystemEventArgs e)
        {
            if (this.FileCreated != null)
            {
                this.FileCreated(this, e);
            }
        }

        protected virtual void OnFileDeleted(FileSystemEventArgs e)
        {
            if (this.FileDeleted != null)
            {
                this.FileDeleted(this, e);
            }
        }

        protected virtual void OnFileChanged(FileSystemEventArgs e)
        {
            if (this.FileChanged != null)
            {
                this.FileChanged(this, e);
            }
        }

        protected virtual void OnFileRenamed(RenamedEventArgs e)
        {
            if (this.FileRenamed != null)
            {
                this.FileRenamed(this, e);
            }
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            DateTime lastWriteTime = File.GetLastWriteTime(e.FullPath);
            if ((lastWriteTime != this.lastRead) && File.Exists(e.FullPath))
            {
                this.lastRead = lastWriteTime;
                if (this.slimOutput)
                {
                    this.outputControl.Invoke(new Action(() => this.outputControl.Text = this.outputControl.Text + "\nChanged: " + e.FullPath.Replace(this.watcher.Path, "")));
                }
                else
                {
                    this.outputControl.Invoke(new Action(() => this.outputControl.Text = this.outputControl.Text + "\nChanged: " + e.FullPath));
                }
                this.OnFileChanged(e);
            }
        }

        public void OnRenamed(object source, RenamedEventArgs e)
        {
            DateTime lastWriteTime = File.GetLastWriteTime(e.FullPath);
            if (lastWriteTime != this.lastRead)
            {
                this.lastRead = lastWriteTime;
                if (this.slimOutput)
                {
                    if (this.outputControl.InvokeRequired)
                    {
                        this.outputControl.Invoke(new Action(() => this.outputControl.Text = this.outputControl.Text +
                            "\nRenamed: " +
                            e.FullPath.Replace(this.watcher.Path, "")));
                    }
                }
                else if (this.outputControl.InvokeRequired)
                {
                    this.outputControl.Invoke(new Action(() =>
                        this.outputControl.Text = this.outputControl.Text + "\nRenamed: " + e.FullPath));
                }
                this.OnFileRenamed(e);
            }
        }

        protected virtual void OnServiceStarted()
        {
            if (this.ServiceStarted != null)
            {
                this.ServiceStarted(this, null);
            }
        }

        protected virtual void OnServiceStopped(FileSystemEventArgs e)
        {
            if (this.ServiceStopped != null)
            {
                this.ServiceStopped(this, null);
            }
        }

        public void StartService()
        {
            this.ServiceStarted(this, null);
            this.watcher.EnableRaisingEvents = watcher.EnableRaisingEvents != null ? watcher.EnableRaisingEvents : true;
        }

        public void StopService()
        {
            this.ServiceStopped(this, null);
            if (this.watcher.EnableRaisingEvents)
            {
                this.watcher.EnableRaisingEvents = false;
            }
        }

        // Properties
        [Category("XanderUI"), Browsable(true), Description("Choose when the watcher updates")]
        public NotifyFilters UpdateOn
        {
            get =>
                this.watcher.NotifyFilter;
            set =>
                this.watcher.NotifyFilter = value;
        }

        [Category("XanderUI"), Browsable(true), Description("Watch subdirectories")]
        public bool WatchSubdirectories
        {
            get =>
                this.watcher.IncludeSubdirectories;
            set =>
                this.watcher.IncludeSubdirectories = value;
        }

        [Category("XanderUI"), Browsable(true), Description("The path to watch")]
        public string WatchPath
        {
            get =>
                this.watcher.Path;
            set
            {
                if (Directory.Exists(value))
                {
                    this.watcher.Path = value;
                }
                else if (File.Exists(value))
                {
                    this.watcher.Path = Path.GetDirectoryName(value);
                    this.watcher.Filter = Path.GetFileName(value);
                }
            }
        }

        [Category("XanderUI"), Browsable(true), Description("Filter for certin files")]
        public string Filters
        {
            get =>
                this.watcher.Filter;
            set =>
                this.watcher.Filter = value;
        }

        [Category("XanderUI"), Browsable(true), Description("The control to output to(via Text)")]
        public Control OutputControl
        {
            get =>
                this.outputControl;
            set =>
                this.outputControl = value;
        }

        [Category("XanderUI"), Browsable(true), Description("Remove WatchPath from output")]
        public bool SlimOutput
        {
            get =>
                this.slimOutput;
            set =>
                this.slimOutput = value;
        }
    }



}
