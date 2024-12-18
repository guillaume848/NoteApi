using Microsoft.Extensions.Logging;
using Notes.viewModels;
using viewModels.Notes;

namespace Notes
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Register NotesApiService with HttpClient
            builder.Services.AddHttpClient<NotesApiService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7029/"); // Update this to your API's URL
            });

            // Register NotesViewModel
            builder.Services.AddSingleton<NotesViewModel>();
            //builder.Services.AddTransient<NoteDetailsViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
