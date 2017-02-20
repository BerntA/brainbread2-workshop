//=========       Copyright © Reperio Studios 2013-2017 @ Bernt Andreas Eide!       ============//
//
// Purpose: Simple localization support.
//
//=============================================================================================//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshopper.Core;

namespace Workshopper.Filesystem
{
    public static class Localization
    {
        private static List<KeyValuePair<String, String>> _localizedTokens;
        public static bool LoadLocalization(string language)
        {
            string path = string.Format("{0}\\localization\\workshopper_{1}.txt", Globals.GetAppPath(), language);
            if (!File.Exists(path))
                return false;

            KeyValues pkvLocalizationData = new KeyValues();
            if (!pkvLocalizationData.LoadFromFile(path))
                return false;

            if (_localizedTokens != null)
                _localizedTokens.Clear();
            else
                _localizedTokens = new List<KeyValuePair<string, string>>();

            foreach (KeyValueItem item in pkvLocalizationData.GetSubItems().ToArray())
            {
                if (string.IsNullOrEmpty(item.key) || string.IsNullOrEmpty(item.value))
                    continue;

                _localizedTokens.Add(new KeyValuePair<string, string>(item.key, item.value));
            }

            pkvLocalizationData.Dispose();
            pkvLocalizationData = null;
            return true;
        }

        public static string GetTextForToken(string token, params string[] args)
        {
            string result = GetTextForToken(token);
            if (!string.IsNullOrEmpty(result))
            {
                for (int i = 0; i < args.Length; i++)
                    result = result.Replace(string.Format("%s{0}", (i + 1)), args[i]);
            }

            return result;
        }

        public static string GetTextForToken(string token)
        {
            if (_localizedTokens == null)
                return null;

            for (int i = 0; i < _localizedTokens.Count(); i++)
            {
                if (_localizedTokens[i].Key.Equals(token, StringComparison.CurrentCulture))
                    return _localizedTokens[i].Value;
            }

            return null;
        }
    }
}