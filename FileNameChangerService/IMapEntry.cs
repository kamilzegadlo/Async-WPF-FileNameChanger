using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileNameChanger.Service
{
    public interface IMapEntry
    {
        DirectoryInfo SourceFile { get; }
        DirectoryInfo DestinationFile { get; }
    }
}
