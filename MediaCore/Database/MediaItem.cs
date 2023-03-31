using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MediaCore.Database
{
    public class MediaItem
    {

        /// <summary>
        /// Die Datenbank-ID
        /// </summary>
        [JsonIgnore]
        public int Id { get; set; }

        /// <summary>
        /// Die Resource identifiziert ein Image eindeutig im System.
        /// Die Resource wird zum abrufen des Images benutzt.
        /// </summary>
        public Guid Resource { get; set; }

        /// <summary>
        /// Der Image-Type: Jpeg, Png, Gif oder pdf.
        /// </summary>
        public MediaType Type { get; set; }


        public MediaData? MediaData { get; set; }

        public int MediaDataId { get; set; }

        /// <summary>
        /// Name des Images.
        /// Der Name ist optional und wird nicht wirklich gebraucht, da ein Image über seine Resource identifiziert wird.
        /// </summary>
        [StringLength(500)]
        public string Name { get; set; }

    }
}
