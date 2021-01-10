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
                throw new BadRequestException("Request does not fulfill expected format.");
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
                throw new BadRequestException("Missing GUID.");
            }
            catch (FormatException)
            {
                throw new BadRequestException("Parameter \"" + guid + "\"is not a valid GUID.");
            }
        }
    }
}