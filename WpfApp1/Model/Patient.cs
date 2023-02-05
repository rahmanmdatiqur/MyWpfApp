using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WpfApp1.Model
{
    public  class Patient
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string  FathersName { get; set; }
        public string  Gender { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public string DateTime { get; set; }
        public ImageSource ImageSrc { get; set; }
        public string ImageTitle { get; set; }



    }
}
