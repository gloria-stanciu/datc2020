using System;
using Google.Apis.Drive.v3;
using Google.Apis.Auth.OAuth2;
using System.Threading;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System.IO;
using System.Threading.Tasks;
using System.Net;

namespace L03
{
    class Program
    {
        public static DriveService _service;
        public static string _token;
        static void Main(string[] args)
        {
            init();
            GetMyFiles();

        }

        static void init(){
            string[] scopes = new string[]{
                DriveService.Scope.Drive,
                DriveService.Scope.DriveFile
            };

            var clientId = "376043208932-om5l1cbhihogq5o7dp38q8u7dsnqhipt.apps.googleusercontent.com";
            var clientSecret = "2RblI3AUe_5QeM7T_-7g6EmUf";

            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets{
                    ClientId = clientId,
                    ClientSecret = clientSecret
                },
                scopes,
                Environment.UserName,
                CancellationToken.None,
                null
            ).Result;

            _service = new DriveService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = credential
            });
            _token  = credential.Token.AccessToken;
            Console.WriteLine("Token: " +  _token);
        }
        static void GetMyFiles()
        {
            var request = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/drive/v3/files?q='root'%20in%20parents");
            request.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + _token );
            using(var response = request.GetResponse())
            {
                using(Stream data = response.GetResponseStream())
                using (var reader = new StreamReader(data))
                {
                    string text = reader.ReadToEnd();
                    var myData = JObject.Parse(text);
                    foreach(var file in myData["files"])
                    {
                        if(file["mimeType"].ToString()!="application/vnd/google-apps.folder")
                        {
                            Console.WriteLine("File name: " + file["name"]);
                        }
                    }
                }
            }
        }

        public static async Task<Google.Apis.Drive.v3.Data.File> Upload(IFormFile file, string documentId)
        {
            // var name = ($"{DateTime.UtcNow.ToString()}.{Path.GetExtension(file.FileName)}");
            var name = file.Name;
            var mimeType = file.ContentType;

            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = name,
                MimeType = mimeType,
                Parents = new[] { documentId }
            };

            FilesResource.CreateMediaUpload request;
            using(var stream = file.OpenReadStream())
            {
                request = _service.Files.Create(
                    fileMetadata, stream, mimeType
                );
                request.Fields = "id, name, parents, createdTime, modifiedTime, mimeType, thumbnailLink";
                await request.UploadAsync();
            }
            return request.ResponseBody;
        }
    }
}
