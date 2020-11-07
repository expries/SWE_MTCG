using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MTCG.Server
{
    // matching uri against uri patterns
    // uri pattern example: /users/{userId}/purchases/{purchaseId}
    // {userId}, {purchaseId} are placeholders for (alphanumeric) values
    public static class UriMatcher
    {
        // get values of placeholders that are set in uri pattern
        public static Dictionary<string, string> GetParameters(string uri, string uriPattern)
        {
            var uriParameters = new Dictionary<string, string>();
            var placeholders = GetPlaceholders(uriPattern);
            var match = MatchAgainstPattern(uri, uriPattern);

            // uri matches pattern and there is a regex match for every placeholder
            if (!match.Success || match.Groups.Values.Count() != placeholders.Count + 1)
            {
                return uriParameters;
            }
            
            for (int j = 0; j < placeholders.Count; j++)
            {
                string name = placeholders[j];
                string value = match.Groups.Values.ElementAt(j + 1).Value;
                uriParameters[name] = value;
            }
            
            return uriParameters;
        }

        // get regex match of uri against pattern
        // every placeholder in pattern is replaced with a regex group
        // and then the uri is matched against it
        public static Match MatchAgainstPattern(string uri, string uriPattern)
        {
            var placeholderNames = GetPlaceholders(uriPattern);
            foreach (string name in placeholderNames)
            {
                string placeholder = "{" + name + "}";
                uriPattern = uriPattern.Replace(placeholder, "([a-zA-Z0-9]+)");
            }
            uriPattern = "^" + uriPattern + "$";
            return Regex.Match(uri, uriPattern);
        }

        // get all the placeholders in a uri pattern
        private static IList<string> GetPlaceholders(string uriPattern)
        {
            var parameters = new List<string>();
            string placeholder;
            
            while ((placeholder = GetNextPlaceholder(uriPattern)) != null)
            {
                string paramName = placeholder.Substring(1, placeholder.Length - 2);
                uriPattern = uriPattern.Replace("{" + paramName + "}", "");
                parameters.Add(paramName);
            }
            
            return parameters;
        }

        // get next placeholder in uri pattern
        private static string GetNextPlaceholder(string uriPattern)
        {
            int i = uriPattern.IndexOf("{", StringComparison.Ordinal);
            if (i < 0) return null;
            string pastSegment = uriPattern.Substring(i);
            int j = pastSegment.IndexOf("}", StringComparison.Ordinal);
            if (j < 0) return null;
            return uriPattern.Substring(i, j + 1);
        }
    }
}