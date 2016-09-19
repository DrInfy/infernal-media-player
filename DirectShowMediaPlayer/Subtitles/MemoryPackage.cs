using System;
using System.IO;
using System.IO.Packaging;
using System.Threading;

namespace Imp.DirectShow.Subtitles
{
    internal sealed class MemoryPackage : IDisposable
    {
        private static int packageCounter;

        private readonly Uri packageUri = new Uri("payload://memorypackage" + Interlocked.Increment(ref packageCounter), UriKind.Absolute);
        private readonly Package package = Package.Open(new MemoryStream(), FileMode.Create);
        private int partCounter;

        //public MemoryPackage(string code)
        //{
        //    PackageStore.AddPackage(new Uri("payload://" + code , UriKind.Absolute), this.package);
        //}

        public MemoryPackage()
        {
            PackageStore.AddPackage(this.packageUri, this.package);
        }

        public Uri CreatePart(Stream stream, string addition)
        {
            return this.CreatePart(stream, "application/octet-stream", addition);
        }

        public Uri CreatePart(Stream stream, string contentType, string addition)
        {
            var partUri = new Uri("/stream" + (++this.partCounter) + addition , UriKind.Relative);

            var part = this.package.CreatePart(partUri, contentType);

            using (var partStream = part.GetStream())
                CopyStream(stream, partStream);

            // Each packUri must be globally unique because WPF might perform some caching based on it.
            return PackUriHelper.Create(this.packageUri, partUri);
        }

        public void DeletePart(Uri packUri)
        {
            this.package.DeletePart(PackUriHelper.GetPartUri(packUri));
        }

        public void Dispose()
        {
            PackageStore.RemovePackage(this.packageUri);
            this.package.Close();
        }

        private static void CopyStream(Stream source, Stream destination)
        {
            const int bufferSize = 4096;

            byte[] buffer = new byte[bufferSize];
            int read;
            while ((read = source.Read(buffer, 0, buffer.Length)) != 0)
                destination.Write(buffer, 0, read);
        }
    }
}
