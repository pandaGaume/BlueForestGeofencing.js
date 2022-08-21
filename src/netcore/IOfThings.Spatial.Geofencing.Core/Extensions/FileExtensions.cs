using IOfThings.Spatial.Geofencing.Text.Json;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IOfThings.Spatial.Geofencing
{
    public static class FileExtensions
    {
        public static IEnumerable<T> OpenGeofences<T>(this FileInfo f, ILogger logger = null) where T: IGeofence
        {
            if (!f.Exists)
            {
                logger?.LogWarning("File not found {n}", f.FullName);
                return Enumerable.Empty<T>();
            }
            try
            {
                using (Stream input = f.OpenRead())
                {
                    string json = null;
                    using (StreamReader reader = new StreamReader(input))
                    {
                        json = reader.ReadToEnd();
                    }
                    return GeofencingJsonSerializer.Deserialize<T>(json);
                }
            }
            catch (Exception e)
            {
                logger?.LogError(e, "Unable to read {n}", f.FullName);
                return Enumerable.Empty<T>();
            }
        }
    }
}
