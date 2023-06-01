using Amazon.S3;
using Amazon.S3.Model;
using MoodReboot.Helpers;

namespace MvcCoreAWSS3.Services
{
    public class ServiceStorageS3
    {
        private readonly IAmazonS3 ClientS3;

        public ServiceStorageS3(IAmazonS3 clientS3)
        {
            this.ClientS3 = clientS3;
        }

        public async Task<bool> UploadFileAsync(string fileName, Stream stream, Containers bucket)
        {
            PutObjectRequest request = new()
            {
                BucketName = HelperPathAWS.MapBucketName(bucket),
                InputStream = stream,
                Key = fileName
            };

            PutObjectResponse response = await this.ClientS3.PutObjectAsync(request);

            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> DeleteFileAsync(string fileName, Containers bucket)
        {
            DeleteObjectResponse response = await this.ClientS3.DeleteObjectAsync(HelperPathAWS.MapBucketName(bucket), fileName);

            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<List<Tuple<string, List<string>>>> GetFilesAsync(Containers bucket)
        {
            ListObjectsResponse response = await this.ClientS3.ListObjectsAsync(HelperPathAWS.MapBucketName(bucket));
            List<Tuple<string, List<string>>> filesVersions = new();

            foreach (var file in response.S3Objects)
            {
                var fileVersions = await this.GetFileVersionsAsync(file.Key, bucket);
                filesVersions.Add(Tuple.Create(file.Key, fileVersions));
            }

            return filesVersions;
        }

        public async Task<Stream> GetFileAsync(string fileName, Containers bucket)
        {
            GetObjectResponse response = await this.ClientS3.GetObjectAsync(HelperPathAWS.MapBucketName(bucket), fileName);
            return response.ResponseStream;
        }

        public async Task<List<string>> GetFileVersionsAsync(string fileName, Containers bucket)
        {
            ListVersionsResponse response = await this.ClientS3.ListVersionsAsync(HelperPathAWS.MapBucketName(bucket));
            List<S3ObjectVersion> versions = response.Versions.Where(x => x.Key == fileName).ToList();
            return versions.Select(x => x.VersionId).ToList();
        }

        public async Task DeleteVersionAsync(string fileName, string versionId, Containers bucket)
        {
            DeleteObjectRequest request = new()
            {
                BucketName = HelperPathAWS.MapBucketName(bucket),
                VersionId = versionId,
                Key = fileName
            };

            await this.ClientS3.DeleteObjectAsync(request);
        }
    }
}
