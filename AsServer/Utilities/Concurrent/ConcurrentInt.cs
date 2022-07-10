using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsServer.Utilities.Concurrent
{
    /// <summary>
    /// 线程安全的int 类型
    /// </summary>
    public class ConcurrentInt
    {
        private int _value;

        public ConcurrentInt(int value) { _value = value; }

        /// <summary>
        /// 添加并获取
        /// </summary>
        /// <returns></returns>
        public int Add_Get()
        {
            lock (this)
            {
                _value++;
                return _value;
            }
        }

        public int Reduce_Get()
        {
            lock (this)
            {
                _value--;
                return _value;
            }
        }

        public int Get()
        {
            return _value;
        }
    }
}
