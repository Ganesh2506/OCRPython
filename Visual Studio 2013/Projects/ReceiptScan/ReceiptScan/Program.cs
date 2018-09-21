// This code requires the Nuget package Microsoft.AspNet.WebApi.Client to be installed.
// Instructions for doing this in Visual Studio:
// Tools -> Nuget Package Manager -> Package Manager Console
// Install-Package Microsoft.AspNet.WebApi.Client

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CallRequestResponseService
{
    class Program
    {
        static void Main(string[] args)
        {
            InvokeRequestResponseService().Wait();
        }

        static async Task InvokeRequestResponseService()
        {
            using (var client = new HttpClient())
            {
                var scoreRequest = new
                {
                    Inputs = new Dictionary<string, List<Dictionary<string, string>>>() {
                        {
                            "input1",
                            new List<Dictionary<string, string>>(){new Dictionary<string, string>(){
                                            {
                                                "Categories", ""
                                            },
                                            {
                                                //AIRLINES RPRTING CORPTAF (Available in Training data)
                                              //  "url", "https://az742515.vo.msecnd.net/cdn/receiptuploads/publicishealth/joycesamaras@astrazenecacom/-869879032_127201742859PM.jpg" 
                                             // "url","https://az742515.vo.msecnd.net/cdn/receiptuploads/publicishealth/tracykellum@touchpointsolutionscom/-1973106032_2102017105605AM.jpg"

                                             //AMERICAN AIRLINES INC 
                                            // "url","https://az742515.vo.msecnd.net/cdn/receiptuploads/publicishealth/jaymurray@novartiscom/img_0075_210201753858PM.jpg"

                                            //BVP TENANT, LLC (Available in Training data)
                                            //"url","https://az742515.vo.msecnd.net/cdn/receiptuploads/publicishealth/michellejacuzzi@novartiscom/-1893866032_29201715525PM.jpg"

                                            //BVP TENANT, LLC
                                            //"url","https://az742515.vo.msecnd.net/cdn/receiptuploads/publicishealth/jeannastarkey@novartiscom/image_220201740439PM.jpg"

                                            "url","https://az742515.vo.msecnd.net/cdn/receiptuploads/publicishealth/robertwright@novartiscom/bwright_2-10-17_210201792956PM.pdf"


                                            },
                                            {
                                                "Amount", ""
                                            },
                                }
                            }
                        },
                    },
                    GlobalParameters = new Dictionary<string, string>()
                    {
                    }
                };

                const string apiKey = "PC7NA/tUNXgUOxiVqeSogqfHIWlSR46/iVYLKrGWmh1/qJ8KJfbyr0/5vzwVZGWWNZ9nlgp36CARymV/KaMneQ=="; // Replace this with the API key for the web service
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                client.BaseAddress = new Uri("https://uswestcentral.services.azureml.net/subscriptions/5365a99880ee4ac0b274377d635e967b/services/d9a943b3db784ce597a07cc59c5afc19/execute?api-version=2.0&format=swagger");

                // WARNING: The 'await' statement below can result in a deadlock
                // if you are calling this code from the UI thread of an ASP.Net application.
                // One way to address this would be to call ConfigureAwait(false)
                // so that the execution does not attempt to resume on the original context.
                // For instance, replace code such as:
                //      result = await DoSomeTask()
                // with the following:
                //      result = await DoSomeTask().ConfigureAwait(false)

                HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Result: {0}", result);
                    Console.ReadLine();
                }
                else
                {
                    Console.WriteLine(string.Format("The request failed with status code: {0}", response.StatusCode));

                    // Print the headers - they include the requert ID and the timestamp,
                    // which are useful for debugging the failure
                    Console.WriteLine(response.Headers.ToString());

                    string responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseContent);
                    Console.ReadLine();
                }
            }
        }
    }
}

////// This code requires the Nuget package Microsoft.AspNet.WebApi.Client to be installed.
////// Instructions for doing this in Visual Studio:
////// Tools -> Nuget Package Manager -> Package Manager Console
////// Install-Package Microsoft.AspNet.WebApi.Client

////using System;
////using System.Collections.Generic;
////using System.IO;
////using System.Net.Http;
////using System.Net.Http.Formatting;
////using System.Net.Http.Headers;
////using System.Text;
////using System.Threading.Tasks;

////namespace CallRequestResponseService
////{
////    class Program
////    {
////        static void Main(string[] args)
////        {
////            InvokeRequestResponseService().Wait();
////        }

////        static async Task InvokeRequestResponseService()
////        {
////            using (var client = new HttpClient())
////            {
////                var scoreRequest = new
////                {
////                    Inputs = new Dictionary<string, List<Dictionary<string, string>>>() {
////                        {
////                            "input1",
////                            new List<Dictionary<string, string>>(){new Dictionary<string, string>(){
////                                            {
////                                                "Categories", ""
////                                            },
////                                            {
////                                                "url", "https://az742515.vo.msecnd.net/cdn/receiptuploads/garlandind/blambert@garlandindcom/146474264.jpg"
////                                            },
////                                }
////                            }
////                        },
////                    },
////                    GlobalParameters = new Dictionary<string, string>()
////                    {
////                    }
////                };

////                const string apiKey = "xEyO0YCi9ChhzQqqojAduBHo0tW6BmoHrfvBgtKn9kR45Ks8mbO9EeoDm3pi8Y5ZvVKV/YJL9pTJKVhGK2022g=="; // Replace this with the API key for the web service
////                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
////                client.BaseAddress = new Uri("https://uswestcentral.services.azureml.net/subscriptions/5365a99880ee4ac0b274377d635e967b/services/b2e4edb3ae5a4fa2b08d1aa851396cf7/execute?api-version=2.0&format=swagger");

////                // WARNING: The 'await' statement below can result in a deadlock
////                // if you are calling this code from the UI thread of an ASP.Net application.
////                // One way to address this would be to call ConfigureAwait(false)
////                // so that the execution does not attempt to resume on the original context.
////                // For instance, replace code such as:
////                //      result = await DoSomeTask()
////                // with the following:
////                //      result = await DoSomeTask().ConfigureAwait(false)

////                HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest);

////                if (response.IsSuccessStatusCode)
////                {
////                    string result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
////                    Console.WriteLine("Result: {0}", result);
////                    Console.ReadLine();
////                }
////                else
////                {
////                    Console.WriteLine(string.Format("The request failed with status code: {0}", response.StatusCode));

////                    // Print the headers - they include the requert ID and the timestamp,
////                    // which are useful for debugging the failure
////                    Console.WriteLine(response.Headers.ToString());

////                    string responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
////                    Console.WriteLine(responseContent);
////                    Console.ReadLine();
////                }
////            }
////        }
////    }
////}