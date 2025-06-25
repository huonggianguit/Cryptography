using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finance_Pro.Data;
using System.Windows.Controls;
using Finance_Pro.Models;
using System.Diagnostics;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using System.Windows.Media.Imaging;

namespace Finance_Pro.ViewModels
{

    public class ListExpensesVM: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ObservableCollection<CategoryExpenses> _collecData;

        public ObservableCollection<CategoryExpenses> CollecData
        {
            get => _collecData;
            set
            {
                _collecData = value;
                OnPropertyChanged(nameof(CollecData));
            }
        }

        private ObservableCollection<SubCategoryExpenses> _CategoryExpensesProp;

        public ObservableCollection<SubCategoryExpenses> CategoryExpensesProp
        {
            get => _CategoryExpensesProp;
            set
            {
                _CategoryExpensesProp = value;
                OnPropertyChanged(nameof(CategoryExpensesProp));
            }
        }

        private string _selectedCategoryName;
        public string SelectedCategoryName
        {
            get => _selectedCategoryName;
            set
            {
                _selectedCategoryName = value;
                OnPropertyChanged(nameof(SelectedCategoryName));
            }
        }
        private BitmapImage _selectedCategoryImage;
        public BitmapImage SelectedCategoryImage
        {
            get => _selectedCategoryImage;
            set
            {
                _selectedCategoryImage = value;
                OnPropertyChanged(nameof(SelectedCategoryImage));
            }
        }
        private bool _checkSelect1;
        public bool CheckSelect1
        {
            get => _checkSelect1;
            set
            {
                _checkSelect1 = value;
                OnPropertyChanged(nameof(CheckSelect1));
            }
        }
        private bool _isChecked;

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    OnPropertyChanged(nameof(IsChecked));
                }
            }
        }


        public ICommand ChooseCategory { get; private set; }

        public ListExpensesVM()
        {
            Debug.WriteLine("Vô hàm load");
            // Khởi tạo ObservableCollection
            CollecData = new ObservableCollection<CategoryExpenses>();
            // Gọi hàm LoadCategoriesFromDatabase để tải dữ liệu
            DataProgram.LoadCategoriesFromDatabase(CollecData);
            ChooseCategory = new RelayCommand <object>(chooseCategory);
          
        }

        public void chooseCategory(object parameter)
        {
            CategoryExpensesProp = new ObservableCollection<SubCategoryExpenses>();

            // `parameter` là `DataContext` của `Button`.
            if (parameter is CategoryExpenses category)
            {
                // Lấy giá trị từ `Image` và lưu lại.
                SelectedCategoryImage = category.Image;
                SelectedCategoryName = category.Name;
                IsChecked = false;

                if (category.CategoryExpensesProp != null)
                {
                    foreach (var subCategory in category.CategoryExpensesProp)
                    {
                        CategoryExpensesProp.Add(subCategory);
                    }
                }
            }
            }
    }

}
