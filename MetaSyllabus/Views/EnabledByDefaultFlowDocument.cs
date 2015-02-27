using System.Windows.Documents;

namespace MetaSyllabus.Views
{
    /// <summary>
    /// A flow document that can have working hyperlinks
    /// </summary>
    public class EnabledByDefaultFlowDocument : FlowDocument
    {
        protected override bool IsEnabledCore
        {
            get
            {
                return true;
            }
        }
    }
}
