using Microsoft.AspNetCore.Mvc;
using TTSWebApi.Services;
using TTSWebApi.Services.QueueService;

namespace TTSWebApi.Controllers {
	[ApiController]
	[Route("api/TTS")]
	public class TTSController: ControllerBase {
		private readonly ILogger<TTSController> _logger;
		private readonly ITTSService _tts;
		private readonly IBackgroundTaskQueue _backgroundTaskQueue;

		public TTSController(IBackgroundTaskQueue taskQueue, ITTSService tts, ILogger<TTSController> logger) {
			_logger = logger;
			_tts = tts;
			_backgroundTaskQueue = taskQueue;
		}

		[HttpGet("play")]
		public async Task<IActionResult> Play([FromQuery(Name = "text")] string str) {
			int result = await _tts.Speak(str);
			//await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async (token) => {
			//	result = await _tts.Speak(str);
			//});
			return result == -1 ? BadRequest() : Ok();
		}

		[HttpPost("play")]
		public async Task<IActionResult> PlayPost([FromBody] string str) {
			int result = await _tts.Speak(str);
			//await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async (token) => {
			//	result = await _tts.Speak(str);
			//});
			return result == -1 ? BadRequest() : Ok();
		}

		[HttpGet("cancel")]
		public async Task<IActionResult> Cancel() {
			int result = await _tts.Cancel();
			//await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async (token) => {
			//	result = await _tts.Cancel();
			//});
			return result == -1 ? BadRequest() : Ok();
		}
	}
}