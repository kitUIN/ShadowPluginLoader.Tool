using System.Windows;

namespace ShadowPluginLoader.Tool;

internal interface I18N
{
    void CheckText();
}

internal class I18NWindow : Window, I18N
{
    public bool IsChinese
    {
        get => Program.IsCn;
        set
        {
            Program.IsCn = value;
            CheckText();
        }
    }

    public void CheckText()
    {
    }
}