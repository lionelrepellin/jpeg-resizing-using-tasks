using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace JpegResizing
{
    public class JpegResizer
    {
        private string _picturesDirectory;
        private ConcurrentStack<string> _jpegFiles;
        private List<ResizeParameter> _parameters;
        private ImageCodecInfo _imageCodecInfo;

        private const string RESIZE_DIRECTORY = "Resize";

        public int ProcessedFileCount { get; private set; }


        public JpegResizer(string picturesDirectory)
        {
            if (string.IsNullOrEmpty(picturesDirectory))
                throw new ArgumentNullException("picturesDirectory");

            if (!Directory.Exists(picturesDirectory))
                throw new DirectoryNotFoundException(picturesDirectory);

            _picturesDirectory = picturesDirectory;
            Initialize();
        }


        private void Initialize()
        {
            _parameters = new List<ResizeParameter>();
            _imageCodecInfo = GetImageCodecInfo(ImageFormat.Jpeg);
            _jpegFiles = LoadJpegFiles();
            CreateResizeDirectory();
        }


        private ConcurrentStack<string> LoadJpegFiles()
        {
            var jpegFiles = Directory.GetFiles(_picturesDirectory, "*.jpg", SearchOption.TopDirectoryOnly);
            return new ConcurrentStack<string>(jpegFiles);
        }


        private void CreateResizeDirectory()
        {
            var resizeDirectory = Path.Combine(_picturesDirectory, RESIZE_DIRECTORY);

            if (!Directory.Exists(resizeDirectory))
                Directory.CreateDirectory(resizeDirectory);
        }


        public void AddParameter(ResizeParameter resizeParameter)
        {
            if (resizeParameter == null || !resizeParameter.IsValid())
                throw new Exception("Invalid data to process resizing");

            _parameters.Add(resizeParameter);

            CheckSuffixUnicity();
        }


        private void CheckSuffixUnicity()
        {
            if (_parameters.Count != _parameters.Select(s => s.OutputSuffix).Distinct().Count())
                throw new Exception("The same suffix is used many times");
        }


        public void Resize()
        {
            while (_jpegFiles.Any())
            {
                string sourceFile;

                if (_jpegFiles.TryPop(out sourceFile))
                {
                    var filename = Path.GetFileName(sourceFile);
                    var extension = Path.GetExtension(sourceFile);
                    var fullFilename = Path.Combine(_picturesDirectory, RESIZE_DIRECTORY, filename);

                    using (var originalBitmap = new Bitmap(sourceFile))
                    {
                        foreach (var parameter in _parameters)
                        {
                            using (var resizedBitmap = new Bitmap(originalBitmap, GetNewSize(originalBitmap.Size, parameter.Size)))
                            using (var encoderParameters = new EncoderParameters(3))
                            {
                                encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, parameter.QualityLevel);
                                encoderParameters.Param[1] = new EncoderParameter(Encoder.ScanMethod, (int)EncoderValue.ScanMethodInterlaced);
                                encoderParameters.Param[2] = new EncoderParameter(Encoder.RenderMethod, (int)EncoderValue.RenderProgressive);

                                var resizedFilename = fullFilename.Replace(extension, string.Format("-{0}{1}", parameter.OutputSuffix, extension));
                                resizedBitmap.Save(resizedFilename, _imageCodecInfo, encoderParameters);

                                Console.WriteLine("File resized: {0} [{1}]", resizedFilename, System.Threading.Tasks.Task.CurrentId);
                            }
                        }
                        ProcessedFileCount++;
                    }
                }
            }
        }


        private static Size GetNewSize(Size sourceSize, Size targetSize)
        {
            var sourceRatio = sourceSize.Width * 1.0 / sourceSize.Height;
            var targetRatio = targetSize.Width * 1.0 / targetSize.Height;

            if (sourceRatio == targetRatio)
            {
                return targetSize;
            }

            if (sourceSize.Width > sourceSize.Height)
            {
                // landscape
                return new Size
                {
                    Width = targetSize.Width,
                    Height = Convert.ToInt32(targetSize.Width / sourceRatio)
                };
            }
            else
            {
                // portrait (or square)
                return new Size
                {
                    Width = Convert.ToInt32(targetSize.Height * sourceRatio),
                    Height = targetSize.Height
                };
            }
        }


        private static ImageCodecInfo GetImageCodecInfo(ImageFormat imageFormat)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            return codecs.First(c => c.FormatID == imageFormat.Guid);
        }
    }
}
