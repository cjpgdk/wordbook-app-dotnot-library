using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace wordbook.cjpg.app.client
{
    /// <summary>
    /// Class Dictionaries. This class is just a wrapper for the list of dictionaries
    /// </summary>
    /// <seealso cref="Dictionary"/>
    public class Dictionaries
    {
        /// <summary>
        /// The <see cref="Dictionary"/> list.
        /// </summary>
        public List<Dictionary> Items { get; set; }
    }

    /// <summary>
    /// Class Dictionary
    /// </summary>
    [DataContract]
    public class Dictionary
    {
        private string m_info = null;
        private string m_alphabet = null;
        /// <summary>
        /// The id of the dictionary
        /// </summary>
        [DataMember(Name = "id")]
        public string Id { get; set; }
        /// <summary>
        /// The long verison of the dictionary name
        /// </summary>
        [DataMember(Name = "long")]
        public string LongName { get; set; }
        /// <summary>
        /// The short verison of the dictionary name
        /// </summary>
        [DataMember(Name = "short")]
        public string ShortName { get; set; }
        /// <summary>
        /// Url to the alphabet definition of this dictionary
        /// </summary>
        [DataMember(Name = "alphabet")]
        public string AlphabetUrl { get; set; }
        /// <summary>
        /// Url to the info definition of this dictionary
        /// </summary>
        [DataMember(Name = "info")]
        public string InfoUrl { get; set; }
        /// <summary>
        /// Url to the url definition of this dictionary
        /// </summary>
        [DataMember(Name = "url")]
        public string Url { get; set; }
        /// <summary>
        /// Get the id of the source language of the dictionary
        /// </summary>
        [IgnoreDataMember]
        public int SourceLanguageId
        {
            get
            {
                string[] ids = this.Id.Split('-');
                return int.Parse(ids[0]);
            }
        }
        /// <summary>
        /// Get the id of the destination language of the dictionary 
        /// </summary>
        [IgnoreDataMember]
        public int DestinationLanguageId
        {
            get
            {
                string[] ids = this.Id.Split('-');
                return int.Parse(ids[1]);
            }
        }
        /// <summary>
        /// Get the info of this dictionary.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public async Task<string> Info()
        {
            if (m_info != null)
            {
                return m_info;
            }

            Definitions definitions;
            if (WordbookClient.CurrentInstance == null)
            {
                definitions = await (new WordbookClient()).Definitions(0, "00databaseinfo", this.SourceLanguageId, this.DestinationLanguageId);
            }
            else
            {
                definitions = await WordbookClient.CurrentInstance.Definitions(0, "00databaseinfo", this.SourceLanguageId, this.DestinationLanguageId);
            }

            if (definitions.Items.ContainsKey(this.Id) && definitions.Items[this.Id].Count > 0)
            {
                m_info = definitions.Items[this.Id][0].definition;
            }

            return m_info;
        }
        /// <summary>
        /// Get the alphabet of this dictionary.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public async Task<string> Alphabet()
        {
            if (m_alphabet != null)
            {
                return m_alphabet;
            }

            Definitions definitions;
            if (WordbookClient.CurrentInstance == null)
            {
                definitions = await (new WordbookClient()).Definitions(0, "00databasealphabet", this.SourceLanguageId, this.DestinationLanguageId);
            }
            else
            {
                definitions = await WordbookClient.CurrentInstance.Definitions(0, "00databasealphabet", this.SourceLanguageId, this.DestinationLanguageId);
            }

            if (definitions.Items.ContainsKey(this.Id) && definitions.Items[this.Id].Count > 0)
            {
                m_alphabet = definitions.Items[this.Id][0].definition;
            }

            return m_alphabet;
        }
    }
}