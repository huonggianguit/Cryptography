using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Finance_Pro.Models;
using MySql.Data.MySqlClient;

namespace Finance_Pro.Data
{
    public static class DataProgram
    {

        public static void LoadCategoriesFromDatabase(ObservableCollection<CategoryExpenses> CollecData)

        {
            Debug.WriteLine("VÔ HÀM NÀY LẤY DATA 1");
            try
            {
                // Sử dụng ConnectSQL để kết nối cơ sở dữ liệu
                using (var connection = new ConnectSQL())  // Tạo kết nối và tự động đóng khi thoát khỏi using
                {
                    // Lấy danh sách CategoryExpenses
                    string categoryQuery = "SELECT * FROM category_expenses";
                    var categoriesData = connection.ExecuteQuery(categoryQuery);
                    Debug.WriteLine("VÔ HÀM NÀY LẤY DATA");
                    // Tạo danh sách CategoryExpenses từ kết quả
                    foreach (var row in categoriesData)
                    {
                        var category = new CategoryExpenses
                        {
                            id = Convert.ToInt32(row["id"]),
                            Name = Convert.ToString(row["name"]) ?? string.Empty,

                            CategoryExpensesProp = new ObservableCollection<SubCategoryExpenses>()
                        };

                        // Kiểm tra xem có dữ liệu ảnh không (từ cột "image")
                        if (row["image"] != DBNull.Value)
                        {
                            byte[] imageBytes = (byte[])row["image"];
                            using (var ms = new MemoryStream(imageBytes))
                            {
                                BitmapImage bitmap = new BitmapImage();
                                bitmap.BeginInit();
                                bitmap.StreamSource = ms;
                                bitmap.CacheOption = BitmapCacheOption.OnLoad;  // Chế độ lưu ảnh sau khi tải
                                bitmap.EndInit();

                                // Gán hình ảnh vào thuộc tính Image của CategoryExpenses
                                category.Image = bitmap;
                            }
                        }

                        CollecData.Add(category);
                    }

                    // Duyệt qua từng Category để lấy SubCategoryExpenses
                    foreach (var category in CollecData)
                    {
                        string subCategoryQuery = $"SELECT * FROM sub_category_expenses WHERE id_category_expenses = {category.id}";

                        var subCategoriesData = connection.ExecuteQuery(subCategoryQuery);
                        foreach (var row in subCategoriesData)
                        {
                            var subCategory = new SubCategoryExpenses
                            {
                                id = Convert.ToInt32(row["id"]),
                                idCategoryExpenses = Convert.ToInt32(row["id_category_expenses"]),
                                Name = row["name"].ToString(),
                                is_Details = row["is_details"].ToString(),
                            };

                            if (row["image"] != DBNull.Value)
                            {
                                byte[] imageBytes = (byte[])row["image"];
                                using (var ms = new MemoryStream(imageBytes))
                                {
                                    BitmapImage bitmap = new BitmapImage();
                                    bitmap.BeginInit();
                                    bitmap.StreamSource = ms;
                                    bitmap.CacheOption = BitmapCacheOption.OnLoad;  // Chế độ lưu ảnh sau khi tải
                                    bitmap.EndInit();

                                    // Gán hình ảnh vào thuộc tính Image của CategoryExpenses
                                    subCategory.Image = bitmap;
                                }
                            }

                            // Kiểm tra giá trị của is_Details
                            if (subCategory.is_Details == "1") // Nếu is_Details bằng "1"
                            {
                                subCategory.SubSubCategoriesProp = new ObservableCollection<DetailsCategoryExpenses>();
                            }
                            category.CategoryExpensesProp.Add(subCategory);

                        }

                    }

                    // Duyệt qua từng SubCategory để lấy DetailsCategoryExpenses nếu is_Details = 1
                    foreach (var category in CollecData)
                    {
                        foreach (var subCategory in category.CategoryExpensesProp)
                        {
                            if (subCategory.is_Details == "1")
                            {
                                string detailsQuery = $"SELECT * FROM details_category_expenses WHERE id_sub_category_expenses = {subCategory.id}";
                                var detailsData = connection.ExecuteQuery(detailsQuery);
                                foreach (var row in detailsData)
                                {
                                    var detail = new DetailsCategoryExpenses
                                    {
                                        id = Convert.ToInt32(row["id"]),
                                        idSubCategoryExpenses = Convert.ToInt32(row["id_sub_category_expenses"]),
                                        Name = row["name"].ToString()
                                    };

                                    if (row["image"] != DBNull.Value)
                                    {
                                        byte[] imageBytes = (byte[])row["image"];
                                        using (var ms = new MemoryStream(imageBytes))
                                        {
                                            BitmapImage bitmap = new BitmapImage();
                                            bitmap.BeginInit();
                                            bitmap.StreamSource = ms;
                                            bitmap.CacheOption = BitmapCacheOption.OnLoad;  // Chế độ lưu ảnh sau khi tải
                                            bitmap.EndInit();

                                            // Gán hình ảnh vào thuộc tính Image của CategoryExpenses
                                            detail.Image = bitmap;
                                        }
                                    }

                                    subCategory.SubSubCategoriesProp.Add(detail);
                                }
                            }

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tải dữ liệu từ SQL: {ex.Message}");
            }
        }


    }
}


