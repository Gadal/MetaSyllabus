using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

//TODO: Implement search params/substring search/wildcards

namespace MetaSyllabus.Utilities
{
    /// <summary>
    /// A key/value store implemented using a Patricia trie.  Enables fast
    /// searching.
    /// </summary>
    /// <remarks>
    /// As it stands, this isn't functionally different from a lookup table.
    /// But I can add various search parameters, substring search, wildcards, etc
    /// if the index's backing store is formatted this way.
    /// </remarks>
    public class PatriciaTrie<T>
    {
        private Char[] Key;
        private List<T> TValue;
        private List<PatriciaTrie<T>> Children;

        public PatriciaTrie()
        {
            Key = new Char[0];
            TValue = new List<T>();
            Children = new List<PatriciaTrie<T>>();
        }

        public void Insert(string key, T t)
        {
            if (String.IsNullOrEmpty(key))
                return;

            // Split key into words: "political science" -> {"political", "science"}
            IEnumerable<Char[]> formattedKeys = new string(key.ToLowerInvariant()
                                                              .ToCharArray()
                                                              .Where(c => char.IsLetterOrDigit(c) ||
                                                                          c == ' ' ||
                                                                          c == '-')
                                                              .ToArray())
                                                              .Split(' ')
                                                              .Select(x => x.ToCharArray());

            foreach (Char[] formattedKey in formattedKeys)
            {
                InsertInternal(formattedKey, t);
            }
        }

        private void InsertInternal(Char[] key, T t)
        {
            if (Key.SequenceEqual(key))
            {
                TValue.Add(t);
                return;
            }

            // Look for a branch where at least the first character matches. bb matches b, ba, bc, bbb, etc.
            // Only one branch will match.
            PatriciaTrie<T> match = null;
            int earliestDiscrepancy = -1;
            foreach (PatriciaTrie<T> child in Children)
            {
                earliestDiscrepancy = -1;
                for (int i = 0; i < Math.Min(key.Length - Key.Length, child.Key.Length); i++)
                {
                    if (key[i + Key.Length].CompareTo(child.Key[i]) != 0) // x.CompareTo(y) returns 0 if x == y.
                    {
                        earliestDiscrepancy = i;
                        break;
                    }
                }

                if (earliestDiscrepancy == 0)
                {
                    // This is not the branch you're looking for.
                    continue;
                }
                else
                {
                    match = child;
                    break;
                }
            }

            if (match != null)
            {
                Debug.Assert(earliestDiscrepancy != 0, "Invalid branch selection while inserting into Patricia Trie.");

                if (earliestDiscrepancy == -1)
                {
                    if (key.Length - Key.Length >= match.Key.Length)
                    {
                        // This is the branch you're looking for.  Hand off to child.
                        // [fig->ht + fighter] becomes [fig->ht->er]
                        //  0    1    2                 0    1   2
                        match.InsertInternal(key.Skip(Key.Length).ToArray(), t);
                    }
                    else
                    {
                        // This is the branch you're looking for.
                        // Create a new node, and stick it between this node and the child node.
                        // Adjust keys appropriately.
                        // [fig->hter + fight] becomes [fig->ht->er]
                        //  0    1      2               0    2   1
                        match.Key = match.Key.Skip(key.Length - Key.Length).ToArray();
                        PatriciaTrie<T> newTrie = new PatriciaTrie<T>() { Key = key.Skip(Key.Length).ToArray() };
                        newTrie.TValue.Add(t);
                        newTrie.Children.Add(match);
                        Children.Add(newTrie);
                        Children.Remove(match);
                    }
                }
                else
                {
                    // We have some common characters.  Create a first new node with those common characters as its Key,
                    // and insert child and a second new node as children of the first new node.
                    // [halt->er + halted] becomes [halt->e->r] (branch one)
                    //  0     1    2                0     3  1
                    //                             [halt->e->d] (branch two)
                    //                              0     3  2
                    PatriciaTrie<T> newTrieA = new PatriciaTrie<T>() { Key = match.Key.Take(earliestDiscrepancy).ToArray() };
                    PatriciaTrie<T> newTrieB = new PatriciaTrie<T>() { Key = key.Skip(earliestDiscrepancy + Key.Length).ToArray() };
                    newTrieB.TValue.Add(t);
                    match.Key = match.Key.Skip(earliestDiscrepancy).ToArray();
                    newTrieA.Children.Add(newTrieB);
                    newTrieA.Children.Add(match);
                    Children.Remove(match);
                    Children.Add(newTrieA);
                }
            }
            else // match == null
            {
                // No appropriate branch was found.  Create a new one.
                // [halt->er + halts] becomes [halt->er] (branch one)
                //  0     1    2               0     1
                //                            [halt->s ] (branch two)
                //                             0     2
                PatriciaTrie<T> newTrie = new PatriciaTrie<T>() { Key = key.Skip(Key.Length).ToArray() };
                newTrie.TValue.Add(t);
                Children.Add(newTrie);
            }
        }

        /// <summary>
        /// Finds all T values associated with a given query.  If more than one word is found in the query string, Search() returns
        /// all T values associated with any of those words. Ignores non-alphanumeric, non-hyphen, non-space characters.
        /// </summary>
        public IEnumerable<T> Search(string query)
        {
            if (String.IsNullOrEmpty(query))
                return new List<T>();

            // Split key into words: "political science" -> {"political", "science"}
            IEnumerable<Char[]> formattedKeys = new string(query.ToLowerInvariant()
                                                                .ToCharArray()
                                                                .Where(c => char.IsLetterOrDigit(c) ||
                                                                            c == ' ' ||
                                                                            c == '-')
                                                                .ToArray())
                                                                .Split(' ')
                                                                .Select(x => x.ToCharArray());

            HashSet<T> found = new HashSet<T>();
            foreach (Char[] formattedKey in formattedKeys)
            {
                foreach (T returned in SearchInternal(formattedKey))
                {
                    if (!found.Contains(returned))
                    {
                        found.Add(returned);
                    }
                }
            }
            return found;
        }

        private IEnumerable<T> SearchInternal(Char[] query)
        {
            if (Key.SequenceEqual(query))
            {
                // The query is in the trie
                return TValue;
            }

            foreach (PatriciaTrie<T> child in Children)
            {
                bool edgeMatches = true;
                for (int i = 0; i < Math.Min(query.Length - Key.Length, child.Key.Length); i++)
                {
                    if (query[i + Key.Length].CompareTo(child.Key[i]) != 0) // 0 signifies equality.
                    {
                        edgeMatches = false;
                        break;
                    }
                }

                if (edgeMatches)
                {
                    // Child may have query.  Continue searching
                    return child.SearchInternal(query.Skip(Key.Length).ToArray());
                }
            }

            // The query is not in the trie
            return new List<T>();
        }
    }
}
