using MyHeart.DTO.Questions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.MobileApp.ViewModels
{
    public class QuestionCommentViewModel : BaseViewModel
    {
        private string message;
        private string user;

        public int Id { get; set; }
        public string User
        {
            get => user;
            set => SetProperty(ref user, value);
        }
        public string Message 
        { 
            get => message; 
            set => SetProperty(ref message, value); 
        }
        public bool IsMine { get; set; }
        public DateTime CreatedAt { get; set; }
        public QuestionCommentViewModel(QuestionCommentDTO dto, int userId)
        {
            Id = dto.Id;
            CreatedAt = dto.CreationDate;
            Message = dto.Text;
            User = dto.LastUpdateUser;
            IsMine = dto.SenderId == userId;
        }
        public QuestionCommentViewModel()
        {

        }
    }
}
