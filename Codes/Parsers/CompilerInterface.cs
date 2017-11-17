using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Parsers
{
    public interface  CompilerInterface
    {
        void Init();
        string[] ReadARecord(int expectedFieldNum);
        
        DateTime SpringOverHeaders(CultureInfo _culture);
        void CloseFile();
        void ReadNode();
        bool IsNotEndElement();
        bool XMLLoaded();
    }
}
