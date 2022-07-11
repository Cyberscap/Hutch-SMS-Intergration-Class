using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cyberscap.Functions
{
    class SMS //@Akalanka1337 -7/2/22
    {
        //Put Token Here
        private string apiBaseURL = "https://bsms.hutch.lk";
        private string accessToken = "";
        private string champName = "CSBase";
        private string smsMask = "Cyberscap";

        public async Task<string> sendSms(string numbers, string msg) //Send New SMS
        {
            string responseInString = "";
            string accessTokenGen = await this.getHutAuthToken(); //Use this to get token using username&Password
            using (var wb = new HttpClient())
            {

                wb.BaseAddress = new Uri(apiBaseURL);
                dynamic data = new ExpandoObject();
                data.campaignName = champName;
                data.mask = smsMask;
                data.numbers = numbers;
                data.content = msg;

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(data);

                wb.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json;");
                wb.DefaultRequestHeaders.Add("Accept", "*/*");
                wb.DefaultRequestHeaders.Add("X-API-VERSION", "v1");
                wb.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessTokenGen);

                var final = new System.Net.Http.StringContent(json, Encoding.UTF8, "application/json");

                var response = await wb.PostAsync("/api/sendsms", final);
                string hook = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                dynamic readData = JObject.Parse(hook);
                if ((int)response.StatusCode == 200 && readData.serverRef != null)
                {
                    responseInString = "Sent";
                }
                else
                {
                    responseInString = "Failed";
                }
            }
            return responseInString;
        }

        public async Task<string> getHutAuthToken() //Get Hutch Auth Token (NSFW) 
        {
            string Huusername = "admin@hutch.lk";
            string Hupassword = "tempPass";
            string responseInString = "";
            using (var wb = new HttpClient())
            {

                wb.BaseAddress = new Uri(apiBaseURL);
                dynamic data = new ExpandoObject();
                data.username = Huusername;
                data.password = Hupassword;

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                wb.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json;");
                wb.DefaultRequestHeaders.Add("Accept", "*/*");
                wb.DefaultRequestHeaders.Add("X-API-VERSION", "v1");
                var final = new System.Net.Http.StringContent(json, Encoding.UTF8, "application/json");

                var response = await wb.PostAsync("/api/login", final);
                string hook = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                dynamic readData = JObject.Parse(hook);
                responseInString = readData.accessToken;
            }
            return responseInString;
        }

    }
}
