using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Media.ViewModels
{

    public class UploadedFile
    {

        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public long Size { get; set; }

        public UploadedFile()
        {

        }

        public UploadedFile(string name, long size)
        {
            Name = name;
            Size = size;
        }
    }


}
