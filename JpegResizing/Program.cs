using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JpegResizing
{
    class Program
    {
        /// <summary>
        /// Set to True to see the power in action !
        /// </summary>
        private static bool _useMultiThreading = false;

        /// <summary>
        /// Directory where Jpeg files are placed
        /// original files won't be modified
        /// </summary>
        private static string _jpegDirectory = @"C:\Pictures";


        static void Main(string[] args)
        {
            var jpegResizer = new JpegResizer(_jpegDirectory);
            jpegResizer.AddParameter(new ResizeParameter(new Size(900, 600), 95, "HIGH"));
            jpegResizer.AddParameter(new ResizeParameter(new Size(450, 300), 90, "MEDIUM"));
            jpegResizer.AddParameter(new ResizeParameter(new Size(120, 80), 85, "SMALL"));

            var timer = new Stopwatch();
            timer.Start();

            if (_useMultiThreading)
            {
                var currentTask = 0;
                var nbTask = Environment.ProcessorCount;
                var tasks = new Task[nbTask];

                while (currentTask < nbTask)
                {
                    tasks[currentTask] = new Task(new Action(jpegResizer.Resize));
                    tasks[currentTask].Start();
                    currentTask++;
                }

                Task.WaitAll(tasks);
            }
            else
            {
                jpegResizer.Resize();
            }

            timer.Stop();

            Console.WriteLine("Elapsed time: {0}ms / file count: {1}", timer.ElapsedMilliseconds, jpegResizer.ProcessedFileCount);
            Console.Read();
        }
    }
}
