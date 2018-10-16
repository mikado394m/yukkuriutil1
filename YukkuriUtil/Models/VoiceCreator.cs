using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;

using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NAudio;
using NAudio.Wave;

namespace YukkuriUtil.Models {
	public class VoiceCreator : NotificationObject, IDisposable {
		/*
		 * NotificationObjectはプロパティ変更通知の仕組みを実装したオブジェクトです。
		 */

		ProfileSetting profile;

		string oldVoiceText = null;
		string oldShowText;

		string oldWavOutPath;
		int oldVoiceTime;

		string oldExoOutPath;

		public VoiceCreator(ProfileSetting profile) {
			this.profile = profile;
			Process.Start(profile.SoftalkPath, "/X:1");
		}

		public void ClearCache() {
			oldVoiceText = null;
		}

		// 生成されたwavの長さ(ミリ秒)を返す
		private async Task<int> createWavAsync(string filePath, VoiceSetting setting, string voiceText) {
			// SofTalk.exe に音声を生成させる
			Process.Start(
				profile.SoftalkPath,
				string.Format(
					"/T:{0} /U:{1} /V:{2} /O:{3} /S:{4} /R:\"{5}\" /W:{6}",
					new object[] {
						setting.SoftalkLibrary,
						setting.SoftalkVoiceID,
						setting.SoftalkVolume,
						setting.SoftalkPitch,
						setting.SoftalkSpeed,
						filePath,
						voiceText
					}
				)
			);

			FileStream wavFs;

			// 音声が完成するまで待つ
			while (true) {
				await Task.Delay(50);

				try {
					wavFs = new FileStream(filePath, FileMode.Open);
                    break;
				} catch {
				}
			}

			// 音声の長さを調べる
			var wav = new WaveFileReader(wavFs);
			var voiceTime = (int)wav.TotalTime.TotalMilliseconds;

			wav.Dispose();
			wavFs.Dispose();
			return voiceTime;
		}

		private void createExo(string filePath, string template, string showText, string wavOutPath, int voiceTime) {
			// 表示テキストをExoで利用できるようにする
			var exoShowText =
				BitConverter.ToString(Encoding.Unicode.GetBytes(showText))
				.Replace("-", "")
				.ToLower()
				.PadRight(4096, '0');

			var exoVoiceTime = (int)((voiceTime + profile.AudioTimeFix) * (profile.AviutlFps / 1000f));
			Debug.Write(exoVoiceTime);
			Debug.Write(voiceTime);

			var exoStr = string.Format(template, exoShowText, wavOutPath, exoVoiceTime);

			using (var exoFs = new StreamWriter(filePath, false, Encoding.GetEncoding("shift_jis"))) {
				exoFs.Write(exoStr);
			}
		}

		public async Task<string> CreateAsync(string voiceText, VoiceSetting setting, string showText) {
			string time = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss fffffff");
			string wavOutPath = Path.GetFullPath(Path.Combine(profile.AudioOutPath, time + ".wav"));
			string exoOutPath = Path.GetFullPath(Path.Combine(profile.AudioOutPath, time + ".exo"));

			// 以前と内容が被っていた場合
			if (oldVoiceText != null && oldVoiceText == voiceText) {
				if (oldShowText == showText && oldExoOutPath != "") {
					return oldExoOutPath;
				}

				oldShowText = showText;
				oldExoOutPath = exoOutPath;

				createExo(exoOutPath, setting.ExoTemplate, showText, oldWavOutPath, oldVoiceTime);
				return exoOutPath;
			}

			var voiceTime = await createWavAsync(wavOutPath, setting, voiceText);
			createExo(exoOutPath, setting.ExoTemplate, showText, wavOutPath, voiceTime);

			oldShowText = showText;
			oldWavOutPath = wavOutPath;
			oldVoiceText = voiceText;
			oldExoOutPath = exoOutPath;
			oldVoiceTime = voiceTime;
			return exoOutPath;
		}

		public void Dispose() {
			try {
				Process.Start(profile.SoftalkPath, "/close");
			} catch {
			}
		}
	}
}
