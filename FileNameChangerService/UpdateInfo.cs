using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileNameChanger.Service
{
    public class UpdateInfo : IUpdateInfo, ICloneable
    {
        private StringBuilder _message;
        private int _counter;
        private DateTime _updateTime;
        
        //I am creating this class because when i test it on 168433 fiels, the updates freeze UI.
        public UpdateInfo()
        {
            _message = new StringBuilder();
            _counter = 0;
            _updateTime = System.DateTime.Now;
        }

        protected UpdateInfo(StringBuilder message, int counter, DateTime updateTime)
        {
            _message = new StringBuilder(message.ToString());
            _counter = counter;
            _updateTime = updateTime;
        }

        public Object Clone()
        {
            return new UpdateInfo(this._message,this._counter, this._updateTime);
        }

        public string GetMessage { get { return _message.ToString(); } }
        public int GetCounter { get { return _counter; } }

        public void ResetCounter()
        {
            _counter = 0;
            _updateTime = System.DateTime.Now;
        }

        public void AddMessage(string message)
        {
            _message.AppendLine(message);
            _message.AppendLine();
            ++_counter;
        }

        public DateTime UpdateTime { get { return _updateTime; } }
    }
}
