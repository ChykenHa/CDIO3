using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace OnlineShop
{
    public partial class TaiKhoan : System.Web.UI.Page
    {
        // Connection string từ web.config
        string connect = @"Data Source=DARLING\SQLEXPRESS;Initial Catalog=OnlineShopDB;Integrated Security=True";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Tải thông tin người dùng
                LoadUserInfo();

                // Kiểm tra tab được yêu cầu
                string tab = Request.QueryString["tab"];
                if (!string.IsNullOrEmpty(tab))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ActivateTab",
                        "activateTab('" + tab + "');", true);
                }
            }
        }

        private void LoadUserInfo()
        {
            int userId = Convert.ToInt32(Session["id_khachhang"]);

            using (SqlConnection conn = new SqlConnection(connect))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM KhachHang WHERE id_khachhang = @UserId", conn);
                cmd.Parameters.AddWithValue("@UserId", userId);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        // Hiển thị thông tin người dùng
                        UserName.Text = reader["tendangnhap"].ToString();
                        txtFullName.Text = reader["hoten"].ToString();
                        txtEmail.Text = reader["email"].ToString();
                        txtPhone.Text = reader["dienthoai"].ToString();

                        // Xử lý ngày sinh (nếu có)
                        if (reader["ngaysinh"] != DBNull.Value)
                        {
                            DateTime birthDate = Convert.ToDateTime(reader["ngaysinh"]);
                            txtBirthdate.Text = birthDate.ToString("yyyy-MM-dd");
                        }

                        // Xử lý giới tính (nếu có - giả sử có trường gioitinh)
                        if (reader["gioitinh"] != DBNull.Value)
                        {
                            string gender = reader["gioitinh"].ToString().ToLower();
                            if (gender == "nam")
                                ddlGender.SelectedValue = "male";
                            else if (gender == "nữ" || gender == "nu")
                                ddlGender.SelectedValue = "female";
                            else
                                ddlGender.SelectedValue = "other";
                        }

                        // Tải ảnh đại diện nếu có
                        if (reader["avatar"] != DBNull.Value && !string.IsNullOrEmpty(reader["avatar"].ToString()))
                        {
                            UserAvatar.ImageUrl = "~/Img/avatars/" + reader["avatar"].ToString();
                        }
                    }
                    reader.Close();

                    // Tải thông tin đơn hàng
                    LoadOrderInfo(userId);

                    // Tải thông tin địa chỉ
                    LoadAddressInfo(userId);
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi
                    ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                        "alert('Lỗi khi tải thông tin: " + ex.Message + "');", true);
                }
            }
        }

        private void LoadOrderInfo(int userId)
        {
            // Tải thông tin đơn hàng (giả sử có bảng DonHang)
            // Code này sẽ cần điều chỉnh tùy thuộc vào cấu trúc bảng DonHang của bạn
            // ...
        }

        private void LoadAddressInfo(int userId)
        {
            // Tải thông tin địa chỉ (giả sử có bảng DiaChi hoặc sử dụng trường diachi trong KhachHang)
            // ...
        }

        protected void BtnInfo_Click(object sender, EventArgs e)
        {
            // Chuyển đến tab thông tin
            ClientScript.RegisterStartupScript(this.GetType(), "ActivateTab",
                "activateTab('info');", true);
        }

        protected void BtnOrders_Click(object sender, EventArgs e)
        {
            // Chuyển đến tab đơn hàng
            ClientScript.RegisterStartupScript(this.GetType(), "ActivateTab",
                "activateTab('orders');", true);
        }

        protected void BtnAddresses_Click(object sender, EventArgs e)
        {
            // Chuyển đến tab địa chỉ
            ClientScript.RegisterStartupScript(this.GetType(), "ActivateTab",
                "activateTab('addresses');", true);
        }

        protected void BtnSecurity_Click(object sender, EventArgs e)
        {
            // Chuyển đến tab bảo mật
            ClientScript.RegisterStartupScript(this.GetType(), "ActivateTab",
                "activateTab('security');", true);
        }

        protected void txtFullName_TextChanged(object sender, EventArgs e)
        {
            // Xử lý khi thay đổi họ tên
        }

        protected void txtEmail_TextChanged(object sender, EventArgs e)
        {
            // Xử lý khi thay đổi email
        }

        protected void txtPhone_TextChanged(object sender, EventArgs e)
        {
            // Xử lý khi thay đổi số điện thoại
        }

        protected void txtBirthdate_TextChanged(object sender, EventArgs e)
        {
            // Xử lý khi thay đổi ngày sinh
        }

        protected void ddlGender_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Xử lý khi thay đổi giới tính
        }

        protected void SaveInfo_Click(object sender, EventArgs e)
        {
            // Lưu thông tin cá nhân
            int userId = Convert.ToInt32(Session["id_khachhang"]);

            using (SqlConnection conn = new SqlConnection(connect))
            {
                SqlCommand cmd = new SqlCommand("UPDATE KhachHang SET hoten = @FullName, dienthoai = @Phone, diachi = @Address WHERE id_khachhang = @UserId", conn);
                cmd.Parameters.AddWithValue("@FullName", txtFullName.Text.Trim());
                cmd.Parameters.AddWithValue("@Phone", txtPhone.Text.Trim());
                cmd.Parameters.AddWithValue("@Address", ""); // Thêm trường địa chỉ nếu cần
                cmd.Parameters.AddWithValue("@UserId", userId);

                try
                {
                    conn.Open();
                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        // Cập nhật tên hiển thị
                        UserName.Text = txtFullName.Text.Trim();

                        // Hiển thị thông báo thành công
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                            "alert('Cập nhật thông tin thành công!');", true);
                    }
                    else
                    {
                        // Hiển thị thông báo lỗi
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                            "alert('Không thể cập nhật thông tin!');", true);
                    }
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi
                    ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                        "alert('Lỗi khi cập nhật: " + ex.Message + "');", true);
                }
            }
        }

        protected void EditAddress_Click(object sender, EventArgs e)
        {
            // Lấy ID địa chỉ từ CommandArgument
            string addressId = ((Button)sender).CommandArgument;

            // Chuyển đến trang chỉnh sửa địa chỉ với ID tương ứng
            Response.Redirect("ChinhSuaDiaChi.aspx?id=" + addressId);
        }

        protected void RemoveAddress_Click(object sender, EventArgs e)
        {
            // Lấy ID địa chỉ từ CommandArgument
            string addressId = ((Button)sender).CommandArgument;

            // Xóa địa chỉ (cần cài đặt thêm)
            // ...

            // Sau khi xóa, làm mới trang
            Response.Redirect("TaiKhoan.aspx?tab=addresses");
        }

        protected void ViewOrderDetails_Click(object sender, EventArgs e)
        {
            // Lấy ID đơn hàng từ CommandArgument
            string orderId = ((Button)sender).CommandArgument;

            // Chuyển đến trang chi tiết đơn hàng
            Response.Redirect("ChiTietDonHang.aspx?id=" + orderId);
        }

        protected void ChangePassword_Click(object sender, EventArgs e)
        {
            // Chuyển đến trang đổi mật khẩu
            Response.Redirect("DoiMatKhau.aspx");
        }

        protected void Setup2FA_Click(object sender, EventArgs e)
        {
            // Chuyển đến trang thiết lập xác thực hai lớp
            Response.Redirect("XacThucHaiLop.aspx");
        }

        protected void ViewLoginHistory_Click(object sender, EventArgs e)
        {
            // Chuyển đến trang lịch sử đăng nhập
            Response.Redirect("LichSuDangNhap.aspx");
        }

        protected void SetupSecurityAlerts_Click(object sender, EventArgs e)
        {
            // Chuyển đến trang cài đặt thông báo bảo mật
            Response.Redirect("CaiDatThongBao.aspx");
        }
    }
}