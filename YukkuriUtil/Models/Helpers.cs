using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YukkuriUtil.Models {
	public static class Helpers {
		public static bool IsRange(int value, int first = int.MinValue, int last = int.MaxValue) {
			return (value >= first && value <= last);
		}
	}
}
