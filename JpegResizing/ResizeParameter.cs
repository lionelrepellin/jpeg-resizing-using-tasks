using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JpegResizing
{
    public class ResizeParameter
    {
        public Size Size { get; private set; }
        public long QualityLevel { get; private set; }
        public string OutputSuffix { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="size">Width and height to resize picture</param>
        /// <param name="qualityLevel">Should be set between 0 and 100</param>
        /// <param name="outputSuffix">Suffix to identify resized picture</param>
        public ResizeParameter(Size size, long qualityLevel, string outputSuffix)
        {
            this.Size = size;
            this.QualityLevel = qualityLevel;
            this.OutputSuffix = outputSuffix;
        }

        public bool IsValid()
        {
            return Size.Width > 0 &&
                    Size.Height > 0 &&
                    QualityLevel >= 0 &&
                    QualityLevel <= 100 &&
                    !string.IsNullOrEmpty(OutputSuffix);
        }
    }
}
