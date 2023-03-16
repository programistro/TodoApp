using System.Xml.Linq;

namespace App.Pages;

public partial class TodoEditor : ContentPage
{
	public TodoEditor() => InitializeComponent();

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        XDocument xdoc = XDocument.Load(MainPage.Path);
        // получаем корневой узел
        XElement? arrayTodoModel = xdoc.Element("ArrayOfTodoModel");
        if (arrayTodoModel is not null)
        {
            // проходим по всем элементам person
            foreach (XElement person in arrayTodoModel.Elements("TodoModel"))
            {

                XAttribute? id = person.Attribute("id");
                XElement? content = person.Element("Content");
                XElement? data = person.Element("Data");

                //DisplayAlert("OK", $"""
                //    id:{id?.Value}
                //    content:{content?.Value}
                //    data:{data?.Value}
                //    """, "ok");

                if (id?.Value == MainPage.Title.ToString())
                {
                    editor.Text = content?.Value;
                }

                //DisplayAlert("OK", $"{MainPage.Title}", "Ok");
            }
        }
    }
    private void ContentPage_NavigatedFrom(object sender, NavigatedFromEventArgs e)
    {
        XDocument xdoc = XDocument.Load(MainPage.Path);

        // получим элемент person с id = "Tom"
        var todo = xdoc.Element("ArrayOfTodoModel")?
            .Elements("TodoModel")
            .FirstOrDefault(p => p.Attribute("id")?.Value == MainPage.Title.ToString());

        if (todo != null)
        {
            //  меняем вложенный элемент content
            var content = todo.Element("Content");
            if (content != null) content.Value = editor.Text;

            xdoc.Save(MainPage.Path);
            MainPage.ReSelected();
        }
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        XDocument xdoc = XDocument.Load(MainPage.Path);

        // получим элемент person с id = "Tom"
        var todo = xdoc.Element("ArrayOfTodoModel")?
            .Elements("TodoModel")
            .FirstOrDefault(p => p.Attribute("id")?.Value == MainPage.Title.ToString());

        if (todo != null)
        {
            todo.Remove();

            xdoc.Save(MainPage.Path);

            MainPage.Reload();
        }
    }
}