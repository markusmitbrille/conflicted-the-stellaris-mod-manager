using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Conflicted.Model
{
    internal class ModRegistry : IDictionary<string, Mod>, ICollection<KeyValuePair<string, Mod>>, IEnumerable<KeyValuePair<string, Mod>>, IEnumerable, IDictionary, ICollection, IReadOnlyDictionary<string, Mod>, IReadOnlyCollection<KeyValuePair<string, Mod>>, ISerializable, IDeserializationCallback
    {
        private readonly Dictionary<string, Mod> collection = new Dictionary<string, Mod>();

        private List<ModFile> conflictedFiles = new List<ModFile>();
        private List<ModElement> conflictedElements = new List<ModElement>();

        public IReadOnlyList<ModFile> ConflictedFiles => conflictedFiles;
        public IReadOnlyList<ModElement> ConflictedElements => conflictedElements;

        public ICollection<string> Keys => ((IDictionary<string, Mod>)collection).Keys;
        public ICollection<Mod> Values => ((IDictionary<string, Mod>)collection).Values;

        public int Count => collection.Count;
        public bool IsReadOnly => ((IDictionary<string, Mod>)collection).IsReadOnly;
        public bool IsFixedSize => ((IDictionary)collection).IsFixedSize;
        public object SyncRoot => ((IDictionary)collection).SyncRoot;
        public bool IsSynchronized => ((IDictionary)collection).IsSynchronized;

        ICollection IDictionary.Keys => ((IDictionary)collection).Keys;
        ICollection IDictionary.Values => ((IDictionary)collection).Values;

        IEnumerable<string> IReadOnlyDictionary<string, Mod>.Keys => ((IReadOnlyDictionary<string, Mod>)collection).Keys;
        IEnumerable<Mod> IReadOnlyDictionary<string, Mod>.Values => ((IReadOnlyDictionary<string, Mod>)collection).Values;

        public object this[object key]
        {
            get => ((IDictionary)collection)[key];
            set
            {
                ((IDictionary)collection)[key] = value;
                FindFileConflicts();
                FindElementConflicts();
            }
        }

        public Mod this[string key]
        {
            get => collection[key];
            set
            {
                collection[key] = value;
                FindFileConflicts();
                FindElementConflicts();
            }
        }

        public ModRegistry()
        {
            collection = new Dictionary<string, Mod>();
        }

        public ModRegistry(int capacity)
        {
            collection = new Dictionary<string, Mod>(capacity);
        }

        public ModRegistry(IEqualityComparer<string> comparer)
        {
            collection = new Dictionary<string, Mod>(comparer);
        }

        public ModRegistry(IDictionary<string, Mod> dictionary)
        {
            collection = new Dictionary<string, Mod>(dictionary);
            FindFileConflicts();
            FindElementConflicts();
        }

        public ModRegistry(int capacity, IEqualityComparer<string> comparer)
        {
            collection = new Dictionary<string, Mod>(capacity, comparer);
        }

        public ModRegistry(IDictionary<string, Mod> dictionary, IEqualityComparer<string> comparer)
        {
            collection = new Dictionary<string, Mod>(dictionary, comparer);
            FindFileConflicts();
            FindElementConflicts();
        }

        public void FindFileConflicts()
        {
            conflictedFiles = collection.Values
                .SelectMany(mod => mod.Files)
                .GroupBy(file => file.ID)
                .Where(fileGroup => fileGroup.Count() > 1)
                .SelectMany(fileGroup => fileGroup)
                .ToList();
        }

        public void FindElementConflicts()
        {
            conflictedElements = collection.Values
                .SelectMany(mod => mod.Files)
                .SelectMany(file => file.Elements)
                .GroupBy(element => element.Name)
                .Where(elementGroup => elementGroup.Count() > 1)
                .SelectMany(elementGroup => elementGroup)
                .ToList();
        }

        public bool ContainsKey(string key)
        {
            return collection.ContainsKey(key);
        }

        public void Add(string key, Mod value)
        {
            collection.Add(key, value);
            FindFileConflicts();
            FindElementConflicts();
        }

        public bool Remove(string key)
        {
            bool result = collection.Remove(key);
            FindFileConflicts();
            FindElementConflicts();
            return result;
        }

        public bool TryGetValue(string key, out Mod value)
        {
            return collection.TryGetValue(key, out value);
        }

        public void Add(KeyValuePair<string, Mod> item)
        {
            ((IDictionary<string, Mod>)collection).Add(item);
            FindFileConflicts();
            FindElementConflicts();
        }

        public void Clear()
        {
            collection.Clear();
            FindFileConflicts();
            FindElementConflicts();
        }

        public bool Contains(KeyValuePair<string, Mod> item)
        {
            return ((IDictionary<string, Mod>)collection).Contains(item);
        }

        public void CopyTo(KeyValuePair<string, Mod>[] array, int arrayIndex)
        {
            ((IDictionary<string, Mod>)collection).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, Mod> item)
        {
            bool result = ((IDictionary<string, Mod>)collection).Remove(item);
            FindFileConflicts();
            FindElementConflicts();
            return result;
        }

        public IEnumerator<KeyValuePair<string, Mod>> GetEnumerator()
        {
            return ((IDictionary<string, Mod>)collection).GetEnumerator();
        }

        public bool Contains(object key)
        {
            return ((IDictionary)collection).Contains(key);
        }

        public void Add(object key, object value)
        {
            ((IDictionary)collection).Add(key, value);
            FindFileConflicts();
            FindElementConflicts();
        }

        public void Remove(object key)
        {
            ((IDictionary)collection).Remove(key);
            FindFileConflicts();
            FindElementConflicts();
        }

        public void CopyTo(Array array, int index)
        {
            ((IDictionary)collection).CopyTo(array, index);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            collection.GetObjectData(info, context);
        }

        public void OnDeserialization(object sender)
        {
            collection.OnDeserialization(sender);
            FindFileConflicts();
            FindElementConflicts();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IDictionary<string, Mod>)collection).GetEnumerator();
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary)collection).GetEnumerator();
        }
    }
}