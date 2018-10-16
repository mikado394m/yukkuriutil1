using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Serialization;
using System.IO;
using YukkuriUtil.Constants;
using static YukkuriUtil.Constants.SettingConstants;

namespace YukkuriUtil.Models {
	public class SettingLoader {
        public string FilePath
        {
            private set;
            get;
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
			FilePath = filePath;
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

	public class ProfileSetting : ICloneable {
		public ProfileSetting() {
		}

		public ProfileSetting Clone() {
			return (ProfileSetting)(((ICloneable)this).Clone());
		}

		object ICloneable.Clone() {
			return MemberwiseClone();
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
	}

	public class VoiceSetting {
		public VoiceSetting() {
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
				if (!Helpers.InRange(value, SoftalkLibraryFirst, SoftalkLibraryLast)) {
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
				if (!Helpers.InRange(value, SoftalkVoiceIDFirst)) {
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
				if (!Helpers.InRange(value, SoftalkSpeedFirst, SoftalkSpeedLast)) {
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
				if (!Helpers.InRange(value, SoftalkVolumeFirst, SoftalkVolumeLast)) {
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
				if (!Helpers.InRange(value, SoftalkPitchFirst, SoftalkPitchLast)) {
					throw new ArgumentOutOfRangeException();
				}
				_SoftalkPitch = value;
			}
			get {
				return _SoftalkPitch;
			}
		}

        private int _A10Pitch = 100;

        public int A10Pitch
        {
            set
            {
                if (!Helpers.InRange(value, SofTalk.A10PitchMin, SofTalk.A10PitchMax))
                {
                    throw new ArgumentOutOfRangeException();
                }
                _A10Pitch = value;

            }
            get => _A10Pitch;
        }

        private int _A10Pitch2 = 100;

        public int A10Pitch2
        {
            set
            {
                if (!Helpers.InRange(value, SofTalk.A10Pitch2Min, SofTalk.A10Pitch2Max))
                {
                    throw new ArgumentOutOfRangeException();
                }
                _A10Pitch2 = value;

            }
            get => _A10Pitch2;
        }

        private int _A10Accent = 100;

        public int A10Accent
        {
            set
            {
                if (!Helpers.InRange(value, SofTalk.A10AccentMin, SofTalk.A10AccentMax))
                {
                    throw new ArgumentOutOfRangeException();
                }
                _A10Accent = value;

            }
            get => _A10Accent;
        }

        public int AudioOffset { get; set; } = -300;

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
