using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MediaCore.Model
{

    public enum ImageType
    {
        Jpeg,
        Png,
        Gif,
        PDF,
        Svg,
        Unknown

    }
    public class Image
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
        public ImageType Type { get; set; }

        /// <summary>
        /// Der binäre Inhalt des Bildes / pdf.
        /// </summary>
        [JsonIgnore]
        public byte[] Content { get; set; }

        /// <summary>
        /// Name des Images.
        /// Der Name ist optional und wird nicht wirklich gebraucht, da ein Image über seine Resource identifiziert wird.
        /// </summary>
        [StringLength(500)]
        public string Name { get; set; }

        /// <summary>
        /// Id des
        /// </summary>
        public int MandantId { get; set; }
    }
}
