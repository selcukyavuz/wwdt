using System;
using Newtonsoft.Json;

namespace Company.Function
{   public class Entry
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "parentId")]
        public string ParentId { get; set; }

        [JsonProperty(PropertyName = "partitionKey")]
        public string PartitionKey { get; set; }

        [JsonProperty(PropertyName = "who")]
        public string Who { get; set; }

        [JsonProperty(PropertyName = "when")]
        public DateTime When { get; set; }

        [JsonProperty(PropertyName = "what")]
        public string What { get; set; }

        [JsonProperty(PropertyName = "currentEntity")]
        public Entity CurrentEntity { get; set; }

        [JsonProperty(PropertyName = "oldEntity")]
        public Entity OldEntity { get; set; }

        [JsonProperty(PropertyName = "ip")]
        public string Ip { get; set; }

        [JsonProperty(PropertyName = "computerName")]
        public string ComputerName { get; set; }

        [JsonProperty(PropertyName = "ticketNumber")]
        public string TicketNumber { get; set; }

        [JsonProperty(PropertyName = "application")]
        public string Application { get; set; }

        public string _ts { get; set; }

        [JsonProperty(PropertyName = "timeStamp")]
        public DateTime? TimeStamp
        { 
            get
            {
                return double.TryParse(_ts, out double ts) ? ts.UnixTimeStampToDateTimeNullable() : null;
            }
        }
        
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
