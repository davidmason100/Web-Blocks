﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebBlocks.Models.Angular;
using WebBlocks.Serialisation.JavascriptConverters;

namespace WebBlocks.Providers
{
    /// <summary>
    /// Provides access to the current page containers
    /// </summary>
    public class LayoutBuilderProvider
    {
        public AngularLayoutBuilderModel LayoutBuilder { get; set; }

        /// <summary>
        /// Initialises and loads the current page containers
        /// </summary>
        public LayoutBuilderProvider(string webBlocksValue)
        {
            if (!string.IsNullOrEmpty(webBlocksValue))
            {
                var jObject = JsonConvert.DeserializeObject(webBlocksValue);
                var jsonSerialiser = new JsonSerializer();
                jsonSerialiser.Converters.Add(new IBlockConverter());
                jsonSerialiser.Converters.Add(new IContainerConverter());
                jsonSerialiser.Converters.Add(new IBlockElementAttributeConverter());
                jsonSerialiser.Converters.Add(new IContainerPermissionsConverter());
                LayoutBuilder = ((JObject)jObject).ToObject<AngularLayoutBuilderModel>(jsonSerialiser);
            }
            else
                LayoutBuilder = new AngularLayoutBuilderModel();
        }

        /// <summary>
        /// Get a container by id
        /// </summary>
        /// <param name="name">name of the container</param>
        /// <returns>The container</returns>
        public Container ContainerByName(string name)
        {
            if (LayoutBuilder == null) return null;
            Container container = null;
            LayoutBuilder.Containers.TryGetValue(name, out container);
            return container;
        }
    }
}