using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace wordbook.cjpg.app.client
{
    /// <summary>
    /// Class WordbookClient.
    /// </summary>
    public class WordbookClient
    {
        /// <summary>
        /// The current instance of <see cref="WordbookClient"/>
        /// </summary>
        internal static WordbookClient CurrentInstance { get; private set; }
        /// <summary>
        /// Gets the <see cref="System.Net.Http.HttpClient"/> object, for sending requests to the API.
        /// </summary>
        internal static HttpClient WebClient { get; private set; }
        /// <summary>
        /// The base url for API requests
        /// </summary>
        protected const string API_BASE_URL = "https://wordbook.cjpg.app";
        /// <summary>
        /// Api dictionaries path
        /// </summary>
        protected const string API_PATH_DICTIONARIES = "/dictionaries";
        /// <summary>
        /// Api suggestions path
        /// </summary>
        protected const string API_PATH_SUGGESTIONS = "/suggestions";
        /// <summary>
        /// Api definitions path
        /// </summary>
        protected const string API_PATH_DEFINITIONS = "/definitions";
        /// <summary>
        /// The version of this library!
        /// </summary>
        protected const string _VERSION = "1.0";
        /// <summary>
        /// Initializes a new instance of the <see cref="WordbookClient"/> class
        /// </summary>
        public WordbookClient()
        {
            WebClient = new HttpClient();
            WebClient.DefaultRequestHeaders.Add("User-Agent", "Dot.Net-Wordbook-Client/" + _VERSION);
            WebClient.DefaultRequestHeaders.Add("Accept", "application/json");
            CurrentInstance = this;
        }
        /// <summary>
        /// Get suggestions
        /// </summary>
        /// <param name="query"></param>
        /// <param name="dictionaryId"></param>
        /// <returns>A <see cref="Suggestions"/> object</returns>
        public async Task<Suggestions> Suggestions(string query, string dictionaryId = null)
        {
            // build the query args.
            Dictionary<string, string> args = new Dictionary<string, string>() 
            {
                { "query", query }
            }; 
            if (!string.IsNullOrEmpty(dictionaryId) && !string.IsNullOrWhiteSpace(dictionaryId))
            {
                args.Add("language", dictionaryId);
            }
            // call the api
            string response = "";
            using (HttpResponseMessage r = await WordbookClient.WebClient.GetAsync(this.GetApiUri(API_PATH_SUGGESTIONS, args)))
            {
                r.EnsureSuccessStatusCode();
                response = await r.Content.ReadAsStringAsync();
            }
            // quick test the response
            if (string.IsNullOrEmpty(response) || string.IsNullOrWhiteSpace(response))
            {
                return new Suggestions();
            }

            // convert the response to object
            Suggestions suggestions;
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(response)))
            {
                DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings();
                settings.UseSimpleDictionaryFormat = true;
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Suggestions));
                suggestions = serializer.ReadObject(ms) as Suggestions;
            }
            return suggestions;
        }
        /// <summary>
        /// Locate the definition(s) for a word by it's id or the word it self.
        /// </summary>
        /// <param name="wordId">word id</param>
        /// <param name="word">word to lookup (Ignored if <paramref name="wordId"/> > 0), If using the word to lookup the definition both <paramref name="srcLanguageId"/> and <paramref name="destLanguageId"/> must be present in the request!</param>
        /// <param name="srcLanguageId">The id of the source language</param>
        /// <param name="destLanguageId">The id of the destination language</param>
        /// <returns>A <see cref="wordbook.cjpg.app.client.Definitions"/> object</returns>
        /// <seealso cref="wordbook.cjpg.app.client.Definitions"/>
        /// <seealso cref="wordbook.cjpg.app.client.Definition"/>
        public async Task<Definitions> Definitions(int wordId, string word = null, int srcLanguageId = 0, int destLanguageId = 0)
        {
            // build the query args.
            Dictionary<string, string> args = new Dictionary<string, string>();
            if (wordId > 0) 
            {
                args.Add("id", wordId.ToString());
            } 
            else if (!string.IsNullOrEmpty(word) && !string.IsNullOrWhiteSpace(word))
            {
                args.Add("word", word);
            }
            if (srcLanguageId > 0)
            {
                args.Add("src_language_id", srcLanguageId.ToString());
            }
            if (destLanguageId > 0)
            {
                args.Add("dest_language_id", destLanguageId.ToString());
            }

            // call the api
            string response = "";
            using (HttpResponseMessage r = await WordbookClient.WebClient.GetAsync(this.GetApiUri(API_PATH_DEFINITIONS, args)))
            {
                r.EnsureSuccessStatusCode();
                response = await r.Content.ReadAsStringAsync();
            }
            // quick test the response
            if (string.IsNullOrEmpty(response) || string.IsNullOrWhiteSpace(response))
            {
                return new Definitions();
            }

            // convert the response to object
            Dictionary<string, List<Definition>> definitions;
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(response)))
            {
                DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings();
                settings.UseSimpleDictionaryFormat = true;
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Dictionary<string, List<Definition>>), settings);
                definitions = serializer.ReadObject(ms) as Dictionary<string, List<Definition>>;
            }
            return new Definitions() { Items = definitions };
        }
        /// <summary>
        /// Get all available dictionaries.
        /// </summary>
        /// <returns>A <see cref="wordbook.cjpg.app.client.Dictionaries"/> object</returns>
        /// <seealso cref="wordbook.cjpg.app.client.Dictionaries"/>
        /// <seealso cref="wordbook.cjpg.app.client.Dictionary"/>
        public async Task<Dictionaries> Dictionaries()
        {
            // call the api
            string response = "";
            using (HttpResponseMessage r = await WebClient.GetAsync(this.GetApiUri(API_PATH_DICTIONARIES)))
            {
                r.EnsureSuccessStatusCode();
                response = await r.Content.ReadAsStringAsync();
            }
            // quick test the response
            if (string.IsNullOrEmpty(response) || string.IsNullOrWhiteSpace(response))
            {
                return new Dictionaries();
            }

            // convert the response to object
            List<Dictionary> dictionaries;
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(response)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Dictionary>));
                dictionaries = serializer.ReadObject(ms) as List<Dictionary>;
            }
            return new Dictionaries() { Items = dictionaries };
        }
        /// <summary>
        /// Get an <see cref="Uri"/> object to use when making api calls.
        /// </summary>
        /// <param name="path">A <see cref="string"/> object that contains the path of the api.</param>
        /// <param name="args">A <see cref="Dictionary{TKey, TValue}"/> object that contains the query parameters to use for the api request</param>
        /// <returns>A <see cref="Uri"/> object.</returns>
        protected Uri GetApiUri(string path, Dictionary<string, string> args = null)
        {
            string uri = $"{API_BASE_URL}{path}";
            if (args != null)
            {
                uri += "?"+string.Join("&", args.Select(x => {
                    string key = Uri.EscapeDataString(x.Key);
                    string value = Uri.EscapeDataString(x.Value);
                    return $"{key}={value}";
                }));
            }
            return new Uri(uri);
        }
    }
}
