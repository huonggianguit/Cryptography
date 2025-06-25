using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Finance_Pro.Models
{
    public class CategoryExpenses
    {
        public int id { get; set; } // Danh sách phân chia chi tiêu
        public string Name { get; set; } // Danh sách phân chia chi tiêu
        public BitmapImage Image { get; set; }
        public ObservableCollection<SubCategoryExpenses> CategoryExpensesProp { get; set; } // Phân loại 
    }

    public class SubCategoryExpenses
    {
        public int id { get; set; } // Danh sách phân chia chi tiêu
        public int idCategoryExpenses { get; set; } // Danh sách phân chia chi tiêu

        public string Name { get; set; } // Tên loại phụ (có thể là phụ 1, phụ 2,...)
        public BitmapImage Image { get; set; }
        public string is_Details { get; set; } // Tên loại phụ (có thể là phụ 1, phụ 2,...)

        public ObservableCollection<DetailsCategoryExpenses> SubSubCategoriesProp { get; set; } // Danh sách loại phụ nữa (nếu có)
    }

    public class DetailsCategoryExpenses
    {
        public int id { get; set; } // Danh sách phân chia chi tiêu

        public int idSubCategoryExpenses { get; set; } // Danh sách phân chia chi tiêu

        public string Name { get; set; } // Chi tiết
        public BitmapImage Image { get; set; }
    }
}
