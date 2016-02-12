//=========       Copyright © Reperio Studios 2013-2016 @ Bernt Andreas Eide!       ============//
//
// Purpose: Shared Definitons
//
//=============================================================================================//

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace workshopper.core
{
    public static class globals
    {
        public static string GetAppPath() { return Application.StartupPath; }
        public static string GetTexturePath() { return string.Format("{0}\\assets", GetAppPath()); }
        public static Image GetTexture(string folder, string name, string extension = "png")
        {
            try
            {
                return Image.FromFile(string.Format("{0}\\{1}\\{2}.{3}", GetTexturePath(), folder, name, extension));
            }
            catch (Exception ex)
            {
                utils.LogAction(ex.Message);
                return Properties.Resources.unknown;
            }
        }

        public static Image GetTexture(string name, string extension = "png")
        {
            string folder = null;

            try
            {
                foreach (string file in Directory.EnumerateFiles(GetTexturePath(), string.Format("*.{0}", extension), SearchOption.AllDirectories))
                {
                    string rawFileName = Path.GetFileNameWithoutExtension(file);
                    if ((rawFileName.Contains(name)) && (rawFileName.Length == name.Length))
                        folder = Path.GetDirectoryName(file).Replace(GetTexturePath(), "");
                }

                return Image.FromFile(string.Format("{0}\\{1}\\{2}.{3}", GetTexturePath(), folder, name, extension));
            }
            catch (Exception ex)
            {
                utils.LogAction(ex.Message);
                return Properties.Resources.unknown;
            }
        }

        public static Color GetColorFraction(Color start, Color end, int offset = 0)
        {
            int r = (int)start.R,
            g = (int)start.G,
            b = (int)start.B;

            r += ((start.R > end.R) ? (-1 - offset) : (1 + offset));
            g += ((start.G > end.G) ? (-1 - offset) : (1 + offset));
            b += ((start.B > end.B) ? (-1 - offset) : (1 + offset));

            if (r < 0)
                r = 0;
            else if (r > 255)
                r = 255;

            if (g < 0)
                g = 0;
            else if (g > 255)
                g = 255;

            if (b < 0)
                b = 0;
            else if (b > 255)
                b = 255;

            return Color.FromArgb(r, g, b);
        }
    }
}
