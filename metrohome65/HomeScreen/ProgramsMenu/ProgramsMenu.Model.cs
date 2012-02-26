using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MetroHome65.HomeScreen.ProgramsMenu
{
    struct FileDescr
    {
        public String Name;
        public String Path;
    }

    // container for programs shortcurs
    class ProgramsList : BindingList<object>
    {
        protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction)
        {
            var items = Items as List<object>;
            if (items != null)
                items.Sort((f1, f2) => String.CompareOrdinal(((FileDescr)f1).Name, ((FileDescr)f2).Name));
        }

        public void SortByName()
        {
            var propDescriptors = TypeDescriptor.GetProperties(typeof(FileDescr));
            ApplySortCore(propDescriptors["Name"], ListSortDirection.Ascending);
        }
    }
}
