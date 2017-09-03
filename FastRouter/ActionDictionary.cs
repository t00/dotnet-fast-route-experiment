using System;
using System.Collections.Generic;

namespace FastRouter
{
    public class ActionDictionary: Dictionary<char, ActionDictionary>
    {
        public static bool UseLookup { get; set; }

        public Action<ActionStatus> Action { get; set; }

        public int MinLength { get; set; }

        public string Suffix
        {
            get => suffix;
            set
            {
                suffix = value;
                SuffixLength = value.Length;
            }
        }

        public int SuffixLength { get; private set; }

        public new bool TryGetValue(char key, out ActionDictionary value)
        {
            if (index == null && UseLookup)
            {
                index = new ActionDictionary[256];
                isIndexValid = true;
                foreach (var k in this)
                {
                    var dictChar = (int)k.Key;
                    if (dictChar >= 0 && dictChar < index.Length)
                    {
                        index[dictChar] = k.Value;
                    }
                    else
                    {
                        isIndexValid = false;
                        break;
                    }
                }
            }
            if (isIndexValid)
            {
                var ch = (short)key;
                if (ch >= 0 && ch < 256)
                {
                    value = index[ch];
                    return value != null;
                }
            }
            return base.TryGetValue(key, out value);
        }

        public ActionDictionary(int minLength)
        {
            MinLength = minLength;
        }

        public ActionDictionary(int minLength, string suffix)
            : this(minLength)
        {
            Suffix = suffix;
        }

        public ActionDictionary(int minLength, string suffix, Action<ActionStatus> action)
            : this(minLength, suffix)
        {
            Action = action;
        }

        private ActionDictionary[] index = null;

        private bool isIndexValid;

        private string suffix;
    }
}
