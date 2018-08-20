using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarCategoryConsoleApp
{
    class CategoryListModel
    {
        public List<ValueCategoryModel> Value { get; set; }
    }
    class ValueCategoryModel
    {
        public string id { get; set; }
        public string displayName { get; set; }
        public string color { get; set; }
    }
}
