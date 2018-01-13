using System;
using System.IO;
using System.Web.Mvc;
using MicroCreations.Batch.Common.Operations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MicroCreations.Batch.Mvc
{
    public class MicroCreationsBatchModelBinder : DefaultModelBinder
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public MicroCreationsBatchModelBinder()
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            };
            jsonSerializerSettings.Converters.Add(new StringEnumConverter());

            _jsonSerializerSettings = jsonSerializerSettings;
        }

        public MicroCreationsBatchModelBinder(JsonSerializerSettings jsonSerializerSettings)
        {
            _jsonSerializerSettings = jsonSerializerSettings;
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var modelType = bindingContext.ModelType;
            var request = controllerContext.HttpContext.Request;
            var isContentTypeJson = request.ContentType.Equals("application/json", StringComparison.OrdinalIgnoreCase);

            if (!isContentTypeJson || modelType != typeof(BatchOperationRequest) && modelType != typeof(BatchOperationResponse))
            {
                return base.BindModel(controllerContext, bindingContext);
            }

            using (var sr = new StreamReader(request.InputStream))
            {
                if (modelType == typeof(BatchOperationRequest))
                {
                    return JsonConvert.DeserializeObject<BatchOperationRequest>(sr.ReadToEnd(), _jsonSerializerSettings);
                }

                return JsonConvert.DeserializeObject<BatchOperationResponse>(sr.ReadToEnd(), _jsonSerializerSettings);
            }
        }
    }
}
