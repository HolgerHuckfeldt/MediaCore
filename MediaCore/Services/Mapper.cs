using MediaCore.Model;

namespace MediaCore.Services
{
    public class Mapper
    {
        public static Model.Image Map(Database.MediaItem i)
        {
            var result = new Model.Image()
            {
                Name = i.Name,
                Type = (ImageType)i.Type,
                Id = i.Id,
                MandantId = 0,
                Resource = i.Resource
            };

            if (i.MediaData != null)
            {
                result.Content = i.MediaData.Content;
            }

            return result;
        }

        public static List<Model.Image> Map(IEnumerable<Database.MediaItem> images)
        {
            var result = new List<Model.Image>();
            foreach (var i in images)
            {
                result.Add(Map(i));
            }
            return result;
        }
    }
}
