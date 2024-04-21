using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using ShadowPluginLoader.MetaAttributes;

namespace ShadowPluginLoader.Tool
{
    internal class ExportMetaWindow: I18NWindow
    {
        #region Fields
        
        private readonly Button _languageButton = new()
        {
            Width = 120,
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(10),
            Height = 30
        };
        private readonly Button _exportButton = new()
        {
            Width = 400,
            Margin = new Thickness(0,10,0,0),
            Height = 20
        };
        private readonly ComboBox _exportTypeComboBox = new()
        {
            Width = 400,
            Height = 20,
        };

        private readonly Grid _grid = new();

        private readonly TextBlock _header = new();
        private readonly StackPanel _stackPanel = new()
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Orientation = Orientation.Vertical,
        };
        private readonly Assembly _assembly;


        #endregion
        
        public ExportMetaWindow(string dllPath)
        {
            Init();
            CheckText();
            _assembly = Assembly.LoadFrom(dllPath);
            LoadTypes();
            _exportButton.Click += ExportButtonOnClick;
            _languageButton.Click += (sender, args) =>
            {
                IsChinese = !IsChinese;
                CheckText();
            };
        }

        private static void WriteDefineFile(Type type)
        {
            var required = new List<string>();
            JsonWriterOptions writerOptions = new() { Indented = true, };

            using MemoryStream stream = new();
            using Utf8JsonWriter writer = new(stream, writerOptions);
            var properties = type.GetProperties();
            writer.WriteStartObject();
            writer.WritePropertyName("Namespace");
            writer.WriteStringValue(type.Namespace);
            writer.WritePropertyName("Type");
            writer.WriteStringValue(type.Name);
            
            writer.WriteStartObject("Properties");
            
            foreach (var property in properties)
            {
                if (property.Name == "TypeId" || !Program.CheckExportPropertyType(property.PropertyType.Name)) continue;
                var m = property.GetCustomAttribute<MetaAttribute>();
                if (m is not null)
                {
                    if (m.Exclude) continue;
                    if (m.Required) required.Add(property.Name);
                }
                writer.WriteStartObject(property.Name);
                writer.WritePropertyName("Type");
                writer.WriteStringValue(property.PropertyType.Name);
                
                if (!string.IsNullOrEmpty(m?.Regex))
                {
                    writer.WritePropertyName("Regex");
                    writer.WriteStringValue(m.Regex);
                }
                writer.WritePropertyName("PropertyGroupName");
                writer.WriteStringValue(string.IsNullOrEmpty(m?.PropertyGroupName)
                    ? property.Name
                    : m.PropertyGroupName);
                Console.WriteLine($"[ExportMeta] {property.Name}: {property.PropertyType.Name} -> plugin.d.json");
                writer.WriteEndObject();
            }
            writer.WriteEndObject();
            writer.WriteStartArray("Required");
            foreach (var req in required)
            {
                writer.WriteStringValue(req);
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
            writer.Flush();

            var json = Encoding.UTF8.GetString(stream.ToArray());
            
            File.WriteAllText(Path.Combine(Program.OutputPath,"plugin.d.json"),json);
        }
        public static bool ExportMeta(Assembly asm, string typeName)
        {
            var type = asm.GetType(typeName);
            if (type is null) return false;
            WriteDefineFile(type);
            Program.BuildCache(typeName);
            Console.WriteLine($"[ExportMeta] plugin.d.json -> {Path.Combine(Program.OutputPath,"plugin.d.json")}");
            return true;
        }
        private void LoadTypes()
        {
            foreach (var type in _assembly.GetExportedTypes())
            {
                if (type.GetInterfaces().Any(t => t.FullName == Program.TypeIPluginMetaData))
                {
                    _exportTypeComboBox.Items.Add(new ComboBoxItem
                    {
                        Content = type.FullName
                    });
                }
            }

            if (_exportTypeComboBox.Items.Count == 0) ShowError(2);
        }

        private static void ShowError(int m = 1)
        {
            var caption = Program.IsCn? "失败": "Error";
            var message = m switch
            {
                1 => Program.IsCn ? "导出失败，请检查你的输入" : "Export failed, please check your input",
                2 => Program.IsCn ? "未发现可用的元数据类" : "No available metadata classes found",
                _ => ""
            };
            Message.Show(MessageBoxImage.Information,
                caption,
                message
            );
        }
        private static MessageBoxResult ShowOk()
        {
            return Message.Show(MessageBoxImage.Information,
                Program.IsCn?"成功":"Success",
                Program.IsCn?"导出成功!\n类型文件: plugin.d.json":"Export Successfully!\nType File: plugin.d.json"
            );
        }
        private void ExportButtonOnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_exportTypeComboBox.Text) ||
                !ExportMeta(_assembly, _exportTypeComboBox.Text)) return;
            var res = ShowOk();
            if (res == MessageBoxResult.OK)
            {
                Close();
            }
        }

        private void Init()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            _stackPanel.Children.Add(_header);
            _stackPanel.Children.Add(_exportTypeComboBox);
            _stackPanel.Children.Add(_exportButton);
            _grid.Children.Add(_stackPanel);
            _grid.Children.Add(_languageButton);
            Width = 500;
            Height = 300;
            Content = _grid;
        }

        private new void CheckText()
        {
            if (IsChinese)
            {
                Title = "导出你的插件元数据类型";
                _exportButton.Content = "导出元数据类型";
                _languageButton.Content = "语言: 中文";
                _header.Text = "请选择要导出的元数据类型的类";
            }
            else
            {
                Title = "Export Plugin Meta Type";
                _exportButton.Content = "Export Plugin Meta Type";
                _languageButton.Content = "Language: English";
                _header.Text = "Please select the class for the type of metadata to be exported";
            }
        }
    }

}