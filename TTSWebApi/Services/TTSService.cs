using System.Speech.Synthesis;

namespace TTSWebApi.Services {
	public interface ITTSService {
		Task<int> Speak(string text);
		Task<int> Cancel();
	}

	public class TTSService: ITTSService {
		private readonly ILogger<TTSService> _logger;

		private SpeechSynthesizer Synthesizer { get; set; } = new SpeechSynthesizer();

		public TTSService(ILogger<TTSService> logger) {
			_logger = logger;
			Synthesizer.SetOutputToDefaultAudioDevice();
			Synthesizer.StateChanged += new EventHandler<StateChangedEventArgs>(Synth_StateChanged);
			Synthesizer.SpeakProgress += new EventHandler<SpeakProgressEventArgs>(Synth_SpeakProgress);
			Synthesizer.SpeakCompleted += new EventHandler<SpeakCompletedEventArgs>(Synth_SpeakCompleted);
		}

		public async Task<int> Speak(string text) {
			if (string.IsNullOrEmpty(text)) {
				_logger.LogWarning("Empty or Null string.");
				return -1;
			}

			_logger.LogInformation("Get string: {text}", text);
			await Task.Run(() => Synthesizer.SpeakAsync(text));
			_logger.LogInformation("End: {text}", text);
			return 0;
		}

		public async Task<int> Cancel() {
			try {
				_logger.LogInformation("Cancel");
				await Task.Run(Synthesizer.SpeakAsyncCancelAll);
				return 0;
			} catch {
				return -1;
			}
		}

		private void Synth_SpeakCompleted(object sender, SpeakCompletedEventArgs e) {
			_logger.LogDebug("Speak Completed.");
		}

		private void Synth_StateChanged(object sender, StateChangedEventArgs e) {
			_logger.LogDebug($"Synthesizer State: {e.State}\tPrevious State: {e.PreviousState}");
		}

		private void Synth_SpeakProgress(object sender, SpeakProgressEventArgs e) {
			_logger.LogDebug(e.Text);
		}
	}
}
