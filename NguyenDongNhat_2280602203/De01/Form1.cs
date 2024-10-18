using De01.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace De01
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void LoadLopData()
        {
            using (var context = new StudentContextDB())
            {
                var lopList = context.Lops.ToList();


                cboLop.DataSource = lopList;
                cboLop.DisplayMember = "TenLop";  
                cboLop.ValueMember = "MaLop";     
            }
        }

        private void LoadSinhvienData()
        {
            using (var context = new StudentContextDB())
            {
                var sinhvienList = context.Sinhviens
                                    .Select(sv => new
                                    {
                                        MaSV = sv.MaSV,
                                        HoTenSV = sv.HotenSV,
                                        NgaySinh = sv.Ngaysinh,
                                        TenLop = sv.Lop.TenLop 
                                    })
                                    .ToList();

                dgvSinhvien.DataSource = sinhvienList;
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            LoadLopData();     
            LoadSinhvienData();
            btThem.Enabled = true;
            btXoa.Enabled = false;
            btSua.Enabled = false;
            btLuu.Enabled = false;
            btKhongLuu.Enabled = false;

        }
       

        private void btThem_Click(object sender, EventArgs e)
        {
            using (var context = new StudentContextDB())
            {
                var newSinhvien = new Sinhvien
                {
                    MaSV = txtMaSV.Text,
                    HotenSV = txtHotenSV.Text,
                    Ngaysinh = dtNgaysinh.Value,
                    MaLop = cboLop.SelectedValue.ToString()
                };

                context.Sinhviens.Add(newSinhvien);
                context.SaveChanges();

                // Load lại danh sách sinh viên
                LoadSinhvienData();
            }
        }

        private void cboLop_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboLop.SelectedValue != null)  // Kiểm tra nếu ComboBox đã có giá trị được chọn
            {
                string selectedMaLop = cboLop.SelectedValue.ToString();  // Lấy mã lớp đã chọn

                using (var context = new StudentContextDB())
                {
                    // Truy vấn danh sách sinh viên thuộc lớp đã chọn
                    var sinhvienList = context.Sinhviens
    .Where(sv => sv.MaLop == selectedMaLop)  // Filter by class code
    .AsEnumerable()  // Switch to LINQ to Objects
    .Select(sv => new
    {
        MaSV = sv.MaSV,
        HoTenSV = sv.HotenSV,
        NgaySinh = sv.Ngaysinh,
        TenLop = sv.Lop.TenLop
    })
    .ToList();



                    // Đổ dữ liệu vào DataGridView
                    dgvSinhvien.DataSource = sinhvienList;
                }
            }
        }



        private void btLuu_Click(object sender, EventArgs e)
        {
           
        }

        private void btSua_Click(object sender, EventArgs e)
        {
            using (var context = new StudentContextDB())
            {
                // Tìm sinh viên theo MaSV
                var selectedMaSV = txtMaSV.Text;
                var sinhvien = context.Sinhviens.FirstOrDefault(sv => sv.MaSV == selectedMaSV);

                if (sinhvien != null)
                {
                    // Cập nhật thông tin sinh viên
                    sinhvien.HotenSV = txtHotenSV.Text;
                    sinhvien.Ngaysinh = dtNgaysinh.Value;
                    sinhvien.MaLop = cboLop.SelectedValue.ToString();

                    context.SaveChanges();

                    // Load lại danh sách sinh viên
                    LoadSinhvienData();
                }
            }
        }

        private void dgvSinhvien_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dgvSinhvien.Rows[e.RowIndex];
                txtMaSV.Text = selectedRow.Cells["MaSV"].Value.ToString();
                txtHotenSV.Text = selectedRow.Cells["HotenSV"].Value.ToString();
                dtNgaysinh.Value = Convert.ToDateTime(selectedRow.Cells["NgaySinh"].Value);
                cboLop.Text = selectedRow.Cells["TenLop"].Value.ToString();

                // Cập nhật trạng thái các Button
                btThem.Enabled = false;
                btXoa.Enabled = true;
                btSua.Enabled = true;
                btLuu.Enabled = false;
                btKhongLuu.Enabled = true;
            }
        }

        private void btXoa_Click(object sender, EventArgs e)
        {
            using (var context = new StudentContextDB())
            {
                // Tìm sinh viên theo MaSV
                var selectedMaSV = txtMaSV.Text;
                var sinhvien = context.Sinhviens.FirstOrDefault(sv => sv.MaSV == selectedMaSV);

                if (sinhvien != null)
                {
                    // Xóa sinh viên
                    context.Sinhviens.Remove(sinhvien);
                    context.SaveChanges();

                    // Load lại danh sách sinh viên
                    LoadSinhvienData();
                }
            }
        }

        private void btKhongLuu_Click(object sender, EventArgs e)
        {
            // Xóa nội dung trong các control
            txtMaSV.Clear();
            txtHotenSV.Clear();
            dtNgaysinh.Value = DateTime.Now;
            cboLop.SelectedIndex = -1;

            // Load lại danh sách sinh viên
            LoadSinhvienData();
        }

        private void btThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
