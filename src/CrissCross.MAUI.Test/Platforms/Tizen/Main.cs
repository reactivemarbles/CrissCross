using System;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace CrissCross.MAUI.Test
{
    /// <summary>
    /// Program.
    /// </summary>
    internal class Program : MauiApplication
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        static void Main(string[] args)
        {
            var app = new Program();
            app.Run(args);
        }
    }
}
