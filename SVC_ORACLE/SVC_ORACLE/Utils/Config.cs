using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text;

namespace Utils
{
    #region [Config]

    /// <summary>
    /// Provides work with xml-based config file.
    /// </summary>
    /// <typeparam name="K">Key type. Do not use non-covertible-to-text types.</typeparam>
    /// <typeparam name="V">Value type. Do not use non-covertible-to-text types.</typeparam>
    public class Config<K, V>
    {
        /// <summary>
        /// Config data collection.
        /// </summary>
        private List<Pair<K, V>> configData;

        /// <summary>
        /// Config file name without path.
        /// </summary>
        private string configName;

        public string configDirPath { get; private set; } = Environment.CurrentDirectory + @"\Config\";


        /// <summary>
        /// Gets or sets the config from/to file through XML Serialization.
        /// </summary>
        /// <returns>Config data collection.</returns>
        private List<Pair<K, V>> ConfigFromToFile
        {
            get
            {
                try
                {
                    using (var fs = new FileStream(configDirPath + configName, FileMode.Open))
                    {
                        var xmlSer = new XmlSerializer(typeof(List<Pair<K, V>>));
                        return (List<Pair<K, V>>)xmlSer.Deserialize(fs);
                    }
                }
                catch (Exception)
                {
                    return new List<Pair<K, V>>();
                }
            }
            set
            {
                try
                {
                    if (!Directory.Exists(configDirPath))
                        Directory.CreateDirectory(configDirPath);
                    using (var fs = new FileStream(configDirPath + configName, File.Exists(configDirPath + configName) ? FileMode.Truncate : FileMode.CreateNew))
                    {
                        var xmlSer = new XmlSerializer(typeof(List<Pair<K, V>>));
                        xmlSer.Serialize(fs, value);
                    }
                }
                catch (Exception ex)
                {
                    Log.Write(LogType.ERROR, ex, "Writing log error");
                }
            }
        }

        public void Update()
        {
            configData = ConfigFromToFile;
        }

        public void RemoveFile()
        {
            File.Delete(configDirPath + configName);
        }

        /// <summary>
        /// Gets or sets config parameter value. If already exists - rewrite, overwise add.
        /// </summary>
        /// <param name="Key">The unique key of config parameter.</param>
        /// <returns>Config parameter value. If key is not found - returns null.</returns>
        public V this[K key]
        {
            get
            {
                //configData = ConfigFromToFile;
                foreach (var item in configData)
                {
                    if (item.Key.Equals(key))
                        return item.Value;
                }
                return default(V);
            }
            set
            {
                for (int i = 0; i < configData.Count; i++)
                {
                    if (configData[i].Key.Equals(key))
                    {
                        configData[i].Value = value;
                        ConfigFromToFile = configData;
                        return;
                    }
                }
                configData.Add(new Pair<K, V>(key, value));
                ConfigFromToFile = configData;
            }
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        /// <param name="fileName">File name of the config file without path.</param>
        public Config(string fileName)
        {
            configName = fileName;
            configData = ConfigFromToFile;
            configDirPath = Environment.CurrentDirectory + @"\Config\";
        }

        /// <summary>
        /// Init constructor with default file name.
        /// </summary>
        public Config() : this("Default.cfg") { }

        /// <summary>
        /// Analog of KeyValuePair.
        /// Used because KeyValuePair class is not serializable.
        /// </summary>
        /// <typeparam name="TName">Key type.</typeparam>
        /// <typeparam name="TValue">Value type.</typeparam>
        public class Pair<TName, TValue>
        {
            public TName Key { get; set; }
            public TValue Value { get; set; }
            public Pair(TName key, TValue value)
            {
                Key = key;
                Value = value;
            }
            public Pair() : this(default(TName), default(TValue)) { }
        }
    }

    #endregion
}
