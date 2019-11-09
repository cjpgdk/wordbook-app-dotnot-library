using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace wordbook.cjpg.app.client
{
    /// <summary>
    /// Class Suggestions
    /// </summary>
    /// <seealso cref="WordbookClient.Suggestions(string, string)"/>
    [DataContract]
    public class Suggestions
    {
        /// <summary>
        /// A <see cref="List{Suggestion}"/> object
        /// </summary>
        /// <seealso cref="Suggestion"/>
        [DataMember(Name = "suggestions")]
        public List<Suggestion> Items { get; set; }
    }
    /// <summary>
    /// Class Suggestion
    /// </summary>
    /// <seealso cref="SuggestionData"/>
    [DataContract]
    public class Suggestion
    {
        /// <summary>
        /// Additional <see cref="SuggestionData"/> for the word.
        /// </summary>
        /// <seealso cref="SuggestionData"/>
        [DataMember(Name = "data")]
        public SuggestionData Data { get; set; }
        /// <summary>
        /// The word or phrase.
        /// </summary>
        [DataMember(Name = "value")]
        public string Word { get; set; }
        /// <summary>
        /// Get the definitions for this word!
        /// </summary>
        /// <param name="destinationLanguageId"></param>
        /// <returns>A <see cref="Definitions"/> object</returns>
        public async Task<Definitions> GetDefinitions(int destinationLanguageId = 0)
        {
            Definitions definitions;
            if (WordbookClient.CurrentInstance == null)
            {
                definitions = await(new WordbookClient()).Definitions(this.Data.WordId, null, this.Data.LanguageId, destinationLanguageId);
            }
            else
            {
                definitions = await WordbookClient.CurrentInstance.Definitions(this.Data.WordId, null, this.Data.LanguageId, destinationLanguageId);
            }
            return definitions;
        }
    }
    /// <summary>
    /// Class SuggestionData
    /// </summary>
    [DataContract]
    public class SuggestionData
    {
        /// <summary>
        /// Id if the word.
        /// </summary>
        [DataMember(Name = "word_id")]
        public int WordId { get; set; }
        /// <summary>
        /// The word or phrase.
        /// </summary>
        [DataMember(Name = "word")]
        public string Word { get; set; }
        /// <summary>
        /// Full name of the source language.
        /// </summary>
        [DataMember(Name = "language")]
        public string Language { get; set; }
        /// <summary>
        /// Id of the source language
        /// </summary>
        [DataMember(Name = "language_id")]
        public int LanguageId { get; set; }
    }

}
