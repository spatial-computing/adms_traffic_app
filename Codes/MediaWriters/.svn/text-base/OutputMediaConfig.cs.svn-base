/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MediaWriters
{
    public class OutputMediaConfig { }
    public class FileMediaConfig : OutputMediaConfig
    {
        public string OutputFileName { get; set; }
        public FileMediaConfig(string outputFile)
        {
            OutputFileName = outputFile;
        }

        private static OutputMediaConfig GetOutputFileMediaConfig(string outputFilePath)
        {
            return new FileMediaConfig(outputFilePath);
        }

    }


}
