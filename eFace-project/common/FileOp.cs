using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace common
{
    public class FileOp
    {
        public static string SelectPicture()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter =
                "JPEG File Interchange Format (*.jpg)|*.jpg;*.jpeg|" +
                "Windows Bitmap(*.bmp)|*.bmp|" +
                "Graphics Interchange Format (*.gif)|(*.gif)|" +
                "Portable Network Graphics (*.png)|*.png|" +
                "Tag Image File Format (*.tif)|*.tif;*.tiff";
            openFileDialog.ShowDialog();
            return openFileDialog.FileName;
        }
    }

}
