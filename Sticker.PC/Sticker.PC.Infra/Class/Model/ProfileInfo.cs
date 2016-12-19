using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sticker.PC.Infra.Class.Model
{
    public class ProfileInfo
    {
        public int ThumbnailNum { get; set; }
        public string ThumbnailPath { get; set; }

        public ProfileInfo(int thumbnailNum)
        {
            this.ThumbnailNum = thumbnailNum;
            ThumbnailPath = GetThumbnailPath();
        }

        public string GetThumbnailPath()
        {
            return "/Sticker.PC.Infra;component/Resources/Images/Profile/Avatar/img_thumb" + ThumbnailNum.ToString() + ".png";
        }
    }
}
