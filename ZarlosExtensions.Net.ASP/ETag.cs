using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace ZarlosExtensions.Net.ASP
{
    /// <summary>
    /// added a ETag base on the output
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class,
             AllowMultiple = false)]
    public class ETag : ActionFilterAttribute
    {

        public enum HashingAlgorithm {
            None,
            MD5,
            SHA1
        }

        private HashingAlgorithm UseHashingAlgorithm = HashingAlgorithm.None;

        public ETag(HashingAlgorithm UseHashingAlgorithm) {
            this.UseHashingAlgorithm = UseHashingAlgorithm;
        }
        private Func<IActionResult, string> MakeETagString { get; set; } = null;

        public ETag(Func<IActionResult, string> MakeETag) {
            this.MakeETagString = MakeETag;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var request = context.HttpContext.Request;
            var response = context.HttpContext.Response;
            if (
                request.Method == HttpMethod.Get.Method &&
                response.StatusCode == (int)HttpStatusCode.OK
                )
            {
                // from the response body
                var etag = GenerateHashETag(context.Result);                //fetch etag from the incoming request header
                if (request.Headers.Keys.Contains(HeaderNames.IfNoneMatch))
                {
                    var incomingEtag =
                                  request.Headers[HeaderNames.IfNoneMatch]
                                      .ToString();                    // if both the etags are equal
                    // raise a 304 Not Modified Response
                    if (incomingEtag.Equals(etag))
                    {
                        context.Result =
                                  new StatusCodeResult(
                                  (int)HttpStatusCode.NotModified);
                    }
                }                // add ETag response header 
                response.Headers.Add(HeaderNames.ETag, new[] { etag });
            }            
            base.OnActionExecuted(context);
        }        
        private string GenerateHashETag(IActionResult response)
        {

            var responseString = JsonConvert.SerializeObject(response);
            
            if(this.MakeETagString != null) {
                return this.MakeETagString(response);
            }

            byte[] hash = null;
            byte[] responsebytes = UTF8Encoding.UTF8.GetBytes(responseString);
            if(this.UseHashingAlgorithm == HashingAlgorithm.SHA1) {
                using SHA1 sha1 = SHA1Managed.Create();
                hash = sha1.ComputeHash(responsebytes);
            }

            if(this.UseHashingAlgorithm == HashingAlgorithm.MD5) {
                using MD5 md5 = MD5.Create();
                hash = md5.ComputeHash(responsebytes);
            }

            StringBuilder formatted = new StringBuilder(2 * hash.Length);
            foreach (byte b in hash)
            {
                formatted.AppendFormat("{0:X2}", b);
            }
            return formatted.ToString();
        }
    }
}
