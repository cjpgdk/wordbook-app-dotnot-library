using System.Collections.Generic;
using System.Runtime.Serialization;

namespace wordbook.cjpg.app.client
{
    /// <summary>
    /// Class Definitions. This class is just a wrapper for the list of definitions
    /// </summary>
    /// <seealso cref="Definition"/>
    public class Definitions
    {
        /// <summary>
        /// The <see cref="Definition"/> list.
        /// </summary>
        /// <example>
        /// <code>
        /// Definitions definitions = ....
        /// /*
        ///  * dictionary-id == 'Source language ID'-'Destination language ID'
        ///  */
        /// List&lt;Definition> definitionsFromDict = definitions.Items["dictionary-id"];
        /// </code>
        /// </example>
        public Dictionary<string, List<Definition>> Items { get; set; }
    }

    [DataContract]
    public class Definition
    {
        [DataMember]
        public string definition { get; set; }
        [DataMember]
        public int dest_language_id { get; set; }
        [DataMember]
        public string dictionary { get; set; }
        [DataMember]
        public int src_language_id { get; set; }
        [DataMember]
        public int word_id { get; set; }
    }

}
