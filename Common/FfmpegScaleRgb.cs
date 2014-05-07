﻿namespace Hudl.Ffmpeg.Common
{
    /// <summary>
    /// Describes a Red Green Blue intensity ratio that is used to intensify that colors
    /// </summary>
    public class FfmpegScaleRgb
    {
        public FfmpegScaleRgb()
        {
            Red = new FfmpegScale();
            Green = new FfmpegScale();
            Blue = new FfmpegScale();
        }

        /// <summary>
        /// property containing the double value of ratios to the Red color intensity
        /// </summary>
        public FfmpegScale Red { get; set; } 

        /// <summary>
        /// property containing the double value of ratios to the Green color intensity
        /// </summary>
        public FfmpegScale Green { get; set; } 

        /// <summary>
        /// property containing the double value of ratios to the Blue color intensity
        /// </summary>
        public FfmpegScale Blue  { get; set; } 
    }
}