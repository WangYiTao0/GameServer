using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace AsServer.Utilities.Timer
{
    /// <summary>
    /// 定时任务(计时器)管理类
    /// </summary>
    public class TimerManager
    {
        private TimerManager instance;
        public TimerManager Instance 
        { 
            get 
            {
                lock (instance)
                {
                    if (instance == null)
                        instance = new TimerManager();
                    return instance;
                }
            }
        }

        /// <summary>
        /// 实现定时类的主要功能就是这个Timer类
        /// </summary>
        private System.Timers.Timer timer;

        // ConcurrentDictionary 多线程安全访问
        /// <summary>
        /// 存储任务 id 和 任务模型的映射
        /// </summary>
        private ConcurrentDictionary<int, TimerModel> _idModelDict = new ConcurrentDictionary<int, TimerModel>();

        /// <summary>
        /// 要移除的id列表
        /// </summary>
        private List<int> _removeList = new List<int>();

        /// <summary>
        /// 用来表示id
        /// </summary>
        private int _id = 0;

        public TimerManager()
        {
            timer = new System.Timers.Timer(1000);
            timer.Elapsed += OnTimerElapsed;
        }

        /// <summary>
        /// 达到时间间隔触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            lock (_removeList)
            {
                TimerModel tmpModel = null;

                foreach (var id in _removeList)
                {
                    _idModelDict.TryRemove(id, out tmpModel);
                }

                _removeList.Clear();
            }

            foreach (TimerModel model in _idModelDict.Values)
            {
                //比当前时间小时 
                if (model.Time < DateTime.Now.Ticks)
                    model.Run();
            }
        }

        /// <summary>
        /// 添加定时任务 指定触发的时间
        /// </summary>
        public void AddTimeEvent(DateTime dateTime, TimerDelegate timerDelegate)
        {
            long delayTime = dateTime.Ticks - DateTime.Now.Ticks;

            if (delayTime <= 0)
                return;
            AddTimeEvent(delayTime, timerDelegate);
        }

        /// <summary>
        /// 添加定时任务 指定延迟的时间
        /// </summary>
        /// <param name="delayTime">毫秒</param>
        /// <param name="timerDelegate"></param>
        public void AddTimeEvent(long delayTime, TimerDelegate timerDelegate)
        {
            TimerModel model = new TimerModel(_id++, DateTime.Now.Ticks+ delayTime, timerDelegate);
            _idModelDict.TryAdd(model.Id, model);


        }

    }
}
