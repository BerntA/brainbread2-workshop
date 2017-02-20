//=========       Copyright © Reperio Studios 2013-2017 @ Bernt Andreas Eide!       ============//
//
// Purpose: Allows forms / user controls to load their layout dynamically by parsing a certain file which holds information about postion, sizing, etc...
//
//=============================================================================================//

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Workshopper.Core;
using Workshopper.Filesystem;

namespace Workshopper.UI
{
    public class DynamicLayoutLoader
    {
        #region DynamicLayoutLoader Static definitions
        private static List<DynamicLayoutLoader> _layoutObjects = new List<DynamicLayoutLoader>();

        public static void LoadLayoutFiles()
        {
            _layoutObjects.Clear();

            foreach (string file in Directory.EnumerateFiles(string.Format("{0}\\layout\\", Globals.GetAppPath()), "*.res", SearchOption.AllDirectories))
            {
                DynamicLayoutLoader layoutItem = LoadLayoutFile(file);
                if (layoutItem == null)
                    continue;

                _layoutObjects.Add(layoutItem);
            }
        }

        public static DynamicLayoutLoader LoadLayoutForControl(string layoutFile, Control ctrl)
        {
            foreach (DynamicLayoutLoader layoutItem in _layoutObjects.ToArray())
            {
                if (layoutItem.GetName().Equals(layoutFile, StringComparison.CurrentCulture))
                {
                    layoutItem.LoadLayoutForControl(ctrl.Controls);
                    return layoutItem;
                }
            }

            return null;
        }

        private static DynamicLayoutLoader LoadLayoutFile(string layoutFile)
        {
            if (string.IsNullOrEmpty(layoutFile))
                return null;

            KeyValues pkvResFile = new KeyValues();
            if (!pkvResFile.LoadFromFile(layoutFile))
                return null;

            DynamicLayoutLoader layoutObj = new DynamicLayoutLoader(Path.GetFileNameWithoutExtension(layoutFile));
            layoutObj.AddResItem(pkvResFile, true);
            for (KeyValues sub = pkvResFile.GetFirstKey(); sub != null; sub = pkvResFile.GetNextKey())
                layoutObj.AddResItem(sub);

            pkvResFile.Dispose();
            pkvResFile = null;

            return layoutObj;
        }
        #endregion

        public delegate void OnLoadLayout(object sender, EventArgs e);
        public event OnLoadLayout OnLoadedLayout;
        protected List<CustomResourceItem> _childResourceItems;
        protected CustomResourceItem _mainResourceItem;
        private string _name;

        public DynamicLayoutLoader(string name)
        {
            _name = name;
            _childResourceItems = new List<CustomResourceItem>();
        }

        virtual public string GetName() { return _name; }

        virtual public void AddResItem(KeyValues pkv, bool bMainOverride = false)
        {
            if (bMainOverride)
            {
                _mainResourceItem = new CustomResourceItem(
    "",
    new Rectangle(
        GetControlCoordinate(null, pkv, "xpos"),
    GetControlCoordinate(null, pkv, "ypos", true),
    pkv.GetInt("wide"),
     pkv.GetInt("tall")
     ),
     GetColorForString(pkv.GetString("FgColor")),
     GetColorForString(pkv.GetString("BgColor"))
    );
            }
            else
            {
                CustomResourceItem customItem = new CustomResourceItem(
                    pkv.GetName(),
                    new Rectangle(
                        GetControlCoordinate(null, pkv, "xpos"),
                    GetControlCoordinate(null, pkv, "ypos", true),
                    pkv.GetInt("wide"),
                     pkv.GetInt("tall")
                     ),
                     GetColorForString(pkv.GetString("FgColor")),
                     GetColorForString(pkv.GetString("BgColor"))
                    );

                _childResourceItems.Add(customItem);
            }
        }

        virtual public CustomResourceItem GetMainResItem() { return _mainResourceItem; }
        virtual public CustomResourceItem GetChildResItem(string name)
        {
            foreach (CustomResourceItem item in _childResourceItems.ToArray())
            {
                if (item.GetKeyName().Equals(name, StringComparison.CurrentCulture))
                    return item;
            }

            return null;
        }

        virtual public Rectangle GetResItemBounds(string name)
        {
            CustomResourceItem item = GetChildResItem(name);
            if (item != null)
                return item.GetBounds();

            return Rectangle.Empty;
        }

        virtual public Color GetResItemFgColor(string name)
        {
            CustomResourceItem item = GetChildResItem(name);
            if (item != null)
                return item.GetFgColor();

            return Color.Empty;
        }

        virtual public Color GetResItemBgColor(string name)
        {
            CustomResourceItem item = GetChildResItem(name);
            if (item != null)
                return item.GetBgColor();

            return Color.Empty;
        }

        virtual public void LoadLayoutForControl(System.Windows.Forms.Control.ControlCollection controls)
        {
            bool bHasAtLeastOneItem = false;
            foreach (Control ctrl in controls)
            {
                CustomResourceItem item = GetChildResItem(ctrl.Name);
                if (item == null)
                    continue;

                LoadSettingsForControl(ctrl, item);
                bHasAtLeastOneItem = true;
            }

            if (bHasAtLeastOneItem && (OnLoadedLayout != null))
                OnLoadedLayout(this, new EventArgs());
        }

        /// <summary>
        /// Loads control settings for a single control.
        /// For positioning you may use:
        /// xpos r25 <- move 25 pixels to the left from the right side.
        /// xpos c25 <- middle of the screen + 25 to the right.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="resourceInfo"></param>
        virtual protected void LoadSettingsForControl(Control control, CustomResourceItem resourceInfo)
        {
            Rectangle screenBounds = Screen.GetBounds(control);

            Color fgColor = resourceInfo.GetFgColor(), bgColor = resourceInfo.GetBgColor();
            if (fgColor != Color.Empty)
                control.ForeColor = fgColor;

            if (bgColor != Color.Empty)
                control.BackColor = bgColor;

            control.Bounds = resourceInfo.GetBounds();
            control.Invalidate();
        }

        virtual protected bool UsesPositioningTokens(string str)
        {
            if (str.Length > 0)
            {
                char token = str[0];
                if (token == 'c' || token == 'r')
                    return true;
            }

            return false;
        }

        virtual protected int GetControlCoordinate(Control control, KeyValues sub, string key, bool bVertical = false)
        {
            int coord = sub.GetInt(key);
            string raw = sub.GetString(key);
            if (UsesPositioningTokens(raw))
            {
                Rectangle screenBounds = Screen.PrimaryScreen.Bounds;

                int value = sub.GetInt(key, 0, 1);
                char token = raw[0];
                switch (token)
                {
                    case 'r':
                        coord = (bVertical ? screenBounds.Height : screenBounds.Width) - Math.Abs(value);
                        break;
                    case 'c':
                        coord = (bVertical ? (screenBounds.Height / 2) : (screenBounds.Width / 2)) + value;
                        break;
                }
            }

            return coord;
        }

        virtual protected Color GetColorForString(string str)
        {
            try
            {
                string[] newStr = str.Split(' ');
                if (newStr.Length == 4) // Must always be : | R G B A |<--- ex 255 255 255 255
                {
                    return Color.FromArgb(
                        int.Parse(newStr[3]),
                        int.Parse(newStr[0]),
                        int.Parse(newStr[1]),
                        int.Parse(newStr[2])
                        );
                }
            }
            catch
            {
                return Color.Empty;
            }

            return Color.Empty;
        }
    }
}
