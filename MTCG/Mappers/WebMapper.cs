using System;
using MTCG.Exceptions;
using Newtonsoft.Json;

namespace MTCG.Mappers
{
    public static class WebMapper
    {
        public static TObject MapJsonTo<TObject>(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<TObject>(json);
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
            catch (ArgumentNullException)
            {
                throw new BadRequestException();
            }
            catch (FormatException)
            {
                throw new BadRequestException();
            }
        }
    }
}