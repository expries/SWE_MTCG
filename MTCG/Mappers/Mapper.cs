using MTCG.Exceptions;
using Newtonsoft.Json;

namespace MTCG.Mappers
{
    public static class Mapper
    {
        public static TObj MapJsonTo<TObj>(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<TObj>(json);
            }
            catch
            {
                throw new BadRequestException();
            }
        }
    }
}