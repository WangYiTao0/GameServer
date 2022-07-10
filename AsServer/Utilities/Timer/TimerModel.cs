using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsServer.Utilities.Timer
{
    /// <summary>
    /// 当定时器达到时间后触发
    /// </summary>
    public delegate void TimerDelegate();

    public class TimerModel
    {
        public int Id;

        /// <summary>
        /// 任务执行事件
        /// </summary>
        public long Time;

        private TimerDelegate _timeDelegate;
        public TimerModel(int id, long time, TimerDelegate td)
        {
            _id = id;
            Time = time;
            _timeDelegate= td;   
        }

        /// <summary>
        /// 触发任务
        /// </summary>
        public void Run()
        {
            _timeDelegate?.Invoke();
        }
    }
}
