using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsServer.Utilities
{
    public delegate void ExecuteDelegate();

    /// <summary>
    /// 单线程池
    /// 提供给应用层 
    /// </summary>
    public class SingleExcute
    {
        /// <summary>
        /// 互斥锁 保证共享数据的完整性
        /// </summary>
        public Mutex mutex;

        SingleExcute()
        {
            mutex = new Mutex();
        }
        public void Excute(ExecuteDelegate executeDelegate)
        {
            lock(this)
            {
                mutex.WaitOne();
                executeDelegate?.Invoke();
                mutex.ReleaseMutex();
            }
        }
    }
}
