using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using YukkuriUtil.Models;

namespace YukkuriUtil.Statics {
	public static class SettingStatic {
		public static readonly Range SoftalkLibraryRange = new Range(7, 10);
		public static readonly Range SoftalkVoiceIDRange = new Range(0);
		public static readonly Range SoftalkSpeedRange = new Range(1, 300);
		public static readonly Range SoftalkVolumeRange = new Range(1, 100);
		public static readonly Range SoftalkPitchRange = new Range(1, 300);
	}
}
