using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TestSystem.Database;

namespace TestSystem
{
    /// <summary>
    /// Логика взаимодействия для TestingFormView.xaml
    /// </summary>
    public partial class TestingFormView : UserControl
    {
        public TestingFormView()
        {
            InitializeComponent();
        }
    }

    public class QuestionTypeSelector : DataTemplateSelector
    {
        public DataTemplate OneAnswerTemplate {  get; set; }
        public DataTemplate ManyAnswersTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null) return null;

            if (item is not Question q) return null;

            if (q.AnswerType == (int)StaticData.AnswerType.Single)
            {
                return OneAnswerTemplate;
            }

            return ManyAnswersTemplate;
        }
    }
}
