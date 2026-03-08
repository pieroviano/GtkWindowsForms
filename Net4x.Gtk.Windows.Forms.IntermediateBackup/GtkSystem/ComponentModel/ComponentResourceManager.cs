using System.Globalization;

namespace GtkSystem.ComponentModel
{
    /// <summary>
    /// This is System.ComponentModel.ComponentResourceManager image retrieval implementation
    /// Usage method： at form create new under the projectSystem.ComponentModel.ComponentResourceManager and inheritGtkSystem.ComponentModel.ComponentResourceManager that is enough
    /// </summary>
    public class ComponentResourceManager : System.ComponentModel.ComponentResourceManager
    {

        private Type formtype;

        private string formName;
        
        public ComponentResourceManager(Type form) : base(form)
        {
            formtype = form;
            formName = form.Name;
		}
        public override object GetObject(string name, CultureInfo culture)
        {
            return GetObject(name);
        }

        public override object GetObject(string name)
        {
            GtkSystem.Resources.ResourceManager temp = new GtkSystem.Resources.ResourceManager(formtype.FullName, formtype.Assembly);
            return temp.GetObject(name);
        }

        public override string GetString(string name)
        {
            GtkSystem.Resources.ResourceManager temp = new GtkSystem.Resources.ResourceManager(formtype.FullName, formtype.Assembly);
            return temp.GetString(name);
        }

    }

}