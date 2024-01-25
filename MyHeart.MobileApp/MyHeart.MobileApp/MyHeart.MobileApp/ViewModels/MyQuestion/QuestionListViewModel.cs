using MyHeart.DTO.Questions;
using MyHeart.DTO.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.MobileApp.ViewModels
{
    public class QuestionListViewModel : BaseViewModel
    {
        private QuestionStatus status;
        private string subject;
        private string body;

        public string Subject 
        { 
            get => subject;
            set => SetProperty(ref subject, value);
        }
        public string Body 
        {
            get => body;
            set => SetProperty(ref body, value);
        }
        public QuestionStatus Status 
        { 
            get => status; 
            set => SetProperty(ref status, value); 
        }
        public string CreationDate { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
        public QuestionListViewModel(QuestionListDTO dto)
        {
            Id = dto.Id;
            Subject = dto.Subject;
            Body = dto.Body;
            Status = dto.Status;
            CreationDate = dto.CreationDate;
            UserId = dto.UserId;
            Name = dto.Name;
        }
        public QuestionListViewModel()
        {
        }
    }
}
