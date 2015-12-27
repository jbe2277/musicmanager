using System;
using System.Threading.Tasks;

namespace Waf.MusicManager.Applications.Services
{
    public class TranscodingTaskEventArgs : EventArgs
    {
        private readonly string fileName;
        private readonly Task transcodingTask;


        public TranscodingTaskEventArgs(string fileName, Task transcodingTask)
        {
            this.fileName = fileName;
            this.transcodingTask = transcodingTask;
        }


        public string FileName { get { return fileName; } }

        public Task TranscodingTask { get { return transcodingTask; } }
    }
}
