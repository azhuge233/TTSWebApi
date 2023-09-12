using TTSWebApi.Services;
using TTSWebApi.Services.QueueService;

namespace TTSWebApi {
	public class Program {
		public static async Task Main(string[] args) {
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllers();
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			builder.Services.AddHostedService<QueuedHostedService>();
			builder.Services.AddSingleton<IBackgroundTaskQueue>(_ => {
				if(!int.TryParse(builder.Configuration["QueueCapacity"], out var queueCapacity)) queueCapacity = 5;
				return new DefaultBackgroundTaskQueue(queueCapacity);
			});

			// builder.Services.AddSingleton<ITTSService, TTSService>();
			builder.Services.AddScoped<ITTSService, TTSService>();
			// builder.Services.AddTransient<ITTSService, TTSService>();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment()) {
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseAuthorization();

			app.MapControllers();

			await app.RunAsync();
		}
	}
}