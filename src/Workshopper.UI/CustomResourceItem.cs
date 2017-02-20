//=========       Copyright © Reperio Studios 2013-2017 @ Bernt Andreas Eide!       ============//
//
// Purpose: A resource item, for each item in a .res file this is the data stored per item.
//
//=============================================================================================//

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshopper.Filesystem;

namespace Workshopper.UI
{
    public sealed class CustomResourceItem
    {
        public string GetKeyName() { return _keyName; }
        public Rectangle GetBounds() { return _bounds; }
        public Color GetFgColor() { return _fgColor; }
        public Color GetBgColor() { return _bgColor; }

        private string _keyName;
        private Rectangle _bounds;
        private Color _fgColor;
        private Color _bgColor;

        public CustomResourceItem(string name, Rectangle bounds, Color fgColor, Color bgColor)
        {
            _keyName = name;
            _bounds = bounds;
            _fgColor = fgColor;
            _bgColor = bgColor;
        }
    }
}