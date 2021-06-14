using Firebase.Auth;
using Firebase.Storage;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AVC.Service
{
    public static class FirebaseService
    {
        private static readonly string ApiKey = "AIzaSyCsH2SBgyRKIyhBUOaWBNADX78ZaDfXtMw";
        private static readonly string Bucket = "avc-project-2bcf5.appspot.com";
        private static readonly string AuthEmail = "avc@gmail.com";

        public static async Task<string> UploadFileToFirebaseStorage(Stream stream, string filename, string folder, IConfiguration config, string oldFileUrl = null)
        {
            string authPassword = config["FirebaseAuthPassword"];
            string uploadedFileLink = string.Empty;

            // of course you can login using other method, not just email+password
            FirebaseAuthProvider auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
            FirebaseAuthLink a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, authPassword);

            // you can use CancellationTokenSource to cancel the upload midway
            CancellationTokenSource cancellation = new CancellationTokenSource();

            //Start Delete Old File
            if (oldFileUrl != null && oldFileUrl.Contains("firebasestorage.googleapis.com"))
            {
                string subString = oldFileUrl.Substring(oldFileUrl.IndexOf("AVCStorage"));
                Task delTask = new FirebaseStorage(
               Bucket,
               new FirebaseStorageOptions
               {
                   AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                   ThrowOnCancel = true // when you cancel the upload, exception is thrown. By default no exception is thrown
               })
               .Child("AVCStorage").Child(folder)
               .Child(oldFileUrl)
               .DeleteAsync();
            }
                //Finish Delete Old File

                FirebaseStorageTask task = new FirebaseStorage(
               Bucket,
               new FirebaseStorageOptions
               {
                   AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                   ThrowOnCancel = true // when you cancel the upload, exception is thrown. By default no exception is thrown
               })
               .Child("AVCStorage").Child(folder)
               .Child(filename)
               .PutAsync(stream, cancellation.Token);

            uploadedFileLink = await task;


            return uploadedFileLink;
        }
    }
}
