using System;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Plato.Internal.Layout.Alerts
{

    public class AlertInfoConverter : JsonConverter
    {

        private readonly HtmlEncoder _htmlEncoder;

        public AlertInfoConverter(HtmlEncoder htmlEncoder)
        {
            _htmlEncoder = htmlEncoder;
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {

            var jo = JObject.Load(reader);
            
            var notifyEntry = new AlertInfo
            {
                Message = new HtmlString(jo.Value<string>("Message"))
            };

            if (Enum.TryParse(jo.Value<string>("Type"), out AlertType type))
            {
                notifyEntry.Type = type;
            }

            return notifyEntry;

        }

        public override void WriteJson(
            JsonWriter writer,
            object value, 
            JsonSerializer serializer)
        {
            var alertInfo = value as AlertInfo;
            if (alertInfo == null) return;
            
            var sb = new StringBuilder();
            using (var stringWriter = new StringWriter(sb))
            {
                alertInfo.Message.WriteTo(stringWriter, _htmlEncoder);
            }

            var o = new JObject
            {
                new JProperty(nameof(AlertInfo.Message), sb.ToString()),
                new JProperty(nameof(AlertInfo.Type), alertInfo.Type.ToString())
            };

            o.WriteTo(writer);

        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(AlertInfo).IsAssignableFrom(objectType);
        }


    }
}
