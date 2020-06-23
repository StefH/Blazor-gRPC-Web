using Fileupload;
using Grpc.Core;
using System;
using System.Threading.Tasks;

namespace Server.Services
{
    public class UploadFileService : FileUpload.FileUploadBase
    {
        public override async Task<UploadResponse> Upload(UploadRequest request, ServerCallContext context)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));

            int x = 0;
            
            var bytes = request.Content.ToByteArray();

            return new UploadResponse { Size = bytes.Length };
        }




        //public override async Task StartCounter(CounterRequest request, IServerStreamWriter<CounterResponse> responseStream, ServerCallContext context)
        //{
        //    var count = request.Start;

        //    while (!context.CancellationToken.IsCancellationRequested)
        //    {
        //        await responseStream.WriteAsync(new CounterResponse
        //        {
        //            Count = ++count
        //        });

        //        await Task.Delay(TimeSpan.FromSeconds(1));
        //    }
        //}
    }
}
