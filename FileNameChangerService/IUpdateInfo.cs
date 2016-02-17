using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileNameChanger.Service
{
    public interface IUpdateInfo : ICloneable
    {
        string GetMessage { get; }

        int GetCounter { get; }

        void AddMessage(string message);

        DateTime UpdateTime { get; }

        void ResetCounter();
    }
}
