using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sticker.PC.Module.App.Gallery.Global
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace Sticker.PC.Module.App.Gallery.Global
    {
        public class GlobalEngine
        {
            #region Singleton
            private static GlobalEngine _instance = new GlobalEngine();

            public static GlobalEngine Instance
            {
                get
                {
                    return _instance;
                }
            }

            private GlobalEngine()
            {

            }
            #endregion

            public int GallerySelectPosition { get; set; }
        }
    }
}
