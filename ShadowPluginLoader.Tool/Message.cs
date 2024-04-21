using System.Windows;

namespace ShadowPluginLoader.Tool;

public static class Message
{
    public static MessageBoxResult Show(MessageBoxImage icon,string caption, string message)
    {
        var result= MessageBox.Show(message, caption, MessageBoxButton.OKCancel, icon, MessageBoxResult.Yes);
        return result;
    }
}