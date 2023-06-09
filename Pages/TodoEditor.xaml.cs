using System.Xml.Linq;

namespace App.Pages;

public partial class TodoEditor : ContentPage
{
	public TodoEditor() => InitializeComponent();

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        XDocument xdoc = XDocument.Load(MainPage.Path);
        // �������� �������� ����
        XElement? arrayTodoModel = xdoc.Element("ArrayOfTodoModel");
        if (arrayTodoModel is not null)
        {
            // �������� �� ���� ��������� person
            foreach (XElement person in arrayTodoModel.Elements("TodoModel"))
            {

                XAttribute? id = person.Attribute("id");
                XElement? content = person.Element("Content");
                XElement? data = person.Element("Data");

                if (id?.Value == MainPage.Title.ToString())
                {
                    editor.Text = content?.Value;
                }
            }
        }
    }
    private void ContentPage_NavigatedFrom(object sender, NavigatedFromEventArgs e)
    {
        XDocument xdoc = XDocument.Load(MainPage.Path);

        // ������� ������� person � id = "Tom"
        var todo = xdoc.Element("ArrayOfTodoModel")?
            .Elements("TodoModel")
            .FirstOrDefault(p => p.Attribute("id")?.Value == MainPage.Title.ToString());

        if (todo != null)
        {
            //  ������ ��������� ������� content
            var content = todo.Element("Content");
            if (content != null) content.Value = editor.Text;

            xdoc.Save(MainPage.Path);
            MainPage.ReSelected();
        }
    }
}