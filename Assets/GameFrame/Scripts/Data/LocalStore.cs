using System;
using System.Globalization;
using UnityEngine;

namespace UnityCore
{
    public sealed class LocalStore
    {
        private readonly string _prefix;

        public LocalStore(string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
                throw new ArgumentException("A storage prefix is required.", nameof(prefix));

            // Include the prefix length so delimiters in a prefix or key cannot cause collisions.
            _prefix = prefix.Length.ToString(CultureInfo.InvariantCulture) + ":" + prefix + ":";
        }

        public int GetInt(string key, int fallback = 0) => PlayerPrefs.GetInt(Key(key), fallback);
        public float GetFloat(string key, float fallback = 0f) => PlayerPrefs.GetFloat(Key(key), fallback);
        public string GetString(string key, string fallback = "") => PlayerPrefs.GetString(Key(key), fallback);
        public bool GetBool(string key, bool fallback = false) => GetInt(key, fallback ? 1 : 0) != 0;

        public void SetInt(string key, int value) => PlayerPrefs.SetInt(Key(key), value);
        public void SetFloat(string key, float value) => PlayerPrefs.SetFloat(Key(key), value);
        public void SetString(string key, string value) => PlayerPrefs.SetString(Key(key), value);
        public void SetBool(string key, bool value) => SetInt(key, value ? 1 : 0);

        public void Delete(string key) => PlayerPrefs.DeleteKey(Key(key));
        public void Save() => PlayerPrefs.Save();

        private string Key(string key) => _prefix + (key ?? throw new ArgumentNullException(nameof(key)));
    }
}
