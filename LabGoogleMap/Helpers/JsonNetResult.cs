
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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

        //public override void ExecuteResult(ControllerContext context)
        //{
        //    if (context == null)
        //        throw new ArgumentNullException("context");

        //    HttpResponseBase response = context.HttpContext.Response;

        //    response.ContentType = !string.IsNullOrEmpty(ContentType)
        //      ? ContentType
        //      : "application/json";

        //    if (ContentEncoding != null)
        //        response.ContentEncoding = ContentEncoding;

        //    if (Data != null)
        //    {
        //        JsonTextWriter writer = new JsonTextWriter(response.Output) { Formatting = Formatting };
        //        JsonSerializer serializer = JsonSerializer.Create(SerializerSettings);
        //        serializer.Serialize(writer, Data);

        //        writer.Flush();
        //    }
        //}
    }
}
