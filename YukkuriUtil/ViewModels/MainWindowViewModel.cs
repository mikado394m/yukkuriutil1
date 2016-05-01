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
using System.Windows.Media;
using System.Threading.Tasks;

namespace YukkuriUtil.ViewModels {
	public class MainWindowViewModel : ViewModel, IDisposable {
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

		SettingLoader setting = new SettingLoader("setting.xml");
		VoiceCreator voiceCreator;

		public void Initialize() {
			setting.Deserialize();
			SelectionProfile = setting.Setting.Profiles[setting.Setting.SelectionProfile].Clone();
			ApplySettingChange();

			while (true) {
				try {
					voiceCreator = new VoiceCreator(SelectionProfile);
					break;
				} catch {
					var res = MessageBox.Show(
						"SofTalk.exe のパスが設定されていません!\n設定ウィンドウを開きますか?",
						"エラー",
						MessageBoxButton.YesNo,
						MessageBoxImage.Error
					);

					if (res == MessageBoxResult.Yes) {
						OpenSettingWindow();
					} else {
						Messenger.Raise(new WindowActionMessage(WindowAction.Close, "CloseWindow"));
						return;
					}
				}
			}
		}

		#region VoiceText変更通知プロパティ
		private string _VoiceText = "";

		public string VoiceText {
			get {
				return _VoiceText;
			}
			set {
				if (_VoiceText == value)
					return;
				_VoiceText = value;
				RaisePropertyChanged();
				RaisePropertyChanged("SelectionVoice");
			}
		}
		#endregion

		#region ShowText変更通知プロパティ
		private string _ShowText = "";

		public string ShowText {
			get {
				return _ShowText;
			}
			set {
				if (_ShowText == value)
					return;
				_ShowText = value;
				if (IsTextCopy) {
					VoiceText = value;
				}
				RaisePropertyChanged();
			}
		}
		#endregion

		#region IsTextCopy変更通知プロパティ
		private bool _IsTextCopy = true;

		public bool IsTextCopy {
			get {
				return _IsTextCopy;
			}
			set {
				if (_IsTextCopy == value)
					return;
				_IsTextCopy = value;
				if (value == true) {
					VoiceText = ShowText;
				}
				RaisePropertyChanged();
			}
		}
		#endregion

		#region SelectionProfile変更通知プロパティ
		private ProfileSetting _SelectionProfile;

		public ProfileSetting SelectionProfile {
			get {
				return _SelectionProfile;
			}
			set {
				if (_SelectionProfile == value)
					return;
				_SelectionProfile = value;
				RaisePropertyChanged();
			}
		}
		#endregion

		#region VoiceSettings変更通知プロパティ
		private BindingList<VoiceSetting> _VoiceSettings;

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

		#region SelectionVoice変更通知プロパティ
		private int _SelectionVoice;

		public int SelectionVoice {
			get {
				return _SelectionVoice;
			}
			set {
				if (_SelectionVoice == value)
					return;
				_SelectionVoice = value;
				setting.Setting.SelectionVoice = value;
				voiceCreator?.CacheClear();
				RaisePropertyChanged();
			}
		}
		#endregion

		#region AreaText変更通知プロパティ
		private string _AreaText = "この領域をAviUtlのウィンドウに\nD&Dしてください。";

		public string AreaText {
			get {
				return _AreaText;
			}
			set {
				if (_AreaText == value)
					return;
				_AreaText = value;
				RaisePropertyChanged();
			}
		}
		#endregion

		#region AreaColor変更通知プロパティ
		private Brush _AreaColor = Brushes.LightGray;

		public Brush AreaColor {
			get {
				return _AreaColor;
			}
			set {
				if (_AreaColor == value)
					return;
				_AreaColor = value;
				RaisePropertyChanged();
			}
		}
		#endregion

		// 設定ウィンドウを開く
		public void OpenSettingWindow() {
			voiceCreator?.CacheClear();

			// 引数
			var config = new SettingWindowViewModel();
			config.Setting = setting.Setting;

			// 実際に呼び出す
			var message = new TransitionMessage(typeof(Views.SettingWindow), config, TransitionMode.Modal, "OpenSettingWindow");
			Messenger.Raise(message);

			// 戻り値
			setting.Setting = config.Setting;
			ApplySettingChange();
		}

		// 設定変更を適用
		private void ApplySettingChange() {
			VoiceSettings = new BindingList<VoiceSetting>(setting.Setting.Voices);
			SelectionVoice = setting.Setting.SelectionVoice;
		}

		public void DisabledTextCopy() {
			IsTextCopy = false;
		}

		public void EnabledTextCopy() {
			IsTextCopy = true;
		}

		public async void DragStart(DependencyObject e) {
			// 生成中メッセージ
			AreaText = "音声を生成中…\nマウスを放さないでください。";
			AreaColor = Brushes.LightYellow;

			var outPath = await Task.Run(() => {
				return voiceCreator.Create(
					VoiceText,
					setting.Setting.Voices[SelectionVoice],
					ShowText
				);
			});

			// 生成完了メッセージ
			AreaText = "準備完了!\nAviUtlのウィンドウ上で\nマウスを放してください。";
			AreaColor = Brushes.LightGreen;

			// ドラッグ&ドロップ本体
			await Task.Run(async () => {
				await DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {
					DragDrop.DoDragDrop(
						e,
						new DataObject(DataFormats.FileDrop, new string[] { outPath }),
						DragDropEffects.Copy
					);
				}));
			});

			// 元に戻す
			AreaText = "この領域をAviUtlのウィンドウに\nD&Dしてください。";
			AreaColor = Brushes.LightGray;
		}

		// フォームが閉じた
		public new void Dispose() {
			setting.Serialize();
			voiceCreator?.Dispose();
		}
	}
}
