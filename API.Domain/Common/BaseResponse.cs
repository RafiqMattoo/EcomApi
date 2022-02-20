using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Domain
{
    public class BaseResponse<T>
    {
        [JsonProperty(PropertyName = "code")]
        [Description("Status Code")]
        [Required]
        public int Code { get; set; }

        /// <summary>
        /// Contains message based on code
        /// </summary>
        [Description("Contains message based on code")]
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        /// <summary>
        /// Contains data section
        /// </summary>
        [Description("Contains data section")]
        [JsonProperty(PropertyName = "data")]
        public T Data { get; set; }


    }
}
