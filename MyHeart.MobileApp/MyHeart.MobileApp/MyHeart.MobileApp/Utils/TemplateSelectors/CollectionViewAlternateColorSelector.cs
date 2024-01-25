using MyHeart.DTO.ProfessionalInformation;
using MyHeart.MobileApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;

namespace MyHeart.MobileApp.Utils.TemplateSelectors
{
    class CollectionViewAlternateColorSelector : DataTemplateSelector
    {
        public DataTemplate EvenTemplate { get; set; }
        public DataTemplate UnevenTemplate { get; set; }
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var itemSource = BindableLayout.GetItemsSource(container);
            var items = itemSource as ObservableCollection<TherapyNewsViewModel>;
            int index = items.IndexOf(item as TherapyNewsViewModel);

            return index % 2 == 0 ? EvenTemplate : UnevenTemplate;
        }
    }
}
