using System;
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

        public static Guid MapToGuid(string guid)
        {
            try
            {
                return Guid.Parse(guid);
            }
            catch
            {
                throw new BadRequestException();
            }
        }
    }
}