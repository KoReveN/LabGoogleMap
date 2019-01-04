
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace Microsoft.AspNetCore.Mvc
{
    public class JsonNetResult : ActionResult
    {
        public Encoding ContentEncoding { get; set; }
        public string ContentType { get; set; }
        public object Data { get; set; }

        public JsonSerializerSettings SerializerSettings { get; set; }
        public Formatting Formatting { get; set; }

        public JsonNetResult()
        {
            SerializerSettings = new JsonSerializerSettings();
        }

        public JsonNetResult(object data)
        {
            SerializerSettings = new JsonSerializerSettings();
            this.Data = data;
        }
        /// <summary>
        /// Easy helper for most AJAX calls using JsonNetResult
        /// </summary>
        /// <param name="success"></param>
        /// <param name="message"></param>
        public JsonNetResult(bool success, string message = "")
        {
            SerializerSettings = new JsonSerializerSettings();
            this.Data = new { success, message };
        }

        //public override void ExecuteResult(ControllerContext context) { }

        public override void ExecuteResult(ActionContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            HttpResponse response = context.HttpContext.Response;

            response.ContentType = !string.IsNullOrEmpty(ContentType)
              ? ContentType
              : "application/json";

            if (ContentEncoding != null)
            {
                var mediaType = new MediaTypeHeaderValue("application/json");
                mediaType.Encoding = ContentEncoding;
                response.ContentType = mediaType.ToString();
            }
                

            if (Data != null)
            {
                JsonTextWriter writer = new JsonTextWriter(new StreamWriter(response.Body)) { Formatting = Formatting };
                JsonSerializer serializer = JsonSerializer.Create(SerializerSettings);
                serializer.Serialize(writer, Data);

                writer.Flush();
            }
        }
    }
}
