using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Serialization;
using System.IO;
using static YukkuriUtil.Statics.SettingStatic;

namespace YukkuriUtil.Models {
	public class SettingLoader {
		private string filePath;
		public string FilePath {
			get {
				return filePath;
			}
		}

		public AppSetting Setting {
			set;
			get;
		} = new AppSetting() {
			Profiles = new List<ProfileSetting>() { new ProfileSetting() { Name = "Default" } },
			SelectionProfile = 0,
			Voices = new List<VoiceSetting>() { new VoiceSetting() { Name = "Default" } },
			SelectionVoice = 0
		};

		public SettingLoader(string filePath) {
			this.filePath = filePath;
		}

		// 設定ファイルを読み込む
		// 実際に読み込んだ場合はtrue
		public bool Deserialize() {
			if (!File.Exists(FilePath)) {
				return false;
			}

			var serializer = new XmlSerializer(typeof(AppSetting));
			using (var sr = new StreamReader(FilePath, new UTF8Encoding(false))) {
				Setting = (AppSetting)serializer.Deserialize(sr);
			}

			return true;
		}

		public void Serialize() {
			var serializer = new XmlSerializer(typeof(AppSetting));
			using (var sw = new StreamWriter(FilePath, false, new UTF8Encoding(false))) {
				serializer.Serialize(sw, Setting);
			}
		}
	}

	public class AppSetting {
		public AppSetting() {
		}

		public AppSetting(
			List<ProfileSetting> profiles,
			int selectionProfile,
			List<VoiceSetting> voices,
			int selectionVoice
		) {
			Profiles = profiles;
			SelectionProfile = selectionProfile;
			Voices = voices;
			SelectionVoice = selectionVoice;
		}

		// 0番目はデフォルト値
		public List<ProfileSetting> Profiles {
			set;
			get;
		}

		// 0番目はデフォルト値
		public List<VoiceSetting> Voices {
			set;
			get;
		}

		public int SelectionProfile {
			get;
			set;
		} = 0;

		public int SelectionVoice {
			get;
			set;
		}
	}

	public class ProfileSetting {
		public ProfileSetting() {
		}

		public ProfileSetting(
			string name,
			string softalkPath,
			string audioOutPath
		) {
			Name = name;
			SoftalkPath = softalkPath;
			AudioOutPath = audioOutPath;
		}

		public ProfileSetting Clone() {
			return (ProfileSetting)MemberwiseClone();
		}

		public string Name {
			get;
			set;
		} = "";

		// SofTalk.exeへのパス
		// 必ずしもディレクトリにアクセスできるとは限らないので注意
		public string SoftalkPath {
			get;
			set;
		} = @".\softalk\SofTalk.exe";

		// 音声の出力先
		// 必ずしもディレクトリにアクセスできるとは限らないので注意
		public string AudioOutPath {
			get;
			set;
		} = @".\audioout";

		// AviUtlのFPS
		public uint AviutlFps {
			get;
			set;
		} = 30;

		// 音声ファイルの長さ補正値(ミリ秒)
		public int AudioTimeFix {
			get;
			set;
		} = -300;
	}

	public class VoiceSetting {
		public VoiceSetting() {
		}

		public VoiceSetting(
			string name,
			int softalkLibrary,
			int softalkVoiceID,
			int softalkSpeed,
			int softalkVolume,
			int softalkPitch,
			string exoTemplate
		) {
			Name = name;
			SoftalkLibrary = softalkLibrary;
			SoftalkVoiceID = softalkVoiceID;
			SoftalkSpeed = softalkSpeed;
			SoftalkVolume = softalkVolume;
			SoftalkPitch = softalkPitch;
			ExoTemplate = exoTemplate;
		}

		public VoiceSetting Clone() {
			return (VoiceSetting)MemberwiseClone();
		}

		public string Name {
			get;
			set;
		} = "";

		private int _SoftalkLibrary = 7;

		// SofTalkの発声ライブラリ
		public int SoftalkLibrary {
			set {
				if (value != SoftalkLibraryRange) {
					throw new ArgumentOutOfRangeException();
				}
				_SoftalkLibrary = value;
			}
			get {
				return _SoftalkLibrary;
			}
		}

		private int _SoftalkVoiceID = 0;

		// SofTalkの声番号
		public int SoftalkVoiceID {
			set {
				if (value != SoftalkVoiceIDRange) {
					throw new ArgumentOutOfRangeException();
				}
				_SoftalkVoiceID = value;
			}
			get {
				return _SoftalkVoiceID;
			}
		}

		private int _SoftalkSpeed = 100;

		// SofTalkの再生速度
		public int SoftalkSpeed {
			set {
				if (value != SoftalkSpeedRange) {
					throw new ArgumentOutOfRangeException();
				}
				_SoftalkSpeed = value;
			}
			get {
				return _SoftalkSpeed;
			}
		}

		private int _SoftalkVolume = 100;

		// SofTalkの音量
		public int SoftalkVolume {
			set {
				if (value != SoftalkVolumeRange) {
					throw new ArgumentOutOfRangeException();
				}
				_SoftalkVolume = value;
			}
			get {
				return _SoftalkVolume;
			}
		}

		private int _SoftalkPitch = 100;

		// SofTalkの音程
		public int SoftalkPitch {
			set {
				if (value != SoftalkPitchRange) {
					throw new ArgumentOutOfRangeException();
				}
				_SoftalkPitch = value;
			}
			get {
				return _SoftalkPitch;
			}
		}

		public string ExoTemplate {
			get;
			set;
		} =
	@"[0]
start=1
end={2}
layer=1
overlay=1
audio=1
[0.0]
_name=音声ファイル
再生位置=0.00
再生速度=100.0
ループ再生=0
動画ファイルと連携=0
file={1}
[0.1]
_name=標準再生
音量=100.0
左右=0.0";
	}
}
