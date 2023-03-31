using MediaCore.Database;
using MediaCore.Model;
using Microsoft.EntityFrameworkCore;

namespace MediaCore.Services
{
    public class ImageService
    {

        public ImageService()
        {
        }

        public byte[]? GetImageData(Guid resource)
        {
            using var context = new MediaContext();
            var image = context.MediaItems.Include(s=>s.MediaData).FirstOrDefault(s => s.Resource == resource);
            return image?.MediaData?.Content ?? null;
        }

        public async Task<(MediaType, Stream)> GetImageByteStream(Guid id)
        {
            await using var context = new MediaContext();
            await context.Database.OpenConnectionAsync();
            try
            {
                var conn = context.Database.GetDbConnection();
                var cmd = conn.CreateCommand();
                var parm = cmd.CreateParameter();
                cmd.Parameters.Add(parm);
                parm.ParameterName = "@id";
                parm.Value = id;
                cmd.CommandText = "SELECT mi.Type, Content FROM  MediaItem mi JOIN MediaData md ON mi.MediaDataId = md.Id WHERE mi.Resource = @id";
                await using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var t = reader.GetInt32(0);
                    return ((MediaType)t, reader.GetStream(1));
                }
            }
            finally
            {
                await context.Database.CloseConnectionAsync();
            }

            return (MediaType.Jpeg, null);
        }

        public async Task<Guid> AddImage(byte[] data, string name, MediaType type)
        {
            var hash = Hasher.CreateHash(data);

            await using var context = new MediaContext();

            var existingImageData = await context.MediaDatas.FirstOrDefaultAsync(s => s.Hash.SequenceEqual(hash));

            if (existingImageData != default)
            {
                var newImage = new Database.MediaItem()
                {
                    MediaData = existingImageData,
                    Name = name,
                    Type = type,
                    Resource = Guid.NewGuid(),
                };
                context.MediaItems.Add(newImage);
                await context.SaveChangesAsync();
                return newImage.Resource;
            }
            else
            {
                var newImageData = new MediaData() { Content = data, Hash = hash, ContentLength = data.Length };
                await context.MediaDatas.AddAsync(newImageData);

                var newImage = new Database.MediaItem()
                {
                    MediaData = newImageData,
                    Name = name,
                    Type = type,
                    Resource = Guid.NewGuid()
                };

                await context.MediaItems.AddAsync(newImage);
                await context.SaveChangesAsync();
                return newImage.Resource;
            }
        }

        public async Task<IList<Image>> GetImages()
        {
            await using var context = new MediaContext();

            return Mapper.Map(await context.MediaItems.Include(s=>s.MediaData).ToListAsync());
        }

        public async Task<bool> Delete(Guid resource)
        {
            await using var context = new MediaContext();

            // Load the item to delete, including the Data
            var itemToDelete = await context.MediaItems.Include(s=>s.MediaData).FirstOrDefaultAsync(s => s.Resource == resource);

            if (itemToDelete == null)
            {
                return false;
            }

            // If the item to delete is the only one using the data, delete the data as well
            if (itemToDelete.MediaData != null)
            {
                if (await context.MediaItems.CountAsync(s => s.MediaDataId == itemToDelete.MediaDataId) == 1)
                {
                    // only remove if the item is the only one using the data, means count == 1
                    context.MediaDatas.Remove(itemToDelete.MediaData);
                }
            }

            // delete the item.
            context.MediaItems.Remove(itemToDelete);
            await context.SaveChangesAsync();

            return false;
        }
    }
}
