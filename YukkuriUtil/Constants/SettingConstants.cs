using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using YukkuriUtil.Models;

namespace YukkuriUtil.Constants {
	public static class SettingConstants {
		public const int SoftalkLibraryFirst = 7;
		public const int SoftalkLibraryLast = 11;

		public static readonly List<Tuple<int, string>> SoftalkLibraryList
			= new List<Tuple<int, string>> {
				new Tuple<int, string>(7, "AquesTalk"),
				new Tuple<int, string>(8, "SAPI"),
				new Tuple<int, string>(9, "Speech Platform"),
				new Tuple<int, string>(10, "AquesTalk2"),
                new Tuple<int, string>(11, "AquesTalk10")
			};

		public const int SoftalkVoiceIDFirst = 0;

		public const int SoftalkSpeedFirst = 1;
		public const int SoftalkSpeedLast = 300;

		public const int SoftalkVolumeFirst = 1;
		public const int SoftalkVolumeLast = 100;

		public const int SoftalkPitchFirst = 1;
		public const int SoftalkPitchLast = 300;
	}

    public static class SofTalk
    {
        public const int A10PitchMin = 20;
        public const int A10PitchMax = 200;

        public const int A10Pitch2Min = 50;
        public const int A10Pitch2Max = 200;

        public const int A10AccentMin = 0;
        public const int A10AccentMax = 200;
    }
}
