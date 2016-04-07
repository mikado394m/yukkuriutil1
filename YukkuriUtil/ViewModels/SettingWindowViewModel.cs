using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using YukkuriUtil.Models;

using System.Windows;

namespace YukkuriUtil.ViewModels {
	public class SettingWindowViewModel : ViewModel {
		/* コマンド、プロパティの定義にはそれぞれ 
		 * 
		 *  lvcom   : ViewModelCommand
		 *  lvcomn  : ViewModelCommand(CanExecute無)
		 *  llcom   : ListenerCommand(パラメータ有のコマンド)
		 *  llcomn  : ListenerCommand(パラメータ有のコマンド・CanExecute無)
		 *  lprop   : 変更通知プロパティ(.NET4.5ではlpropn)
		 *  
		 * を使用してください。
		 * 
		 * Modelが十分にリッチであるならコマンドにこだわる必要はありません。
		 * View側のコードビハインドを使用しないMVVMパターンの実装を行う場合でも、ViewModelにメソッドを定義し、
		 * LivetCallMethodActionなどから直接メソッドを呼び出してください。
		 * 
		 * ViewModelのコマンドを呼び出せるLivetのすべてのビヘイビア・トリガー・アクションは
		 * 同様に直接ViewModelのメソッドを呼び出し可能です。
		 */

		/* ViewModelからViewを操作したい場合は、View側のコードビハインド無で処理を行いたい場合は
		 * Messengerプロパティからメッセージ(各種InteractionMessage)を発信する事を検討してください。
		 */

		/* Modelからの変更通知などの各種イベントを受け取る場合は、PropertyChangedEventListenerや
		 * CollectionChangedEventListenerを使うと便利です。各種ListenerはViewModelに定義されている
		 * CompositeDisposableプロパティ(LivetCompositeDisposable型)に格納しておく事でイベント解放を容易に行えます。
		 * 
		 * ReactiveExtensionsなどを併用する場合は、ReactiveExtensionsのCompositeDisposableを
		 * ViewModelのCompositeDisposableプロパティに格納しておくのを推奨します。
		 * 
		 * LivetのWindowテンプレートではViewのウィンドウが閉じる際にDataContextDisposeActionが動作するようになっており、
		 * ViewModelのDisposeが呼ばれCompositeDisposableプロパティに格納されたすべてのIDisposable型のインスタンスが解放されます。
		 * 
		 * ViewModelを使いまわしたい時などは、ViewからDataContextDisposeActionを取り除くか、発動のタイミングをずらす事で対応可能です。
		 */

		/* UIDispatcherを操作する場合は、DispatcherHelperのメソッドを操作してください。
		 * UIDispatcher自体はApp.xaml.csでインスタンスを確保してあります。
		 * 
		 * LivetのViewModelではプロパティ変更通知(RaisePropertyChanged)やDispatcherCollectionを使ったコレクション変更通知は
		 * 自動的にUIDispatcher上での通知に変換されます。変更通知に際してUIDispatcherを操作する必要はありません。
		 */

		public SettingWindowViewModel() : base() {
		}

		public void Initialize() {
		}

		private int selectionProfile;
		private int selectionVoice;

		public AppSetting Setting {
			get {
				return new AppSetting(
					new List<ProfileSetting>(ProfileSettings),
					selectionProfile,
					new List<VoiceSetting>(VoiceSettings),
					selectionVoice);
			}
			set {
				ProfileSettings = new BindingList<ProfileSetting>(value.Profiles);
				selectionProfile = value.SelectionProfile;
				VoiceSettings = new BindingList<VoiceSetting>(value.Voices);
				selectionVoice = value.SelectionVoice;
			}
		}

		#region ProfileSettings変更通知プロパティ
		private BindingList<ProfileSetting> _ProfileSettings = new BindingList<ProfileSetting>();

		public BindingList<ProfileSetting> ProfileSettings {
			get {
				return _ProfileSettings;
			}
			set {
				if (_ProfileSettings == value)
					return;
				_ProfileSettings = value;
				RaisePropertyChanged();
			}
		}
		#endregion

		public void AddProfileSetting() {
			ProfileSettings.Add(ProfileSettings[0].Clone());
		}

		public void AcceptProfileSetting(ProfileSetting target) {
			selectionProfile = ProfileSettings.IndexOf(target);
			MessageBox.Show(
				"変更は、アプリケーションを再起動するまで適用されません。",
				"情報",
				MessageBoxButton.OK,
				MessageBoxImage.Information
			);
		}

		public void RemoveProfileSetting(ProfileSetting target) {
			var index = ProfileSettings.IndexOf(target);

			if (selectionProfile == index) {
				MessageBox.Show(
					"選択中のプロファイルを削除することはできません。",
					"エラー",
					MessageBoxButton.OK,
					MessageBoxImage.Error
				);
				return;
			}

			if (selectionProfile > index) {
				selectionProfile--;
			}

			ProfileSettings.RemoveAt(index);
		}

		#region VoiceSettings変更通知プロパティ
		private BindingList<VoiceSetting> _VoiceSettings = new BindingList<VoiceSetting>();

		public BindingList<VoiceSetting> VoiceSettings {
			get {
				return _VoiceSettings;
			}
			set {
				if (_VoiceSettings == value)
					return;
				_VoiceSettings = value;
				RaisePropertyChanged();
			}
		}
		#endregion

		public void AddVoiceSetting() {
			VoiceSettings.Add(VoiceSettings[0].Clone());
		}

		public void RemoveVoiceSetting(VoiceSetting target) {
			var index = VoiceSettings.IndexOf(target);

			if (selectionVoice == index) {
				MessageBox.Show(
					"選択中の声設定を削除することはできません。",
					"エラー",
					MessageBoxButton.OK,
					MessageBoxImage.Error
				);
				return;
			}

			if (selectionVoice > index) {
				selectionVoice--;
			}

			VoiceSettings.RemoveAt(index);
		}
	}
}
