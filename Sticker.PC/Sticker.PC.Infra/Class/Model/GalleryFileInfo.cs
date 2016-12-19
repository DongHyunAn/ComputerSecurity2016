using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sticker.PC.Infra.Class.Model
{
    public class GalleryInfoWrapper
    {
        public ObservableCollection<GalleryFileInfo> GalleryFileList { get; set; }
    }
    public class GalleryFileInfo
    {
        public string ImageFilePath { get; set; }
    }
}
