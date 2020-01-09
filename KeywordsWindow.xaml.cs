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
using System.Windows.Shapes;

namespace ImageBank
{
    /// <summary>
    /// Interaction logic for KeywordsWindow.xaml
    /// </summary>
    public partial class KeywordsWindow : Window
    {
        public KeywordsWindow()
        {
            InitializeComponent();
        }

        private void ButtonCancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /*
        private static void GetLogicalChildCollection<T>(DependencyObject parent, List<T> logicalCollection) where T : DependencyObject
        {
            IEnumerable children = LogicalTreeHelper.GetChildren(parent);
            foreach (object child in children)
            {
                if (child is DependencyObject)
                {
                    DependencyObject depChild = child as DependencyObject;
                    if (child is T)
                    {
                        logicalCollection.Add(child as T);
                    }

                    GetLogicalChildCollection(depChild, logicalCollection);
                }
            }
        }
        
        List<Button> listButtons = new List<Button>();
        GetLogicalChildCollection(this, listButtons);         
         
         
         */
    }
}
