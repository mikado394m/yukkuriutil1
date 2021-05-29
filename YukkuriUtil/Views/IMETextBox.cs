using System.Windows.Controls;

namespace YukkuriUtil.Views
{
    // http://denasu.com/blog/2015/06/diary3087
    public class IMETextBox : TextBox
    {
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            GetBindingExpression(TextProperty).UpdateSource();
            base.OnTextChanged(e);
        }
    }
}
