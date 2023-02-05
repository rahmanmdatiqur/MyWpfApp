using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfApp1.Model;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public FileInfo TempImageFile { get; set; }
        public BitmapImage DefaultImage => new BitmapImage(new Uri(GetImagePath() + "default.jpg"));

        public MainWindow()
        {
            InitializeComponent();
            List<string> Gender = new List<string>()
            {
                "Male",
                "Female"
            };
            this.cmbGender.ItemsSource = Gender;
            cmbGender.Text = "Male";

            var path = Path.GetDirectoryName(GetImagePath());
            if (!File.Exists(@"patient.json"))
            {
                File.CreateText(@"patient.json").Close();
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            formPic.Source = DefaultImage;
        }

        private void BtnInsert_Click(object sender, RoutedEventArgs e)
        {
            Patient pt = new Patient();
            pt.ID = Convert.ToInt32(txtID.Text);
            pt.Name = txtName.Text;
            pt.FathersName = txtFathersName.Text;
            pt.Gender = cmbGender.SelectedItem.ToString();
            pt.Age = Convert.ToInt32(txtAge.Text);
            pt.Email = txtEmail.Text;
            pt.ContactNo = txtContactNo.Text;
            pt.DateTime = txtDateTime.Text;
            pt.ImageTitle = (TempImageFile != null) ? $"{int.Parse(txtID.Text) + TempImageFile.Extension}" : "default.jpg";

            var newPatientMember = "{'Id':'" + pt.ID + "','Name':'" +pt.Name + "'," +
                "'FathersName':'" + pt.FathersName + "','Gender':'" + pt.Gender + "'," +
                "'Age':'" + pt.Age + "','Email':'" + pt.Email + "','DateTime':'" + pt.DateTime + "','ImageTitle':'" + pt.ImageTitle + "','ImageSrc':'" + pt.ImageSrc + "','ContactNo':'" + pt.ContactNo + "'}";

            var json = File.ReadAllText(@"patient.json");
            var jsonObj = JObject.Parse(json);
            JArray patientArray = jsonObj.GetValue("Patient") as JArray;

            var newPatient = JObject.Parse(newPatientMember);
            patientArray.Add(newPatient);

            jsonObj["Patient"] = patientArray;
            string newJsonResult = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);

            if (TempImageFile != null)
            {
                TempImageFile.CopyTo(GetImagePath() + pt.ImageTitle);
                TempImageFile = null;
            }

            File.WriteAllText(@"patient.json", newJsonResult);
            MessageBox.Show("Data Saved Successfully!!!", "Saved", MessageBoxButton.OK, MessageBoxImage.Warning);

            ShowData();
            AllClear();
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {

            btnUpdate.Visibility = Visibility.Visible;

            Button b = sender as Button;
            Patient ptbtn = b.CommandParameter as Patient;
            int ptId =ptbtn.ID;

            txtID.Text = ptId.ToString();
            txtName.Text = ptbtn.Name.ToString();
            txtFathersName.Text = ptbtn.FathersName.ToString();
            txtAge.Text = ptbtn.Age.ToString();
            txtEmail.Text = ptbtn.Email.ToString();
            txtContactNo.Text = ptbtn.ContactNo.ToString();
            txtDateTime.Text = ptbtn.DateTime.ToString();
            cmbGender.SelectedItem = ptbtn.Gender.ToString();
            txtID.IsEnabled = true;
            formPic.Source = ImageInstance(new Uri(GetImagePath() + ptbtn.ImageTitle));
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {

            string jsonD = File.ReadAllText(@"patient.json");
            var jsonObj = JObject.Parse(jsonD);
            JArray ptDeleteArray = (JArray)jsonObj["Patient"];

            Button b = sender as Button;
           Patient ptbtn = b.CommandParameter as Patient;
            int ptId = ptbtn.ID;

            if (ptId > 0)
            {
                var patientToDeleted =ptDeleteArray.FirstOrDefault(obj => obj["Id"].Value<int>() == ptId);
               ptDeleteArray.Remove(patientToDeleted);

                string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                File.WriteAllText(@"patient.json", output);
                FileInfo thisFile = new FileInfo(GetImagePath() + ptbtn.ImageTitle);
                if (thisFile.Name != "default.jpg") //Delete image (Not default image)
                {
                    thisFile.Delete();
                }
                MessageBox.Show("Data Deleted Successfully!!!", "Delete", MessageBoxButton.OK, MessageBoxImage.Warning);
                ShowData();
                AllClear();
            }
            else
            {
                MessageBox.Show("Not Deleted!!!!");
            }
        }
        private void btnShowAll_Click(object sender, RoutedEventArgs e)
        {
            ShowData();
        }
        private bool IsValidJson(string data)   //check whether file contains json format or not
        {

            try
            {
                var temp = JObject.Parse(data);  //Try to parse json data if can't will throw exception
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        private void ShowData()
        {
            var json = File.ReadAllText(@"patient.json");

            if (!IsValidJson(json))
            {
                return;
            }

            var jsonObj = JObject.Parse(json);
            var empJson = jsonObj.GetValue("Patient").ToString();
            var empList = JsonConvert.DeserializeObject<List<Patient>>(empJson);   //Deserialize to List<Employee>
            empList = empList.OrderBy(x => x.ID).ToList();  //Sorting List<Employee> by Id (Ascending)

            foreach (var item in empList)
            {
                item.ImageSrc = ImageInstance(new Uri(GetImagePath() + item.ImageTitle));   //Create image instance for all Employee
            }
            lstPatient.ItemsSource = empList;
            lstPatient.Items.Refresh();

            GC.Collect();                   //Call garbage collector to release unused image instance resource
            GC.WaitForPendingFinalizers();









            //var json = File.ReadAllText(@"patient.json");
            //var jsonObj = JObject.Parse(json);
            //if (jsonObj != null)
            //{
            //    JArray ptArray = (JArray)jsonObj["Patient"];
            //    List<Patient> pt = new List<Patient>();
            //    foreach (var item in ptArray)
            //    {
            //        pt.Add(new Patient()
            //        {
            //            ID = Convert.ToInt32(item["Id"]),
            //            Gender = item["Gender"].ToString(),
            //            Name = item["Name"].ToString(),
            //            FathersName = item["FathersName"].ToString(),
            //            Age = Convert.ToInt32(item["Age"]),
            //            Email = item["Email"].ToString(),
            //            ContactNo = item["Contact"].ToString(),
            //            DateTime=item["DateTime"].ToString()
             
            //        }); 
            //        lstPatient.ItemsSource = pt;
            //    }
            //    lstPatient.Items.Refresh();
            //}
        }
        public ImageSource ImageInstance(Uri path)  //Create image instance rather than referencing image file
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = path;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bitmap.DecodePixelWidth = 300;
            bitmap.EndInit();
            return bitmap;
        }
        private void AllClear()
        {
            txtID.Clear();
            txtName.Clear();
            txtFathersName.Clear();
            txtEmail.Clear();
            txtAge.Clear();
            txtContactNo.Clear();
            cmbGender.SelectedIndex = -1;
            txtID.IsEnabled = true;
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            var jsonU = File.ReadAllText(@"Patient.json");
            var jsonObj = JObject.Parse(jsonU);
            JArray ptUpdateArray = (JArray)jsonObj["Patient"];

            var Id = Convert.ToInt32(txtID.Text);
            var Gender = cmbGender.SelectedItem.ToString();
            var Name = txtName.Text;
            var FathersName = txtFathersName.Text;
            var Age = txtAge.Text;
            var Email = txtEmail.Text;
            var Contact = txtContactNo.Text;
            var DateTime = txtDateTime.Text;
            FileInfo oldImageFile = null;

            foreach (var patient in ptUpdateArray.Where(obj => obj["Id"].Value<int>() == Id))
            {
                patient["Gender"] = !string.IsNullOrEmpty(Gender) ? Gender : "";
                patient["Name"] = !string.IsNullOrEmpty(Name) ? Name : "";
                patient["FathersName"] = !string.IsNullOrEmpty(FathersName) ? FathersName : "";
                patient["Age"] = !string.IsNullOrEmpty(Age) ? Age : "";
                patient["Email"] = !string.IsNullOrEmpty(Email) ? Email : "";
                patient["Contact"] = !string.IsNullOrEmpty(Contact) ? Contact : "";
                patient["DateTime"] = !string.IsNullOrEmpty(DateTime) ? DateTime : "";
                oldImageFile = patient["ImageTitle"].ToString() != "default.jpg" ? new FileInfo(GetImagePath() + patient["ImageTitle"]) : null;

                if (TempImageFile != null && oldImageFile == null)
                {
                    TempImageFile.CopyTo(GetImagePath() + patient["Id"].ToString()+TempImageFile.Extension);
                    TempImageFile = null;
                }
                else if (TempImageFile != null && oldImageFile != null)
                {
                    oldImageFile.Delete();
                    TempImageFile.CopyTo(GetImagePath() + patient["Id"].ToString() + TempImageFile.Extension);
                    TempImageFile = null;
                }
            }
            jsonObj["Patient"] =ptUpdateArray;
            string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);;

            File.WriteAllText(@"patient.json", output);
            MessageBox.Show("Data Updated Successfully!!!!");
            ShowData();
            AllClear();
        }

        private void View_Click(object sender, RoutedEventArgs e)
        {
            ViewWindow view = new ViewWindow();
            view.Show();
            this.Close();
            Button b = sender as Button;
            Patient ptbtn = b.CommandParameter as Patient;
            view.txtView.Text = $"  ID:\t\t{ ptbtn.ID}\n Name:\t\t{ptbtn.Name}\n" +
                $" FathersName:\t {ptbtn.FathersName}\n Gender:\t\t {ptbtn.Gender}\n Age: \t\t {ptbtn.Age} \n " +
                $"ContactNo:\t {ptbtn.ContactNo}\n Email: \t\t{ptbtn.Email}\n DateTime:\t {ptbtn.DateTime}";
            view.imageBox.Source = new BitmapImage(new Uri(GetImagePath() + ptbtn.ImageTitle));

        }

        private void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "Image Files(*.jpg; *.jpeg; *.png)|*.jpg; *.jpeg; *.png;";
            fd.Title = "Select an Image";
            if (fd.ShowDialog().Value == true)
            {
                formPic.Source = new BitmapImage(new Uri(fd.FileName));
                TempImageFile = new FileInfo(fd.FileName);
            }
        }

        private void btnUpdate_Click_1(object sender, RoutedEventArgs e)
        {

        }
        public string GetImagePath()    //Get the Image Directory Path Where Image is stored
        {
            var currentAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            string assemblyDirectory = Path.GetDirectoryName(currentAssembly.Location);             // debug folder
            string ImagePath = Path.GetFullPath(Path.Combine(assemblyDirectory, @"..\..\Image\"));    // ..\..\ Navigate two levels up => Project folder

            return ImagePath;
        }

        private void txtContactNo_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
