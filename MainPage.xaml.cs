using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace App;

public partial class MainPage : ContentPage
{
    public static List<TodoModel> TodoModel { get; set; } = new List<TodoModel>();
    public static string Path { get; set; } = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/Todo.xml";
    public static string Content { get; set; }
    public static object Title { get; set; }
    public static MainPage Instance { get; private set; }
    public ICommand MyCommand => new Command<string>((string item) =>
    {
        DisplayAlert("", item, "");
    });
    public string Com { get; set; }
    public MainPage()
    {
        InitializeComponent();

        Instance = this;
    }

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        try
        {
            if (!File.Exists(Path) || File.ReadAllText(Path) == "")
            {
                File.Create(Path).Close();

                File.WriteAllText(Path, """
            <?xml version="1.0" encoding="utf-8"?>
            <ArrayOfTodoModel>
                
            </ArrayOfTodoModel>
            """);

            }

            //DisplayAlert("Text", File.ReadAllText(Path), "Ok");

            using (FileStream fs = new FileStream(Path, FileMode.OpenOrCreate))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<TodoModel>));
                TodoModel = xmlSerializer.Deserialize(fs) as List<TodoModel>;
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

        TodoList.ItemsSource = TodoModel;
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        Content = await DisplayPromptAsync("Добавить", "Введите название:", "OK", "Отмена");

        if (Content == null || Content == "")
            return;

        TodoModel item = new TodoModel() { Title = Content, Data = DateTime.Now.ToLongDateString() };

        TodoModel.Add(item);

        TodoList.ItemsSource = null;
        TodoList.ItemsSource = TodoModel;

        Title = item.Title;
        await Shell.Current.GoToAsync("TodoEditor");

        Save();
    }
    public void Save()
    {
        XmlDocument xDoc = new XmlDocument();
        xDoc.Load(Path);
        XmlElement? xRoot = xDoc.DocumentElement;

        // создаем новый элемент TodoModel
        XmlElement todoModelElem = xDoc.CreateElement("TodoModel");

        // создаем атрибут id
        XmlAttribute idAttr = xDoc.CreateAttribute("id");

        XmlElement dataElem = xDoc.CreateElement("Data");
        XmlElement titleElem = xDoc.CreateElement("Title");
        XmlElement contentElem = xDoc.CreateElement("Content");

        // создаем текстовые значения для элементов и атрибута
        XmlText idText = xDoc.CreateTextNode(Content);
        XmlText dataText = xDoc.CreateTextNode(DateTime.Now.ToLongDateString());
        XmlText contentText = xDoc.CreateTextNode("");

        XmlText titleText = xDoc.CreateTextNode(Content);

        //добавляем узлы
        idAttr.AppendChild(idText);
        dataElem.AppendChild(dataText);
        contentElem.AppendChild(contentText);
        titleElem.AppendChild(titleText);

        todoModelElem.Attributes.Append(idAttr);
        // добавляем элементы company и age
        todoModelElem.AppendChild(dataElem);
        todoModelElem.AppendChild(contentElem);
        todoModelElem.AppendChild(titleElem);
        // добавляем в корневой элемент новый элемент person
        xRoot?.AppendChild(todoModelElem);
        // сохраняем изменения xml-документа в файл
        xDoc.Save(Path);
    }

    private async void TodoList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        var selectedTitle = e.SelectedItem as TodoModel;
        if (selectedTitle != null)
        {
            Title = selectedTitle.Title;
            await Shell.Current.GoToAsync("TodoEditor");
        }
    }

    public static void Reload()
    {
        if (MainPage.Instance != null)
        {
            using (FileStream fs = new FileStream(Path, FileMode.OpenOrCreate))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<TodoModel>));
                TodoModel = xmlSerializer.Deserialize(fs) as List<TodoModel>;
            }

            MainPage.Instance.TodoList.ItemsSource = TodoModel;
        }
    }
    public static void ReSelected() => MainPage.Instance.TodoList.SelectedItem = null;

    private void MenuFlyoutItem_Clicked(object sender, EventArgs e)
    {
        var menuitem = sender as MenuItem;
        if (menuitem != null)
        {
            var name = menuitem.BindingContext as TodoModel;
            //DisplayAlert("Alert", "Delete " + name.Title, "Ok");

            XDocument xdoc = XDocument.Load(MainPage.Path);

            // получим элемент person с id = "Tom"
            var todo = xdoc.Element("ArrayOfTodoModel")?
                .Elements("TodoModel")
                .FirstOrDefault(p => p.Attribute("id")?.Value == name.Title.ToString());

            if (todo != null)
            {
                todo.Remove();

                xdoc.Save(MainPage.Path);

                MainPage.Reload();
            }
        }
    }
}