using MyHeart.DTO.User;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace MyHeart.MobileApp.ViewModels
{
    public class ReportFileViewModel : BaseViewModel
    {
        private bool _isSelected;

        public byte[] Content { get; set; }
        public string Extension { get; set; }
        public string Name { get; set; }
        public Command ShareFileCommand { get; }
        public ImageSource ImgSource
        {
            get => ImageSource.FromStream(() => new MemoryStream(Content ?? new byte[0] )); 
        }

        public ReportFileViewModel()
        {
            ShareFileCommand = new Command(() => {

            });
        }
        public ReportFileViewModel(UserReportFileDTO dto)
        {
            Content = dto.Content;
            Extension = dto.Extension;
            Name = dto.Name;
        }
        public ReportFileViewModel(FileInfo fileInfo) : this()
        {
            if (!fileInfo.Exists) throw new FileNotFoundException();

            Extension = fileInfo.Extension;
            Name = fileInfo.Name;
            var content = new StreamReader(fileInfo.OpenRead()).ReadToEnd();
            Content = Encoding.UTF8.GetBytes(content);
        }
        public bool IsPdf() => Extension.ToLower() == "pdf";
        public bool IsImage() => !IsPdf();
        public bool IsSelected 
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}
