using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileNameChanger.Service
{
    public class MapEntry : IMapEntry
    {
        private DirectoryInfo _sourceFile;
        private DirectoryInfo _destinationFile;

        public MapEntry(DirectoryInfo from, DirectoryInfo to)
        {
            _sourceFile=from;
            _destinationFile=to;
        }

        public DirectoryInfo SourceFile { get { return _sourceFile; } }
        public DirectoryInfo DestinationFile { get { return _destinationFile; } }
    }
}
