using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections;

namespace Utils
{
    #region [Config]

    /// <summary>
    /// Provides work with xml-based config file.
    /// </summary>
    /// <typeparam name="K">Key type. Do not use non-covertible-to-text types.</typeparam>
    /// <typeparam name="V">Value type. Do not use non-covertible-to-text types.</typeparam>
    public class Config<K, V> : IDictionary<K, V>
    {
        /// <summary>
        /// Config data collection.
        /// </summary>
        private List<Pair<K, V>> configData;

        /// <summary>
        /// Config file name without path.
        /// </summary>
        private string configName;

        public string ConfigDirPath { get; private set; } = Environment.CurrentDirectory + @"\Config\";

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
                    using (var fs = new FileStream(ConfigDirPath + configName, FileMode.Open))
                    {
                        var xmlSer = new XmlSerializer(typeof(List<Pair<K, V>>));
                        return (List<Pair<K, V>>)xmlSer.Deserialize(fs);
                    }
                }
                catch (Exception ex)
                {
                    Log.Write(LogType.ERROR, ex, "Reading config error");
                    return new List<Pair<K, V>>();
                }
            }
            set
            {
                try
                {
                    if (!Directory.Exists(ConfigDirPath))
                        Directory.CreateDirectory(ConfigDirPath);
                    using (var fs = new FileStream(ConfigDirPath + configName, File.Exists(ConfigDirPath + configName) ? FileMode.Truncate : FileMode.CreateNew))
                    {
                        var xmlSer = new XmlSerializer(typeof(List<Pair<K, V>>));
                        xmlSer.Serialize(fs, value);
                    }
                }
                catch (Exception ex)
                {
                    Log.Write(LogType.ERROR, ex, "Writing config error");
                }
            }
        }

        #region IDictionary implementation

        /// <summary>
        /// Gets the count of elements of configuration.
        /// </summary>
        public int Count
        {
            get
            {
                return configData.Count;
            }
        }

        /// <summary>
        /// Gets or sets read-only flag of configuration file.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                FileInfo file = new FileInfo(ConfigDirPath + configName);
                return file.IsReadOnly;
            }
            set
            {
                FileInfo file = new FileInfo(ConfigDirPath + configName);
                file.IsReadOnly = value;
            }
        }

        /// <summary>
        /// Represents the keys collection of configuration.
        /// </summary>
        public ICollection<K> Keys
        {
            get
            {
                return configData.Select(x => x.Key).ToList();
            }
        }

        /// <summary>
        /// Represents the values collection of configuation.
        /// </summary>
        public ICollection<V> Values
        {
            get
            {
                return configData.Select(x => x.Value).ToList();
            }
        }

        /// <summary>
        /// IEnumerable implementation. 
        /// </summary>
        /// <returns>Enumerator of key-value pair.</returns>
        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            foreach (var item in configData)
            {
                yield return new KeyValuePair<K, V>(item.Key, item.Value);
            }
        }

        /// <summary>
        /// IEnumerable implementation. 
        /// </summary>
        /// <returns>Enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Add new element to configuration, if specified element key is not exists.
        /// </summary>
        /// <param name="item">New item.</param>
        public void Add(KeyValuePair<K, V> item)
        {
            if (ContainsKey(item.Key))
            {
                configData.Add(new Pair<K, V>(item.Key, item.Value));
                ConfigFromToFile = configData;
            }
        }

        /// <summary>
        /// Removes all elements of configuration.
        /// </summary>
        public void Clear()
        {
            configData.Clear();
            ConfigFromToFile = configData;
        }

        /// <summary>
        /// Determines whether specified element contains by configuration.
        /// </summary>
        /// <param name="item">Elements to search.</param>
        /// <returns>True if exists, otherwise false.</returns>
        public bool Contains(KeyValuePair<K, V> item)
        {
            foreach (var el in configData)
            {
                if (el.Key.Equals(item.Key) && el.Value.Equals(item.Value))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Removes specified element from configuration.
        /// </summary>
        /// <param name="item">Element to remove (value-based comparsion).</param>
        /// <returns>True on successful remove, otherwise false.</returns>
        public bool Remove(KeyValuePair<K, V> item)
        {
            for (int i = 0; i < configData.Count; i++)
            {
                if (configData[i].Key.Equals(item.Key) && configData[i].Value.Equals(item.Value))
                {
                    configData.RemoveAt(i);
                    ConfigFromToFile = configData;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether specified key contains by configuration.
        /// </summary>
        /// <param name="key">Element key to search.</param>
        /// <returns>True if exists, otherwise false.</returns>
        public bool ContainsKey(K key)
        {
            foreach (var el in configData)
            {
                if (el.Key.Equals(key))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Add new element to configuration, if specified element key is not exists.
        /// </summary>
        /// <param name="key">Key of ney item.</param>
        /// <param name="value">Value of new item.</param>
        public void Add(K key, V value)
        {
            Add(new KeyValuePair<K, V>(key, value));
        }

        /// <summary>
        /// Removes the element by specified key.
        /// </summary>
        /// <param name="key">Key of element to remove.</param>
        /// <returns>>True on successful remove, otherwise false.</returns>
        public bool Remove(K key)
        {
            for (int i = 0; i < configData.Count; i++)
            {
                if (configData[i].Key.Equals(key))
                {
                    configData.RemoveAt(i);
                    ConfigFromToFile = configData;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the value by specified key.
        /// </summary>
        /// <param name="key">Element key to search.</param>
        /// <param name="value">Result value. If value was not found - default value.</param>
        /// <returns>True if found, otherwise false.</returns>
        public bool TryGetValue(K key, out V value)
        {
            foreach (var item in configData)
            {
                if (item.Key.Equals(key))
                {
                    value = item.Value;
                    return true;
                }
            }
            value = default(V);
            return false;
        }

        /// <summary>
        /// NOT IMPLEMENTED!
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        } 
        #endregion

        /// <summary>
        /// Read data from configuration file.
        /// </summary>
        public void Update()
        {
            configData = ConfigFromToFile;
        }

        /// <summary>
        /// Remove current configuration file.
        /// </summary>
        public void RemoveFile()
        {
            File.Delete(ConfigDirPath + configName);
        }

        /// <summary>
        /// Gets or sets config parameter value. If already exists - rewrite, otherwise add.
        /// </summary>
        /// <param name="Key">The unique key of configuration element.</param>
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
            ConfigDirPath = Environment.CurrentDirectory + @"\Config\";
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
