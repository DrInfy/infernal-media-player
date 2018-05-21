#region Usings

using Newtonsoft.Json;

#endregion

namespace Imp.MpvPlayer.Containers
{
    public class BaseTrack
    {
        #region  Public Fields and Properties

        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "src-id")]
        public int srcId { get; set; }

        [JsonProperty(PropertyName = "lang")]
        public string lang { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "demux-w")]
        public int? Width { get; set; }

        [JsonProperty(PropertyName = "demux-h")]
        public int? Height { get; set; }

        [JsonProperty(PropertyName = "demux-fps")]
        public double? Fps { get; set; }

        [JsonProperty(PropertyName = "demux-channel-count")]
        public int? Channels { get; set; }

        [JsonProperty(PropertyName = "selected")]
        public bool IsSelected { get; set; }

        #endregion
    }
}